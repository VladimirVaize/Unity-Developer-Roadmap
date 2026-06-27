# 📦 Asset Store and Packages: Package Manager, Asset Import, Dependency Updates, Creating Custom Packages
Unity Package Manager (UPM) is a package management system that revolutionized how we work with assets and dependencies in Unity. 
Instead of the old "download and import to Assets" approach, UPM offers centralized version management, automatic dependency updates, and a clean project structure.

---

## 1. Unity Package Manager (UPM) — Core Concepts
Package Manager is a tool for adding, updating, and removing packages in a Unity project. 
It replaces the legacy Asset Store with a versioning-supported solution.

### 📁 UPM Package Structure:
```text
MyPackage/
├── package.json           # 📄 Package manifest (required)
├── Runtime/               # 🔧 Runtime code
│   ├── Scripts/
│   └── Prefabs/
├── Editor/                # 🛠️ Editor code (not in builds)
│   └── EditorScripts/
├── Documentation~/        # 📚 Documentation (optional)
├── Tests/                 # 🧪 Tests (optional)
│   ├── Editor/
│   └── Runtime/
├── Samples~/              # 📂 Usage examples (optional)
└── Resources/             # 📀 Resources
```

### 📄 package.json — Package Manifest:
```json
{
    "name": "com.mycompany.mypackage",
    "version": "1.0.0",
    "displayName": "My Custom Package",
    "description": "A package for handling game data serialization",
    "unity": "2021.3",
    "unityRelease": "0f1",
    "dependencies": {
        "com.unity.textmeshpro": "3.0.6",
        "com.unity.ugui": "1.0.0"
    },
    "keywords": [
        "data",
        "serialization",
        "json"
    ],
    "author": {
        "name": "MyCompany",
        "email": "support@mycompany.com",
        "url": "https://www.mycompany.com"
    },
    "samples": [
        {
            "displayName": "Basic Example",
            "description": "Basic usage example",
            "path": "Samples~/BasicExample"
        }
    ]
}
```

---

## 2. Working with Package Manager
### 🖥️ Package Manager Interface:
Access: `Window → Package Manager`

| Tab | Description |
| --- | --- |
| Packages: In Project | Packages already added to the project |
| Packages: My Assets | Your Asset Store purchases | 
| Packages: Built-in | Unity built-in packages |
| Packages: Unity Registry | Official Unity packages |
| Packages: My Registries | Custom registries |
| Packages: Asset Store | Packages from Asset Store |

### 📦 Adding a Package via UI:
1. Open `Window → Package Manager`
2. Select source (Unity Registry / My Assets)
3. Find the package
4. Click Install

### 📦 Adding a Package via Git URL:
Method 1: Via UI
1. Package Manager → "➕" → "Add package from git URL"
2. Enter URL, e.g.: `https://github.com/Unity-Technologies/InputSystem.git`

Method 2: Via manifest.json
```json
// Project/Packages/manifest.json
{
    "dependencies": {
        "com.unity.inputsystem": "https://github.com/Unity-Technologies/InputSystem.git",
        "com.mycompany.custom": "https://github.com/mycompany/custom-package.git#v1.2.0"
    }
}
```

### 📦 Adding a Package Locally (for development):
```json
// Project/Packages/manifest.json
{
    "dependencies": {
        "com.mycompany.localpackage": "file:../MyLocalPackage"
    }
}
```

### 📦 Adding a Package from Asset Store:
1. Purchase the asset in Asset Store (via browser or Unity window)
2. In Unity: `Window → Asset Store` → find asset → Download
3. After download: Import

> [!Important]
> Asset Store assets import to the `Assets/` folder and are NOT UPM packages.
> This is the legacy system, still used for most commercial assets.

---

## 3. Asset Import
### 🏪 Importing from Asset Store:
1. Open `Window → Asset Store`
2. Find the asset
3. Download
4. Import
5. Select which files to import

```csharp
// Example script for importing an asset via code (used in CI/CD)
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

public class AssetImporter
{
    [MenuItem("Tools/Import Asset")]
    public static void ImportAsset()
    {
        // Adding package via UPM (if it's a UPM package)
        AddRequest request = Client.Add("com.unity.textmeshpro");
        // Wait for completion...
    }
}
#endif
```

### 📂 Organizing Imported Assets:
```text
Assets/
├── _MyProject/                # 📁 Project's own assets
│   ├── Scripts/
│   ├── Scenes/
│   └── Prefabs/
├── Plugins/                   # 🔌 Plugins (all platforms)
├── TextMesh Pro/              # 📝 Asset Store asset
├── DOTween/                   # 🌀 Asset Store asset
└── Packages/                  # 📦 Symlinks to UPM packages (auto)
```

> [!Tip]
> Create a `_Project` folder for your own files to separate them from imported assets. This simplifies searching and backup.

### ⚠️ Common Import Issues:
1. Version conflicts — two packages require different versions of the same dependency
2. DLL duplication — different packages may contain the same libraries
3. Assembly Definition — imported assets may not work with your asmdefs

---

## 4. Dependency Updates
### 🔄 Updating a Package in UPM:
1. Open `Package Manager`
2. Select the package from In Project list
3. Click Update (if available)

### 📝 Updating via manifest.json:
```json
// Before:
"com.unity.textmeshpro": "3.0.6"

// After:
"com.unity.textmeshpro": "3.0.8"  // Updated version
```

### 🧩 Resolving Dependency Conflicts:
```json
// Project/Packages/manifest.json
{
    "dependencies": {
        // Direct dependency
        "com.mycompany.mypackage": "1.0.0",
        
        // Override version to resolve conflict
        "com.unity.textmeshpro": "3.0.8"
    }
}
```

### 🛠️ CLI Commands for Dependency Management:
```bash
# Install package via command line
unity-editor -projectPath ./MyProject -executeMethod PackageManagerHelper.InstallPackage

# Clear package cache
rm -rf Library/PackageCache
# Reopen project — packages will reload
```

---

## 5. Creating Custom Packages
### 🚀 Step 1: Create Package Structure
```bash
# Create package folder
mkdir MyPackage
cd MyPackage

# Create structure
mkdir Runtime
mkdir Editor
mkdir Documentation~
mkdir Samples~
mkdir Tests

# Create manifest
touch package.json
```

### 📄 Step 2: Fill package.json
```json
{
    "name": "com.mycompany.utility",
    "version": "1.0.0",
    "displayName": "My Utility Package",
    "description": "A collection of reusable Unity utilities",
    "unity": "2021.3",
    "dependencies": {
        "com.unity.textmeshpro": "3.0.6"
    },
    "keywords": ["utility", "tools", "helpers"],
    "author": {
        "name": "MyCompany",
        "email": "dev@mycompany.com",
        "url": "https://mycompany.com"
    }
}
```

### 🧩 Step 3: Add Code
Runtime/Scripts/MyHelper.cs:
```csharp
using UnityEngine;

namespace MyCompany.Utility
{
    public static class MyHelper
    {
        public static void LogMessage(string message)
        {
            Debug.Log($"[MyPackage] {message}");
        }
        
        public static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null)
                component = obj.AddComponent<T>();
            return component;
        }
    }
}
```

Editor/MyPackageMenu.cs:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyCompany.Utility.Editor
{
    public static class MyPackageMenu
    {
        [MenuItem("Tools/My Package/Hello World")]
        public static void HelloWorld()
        {
            MyHelper.LogMessage("Hello from My Package!");
        }
    }
}
#endif
```

### 🧪 Step 4: Add Samples
Create folder `Samples~/BasicExample/` with:
- `BasicExample.unity`
- `ExampleScript.cs`

In `package.json` add:
```json
"samples": [
    {
        "displayName": "Basic Example",
        "description": "Shows how to use the utility package",
        "path": "Samples~/BasicExample"
    }
]
```

After importing the package, the Import button will appear in Package Manager.

### 📦 Step 5: Add Tests
Tests/Runtime/MyHelperTests.cs:

```csharp
using NUnit.Framework;
using MyCompany.Utility;

public class MyHelperTests
{
    [Test]
    public void LogMessage_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => MyHelper.LogMessage("Test"));
    }
}
```

### 🏷️ Step 6: Publish the Package
Option 1: Local Package
```json
// In project's manifest.json
"dependencies": {
    "com.mycompany.utility": "file:../MyPackage"
}
```

Option 2: Git Repository
```bash
git init
git add .
git commit -m "Initial package commit"
git tag v1.0.0
git remote add origin https://github.com/mycompany/MyPackage.git
git push -u origin main --tags
```

```json
// In manifest.json
"dependencies": {
    "com.mycompany.utility": "https://github.com/mycompany/MyPackage.git#v1.0.0"
}
```

Option 3: Public Registry (npm / OpenUPM)
```bash
# Publish to OpenUPM
npx openupm publish
```

---

## 6. Assembly Definition for Packages
To isolate the package, add `Assembly Definition` files:

Runtime/MyPackage.Runtime.asmdef:
```json
{
    "name": "MyCompany.Utility.Runtime",
    "rootNamespace": "MyCompany.Utility",
    "references": [],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": []
}
```

Editor/MyPackage.Editor.asmdef:
```json
{
    "name": "MyCompany.Utility.Editor",
    "rootNamespace": "MyCompany.Utility.Editor",
    "references": [
        "MyCompany.Utility.Runtime"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": []
}
```

---

## 7. Updating and Maintaining Packages
### 📋 Semantic Versioning (SemVer):

| Version | Meaning |
| --- | --- |
| 1.0.0 | First stable version |
| 1.0.1 | Bug fixes (non-breaking) |
| 1.1.0 | New features (non-breaking) |
| 2.0.0 | Breaking changes (API changed) |

### 🔄 Updating a Package:
1. Change version in `package.json`
2. Create Git tag: `git tag v1.1.0`
3. Push tag: `git push --tags`
4. In project: `Package Manager → Update`

### 🧹 Removing Obsolete Packages:
```json
// Remove the line from manifest.json
// Or via Package Manager: Remove
```

---

## 8. Best Practices
### ✅ Recommendations:
1. Use SemVer for package versioning
2. Document changes in `CHANGELOG.md`
3. Use Assembly Definitions for code isolation
4. Add samples in `Samples~` to demonstrate usage
5. Test the package before publishing (in a separate project)
6. Store sources in Git with version tags
7. For commercial packages use Asset Store Tool

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Hard binding to Unity version
"unity": "2022.3"  // Too specific

// ✅ CORRECT:
"unity": "2021.3"  // Minimum version

// ❌ ERROR: Package uses code from Assets/ folder
// Cannot reference files outside the package

// ❌ ERROR: No asmdef
// Package adds all scripts to Assembly-CSharp

// ❌ ERROR: Forgot to add Samples~ to .gitignore?
// Sample folder with ~ is ignored by Unity during import
```

---

### ⭐ If this project was useful, put a star on GitHub!
