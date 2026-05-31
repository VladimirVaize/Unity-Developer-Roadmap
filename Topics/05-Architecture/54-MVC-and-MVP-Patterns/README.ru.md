# 🧩 Паттерны MVC и MVP в Unity: Разделение данных, представления и логики (особенно для UI)

При разработке интерфейсов в Unity легко запутаться, когда логика, данные и отображение смешаны в одном скрипте 
(`Update()` проверяет нажатия, меняет текст, обновляет здоровье и т.д.). 
MVC (Model-View-Controller) и MVP (Model-View-Presenter) — это архитектурные паттерны, которые разделяют приложение на три независимые части. 
Это делает код понятным, тестируемым и легко изменяемым.

---

## 1. 📌 Проблема: типичный "хаос" в UI-скрипте
```csharp
// Плохо: всё в одном месте
public class PlayerUIBad : MonoBehaviour {
    public Text healthText;
    public Slider healthSlider;
    public Button jumpButton;
    int health = 100;

    void Start() {
        jumpButton.onClick.AddListener(() => {
            // логика прыжка
            Debug.Log("Jump!");
        });
    }

    void Update() {
        // отображение
        healthText.text = "HP: " + health;
        healthSlider.value = health;
    }

    public void TakeDamage(int dmg) {
        health -= dmg; // бизнес-логика
        // смешано с UI-обновлением
    }
}
```
Проблемы: сложно тестировать, менять UI (с Text на Slider), переиспользовать логику.

---

## 2. 🧠 Паттерн MVC (Model-View-Controller)
### Компоненты:

| Компонент | Роль | Где живёт в Unity |
| --- | --- | --- |
| Model | Данные и бизнес-логика (здоровье, счёт, правила). Не знает о View/Controller. | Обычный C# класс (не `MonoBehaviour`) |
| View | Отображение данных (текст, полоски здоровья, анимации). Только визуал. | `MonoBehaviour` на UI-объекте |
| Controller | Посредник: принимает ввод (клики, клавиши), обновляет Model, говорит View обновиться. | `MonoBehaviour` (обычно на том же или отдельном объекте) |

### Схема работы:
```text
Пользователь → Controller → Model → (уведомление) → View → обновление UI
```

### Пример MVC в Unity (UI здоровья):
```csharp
// Model: чистые данные + события
public class HealthModel {
    public int Health { get; private set; }
    public System.Action<int> OnHealthChanged;

    public void TakeDamage(int amount) {
        Health -= amount;
        OnHealthChanged?.Invoke(Health);
    }
}

// View: только отображение
public class HealthView : MonoBehaviour {
    public Text healthText;
    public void UpdateHealth(int health) => healthText.text = $"HP: {health}";
}

// Controller: связывает Model и View
public class HealthController : MonoBehaviour {
    [SerializeField] HealthView view;
    HealthModel model = new HealthModel();

    void Start() {
        model.OnHealthChanged += view.UpdateHealth;
        model.TakeDamage(0); // начальное отображение
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            model.TakeDamage(10);
    }
}
```

Плюсы MVC: Model не зависит от Unity → легко юнит-тестировать.

Минусы: Controller часто раздувается; View может быть пассивной (не всегда удобно для сложного UI).

---

## 3. 🎭 Паттерн MVP (Model-View-Presenter) – предпочтительнее для Unity UI
Отличие от MVC: Presenter полностью управляет View, а View только вызывает методы Presenter (например, по нажатию кнопки). View не знает о Model. Presenter обновляет View через интерфейс.

### Компоненты MVP:

| Компонент | Роль |
| --- | --- |
| Model | Те же данные и бизнес-логика (C# класс). |
| View | Пассивный интерфейс (только отображение и события ввода). Реализует интерфейс `IView`. |
| Presenter | Содержит логику, подписывается на события View, обновляет Model и View. |

### Схема:
```text
Пользователь → View (вызывает метод Presenter) → Presenter → Model
                ← Presenter обновляет View через интерфейс ←
```

### Пример MVP в Unity (UI магазина очков):
```csharp
// Model
public class ScoreModel {
    public int Score { get; private set; }
    public void AddScore(int value) => Score += value;
}

// Интерфейс View
public interface IScoreView {
    void UpdateScoreDisplay(int score);
    event Action OnBuyButtonClicked;
}

// Реализация View (MonoBehaviour)
public class ScoreView : MonoBehaviour, IScoreView {
    [SerializeField] Text scoreText;
    [SerializeField] Button buyButton;

    public event Action OnBuyButtonClicked;

    void Awake() => buyButton.onClick.AddListener(() => OnBuyButtonClicked?.Invoke());

    public void UpdateScoreDisplay(int score) => scoreText.text = $"Score: {score}";
}

// Presenter (обычно не MonoBehaviour, но для удобства можно)
public class ScorePresenter {
    ScoreModel model;
    IScoreView view;

    public ScorePresenter(ScoreModel model, IScoreView view) {
        this.model = model;
        this.view = view;
        view.OnBuyButtonClicked += HandleBuy;
        UpdateView();
    }

    void HandleBuy() {
        // бизнес-логика
        if (model.Score >= 10) {
            model.AddScore(-10);
            UpdateView();
            Debug.Log("Item bought!");
        }
    }

    void UpdateView() => view.UpdateScoreDisplay(model.Score);
}

// В MonoBehaviour на сцене (Composition Root)
public class GameRoot : MonoBehaviour {
    [SerializeField] ScoreView scoreView;

    void Start() {
        var model = new ScoreModel();
        var presenter = new ScorePresenter(model, scoreView);
    }
}
```

### Плюсы MVP:
- View абсолютно глупая (легко заменить на другую реализацию).
- Presenter можно тестировать без Unity (нет `MonoBehaviour`).
- Чёткое разделение: UI-дизайнер может работать над View, программист — над Presenter/Model.

---

## 4. 🆚 MVC против MVP – что выбрать для Unity UI?

| Критерий | MVC | MVP |
| --- | --- | --- |
| Сложность реализации | Проще для маленьких проектов | Чуть сложнее, но лучше для масштаба |
| Тестируемость Model | ✅ | ✅ |
| Тестируемость логики управления | ❌ (Controller в MonoBehaviour) | ✅ (Presenter – обычный C#) |
| Смена UI-фреймворка (Text → TMP → Slider) | Средне | Легко (через интерфейс) |
| Когда использовать | Прототипы, простые UI | Большие проекты, сложное взаимодействие UI |

Рекомендация: Для серьёзного проекта с UI (инвентарь, магазин, настройки) используйте MVP. Для простого HUD (здоровье, патроны) подойдёт и упрощённый MVC.

---

## 5. 🔧 Практические советы для Unity
1. Model – чистый C#, не наследуйте от `MonoBehaviour`. Используйте `event` / `Action` для уведомлений.
2. View – только обращение к UI-компонентам (Text, Slider, Image). Без условной логики (`if (health < 0)` не должно быть во View).
3. Presenter / Controller – подписывайтесь на события View и события Model. Не держите ссылки на `GameObject` / `Transform`.
4. Composition Root – отдельный скрипт, который создаёт Model, Presenter и связывает с View на старте.
5. Для тестирования – напишите юнит-тест для Model и для Presenter (подставив мок-View).

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
