# 📦 Serialization in Unity: Controlling Field Visibility in the Inspector
Serialization is the process of converting object data into a format that can be saved (e.g., to a scene or prefab) and restored later. 
Unity automatically serializes public fields of most types, displaying them in the Inspector window. However, this is often not enough. 
In this material, we cover three key attributes for fine-tuning serialization: `[SerializeField]`, `[HideInInspector]`, and `[System.Serializable]`.

## 🎯 Why control serialization?
- Hide internal variables that should not be edited in the Inspector.
- Show `private` or `protected` fields in the Inspector without breaking encapsulation.
- Work with complex data types (structs, classes) that Unity does not serialize by default.

---

## 1. `[SerializeField]` — Show a private field
### Purpose:
By default, Unity does not serialize private (`private`, `protected`, `internal`) fields. 
The `[SerializeField]` attribute forces Unity to show such a field in the Inspector and save its value without making the field public. 
This helps maintain encapsulation.

### How to use:
- Add `[SerializeField]` before a private field declaration.
- The field will appear in the Inspector, but other scripts cannot access it directly (without special methods).

### 📌 Example:
```csharp
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibilityDuration = 1.5f;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }
}
```

> In the Inspector, you will see the fields `Max Health` and `Invincibility Duration` and can change them. Meanwhile, another script cannot accidentally modify `maxHealth` directly.

---

## 2. `[HideInInspector]` — Hide a public field
### Purpose:
The reverse situation: you have a public field that Unity automatically shows in the Inspector, but you do not want it to appear or be editable there. 
For example, a technical variable or reference that is set by code.

### How to use:
- Add `[HideInInspector]` before a `public` field.
- The field remains public (accessible from other scripts) but disappears from the Inspector.

### 📌 Example:
```csharp
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [HideInInspector] public Transform playerTarget; // Assigned by code, no manual editing needed
    public float attackRange = 5f;                   // Visible and editable

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
```

> `playerTarget` does not clutter the Inspector, though other scripts can read it.

---

## 3. `[System.Serializable]` — Serialize your own classes and structs
### Purpose:
Unity cannot automatically serialize ordinary classes and structs that you define. 
If you declare `public MyCustomClass data` in a MonoBehaviour, you will see nothing in the Inspector. 
The `[System.Serializable]` attribute tells Unity: 
"This type can be serialized — show its fields in the Inspector, save them in the scene/prefab."

### How to use:
- Add `[System.Serializable]` before the class or struct definition.
- Inside, use public fields or `[SerializeField]` private fields.
- Then declare a variable of that type in your script.

### 📌 Example with a struct:
```csharp
using UnityEngine;

[System.Serializable]
public struct WeaponStats
{
    public string weaponName;
    public int damage;
    public float fireRate;
}

public class PlayerCombat : MonoBehaviour
{
    public WeaponStats mainWeapon;   // Appears in the Inspector as a foldout block
    [SerializeField] private WeaponStats secondaryWeapon; // Also visible but private
}
```

### 📌 Example with a class:
```csharp
[System.Serializable]
public class InventorySlot
{
    public string itemID;
    public int quantity;
    [HideInInspector] public int internalIndex; // Fields inside a serializable class can also be hidden
}

public class Inventory : MonoBehaviour
{
    public InventorySlot[] slots; // In the Inspector, you see an array with editable elements
}
```

> [!Important]
> For classes, Unity requires them to be non-abstract, have a parameterless constructor, and contain fields of serializable types.

---

## 📊 Summary Table

| Attribute | Applied to | Effect in Inspector | Code Access |
|-------------------|-------------------|-----------------|-----------------|
| `[SerializeField]` | `private` / `protected` | ✅ Shows | ❌ No direct access |
| `[HideInInspector]` | `public` | ❌ Hides | ✅ Yes (public) |
| `[System.Serializable]` | Custom classes/structs | ✅ Allows seeing fields | Depends on field access level |

---

## ⚠️ Common mistakes
1. Forgetting `[System.Serializable]` → instead of fields, you see an empty label or nothing in the Inspector.
2. Using `[SerializeField]` on properties (with `get/set`) → Unity does not serialize them.
3. Trying to serialize `Dictionary` or `Queue` → they are not supported out-of-the-box (workarounds needed).
4. Hiding a field with `[HideInInspector]` but expecting it to load a saved value → the value is saved, but you cannot see it in the editor (which is sometimes useful).

---

### ⭐ If this project was useful, put a star on GitHub!
