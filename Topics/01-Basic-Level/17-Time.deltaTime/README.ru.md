# ⏱️ Time.deltaTime и масштаб времени в Unity

> [!Note]
> Этот материал объясняет, как работать со временем в Unity, чтобы ваши игры работали одинаково на устройствах с разной частотой кадров (FPS).
> Вы узнаете о `Time.deltaTime`, зависимости от кадров, управлении паузой и замедлением времени через `Time.timeScale`, а также о `Time.fixedDeltaTime` для физики.

---

## 🎞️ Зависимость от кадров (Frame Rate Dependence)

### Проблема:
Если вы перемещаете объект так:
```csharp
transform.Translate(5, 0, 0); // +5 метров по X каждый кадр
```
- На мощном ПК (60 FPS) объект пройдёт `5 * 60 = 300` метров за секунду.
- На слабом устройстве (30 FPS) — `5 * 30 = 150` метров за секунду.
- Итог: скорость зависит от производительности, что неприемлемо для игры.

### Решение:
Использовать время, прошедшее между кадрами — `Time.deltaTime`.

---

## ⏲️ Time.deltaTime

### Что это:
Время в секундах, которое прошло с момента последнего кадра. В `Update()` он показывает длительность предыдущего кадра.

### Для чего нужно:
Делать движение и любые непрерывные изменения независимыми от частоты кадров.

### Как использовать:
```csharp
void Update()
{
    // Скорость 5 метров в секунду (а не в кадр)
    float speed = 5f;
    transform.Translate(speed * Time.deltaTime, 0, 0);
}
```

- При 60 FPS: `deltaTime ≈ 0.0167`, за секунду `5 * 0.0167 * 60 ≈ 5` метров.
- При 30 FPS: `deltaTime ≈ 0.0333`, за секунду `5 * 0.0333 * 30 ≈ 5` метров.
- Результат: одинаковое расстояние за секунду на любом FPS.

### Примеры применения:
- Движение персонажа: `velocity * Time.deltaTime`
- Вращение: `transform.Rotate(0, 90 * Time.deltaTime, 0)`
- Плавное изменение цвета/прозрачности
- Таймеры обратного отсчёта:
```csharp
timer -= Time.deltaTime;
if (timer <= 0) { /* действие */ }
```

---

## ⏸️ Time.timeScale (Масштаб времени)

### Что это:
Глобальный множитель скорости времени. По умолчанию `1.0`.

### Значения:
- `1.0` — нормальная скорость
- `0.5` — замедление в 2 раза
- `2.0` — ускорение в 2 раза
- `0.0` — полная остановка времени (пауза)

> [!Important]
> `Time.timeScale` влияет на `Time.deltaTime`, но не влияет на `Time.unscaledDeltaTime` (реальное время без масштаба).

### Как использовать:
```csharp
// Пауза
Time.timeScale = 0f;

// Замедление (bullet time, slow-mo)
Time.timeScale = 0.3f;

// Возврат к норме
Time.timeScale = 1f;
```

### Пример: Пауза через пробел
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
    {
        // Переключаем паузу
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;  // Продолжить
        else
            Time.timeScale = 0f;  // Пауза
    }
}
```

### Игнорирование Time.timeScale (интерфейс паузы)
Если нужно, чтобы UI (меню паузы, анимация курсора) продолжало работать при паузе:
```csharp
// Используйте unscaledDeltaTime
void Update()
{
    float realTime = Time.unscaledDeltaTime;
    // Анимация кнопок, мигание курсора и т.д.
}
```

---

## ⚙️ Time.fixedDeltaTime (Фиксированное время)

### Что это:
Интервал между вызовами метода `FixedUpdate()`. По умолчанию `0.02` секунды (50 раз в секунду).

### Для чего нужно:
`FixedUpdate()` используется для физики (Rigidbody, столкновения, силы). В отличие от `Update()`, он вызывается через равные промежутки времени, независимо от FPS.

### Связь с Time.timeScale:
`Time.fixedDeltaTime` автоматически умножается на `Time.timeScale`, чтобы физика тоже замедлялась/ускорялась.

### Важное правило:
- Движение, поворот, анимацию, таймеры → `Update()` + `Time.deltaTime`
- Физику (силы, скорость Rigidbody, импульсы) → `FixedUpdate()` + `Time.fixedDeltaTime`

### Пример: Движение персонажа с физикой
```csharp
public float moveSpeed = 5f;
private Rigidbody rb;

void Start()
{
    rb = GetComponent<Rigidbody>();
}

void FixedUpdate()
{
    float horizontal = Input.GetAxis("Horizontal");
    Vector3 movement = new Vector3(horizontal, 0, 0) * moveSpeed;
    
    // Для физики используем fixedDeltaTime
    rb.velocity = movement * Time.fixedDeltaTime;
}
```

### Изменение Time.fixedDeltaTime вручную
Если вы сильно меняете `Time.timeScale` (например, замедление x0.1), Unity автоматически подстраивает `fixedDeltaTime`. Но если нужно вручную:
```csharp
Time.fixedDeltaTime = 0.02f; // сброс к стандарту
```
Или при динамическом изменении таймскейла:
```csharp
Time.timeScale = 0.5f;
Time.fixedDeltaTime = 0.02f * Time.timeScale;
```

---

## 📊 Сводная таблица

| Параметр | Где используется | Зависит от timeScale | Назначение |
|-----------------|------------|----------|-----------------------|
| `Time.deltaTime` | `Update()` | ✅ Да | Плавное движение, таймеры, анимация |
| `Time.unscaledDeltaTime` | `Update()` | ❌ Нет | UI, эффекты во время паузы |
| `Time.fixedDeltaTime` | `FixedUpdate()` | ✅ Да | Физика, силы, velocity |
| `Time.timeScale` | Глобально | — | Пауза, замедление/ускорение |

---

## 🎮 Практический пример (всё вместе)
Представьте игру-гонку. При нажатии `R` включается замедление (bullet time), а при `P` — пауза:
```csharp
void Update()
{
    // Нормальное движение (зависит от timeScale)
    float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
    transform.Translate(move, 0, 0);
    
    // Замедление
    if (Input.GetKeyDown(KeyCode.R))
        Time.timeScale = 0.3f;
    
    // Возврат к норме
    if (Input.GetKeyDown(KeyCode.T))
        Time.timeScale = 1f;
    
    // Пауза
    if (Input.GetKeyDown(KeyCode.P))
    {
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;
        else
            Time.timeScale = 0f;
    }
}

void FixedUpdate()
{
    // Физика машины (автоматически замедлится при timeScale = 0.3)
    rb.AddForce(transform.forward * engineForce * Time.fixedDeltaTime);
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
