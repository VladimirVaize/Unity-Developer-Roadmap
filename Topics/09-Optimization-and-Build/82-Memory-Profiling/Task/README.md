# 🎯 Task: «Finding and Fixing Memory Leaks in a Game Project»
You have received a project where players complain about FPS drops and crashes after prolonged play. 
Your task is to find and fix memory leaks using Memory Profiler and other profiling tools.

### 📝 Project Source Code (contains leaks):
```csharp
// ===== 1. EnemySpawner.cs =====
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int maxEnemies = 10;
    private List<Enemy> _activeEnemies = new List<Enemy>();
    private static List<GameObject> _allSpawnedObjects = new List<GameObject>(); // Static leak!
    
    void Update()
    {
        if (_activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
        }
    }
    
    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab);
        _activeEnemies.Add(enemy.GetComponent<Enemy>());
        _allSpawnedObjects.Add(enemy);
        
        // Event subscription WITHOUT unsubscription!
        enemy.GetComponent<Enemy>().OnDeath += HandleEnemyDeath;
    }
    
    void HandleEnemyDeath(Enemy enemy)
    {
        // Enemy destroyed but never removed from list!
        Destroy(enemy.gameObject);
    }
}

// ===== 2. UIManager.cs =====
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text scoreText;
    public Text healthText;
    private List<string> _logMessages = new List<string>(); // Grows infinitely
    
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // Object never destroyed!
    }
    
    void Start()
    {
        GameManager.OnScoreChanged += UpdateScore; // Leak!
        GameManager.OnHealthChanged += UpdateHealth; // Leak!
    }
    
    void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
        _logMessages.Add("Score updated to " + score); // Infinite growth!
    }
    
    void UpdateHealth(int health)
    {
        healthText.text = "Health: " + health;
    }
}

// ===== 3. AudioManager.cs =====
public class AudioManager : MonoBehaviour
{
    private AudioClip[] _allClips;
    private Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();
    
    void Start()
    {
        // Loading ALL clips from Resources folder (Huge memory!)
        _allClips = Resources.LoadAll<AudioClip>("Audio/");
        
        foreach (var clip in _allClips)
        {
            _clipCache[clip.name] = clip; // Caching everything!
        }
    }
    
    public void PlaySound(string name)
    {
        if (_clipCache.ContainsKey(name))
        {
            AudioSource.PlayClipAtPoint(_clipCache[name], Vector3.zero);
        }
    }
}

// ===== 4. CoroutineLeak.cs =====
public class CoroutineLeak : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LeakyCoroutine()); // Infinite coroutine!
    }
    
    IEnumerator LeakyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("Still running...");
        }
    }
}
```

---

## 📋 Tasks:
### Part 1: Identifying Leaks
1. Install Memory Profiler via Package Manager
2. Run the project and take a memory snapshot after 5 seconds
3. Play for 2 minutes (enemy spawning, UI updates, sounds)
4. Take a second snapshot
5. Compare snapshots and find:
   - Which object types increased in count?
   - Which objects should have been destroyed but remain?
   - Where is the largest memory growth?
  
### Part 2: Fixing Leaks
Fix all leaks in the code above:
1. Static collection `_allSpawnedObjects`
2. Unsubscribed events in `UIManager`
3. Infinite growth of `_logMessages` list
4. Caching all audio clips in `AudioManager`
5. Infinite coroutine in `CoroutineLeak`
6. Enemy leak on death (`_activeEnemies` list)

### Part 3: Documentation and Verification
7. Write a report describing:
   - Which leaks were found
   - What methods were used to detect them
   - How they were fixed
   - Results after fixing (memory comparison)
  
8. Add automatic memory check to code:
```csharp
public class MemoryCheck : MonoBehaviour
{
    [ContextMenu("Check Memory Status")]
    void CheckMemory()
    {
        // 1. Check active object count
        // 2. Check managed heap size
        // 3. Check cached audio clips count
        // 4. Log warning if leaks detected
    }
}
```

---

## 🧰 Implementation Requirements:
- Use Memory Profiler for analysis
- All fixes must be documented in code
- Add scene unload handling (`SceneManager.sceneUnloaded`)
- Implement Object Pool for enemies instead of Instantiate/Destroy
- Add maximum size for `_logMessages` (e.g., 100 entries)
- Use WeakReference for leak checking in `MemoryCheck`

---

## 🔍 Expected Results:
Before fixes:
- Memory grows by ~50-100 MB over 5 minutes of gameplay
- FPS drops from 60 to 30 over 10 minutes
- Crash after 15-20 minutes of play

After fixes:
- Memory stable (±5 MB)
- FPS stays at 60
- Game runs without crashes for over an hour

---

### ⭐ If this project was useful, put a star on GitHub!
