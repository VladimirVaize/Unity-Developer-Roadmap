# 🧪 Practical Task: Dependency Injection (DI) with Zenject / Extenject

Goal: Strengthen your skills in working with the Zenject (Extenject) DI container in Unity. 
You will create a game service management system, learn how to register dependencies, and use injections in various scenarios.

---

## 📥 Initial Conditions
You have a Unity project where you need to implement several game services:
- Score Service (`IScoreService`)
- Save Service (`ISaveService`)
- Enemy Spawn Service (`IEnemySpawnService`)

All services must be registered in the Zenject DI container.

---

## 🎯 Tasks
### Step 1: Install Zenject / Extenject
1. Install Extenject via Package Manager (URL: https://github.com/Mathijs-Bakker/Extenject.git?path=UnityProject/Assets/Plugins/Zenject/Source)
2. Or download the `.unitypackage` from <a href="https://github.com/Mathijs-Bakker/Extenject/releases">Releases</a>.

### Step 2: Create Services and Register Them
1. Create an `IScoreService` interface with methods:
   - `void AddScore(int points)`
   - `int GetScore()`
   - `void ResetScore()`
  
2. Create an implementation `ScoreService` that stores the current score in a private field.
3. Create an `ISaveService` interface with methods:
   - `void SaveInt(string key, int value)`
   - `int LoadInt(string key, int defaultValue)`
  
4. Create a `PlayerPrefsSaveService` implementation using `PlayerPrefs`.
5. Create an `IEnemySpawnService` interface with the method `void SpawnEnemy(Vector3 position)`.
6. Create an `EnemySpawnService` implementation that instantiates an enemy from a prefab.
7. Create an Installer (`GameServicesInstaller : MonoInstaller`) and register all three services:
   - `IScoreService` as a singleton (`.AsSingle()`)
   - `ISaveService` as a singleton
   - `IEnemySpawnService` as a singleton
  
### Step 3: Using Injections in MonoBehaviour
1. Create a `GameManager` script on the scene.
2. Via field injection (`[Inject]`), receive all three services.
3. In the `Start()` method:
   - Load the saved score via `ISaveService`.
   - If no score exists, set it to 0.
   - Log the current score to the console.
  
4. Add handling for the `Space` key press:
   - When pressed, add 10 points via `IScoreService`.
   - Save the new value via `ISaveService`.
   - Spawn an enemy via `IEnemySpawnService` at a random position.
  
### Step 4: Constructor Injection in a Plain C# Class
1. Create a plain C# class `ScoreDisplay` (not `MonoBehaviour`).
2. Add a constructor that accepts `IScoreService`.
3. Add a method `void DisplayScore()` that logs the score to the console.
4. Register `ScoreDisplay` in the container using `.AsSingle()`.
5. Inject `ScoreDisplay` into `GameManager` via constructor (for this, `GameManager` must be registered in the container — the easiest way: add an `[Inject]` field).

### Step 5: Using a Factory for Dynamic Object Creation
1. Create an `IPickupItem` interface with the method `void Collect()`.
2. Create a `CoinPickup : IPickupItem` class that implements `Collect()` (adds points).
3. Create a factory for `CoinPickup`:
```csharp
public class CoinPickupFactory : PlaceholderFactory<CoinPickup> { }
```

4. Register the factory in the Installer:
```csharp
Container.BindFactory<CoinPickup, CoinPickupFactory>();
```

5. In `EnemySpawnService`, after spawning the enemy, create a coin via the factory and place it next to the enemy.

### Step 6: Validation and Debugging
1. In the Unity Editor, select `Zenject → Validate Current Scene`.
2. Ensure all dependencies are resolved correctly.
3. Fix any errors if present.

---

## ⭐ Bonus Task
1. Create an alternative implementation of `ISaveService` — `FileSaveService`, which saves data to a JSON file.
2. Implement conditional binding: depending on a preprocessor symbol (`UNITY_EDITOR`), register either `PlayerPrefsSaveService` or `FileSaveService`.
```csharp
#if UNITY_EDITOR
    Container.Bind<ISaveService>().To<PlayerPrefsSaveService>().AsSingle();
#else
    Container.Bind<ISaveService>().To<FileSaveService>().AsSingle();
#endif
```

---

## ✅ Success Criteria
- All services are registered in the Installer.
- `GameManager` correctly receives dependencies via `[Inject]`.
- When pressing `Space`, points increase, are saved, and an enemy spawns.
- `ScoreDisplay` (plain C# class) is successfully injected and displays the score.
- The factory creates coins without errors.
- Scene validation (`Zenject → Validate`) shows no errors.
- (⭐) Conditional binding works correctly in the editor and in builds.

---

### ⭐ If this project was useful, put a star on GitHub!
