# 🎲 Instantiate and Destroy: Creating and Destroying Objects at Runtime

> [!NOTE]
> In static scenes, all objects are placed manually before pressing Play.
> But real games constantly create and remove objects: bullets, enemies, debris, explosion effects, collectible coins.
> In Unity, two methods handle this: `Instantiate()` (creation) and `Destroy()` (destruction).

---

## 📦 Instantiate — creating an object
### Purpose:
Create a copy of an existing GameObject (usually a prefab) during gameplay. For example, spawning bullets, enemies, or particles.

### How to use:
```csharp
public GameObject bulletPrefab; // drag your bullet prefab here in the Inspector
public Transform firePoint;     // where the bullet comes from

void Shoot()
{
    Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
}
```

### Variations of `Instantiate`:
```csharp
// 1. Prefab only (appears at world center with Quaternion.identity)
Instantiate(enemyPrefab);

// 2. Prefab + position + rotation
Instantiate(enemyPrefab, new Vector3(0, 5, 0), Quaternion.identity);

// 3. Full version: prefab, position, rotation, and parent transform
GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, parentTransform);
```

> 💡 Tip: Save the result of Instantiate to a variable to control the created object later (e.g., change its speed or destroy it after a few seconds).

### Example:
You're making a shooter. On each mouse click, `Shoot()` is called, and a new bullet (a copy of the `Bullet` prefab) appears in the scene. Each bullet flies independently.

---

## 💥 Destroy — destroying an object
### Purpose:
Remove an object from the scene. This frees memory and resources. You can destroy the object the script is attached to, or another object (e.g., an enemy when hit by a bullet).

### How to use:
```csharp
// Destroy the object this script is attached to
Destroy(gameObject);

// Destroy another object
Destroy(otherGameObject);

// Destroy with a delay in seconds (very useful for effects)
Destroy(explosionEffect, 2f); // explosion disappears after 2 seconds
```

### Example:
A bullet hits an enemy. In the bullet's script, you write:

```csharp
void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.tag == "Enemy")
    {
        Destroy(collision.gameObject); // destroy the enemy
        Destroy(gameObject);           // destroy the bullet itself
    }
}
```

---

## 🔁 Common usage patterns

| Situation | What to do |
|----------------------------|--------------------------------------|
| 🎯 Shooting | `Instantiate(bulletPrefab)` → bullet flies |
| 💀 Bullet hits enemy | `Destroy(enemy)` + `Destroy(bullet)` | 
| 🧨 Enemy dies — leave an explosion | `Instantiate(explosionPrefab, enemy.position, ...)` → then `Destroy(enemy)` |
| 🪙 Coin collected | `Destroy(coin)` and increase score |
| 🚀 Spawn a wave of enemies | `for` loop with `Instantiate(enemyPrefab, randomPosition, ...)` |
| ⏱ Remove an effect after time | `Destroy(particleEffect, 1.5f)` |

---

## ⚠️ Important notes
- `Destroy(gameObject)` does not happen instantly in the same frame (usually at the end of the frame). For most tasks, this doesn't matter.
- Do not try to call `Destroy` on an already destroyed object — it will cause an error. Check with `if (object != null)`.
- For immediate removal from memory, there is `DestroyImmediate()`, but it is rarely used and can be dangerous.
- If you need to temporarily hide an object instead of destroying it, use `gameObject.SetActive(false)` (this is faster than Destroy and Instantiate again).

---

## 🧪 Simple complete example (enemy spawner)
```csharp
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        
        // Automatically destroy the enemy after 5 seconds if it doesn't die earlier
        Destroy(newEnemy, 5f);
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
