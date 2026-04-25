# 🐛 Debugging and Logging in Unity: From Debug.Log to File Output

> [!Note]
> Logging is your game's "black box." It lets you peek inside running code without stopping execution.
> This article covers four levels of logging: from basic console output to writing errors to a file in a production build.

---

## 📢 1. Debug.Log (and its family)
The simplest and fastest way to see what's happening in your code.

### Methods of the `Debug` family:
- `Debug.Log("Message")` — regular informational message (white text).
- `Debug.LogWarning("Warning")` — yellow warning (not critical, but worth attention).
- `Debug.LogError("Error")` — red error (usually breaks logic but doesn't stop the game).

### How to use directly in code:
```csharp
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"Player took {amount} damage. Health left: {health}");
        
        if (health <= 0)
        {
            Debug.LogError("PLAYER IS DEAD! Triggering death animation.");
        }
        
        if (health < 20)
        {
            Debug.LogWarning("Player health is critically low!");
        }
    }
}
```

### Where to see the result:
In the Unity editor — Console window (`Window → General → Console`).

### Pros:
- Instant, simple, requires no setup.
- Click on a message in the console — Unity highlights the object that sent the log.

### Cons:
- Remain in builds (if not removed) and reduce performance.
- Clutter the console in the final version of the game.

---

## 🚦 2. Conditional: How to remove logs from builds without deleting code
The `[Conditional]` attribute completely removes method calls from the build if a conditional compilation symbol is not defined.

### Example without Conditional (logs remain in the build):
```csharp
public static class MyLogger
{
    public static void Log(string message)
    {
        Debug.Log($"[LOG] {message}");
    }
}
```

### Example with Conditional (logs disappear from RELEASE builds):
```csharp
using UnityEngine;
using System.Diagnostics;

public static class MyLogger
{
    [Conditional("UNITY_EDITOR")]          // Works only in the editor
    public static void LogEditor(string message)
    {
        Debug.Log($"[EDITOR] {message}");
    }
    
    [Conditional("DEVELOPMENT_BUILD")]     // Works in DEVELOPMENT builds
    public static void LogDev(string message)
    {
        Debug.Log($"[DEV] {message}");
    }
    
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogEditorOrDev(string message)
    {
        Debug.Log($"[EDITOR OR DEV] {message}");
    }
}
```

### How it works:
1. In `Player Settings` → `Other Settings` → `Script Compilation` → `Scripting Define Symbols`, add `DEVELOPMENT_BUILD` (or create your own symbol, e.g., `MY_GAME_LOGS`).
2. If the symbol is NOT defined — all calls to methods marked with `[Conditional("SYMBOL")]` are completely cut out by the compiler.
3. Important: The method and its body physically exist in the code, but the calls disappear — this doesn't make an empty method faster; it removes the call itself.

### Typical symbols in Unity:
- `UNITY_EDITOR` — always defined when code is running inside the editor.
- `DEVELOPMENT_BUILD` — defined for Development builds (the `Development Build` checkbox during building).
- `UNITY_ANDROID` / `UNITY_IOS` — automatically defined when building for that platform.

---

## 📦 3. Logging in builds: Development vs Release

### Development Build (checkbox in Build Settings):
- Enables the Profiler.
- Allows remote debugging of the game.
- Retains all `Debug.Log` in the build (they can be viewed via the log file).
- Adds performance overhead — not for release!

### Release Build (without Development Build checkbox):
- `Debug.Log` is technically called, but the Unity runtime in a build does not output them to a console. However, they still execute and create overhead.
- To remove the overhead — use `[Conditional]` or `#if ... #endif` preprocessor directives.

### Preprocessor directives (alternative to Conditional):
```csharp
public static void MyExpensiveLog(string message)
{
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log(message);
    #endif
}
```
Difference from `[Conditional]`: the directive removes all code inside the block, while `Conditional` removes only method calls, leaving the method body in the build. 
For most cases, `Conditional` is cleaner and more convenient.

---

## 💾 4. Outputting logs to a file on disk
In a finished build, there is no console. To find out what went wrong for the player, you need to write logs to a file.

### Where Unity stores logs:
- Windows: `%USERPROFILE%\AppData\LocalLow\<CompanyName>\<ProductName>\Player.log`
- Mac: `~/Library/Logs/<CompanyName>/<ProductName>/Player.log`
- Linux: `~/.config/unity3d/<CompanyName>/<ProductName>/Player.log`

`CompanyName` and `ProductName` are taken from `Edit → Project Settings → Player`.

### How to write your own log to this file:
```csharp
using UnityEngine;
using System.IO;

public class FileLogger : MonoBehaviour
{
    private string logPath;
    
    void Awake()
    {
        // Application.persistentDataPath — another option (cross-platform folder)
        logPath = Path.Combine(Application.persistentDataPath, "my_game_log.txt");
        Debug.Log($"Log file will be here: {logPath}");
        
        // Intercept all Debug.Log messages
        Application.logMessageReceived += HandleLog;
    }
    
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string entry = $"[{System.DateTime.Now:HH:mm:ss}] [{type}] {logString}\n";
        if (type == LogType.Error || type == LogType.Exception)
        {
            entry += $"STACK: {stackTrace}\n";
        }
        
        File.AppendAllText(logPath, entry);
    }
    
    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    // Manual write
    public static void WriteToFile(string message)
    {
        string path = Path.Combine(Application.persistentDataPath, "my_game_log.txt");
        File.AppendAllText(path, $"[{System.DateTime.Now:HH:mm:ss}] {message}\n");
    }
}
```

### Subscribing to `Application.logMessageReceived`:
- Catches all `Debug.Log`, `Debug.LogWarning`, `Debug.LogError` calls, even from third-party plugins.
- Works in builds, including Release.
- Allows you to duplicate everything to a file without rewriting every log message.

---

## 🧠 Summary: Logging strategy in a project

| Stage | What to use |
|--------------------|------------------------------------------|
| Early development | `Debug.Log` + Console window |
| Feature testing | `[Conditional("DEVELOPMENT_BUILD")]` + Development Build |
| Release build | Disable all logs via `[Conditional]` + keep only critical errors in a file via `Application.logMessageReceived` |
| Live support (LiveOps) | Write selected logs to `Application.persistentDataPath` and give the player a "Send log" button |

---

### ⭐ If this project was useful, put a star on GitHub!
