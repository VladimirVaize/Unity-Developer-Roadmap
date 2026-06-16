# 🎯 Задача: «Поиск и устранение утечек памяти в игровом проекте»
Вы получили проект, в котором игроки жалуются на падение FPS и вылеты после длительной игры. 
Ваша задача — найти и исправить утечки памяти, используя Memory Profiler и другие инструменты профилирования.

### 📝 Исходный код проекта (содержит утечки):
```csharp
// ===== 1. EnemySpawner.cs =====
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int maxEnemies = 10;
    private List<Enemy> _activeEnemies = new List<Enemy>();
    private static List<GameObject> _allSpawnedObjects = new List<GameObject>(); // Статическая утечка!
    
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
        
        // Подписка на события БЕЗ отписки!
        enemy.GetComponent<Enemy>().OnDeath += HandleEnemyDeath;
    }
    
    void HandleEnemyDeath(Enemy enemy)
    {
        // Врага уничтожили, но из списка не удалили!
        Destroy(enemy.gameObject);
    }
}

// ===== 2. UIManager.cs =====
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text scoreText;
    public Text healthText;
    private List<string> _logMessages = new List<string>(); // Растёт бесконечно
    
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // Объект никогда не уничтожается!
    }
    
    void Start()
    {
        GameManager.OnScoreChanged += UpdateScore; // Утечка!
        GameManager.OnHealthChanged += UpdateHealth; // Утечка!
    }
    
    void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
        _logMessages.Add("Score updated to " + score); // Бесконечный рост!
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
        // Загружаем ВСЕ клипы из папки Resources (Огромная нагрузка!)
        _allClips = Resources.LoadAll<AudioClip>("Audio/");
        
        foreach (var clip in _allClips)
        {
            _clipCache[clip.name] = clip; // Кэшируем всё!
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
        StartCoroutine(LeakyCoroutine()); // Бесконечная корутина!
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

## 📋 Задачи для выполнения:
### Часть 1: Выявление утечек
1. Установите Memory Profiler через Package Manager
2. Запустите проект и сделайте снимок памяти через 5 секунд
3. Поиграйте 2 минуты (спавн врагов, обновление UI, звуки)
4. Сделайте второй снимок
5. Сравните снимки и найдите:
   - Какие типы объектов увеличились в количестве?
   - Какие объекты должны были быть уничтожены, но остались?
   - Где наибольший рост памяти?
  
### Часть 2: Исправление утечек
Исправьте все утечки в коде выше:
1. Статическая коллекция `_allSpawnedObjects`
2. Неотписанные события в `UIManager`
3. Бесконечный рост списка `_logMessages`
4. Кэширование всех аудиоклипов в `AudioManager`
5. Бесконечная корутина в `CoroutineLeak`
6. Утечка врагов при смерти (список `_activeEnemies`)

### Часть 3: Документация и проверка
7. Напишите отчёт с описанием:
   - Какие утечки были найдены
   - Какие методы использовали для их обнаружения
   - Как их исправили
   - Результаты после исправления (сравнение памяти)
  
8. Добавьте автоматическую проверку памяти в код:
```csharp
public class MemoryCheck : MonoBehaviour
{
    [ContextMenu("Check Memory Status")]
    void CheckMemory()
    {
        // 1. Проверить количество активных объектов
        // 2. Проверить размер managed heap
        // 3. Проверить количество кэшированных аудиоклипов
        // 4. Вывести предупреждение при обнаружении утечек
    }
}
```

---

## 🧰 Требования к реализации:
- Используйте Memory Profiler для анализа
- Все исправления должны быть задокументированы в коде
- Добавьте обработку выгрузки сцен (`SceneManager.sceneUnloaded`)
- Реализуйте Object Pool для врагов вместо Instantiate/Destroy
- Добавьте максимальный размер для `_logMessages` (например, 100 записей)
- Используйте WeakReference для проверки утечек в `MemoryCheck`

---

## 🔍 Ожидаемый результат:
До исправления:
- Память растёт на ~50-100 МБ за 5 минут игры
- FPS падает с 60 до 30 за 10 минут
- Вылет после 15-20 минут игры

После исправления:
- Память стабильна (±5 МБ)
- FPS держится на 60
- Игра работает без вылетов в течение часа

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
