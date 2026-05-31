# 🧩 MVC and MVP Patterns in Unity: Separating Data, View, and Logic (Especially for UI)

When developing UI in Unity, it's easy to end up with a mess where logic, data, 
and display are all mixed in one script (`Update()` checks inputs, updates text, changes health, etc.). 
MVC (Model-View-Controller) and MVP (Model-View-Presenter) are architectural patterns that separate an application into three independent parts. 
This makes code clear, testable, and easy to modify.

---

## 1. 📌 The problem: typical "chaos" in a UI script
```csharp
// Bad: everything in one place
public class PlayerUIBad : MonoBehaviour {
    public Text healthText;
    public Slider healthSlider;
    public Button jumpButton;
    int health = 100;

    void Start() {
        jumpButton.onClick.AddListener(() => {
            // jump logic
            Debug.Log("Jump!");
        });
    }

    void Update() {
        // display
        healthText.text = "HP: " + health;
        healthSlider.value = health;
    }

    public void TakeDamage(int dmg) {
        health -= dmg; // business logic
        // mixed with UI update
    }
}
```
Problems: hard to test, change UI (from Text to Slider), reuse logic.

---

## 2. 🧠 MVC Pattern (Model-View-Controller)
### Components:

| Component | Role | Where in Unity |
| --- | --- | --- |
| Model | Data and business logic (health, score, rules). Knows nothing about View/Controller. | Plain C# class (not `MonoBehaviour`) |
| View | Displays data (text, health bars, animations). Visual only. | `MonoBehaviour` on a UI object |
| Controller | Mediator: receives input (clicks, keys), updates Model, tells View to update. | `MonoBehaviour` (usually on same or separate object) |

### Workflow:
```text
User → Controller → Model → (notification) → View → UI update
```

### MVC Example in Unity (Health UI):
```csharp
// Model: pure data + events
public class HealthModel {
    public int Health { get; private set; }
    public System.Action<int> OnHealthChanged;

    public void TakeDamage(int amount) {
        Health -= amount;
        OnHealthChanged?.Invoke(Health);
    }
}

// View: only display
public class HealthView : MonoBehaviour {
    public Text healthText;
    public void UpdateHealth(int health) => healthText.text = $"HP: {health}";
}

// Controller: binds Model and View
public class HealthController : MonoBehaviour {
    [SerializeField] HealthView view;
    HealthModel model = new HealthModel();

    void Start() {
        model.OnHealthChanged += view.UpdateHealth;
        model.TakeDamage(0); // initial display
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            model.TakeDamage(10);
    }
}
```

Pros: Model doesn't depend on Unity → easy unit testing.

Cons: Controller often becomes bloated; View can be passive (not always convenient for complex UI).

---

## 3. 🎭 MVP Pattern (Model-View-Presenter) – preferred for Unity UI
Difference from MVC: The Presenter fully controls the View, and the View only calls Presenter methods (e.g., on button click). 
The View does not know about the Model. The Presenter updates the View through an interface.

### MVP Components:

| Component | Role |
| --- | --- |
| Model | Same data and business logic (C# class). |
| View | Passive interface (display and input events only). Implements an `IView` interface. |
| Presenter | Contains logic, subscribes to View events, updates Model and View. |

### Workflow:
```text
User → View (calls Presenter method) → Presenter → Model
                ← Presenter updates View via interface ←
```

### MVP Example in Unity (Score shop UI):
```csharp
// Model
public class ScoreModel {
    public int Score { get; private set; }
    public void AddScore(int value) => Score += value;
}

// View interface
public interface IScoreView {
    void UpdateScoreDisplay(int score);
    event Action OnBuyButtonClicked;
}

// View implementation (MonoBehaviour)
public class ScoreView : MonoBehaviour, IScoreView {
    [SerializeField] Text scoreText;
    [SerializeField] Button buyButton;

    public event Action OnBuyButtonClicked;

    void Awake() => buyButton.onClick.AddListener(() => OnBuyButtonClicked?.Invoke());

    public void UpdateScoreDisplay(int score) => scoreText.text = $"Score: {score}";
}

// Presenter (usually not MonoBehaviour, but can be for convenience)
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
        // business logic
        if (model.Score >= 10) {
            model.AddScore(-10);
            UpdateView();
            Debug.Log("Item bought!");
        }
    }

    void UpdateView() => view.UpdateScoreDisplay(model.Score);
}

// Composition Root (MonoBehaviour in the scene)
public class GameRoot : MonoBehaviour {
    [SerializeField] ScoreView scoreView;

    void Start() {
        var model = new ScoreModel();
        var presenter = new ScorePresenter(model, scoreView);
    }
}
```

### Pros of MVP:
- View is completely dumb (easy to replace with another implementation).
- Presenter can be tested without Unity (no `MonoBehaviour`).
- Clear separation: UI designer works on View, programmer on Presenter/Model.

---

## 4. 🆚 MVC vs MVP – which to choose for Unity UI?

| Criteria | MVC | MVP |
| Implementation complexity | Simpler for small projects | Slightly more complex, better for scale |
| Model testability | ✅ | ✅ |
| Control logic testability | ❌ (Controller is MonoBehaviour) | ✅ (Presenter is plain C#) |
| Changing UI framework (Text → TMP → Slider) | Moderate | Easy (via interface) |
| When to use | Prototypes, simple UI | Large projects, complex UI interactions |

Recommendation: For serious projects with UI (inventory, shop, settings), use MVP. For a simple HUD (health, ammo), a simplified MVC is fine.

---

## 5. 🔧 Practical tips for Unity
1. Model – plain C#, don't inherit from `MonoBehaviour`. Use `event` / `Action` for notifications.
2. View – only access UI components (Text, Slider, Image). No conditional logic (`if (health < 0)` should not be in View).
3. Presenter / Controller – subscribe to View events and Model events. Do not keep references to `GameObject` / `Transform`.
4. Composition Root – a separate script that creates the Model, Presenter, and binds it to the View at startup.
5. For testing – write unit tests for Model and for Presenter (using a mock View).

---

### ⭐ If this project was useful, put a star on GitHub!
