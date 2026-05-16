# 🗑️ Garbage Collector (GC) in Unity: Object Pooling and Avoiding Allocations

Performance is a key aspect of any game. One of the main enemies of smooth FPS in Unity is the Garbage Collector (GC). 
In this material, we'll explore what GC is, why it causes "stutters" (freezes), and how to fight it using Object Pooling and avoiding allocations in Update.

---

## 📚 What is the Garbage Collector?
The Garbage Collector is a mechanism that automatically frees memory occupied by objects that are no longer in use.

### How it works in Unity (simplified):
1. You create an object (e.g., `new GameObject()`, an `array`, a `string`, a `list`).
2. Unity allocates memory for it on the managed heap.
3. When the object is no longer needed (no references point to it), memory is not freed immediately.
4. Periodically, GC runs, finds "garbage" and frees memory.
5. The problem: While GC is working, the game pauses (freezes) for several milliseconds or even longer.

> [!Important]
> In Unity, GC works on the main thread — so even a short pause is noticeable in dynamic scenes.

---

## 🧠 Why is GC dangerous in games?
- Spikes on the frame time graph: The game runs smoothly (60 FPS), then a sharp drop due to GC.
- On mobile devices: GC pauses can be very long (up to 100–200 ms) due to slower memory.
- In VR: Any pause causes discomfort and motion sickness.

---

# ⚡ The main rule: The fewer allocations (object creations) during gameplay — the better.
## 🎯 1. Avoiding allocations in Update
The `Update()` method is called every frame (often 60+ times per second). If you create garbage inside it, GC will trigger very frequently.

### 🔴 Bad (creates garbage every frame):
```csharp
void Update()
{
    // Creates a new string every frame
    string message = "Score: " + score;
    
    // Creates a new array every frame
    Vector3[] tempArray = new Vector3[10];
    
    // Creates a new list every frame
    List<int> tempList = new List<int>();
    
    // Boxing — int to object
    object box = currentHealth;
}
```

### ✅ Good (zero allocations):
```csharp
// Reusable field
private Vector3[] reusableArray = new Vector3[10];
private List<int> reusableList = new List<int>();

void Update()
{
    // Use StringBuilder for strings (or cache the result)
    // Or simply avoid string concatenation in UI
    
    // Reuse arrays and lists
    System.Array.Clear(reusableArray, 0, reusableArray.Length);
    reusableList.Clear();
    
    // Use struct instead of class (where possible)
    // structs are stored on the stack, not on the heap
}
```

### 📝 Common sources of allocations in Update:

| Source                    | Why allocation?                          | How to fix                                    |
| ---                       | ---                                      | ---                                           |
| `"a" + "b"`               | Strings are immutable                    | `StringBuilder`, caching                      |
| `new List<T>()`           | Memory allocated for collection          | Reuse the list (`.Clear()`)                   |
| `new MyClass()`           | class on the heap                        | Use struct or object pool                     |
| `foreach`                 | On some collections, creates an iterator | Use `for` loop                                |
| `GetComponent` each frame | Internal allocation                      | Cache components in `Start()`                 |
| `GameObject.Find`         | String allocation                        | Use references or `Transform.Find` with cache |

---

## 🔄 2. Object Pooling
Object pooling is a pattern where you do NOT create and destroy objects during gameplay — you reuse existing ones.

### Why is it needed?
- `Instantiate` / `Destroy` → memory allocation (new object) + GC work when destroyed.
- In shooters (bullets), enemy spawning, particles — without a pool, the game freezes every few seconds.

### 🏊 How object pooling works:
1. Create N objects in advance (e.g., 20 bullets).
2. Store them all in a queue (`Queue`, `Stack`, `List`).
3. When a bullet is needed — take it from the pool, activate it, move it.
4. When the bullet hits or goes out of bounds — deactivate it and return it to the pool.
5. Never call `Instantiate` and `Destroy` during gameplay.

### 📋 Simple object pool example:
```csharp
using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 20;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    
    private void Start()
    {
        // Pre-create objects
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
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
            // Optional: expand the pool
            GameObject newObj = Instantiate(prefab);
            return newObj;
        }
    }
    
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

### 🎮 Usage example (bullet script):
```csharp
public class Bullet : MonoBehaviour
{
    private ObjectPool pool;
    
    public void Initialize(ObjectPool ownerPool)
    {
        pool = ownerPool;
        // Auto-return timer
        Invoke(nameof(ReturnToPool), 3f);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        ReturnToPool();
    }
    
    private void ReturnToPool()
    {
        CancelInvoke();
        pool.ReturnObject(gameObject);
    }
}
```

### 📊 When to use pooling, when to avoid allocations?

| Situation                                         | Solution                                        |
| ---                                               | ---                                             |
| Bullets, enemies, fragments, particles            | ✅ Object pooling is mandatory                 |
| UI elements that are frequently created/destroyed | ✅ Object pooling                              |
| Temporary arrays in Update                        | ✅ Reuse / stack memory (`Span<T>`)            | 
| Logging strings in Update                         | ✅ Disable or cache                            |
| One‑time objects (level loading)                  | ❌ No pool needed; Instantiate/Destroy is fine |
| Static objects (walls, floor)                     | ❌ No pool needed                              |

---

## 🛠️ Useful tips
- Profiler: Open `Window → Analysis → Profiler`, enable GC Alloc. See which methods create garbage.
- Deep Profile: Shows allocations inside every method (slow but precise).
- Use structs: If an object is small and short‑lived, consider `struct` (but copying can be expensive).
- Libraries: `UnityEngine.Pool` (official API) — `ObjectPool<T>`, `CollectionPool<T>`, etc.

---

## 🔗 Example with Unity's official pool (recommended approach):
```csharp
using UnityEngine;
using UnityEngine.Pool;

public class AdvancedPool : MonoBehaviour
{
    private IObjectPool<GameObject> pool;
    
    private void Start()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: 20,
            maxSize: 50
        );
    }
    
    public GameObject Get() => pool.Get();
    public void Release(GameObject obj) => pool.Release(obj);
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
