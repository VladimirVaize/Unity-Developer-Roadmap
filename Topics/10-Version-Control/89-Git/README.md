# 🔀 Git for Unity: .gitignore, Scene Management (Smart Merge), Prefab Conflicts, Git LFS for Large Assets
Unity and Git are a powerful combination but require proper configuration due to binary files, 
complex scene structures, and prefabs. This guide covers all aspects of effective Git usage in Unity projects.

---

## 1. .gitignore for Unity — What NOT to Store in the Repository
Many Unity files are auto-generated or cache. Storing them in Git bloats the repository and creates conflicts.

### 📁 Basic Unity Project Structure:
```text
MyUnityProject/
├── Assets/               # 🔥 EVERYTHING IMPORTANT HERE
│   ├── Scripts/
│   ├── Scenes/
│   ├── Prefabs/
│   ├── Textures/
│   └── ...
├── Packages/             # 🟡 Package manifests (store)
├── ProjectSettings/      # 🟡 Project settings (store)
├── Library/              # ❌ DO NOT STORE (cache, metadata)
├── Temp/                 # ❌ DO NOT STORE
├── Logs/                 # ❌ DO NOT STORE
└── UserSettings/         # ❌ DO NOT STORE (local settings)
```

### 📄 Full .gitignore for Unity:
```gitignore
# ==========================================
# 🔴 Unity generated folders (DO NOT STORE)
# ==========================================
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/
/[Mm]emoryCaptures/

# ==========================================
# 🔴 Unity meta files with specific suffixes
# ==========================================
*.cs.meta
*.unity.meta
*.prefab.meta
*.asset.meta
*.mat.meta
*.anim.meta
*.controller.meta

# ==========================================
# 🔴 Auto-generated files
# ==========================================
*.pidb.meta
*.pdb.meta
*.mdb.meta
*.exe.meta
*.dll.meta

# ==========================================
# 🔴 Plugin folders for specific platforms
# ==========================================
[Ll]ibrary/Plugins/[Aa]ndroid/
[Ll]ibrary/Plugins/[Ii]OS/

# ==========================================
# 🔴 Visual Studio / Rider files
# ==========================================
*.csproj
*.unityproj
*.sln
*.suo
*.user
*.userprefs
*.pidb
*.swp
*.slnf

# ==========================================
# 🔴 Git LFS folder (if used)
# ==========================================
/lfs/

# ==========================================
# 🔴 Logs and cache
# ==========================================
*.log
*.cache
*.tmp
*.temp

# ==========================================
# 🔴 System files
# ==========================================
.DS_Store
Thumbs.db
Desktop.ini

# ==========================================
# 🔴 Rider / VS Code
# ==========================================
.idea/
.vs/
.vscode/
*.code-workspace
```

### 🛠️ How to Apply .gitignore:
1. Create `.gitignore` in the project root
2. Paste the content above
3. Run `git add .gitignore && git commit -m "Add .gitignore"`
4. If files are already in the repo, remove them: `git rm -r --cached Library/`

---

## 2. Scene Management in Git — Smart Merge
Unity scenes are stored in YAML format (text) but with many object GUIDs. This makes them complex for standard merging.

### 🔍 Scene Problem:
```yaml
--- !u!1 &123456789
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 987654321}  # <--- GUID changes when modified
  m_Layer: 0
  m_Name: Player
```

Problem: When modifying a scene, GUIDs change. Standard `git merge` doesn't understand YAML structure and creates conflicts on every changed GUID.

### ✅ Solution: Smart Merge (UnityYAMLMerge)
Unity provides UnityYAMLMerge — a tool that understands YAML structure and performs "smart" merging for scenes, prefabs, and assets.

### ⚙️ Configuring UnityYAMLMerge:
For all projects (globally):
```bash
git config --global merge.unityyamlmerge.name "Unity Smart Merge (YAML)"
git config --global merge.unityyamlmerge.driver 'C:/Program Files/Unity/Hub/Editor/2022.3.21f1/Editor/Data/Tools/UnityYAMLMerge.exe merge --force -p --fallback base "%P" "%O" "%B"'
```

For a specific project (locally):
```bash
echo "*.unity merge=unityyamlmerge" >> .gitattributes
echo "*.prefab merge=unityyamlmerge" >> .gitattributes
echo "*.asset merge=unityyamlmerge" >> .gitattributes
```

### 📄 Full .gitattributes:
```gitattributes
# ==========================================
# 🟢 Unity Smart Merge for YAML files
# ==========================================
*.unity merge=unityyamlmerge
*.prefab merge=unityyamlmerge
*.asset merge=unityyamlmerge
*.mat merge=unityyamlmerge
*.anim merge=unityyamlmerge
*.controller merge=unityyamlmerge

# ==========================================
# 🟢 Git LFS for large files
# ==========================================
*.psd filter=lfs diff=lfs merge=lfs -text
*.tga filter=lfs diff=lfs merge=lfs -text
*.png filter=lfs diff=lfs merge=lfs -text
*.jpg filter=lfs diff=lfs merge=lfs -text
*.jpeg filter=lfs diff=lfs merge=lfs -text
*.fbx filter=lfs diff=lfs merge=lfs -text
*.obj filter=lfs diff=lfs merge=lfs -text
*.blend filter=lfs diff=lfs merge=lfs -text
*.3ds filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text
*.ogg filter=lfs diff=lfs merge=lfs -text
*.aiff filter=lfs diff=lfs merge=lfs -text
*.avi filter=lfs diff=lfs merge=lfs -text
*.mov filter=lfs diff=lfs merge=lfs -text
*.mp4 filter=lfs diff=lfs merge=lfs -text
*.unitypackage filter=lfs diff=lfs merge=lfs -text
```

### 🎮 How Smart Merge Works:
```text
Your branch (main)       Colleague's branch (feature)
     |                           |
  Scene.unity               Scene.unity
     |                           |
     +--------[ MERGE ]---------+
                |
    Smart Merge compares objects by GUID,
    not line by line. Only actually changed
    objects cause conflicts.
```
Result: Usually 90% of conflicts resolve automatically, the remaining 10% require manual intervention.

---

## 3. Prefab Conflicts
Prefabs are reusable objects. Their conflicts occur most frequently.

### 🔍 Typical Prefab Conflicts:
1. Hierarchy changes (adding/removing children)
2. Component changes (adding/removing scripts)
3. Value changes (public variable modifications)

### 📝 Conflict Example:
```yaml
# Original prefab (base)
Transform:
  m_LocalPosition: {x: 0, y: 0, z: 0}

# Branch A (changed position)
Transform:
  m_LocalPosition: {x: 10, y: 0, z: 0}

# Branch B (changed scale)
Transform:
  m_LocalScale: {x: 2, y: 2, z: 2}

# MERGE: Both changes conflict!
```

### ✅ Solution: Proper Prefab Management
1. Use Variant Prefabs:
   - Base prefab holds common data
   - Variants inherit and override only what's needed
  
2. Split large prefabs into smaller ones:
```text
❌ Bad: Enemy (everything in one prefab)
✅ Good: Enemy → EnemyModel, EnemyController, EnemyHealth
```
3. Use Addressables for large assets
4. Team communication:
   - Who works on a prefab — informs others
   - Make small commits with descriptions
  
---

## 4. Git LFS for Large Assets
Git LFS (Large File Storage) — Git extension for working with large files.

### 📊 Statistics:
| File | Size | Git (without LFS) | Git LFS |
| --- | --- | --- | --- |
| Texture (PNG) | 10 MB | ⚠️ Bloats repo | ✅ Stored separately |
| Model (FBX) | 50 MB | ❌ Performance issues | ✅ Optimal |
| Video (MP4) | 100 MB | ❌ Impossible to store | ✅ Works great |
| Audio (WAV) | 20 MB | ❌ Slow clone | ✅ Fast clone |

### 🚀 Installing Git LFS:
```bash
# 1. Install Git LFS
# Windows: https://git-lfs.github.com/
# macOS: brew install git-lfs
# Linux: sudo apt install git-lfs

# 2. Initialize in the repository
git lfs install

# 3. Add files to track
git lfs track "*.psd"
git lfs track "*.png"
git lfs track "*.fbx"
git lfs track "*.wav"
git lfs track "*.mp4"

# 4. Check settings
git lfs track

# 5. Add .gitattributes to repository
git add .gitattributes
git commit -m "Add Git LFS configuration"

# 6. Clone repository with LFS
git clone <repository-url>
git lfs pull  # Download LFS files
```

### 📄 Git LFS Configuration in Unity Project:
```bash
# Set LFS for entire project
git lfs track "Assets/Textures/**/*.png"
git lfs track "Assets/Models/**/*.fbx"
git lfs track "Assets/Audio/**/*.wav"
git lfs track "Assets/Animations/**/*.anim"
git lfs track "Packages/*.tgz"

# Check which files are already in LFS
git lfs ls-files

# Migrate existing files to LFS
git lfs migrate import --include="*.png,*.fbx,*.wav"
```

---

## 5. Complete Git Repository Setup for Unity
```bash
# 1. Create repository
git init

# 2. Add .gitignore
echo "# Unity .gitignore" > .gitignore
curl -o .gitignore https://raw.githubusercontent.com/github/gitignore/main/Unity.gitignore

# 3. Add .gitattributes for Smart Merge
echo "*.unity merge=unityyamlmerge" >> .gitattributes
echo "*.prefab merge=unityyamlmerge" >> .gitattributes
echo "*.asset merge=unityyamlmerge" >> .gitattributes

# 4. Set up Git LFS
git lfs install
git lfs track "*.psd" "*.png" "*.fbx" "*.wav" "*.mp4"

# 5. Configure UnityYAMLMerge (globally)
git config --global merge.unityyamlmerge.name "Unity Smart Merge"
git config --global merge.unityyamlmerge.driver 'C:/Program Files/Unity/Editor/Data/Tools/UnityYAMLMerge.exe merge --force -p --fallback base "%P" "%O" "%B"'

# 6. First commit
git add .
git commit -m "Initial Unity project setup with Git LFS and Smart Merge"

# 7. Add remote repository
git remote add origin https://github.com/username/unity-game.git
git push -u origin main
```

---

## 6. Best Practices and Common Mistakes
### ✅ Recommendations:
1. Commit often, push regularly — small commits are easier to merge
2. Use Feature Branches — don't work directly on main
3. Write meaningful commit messages — colleagues should understand changes
4. Use Smart Merge — saves 90% of conflict resolution time
5. Store large files in LFS — otherwise the repository will bloat
6. Test before pushing — broken builds affect the entire team

### ❌ Common Mistakes:
```bash
# ❌ ERROR: Forgot .gitignore, Library got into the repository
git add .  # Will add Library
# ✅ CORRECT: Add .gitignore BEFORE first commit

# ❌ ERROR: Forgot to configure Smart Merge
# Scene and prefab conflicts become a nightmare
# ✅ CORRECT: Configure UnityYAMLMerge BEFORE starting work

# ❌ ERROR: Uploading large files without LFS
git add Assets/Textures/*.png  # 500 MB in repo
# ✅ CORRECT: git lfs track "*.png"

# ❌ ERROR: Commit with scene changes without comment
git commit -m "update"  # What changed?
# ✅ CORRECT: "Fix player spawning position in Level1"

# ❌ ERROR: Working directly on main
# ✅ CORRECT: git checkout -b feature/new-ui
```

---

### ⭐ If this project was useful, put a star on GitHub!
