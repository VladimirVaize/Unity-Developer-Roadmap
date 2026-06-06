# ⏳ Asynchronous Operations in Unity: Loading Without Freezes
This material covers key asynchronous mechanisms in Unity: `AsyncOperation`, `yield return`, callbacks, `ResourceRequest`, and `AssetBundleRequest`. 
These tools allow you to load scenes, resources, and AssetBundles without blocking the main thread and causing game freezes.

---

## 📖 1. AsyncOperation (Base Class for Async Operations)
### 🎯 Purpose:
`AsyncOperation` is the base class for all asynchronous operations in Unity (scene loading, unloading, AssetBundle loading, etc.). 
It allows you to track operation progress and execute actions after completion without blocking gameplay.

### ⚙️ How to use:
```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private AsyncOperation asyncLoad;

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Start async scene loading
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Prevent immediate scene activation (to show loading screen)
        asyncLoad.allowSceneActivation = false;

        // Wait until progress reaches 90% (assumed "loading complete")
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");
            yield return null; // wait one frame
        }

        Debug.Log("Loading complete! Activating scene...");
        asyncLoad.allowSceneActivation = true;
    }
}
```

### 📌 Real-life example:
You're making an RPG with large levels. When transitioning between zones, a loading screen with a progress bar is shown, and the game continues running (no freezes).

---

## 🔄 2. yield return (Coroutines and Waiting)
### 🎯 Purpose:
`yield return` is an instruction in coroutines (IEnumerator) that tells Unity "pause execution of this method until the next frame (or another event)". 
This is the foundation of async code in Unity without multithreading.

### ⚙️ How to use:
```csharp
using System.Collections;
using UnityEngine;

public class CoroutineExamples : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ExampleCoroutine());
    }

    IEnumerator ExampleCoroutine()
    {
        Debug.Log("Coroutine start - frame 1");
        
        // Wait 2 seconds (real time)
        yield return new WaitForSeconds(2f);
        Debug.Log("2 seconds passed");
        
        // Wait 1 frame (until next Update)
        yield return null;
        Debug.Log("Next frame");
        
        // Wait for another async operation to complete
        AsyncOperation op = SceneManager.LoadSceneAsync("Menu");
        yield return op; // coroutine pauses until scene loads
        Debug.Log("Scene loaded!");
        
        // Wait until end of frame (after all Updates and rendering)
        yield return new WaitForEndOfFrame();
        Debug.Log("End of frame, can take screenshot now");
    }
}
```

### 📌 Real-life example:
You spawn 100 enemies, but not all in one frame (otherwise you get a freeze). Use `yield return null` after each spawn to distribute load across multiple frames.

---

## 📞 3. Callbacks
### 🎯 Purpose:
Callbacks are functions passed as parameters that get called after an asynchronous operation completes. They allow you to organize code without "callback hell" or deeply nested coroutines.

### ⚙️ How to use:
```csharp
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CallbackExample : MonoBehaviour
{
    // Create a delegate for the callback (or use Action)
    public void LoadSceneWithCallback(string sceneName, Action onComplete)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, onComplete));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, Action onComplete)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;
        
        // Wait one frame after activation for scene to initialize
        yield return null;
        
        // Invoke the callback if provided
        onComplete?.Invoke();
        Debug.Log("Callback invoked!");
    }

    void Start()
    {
        LoadSceneWithCallback("GameLevel", () =>
        {
            Debug.Log("Level loaded! Can initialize player.");
            FindObjectOfType<Player>().Initialize();
        });
    }
}
```

### 📌 Real-life example:
You load a dialog window prefab from Resources and after loading, invoke a callback that subscribes buttons to events and shows the window.

---

## 📦 4. ResourceRequest (Async Loading from Resources)
### 🎯 Purpose:
`ResourceRequest` is a subclass of `AsyncOperation` returned when asynchronously loading resources from the `Resources` folder. 
Allows loading prefabs, textures, audio, etc. without blocking.

### ⚙️ How to use:
```csharp
using System.Collections;
using UnityEngine;

public class ResourceRequestExample : MonoBehaviour
{
    public string prefabPath = "Enemies/Goblin";
    
    IEnumerator Start()
    {
        // Asynchronously load prefab from Resources folder
        ResourceRequest request = Resources.LoadAsync<GameObject>(prefabPath);
        
        // Wait for loading to complete
        yield return request;
        
        // Get the loaded object
        GameObject goblinPrefab = request.asset as GameObject;
        
        if (goblinPrefab != null)
        {
            Instantiate(goblinPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Goblin loaded and instantiated!");
        }
        
        // Alternative with callback
        StartCoroutine(LoadWithCallback("Weapons/Sword", (asset) =>
        {
            Instantiate(asset);
            Debug.Log("Sword loaded!");
        }));
    }
    
    IEnumerator LoadWithCallback(string path, System.Action<Object> callback)
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>(path);
        yield return request;
        callback?.Invoke(request.asset);
    }
}
```

### 📌 Real-life example:
You have 50 different enemies. When an enemy appears, you asynchronously load its prefab from `Resources/Enemies/` to avoid keeping all of them in memory simultaneously.

---

## 🧩 5. AssetBundleRequest (Async Loading from AssetBundle)
### 🎯 Purpose:
`AssetBundleRequest` is a subclass of `AsyncOperation` for asynchronously loading assets from AssetBundles 
(resource packages that can be loaded separately from the main application, e.g., DLC or server content).

### ⚙️ How to use:
```csharp
using System.Collections;
using UnityEngine;

public class AssetBundleExample : MonoBehaviour
{
    public string bundleURL = "https://myserver.com/character_bundle";
    public string assetName = "Hero";

    IEnumerator LoadAssetFromBundle()
    {
        // 1. Asynchronously load AssetBundle from internet or disk
        using (WWW www = new WWW(bundleURL))
        {
            yield return www; // wait for bundle to load
            
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"Bundle load error: {www.error}");
                yield break;
            }
            
            // 2. Get AssetBundle from downloaded data
            AssetBundle bundle = www.assetBundle;
            
            // 3. Asynchronously load specific asset from bundle
            AssetBundleRequest request = bundle.LoadAssetAsync<GameObject>(assetName);
            yield return request;
            
            // 4. Get and use the loaded asset
            GameObject hero = request.asset as GameObject;
            if (hero != null)
            {
                Instantiate(hero, Vector3.zero, Quaternion.identity);
                Debug.Log("Hero loaded from AssetBundle!");
            }
            
            // 5. Unload the bundle to free memory
            bundle.Unload(false);
        }
    }
    
    IEnumerator Start()
    {
        yield return StartCoroutine(LoadAssetFromBundle());
    }
}
```

### 📌 Real-life example:
You release an update with a new car model. The server delivers an AssetBundle, the game asynchronously downloads it and loads the model without restarting the application.

---

## 🧠 Comparison Table

| Mechanism | Used For | Blocks Game? | Ideal For |
| --- | --- | --- | --- |
| `AsyncOperation` | Scene loading, unloading | No | Loading screens |
| `yield return` | Coroutines, waiting | No (only suspends the coroutine) | Spawning objects, timers |
| Callbacks (Action) | Completion notifications | No | Sequential operations |
| `ResourceRequest` | Loading from Resources folder | No | Prefabs, configs, audio |
| `AssetBundleRequest` | Loading from AssetBundle | No | DLC, server content, mods |

---

### ⭐ If this project was useful, put a star on GitHub!
