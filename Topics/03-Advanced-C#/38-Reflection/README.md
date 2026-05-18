# 🔍 Reflection in Unity: Introspection and Metaprogramming
This material covers key reflection and attribute mechanisms in Unity: `System.Reflection`, 
finding components with interfaces via `GetComponents`, `[RequireComponent]` and `[ContextMenu]` attributes. 
These tools allow you to write more flexible, automated, and cleaner code.

---

## 📖 1. System.Reflection Namespace
### 🎯 Purpose:
Reflection allows you to inspect your code's structure at runtime: get information about types (classes), 
their fields, methods, properties, attributes — and even invoke methods or modify `private` fields. 
It's a powerful tool for creating editor tools, serialization systems, modular architectures, and more.

### ⚙️ How to use:
```csharp
using System.Reflection;
using UnityEngine;

public class ReflectionExample : MonoBehaviour
{
    private int secretHealth = 100;

    void Start()
    {
        // Get the current object's type
        Type type = this.GetType();

        // Get a private field by name
        FieldInfo healthField = type.GetField("secretHealth", 
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Read the value
        int currentHealth = (int)healthField.GetValue(this);
        Debug.Log($"Health: {currentHealth}");

        // Change the value
        healthField.SetValue(this, 50);
        Debug.Log($"New health: {(int)healthField.GetValue(this)}");
    }
}
```

### 📌 Real-life example:
You're writing a Save System that automatically finds all fields marked with `[SaveField]` (even private ones) and saves them to JSON, without requiring manual copying of each field.

---

## 🔌 2. GetComponents with Interfaces
### 🎯 Purpose:
The standard `GetComponent<T>()` doesn't work directly with interfaces. 
However, you can get all components on an object and filter those that implement the desired interface. 
This is useful for systems where different objects behave differently but share a common contract (e.g., all can take damage).

### ⚙️ How to use:
```csharp
public interface IDamageable
{
    void TakeDamage(int amount);
}

public class Player : MonoBehaviour, IDamageable
{
    public void TakeDamage(int amount) => Debug.Log($"Player took {amount} damage");
}

public class Enemy : MonoBehaviour, IDamageable
{
    public void TakeDamage(int amount) => Debug.Log($"Enemy took {amount} damage");
}

public class DamageDealer : MonoBehaviour
{
    void DealDamageToAll(GameObject target)
    {
        // Get all components on target that implement IDamageable
        IDamageable[] damageables = target.GetComponents<IDamageable>();
        
        foreach (var d in damageables)
        {
            d.TakeDamage(10);
        }
    }
}
```

### 📌 Real-life example:
You have an exploding grenade. Within the blast radius are the player, enemies, destructible crates, and doors. 
All implement `IDamageable`. The grenade finds all components with this interface in the radius and calls `TakeDamage()`.

---

## 🔧 3. [RequireComponent] Attribute
### 🎯 Purpose:
This attribute ensures that when you add your script to a GameObject, Unity automatically adds the specified components if they don't already exist. 
This prevents "Missing Component" errors at runtime.

### ⚙️ How to use:
```csharp
using UnityEngine;

// Require Rigidbody and Collider on the object
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        // Now safe to get components — they are guaranteed to exist
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
}
```

### 📌 Real-life example:
A `HealthSystem` script always requires an `Animator` to play death animations and a `Collider` to handle hits. 
The `[RequireComponent]` attribute automatically adds them when the object is created.

---

## 🖱️ 4. [ContextMenu] Attribute
### 🎯 Purpose:
Allows you to invoke a method of your script directly from the component's context menu in the Unity Editor (right-click on the component header in the Inspector). 
Ideal for editor utilities, test actions, and scene setup.

### ⚙️ How to use:
```csharp
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemyCount = 10;

    [ContextMenu("Spawn Enemies")]
    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Instantiate(enemyPrefab, Random.insideUnitSphere * 10, Quaternion.identity);
        }
        Debug.Log($"{enemyCount} enemies spawned!");
    }

    [ContextMenu("Clear Enemies")]
    void ClearEnemies()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies) DestroyImmediate(e);
        Debug.Log("All enemies cleared");
    }
}
```

### 📌 Real-life example:
You have a `DatabaseInitializer` script. Via `[ContextMenu("Reset Database")]` you can, with one click, clear and fill PlayerPrefs or a ScriptableObject with test data.

---

## 🧠 Combining all techniques in one example
Imagine a damage system with automatic addition of required components:
```csharp
[RequireComponent(typeof(Collider), typeof(Animator))]
public class DestructibleObject : MonoBehaviour, IDamageable
{
    [ContextMenu("Debug Health")]
    void DebugHealth() => Debug.Log($"Health: {health}");

    private int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    void Die() => Destroy(gameObject);
}

// In another script, use the interface:
public class Explosion : MonoBehaviour
{
    void Explode()
    {
        var damageables = FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>();
        foreach (var d in damageables) d.TakeDamage(50);
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
