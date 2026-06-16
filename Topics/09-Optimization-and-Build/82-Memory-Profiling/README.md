# 🧠 Memory Profiling in Unity: Memory Profiler, Memory Leaks
Memory management is a critical aspect of game development, especially on mobile platforms. Unity provides powerful tools for analyzing memory usage and detecting leaks. 
This guide covers Memory Profiler, common causes of memory leaks, and methods for detection and fixing.

---

## 1. Memory Management Basics in Unity
Unity uses two memory management systems:
| System | Description |
| --- | --- |
| Managed Memory | Managed by Garbage Collector. Contains C# objects, arrays, strings. |
| Native/Unmanaged Memory | Managed manually by Unity. Contains textures, meshes, audio clips, shaders. |

### 📊 Memory Types:
| Type | What it contains | Who manages |
| --- | --- | --- |
| Managed Heap | Scripts, GameObject, Component, collections | GC (Garbage Collector) |
| Native Heap | Textures, meshes, AudioClips, animations | Unity (automatic) |
| Graphics Memory | GPU rendering data | Graphics driver |

---

## 2. Introduction to Memory Profiler
Memory Profiler is a Unity package for detailed memory analysis. It allows:
- ✅ Viewing memory distribution by object type
- ✅ Finding leaks (objects that shouldn't exist)
- ✅ Analyzing references between objects
- ✅ Comparing memory snapshots at different times

### 🛠️ Installing Memory Profiler:
1. Window → Package Manager
2. Search: Memory Profiler
3. Install the package

### 🚀 Opening Memory Profiler:
Window → Memory Profiler → Open

---

## 3. Creating and Analyzing Snapshots
A snapshot is a "photograph" of memory state at a specific moment.

### 📸 Taking a Snapshot:
1. Open Memory Profiler
2. Click Capture (or Capture New Snapshot)
3. Wait for capture to complete

### 📊 Analyzing a Snapshot:
| Section | What it shows |
| --- | --- |
| Total Memory | Total memory usage |
| Managed Heap | C# objects |
| Native Objects | Native objects (textures, meshes) |
| Graphics | GPU memory |
| Audio | Audio data |

---

## 4. Common Causes of Memory Leaks
### 🔴 1. Unsubscribed Event Handlers
```csharp
// ❌ BAD: Subscription without unsubscription
public class LeakyClass : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.OnScoreChanged += HandleScoreChange; // Leak!
    }
    
    void HandleScoreChange(int newScore) { }
}

// ✅ GOOD: Always unsubscribe
public class SafeClass : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.OnScoreChanged += HandleScoreChange;
    }
    
    void OnDisable()
    {
        GameManager.OnScoreChanged -= HandleScoreChange; // Unsubscribe!
    }
}
```

### 🔴 2. Static References
```csharp
// ❌ BAD: Static collection never cleared
public static class Cache
{
    public static List<GameObject> EnemyList = new List<GameObject>(); // Always in memory!
}

// ✅ GOOD: Clear static when needed
public static class Cache
{
    public static List<GameObject> EnemyList = new List<GameObject>();
    
    public static void ClearCache()
    {
        EnemyList.Clear();
    }
}
```

### 🔴 3. Forgotten Coroutines
```csharp
// ❌ BAD: Infinite coroutine never stopped
void Start()
{
    StartCoroutine(InfiniteLoop()); // Never stops
}

IEnumerator InfiniteLoop()
{
    while (true)
    {
        yield return null;
    }
}

// ✅ GOOD: Stop coroutine on destroy
private Coroutine _myCoroutine;

void Start()
{
    _myCoroutine = StartCoroutine(InfiniteLoop());
}

void OnDisable()
{
    StopCoroutine(_myCoroutine);
}
```

### 🔴 4. DontDestroyOnLoad Objects
```csharp
// ❌ BAD: Object stays forever
void Awake()
{
    DontDestroyOnLoad(gameObject);
    for (int i = 0; i < 1000; i++)
    {
        var child = new GameObject("Child_" + i);
        child.transform.SetParent(transform);
    }
}

// ✅ GOOD: Control lifecycle
public class PersistentManager : MonoBehaviour
{
    private static PersistentManager _instance;
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void Cleanup()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
```

### 🔴 5. Unmanaged Resources (Textures, Meshes, AudioClips)
```csharp
// ❌ BAD: Load without unload
public class TextureLoader : MonoBehaviour
{
    private Texture2D _largeTexture;
    
    void Start()
    {
        _largeTexture = Resources.Load<Texture2D>("LargeTexture");
        // Texture never unloaded!
    }
}

// ✅ GOOD: Unload via Resources.UnloadUnusedAssets
public class TextureLoader : MonoBehaviour
{
    private Texture2D _largeTexture;
    
    void Start()
    {
        _largeTexture = Resources.Load<Texture2D>("LargeTexture");
    }
    
    void OnDestroy()
    {
        Resources.UnloadAsset(_largeTexture);
        // OR
        Resources.UnloadUnusedAssets();
    }
}
```

### 🔴 6. Addressables Assets
```csharp
// ❌ BAD: Load without release
public class AddressableLeak : MonoBehaviour
{
    async void Start()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>("Enemy");
        await handle.Task;
        GameObject enemy = handle.Result;
        // handle.Release() never called
    }
}

// ✅ GOOD: Release via Release
public class AddressableSafe : MonoBehaviour
{
    private AsyncOperationHandle<GameObject> _handle;
    
    async void Start()
    {
        _handle = Addressables.LoadAssetAsync<GameObject>("Enemy");
        await _handle.Task;
        GameObject enemy = _handle.Result;
        Instantiate(enemy);
    }
    
    void OnDestroy()
    {
        Addressables.Release(_handle);
    }
}
```

---

## 5. Tools for Finding Leaks
### 🔍 1. Memory Profiler — Snapshot Comparison
```csharp
using UnityEngine.Profiling;

public class MemorySnapshotHelper : MonoBehaviour
{
    public void TakeMemorySnapshot(string label)
    {
        // In Memory Profiler this is done manually via UI
        Debug.Log($"Memory snapshot taken: {label}");
    }
}
```

Practice:
1. Take snapshot before loading level
2. Load level
3. Take snapshot after loading
4. Unload level
5. Take snapshot after unloading
6. Compare snapshots → find objects that weren't destroyed

### 🔍 2. Unity Profiler
Window → Profiler → Memory
| Metric | What it tracks |
| --- | --- |
| Total Allocated | Total allocated memory |
| GC Alloc | Allocations in managed heap |
| Texture Memory | Texture memory |
| Mesh Memory | Mesh memory |
| Audio Memory | Audio memory |

### 🔍 3. Using WeakReference to Check Leaks
```csharp
public class WeakReferenceExample : MonoBehaviour
{
    private WeakReference _weakRef;
    
    void Start()
    {
        var obj = new GameObject("TestObject");
        _weakRef = new WeakReference(obj);
        
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        
        if (_weakRef.IsAlive)
        {
            Debug.Log("Object still exists (possible leak!)");
        }
        else
        {
            Debug.Log("Object destroyed (no leak)");
        }
    }
}
```

---

## 6. Preventing Leaks: Patterns and Practices
### 🧩 Singleton Pattern with Cleanup
```csharp
public class SafeSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    
    private void OnApplicationQuit()
    {
        _instance = null;
    }
    
    public static void DestroyInstance()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}
```

### 🧩 Object Pool
```csharp
public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _pool = new Queue<T>();
    private T _prefab;
    private Transform _parent;
    
    public ObjectPool(T prefab, int initialSize, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
        
        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateObject();
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
    
    private T CreateObject()
    {
        T obj = GameObject.Instantiate(_prefab, _parent);
        return obj;
    }
    
    public T Get()
    {
        if (_pool.Count == 0)
        {
            return CreateObject();
        }
        
        T obj = _pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }
    
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
    
    public void ClearPool()
    {
        while (_pool.Count > 0)
        {
            T obj = _pool.Dequeue();
            GameObject.Destroy(obj.gameObject);
        }
    }
}
```

### 🧩 Unloading Unused Resources
```csharp
public class ResourceManager : MonoBehaviour
{
    public void UnloadUnusedResources()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        Addressables.ClearDependencyCacheAsync();
    }
}
```

---

## 7. Example: Full Leak Detection and Fix Cycle
### 🐛 Situation: Enemy Leak in Game
```csharp
// ===== PROBLEMATIC CODE =====
public class EnemySpawner : MonoBehaviour
{
    private List<Enemy> _enemies = new List<Enemy>();
    
    public void SpawnEnemy()
    {
        var enemy = Instantiate(enemyPrefab);
        _enemies.Add(enemy);
        enemy.OnDeath += HandleEnemyDeath; // Leak! No unsubscription
    }
    
    public void HandleEnemyDeath(Enemy enemy)
    {
        // Enemy removed, but still in list!
        // OnDeath event not cleaned up!
    }
}

// ===== FIXED CODE =====
public class FixedEnemySpawner : MonoBehaviour
{
    private List<Enemy> _enemies = new List<Enemy>();
    
    public void SpawnEnemy()
    {
        var enemy = Instantiate(enemyPrefab);
        _enemies.Add(enemy);
        enemy.OnDeath += HandleEnemyDeath;
    }
    
    public void HandleEnemyDeath(Enemy enemy)
    {
        enemy.OnDeath -= HandleEnemyDeath;
        _enemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }
    
    public void ClearAllEnemies()
    {
        foreach (var enemy in _enemies)
        {
            enemy.OnDeath -= HandleEnemyDeath;
            Destroy(enemy.gameObject);
        }
        _enemies.Clear();
    }
}
```

---

## 8. Memory Optimization Recommendations
| Recommendation | Description |
| --- | --- |
| Use object pools | Instead of Instantiate/Destroy |
| Unload unused resources | Call `Resources.UnloadUnusedAssets()` on scene change |
| Use Addressables | For on-demand content management |
| Avoid static collections | Especially for MonoBehaviour |
| Unsubscribe from all events | In `OnDisable` or `OnDestroy` |
| Use WeakReference | For caching objects |
| Compress textures | Reduce size |
| Use Sprite Atlas | For sprite optimization |

---

### ⭐ If this project was useful, put a star on GitHub!
