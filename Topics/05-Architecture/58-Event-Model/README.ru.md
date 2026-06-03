# 📡 Event Model в Unity: Глобальная система событий

Этот материал посвящён созданию глобальной системы событий в Unity для слабосвязанного (decoupled) кода. 
Вы узнаете о трёх основных подходах: Delegate, Action (встроенные делегаты) и ScriptableObject Channels 
(каналы событий на основе ScriptableObject). Каждый подход имеет свои преимущества и сценарии использования.

---

## 1. 🧩 Зачем нужна глобальная система событий?
В типичном игровом проекте объектам часто нужно обмениваться информацией:
- Игрок набрал очки → UI должен обновиться.
- Персонаж получил урон → система здоровья должна среагировать.
- Враг умер → спавнер должен создать нового.

### Проблема прямых ссылок:
```csharp
public class Player : MonoBehaviour {
    public UIManager uiManager; // Прямая ссылка — жёсткая связь
    void OnScore() {
        uiManager.UpdateScore(); // Зависимость от конкретного UI
    }
}
```

### Что даёт событийная модель:
- 📉 Снижение связанности — отправитель не знает о получателе.
- 🔧 Лёгкость поддержки — можно добавить новых слушателей без изменения отправителя.
- 🧪 Удобство тестирования — подсистемы можно тестировать изолированно.

---

## 2. 📜 Delegate (делегат) — классический подход
### Что это:
Delegate (делегат) — это тип, который хранит ссылку на метод с определённой сигнатурой. Это базовый механизм событий в C#.

### Как использовать:
```csharp
// 1. Объявляем делегат (обычно вне класса или внутри как public)
public delegate void ScoreChangedDelegate(int newScore);

// 2. Создаём событие на основе делегата
public class ScoreManager : MonoBehaviour {
    public event ScoreChangedDelegate OnScoreChanged;
    
    private int score;
    
    public void AddScore(int points) {
        score += points;
        // 3. Вызываем событие (если есть подписчики)
        OnScoreChanged?.Invoke(score);
    }
}

// 4. Подписываемся на событие в другом классе
public class UIManager : MonoBehaviour {
    public ScoreManager scoreManager;
    
    void Start() {
        scoreManager.OnScoreChanged += UpdateScoreUI;
    }
    
    void UpdateScoreUI(int newScore) {
        Debug.Log($"Score: {newScore}");
    }
    
    void OnDestroy() {
        scoreManager.OnScoreChanged -= UpdateScoreUI; // ОТПИСКА ВАЖНА!
    }
}
```

### ✅ Плюсы:
- Встроенный механизм C# (нет зависимостей).
- Поддержка нескольких подписчиков.
- Хорошая производительность.

### ❌ Минусы:
- Нужно вручную управлять подпиской/отпиской (риск утечек памяти).
- Сильная связь через прямые ссылки (ScoreManager должен быть известен UIManager).
- Неудобно для глобальной системы — обычно нужен синглтон-менеджер.

---

## 3. 🚀 Action / Func / UnityEvent — встроенные делегаты
### Что это:
`Action<T>` — это готовый делегат от Microsoft (не нужно объявлять свой). UnityEvent — сериализуемая версия, видимая в инспекторе.

### 3.1 Action (System.Action)
```csharp
public class ScoreManager : MonoBehaviour {
    public static Action<int> OnScoreChanged; // Глобальное событие (статическое)
    // Или нестатическое: public Action<int> OnScoreChanged;
    
    private int score;
    
    void AddScore(int points) {
        score += points;
        OnScoreChanged?.Invoke(score);
    }
}

// Любой класс может подписаться без прямой ссылки
public class AchievementSystem : MonoBehaviour {
    void Start() {
        ScoreManager.OnScoreChanged += UnlockAchievements;
    }
    
    void UnlockAchievements(int score) {
        if(score >= 100) Debug.Log("Achievement: 100 points!");
    }
    
    void OnDestroy() {
        ScoreManager.OnScoreChanged -= UnlockAchievements;
    }
}
```

### 3.2 UnityEvent (сериализуемый)
```csharp
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour {
    public UnityEvent<int, int> OnDamageTaken; // Виден в инспекторе!
    // Первый int — урон, второй — текущее здоровье
    
    public void TakeDamage(int damage) {
        int newHealth = currentHealth - damage;
        OnDamageTaken?.Invoke(damage, newHealth);
    }
}
```

В инспекторе можно привязать методы других объектов визуально (без кода).

### ✅ Плюсы:
- `Action` — минималистично, нет лишнего кода.
- `UnityEvent` — виден в инспекторе, можно настраивать через UI.

### ❌ Минусы (общие):
- Статические события глобальны, но могут создавать утечки (легко забыть отписаться).
- Сложно дебажить (кто подписался? кто вызвал?).

---

## 4. 🎯 ScriptableObject Channels (каналы событий) — рекомендуемый подход для средних/больших проектов
### Что это:
ScriptableObject используется как канал (channel) для передачи событий между объектами без прямых ссылок. Это паттерн «Шина событий» (Event Bus) на основе ScriptableObject.

### Как создать свою систему:
Шаг 1: Базовый канал событий
```csharp
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/IntEventChannel")]
public class IntEventChannelSO : ScriptableObject {
    public UnityAction<int> OnEventRaised;
    
    public void RaiseEvent(int value) {
        OnEventRaised?.Invoke(value);
    }
}
```

Шаг 2: Создание каналов в редакторе
- Правой кнопкой по папке `Assets` → `Create` → `Events` → `IntEventChannelSO`
- Назовите: `ScoreChangedChannel`, `PlayerHealthChannel`, `EnemyDiedChannel` и т.д.

Шаг 3: Отправитель (Publisher)
```csharp
public class ScoreManager : MonoBehaviour {
    [SerializeField] private IntEventChannelSO scoreChannel;
    
    private int score;
    
    public void AddScore(int points) {
        score += points;
        scoreChannel.RaiseEvent(score); // Отправляем событие через канал
    }
}
```

Шаг 4: Получатель (Subscriber)
```csharp
public class UIManager : MonoBehaviour {
    [SerializeField] private IntEventChannelSO scoreChannel;
    
    void Start() {
        scoreChannel.OnEventRaised += UpdateScoreUI;
    }
    
    void UpdateScoreUI(int newScore) {
        scoreText.text = $"Score: {newScore}";
    }
    
    void OnDestroy() {
        scoreChannel.OnEventRaised -= UpdateScoreUI;
    }
}
```

Шаг 5: Настройка в инспекторе
- Перетащите созданный `ScoreChangedChannel` в поле `scoreChannel` у `ScoreManager` и у `UIManager`.

### 🎁 Расширенный вариант: Generic канал
```csharp
public class EventChannelSO<T> : ScriptableObject {
    public UnityAction<T> OnEventRaised;
    public void RaiseEvent(T value) => OnEventRaised?.Invoke(value);
}

// Создаём конкретные типы:
// IntEventChannel : EventChannelSO<int>
// StringEventChannel : EventChannelSO<string>
// GameObjectEventChannel : EventChannelSO<GameObject>
```

### ✅ Плюсы ScriptableObject Channels:
- 🧵 Нет глобальных статических переменных — каждый канал живёт как ассет.
- 🎨 Визуальная настройка в инспекторе — легко менять связи без кода.
- 🔍 Легко дебажить — можно посмотреть, кто подписан на канал.
- 🧩 Слабая связанность — отправитель вообще не знает о получателе.
- ♻️ Переиспользование — один канал можно использовать в разных сценах.

### ❌ Минусы:
- Нужно создавать много ассетов (по одному на тип события).
- Чуть сложнее для новичков.

---

## 5. 📊 Сравнение подходов

| Характеристика | Delegate | Action | UnityEvent | ScriptableObject Channel |
| --- | --- | --- | --- | --- |
| Сложность | Средняя | Низкая | Низкая | Средняя |
| Виден в инспекторе | ❌ | ❌ | ✅ | ✅ |
| Глобальность | Через статику | Через статику | ❌ | ✅ (как ассет) |
| Риск утечек памяти | Высокий | Высокий | Средний | Низкий |
| Переиспользование между сценами | ❌ | ❌ | ❌ | ✅ |
| Рекомендуется для | Простых проектов | Простых/средних | UI/инспектор | Средних/больших проектов |

---

## 6. 🎮 Готовая архитектура (рекомендация)
Для игрового проекта среднего размера используйте комбинацию:
### 1. ScriptableObject Channels — для глобальных игровых событий:
- `PlayerDeathChannel`, `ScoreChangedChannel`, `LevelCompletedChannel`

### 2. UnityEvent — для локальных событий внутри префаба:
- Кнопка UI → анимация

### 3. Action — для простых временных связей внутри одного скрипта

Пример архитектуры:
```csharp
Assets/
  Scripts/
    Events/
      Channels/
        IntEventChannelSO.cs
        StringEventChannelSO.cs
      Listeners/
        IntEventListener.cs (опционально, для визуального реагирования)
    Game/
      ScoreManager.cs (отправитель)
      UIManager.cs (получатель)
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
