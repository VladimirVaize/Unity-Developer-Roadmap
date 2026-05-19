# 🧰 Editor Window: Creating Custom Windows in Unity Editor

> [!Note]
> This README focuses on extending Unity Editor capabilities. You will learn how to create custom windows (Editor Windows),
> use EditorGUILayout for automatic layout, and build visual tools for designers (and other team members) so they can configure the game without diving into code.

> [!Important]
> All editor script code must be placed inside an `Editor` folder (e.g., `Assets/Editor/`). Scripts in this folder are not included in the final game build.

---

## 1. Creating a Custom Window (EditorWindow)
### Purpose:
Unity's standard windows (Inspector, Project, Hierarchy) don't always cover your game's specific needs. Creating your own window allows you to:
- Group frequently used settings (level balance, enemy parameters, dialogues).
- Automate repetitive tasks (level generation, asset importing).
- Provide designers/artists with a simple interface without needing to manually edit prefabs.

### How to use:
1. Create a C# script inside the `Editor` folder (e.g., `MyToolsWindow.cs`).
2. Inherit your class from `EditorWindow`.
3. Add a static method with the `[MenuItem("Tools/My Window")]` attribute — this adds an item to Unity's top menu.
4. Inside that method, call `GetWindow<YourWindowType>()`.
5. Override the `OnGUI()` method — this is where you draw the window's interface.

### Example:
```csharp
using UnityEditor;
using UnityEngine;

public class MyToolsWindow : EditorWindow
{
    [MenuItem("Tools/My Simple Tool")]
    public static void ShowWindow()
    {
        GetWindow<MyToolsWindow>("My Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Hello, this is my window!", EditorStyles.boldLabel);
        if (GUILayout.Button("Click me"))
        {
            Debug.Log("Button clicked!");
        }
    }
}
```
After saving, you will see `Tools → My Simple Tool` in the menu — click it to open the window.

---

## 2. EditorGUILayout — Automatic Layout
### Purpose:
`EditorGUILayout` is a set of methods for automatically placing UI elements one below another (vertical) or in a row (horizontal). 
It's similar to `GUILayout` but tailored for the editor, with support for serialized data fields (class fields that persist between Unity restarts).

### Basic methods (how to use):
- `EditorGUILayout.LabelField("Text")` — simple label.
- `EditorGUILayout.TextField("Name:", variable)` — string field.
- `EditorGUILayout.IntField("Count:", variable)` — integer field.
- `EditorGUILayout.Slider("Volume:", 0f, 1f)` — slider.
- `EditorGUILayout.ObjectField("Object:", obj, typeof(GameObject), true)` — field for dragging assets/objects.
- `EditorGUILayout.Space()` — vertical spacing.
- `EditorGUILayout.BeginHorizontal()` / `EndHorizontal()` — place elements in a row.

### Example (enemy settings window):
```csharp
private string enemyName = "Goblin";
private int enemyHealth = 100;
private float enemySpeed = 3.5f;
private GameObject enemyPrefab;

private void OnGUI()
{
    EditorGUILayout.LabelField("Enemy Parameters", EditorStyles.boldLabel);
    
    enemyName = EditorGUILayout.TextField("Enemy Name:", enemyName);
    enemyHealth = EditorGUILayout.IntField("Health:", enemyHealth);
    enemySpeed = EditorGUILayout.Slider("Speed:", enemySpeed, 0f, 10f);
    enemyPrefab = (GameObject)EditorGUILayout.ObjectField("Enemy Prefab:", enemyPrefab, typeof(GameObject), false);
    
    if (GUILayout.Button("Apply to Selected Enemy"))
    {
        Debug.Log($"Applied to {enemyName}");
    }
}
```

---

## 3. Visual Tools for Designers
### Purpose:
Designers, artists, and game designers often don't write code. 
Visual tools allow them to configure the game through user-friendly interfaces 
(buttons, fields, sliders, color pickers) directly inside the editor.

Useful techniques for designers:

### 🔸 Color fields
```csharp
private Color gizmoColor = Color.red;
gizmoColor = EditorGUILayout.ColorField("Area Color:", gizmoColor);
```

### 🔸 Dropdown lists (Enum)
```csharp
public enum Difficulty { Easy, Medium, Hard }
private Difficulty currentDifficulty = Difficulty.Medium;
currentDifficulty = (Difficulty)EditorGUILayout.EnumPopup("Difficulty:", currentDifficulty);
```

### 🔸 Toggle (on/off)
```csharp
private bool enableSpawners = true;
enableSpawners = EditorGUILayout.Toggle("Enable Spawners:", enableSpawners);
```

### 🔸 Buttons for actions like "Clear all", "Generate level", "Give health to all"
```csharp
if (GUILayout.Button("Spawn 10 enemies in scene"))
{
    for (int i = 0; i < 10; i++)
        Instantiate(enemyPrefab, Random.insideUnitSphere * 10f, Quaternion.identity);
}
```

### 🔸 Saving data between sessions (EditorPrefs or SerializedObject)
```csharp
// Save value
EditorPrefs.SetFloat("MyVolume", volume);
// Load value
volume = EditorPrefs.GetFloat("MyVolume", 0.5f);
```

---

## 4. Complete Example: "Item Spawn Tool Window"
```csharp
using UnityEditor;
using UnityEngine;

public class SpawnToolWindow : EditorWindow
{
    private GameObject itemPrefab;
    private int spawnCount = 5;
    private float spawnRadius = 10f;
    private Color gizmoColor = Color.green;

    [MenuItem("Tools/Item Spawn Window")]
    public static void ShowWindow() => GetWindow<SpawnToolWindow>("Spawn Items");

    private void OnEnable()
    {
        spawnCount = EditorPrefs.GetInt("SpawnCount", 5);
        spawnRadius = EditorPrefs.GetFloat("SpawnRadius", 10f);
    }

    private void OnDisable()
    {
        EditorPrefs.SetInt("SpawnCount", spawnCount);
        EditorPrefs.SetFloat("SpawnRadius", spawnRadius);
    }

    private void OnGUI()
    {
        GUILayout.Label("⚙️ Item Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        itemPrefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab:", itemPrefab, typeof(GameObject), false);
        spawnCount = EditorGUILayout.IntSlider("Item Count:", spawnCount, 1, 50);
        spawnRadius = EditorGUILayout.Slider("Spawn Radius:", spawnRadius, 1f, 30f);
        gizmoColor = EditorGUILayout.ColorField("Visualization Color:", gizmoColor);

        EditorGUILayout.Space();

        if (itemPrefab == null)
        {
            EditorGUILayout.HelpBox("Drag an item prefab into the field above", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("🎲 Random Spawn"))
        {
            SpawnItemsRandomly();
        }

        if (GUILayout.Button("🗑️ Delete All Items in Scene"))
        {
            DeleteAllSpawnedItems();
        }
    }

    private void SpawnItemsRandomly()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            Instantiate(itemPrefab, randomPos, Quaternion.identity);
        }
        Debug.Log($"Spawned {spawnCount} items");
    }

    private void DeleteAllSpawnedItems()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in items)
            DestroyImmediate(item);
        Debug.Log($"Deleted {items.Length} items");
    }

    private void OnSceneGUI()
    {
        Handles.color = gizmoColor;
        Handles.DrawWireDisc(Vector3.zero, Vector3.up, spawnRadius);
    }
}
```

---

## 🔁 How designers use such windows
- Open via `Tools → ...`
- Drag prefabs, adjust sliders, click buttons.
- See changes instantly in the scene (spawning, deleting, parameter tweaks).
- Data persists between Unity restarts.

---

### ⭐ If this project was useful, put a star on GitHub!
