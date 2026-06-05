# 📦 Asset Bundles: Packaging Content for Patches and DLC

This material covers AssetBundle — Unity's technology for packaging resources for dynamic loading and updates. 
Although this approach is considered older and largely superseded by Addressable Assets, 
it remains relevant for large projects that require full control over content management, patching, and DLC processes.

---

## 🎯 What is an AssetBundle and why use it?
AssetBundle is an archive file containing resources (models, textures, audio, prefabs, scenes) 
that can be loaded on demand or downloaded from a server after the application is released.

Main purposes of AssetBundle:
- 📉 Reduce initial application size — resources load only when needed
- 🔄 Hot updates (patches) — fix bugs and replace content without repackaging the entire app
- 🛍️ DLC (Downloadable Content) distribution — add new levels, characters, items after release
- 💾 Memory optimization — unload unused resources

---

## 🏗️ AssetBundle Architecture: How It Works
### Main components:
1. Bundles — `.bundle` or `.assetbundle` files containing packaged resources
2. Manifests — service files describing bundle contents and dependencies
3. Cache — local storage for downloaded bundles on the user's device
4. Server (CDN) — remote storage for bundle distribution

### Two main loading APIs:
| API | Purpose | Characteristic |
| --- | --- | --- |
| `AssetBundle.LoadFromFile` | Local loading from StreamingAssets or PersistentDataPath | Sync/async, efficient for LZ4 compression |
| `UnityWebRequest` | Network loading from server + caching | Versioning and cache support via `Caching` |

---

## 🔨 Creating AssetBundles (Build Process)
### Step 1: Assign bundle name in the editor
In the Project window, select any resource (prefab, texture, model). In the Inspector, find the AssetBundle section at the bottom:
- Select `New...` to create a new bundle name
- Optionally specify a Variant (e.g., `hd`, `mobile`, `low`) for different platforms/qualities

### Step 2: Build script
Create a script in the `Editor` folder:
```csharp
using UnityEditor;
using System.IO;

public class AssetBundleBuilder
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string outputPath = "AssetBundles";
        
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        
        // BuildAssetBundleOptions.None — LZMA (smallest size, slow loading)
        // BuildAssetBundleOptions.ChunkBasedCompression — LZ4 (fast loading, larger size)
        BuildPipeline.BuildAssetBundles(
            outputPath,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneWindows64  // Change for your platform
        );
        
        Debug.Log("AssetBundles built successfully!");
    }
}
```

> [!Important]
> Build output should be outside the `Assets/` folder during build to prevent AssetDatabase interference.
> After building, files can be copied to `StreamingAssets` for local packaging or uploaded to a server.

---

## 📥 Loading AssetBundles
### Method 1: Local loading from StreamingAssets
```csharp
using System.Collections;
using UnityEngine;

public class LocalBundleLoader : MonoBehaviour
{
    IEnumerator Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles/characters");
        
        // Asynchronous loading (recommended)
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        yield return request;
        
        AssetBundle bundle = request.assetBundle;
        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            yield break;
        }
        
        // Load asset by name
        GameObject prefab = bundle.LoadAsset<GameObject>("Player");
        Instantiate(prefab);
        
        // Cleanup: false — keeps created objects, removes bundle link
        bundle.Unload(false);
    }
}
```

### Method 2: Network loading with caching (for DLC and patches)
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebBundleLoader : MonoBehaviour
{
    IEnumerator LoadBundleFromWeb()
    {
        string url = "https://yourserver.com/bundles/characters";
        uint version = 2;  // Bundle version — changes trigger re-download
        
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, version))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                GameObject prefab = bundle.LoadAsset<GameObject>("Player");
                Instantiate(prefab);
                bundle.Unload(false);
            }
            else
            {
                Debug.LogError($"Download failed: {request.error}");
            }
        }
    }
}
```

### Loading methods comparison:
| Method | Use case | Characteristics |
| --- | --- | --- |
| `LoadFromFile(Async)` | Local bundles | Fastest, no memory copy |
| `LoadFromMemory(Async)` | Encrypted/custom loading | Requires `byte[]`, double memory |
| `UnityWebRequest` | Network loading | Cache support, versioning |
| `LoadFromCacheOrDownload` | Deprecated | Use `UnityWebRequest` instead of `WWW` |

---

## 🔄 Patching and Content Updates
### How patching works:
1. Client stores local bundle list + their versions (or hashes)
2. Server provides an up-to-date manifest (bundle list and versions)
3. Patcher compares lists and downloads missing/changed bundles

### Example manifest in JSON format:
```json
{
    "bundles": [
        { "name": "characters", "hash": "a3f5c2e1d4b6", "size": 2048000 },
        { "name": "levels/level1", "hash": "b7d9f1a3c5e8", "size": 512000 }
    ]
}
```

### Simple patcher logic:
```csharp
public class PatchManager : MonoBehaviour
{
    [Serializable]
    public class BundleInfo
    {
        public string name;
        public string hash;
    }
    
    [Serializable]
    public class Manifest
    {
        public List<BundleInfo> bundles;
    }
    
    async Task CheckForUpdates()
    {
        // 1. Download manifest from server
        string manifestUrl = "https://yourserver.com/manifest.json";
        string json = await DownloadString(manifestUrl);
        Manifest serverManifest = JsonUtility.FromJson<Manifest>(json);
        
        // 2. Compare with local cache (e.g., PlayerPrefs)
        foreach (var bundle in serverManifest.bundles)
        {
            string localHash = PlayerPrefs.GetString($"bundle_{bundle.name}");
            if (localHash != bundle.hash)
            {
                // 3. Download updated bundle
                await DownloadBundle(bundle.name);
                PlayerPrefs.SetString($"bundle_{bundle.name}", bundle.hash);
            }
        }
    }
}
```

### ⚠️ Important patching notes:
- Unity does not provide built-in differential patching (downloading only changed parts of a file)
- Differential updates require a custom downloader
- With `WWW.LoadFromCacheOrDownload` or `UnityWebRequest`, simply change the version number in the parameter — the API handles the update automatically

---

## 🧩 Managing Dependencies
The main complexity of AssetBundles is correctly handling dependencies between bundles.
### Problem:
If you have:
- `Player.prefab` (uses `Hero.mat`)
- `Hero.mat` (uses `Hero_Diffuse.png`)

And you put the prefab in `characters.bundle` while keeping the material and texture shared — Unity may:
1. Duplicate resources in each bundle (size increase)
2. Lose references when loading in the wrong order

### Solution:
```csharp
// 1. Load dependencies first
IEnumerator LoadWithDependencies()
{
    // First load shared resources
    AssetBundle sharedMat = AssetBundle.LoadFromFile(Path.Combine(bundlePath, "shared_materials"));
    AssetBundle sharedTex = AssetBundle.LoadFromFile(Path.Combine(bundlePath, "shared_textures"));
    
    // Then load the character bundle
    AssetBundle characters = AssetBundle.LoadFromFile(Path.Combine(bundlePath, "characters"));
    
    // Only after loading all dependencies can you safely load assets
    GameObject player = characters.LoadAsset<GameObject>("Player");
    Instantiate(player);
}
```

### Manifest as helper:
The build generates an `AssetBundles.manifest` file containing all dependencies:
```csharp
AssetBundle manifestBundle = AssetBundle.LoadFromFile(bundlePath);
AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

string[] dependencies = manifest.GetAllDependencies("characters");
foreach (string dep in dependencies)
{
    AssetBundle.LoadFromFile(Path.Combine(bundlePath, dep));
}
```

---

## 📊 Comparison with Addressable Assets

| Characteristic | AssetBundle | Addressable Assets |
| --- | --- | --- |
| Control level | Full, manual | Automated |
| Setup complexity | High (manual dependency management) | Low (built-in) |
| Loading flexibility | Any, fully controllable | Limited by framework |
| Performance | Potentially higher (no overhead) | Good, but has overhead |
| When to choose | Large projects with complex requirements | Most medium/small projects |
| Patch support | Full versioning control | Built-in, simple |
| Scripts/code | ❌ Cannot update C# scripts via Bundle | ❌ Cannot either |

> [!Important]
> AssetBundles cannot update C# scripts — only resources. For logic updates, other solutions are required (ILRuntime, HybridCLR)

---

## 🛠️ Practical Tips
### 1. Compression:
- LZMA — smallest size, slow decompression (everything to memory)
- LZ4 — slightly larger, but streaming from disk
- Choose LZ4 (`ChunkBasedCompression`) for local bundles

### 2. Naming:
```csharp
// Format: [category]/[name]_[platform]_[quality]
"characters/heroes_windows_hd"
"levels/dungeon_android_mobile"
```

### 3. Memory:
Always call `bundle.Unload(false)` when the bundle is no longer needed to free memory

### 4. Platforms:
AssetBundles are not cross-platform compatible — build separately for each 

### 5. iOS specifics:
- Set `No Backup` flag for downloaded bundles via `iOS.Device.SetNoBackupFlag()`
- Use `Application.temporaryCachePath` for temporary bundles

---

### ⭐ If this project was useful, put a star on GitHub!
