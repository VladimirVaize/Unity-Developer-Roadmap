# 🧪 Практическое задание: Рефакторинг UI магазина с использованием MVP

Цель: Переписать "грязный" UI-скрипт магазина очков на архитектуру MVP, разделив данные (Model), отображение (View) и логику (Presenter). Научиться тестировать Presenter без Unity.

---

## 📥 Исходный "плохой" код (дан)
```csharp
// BadShopUI.cs – всё в одном месте
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

Задача: Рефакторинг этого кода на MVP без потери функциональности.

---

## 🎯 Задачи
### Этап 1: Создать Model
1. Создайте класс `ShopModel` (обычный C#, не `MonoBehaviour`).
2. Добавьте поле `Coins` (приватное, публичное свойство или поле с геттером).
3. Добавьте метод `bool TrySpendCoins(int amount)` – возвращает `true`, если списание возможно, иначе `false`.
4. Добавьте событие `Action<int> OnCoinsChanged`, которое вызывается при изменении монет.

### Этап 2: Создать интерфейс View
1. Создайте интерфейс `IShopView`.
2. Добавьте метод void `UpdateCoinsDisplay(int coins)`.
3. Добавьте события:
   - `event Action OnBuySwordClicked`
   - `event Action OnBuyShieldClicked`
   - `event Action OnCloseClicked`
  
### Этап 3: Реализовать View в `MonoBehaviour`
1. Создайте скрипт `ShopView : MonoBehaviour, IShopView`.
2. Привяжите в инспекторе поля `coinText`, `buySwordButton`, `buyShieldButton`, `closeButton`.
3. В `Awake()` подпишите кнопки на вызов соответствующих событий интерфейса.
4. Реализуйте `UpdateCoinsDisplay` – обновляет `coinText`.

### Этап 4: Написать Presenter
1. Создайте класс `ShopPresenter` (не `MonoBehaviour`).
2. Конструктор принимает `ShopModel` model и `IShopView` view.
3. В конструкторе подпишитесь на события View:
   - `OnBuySwordClicked` → вызывает `model.TrySpendCoins(50)` и, если успешно, обновляет View через `view.UpdateCoinsDisplay`.
   - `OnBuyShieldClicked` → аналогично с ценой 30.
   - `OnCloseClicked` → просто вызывает `Debug.Log("Close")` (или отключает объект через ссылку, переданную извне).
  
4. Подпишитесь на `model.OnCoinsChanged`, чтобы обновлять View автоматически.
5. В конце конструктора вызовите `view.UpdateCoinsDisplay(model.Coins)`.

### Этап 5: Composition Root
1. Создайте скрипт `GameBootstrapper : MonoBehaviour`.
2. В `Start()` создайте экземпляры:
   - `var model = new ShopModel();` (начальное значение монет – 100).
   - Найдите `ShopView` на сцене (или сериализуйте поле).
   - Создайте `new ShopPresenter(model, view);`
  
3. Убедитесь, что оригинальный `BadShopUI` отключён или удалён.

### Этап 6: (⭐ Бонус) Написать юнит-тест для Presenter
Используя NUnit или Unity Test Framework:
```csharp
[Test]
public void BuySword_DecreasesCoins_WhenEnoughFunds() {
    // Arrange
    var model = new ShopModel(); // coins = 100
    var mockView = new MockShopView(); // ваша мок-реализация IShopView
    var presenter = new ShopPresenter(model, mockView);
    
    // Act
    mockView.TriggerBuySword(); // симулируем нажатие кнопки
    
    // Assert
    Assert.AreEqual(50, model.Coins);
    Assert.IsTrue(mockView.UpdateCoinsCalledWith == 50);
}
```

---

## ✅ Критерии успеха
- `ShopModel` не содержит ссылок на Unity (можно протестировать в редакторе без сцены).
- `ShopView` содержит только UI-код и события – ни одной проверки `if (coins >= cost)`.
- `ShopPresenter` связывает Model и View, содержит всю логику покупки.
- При клике на кнопки монеты корректно уменьшаются, UI обновляется.
- (⭐) Юнит-тест проходит и доказывает, что Presenter работает без сцены.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
