# 🧠 Unity App Lifecycle: Handling Background Mode
Unity provides several `MonoBehaviour` callbacks to detect when your application loses focus, 
pauses, or quits. These are essential for mobile and desktop games.

---

## 1. `OnApplicationFocus(bool hasFocus)`
Called when the application gains or loses focus.

| `hasFocus` | State |
| --- | --- |
| `true` | App is in focus (user interacting) |
| `false` | Focus lost (notification shade, another app opened) |

```csharp
void OnApplicationFocus(bool hasFocus)
{
    if (hasFocus)
    {
        Debug.Log("App gained focus");
        // Resume animations, sound, UI updates
    }
    else
    {
        Debug.Log("Focus lost");
        // Pause game, save state
    }
}
```

---

## 2. `OnApplicationPause(bool pauseStatus)`
Called when the app enters or exits background mode.

| `pauseStatus` | State |
| --- | --- |
| `true` | App is going to background |
| `false` | App is coming back to foreground |

```csharp
void OnApplicationPause(bool pauseStatus)
{
    if (pauseStatus)
    {
        Debug.Log("Game paused (minimized or screen locked)");
        Time.timeScale = 0;
        SaveGame();
    }
    else
    {
        Debug.Log("Return to game");
        Time.timeScale = 1;
        RefreshUI();
    }
}
```

---

## 3. `OnApplicationQuit()`
Called just before the application quits.
```csharp
void OnApplicationQuit()
{
    Debug.Log("Application quitting");
    SaveGame();
    PlayerPrefs.Save();
    DisconnectFromServer();
}
```

> [!Warning]
> On mobile devices, `OnApplicationQuit` is not guaranteed if the app is force-closed from the task switcher.

---

## 4. Full Background Handling Example
```csharp
public class AppLifecycleHandler : MonoBehaviour
{
    private bool isGamePaused = false;

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) EnterBackground();
        else ReturnFromBackground();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) EnterBackground();
        else ReturnFromBackground();
    }

    private void EnterBackground()
    {
        if (isGamePaused) return;
        isGamePaused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;
    }

    private void ReturnFromBackground()
    {
        if (!isGamePaused) return;
        isGamePaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("LastSessionTime", System.DateTime.Now.Hour);
        PlayerPrefs.Save();
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
