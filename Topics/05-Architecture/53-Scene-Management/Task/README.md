# 🧪 Practical Task: Level Loading System with Global Manager

Goal: Reinforce skills in LoadScene, additive loading (Additive), and DontDestroyOnLoad. 
You will create a game with a main menu, two gameplay levels, persistent UI, and a global manager that keeps score.

---

## 📥 Provided Assets (you create them)
1. Three scenes: `MainMenu`, `Level1`, `Level2`.
2. A simple cube (Player) with movement (arrow keys / WASD).
3. A coin object (Coin) on each level (collected on collision).
4. Text UI to display score.

---

## 🎯 Tasks
### Step 1: Scene setup and Build Settings
1. Create three scenes in Unity: `MainMenu.unity`, `Level1.unity`, `Level2.unity`.
2. In `MainMenu`, add a "Start Game" button (UI Button).
3. Open `File` → `Build Settings` → drag all three scenes into the list (order: 0 – MainMenu, 1 – Level1, 2 – Level2).

### Step 2: Global Manager (DontDestroyOnLoad)
1. Create an empty GameObject → name it `GameManager`.
2. Add script `GameManager.cs`:
   - Make it a singleton (duplicate check).
   - In `Awake()`, call `DontDestroyOnLoad(gameObject)`.
   - Store an `int score` variable and an `AddScore(int value)` method.
  
3. Ensure `GameManager` is not destroyed when loading new scenes.

### Step 3: UI scene loaded additively
1. Create a separate scene `UIScene.unity`.
2. In it, create a Canvas → Text (name it `ScoreText`).
3. Write a script `UIManager.cs` that finds `GameManager` and displays the current score.
4. In `MainMenu` and `Level1`, when starting, load `UIScene` additively:
```csharp
SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
```

5. Ensure the UI does not duplicate on reload (check if the scene is already loaded).

### Step 4: Transition between levels while preserving score
1. In `Level1`, add a finish trigger (a zone that loads `Level2` upon entry).
2. When entering the trigger:
   - Call `SceneManager.LoadScene("Level2")` (replaces Level1 with Level2).
   - Ensure that `GameManager` and `UIScene` are NOT destroyed (they persist thanks to `DontDestroyOnLoad` and additive loading).
  
3. In `Level2`, add a coin (+10 points) and a trigger to return to `MainMenu`.

### Step 5: Asynchronous loading and progress bar (bonus ⭐)
1. Replace synchronous level loading with asynchronous loading showing progress.
2. Create a Canvas with a slider in `MainMenu`.
3. When "Start Game" is pressed:
   - Show the slider.
   - Start a coroutine `LoadSceneAsync("Level1")`.
   - Update the slider from `asyncOperation.progress`.
   - Hide the slider after loading completes.
  
### Step 6: Unloading additive scene on exit (bonus)
1. When returning to `MainMenu` from `Level2`, unload `UIScene`:
```csharp
SceneManager.UnloadSceneAsync("UIScene");
```

2. Destroy `GameManager` (call `Destroy(GameManager.Instance.gameObject)`) so that old data does not persist on a new game start.

---

## ✅ Success Criteria
- `GameManager` preserves score between scenes (coins accumulate).
- UI (score) loads additively and does not duplicate.
- Transition `Level1` → `Level2` happens without losing score.
- Upon returning to the main menu, UI is unloaded and GameManager is destroyed.
- (⭐) Asynchronous loading works with a progress bar.

---

### ⭐ If this project was useful, put a star on GitHub!
