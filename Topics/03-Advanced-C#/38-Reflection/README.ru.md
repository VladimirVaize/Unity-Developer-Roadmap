# 🔍 Reflection в Unity: Интроспекция и метапрограммирование
Этот материал покрывает ключевые механизмы рефлексии и атрибутов в Unity: `System.Reflection`, 
поиск компонентов с интерфейсами через `GetComponents`, атрибуты `[RequireComponent]` и `[ContextMenu]`. 
Эти инструменты позволяют писать более гибкий, автоматизированный и чистый код.

---

## 📖 1. System.Reflection (Пространство имён)
### 🎯 Для чего нужно:
Reflection позволяет во время выполнения (runtime) исследовать структуру кода: получать информацию о типах (классах), их полях, 
методах, свойствах, атрибутах — и даже вызывать методы или изменять поля, которые являются `private`. 
Это мощный инструмент для создания редакторских инструментов, систем сериализации, модульных архитектур и т.д.

### ⚙️ Как использовать:
```csharp
using System.Reflection;
using UnityEngine;

public class ReflectionExample : MonoBehaviour
{
    private int secretHealth = 100;

    void Start()
    {
        // Получить тип текущего объекта
        Type type = this.GetType();

        // Получить приватное поле по имени
        FieldInfo healthField = type.GetField("secretHealth", 
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Прочитать значение
        int currentHealth = (int)healthField.GetValue(this);
        Debug.Log($"Здоровье: {currentHealth}");

        // Изменить значение
        healthField.SetValue(this, 50);
        Debug.Log($"Новое здоровье: {(int)healthField.GetValue(this)}");
    }
}
```

### 📌 Пример из реальной жизни:
Вы пишете систему сохранения (Save System), которая автоматически находит все поля с атрибутом `[SaveField]` (даже приватные) и сохраняет их в JSON, не требуя ручного копирования каждого поля.

---

## 🔌 2. GetComponents с интерфейсами
### 🎯 Для чего нужно:
Стандартный `GetComponent<T>()` не работает с интерфейсами напрямую. 
Однако можно получить все компоненты на объекте и отфильтровать те, которые реализуют нужный интерфейс. 
Это полезно для систем, где разные объекты ведут себя по-разному, но имеют общий контракт (например, все получают урон).

### ⚙️ Как использовать:
```csharp
public interface IDamageable
{
    void TakeDamage(int amount);
}

public class Player : MonoBehaviour, IDamageable
{
    public void TakeDamage(int amount) => Debug.Log($"Игрок получил {amount} урона");
}

public class Enemy : MonoBehaviour, IDamageable
{
    public void TakeDamage(int amount) => Debug.Log($"Враг получил {amount} урона");
}

public class DamageDealer : MonoBehaviour
{
    void DealDamageToAll(GameObject target)
    {
        // Получаем все компоненты на target, которые реализуют IDamageable
        IDamageable[] damageables = target.GetComponents<IDamageable>();
        
        foreach (var d in damageables)
        {
            d.TakeDamage(10);
        }
    }
}
```

### 📌 Пример из реальной жизни:
У вас есть взрывная граната. В радиусе взрыва находятся игрок, враги, разрушаемые ящики и двери. 
Все они реализуют `IDamageable`. Граната находит все компоненты с этим интерфейсом в радиусе и вызывает `TakeDamage()`.

---

## 🔧 3. [RequireComponent] (Атрибут требования компонента)
### 🎯 Для чего нужно:
Атрибут гарантирует, что при добавлении вашего скрипта на GameObject, Unity автоматически добавит указанные компоненты, если их ещё нет. 
Это предотвращает ошибки "Missing Component" во время выполнения.

### ⚙️ Как использовать:
```csharp
using UnityEngine;

// Требуем, чтобы у объекта был Rigidbody и Collider
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        // Теперь можно безопасно получать компоненты — они гарантированно существуют
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
}
```

### 📌 Пример из реальной жизни:
Скрипт `HealthSystem` всегда требует `Animator`, чтобы проигрывать анимацию смерти, и `Collider`, чтобы обрабатывать попадания. 
Атрибут `[RequireComponent]` автоматически добавляет их при создании объекта.

---

## 🖱️ 4. [ContextMenu] (Атрибут контекстного меню)
### 🎯 Для чего нужно:
Позволяет вызывать метод вашего скрипта прямо из контекстного меню компонента в редакторе Unity (правой кнопкой мыши по заголовку компонента в Inspector). 
Идеально для редакторских утилит, тестовых действий, настройки сцены.

### ⚙️ Как использовать:
```csharp
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemyCount = 10;

    [ContextMenu("Spawn Enemies")]
    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Instantiate(enemyPrefab, Random.insideUnitSphere * 10, Quaternion.identity);
        }
        Debug.Log($"{enemyCount} врагов создано!");
    }

    [ContextMenu("Clear Enemies")]
    void ClearEnemies()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies) DestroyImmediate(e);
        Debug.Log("Все враги удалены");
    }
}
```
### 📌 Пример из реальной жизни:
У вас есть скрипт `DatabaseInitializer`. Через `[ContextMenu("Reset Database")]` вы одним кликом очищаете и заполняете тестовыми данными базу PlayerPrefs или ScriptableObject.

---

## 🧠 Связка всех техник в одном примере
Представьте систему урона с автоматическим добавлением необходимых компонентов:
```csharp
[RequireComponent(typeof(Collider), typeof(Animator))]
public class DestructibleObject : MonoBehaviour, IDamageable
{
    [ContextMenu("Debug Health")]
    void DebugHealth() => Debug.Log($"Здоровье: {health}");

    private int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    void Die() => Destroy(gameObject);
}

// В другом скрипте используем интерфейс:
public class Explosion : MonoBehaviour
{
    void Explode()
    {
        var damageables = FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>();
        foreach (var d in damageables) d.TakeDamage(50);
    }
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
