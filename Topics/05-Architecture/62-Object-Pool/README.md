# 🔄 Object Pool Pattern in Unity
This material covers the Object Pool concept, its implementation for particles, enemies, 
and bullets, as well as key differences from the standard `Instantiate` / `Destroy` approach.

---

## 📖 1. What is the Object Pool Pattern?
### 🎯 Purpose:
The Object Pool is a creational design pattern that reuses objects instead of destroying them. 
Ready-made objects are stored in a "pool" (queue, list, stack), handed out on request, and returned after use. 
This drastically reduces garbage collector (GC) pressure and improves performance.

### ⚙️ Core idea:
- Instead of: `Instantiate()` → use → `Destroy()`
- We do: Take from pool → use → return to pool

---

## 🚀 2. Difference from Instantiate / Destroy

| Aspect | Instantiate / Destroy | Object Pool |
| --- | --- | --- |
| Memory allocation | New allocation each time | Only when creating the pool |
| Garbage Collector (GC) | High load, freezes | Almost zero load |
| Speed | Slow (especially Destroy) | Fast (activate/deactivate) |
| When to use | Rare objects (UI, managers) | Frequently created objects (bullets, enemies, particles) |
| Code complexity | Low | Medium (need to manage the pool) |

### Example problem with Instantiate/Destroy:
In a shooter, the player fires 10 bullets per second. After one minute, 600 objects are created and destroyed. Each destruction causes GC freezes. The game starts to lag.

### Solution (Object Pool):
Pre-create 30 bullets. Each bullet, after leaving the screen, is not destroyed but returned to the pool and becomes available for the next shot. GC is not triggered.

---

## 🧩 3. Basic Pool Implementation in Unity
### Simple bullet pool
```csharp
using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 20;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    
    void Awake()
    {
        // Pre-create the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false); // Deactivate
            pool.Enqueue(obj);    // Add to queue
        }
    }
    
    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Optionally expand the pool
            Debug.LogWarning("Pool is empty! Expanding...");
            GameObject obj = Instantiate(prefab);
            return obj;
        }
    }
    
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

### Using the pool
```csharp
public class Shooter : MonoBehaviour
{
    [SerializeField] private ObjectPool bulletPool;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bullet = bulletPool.GetObject();
            bullet.transform.position = transform.position;
            // Configure bullet (speed, damage)
        }
    }
}
```

```csharp
public class Bullet : MonoBehaviour
{
    [SerializeField] private ObjectPool ownerPool;
    [SerializeField] private float lifeTime = 2f;
    
    void OnEnable()
    {
        Invoke(nameof(ReturnToPool), lifeTime);
    }
    
    void ReturnToPool()
    {
        ownerPool.ReturnObject(gameObject);
    }
}
```

---

## 💥 4. Implementation for Particles
Special aspect: Particles don't need to be manually returned — they can return automatically after the animation/particle system finishes.
```csharp
public class ParticlePool : MonoBehaviour
{
    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField] private int poolSize = 10;
    
    private Queue<ParticleSystem> pool = new Queue<ParticleSystem>();
    
    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem ps = Instantiate(particlePrefab);
            ps.Stop();
            pool.Enqueue(ps);
        }
    }
    
    public ParticleSystem PlayAt(Vector3 position)
    {
        ParticleSystem ps = pool.Dequeue();
        ps.transform.position = position;
        ps.Play();
        
        // Auto-return via coroutine
        StartCoroutine(ReturnAfterPlay(ps));
        return ps;
    }
    
    private System.Collections.IEnumerator ReturnAfterPlay(ParticleSystem ps)
    {
        yield return new WaitWhile(() => ps.isPlaying);
        ps.Stop();
        pool.Enqueue(ps);
    }
}
```

---

## 👾 5. Implementation for Enemies (more complex)
Special aspect: Enemies have state (health, position). When returning to the pool, you must reset the state.
```csharp
public class Enemy : MonoBehaviour
{
    public int health = 100;
    private EnemyPool pool;
    
    public void Initialize(EnemyPool ownerPool)
    {
        pool = ownerPool;
        health = 100;
        // Reset other parameters
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            pool.ReturnEnemy(this);
    }
    
    void OnDisable()
    {
        // Reset animations, stop coroutines
    }
}

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int poolSize = 15;
    
    private Queue<Enemy> pool = new Queue<Enemy>();
    
    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            Enemy enemy = Instantiate(enemyPrefab);
            enemy.gameObject.SetActive(false);
            pool.Enqueue(enemy);
        }
    }
    
    public Enemy GetEnemy(Vector3 position)
    {
        Enemy enemy = pool.Dequeue();
        enemy.transform.position = position;
        enemy.gameObject.SetActive(true);
        enemy.Initialize(this);
        return enemy;
    }
    
    public void ReturnEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        pool.Enqueue(enemy);
    }
}
```

---

## 🔧 6. Generic Pool
For code reuse, you can create a generic pool:
```csharp
public class GenericPool<T> where T : Component
{
    private T prefab;
    private Queue<T> pool = new Queue<T>();
    private Transform parent;
    
    public GenericPool(T prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        
        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    
    public T Get()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        return Object.Instantiate(prefab, parent);
    }
    
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

---

## 📊 7. When to Use Object Pool vs Not?

| ✅ Use Object Pool | ❌ Don't Use Object Pool |
| --- | --- |
| Frequently created objects (bullets, debris) | Rare objects (bosses, menus) |
| Mobile games (sensitive to GC) | Objects with unique state | 
| Particle systems (many short effects) | Objects created 1-2 times |
| Enemies in roguelike / shooters | Scene loading (Addressables are better) |

---

### ⭐ If this project was useful, put a star on GitHub!
