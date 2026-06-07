# 🎯 Task: «Session Time Tracking & Game Resume»
You are developing a mobile game. Implement a system that:
1. On Pause / Focus loss:
   - Pauses the game (`Time.timeScale = 0`)
   - Mutes all audio (`AudioListener.pause = true`)
   - Saves current time (`System.DateTime.Now`) to `PlayerPrefs` with key `"LastPauseTime"`
  
2. On Return from background:
   - Unpauses the game
   - Unmutes audio
   - Logs to console: `"Returned after X seconds"` where X = difference between current time and `"LastPauseTime"`
  
3. On Application Quit:
   - Saves current time to `PlayerPrefs` with key `"LastQuitTime"`
  
4. On first Start:
   - Checks if the app crashed last time (if `"LastQuitTime"` is missing but `"LastPauseTime"` exists → app was killed in background)
  
---

## 📝 Requirements:
- Use `OnApplicationPause`, `OnApplicationFocus`, `OnApplicationQuit`.
- Use a flag (`isPaused`) to prevent duplicate actions.
- Write all logic in a single script `LifecycleTracker`.

---

## 🔍 Expected console output (example):
```text
[Start] Last quit time not found. Possible crash on last session.
[Focus] Focus lost
[Pause] Paused at 14:30:05
[Focus] Focus regained
[Return] Returned after 12 seconds
[Quit] Quitting at 14:30:22
```

---

### ⭐ If this project was useful, put a star on GitHub!
