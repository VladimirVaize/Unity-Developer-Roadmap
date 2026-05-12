# 🛠️ Custom Editor / PropertyDrawer: Extending the Unity Editor for Your Needs (Editor API)

The Unity Editor is a powerful tool, but its standard interfaces aren't always convenient for complex or repetitive components. 
Custom Editor and PropertyDrawer mechanics allow you to create custom visual elements inside the Inspector window. 
This speeds up development, reduces configuration errors, and gives you full control over the user experience within the editor.

> All code for these extensions must be placed inside an `Editor` folder (or its subfolders) so that Unity does not include them in the final game build.

---

## 📦 1. PropertyDrawer — customizing a single field
### When to use:
You want to change how a single field (or a simple type like `Vector2`) appears in the Inspector, or add validation, a button, or a slider.

### Basic principle:
Create a class that inherits from `PropertyDrawer`. Use the `[CustomPropertyDrawer(typeof(Type))]` attribute to bind it to a specific data type. Then override the `OnGUI()` method to handle drawing.

### 🔧 Example: Drawer for `Vector2` with an extra "Direction" label
```csharp
// File: Assets/Scripts/Editor/DirectionVectorDrawer.cs
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Vector2))]
public class DirectionVectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        SerializedProperty xProp = property.FindPropertyRelative("x");
        SerializedProperty yProp = property.FindPropertyRelative("y");
        
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label.text + " (direction)");
        
        Rect fieldsRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.BeginChangeCheck();
        
        float oldX = xProp.floatValue;
        float oldY = yProp.floatValue;
        Vector2 newValue = EditorGUI.Vector2Field(fieldsRect, "", new Vector2(oldX, oldY));
        
        if (EditorGUI.EndChangeCheck())
        {
            xProp.floatValue = newValue.x;
            yProp.floatValue = newValue.y;
        }
        
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }
}
```

Result: A `Vector2` field in the Inspector will take two rows: the top shows the label with "(direction)", the bottom shows the X and Y fields.

---

## 🧩 2. Custom Editor — full Inspector overhaul for a component
### When to use:
You want to completely redesign the Inspector for a specific component (MonoBehaviour). 
For example, add buttons, tabs, warnings, hide unnecessary fields, or dynamically change property visibility.

### Basic principle:
Your class inherits from `Editor`. The `[CustomEditor(typeof(MyComponent))]` attribute binds it. Override `OnInspectorGUI()` — this is where you draw the entire interface.

### 🧪 Example: Inspector for a player component with a test jump button
```csharp
// File: Assets/Scripts/Player.cs (a regular MonoBehaviour)
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public bool isGrounded = true;
}

// File: Assets/Scripts/Editor/PlayerEditor.cs
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Player player = (Player)target;
        
        EditorGUILayout.LabelField("⚙️ Main Settings", EditorStyles.boldLabel);
        player.speed = EditorGUILayout.FloatField("Speed", player.speed);
        player.jumpForce = EditorGUILayout.FloatField("Jump Force", player.jumpForce);
        
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Is Grounded", player.isGrounded);
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("🏃‍♂️ Test Jump (in Editor)"))
        {
            Debug.Log($"{player.name} would jump with force {player.jumpForce}");
        }
        
        if (Mathf.Approximately(player.jumpForce, 0f))
        {
            EditorGUILayout.HelpBox("Jump Force is 0 — the character won't be able to jump!", MessageType.Warning);
        }
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
```

### Result:
- The speed and jump force fields are editable.
- The `isGrounded` field is read‑only.
- A button appears that logs to the console.
- If `jumpForce == 0`, a yellow warning appears.

---

## 🧰 3. Additional Editor API features

| Feature | Description |
|---------------------------|---------------------------------------|
| `EditorGUILayout.PropertyField()` | Draw a standard field from a SerializedProperty |
| `EditorGUILayout.Slider()` | Slider for float with min/max |
| `EditorGUILayout.ObjectField()` | Field for assigning a reference to another object/asset |
| `EditorGUILayout.EnumPopup()` | Dropdown list for an enum |
| `EditorGUILayout.HelpBox()` | Info/warning/error message box |
| `GUILayout.Button()` + `SceneView` API | Draw buttons in the Scene view (e.g., "Create enemy here") |
| `[InitializeOnLoad]` | Static constructor that runs when the editor starts |
| `EditorApplication.playModeStateChanged` | React to entering/exiting Play Mode |

---

## 🧠 Best Practices
1. Always check `EditorGUI.changed` and call `EditorUtility.SetDirty()` — otherwise Unity may not save changes.
2. Use `SerializedProperty` instead of direct field access — this provides Undo/Redo support and multi‑selection.
3. Never put Editor code in folders that end up in the build (only in `Editor`).
4. Don't forget about height (`GetPropertyHeight`) in PropertyDrawer if your field occupies more than one line.

---

## 🔗 Where to go next
- Unity official docs: <a href="https://docs.unity3d.com/Manual/editor-CustomEditors.html">Custom Editors</a>
- <a href="https://docs.unity3d.com/ScriptReference/PropertyDrawer.html">PropertyDrawer</a>

---

### ⭐ If this project was useful, put a star on GitHub!
