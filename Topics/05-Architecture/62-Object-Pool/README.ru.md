# 🔄 Паттерн «Пул объектов» (Object Pool) в Unity
Этот материал покрывает концепцию пула объектов, его реализацию для частиц, врагов и пуль, 
а также ключевые отличия от стандартного подхода `Instantiate` / `Destroy`.

---

## 📖 1. Что такое паттерн «Пул объектов»?
### 🎯 Для чего нужно:
Пул объектов — это порождающий паттерн проектирования, который вместо уничтожения объектов переиспользует их. 
Готовые объекты хранятся в «пуле» (очередь, список, стек), выдаются по запросу и возвращаются обратно после использования. 
Это радикально снижает нагрузку на сборщик мусора (GC) и повышает производительность.

### ⚙️ Основная идея:
- Вместо: `Instantiate()` → использовать → `Destroy()`
- Используем: Взять из пула → использовать → вернуть в пул

---

## 🚀 2. Отличие от Instantiate / Destroy

| Аспект | Instantiate / Destroy | Object Pool |
| --- | --- | --- |
| Выделение памяти | Каждый раз новая аллокация | Только при создании пула |
| Сборщик мусора (GC) | Высокая нагрузка, фризы | Почти нулевая нагрузка |
| Скорость | Медленно (особенно Destroy) | Быстро (активация/деактивация) |
| Когда использовать | Редкие объекты (UI, менеджеры) | Часто создаваемые объекты (пули, враги, частицы) |
| Сложность кода | Низкая | Средняя (нужно управлять пулом) |

### Пример проблемы с Instantiate/Destroy:
В шутере игрок выпускает 10 пуль в секунду. 
Через минуту создано 600 объектов, которые уничтожаются. 
Каждое уничтожение вызывает фриз GC. Игра начинает тормозить.

### Решение (Object Pool):
Создать 30 пуль заранее. Каждая пуля после вылета за пределы экрана не уничтожается, 
а возвращается в пул и становится доступной для нового выстрела. GC не вызывается.

---

## 🧩 3. Базовая реализация пула в Unity
### Простой пул для пуль
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
        // Создаём пул заранее
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false); // Деактивируем
            pool.Enqueue(obj);    // Кладём в очередь
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
            Debug.LogWarning("Пул пуст! Расширяем...");
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

### Использование пула
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
            // Настройка пули (скорость, урон)
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

## 💥 4. Реализация для частиц
Особенность: Частицы не нужно возвращать в пул вручную — они могут возвращаться автоматически по окончании анимации/системы частиц.
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
        
        // Автоматический возврат через корутину
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

## 👾 5. Реализация для врагов (сложнее)
Особенность: Враги имеют состояние (здоровье, позиция). При возврате в пул нужно сбрасывать состояние.
```csharp
public class Enemy : MonoBehaviour
{
    public int health = 100;
    private EnemyPool pool;
    
    public void Initialize(EnemyPool ownerPool)
    {
        pool = ownerPool;
        health = 100;
        // Сброс других параметров
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            pool.ReturnEnemy(this);
    }
    
    void OnDisable()
    {
        // Сброс анимаций, остановка корутин
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

## 🔧 6. Универсальный пул (Generic Pool)
Для переиспользования кода можно создать generic-пул:
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

## 📊 7. Когда использовать пул, а когда нет?

| ✅ Использовать Object Pool | ❌ Не использовать Object Pool |
| --- | --- |
| Часто создаваемые объекты (пули, осколки) | Редкие объекты (боссы, меню) |
| Мобильные игры (чувствительны к GC) | Объекты с уникальным состоянием |
| Системы частиц (много коротких эффектов) | Объекты, которые создаются 1-2 раза |
| Враги в roguelike / шутерах | Загрузка сцен (Addressables лучше) |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
