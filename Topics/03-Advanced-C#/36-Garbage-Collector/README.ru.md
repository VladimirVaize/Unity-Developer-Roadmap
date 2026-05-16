# 🗑️ Сборщик мусора (GC) в Unity: Пул объектов и избегание аллокаций

Производительность — ключевой аспект любой игры. Один из главных врагов плавного FPS в Unity — сборщик мусора (Garbage Collector, GC). 
В этом материале мы разберём, что такое GC, почему он вызывает "тормоза" (фризы) и как с этим бороться с помощью пула объектов (Object Pooling) и избегания аллокаций в Update.

---

## 📚 Что такое сборщик мусора?
Сборщик мусора — это механизм, который автоматически освобождает память, занятую объектами, которые больше не используются.

### Как это работает в Unity (упрощённо):
1. Вы создаёте объект (например, new `GameObject()`, массив, строку, список).
2. Unity выделяет под него память в "куче" (managed heap).
3. Когда объект больше не нужен (на него нет ссылок), память не освобождается сразу.
4. Периодически GC запускается, ищет "мусор" и освобождает память.
5. Проблема: Во время работы GC игра приостанавливается (фризится) на несколько миллисекунд или даже больше.

> [!Important]
> В Unity GC работает в основном потоке — поэтому даже короткая пауза заметна в динамичных сценах.

---

## 🧠 Почему GC страшен в играх?
- Спайки (spikes) на графике времени кадра: Игра идёт плавно (60 FPS), затем резкий провал из-за GC.
- На мобильных устройствах: Паузы GC могут быть очень долгими (до 100–200 мс) из-за медленной памяти.
- В VR: Любая пауза вызывает дискомфорт и укачивание у пользователя.

---

# ⚡ Главное правило: Чем меньше аллокаций (созданий объектов) во время игры — тем лучше.

## 🎯 1. Избегание аллокаций в Update

Метод `Update()` вызывается каждый кадр (часто 60+ раз в секунду). Если в нём создавать мусор, GC будет запускаться очень часто.

### 🔴 Плохо (создаёт мусор в каждом кадре):
```csharp
void Update()
{
    // Создаёт новую строку каждый кадр
    string message = "Score: " + score;
    
    // Создаёт новый массив каждый кадр
    Vector3[] tempArray = new Vector3[10];
    
    // Создаёт новый список каждый кадр
    List<int> tempList = new List<int>();
    
    // Boxing (упаковка) — int в object
    object box = currentHealth;
}
```

### ✅ Хорошо (без аллокаций):
```csharp
// Поле для переиспользования
private Vector3[] reusableArray = new Vector3[10];
private List<int> reusableList = new List<int>();

void Update()
{
    // Используем StringBuilder для строк (или кэшируем результат)
    // Или просто избегаем конкатенации в UI
    
    // Переиспользуем массив и список
    System.Array.Clear(reusableArray, 0, reusableArray.Length);
    reusableList.Clear();
    
    // Используем struct вместо class (если возможно)
    // struct хранятся в стеке, не в куче
}
```

### 📝 Частые источники аллокаций в Update:

| Источник               | Почему аллокация?                          | Как исправить                                   |
| ---                    | ---                                        | ---                                             |
| `"a" + "b"`            | Строки неизменяемы (immutable)             | `StringBuilder`, кэширование                    |
| `new List<T>()`        | Выделяется память под коллекцию            | Переиспользуйте список (`.Clear()`)             |
| `new MyClass()`        | class в куче                               | Используйте struct или пул объектов             |
| `foreach`              | На некоторых коллекциях создаётся итератор | Используйте `for`                               |
| `GetComponent` в кадре | Внутри может быть аллокация                | Кэшируйте компоненты в `Start()`                |
| `GameObject.Find`      | Аллокация строки поиска                    | Используйте ссылки или `Transform.Find` с кэшем |

---

## 🔄 2. Пул объектов (Object Pooling)
Пул объектов — это паттерн, при котором вы не создаёте и не уничтожаете объекты во время игры, а переиспользуете уже существующие.

### Зачем это нужно?
- Instantiate / Destroy → аллокация памяти (новый объект) + работа GC при удалении.
- В шутерах с пулями, спавне врагов, частицах — без пула игра фризит каждые несколько секунд.

### 🏊 Как работает пул объектов:
1. Заранее создаётся N объектов (например, 20 пуль).
2. Все они хранятся в очереди (`Queue`, `Stack`, `List`).
3. Когда нужна пуля — берётся из пула, активируется, перемещается.
4. Когда пуля попала или улетела — деактивируется и возвращается в пул.
5. Никогда не вызываются `Instantiate` и `Destroy` во время геймплея.

### 📋 Простой пример пула объектов:
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
        // Предварительное создание объектов
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false); // Сразу выключаем
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
            // Опционально: расширяем пул
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

### 🎮 Пример использования (скрипт пули):
```csharp
public class Bullet : MonoBehaviour
{
    private ObjectPool pool;
    
    public void Initialize(ObjectPool ownerPool)
    {
        pool = ownerPool;
        // Таймер авто-возврата
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

### 📊 Когда использовать пул, когда избегать аллокаций?

| Ситуация                                       | Решение                                            |
| ---                                            | ---                                                |
| Пули, враги, осколки, частицы                  | ✅ Пул объектов обязателен                         |
| UI-элементы, которые часто создаются/удаляются | ✅ Пул объектов                                    |
| Временные массивы в Update                     | ✅ Переиспользование / стековая память (`Span<T>`) |
| Логирование строк в Update                     | ✅ Отключить или кэшировать                        |
| Разовые объекты (загрузка уровня)              | ❌ Пул не нужен, можно Instantiate/Destroy         |
| Статичные объекты (стены, пол)                 | ❌ Пул не нужен                                    |

---

## 🛠️ Полезные советы
- Профилировщик (Profiler): Откройте `Window → Analysis → Profiler`, включите GC Alloc. Смотрите, какие методы создают мусор.
- Deep Profile: Показывает аллокации внутри каждого метода (медленно, но точно).
- Используйте struct: Если объект маленький и живёт недолго, подумайте о `struct` (но копирование может дорого стоить).
- Библиотеки: `UnityEngine.Pool` (официальный API) — `ObjectPool<T>`, `CollectionPool<T>` и др.

---

## 🔗 Пример с официальным пулом Unity (рекомендуемый способ):
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

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
