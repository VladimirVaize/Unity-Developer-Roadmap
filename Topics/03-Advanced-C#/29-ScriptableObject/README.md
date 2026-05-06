# 🧠 ScriptableObject in Unity: Data Storage, Configuration, Replacing Singletons

> [!Note]
> `ScriptableObject` is a special type of class in Unity that allows you to store large amounts of data independently of GameObjects.
> Unlike `MonoBehaviour`, it is not tied to a scene and does not require an object in the hierarchy.

---

## 🎯 Key Features

### 1. 📦 Data Storage
You can create a `ScriptableObject` as an asset file in the `Project` window and store in it:
- numbers, strings, booleans
- lists and arrays
- references to other assets (textures, sounds, models, prefabs)
- custom classes and structs

#### Example:
Create a `PlayerStats.asset` containing:<br>
`maxHealth = 100`, `moveSpeed = 5.5`, `jumpForce = 12`, `weaponPrefab` (reference to a sword prefab).

### 2. ⚙️ Configuration (Settings)
The same `ScriptableObject` can be used as a configuration file for different scene objects.<br>
This is convenient when you need multiple enemy types, weapons, levels, or graphic settings.

#### Example:
You create an `EnemyConfig.asset` with fields: `health`, `damage`, `color`, `speed`.<br>
Now you can create 3 different files:
- `GoblinConfig.asset` (health=30, damage=5)
- `OrcConfig.asset` (health=80, damage=15)
- `TrollConfig.asset` (health=150, damage=25)

Then in the enemy component (`MonoBehaviour`), you simply reference the appropriate `EnemyConfig` and read data from it.

### 3. 🔁 Replacing Singletons
Many developers use singletons (`public static GameManager Instance`) for global data access.<br>
Drawbacks of singletons:
- hard to test
- tied to a scene
- lost when reloading a scene
- tight coupling between systems

Solution: `ScriptableObject` as a global service (or event/data manager).<br>
You create a `ScriptableObject` instance in the project and reference it from any script via a `public` field (serialization).

This provides:
- loose coupling
- ability to swap implementations (e.g., replace config for testing)
- data persistence between scenes (if you don't reset it)

Example global storage:
```csharp
// GameDataSO.cs
[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameDataSO : ScriptableObject
{
    public int score;
    public int lives;
    public void AddScore(int value) { score += value; }
}
```

Any script can access this data through a reference to `GameDataSO` assigned in the Inspector.<br>
No `Instance` needed!

---

## 🛠️ How to Create a ScriptableObject
1. Create a C# script inherited from `ScriptableObject`:
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public int value;
    public Sprite icon;
}
```
2. In the editor: right-click in the `Project` window → `Create` → `Game/Item` → a `.asset` file appears.
3. Fill in the fields in the Inspector.
4. Use in any `MonoBehaviour`:
```csharp
public class ItemPicker : MonoBehaviour
{
    public ItemSO itemData; // drag the asset in the Inspector
    void Start()
    {
        Debug.Log(itemData.itemName);
    }
}
```

---

## ✅ Advantages Over Other Approaches

| Approach | Pros | Cons |
|-------------------|-----------------------|----------------------|
| Fields in `MonoBehaviour` | Simple | Data copied to each object, hard to change globally |
| Singleton | Global access | Tight coupling, testing issues |
| `ScriptableObject` | Asset-based config, replaces singletons, loose coupling | Manual state reset required on game restart |

---

## 🧪 Tips & Tricks
- 🧹 Reset state: After game ends, `ScriptableObject` field values (e.g., `score`) remain changed because the asset saves to disk.
  To reset, create a `Reset()` method and call it when the game starts.
- 🧩 Events: `ScriptableObject` can serve as an event bus (Event Channel) — e.g., `VoidEventSO`, `IntEventSO`.
  Different systems subscribe to the event without knowing each other.
- 🧪 Testing: You can create a separate mock `ScriptableObject` for tests without changing production code.

---

### ⭐ If this project was useful, put a star on GitHub!
