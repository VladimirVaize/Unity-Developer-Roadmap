# 🎯 Task: «Web Save System with Notifications»
You are developing a browser game. Implement a system that:
1. Saves game progress to browser `localStorage` (JS call from C#)
2. Shows browser notification when reaching a new high score (via `window.alert` or `Notification API`)
3. Loads saves on game start (C# call from JS)
4. Handles tab focus loss (pauses the game)
5. Displays browser information (platform, language, screen resolution)

---

## 📝 Code Template to Fill:
```csharp
using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLSaveManager : MonoBehaviour
{
    [Header("UI References")]
    public TMPro.TextMeshProUGUI browserInfoText;
    public TMPro.TextMeshProUGUI saveStatusText;
    
    private int _currentScore = 0;
    private int _highScore = 0;
    
    // TODO: 1. Declare DllImport methods for JS functions
    
    void Start()
    {
        // TODO: 2. Load saves from localStorage on startup
        
        // TODO: 3. Display browser information
    }
    
    public void AddScore(int points)
    {
        _currentScore += points;
        
        // TODO: 4. If new high score - show notification via JS
        
        // TODO: 5. Auto-save after 1 second (using coroutine)
    }
    
    // TODO: 6. Method to be called from JS with loaded data
    public void OnDataLoadedFromJS(string jsonData)
    {
        // Deserialize and apply save
    }
    
    // TODO: 7. Save method for JS
    private void SaveToJS()
    {
        
    }
    
    // TODO: 8. Focus handling (pause when tab is inactive)
    void OnApplicationFocus(bool hasFocus)
    {
        
    }
}
```

---

## 📋 Specific Tasks for Implementation:
### Part A: Create .jslib file
Create `Assets/Plugins/WebGL/SaveSystem.jslib` with functions:
1. `SaveToLocalStorage(key, value)` — save string to localStorage
2. `LoadFromLocalStorage(key)` — load string from localStorage
3. `ShowBrowserNotification(title, body)` — show notification (use `window.alert` or `Notification API`)
4. `GetBrowserInfo()` — returns JSON with platform, language, screen resolution
5. `CallUnityMethod(methodName, param)` — call C# method from JS (e.g., `OnDataLoadedFromJS`)

### Part B: Implement C# Script
1. Import all JS methods via `[DllImport("__Internal")]`
2. In `Start()` call `LoadFromLocalStorage("gameSave")` and pass result to Unity
3. Implement `OnDataLoadedFromJS` — parse JSON and restore `_currentScore` and `_highScore`
4. Implement auto-save via coroutine (save to localStorage 1 second after changes)
5. In `AddScore()` when `_highScore` is exceeded, call `ShowBrowserNotification`
6. Add `OnApplicationFocus(false)` handling — set `Time.timeScale = 0` and pause audio
7. On focus return, unpause

### Part C: Bonus (Optional)
- Add "Export Save" button — download JSON file via JS (create Blob and link)
- Add "Import Save" button — load file via browser file picker

---

## 🧰 Implementation Requirements:
- Use `#if !UNITY_EDITOR && UNITY_WEBGL` for all JS calls
- Add error handling (if localStorage is unavailable)
- Use coroutines for delayed saving
- JSON save format should contain `score` and `highscore`

---

## 🔍 Expected Result:
1. On first run, game shows "No save found"
2. When scoring points and reaching a new record, browser notification appears
3. After closing and reopening the page, progress is restored
4. When switching to another tab, the game pauses
5. Browser console (F12) shows save logs

---

## 💡 Hints:
```javascript
// Example notification function in .jslib
ShowBrowserNotification: function(title, body) {
    var t = UTF8ToString(title);
    var b = UTF8ToString(body);
    if (window.Notification && Notification.permission === "granted") {
        new Notification(t, { body: b });
    } else {
        window.alert(t + ": " + b);
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
