# 🎮 Ввод данных в Unity: старый Input Manager (GetKey, GetAxis, GetMouseButton)

> [!NOTE]
>Этот материал посвящён старому Input Manager в Unity — встроенной системе обработки ввода с клавиатуры, мыши и джойстика.<br>
>Несмотря на появление новой системы Input System, старый менеджер остаётся простым и удобным для обучения, прототипов и многих 2D/3D проектов.

---

## 🧠 Основные методы ввода

## 1. GetKey / GetKeyDown / GetKeyUp

### Для чего: 
Обработка конкретных клавиш клавиатуры.

### Как использовать:
- `Input.GetKey(KeyCode.Space)` – пока зажата клавиша, возвращает `true` (каждый кадр).
- `Input.GetKeyDown(KeyCode.W)` – только в том кадре, когда клавишу нажали.
- `Input.GetKeyUp(KeyCode.LeftShift)` – только в том кадре, когда клавишу отпустили.

### Пример:
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
    {
        Jump(); // прыжок ровно один раз за нажатие
    }

    float speed = Input.GetKey(KeyCode.LeftShift) ? 10f : 5f;
    Move(speed);
}
```

## 2. GetMouseButton / GetMouseButtonDown / GetMouseButtonUp

### Для чего:
Обработка кнопок мыши.

### Индексы кнопок:
- `0` – левая кнопка
- `1` – правая кнопка
- `2` – средняя (колесо)

### Как использовать:
- `Input.GetMouseButton(0)` – пока зажата левая кнопка.
- `Input.GetMouseButtonDown(1)` – одно нажатие правой кнопки.
- `Input.GetMouseButtonUp(2)` – отпустили среднюю.

### Пример:

```csharp
void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Shoot(); // выстрел по клику
    }

    if (Input.GetMouseButton(1))
    {
        Aim(); // прицеливание, пока зажата правая кнопка
    }
}
```

## 3. GetAxis – плавное управление

### Для чего:
Плавные значения для движения, поворота, камеры.<br>
Возвращает число от -1 до 1.

### Главные оси:
- `"Horizontal"` – клавиши A / D или ← / →
- `"Vertical"` – клавиши W / S или ↑ / ↓
- `"Mouse X"` – горизонтальное движение мыши
- `"Mouse Y"` – вертикальное движение мыши

### Как использовать:

```csharp
void Update()
{
    float moveX = Input.GetAxis("Horizontal");
    float moveZ = Input.GetAxis("Vertical");
    Vector3 movement = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
    transform.Translate(movement);
}
```

### Важно:
- `GetAxis` – плавное изменение с ускорением/замедлением.
- `GetAxisRaw` – мгновенное (-1, 0, 1) для точных платформеров.

---

## ⚙️ Настройка осей (Input Manager)

Окно: `Edit → Project Settings → Input Manager`

### Вы можете:
- Добавить новую ось (`Size++`)
- Задать клавиши (`Positive Button`, `Negative Button`)
- Изменить чувствительность (`Sensitivity`) и затухание (`Gravity`)

### Пример кастомной оси для прыжка:
- Name: `Jump`
- Positive Button: `space`
- Type: `Key or Mouse Button`

### Использование в коде:
```csharp
if (Input.GetButtonDown("Jump")) { ... }
```

---

## 🧪 Совмещение методов

```csharp
void Update()
{
    // Движение через оси
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");
    Move(h, v);

    // Стрельба мышью
    if (Input.GetMouseButtonDown(0))
        Fire();

    // Переключение оружия колёсиком
    float scroll = Input.GetAxis("Mouse ScrollWheel");
    if (scroll != 0)
        SwitchWeapon(scroll > 0 ? 1 : -1);
}
```

---

## ⚠️ Когда использовать старый Input Manager

### ✅ Хорошо подходит для:
- Обучения и простых игр
- Прототипов (jam, solo dev)
- 2D платформеров, аркад, казуальных игр
- Управления с клавы + мыши без сложных комбинаций

### ❌ Недостатки:
- Сложная поддержка геймпадов
- Неудобная перенастройка клавиш в runtime
- Смешение осей при нескольких джойстиках

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
