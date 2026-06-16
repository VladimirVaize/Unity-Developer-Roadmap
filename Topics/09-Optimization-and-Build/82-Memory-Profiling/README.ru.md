# 🧠 Профилирование памяти в Unity: Memory Profiler, утечки памяти (Memory Leaks)
Управление памятью — критический аспект разработки игр, особенно на мобильных платформах. Unity предоставляет мощные инструменты для анализа использования памяти и выявления утечек. 
В этом руководстве мы разберём Memory Profiler, основные причины утечек и методы их обнаружения и устранения.

---

## 1. Основы управления памятью в Unity
Unity использует две системы управления памятью:
| Система | Описание |
| --- | --- |
| Управляемая память (Managed) | Управляется сборщиком мусора (Garbage Collector). Содержит C# объекты, массивы, строки. |
| Неуправляемая память (Native/Unmanaged) | Управляется Unity вручную. Содержит текстуры, меши, аудиоклипы, шейдеры. |

### 📊 Типы памяти:
| Тип | Что содержит | Кто управляет |
| --- | --- | --- |
| Managed Heap | Скрипты, GameObject, Component, коллекции | GC (Сборщик мусора) |
| Native Heap | Текстуры, меши, AudioClips, анимации | Unity (автоматически) |
| Graphics Memory | Данные на GPU (рендеринг) | Графический драйвер |

---

## 2. Введение в Memory Profiler
Memory Profiler — это пакет Unity для детального анализа памяти. Он позволяет:
- ✅ Смотреть распределение памяти по типам объектов
- ✅ Находить утечки (объекты, которые не должны существовать)
- ✅ Анализировать ссылки между объектами
- ✅ Сравнивать снимки памяти (snapshots) в разные моменты времени

### 🛠️ Установка Memory Profiler:
1. Window → Package Manager
2. Поиск: Memory Profiler
3. Установить пакет

### 🚀 Запуск Memory Profiler:
Window → Memory Profiler → Open

---

## 3. Создание и анализ снимков (Snapshots)
Снимок — это "фотография" состояния памяти в определённый момент времени.

### 📸 Создание снимка:
1. Открыть Memory Profiler
2. Нажать кнопку Capture (или Capture New Snapshot)
3. Подождать завершения захвата

### 📊 Анализ снимка:
| Раздел | Что показывает |
| --- | --- |
| Total Memory | Общее использование памяти |
| Managed Heap | C# объекты |
| Native Objects | Нативные объекты (текстуры, меши) |
| Graphics | GPU память |
| Audio | Аудиоданные |

---

## 4. Основные причины утечек памяти (Memory Leaks)
### 🔴 1. Неотписанные события (Event Handlers)
```csharp
// ❌ ПЛОХО: Подписка без отписки
public class LeakyClass : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.OnScoreChanged += HandleScoreChange; // Утечка!
    }
    
    void HandleScoreChange(int newScore) { }
}

// ✅ ХОРОШО: Всегда отписываемся
public class SafeClass : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.OnScoreChanged += HandleScoreChange;
    }
    
    void OnDisable()
    {
        GameManager.OnScoreChanged -= HandleScoreChange; // Отписка!
    }
}
```

### 🔴 2. Статические ссылки (Static References)
```csharp
// ❌ ПЛОХО: Статическая коллекция никогда не очищается
public static class Cache
{
    public static List<GameObject> EnemyList = new List<GameObject>(); // Всегда в памяти!
}

// ✅ ХОРОШО: Очистка статики при необходимости
public static class Cache
{
    public static List<GameObject> EnemyList = new List<GameObject>();
    
    public static void ClearCache()
    {
        EnemyList.Clear(); // Очистка когда не нужно
    }
}
```

### 🔴 3. Забытые корутины (Coroutines)
```csharp
// ❌ ПЛОХО: Бесконечная корутина без остановки
void Start()
{
    StartCoroutine(InfiniteLoop()); // Никогда не остановится
}

IEnumerator InfiniteLoop()
{
    while (true) // Бесконечный цикл
    {
        yield return null;
    }
}

// ✅ ХОРОШО: Остановка корутины при уничтожении
private Coroutine _myCoroutine;

void Start()
{
    _myCoroutine = StartCoroutine(InfiniteLoop());
}

void OnDisable()
{
    StopCoroutine(_myCoroutine); // Остановка!
}
```

### 🔴 4. Объекты в DontDestroyOnLoad
```csharp
// ❌ ПЛОХО: Объект остаётся навсегда
void Awake()
{
    DontDestroyOnLoad(gameObject); // Утечка, если не очищать
    // Добавляем много дочерних объектов
    for (int i = 0; i < 1000; i++)
    {
        var child = new GameObject("Child_" + i);
        child.transform.SetParent(transform);
    }
}

// ✅ ХОРОШО: Контроль над жизненным циклом
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
            Destroy(gameObject); // Удаляем дубликат
        }
    }
    
    // Метод для очистки перед выходом
    public void Cleanup()
    {
        // Удаляем все дочерние объекты
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
```

### 🔴 5. Неуправляемые ресурсы (Текстуры, Меши, AudioClips)
```csharp
// ❌ ПЛОХО: Загрузка без выгрузки
public class TextureLoader : MonoBehaviour
{
    private Texture2D _largeTexture;
    
    void Start()
    {
        _largeTexture = Resources.Load<Texture2D>("LargeTexture");
        // Текстура никогда не выгружается!
    }
}

// ✅ ХОРОШО: Выгрузка через Resources.UnloadUnusedAssets
public class TextureLoader : MonoBehaviour
{
    private Texture2D _largeTexture;
    
    void Start()
    {
        _largeTexture = Resources.Load<Texture2D>("LargeTexture");
    }
    
    void OnDestroy()
    {
        Resources.UnloadAsset(_largeTexture); // Выгрузка ресурса
        // OR
        Resources.UnloadUnusedAssets(); // Выгрузка всех неиспользуемых ресурсов
    }
}
```

### 🔴 6. Ассеты в Addressables
```csharp
// ❌ ПЛОХО: Загрузка без освобождения
public class AddressableLeak : MonoBehaviour
{
    async void Start()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>("Enemy");
        await handle.Task;
        GameObject enemy = handle.Result;
        // Не вызывается handle.Release()
    }
}

// ✅ ХОРОШО: Освобождение через Release
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
        Addressables.Release(_handle); // Освобождение!
    }
}
```

---

## 5. Инструменты для поиска утечек
### 🔍 1. Memory Profiler — сравнение снимков
```csharp
// Создаём снимки в коде для сравнения
using UnityEngine.Profiling;

public class MemorySnapshotHelper : MonoBehaviour
{
    public void TakeMemorySnapshot(string label)
    {
        // В Memory Profiler это делается вручную через UI
        // Можно использовать для маркировки важных моментов
        Debug.Log($"Memory snapshot taken: {label}");
    }
}
```
Практика:
1. Сделать снимок до загрузки уровня
2. Загрузить уровень
3. Сделать снимок после загрузки
4. Выгрузить уровень
5. Сделать снимок после выгрузки
6. Сравнить снимки → найти объекты, которые не удалились

### 🔍 2. Профилировщик Unity (Profiler)
Window → Profiler → Memory

| Показатель | Что отслеживает |
| --- | --- |
| Total Allocated | Общая выделенная память |
| GC Alloc | Выделения в управляемой куче |
| Texture Memory | Память текстур |
| Mesh Memory | Память мешей |
| Audio Memory | Память аудио |

### 🔍 3. Использование WeakReference для проверки утечек
```csharp
public class WeakReferenceExample : MonoBehaviour
{
    private WeakReference _weakRef;
    
    void Start()
    {
        var obj = new GameObject("TestObject");
        _weakRef = new WeakReference(obj);
        
        // Вызываем сборку мусора
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        
        // Проверяем: объект всё ещё жив?
        if (_weakRef.IsAlive)
        {
            Debug.Log("Объект всё ещё существует (возможная утечка!)");
        }
        else
        {
            Debug.Log("Объект уничтожен (нет утечки)");
        }
    }
}
```

### 🔍 4. Object.FindObjectOfType (проверка утечек MonoBehaviour)
```csharp
public class LeakDetector : MonoBehaviour
{
    [ContextMenu("Check For Leaks")]
    void CheckForLeaks()
    {
        // Проверка на объекты, которые должны были быть уничтожены
        var allObjects = FindObjectsOfType<GameObject>();
        Debug.Log($"Total GameObjects: {allObjects.Length}");
        
        // Ищем объекты с тегом "Leakable"
        var leakableObjects = GameObject.FindGameObjectsWithTag("Leakable");
        if (leakableObjects.Length > 0)
        {
            Debug.LogWarning($"Найдены объекты с тегом Leakable: {leakableObjects.Length}");
            foreach (var obj in leakableObjects)
            {
                Debug.Log($"Leak: {obj.name} (Root: {obj.transform.root.name})");
            }
        }
    }
}
```

---

## 6. Предотвращение утечек: шаблоны и практики
### 🧩 Паттерн Singleton с очисткой
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
    
    // Очистка при завершении игры
    private void OnApplicationQuit()
    {
        _instance = null;
    }
    
    // Альтернатива: ручная очистка
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

### 🧩 Использование Object Pool (Пул объектов)
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

### 🧩 Выгрузка неиспользуемых ресурсов
```csharp
public class ResourceManager : MonoBehaviour
{
    public void UnloadUnusedResources()
    {
        // Метод 1: Выгрузка конкретного ассета
        Resources.UnloadUnusedAssets();
        
        // Метод 2: Принудительная сборка мусора
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        
        // Метод 3: Очистка кэша Addressables
        Addressables.ClearDependencyCacheAsync();
    }
    
    // Автоматическая выгрузка при смене сцены
    void OnSceneUnloaded(Scene scene)
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
```

---

## 7. Пример: Полный цикл обнаружения и исправления утечки
### 🐛 Ситуация: Утечка врагов в игре
```csharp
// ===== ПРОБЛЕМНЫЙ КОД =====
public class EnemySpawner : MonoBehaviour
{
    private List<Enemy> _enemies = new List<Enemy>();
    
    public void SpawnEnemy()
    {
        var enemy = Instantiate(enemyPrefab);
        _enemies.Add(enemy);
        enemy.OnDeath += HandleEnemyDeath; // Утечка! Не отписываемся
    }
    
    public void HandleEnemyDeath(Enemy enemy)
    {
        // Врага удаляем, но он всё ещё в списке!
        // OnDeath событие не очищено!
    }
}

// ===== ИСПРАВЛЕННЫЙ КОД =====
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
        // Отписываемся от события
        enemy.OnDeath -= HandleEnemyDeath;
        
        // Удаляем из списка
        _enemies.Remove(enemy);
        
        // Уничтожаем объект
        Destroy(enemy.gameObject);
    }
    
    // Очистка при выгрузке уровня
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

## 8. Рекомендации по оптимизации памяти
| Рекомендация | Описание |
| --- | --- |
| Используйте пулы объектов | Вместо Instantiate/Destroy |
| Выгружайте неиспользуемые ресурсы | Вызывайте `Resources.UnloadUnusedAssets()` при смене сцены |
| Используйте Addressables | Для управления контентом по запросу |
| Избегайте статических коллекций | Особенно для MonoBehaviour |
| Отписывайтесь от всех событий | В `OnDisable` или `OnDestroy` |
| Используйте WeakReference | Для кэширования объектов |
| Сжимайте текстуры | Уменьшайте размер |
| Используйте Sprite Atlas | Для оптимизации спрайтов |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
