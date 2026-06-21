# 🎯 Task: «Setting Up Git Repository for Team Development»
You are the technical lead of a 5-developer team working on the game "Space Odyssey". 
You need to set up a Git repository for comfortable team collaboration, avoiding typical problems with scene conflicts, prefab conflicts, and large files.

## 📝 What to Do:
### Part 1: Configuring .gitignore and .gitattributes
1. Create a proper `.gitignore` for Unity project:
   - Exclude: `Library/`, `Temp/`, `Logs/`, `UserSettings/`
   - Exclude: `*.csproj`, `*.sln`, `.vs/`
   - Keep: `Assets/`, `Packages/`, `ProjectSettings/`
  
2. Create `.gitattributes` to configure:
   - Smart Merge for `*.unity`, `*.prefab`, `*.asset`
   - Git LFS for images, models, audio, video
  
### Part 2: Configuring Git LFS
3. Install Git LFS and configure tracking:
   - All PNG/JPG/TGA in `Assets/Textures/`
   - All FBX/OBJ in `Assets/Models/`
   - All WAV/MP3 in `Assets/Audio/`
   - All MP4/MOV in `Assets/Videos/`
  
4. Write script `migrate_to_lfs.sh` (bash) or `migrate_to_lfs.bat` (cmd) for automatic migration of existing files to LFS.

### Part 3: Configuring UnityYAMLMerge
5. Configure UnityYAMLMerge for all developers:
   - Write instructions in `README.md`
   - Add path to UnityYAMLMerge depending on Unity version
   - Verify Smart Merge works on a test conflict
  
### Part 4: Branching Strategy
6. Create branching rules:
   - `main` — stable version (only via Pull Request)
   - `develop` — active development
   - `feature/*` — new features
   - `hotfix/*` — urgent fixes
  
7. Write `CONTRIBUTING.md` with:
   - How to create new branches
   - How to make commits (`feat:`, `fix:`, `docs:`)
   - How to resolve conflicts in scenes and prefabs
  
### Part 5: Setting Up Hooks
8. Create pre-commit hook that:
   - Checks that files larger than 50 MB are added via LFS
   - Checks that commit has a description
   - Checks that commit doesn't contain `Library/` or `Temp/`
  
9. Create post-merge hook that:
    - Runs `git lfs pull` after merging
    - Lists changed scenes/prefabs
  
### Part 6: Conflict Testing
10. Create a conflict simulation:
    - Two developers modify the same prefab `Player.prefab`
    - One changes position, the other changes scale
    - Try merging changes with Smart Merge
    - Resolve conflict manually (if Smart Merge fails)
   
11. Create a scene conflict simulation:
    - Two developers modify `Level1.unity`
    - One adds a new GameObject, the other changes lighting
    - Try merging via Smart Merge
   
### Part 7: Documentation
12. Write `README.md` for the repository containing:
    - Cloning instructions (`git clone + git lfs pull`)
    - Git workflow rules
    - How to resolve conflicts
    - Contact for questions
   
---

## 🧰 Implementation Requirements:
- All settings must be documented
- Hooks must be written in bash (for Mac/Linux) and batch (for Windows)
- Verify that `.gitignore` and `.gitattributes` work correctly
- Conflicts must be resolved without data loss
- Repository size must remain < 200 MB (without LFS)

---

## 🔍 Verification:
1. Clone the repository on a new machine
2. Verify that `Library/` is not in the repository
3. Verify that LFS files download via `git lfs pull`
4. Verify that Smart Merge works when merging scenes
5. Verify that pre-commit hook blocks commits with large files without LFS

---

## 💡 Expected Result:
```text
=== Repository Check ===
.gitignore: ✅ Configured (Library, Temp, Logs excluded)
.gitattributes: ✅ Configured (Smart Merge + LFS)
Git LFS: ✅ Installed, all extensions configured
UnityYAMLMerge: ✅ Configured globally
Branches: ✅ main, develop, feature/*, hotfix/*
Hooks: ✅ pre-commit, post-merge working
Prefab conflict test: ✅ Resolved via Smart Merge
Scene conflict test: ✅ Resolved via Smart Merge
Documentation: ✅ README.md, CONTRIBUTING.md written

Repository ready for team development! 🚀
```

---

## 🏆 Bonus Task (Optional):
Set up CI/CD (GitHub Actions / GitLab CI):
- On push to `main`, automatically build the project for Android and iOS
- On push to `develop`, run unit tests
- Send notifications to Discord/Slack about build status

---

### ⭐ If this project was useful, put a star on GitHub!
