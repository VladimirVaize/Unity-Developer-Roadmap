# 🗺️ Scene Management in Unity: LoadScene, Additive Mode, DontDestroyOnLoad

This material covers three key scene management mechanisms in Unity: loading scenes (LoadScene), 
additive loading (LoadSceneMode.Additive), and persisting objects between scenes (DontDestroyOnLoad). 
You'll learn how to build multi-level games, seamless transitions, and global managers.

---

## 1. 📂 LoadScene – loading a scene
### Purpose:
The `SceneManager.LoadScene()` method allows you to load a new scene, completely replacing the current one. This is the foundation for transitions between menus, levels, Game Over screens, etc.

### Basic call methods:
```csharp
using UnityEngine.SceneManagement;

// By scene name (case-sensitive)
SceneManager.LoadScene("Level02");

// By scene index (order in Build Settings)
SceneManager.LoadScene(1);

// With additional parameters (see next section)
SceneManager.LoadScene("Level02", LoadSceneMode.Single); // default mode
```

### 🎮 How to use:
1. Add all required scenes to Build Settings (`File` → `Build Settings` → `Add Open Scenes`).
2. Call loading in any script:
   - When pressing a "Start Game" button.
   - When the player dies – reload the current level.
   - When completing a level – load the next one.
  
Example – reload the current level by pressing `R`:
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.R))
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
```

### ⚠️ Important notes:
- Synchronous loading (`LoadScene`) may cause a brief freeze (especially on heavy scenes). For large levels, use asynchronous loading.
- After loading a new scene, all objects from the previous scene are destroyed (unless marked with `DontDestroyOnLoad`).

---

## 2. ➕ LoadSceneMode.Additive – additive loading
### Purpose:
Additive mode loads a new scene on top of the current one, without destroying existing objects. This allows you to:
- Create persistent worlds (streaming level parts as the player moves).
- Load managers and systems (audio, saves) from a separate scene.
- Build seamless open worlds.

### How to use:
```csharp
// Load a scene additively
SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);

// Asynchronous additive loading (recommended)
StartCoroutine(LoadAdditiveSceneAsync("EnvironmentScene"));
```

### 📌 Real example – loading UI on top of a gameplay scene:
1. You have a `Gameplay` scene with the level and character.
2. You have a `UI` scene with an inventory panel, health bar, and pause menu.
3. When starting the level, you load `UI` additively:

```csharp
SceneManager.LoadScene("UI", LoadSceneMode.Additive);
```

4. UI objects live on top of the gameplay. If you reload `Gameplay` (not additively), the UI stays in place.

### 🔁 How to unload an additive scene:
```csharp
SceneManager.UnloadSceneAsync("UIScene");
```

Example – dynamic room loading in a dungeon:

Player approaches a door → you asynchronously load `Room_Corridor` additively → when the player enters the corridor, you unload the previous room (`UnloadSceneAsync`). This is how seamless open worlds work.

---

## 3. 🛡️ DontDestroyOnLoad – persisting objects between scenes
### Purpose:
Normally, when loading a new scene, Unity destroys all objects from the old scene. The `DontDestroyOnLoad()` method marks an object as "indestructible" – it survives the loading of any new scene.

### Typical uses:
- Global Game Manager – stores score, lives, progress, settings.
- Music player (AudioManager) – music doesn't interrupt when switching levels.
- Network connection – don't lose connection when transitioning between screens.

### How to use:
```csharp
void Awake()
{
    // Make this object indestructible
    DontDestroyOnLoad(gameObject);
}
```

### ⚠️ Duplicate problem (and its solution):
If you reload a scene that already has a `GameManager` with `DontDestroyOnLoad`, and the new scene also has a `GameManager`, you'll end up with two identical managers. This causes errors.

Correct pattern – singleton with duplicate check:
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destroy the duplicate
        }
    }
}
```

### 🧹 How to "kill" a DontDestroyOnLoad object when no longer needed:
If you need to remove a global object (e.g., when returning to the main menu), simply call `Destroy(gameObject)` manually. Unity will allow this despite `DontDestroyOnLoad`.

---

## 4. 🔄 Asynchronous loading (Async) – for large scenes
Synchronous `LoadScene` can cause a freeze on heavy levels. Use the asynchronous version to show a progress bar.

```csharp
IEnumerator LoadYourSceneAsync(string sceneName)
{
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
    
    // While the scene is not fully loaded
    while (!asyncLoad.isDone)
    {
        float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
        Debug.Log($"Loading: {progress * 100}%");
        // Update UI progress bar
        yield return null;
    }
}
```

---

## 5. 🧩 Interaction: complete example
### Game scenario:
1. Main Menu.
2. Gameplay scene.
3. UI scene with health and inventory.
4. Global Game Manager must live forever.

### Implementation:
```csharp
// In MainMenu, on "Start Game" button
public void OnStartGame()
{
    // Load GameManager from a separate scene (if not already loaded)
    SceneManager.LoadScene("GameManagerScene", LoadSceneMode.Additive);
    
    // Load gameplay scene
    SceneManager.LoadScene("Gameplay");
    
    // Load UI on top
    SceneManager.LoadScene("UI", LoadSceneMode.Additive);
}

// In GameManager (singleton, DontDestroyOnLoad)
void Awake()
{
    if (Instance == null) 
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else Destroy(gameObject);
}
```

---

## 📊 Cheat sheet

| Task | Code |
| --- | --- |
| Load a scene with full replacement | `SceneManager.LoadScene("SceneName")` |
| Load additively | `SceneManager.LoadScene("SceneName", LoadSceneMode.Additive)` |
| Unload an additive scene | `SceneManager.UnloadSceneAsync("SceneName")` |
| Make an object persistent | `DontDestroyOnLoad(gameObject)` |
| Asynchronous loading | `SceneManager.LoadSceneAsync("SceneName")` |

---

### ⭐ If this project was useful, put a star on GitHub!
