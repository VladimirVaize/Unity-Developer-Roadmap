# 🧪 Practical Task: Optimizing Shooting with Object Pooling

## 🎯 Goal of the task
Learn to apply Object Pooling to improve game performance. You will create a shooting system where bullets are not created and destroyed, but reused.

---

## 📝 Task Description
You have a simple top‑down shooter game:
- The player controls a character (left mouse click or Space to shoot).
- Each shot creates a bullet (`Bullet` prefab) that moves forward.
- The bullet is destroyed when it hits a wall or after 2 seconds.

### 🔴 Current problem (bad implementation):
```csharp
public class BadShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ❌ Allocation every shot
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = Vector3.forward * 20f;
            Destroy(bullet, 2f); // ❌ Will trigger GC
        }
    }
}
```
Problem: 60 shots → 60 allocations → 60 destructions → the garbage collector will constantly freeze the game.

---

## ✅ Your Task
Rewrite the shooting system using an object pool.

### Requirements:
1. Create a `BulletPool` script that:
   - Pre‑creates 30 bullets (in `Awake` or `Start`).
   - Stores them in a `Queue<GameObject>` or uses `UnityEngine.Pool`.
   - Provides a `GetBullet()` method (activates the bullet, takes it from the pool).
   - Provides a `ReturnBullet(GameObject bullet)` method (deactivates it, returns to the pool).
  
2. Rewrite the player script `GoodShooter`:
   - Instead of `Instantiate`, calls `bulletPool.GetBullet()`.
   - Does NOT call `Destroy` — the bullet returns to the pool on its own.
  
3. Write the `Bullet` script:
   - Moves the bullet forward (using `Rigidbody` or `Transform`).
   - On collision (`OnCollisionEnter`), calls `ReturnBullet`.
   - Automatically returns to the pool after 2 seconds (even without collision).
  
4. Add debug output (for example, in the bullet's `Update`) to show whether the bullet is active.

---

## 📤 Expected result:
- The Hierarchy window should always contain exactly 30 bullets.
- When shooting, one of the disabled bullets becomes active and flies.
- After impact or timeout, the bullet is disabled and returns to the queue.
- In Profiler → GC Alloc, there should be NO memory allocation (0 bytes) during shooting.

---

## 🧠 Bonus (star) task:
- Add automatic pool expansion: if there are no bullets left, create a new one and add it to the pool.
- Make the bullet change color when activated and revert when deactivated.
- Use the official `UnityEngine.Pool.ObjectPool<T>` instead of a manual `Queue`.

---

### ⭐ If this project was useful, put a star on GitHub!
