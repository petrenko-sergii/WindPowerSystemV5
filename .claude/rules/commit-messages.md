# Commit messages

Use semantic commit messages (source: https://gist.github.com/joshbuchea/6f47e86d2510bce28f8e7f42ae84c716).

## Format

```
<type>: <subject>
```

Do not use a scope. Write the subject in the present tense (imperative mood) and start it with a capital letter, e.g. `feat: Add hat wobble`.

## Types

- **feat**: new user-facing functionality
- **fix**: a bug fix affecting users
- **docs**: documentation changes
- **style**: formatting only, no production code change
- **refactor**: restructuring production code (e.g. renaming variables), no behavior change
- **test**: adding or changing tests, no production code change
- **chore**: maintenance tasks, no production code change
- **build**: build system or external dependency changes
- **ci**: CI configuration or script changes
- **perf**: performance improvements
- **revert**: reverts a previous commit

## Examples

```
feat: Add weather-details popup
fix: Handle missing coordinates in weather request
refactor: Remove Effort for Haiku-model
chore: Update CHANGELOG
```
