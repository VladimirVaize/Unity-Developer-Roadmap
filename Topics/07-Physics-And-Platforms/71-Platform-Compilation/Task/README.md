# 🎯 Task: «Cross-platform Input Manager»
You are developing a game that must run on Android, iOS, and in Unity Editor. 
On mobile devices, control is through screen touches, while in the editor it's through the mouse. 
You need to implement an input manager that automatically adapts to the platform.

## 📝 Code Template (to complete):
```csharp
using UnityEngine;
using UnityEngine.Events;

public class CrossPlatformInputManager : MonoBehaviour
{
    [System.Serializable]
    public class InputEvent : UnityEvent<Vector2> { }
    
    public InputEvent OnTap;          // Event on tap/click
    public InputEvent OnDrag;         // Event on drag
    
    private bool _isDragging = false;
    private Vector2 _lastDragPosition;
    
    void Update()
    {
        // ========== TODO: Add platform-specific input handling ==========
        // 1. For UNITY_EDITOR: handle Input.GetMouseButtonDown(0) and Input.GetMouseButton(0)
        // 2. For UNITY_ANDROID and UNITY_IOS: handle Input.touchCount and Input.GetTouch(0)
        // 3. On tap, invoke OnTap?.Invoke(position)
        // 4. On drag, invoke OnDrag?.Invoke(delta) and OnDrag?.Invoke(position)
        // =================================================================
    }
}
```

---

## 📋 Specific Tasks for Implementation:
1. Add platform directives to separate editor and mobile code.
2. Implement editor handling (`UNITY_EDITOR`):
   - Left mouse click = tap
   - Held button + movement = dragging
   - Button release = end dragging
  
3. Implement mobile device handling (`UNITY_ANDROID` and `UNITY_IOS`):
   - First touch = main interaction
   - Handle touch phases: `Began`, `Moved`, `Ended`
  
4. Add conditional compilation for debugging:
   - In `DEVELOPMENT_BUILD`, output detailed logs to console
   - In final build (`!DEVELOPMENT_BUILD`), logs should be absent
  
5. Add platform check for save paths:
   - On Android, save settings to `Application.persistentDataPath`
   - On iOS, use `Application.persistentDataPath + "/Documents/"`
   - In the editor, save to `Application.dataPath` folder
  
6. Create a custom symbol `USE_TACTILE_FEEDBACK`:
   - If the symbol is defined, trigger vibration on each tap (mobile only)
   - On Android use: `Handheld.Vibrate()`
   - On iOS use: `UnityEngine.iOS.Device.PlayVibration()`
   - In the editor, just log "Vibration would play here"
  
---

## 🧰 Implementation Requirements:
- Use at least 4 different platform directives (`UNITY_EDITOR`, `UNITY_ANDROID`, `UNITY_IOS`, `DEVELOPMENT_BUILD`)
- Create a custom compilation symbol via Player Settings
- All common methods should be extracted outside directives
- Add English comments to each `#if` block

---

## 🔍 Verification:
1. Run in Unity Editor — should work with mouse
2. Build Android version — should work with touches
3. Build iOS version — should work with touches
4. Verify that debug logs are absent in final release build
5. Verify that vibration works (if `USE_TACTILE_FEEDBACK` is enabled)

---

## 💡 Expected Console Output (DEVELOPMENT_BUILD):
```text
[Editor] Input initialized
[Editor] Tap at position: (342, 156)
[Editor] Dragging from (342, 156) to (345, 160)
[Editor] Drag ended
```

```text
[Android] Input initialized
[Android] Touch began at: (120, 340)
[Android] Vibration played (USE_TACTILE_FEEDBACK enabled)
[Android] Touch moved to: (125, 342)
[Android] Touch ended
```

---

### ⭐ If this project was useful, put a star on GitHub!
