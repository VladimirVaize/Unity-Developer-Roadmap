# 🎯 Task: «Mobile Gesture Manager with Notifications»
You are developing a mobile puzzle game. You need to implement a system that handles touches, sends reminders to the player, and saves battery.

## 📝 Implementation Template (GameManager.cs):
```csharp
using UnityEngine;
using UnityEngine.UI;

public class MobileGameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text gestureDisplayText;
    public Text notificationStatusText;
    
    [Header("Settings")]
    public bool enablePowerSaving = true;
    public int reminderDelaySeconds = 60;
    
    // ========== PART 1: Touch & Gesture Handling ==========
    // TODO: Implement detector for the following gestures:
    // 1. Tap - display "Tap detected!"
    // 2. Double Tap (within 300ms) - display "Double tap!"
    // 3. Long Press (longer than 0.7 sec) - display "Long press!"
    // 4. Swipe up/down/left/right - display direction
    
    // ========== PART 2: Push Notifications ==========
    // TODO: Implement sending reminders:
    // 1. On game start, schedule a notification after reminderDelaySeconds seconds
    // 2. When launched from a notification, update notificationStatusText
    // 3. Add a button to cancel all notifications
    
    // ========== PART 3: Battery Optimization ==========
    // TODO: Implement power saving:
    // 1. On app pause, lower targetFrameRate to 15
    // 2. On app resume, restore to 60
    // 3. Use a coroutine to update UI (not every frame)
    // 4. If enablePowerSaving == true, disable unnecessary components in background
}
```

## 📋 Specific Requirements:
1. Gesture Detector (implement in separate script `GestureDetector.cs`):
   - Track touch phases (`Began`, `Moved`, `Ended`, `Canceled`)
   - Calculate hold time for long press detection
   - Calculate movement speed for swipe detection
   - Track interval between touches for double tap
  
2. Push Notifications (use Unity Mobile Notifications):
   - On Android: create a notification channel with high priority
   - On iOS: request authorization
   - When launched from a notification, load a special level or show a message
  
3. Optimization:
   - Update UI (gesture display) only 10 times per second
   - On focus loss, pause all non-critical coroutines
   - Only call `System.GC.Collect()` when transitioning between scenes (not in Update)
  
## 🧰 Implementation Requirements:
- Create three scripts: `GestureDetector`, `NotificationManager`, `BatterySaver`
- Use platform directives for Android/iOS in notifications
- Add comments for every important code block
- All settings (hold time, update interval) should be exposed in the Inspector

## 🔍 Verification:
1. Run on device — test all gestures
2. Minimize the game — FPS should drop
3. Wait for the reminder (or speed it up via Inspector) — notification should arrive
4. Tap the notification — the game should open and display a message

## 💡 Hints:
- For double tap detection, use `Time.time - _lastTapTime < 0.3f`
- For long press, track time in `Began` and check in `Ended`
- For swipes, use `touch.deltaPosition` and `touch.deltaTime`
- Always check `Input.touchCount > 0` before accessing `Input.GetTouch(0)`

---

### ⭐ If this project was useful, put a star on GitHub!
