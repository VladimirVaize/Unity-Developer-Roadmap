# 🎯 Practical Task: Logging System for a Dungeon

You are developing an RPG with procedurally generated dungeons. Your task is to implement a logging system that helps debug level generation and the combat system.

---

## 📝 Task Description
You are given a `DungeonGenerator` class that generates rooms and enemies, and a `CombatSystem` class that handles combat. 
In their current form, they use raw `Debug.Log`, which clutters the console and will slow down the release build.

### Requirements:
1. Create a `GameLogger` class with the following methods:
   - `LogGeneration(string message)` — logs for the dungeon generation process. Should work ONLY inside the Unity editor.
   - `LogCombat(string message)` — logs for the combat system. Should work in Development builds and in the editor.
   - `LogCriticalError(string message)` — critical error. Should work ALWAYS (including Release builds) and write the message to a file `critical_errors.txt` inside `Application.persistentDataPath`.
2. Modify the code of the classes, replacing direct `Debug.Log` calls with calls to your `GameLogger`.
3. Set up interception of all logs via `Application.logMessageReceived` so that all `LogGeneration` and `LogCombat` messages are also duplicated to a `full_debug.log` file,
   but only in Development builds (check via `Debug.isDebugBuild`).

---

## 🔧 Initial code (provided)
```csharp
// DungeonGenerator.cs
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public void GenerateDungeon()
    {
        Debug.Log("Starting dungeon generation...");
        // ... generation logic
        Debug.Log("Created room #" + Random.Range(1, 20));
        // ... more logic
        Debug.Log("Generation complete. Total enemies: " + Random.Range(5, 50));
    }
}

// CombatSystem.cs
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public void Attack( GameObject target, int damage )
    {
        Debug.Log($"Attack on {target.name} with {damage} damage");
        // ... combat logic
        if (Random.value < 0.1f)
            Debug.LogError("CRITICAL ERROR: target not found in the combat system!");
    }
}
```

---

## ✅ Expected result
- When running in the Unity editor — all logs (generation, combat, and errors) appear in the console.
- In a Release build (without the Development Build checkbox) — calls to `LogGeneration` and `LogCombat` are not executed at all, but errors are written to a file.
- In a Development build — a `full_debug.log` file appears inside `persistentDataPath` containing ALL logs (duplicated from the console), and a separate `critical_errors.txt` file with critical errors.
- The code must not contain `#if ... #endif` directives (use `[Conditional]` and the `Debug.isDebugBuild` check).

---

## Solution:
- The script with the solution of the task - <a href="../Solution/GameLogger.cs"><code>GameLogger.cs</code></a>
- Modified source scripts:
  - <a href="../Solution/DungeonGenerator.cs"><code>DungeonGenerator.cs</code></a>
  - <a href="../Solution/CombatSystem.cs"><code>CombatSystem.cs</code></a>

---

### ⭐ If this project was useful, put a star on GitHub!
