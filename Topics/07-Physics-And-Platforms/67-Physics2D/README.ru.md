# 🎯 Физика 2D в Unity: Rigidbody2D, Collider2D, слои и отличия от 3D физики

Unity предоставляет две независимые системы физики: Physics (3D) и Physics2D (2D). Они используют разные движки, компоненты и настройки. 
Physics2D оптимизирована для плоского мира (X и Y оси) и не имеет третьего измерения (Z — это просто глубина рендеринга).

---

## 1. Основные отличия 2D физики от 3D физики

| Аспект | Physics (3D) | Physics2D (2D) |
| --- | --- | --- |
| Оси движения | X, Y, Z (свободное движение во всех трёх измерениях) | X, Y (только плоскость, Z используется только для сортировки) |
| Типы коллайдеров | BoxCollider, SphereCollider, CapsuleCollider, MeshCollider | BoxCollider2D, CircleCollider2D, CapsuleCollider2D, PolygonCollider2D, EdgeCollider2D |
| Компонент Rigidbody | Rigidbody (3D) | Rigidbody2D |
| Гравитация | Векторная (по умолчанию (0, -9.81, 0)) | Обычно только по оси Y (по умолчанию -9.81 по Y) |
| Вращение | Вокруг произвольной оси в 3D | Только вокруг оси Z (угол поворота) |
| Физические материалы | PhysicMaterial | PhysicsMaterial2D |
| Система слоёв для коллизий | Layers + Physics Matrix | Layers + Physics2D Matrix (настраивается отдельно) |
| Детекция столкновений | Discrete, Continuous, Continuous Dynamic (для быстрых объектов) | Discrete, Continuous (тоже есть, но проще) |
| Производительность | Тяжелее (3D вычисления) | Легче (2D вычисления, меньше данных) |

Главное правило: Никогда не смешивайте 2D и 3D физику в одном объекте — они не взаимодействуют друг с другом. 
2D объекты сталкиваются только с 2D, 3D — только с 3D.

---

## 2. Rigidbody2D — основа физического поведения
`Rigidbody2D` — это компонент, который включает для объекта управление физикой: гравитацию, скорость, вращение, импульсы.

### 🔧 Основные свойства Rigidbody2D:

| Свойство | Описание | Пример использования |
| --- | --- | --- |
| `bodyType` | Тип тела (Static, Kinematic, Dynamic) | Dynamic — полная симуляция |
| `gravityScale` | Множитель глобальной гравитации | `1` = нормальная гравитация, `0` = невесомость |
| `linearVelocity` | Линейная скорость (Vector2) | `rb.linearVelocity = new Vector2(5, 0)` |
| `angularVelocity` | Угловая скорость (вращение) | `rb.angularVelocity = 180` (град/сек) | 
| `mass` | Масса (влияет на столкновения) | `1` (по умолчанию) |
| `drag` | Сопротивление линейному движению | `0.5` (замедление) |
| `angularDrag` | Сопротивление вращению | `0.05` |
| `freezeRotation` | Запрет вращения (фиксация оси Z) | `true` — не поворачивается |

### 🎮 Примеры кода:
```csharp
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 5f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Настройка Rigidbody2D
        rb.bodyType = RigidbodyType2D.Dynamic; // полная физика
        rb.gravityScale = 1f;                  // нормальная гравитация
        rb.freezeRotation = true;               // не вращать от столкновений
        rb.drag = 1f;                          // небольшое сопротивление воздуха
    }
    
    void Update()
    {
        // Движение через скорость (рекомендуется для физики)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }
    
    public void Jump(float force)
    {
        // Импульс силы для прыжка
        rb.AddForce(new Vector2(0, force), ForceMode2D.Impulse);
    }
}
```

### 📌 Типы тел (bodyType):

| Тип | Описание | Пример |
| --- | --- | --- |
| `Static` | Не двигается, не реагирует на силы (идеален для пола и стен) | Грунт, стены, платформы |
| `Kinematic` | Двигается только через код (`MovePosition`), не реагирует на силы | Лифты, движущиеся платформы |
| `Dynamic` | Полная физическая симуляция, реагирует на гравитацию и столкновения | Игрок, враги, предметы |

```csharp
// Пример Kinematic движения
void MovePlatform(Vector2 newPosition)
{
    rb.MovePosition(newPosition); // телепортация с интерполяцией
}
```

---

## 3. Collider2D — форма для столкновений
`Collider2D` определяет форму объекта для физических столкновений. Без коллайдера объект не взаимодействует с физикой.

### 🧩 Типы Collider2D:
| Тип | Форма | Лучшее применение |
| --- | --- | --- |
| `BoxCollider2D` | Прямоугольник | Твёрдые платформы, стены, ящики |
| `CircleCollider2D` | Круг | Мячи, круглые враги, предметы |
| `CapsuleCollider2D` | Капсула (прямоугольник с полукругами) | Персонажи, враги с округлой формой |
| `PolygonCollider2D` | Произвольный многоугольник | Сложные формы, спрайты (можно сгенерировать по контуру) |
| `EdgeCollider2D` | Открытая линия (по точкам) | Платформы-линии, трамплины |

### 📝 Пример настройки коллайдера:
```csharp
using UnityEngine;

public class ColliderSetup : MonoBehaviour
{
    void Start()
    {
        // Добавляем и настраиваем BoxCollider2D
        BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(1.5f, 1.5f);
        boxCollider.offset = new Vector2(0, 0.5f); // смещение коллайдера
        
        // Делаем коллайдер триггером (не блокирует движение, но генерирует события)
        boxCollider.isTrigger = true;
    }
}
```

### ⚡ События столкновений и триггеров:

| Тип | Методы | Назначение |
| --- | --- | --- |
| Триггер (`isTrigger = true`) | `OnTriggerEnter2D(Collider2D other)` | Зоны урона, сбор предметов, активация дверей |
| Столкновение (`isTrigger = false`) | `OnCollisionEnter2D(Collision2D collision)` | Физический удар, прыжок на платформу |

```csharp
// Триггер — сбор монеты
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Destroy(gameObject);
        ScoreManager.AddPoints(10);
    }
}

// Столкновение — прыжок на врага
void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Enemy"))
    {
        // Проверяем, что игрок сверху
        float contactY = collision.contacts[0].point.y;
        float playerBottom = transform.position.y - GetComponent<Collider2D>().bounds.extents.y;
        
        if (contactY > playerBottom)
        {
            // Убить врага
            Destroy(collision.gameObject);
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
        }
    }
}
```

---

## 4. Слои (Layers и Sorting Layers) — организация коллизий и отрисовки
Unity имеет две разные системы слоёв для 2D физики:
### 🎨 Sorting Layers (сортировка отрисовки)
Определяет порядок рендеринга спрайтов (кто поверх кого).

| Sorting Layer | Порядок (чем ниже, тем дальше) |
| --- | --- |
| Background | -1 (рисуется первым) |
| Default | 0 (по умолчанию) |
| Foreground | 1 (поверх Default) |
| UI | 2 (поверх всего) |

```csharp
// Настройка Sorting Layer через код
SpriteRenderer sr = GetComponent<SpriteRenderer>();
sr.sortingLayerName = "Foreground";
sr.sortingOrder = 5; // порядок внутри слоя (выше = поверх)
```

### 🔒 Layers (для физики)
Layers используются для настройки коллизий (кто с кем сталкивается).

Настройка Physics2D Matrix:
1. `Edit > Project Settings > Physics 2D > Layer Collision Matrix`
2. Ставим галочки — слои сталкиваются, убираем — игнорируют.

```csharp
// Установка Layer через код
gameObject.layer = LayerMask.NameToLayer("Player");

// Игнорирование коллизий между двумя слоями
Physics2D.IgnoreLayerCollision(
    LayerMask.NameToLayer("Player"),
    LayerMask.NameToLayer("Enemy"),
    ignore: true
);
```

### 📋 Пример матрицы коллизий:

| Слой | Player | Enemy | Ground | Collectible |
| --- | --- | --- | --- | --- |
| Player | ✅ | ✅ | ✅ | ✅ |
| Enemy | ✅ | ✅ | ✅ | ❌ |
| Ground | ✅ | ✅ | ✅ | ❌ |
| Collectible | ✅ | ❌ | ❌ | ❌ |

---

## 5. Физические материалы (PhysicsMaterial2D)
`PhysicsMaterial2D` настраивает трение и упругость объектов.

| Свойство | Описание |
| --- | --- |
| `friction` | Сила трения (0 = лёд, 1 = резина) |
| `bounciness` | Упругость (0 = без отскока, 1 = идеальный отскок) |

```csharp
// Создание материала в коде (или через Asset)
PhysicsMaterial2D bouncyMat = new PhysicsMaterial2D("Bouncy");
bouncyMat.bounciness = 0.8f;
bouncyMat.friction = 0.2f;

// Применение к коллайдеру
BoxCollider2D collider = GetComponent<BoxCollider2D>();
collider.sharedMaterial = bouncyMat;
```

---

## 6. Полный пример: 2D платформер с физикой
```csharp
using UnityEngine;

public class Platformer2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D groundCheck;
    
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public LayerMask groundLayer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;
        
        groundCheck = GetComponent<BoxCollider2D>();
        groundCheck.isTrigger = true; // триггер для проверки пола
    }
    
    void Update()
    {
        // Горизонтальное движение
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        
        // Прыжок
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    
    private bool IsGrounded()
    {
        // Проверка через OverlapCircle
        Vector2 feetPos = transform.position - Vector3.up * 0.5f;
        return Physics2D.OverlapCircle(feetPos, 0.3f, groundLayer);
    }
    
    void OnDrawGizmos()
    {
        // Визуализация проверки пола
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Vector2 feetPos = transform.position - Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(feetPos, 0.3f);
    }
}
```

---

## 7. Полезные методы Physics2D

| Метод | Назначение |
| --- | --- |
| `Physics2D.Raycast` | Бросает луч (проверка линии прямой видимости) |
| `Physics2D.OverlapCircle` | Проверяет зону вокруг точки |
| `Physics2D.OverlapBox` | Проверяет прямоугольную область |
| `Physics2D.Linecast` | Проверяет сегмент линии между точками |
| `Physics2D.IgnoreCollision(collider1, collider2)` | Временно игнорирует коллизии |

```csharp
// Луч из позиции игрока вправо
RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 10f);
if (hit.collider != null)
{
    Debug.Log("Hit: " + hit.collider.name);
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
