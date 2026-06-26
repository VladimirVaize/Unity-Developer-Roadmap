# 🎮 Новый Input System в Unity: Action Maps, Processors, Interactions — современная замена старого Input Manager
Unity Input System — это полностью переработанная система ввода, пришедшая на смену устаревшему Input Manager. 
Она решает фундаментальные проблемы старого подхода: жесткую привязку к конкретным клавишам, спагетти-код из 
Input.GetKey в Update() и невозможность легко переключаться между разными устройствами ввода.

---

## 🔥 Почему Input System лучше старого подхода?
Сравнение старого и нового подходов:

| Аспект | Старый Input Manager | Новый Input System |
| --- | --- | --- |
| Привязка к клавишам | Жесткая (`KeyCode.W`) | Абстрактная (Action "Move") |
| Переключение устройств | Ручное, через `#if` | Автоматическое, через Control Schemes |
| Поддержка геймпадов | Ограниченная | Полная, с вибрацией |
| Мобильные устройства | Отдельная логика | Единая система |
| Тестирование | Только на реальном устройстве | Эмуляция в редакторе |
| Код | `Update()` с кучей проверок | Событийно-ориентированный |

---

## 1. Основные понятия Input System
Новая система построена на трех ключевых абстракциях:

| Понятие | Описание | Пример |
| --- | --- | --- |
| Action | Абстрактное действие игрока | "Move", "Jump", "Fire" |
| Binding | Привязка действия к конкретным кнопкам/устройствам | Клавиша `W` → Move |
| Action Map | Группа действий для одного контекста | "Gameplay", "UI", "Menu" |

### 📁 Создание Input Action Asset
1. Project Window → Create → Input Actions
2. Назовите, например, `PlayerControls`
3. Двойной клик открывает Input Action Editor

---

## 2. Action Maps — контекстные группы действий
Action Maps позволяют группировать действия по сценариям использования, избегая конфликтов ввода.

### 🧱 Типовая структура Action Maps:
```text
PlayerControls
├── Gameplay 🎮  (управление игроком в игре)
│   ├── Move (WASD / Left Stick)
│   ├── Jump (Space / Button South)
│   ├── Fire (Left Mouse / Right Trigger)
│   └── Look (Mouse Delta / Right Stick)
├── UI 📋  (управление интерфейсом)
│   ├── Navigate (Arrows / D-Pad)
│   ├── Submit (Enter / Button South)
│   └── Cancel (Escape / Button East)
└── Menu 📱  (меню паузы)
    ├── Pause (Escape / Start)
    └── Select (Mouse Click / Button South)
```

### 🎮 Пример использования Action Map:
```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls _controls;
    
    private void Awake()
    {
        _controls = new PlayerControls();
        
        // Подписка на события из Action Map "Gameplay"
        _controls.Gameplay.Jump.performed += OnJump;
        _controls.Gameplay.Fire.performed += OnFire;
    }
    
    private void OnEnable()
    {
        // Включаем только нужный Action Map
        _controls.Gameplay.Enable();
        _controls.UI.Disable();  // UI отключаем в игре
    }
    
    private void OnDisable()
    {
        _controls.Gameplay.Disable();
    }
    
    private void Update()
    {
        // Получение значения из Action
        Vector2 moveInput = _controls.Gameplay.Move.ReadValue<Vector2>();
        transform.Translate(new Vector3(moveInput.x, 0, moveInput.y) * Time.deltaTime * speed);
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Прыжок!");
        }
    }
}
```

### 🔄 Переключение между Action Maps:
```csharp
public class UIManager : MonoBehaviour
{
    private PlayerControls _controls;
    
    public void OpenPauseMenu()
    {
        // Отключаем игровой ввод, включаем UI
        _controls.Gameplay.Disable();
        _controls.UI.Enable();
        _controls.Menu.Enable();
    }
    
    public void ClosePauseMenu()
    {
        // Возвращаем управление в игре
        _controls.UI.Disable();
        _controls.Menu.Disable();
        _controls.Gameplay.Enable();
    }
}
```

---

## 3. Processors — обработка значений
Processors преобразуют сырой сигнал от устройства перед тем, как он попадёт в ваше действие. Они применяются на трёх уровнях:
1. На Binding — для конкретной привязки
2. На Action — для всех привязок действия
3. На Control — на уровне устройства

### 📋 Популярные Processors:

| Processor | Описание | Пример использования |
| --- | --- | --- |
| `Invert` | Инвертирует значение (ось Y) | Инверсия камеры мыши |
| `AxisDeadzone` | Удаляет малые значения | Устранение дрифта стика |
| `StickDeadzone` | Мёртвая зона для стика | Геймпад: маленькие движения |
| `Scale` | Масштабирует значение | Уменьшение чувствительности камеры |
| `Clamp` | Ограничивает значение | Ограничение значений |
| `Normalize` | Нормализует вектор | Вектор длины 1 |

### ⚙️ Настройка Processors в редакторе:
1. Выберите Binding или Action в Input Action Editor
2. В правой панели найдите Processors
3. Нажмите `+` и выберите нужный Processor
4. Настройте параметры (если есть)

### В коде:
```csharp
// Добавление Processor к Binding
action.AddBinding("<Gamepad>/leftStick")
    .WithProcessor("invertVector2(invertX=false)");
```

### 🎯 Пример: Инверсия камеры + Масштабирование
```text
// В Input Action Editor:
// Action: "Look"
// Binding: Mouse / Delta
// Processors:
//   1. Invert (invertY=true)  — инвертируем вертикаль
//   2. Scale (scale=0.003)    — уменьшаем чувствительность
```
Результат: Мышь контролирует камеру, но вертикаль инвертирована, а чувствительность снижена.

### 🎯 Пример: Мёртвая зона для геймпада
```text
// В Input Action Editor:
// Action: "Move"
// Binding: Gamepad / Left Stick
// Processors:
//   1. StickDeadzone (min=0.2)  — удаляем дрифт
```

---

## 4. Interactions — определение паттернов нажатий
Interactions определяют, как система интерпретирует сигнал от устройства. 
Вместо того чтобы проверять `GetKeyDown` в коде, вы описываете поведение на уровне Asset'а.

### 📋 Популярные Interactions:

| Interaction | Описание | Сигнал |
| --- | --- | --- |
| `Tap` | Одиночное нажатие (быстрое) | `performed` при отпускании |
| `Hold` | Долгое нажатие (удержание) | `performed` по истечении времени |
| `Press` | Нажатие (любое) | `started` / `performed` / `canceled` |
| `SlowTap` | Медленное нажатие | `performed` при отпускании |
| `MultiTap` | Многократное нажатие | Двойной/тройной клик |

### ⚙️ Настройка Interactions в редакторе:
1. Выберите Binding или Action
2. В правой панели найдите Interactions
3. Нажмите `+` и выберите нужный Interaction
4. Настройте параметры:
   - `Tap`: `pressPoint=0.5` (порог срабатывания)
   - `Hold`: `duration=0.5` (время удержания)
   - `MultiTap`: `tapCount=2`, `maxTapSpacing=0.5`
  
### 🎯 Пример 1: Одиночный и двойной клик
```text
// В Input Action Editor:
// Action 1: "Select"  → Tap
// Action 2: "SelectDouble" → MultiTap (tapCount=2)
```

#### В коде:
```csharp
_controls.Gameplay.Select.performed += ctx => SelectItem();
_controls.Gameplay.SelectDouble.performed += ctx => SelectAllItems();
```

### 🎯 Пример 2: Долгое нажатие для спецспособности
```text
// В Input Action Editor:
// Action: "SpecialAttack"
// Binding: Button East (B)
// Interactions: Hold (duration=1.0)
```

#### В коде:
```csharp
_controls.Gameplay.SpecialAttack.performed += ctx => 
{
    Debug.Log("💥 Огненный шар!");
};
```

### 🎯 Пример 3: Прыжок от нажатия (не удержания)
```text
// В Input Action Editor:
// Action: "Jump"
// Binding: Space
// Interactions: Press (behavior=PressOnly)
```

В коде `performed` сработает только при нажатии, а не при удержании.

### 📝 Полный пример: Движение + Прыжок + Спецспособность
```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class AdvancedPlayerController : MonoBehaviour
{
    [Header("Input")]
    public PlayerControls controls;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    private Vector2 moveInput;
    private bool isJumping;
    
    private void Awake()
    {
        controls = new PlayerControls();
        
        // Движение (Value)
        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;
        
        // Прыжок (Press)
        controls.Gameplay.Jump.performed += ctx => Jump();
        
        // Спецспособность (Hold)
        controls.Gameplay.SpecialAttack.performed += ctx => FireSpecial();
    }
    
    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }
    
    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
    
    private void Update()
    {
        transform.Translate(new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.deltaTime);
    }
    
    private void Jump()
    {
        Debug.Log("🦘 Прыжок!");
        // Логика прыжка
    }
    
    private void FireSpecial()
    {
        Debug.Log("🔥 Огненный шар!");
    }
}
```

---

## 5. Control Schemes — поддержка разных устройств
Control Schemes позволяют настраивать разные наборы привязок для разных типов контроллеров.

### 🎮 Пример: Настройка для двух устройств
```yaml
Control Scheme 1: "Keyboard&Mouse"
  - Move: WASD
  - Jump: Space
  - Fire: Left Mouse

Control Scheme 2: "Gamepad"
  - Move: Left Stick
  - Jump: Button South (A)
  - Fire: Right Trigger
```

### 🔄 Переключение между схемами:
```csharp
public class InputDeviceManager : MonoBehaviour
{
    private PlayerControls _controls;
    
    void Start()
    {
        _controls = new PlayerControls();
        
        // Определяем активное устройство
        var devices = InputSystem.devices;
        bool hasGamepad = devices.Any(d => d is Gamepad);
        
        if (hasGamepad)
        {
            _controls.bindingMask = InputBinding.MaskByGroup("Gamepad");
        }
        else
        {
            _controls.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
        }
    }
}
```

---

## 6. Генерация C# класса
При создании Input Action Asset можно автоматически сгенерировать C# класс-обёртку:
1. В инспекторе `.inputactions` ассета
2. Галочка Generate C# Class
3. Кнопка Apply

### Преимущества:
- Type-Safe доступ к действиям
- Intellisense в коде
- Автоматическое создание классов для каждого Action Map

---

## 7. Лучшие практики
### ✅ Рекомендации:
1. Используйте Action Maps — разделяйте Gameplay, UI, Menu
2. Генерируйте C# класс — удобно и безопасно
3. Не смешивайте старый и новый Input System — выберите одно
4. Используйте Events, не опрос в Update — событийный подход лучше
5. Настраивайте Processors в Asset'е — код остаётся чистым
6. Тестируйте в редакторе с разными устройствами — используйте Input Debugger

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Смешивание со старым Input Manager
if (Input.GetKeyDown(KeyCode.Space)) { }  // Не делайте так!
// ✅ ПРАВИЛЬНО: Используйте только Input System

// ❌ ОШИБКА: Не вызывать Enable() для Action Map
controls.Gameplay.Move.ReadValue<Vector2>();  // NullReference!
// ✅ ПРАВИЛЬНО: controls.Gameplay.Enable();

// ❌ ОШИБКА: Забыть отписаться от событий
controls.Gameplay.Jump.performed += OnJump;  // Утечка памяти
// ✅ ПРАВИЛЬНО: В OnDisable отписаться

// ❌ ОШИБКА: Не использовать Action Maps для разных режимов
// Всё в одной куче → конфликты ввода
// ✅ ПРАВИЛЬНО: Gameplay, UI, Menu разделены
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
