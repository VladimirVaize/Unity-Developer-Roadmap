# ⚙️ OnValidate: Automatically Configuring Presets in the Unity Editor

`OnValidate` is a special method in Unity that is automatically called only in the editor (not in game builds). 
It triggers whenever the script is loaded or when you change the values of its fields in the Inspector window. 
This makes it an ideal tool for automatically validating, correcting, and configuring object parameters during development.

---

## 🎯 Why use `OnValidate`?
- ✅ Automatic value correction — e.g., preventing a radius from becoming negative.
- ✅ Updating child objects — changing a material color on all parts of a prefab at once.
- ✅ Preset validation — ensuring object settings follow the rules (e.g., `MaxHealth` > 0).
- ✅ Visual debugging — updating Scene View representation when parameters change.
- ✅ Preset configuration — when applying a preset, `OnValidate` can recalibrate other fields.

> [!Important]
> `OnValidate` is not called at runtime (in game builds). It works ONLY in the Unity editor.

---

## 🧠 Syntax and Usage
Add the method to any class inheriting from `MonoBehaviour`:
```csharp
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float range = 5f;

    private void OnValidate()
    {
        // Automatic value adjustment
        if (damage < 1)
            damage = 1;

        if (range < 0.5f)
            range = 0.5f;

        Debug.Log($"[OnValidate] Weapon configured: damage {damage}, range {range}");
    }
}
```

Now, if you change `damage` to 0 in the Inspector — `OnValidate` immediately resets it to 1.

---

## 🔧 Examples of Use
### 1. Clamping values
```csharp
private void OnValidate()
{
    health = Mathf.Max(1, health);
    speed = Mathf.Clamp(speed, 0f, 20f);
}
```

### 2. Auto-updating child objects
```csharp
private void OnValidate()
{
    var renderers = GetComponentsInChildren<Renderer>();
    foreach (var rend in renderers)
        rend.sharedMaterial.color = teamColor;
}
```

### 3. Keeping two fields in sync
```csharp
private void OnValidate()
{
    if (maxHealth < currentHealth)
        currentHealth = maxHealth;
}
```

### 4. Working with Presets
If a user applies a preset to your component, `OnValidate` will automatically adjust dependent fields.
```csharp
private void OnValidate()
{
    attackSpeed = Mathf.Max(0.2f, attackSpeed);
    attackCooldown = 1f / attackSpeed; // auto-update
}
```

---

## 🧩 When NOT to use `OnValidate`
- ❌ In game builds (useless — won't be called).
- ❌ For heavy operations (creating/deleting objects, scene searching) — may slow down the editor.
- ❌ For logic that must run at runtime — use `Awake`, `Start`, `Update`.

---

## 📌 Summary

| Feature | Description |
|---------------|---------------------------------|
| 🟢 Called | Only in the Unity editor |
| 🔁 Triggers | Script load, Inspector field changes, preset application |
| 🎯 Purpose | Automatic validation, adjustment, synchronization |
| ⚠️ Limitations | Not called in builds, avoid heavy operations |

---

### ⭐ If this project was useful, put a star on GitHub!
