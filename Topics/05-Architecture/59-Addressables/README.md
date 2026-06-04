# 📦 Addressables: Asynchronous Loading and Memory Management

This material covers the Addressables system — Unity's modern solution for asset management that replaces the legacy `Resources` system. 
You'll learn about asynchronous content loading, proper memory management, and key differences from Resources.

---

## 🆚 Addressables vs Resources: Key Differences

| Feature | Resources | Addressables |
| --- | --- | --- |
| Memory unloading | Only via `Resources.UnloadUnusedAssets()` (slow, global) | Per-asset via `Addressables.Release()` (precise, efficient) |
| Loading | Synchronous (`Resources.Load`) | Asynchronous (`LoadAssetAsync`) |
| Package size | Everything in main build | Split into groups (Content Packs) |
| Remote content support | ❌ No | ✅ Yes (CDN hosting) |
| Dependency management | Manual | Automatic (Reference Counting) |

Main Resources drawback: loaded prefabs cannot be unloaded individually — only via global cleanup, which causes freezes.

> When to use Addressables? For medium/large projects (>200 MB or >500 assets), for games with frequent updates (MMO, GaaS), for projects with conditional content loading.

---

## ⚡ 1. Asynchronous Loading
### 🎯 Purpose:
Addressables loads assets asynchronously to avoid blocking the main game thread. While a texture downloads from a server — the game continues running without freezes.

### 📖 Basic Example (via Coroutine)
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesExample : MonoBehaviour
{
    [SerializeField] private string assetAddress = "characters/hero";

    IEnumerator Start()
    {
        // 1. Start async loading
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(assetAddress);
        
        // 2. Wait for completion (yield returns control next frame)
        yield return handle;
        
        // 3. Check success
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject hero = handle.Result;
            Instantiate(hero, transform);
        }
        else
        {
            Debug.LogError($"Failed to load: {assetAddress}");
            Addressables.Release(handle); // Important: release even on error!
        }
    }
}
```

### 📖 Alternative: Event-based
```csharp
void Start()
{
    AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("weapons/sword");
    handle.Completed += OnLoadComplete;
}

private void OnLoadComplete(AsyncOperationHandle<GameObject> handle)
{
    if (handle.Status == AsyncOperationStatus.Succeeded)
    {
        Instantiate(handle.Result);
    }
    else
    {
        Debug.LogError("Failed to load sword");
        Addressables.Release(handle);
    }
}
```

### 📖 Advanced: async/await (C#)
```csharp
public async void LoadWeaponAsync()
{
    try 
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("weapons/axe");
        await handle.Task; // Non-blocking wait
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Instantiate(handle.Result);
        }
        Addressables.Release(handle);
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error: {e.Message}");
    }
}
```

> [!Warning]
> `AsyncOperationHandle.Task` is not available on WebGL.

---

## 🧹 2. Memory Management
### 🎯 Purpose:
Addressables uses reference counting. 
Each `LoadAssetAsync` call increments the counter, each `Release` call decrements it. 
When the counter reaches zero — the asset is unloaded from memory.

### 🔑 Golden Rule: Mirror calls
Every Load must have its own Release:
```csharp
public class WeaponManager : MonoBehaviour
{
    private AsyncOperationHandle<GameObject> _swordHandle;

    public void LoadSword()
    {
        _swordHandle = Addressables.LoadAssetAsync<GameObject>("weapons/sword");
        _swordHandle.Completed += (handle) => 
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                Instantiate(handle.Result);
        };
    }

    public void UnloadSword()
    {
        // Decrement reference count — when zero, asset unloads
        Addressables.Release(_swordHandle);
    }
}
```

### 💡 Working with InstantiateAsync
Addressables has a dedicated `InstantiateAsync` method that automatically manages reference counting for instances:
```csharp
private AsyncOperationHandle<GameObject> _instanceHandle;

public void SpawnEnemy()
{
    _instanceHandle = Addressables.InstantiateAsync("enemies/goblin", transform.position, Quaternion.identity);
    _instanceHandle.Completed += (handle) => 
    {
        Debug.Log("Enemy spawned!");
    };
}

public void DespawnEnemy(GameObject enemy)
{
    Addressables.ReleaseInstance(enemy); // Decrements counter, destroys at zero
}
```

### ⚠️ Common Memory Mistakes
```csharp
// ❌ WRONG: Lost handle → memory leak
void BadExample()
{
    Addressables.LoadAssetAsync<Texture>("ui/icon"); // Handle not saved!
}

// ❌ WRONG: Release before load completes
void BadExample2()
{
    var handle = Addressables.LoadAssetAsync<GameObject>("hero");
    Addressables.Release(handle); // Too early — asset won't load
}

// ✅ CORRECT: Release when asset is no longer needed
private AsyncOperationHandle<GameObject> _heroHandle;

void LoadHero()
{
    _heroHandle = Addressables.LoadAssetAsync<GameObject>("hero");
    _heroHandle.Completed += (h) => Instantiate(h.Result);
}

void OnDestroy()
{
    Addressables.Release(_heroHandle); // Clean up
}
```

---

## 🛠️ 3. AssetReference — Type-safe References
### 🎯 Purpose:
Instead of string addresses (typos → errors), use `AssetReference`. 
The Inspector shows a dropdown list with only Addressable assets.

```csharp
using UnityEngine.AddressableAssets;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private AssetReference _mainMenuScene;   // For scenes
    [SerializeField] private AssetReferenceGameObject _enemyPrefab; // GameObject only
    [SerializeField] private AssetReferenceTexture2D _uiIcon;      // Texture2D only

    public async void LoadMenu()
    {
        if (!_mainMenuScene.RuntimeKeyIsValid()) return; // Validity check
        
        var handle = _mainMenuScene.LoadSceneAsync(UnityEngine.SceneManagement.LoadSceneMode.Single);
        await handle.Task;
    }
}
```

---

## 📚 4. Loading Multiple Assets (Labels & Groups)
### 🎯 Purpose:
Tag assets with labels to load them in groups.
```csharp
[SerializeField] private List<string> _labels = new List<string>() { "enemies", "bosses" };

public void LoadAllEnemies()
{
    // Load all assets with "enemies" and "bosses" labels
    var handle = Addressables.LoadAssetsAsync<GameObject>(
        _labels, 
        null, // optional callback per loaded asset
        Addressables.MergeMode.Union,
        false // Don't fail on partial error
    );
    
    handle.Completed += (op) =>
    {
        foreach (var enemyPrefab in op.Result)
        {
            Instantiate(enemyPrefab);
        }
        Addressables.Release(op); // Release after use
    };
}
```

---

## 🧠 Workflow Summary
```text
1. Setup: Mark asset as Addressable (Addressables Groups window)
2. Load: Addressables.LoadAssetAsync<T>("address") → get AsyncOperationHandle
3. Wait: yield / await / Completed event
4. Use: handle.Result — loaded asset
5. Unload: Addressables.Release(handle) — decrement reference counter
```

---

## 🔍 Debugging: Addressables Event Viewer
Window `Window > Asset Management > Addressables > Event Viewer` shows:
- Blue bar — asset is loaded
- Green part — current reference count
- White line — load moment

> [!Important]
> Enable `Send Profiler Events` in Addressables settings for the viewer to work.

---

### ⭐ If this project was useful, put a star on GitHub!
