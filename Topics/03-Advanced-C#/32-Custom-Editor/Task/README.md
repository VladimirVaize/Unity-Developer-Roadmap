# 📝 Task: Inspector for a "Smart Door" (SmartDoor)

### Context:
You have a `SmartDoor` component that can open manually, automatically when the player approaches, or on a timer. 
In the standard Inspector, all fields are always visible — this is confusing. You need to create a Custom Editor that:
1. Shows a `doorMode` selection field (door mode: manual, proximity, timed).
2. Dynamically displays only relevant fields depending on the selected mode.
3. Adds a "Preview in Scene View" button — when clicked, it highlights the door and logs its mode to the console.
4. If the opening speed (`openSpeed`) is less than or equal to zero, it shows a red error message.

### Simplified `SmartDoor` structure:
```csharp
public enum DoorMode { Manual, Proximity, Timed }

public class SmartDoor : MonoBehaviour
{
    public DoorMode doorMode = DoorMode.Manual;
    
    // For manual mode — not used; for others — yes
    public float openSpeed = 2f;
    
    // For Proximity mode
    public float detectionRadius = 3f;
    
    // For Timed mode
    public float autoCloseDelay = 5f;
}
```

### Requirements:
- Manual mode → show only `doorMode` (hide `openSpeed`, `detectionRadius`, `autoCloseDelay`).
- Proximity mode → show `doorMode`, `openSpeed`, `detectionRadius`.
- Timed mode → show `doorMode`, `openSpeed`, `autoCloseDelay`.
- A "Preview in Scene View" button — placed below the fields.
- A red HelpBox warning if `openSpeed <= 0` and `doorMode != DoorMode.Manual`.

Hint: Use `EditorGUILayout.PropertyField()` to draw fields and `serializedObject.FindProperty()` to access them. 
To hide fields, simply call `PropertyField` only when the mode requires it, or use `EditorGUI.indentLevel` for better styling.

After implementation, make sure Undo/Redo works (thanks to `SerializedObject` and `ApplyModifiedProperties()`).

---

### ⭐ If this project was useful, put a star on GitHub!
