# 🎯 Practical Task: Bullet and Enemy Spawning via Object Pool

## 📋 Task Description
You need to create an Object Pool system for two object types: bullets and minor enemies. 
The system should demonstrate the advantages of pooling over `Instantiate`/`Destroy`, as well as correctly reset object state when returning to the pool.

---

## 🧱 Task Structure
### 📁 Part 1: Bullet Pool
1. Create a `BulletPool` script (MonoBehaviour) that:
   - Has `[SerializeField] private GameObject bulletPrefab`
   - Has `[SerializeField] private int poolSize = 30`
   - Uses `Queue<GameObject>` to store bullets
   - In `Awake()` fills the pool with inactive prefabs
   - Has a method `GetBullet(Vector3 position, Quaternion rotation)` — activates the bullet, sets position, and returns it
   - Has a method `ReturnBullet(GameObject bullet)` — deactivates the bullet and returns it to the queue
  
### 📁 Part 2: Enemy Pool
1. Create an `Enemy` script with:
   - `public int health = 3`
   - Method `TakeDamage(int damage)` — reduces health, returns to pool when health reaches 0
   - Method `ResetState()` — resets health to 3, stops all coroutines, resets animations
   - Method `Initialize(EnemyPool pool, Vector3 position)` — remembers the pool owner and sets position
  
2. Create an `EnemyPool` script (similar to BulletPool), but:
   - Returns `Enemy` instead of `GameObject`
   - Calls `Initialize()` when getting an enemy
   - Calls `ResetState()` when returning an enemy
  
### 📁 Part 3: Spawner Script
Create a `Spawner` script that:
- Has references to `BulletPool` and `EnemyPool`
- In `Update()`:
  - On `Space` press — spawns a bullet from the pool at the spawner's position with a random direction
  - On `E` press — spawns an enemy from the pool at a random position around the spawner
 
- Logs the number of active objects to the console (using `FindObjectsOfType` to demonstrate the difference with pooling)

### 📁 Part 4: Performance Demonstration
Add a button in the Inspector via `[ContextMenu]` to `Spawner`:
- `[ContextMenu("Spawn 1000 Bullets With Instantiate")]` — creates 1000 bullets via `Instantiate` and destroys them via `Destroy` after 1 second (measure execution time)
- `[ContextMenu("Spawn 1000 Bullets With Pool")]` — creates 1000 bullets via the pool and returns them (measure execution time)
- Print the execution time of both methods to the console for comparison

---

## ✅ Completion Criteria
1. ✅ Pool is created once in `Awake()` (no `Instantiate` during gameplay, except for pool expansion)
2. ✅ When returning to the pool, the enemy state is fully reset (health, position, active effects)
3. ✅ Bullet automatically returns to the pool via `Invoke` or `Coroutine` after 2 seconds or on trigger collision
4. ✅ Enemy returns to the pool when `health <= 0`
5. ✅ Use `[ContextMenu]` to demonstrate performance comparison

---

## 🧩 Bonus Task (⭐⭐)
Implement an expandable pool — if the queue is empty, 
the pool automatically creates a new object (with a warning log) and adds it to the pool upon return. 
Also add a `PreWarm(int additionalCount)` method to pre-expand the pool.

---

## 🧪 Expected Result
After completing the task:
1. Pressing `Space` creates a bullet without delays or freezes — it disappears after 2 seconds and is ready for reuse
2. Pressing `E` creates an enemy — after 3 hits, it disappears and reappears with full health
3. Calling the `[ContextMenu]` methods will show a performance difference: `Instantiate`/`Destroy` will cause noticeable freezes and GC pressure, while the pool will run smoothly
4. The console shows that objects are being reused (the same `GameObject` instance is used many times)

---

### ⭐ If this project was useful, put a star on GitHub!
