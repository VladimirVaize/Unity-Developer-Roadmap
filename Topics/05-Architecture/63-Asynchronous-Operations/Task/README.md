# 🎯 Practical Task: Asynchronous Resource Loading Manager
## 📋 Task Description
You need to create an asynchronous game resource loading system (prefabs, scenes, AssetBundles) that uses all the studied mechanisms: 
`AsyncOperation`, `yield return`, callbacks, `ResourceRequest`, and `AssetBundleRequest`. 
The system should load content without freezes and report progress.

---

## 🧱 Task Structure
### 📁 Part 1: Basic Loader with Progress
Create a class `AsyncLoader` with methods:
- `LoadSceneAsync(string sceneName, Action<float> onProgress, Action onComplete)` — loads a scene, reporting progress (0–1)
- The scene should not activate until progress reaches 100% (or 0.9, then allowSceneActivation)

### 📁 Part 2: Loading from Resources
Add a method to `AsyncLoader`:
- `LoadResourceAsync<T>(string path, Action<T> onComplete)` — asynchronously loads an asset from Resources and invokes the callback with the loaded object

### 📁 Part 3: Loading from AssetBundle (simulation)
Create a method:
- `LoadFromAssetBundleAsync(string bundleUrl, string assetName, Action<GameObject> onComplete)`
- Use `WWW` or `UnityWebRequest` to load the bundle (use a local file for testing: `"file://" + Application.dataPath + "/test_bundle"`)
- Then asynchronously load the asset via `AssetBundleRequest`

### 📁 Part 4: Test Script with ContextMenu
Create a script `LoaderTester` with `[ContextMenu]` methods:
- `TestLoadScene` — loads a test scene (create an empty scene "TestScene")
- `TestLoadPrefab` — loads any prefab from Resources (e.g., a cube with a component)
- `TestLoadBundle` — loads an asset from a simulated AssetBundle

### 📁 Part 5: Handling Multiple Loads
Implement a loading queue or parallel loading with a maximum limit (e.g., no more than 2 simultaneous loads). 
Use `yield return` and callbacks to notify when all loads are complete.

---

## ✅ Completion Criteria
1. All loads must be asynchronous and cause no freezes
2. `LoadSceneAsync` must correctly use `allowSceneActivation` and progress reporting
3. `ResourceRequest` is used for loading from Resources
4. `AssetBundleRequest` is used for loading from AssetBundle
5. Callbacks are used to notify completion of each load
6. `[ContextMenu]` allows testing each load type directly from the editor
7. (Bonus) Display progress in the console or on a UI progress bar

---

## 🧩 Bonus Task (⭐⭐)
Create a caching system for loaded assets:
- `Dictionary<string, object> cache`
- If an asset is already loaded, subsequent requests return it from the cache immediately (via callback without reloading)
- Add a method `UnloadAsset(string path)` to remove from cache

---

## 🧪 Expected Result
1. Clicking `TestLoadScene` via ContextMenu → loads "TestScene" with progress output to console (0%, 50%, 90%, activation)
2. `TestLoadPrefab` → loads and instantiates the prefab in the scene
3. `TestLoadBundle` → loads an asset from a bundle (you can create a simple test bundle via AssetBundle Browser)
4. Second call to `TestLoadPrefab` (with caching) → instant response from cache

---

### ⭐ If this project was useful, put a star on GitHub!
