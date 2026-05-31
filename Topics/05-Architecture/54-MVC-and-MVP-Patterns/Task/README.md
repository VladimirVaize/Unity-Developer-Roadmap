# 🧪 Practical Task: Refactoring a Shop UI using MVP

Goal: Rewrite a "dirty" shop UI script into MVP architecture, separating data (Model), display (View), and logic (Presenter). Learn to test the Presenter without Unity.

---

## 📥 Original "bad" code (provided)
```csharp
// BadShopUI.cs – everything in one place
public class BadShopUI : MonoBehaviour {
    public Text coinText;
    public Text itemNameText;
    public Button buySwordButton;
    public Button buyShieldButton;
    public Button closeButton;

    int coins = 100;

    void Start() {
        buySwordButton.onClick.AddListener(() => BuyItem("sword", 50));
        buyShieldButton.onClick.AddListener(() => BuyItem("shield", 30));
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        UpdateUI();
    }

    void BuyItem(string item, int cost) {
        if (coins >= cost) {
            coins -= cost;
            UpdateUI();
            Debug.Log($"Bought {item}!");
        } else {
            Debug.Log("Not enough coins!");
        }
    }

    void UpdateUI() {
        coinText.text = $"Coins: {coins}";
    }
}
```

Task: Refactor this code into MVP without losing functionality.

---

## 🎯 Tasks
### Step 1: Create the Model
1. Create a `ShopModel` class (plain C#, not `MonoBehaviour`).
2. Add a `Coins` field (private, with a public property or getter).
3. Add a method `bool TrySpendCoins(int amount)` – returns `true` if spending is possible, otherwise `false`.
4. Add an event `Action<int> OnCoinsChanged` that fires when coins change.

### Step 2: Create the View interface
1. Create an interface `IShopView`.
2. Add a method `void UpdateCoinsDisplay(int coins)`.
3. Add events:
   - `event Action OnBuySwordClicked`
   - `event Action OnBuyShieldClicked`
   - `event Action OnCloseClicked`
  
### Step 3: Implement the View in `MonoBehaviour`
1. Create a script `ShopView : MonoBehaviour, IShopView`.
2. Assign `coinText`, `buySwordButton`, `buyShieldButton`, `closeButton` in the Inspector.
3. In `Awake()`, subscribe the buttons to invoke the corresponding interface events.
4. Implement `UpdateCoinsDisplay` – updates `coinText`.

### Step 4: Write the Presenter
1. Create a `ShopPresenter` class (not `MonoBehaviour`).
2. Constructor takes `ShopModel` model and `IShopView` view.
3. In the constructor, subscribe to View events:
   - `OnBuySwordClicked` → calls `model.TrySpendCoins(50)` and, if successful, updates View via `view.UpdateCoinsDisplay`.
   - `OnBuyShieldClicked` → same with cost 30.
   - `OnCloseClicked` → just calls `Debug.Log("Close")` (or disables the object via an external reference).
  
4. Subscribe to `model.OnCoinsChanged` to automatically update the View.
5. At the end of the constructor, call `view.UpdateCoinsDisplay(model.Coins)`.

### Step 5: Composition Root
1. Create a script `GameBootstrapper : MonoBehaviour`.
2. In `Start()`, create instances:
   - `var model = new ShopModel();` (initial coins = 100).
   - Find the `ShopView` in the scene (or serialize a field).
   - Create `new ShopPresenter(model, view);`
  
3. Ensure the original `BadShopUI` is disabled or removed.

### Step 6: (⭐ Bonus) Write a unit test for the Presenter
Using NUnit or Unity Test Framework:
```csharp
[Test]
public void BuySword_DecreasesCoins_WhenEnoughFunds() {
    // Arrange
    var model = new ShopModel(); // coins = 100
    var mockView = new MockShopView(); // your mock implementation of IShopView
    var presenter = new ShopPresenter(model, mockView);
    
    // Act
    mockView.TriggerBuySword(); // simulate button click
    
    // Assert
    Assert.AreEqual(50, model.Coins);
    Assert.IsTrue(mockView.UpdateCoinsCalledWith == 50);
}
```

---

## ✅ Success Criteria
- `ShopModel` contains no references to Unity (can be tested in the editor without a scene).
- `ShopView` contains only UI code and events – no `if (coins >= cost)` checks.
- `ShopPresenter` connects Model and View, contains all purchase logic.
- Clicking buttons correctly decreases coins and updates the UI.
- (⭐) The unit test passes, proving the Presenter works without a scene.

---

### ⭐ If this project was useful, put a star on GitHub!
