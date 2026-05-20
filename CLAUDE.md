# CLAUDE.md — AI Assistant Rules for This Mod Project

## Git is Required — Always

Before making **any** file changes, verify git is initialized and the working tree is clean.

```bash
git status
```

If there are uncommitted changes, **stop and ask** before proceeding. Never modify files on top of a dirty working tree.

---

## Workflow Rules

### 1. Branch Before Experimenting
Never make speculative or exploratory changes on `main`. Always create a branch first:

```bash
git checkout -b experiment/description-of-change
```

Use clear branch names:
- `fix/passive-node-crash`
- `feature/new-skill-gems`
- `experiment/rebalance-damage-values`

### 2. Commit Before AND After
- **Before** starting work: confirm the last commit is a clean baseline
- **After** completing a logical change: commit with a descriptive message

```bash
git add .
git commit -m "Short description of what changed and why"
```

### 3. One Thing Per Commit
Do not bundle unrelated changes into a single commit. If fixing a bug and adding a feature, those are two separate commits.

### 4. Describe What AND Why
Good commit messages:
- ✅ `"Increase Fireball base damage by 15% — felt weak at early levels"`
- ✅ `"Fix crash when loading Act 3 map — missing asset reference"`
- ❌ `"changes"`
- ❌ `"update"`

---

## Before Making Any Code/File Change

Run this checklist mentally:

1. Is git initialized? (`git status` should work)
2. Is the working tree clean? (no uncommitted changes)
3. Am I on an appropriate branch? (not `main` for experimental work)
4. Do I understand what file(s) I'm about to change?

If any answer is "no" — pause and resolve it first.

---

## Recovering From Mistakes

If something breaks:

```bash
# Undo all changes since last commit
git restore .

# See history and find a good save point
git log --oneline

# Jump back to a specific commit (read-only look)
git checkout <commit-hash>

# Come back to present
git checkout main
```

---

## Folder Structure Convention

Keep mod projects organized so git diffs stay readable:

```
/my-mod/
  CLAUDE.md          ← this file
  README.md          ← what this mod does
  /src/              ← actual mod files
  /docs/             ← notes, changelogs
  /backups/          ← optional manual backups (git is the real backup)
```

Add a `.gitignore` to exclude junk:

```
*.tmp
*.log
/backups/
```

---

## Summary

> Git is your save system. Always save before letting AI touch your files.
> Branches are cheap — use them freely. Commits are forever — label them well.
