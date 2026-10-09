// PreToolUse hook: blocks destructive Bash commands.
// Reads the tool call JSON from stdin; exit code 2 blocks the call and shows stderr to Claude.
let input = '';
process.stdin.on('data', (chunk) => (input += chunk));
process.stdin.on('end', () => {
  let command = '';
  try {
    command = JSON.parse(input).tool_input?.command ?? '';
  } catch {
    process.exit(0);
  }

  const rules = [
    [/\brm\s+(-[a-z]*\s+)*-[a-z]*[rf][a-z]*\b/i, 'rm with -r/-f flags'],
    [/\brm\s+--(recursive|force)\b/i, 'rm with --recursive/--force'],
    [/\bsudo\s+rm\b/i, 'sudo rm'],
    [/\b(rmdir|rd)\s+(\/s|\/q)/i, 'rmdir /s'],
    [/\bdel\s+(\/[a-z]\s+)*\/[sfq]\b/i, 'del with /s /f /q'],
    [/\bRemove-Item\b[^\n|;]*-Recurse/i, 'Remove-Item -Recurse'],
    [/\bgit\s+reset\s+--hard\b/i, 'git reset --hard'],
    [/\bgit\s+clean\b[^\n|;]*\s-[a-z]*f/i, 'git clean -f'],
    [/\bgit\s+push\b[^\n|;]*(\s--force\b|\s-f\b|\s--force-with-lease\b)/i, 'git push --force'],
    [/\bgit\s+(checkout|restore)\s+(--\s+)?\.(\s|$)/i, 'git checkout/restore . (discards changes)'],
    [/\bgit\s+branch\s+-D\b/, 'git branch -D'],
    [/\b(mkfs(\.\w+)?|format\s+[a-z]:|diskpart)\b/i, 'disk formatting'],
    [/\bdd\s+[^\n|;]*\bof=/i, 'dd of='],
    [/\bdrop\s+(database|table|schema)\b/i, 'SQL DROP'],
    [/\btruncate\s+table\b/i, 'SQL TRUNCATE'],
    [/\bdotnet\s+ef\s+database\s+drop\b/i, 'dotnet ef database drop'],
    [/\bterraform\s+destroy\b/i, 'terraform destroy'],
  ];

  for (const [pattern, label] of rules) {
    if (pattern.test(command)) {
      process.stderr.write(
        `Blocked by PreToolUse hook: destructive command detected (${label}). ` +
          `Ask the user to run it themselves if it is really needed.\n`
      );
      process.exit(2);
    }
  }
  process.exit(0);
});
