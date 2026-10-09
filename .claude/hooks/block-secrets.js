// PreToolUse hook: prevents committing or exposing secrets.json and .env files.
// Reads the tool call JSON from stdin; exit code 2 blocks the call and shows stderr to Claude.
const { spawnSync } = require('child_process');

const SAFE_SUFFIX = /\.(example|sample|template|dist)$/i;

function isSecretFile(p) {
  const name = String(p).split(/[\\/]/).pop().toLowerCase();
  if (!name || SAFE_SUFFIX.test(name)) return false;
  return name === 'secrets.json' || name === '.env' || name.startsWith('.env.');
}

function secretTokens(text) {
  return text
    .split(/[\s"'`=;|&<>(),:@]+/)
    .filter(Boolean)
    .filter(isSecretFile);
}

function git(args, cwd) {
  const r = spawnSync('git', args, { cwd, encoding: 'utf8' });
  return r.status === 0 ? r.stdout.split(/\r?\n/).filter(Boolean) : [];
}

function block(reason) {
  process.stderr.write(
    `Blocked by PreToolUse hook: ${reason}. ` +
      `Secrets (secrets.json, .env files) must not be committed or exposed. ` +
      `Ask the user to handle this themselves if it is really needed.\n`
  );
  process.exit(2);
}

let input = '';
process.stdin.on('data', (chunk) => (input += chunk));
process.stdin.on('end', () => {
  let data;
  try {
    data = JSON.parse(input);
  } catch {
    process.exit(0);
  }

  const tool = data.tool_name;
  const args = data.tool_input ?? {};
  const cwd = data.cwd || process.cwd();

  // File tools: Read / Edit / Write / Grep
  if (tool !== 'Bash') {
    for (const key of ['file_path', 'path', 'glob']) {
      if (typeof args[key] === 'string' && isSecretFile(args[key])) {
        block(`${tool} on a secrets file (${args[key]})`);
      }
    }
    process.exit(0);
  }

  const command = args.command ?? '';

  // Any shell command that names a secrets file (cat, type, Get-Content, git add, curl @file, ...)
  const hits = secretTokens(command);
  if (hits.length) block(`command references a secrets file (${hits[0]})`);

  // git commit: inspect what is actually staged (and tracked changes for -a)
  if (/\bgit\b[^\n|;&]*\bcommit\b/i.test(command)) {
    const files = git(['diff', '--cached', '--name-only'], cwd);
    if (/\s(-[a-z]*a[a-z]*|--all)\b/.test(command)) files.push(...git(['diff', '--name-only'], cwd));
    const staged = files.filter(isSecretFile);
    if (staged.length) block(`git commit would include a secrets file (${staged[0]})`);
  }

  // git add with a broad pathspec (., -A, -u, directories): inspect what would be picked up
  if (/\bgit\b[^\n|;&]*\badd\b[^\n|;&]*(\s(\.|\*|--all|--update|-[a-z]*[Au][a-z]*)(\s|$))/.test(command)) {
    const force = /\s(-f|--force)\b/.test(command);
    const lsArgs = ['ls-files', '--others', '--modified', '--exclude-standard'];
    const files = git(force ? ['ls-files', '--others', '--modified'] : lsArgs, cwd);
    const risky = files.filter(isSecretFile);
    if (risky.length) block(`git add could stage a secrets file (${risky[0]})`);
  }

  process.exit(0);
});
