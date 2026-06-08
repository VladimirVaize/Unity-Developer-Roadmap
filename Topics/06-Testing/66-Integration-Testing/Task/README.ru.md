# 🎯 Задача: «Интеграционное тестирование магазина в RPG»
Вы разрабатываете RPG, в которой игрок может покупать предметы в магазине за монеты. 
Вам нужно написать интеграционные тесты для проверки сценариев покупки с использованием мок-объектов для зависимостей (например, UI, звук, сохранения).

## 📝 Тестируемые классы (упрощённо):
```csharp
// ShopItem.cs
public class ShopItem
{
    public string ItemName { get; }
    public int Price { get; }
    public bool IsPurchased { get; private set; }
    
    public ShopItem(string name, int price)
    {
        ItemName = name;
        Price = price;
    }
    
    public void Purchase(PlayerWallet wallet, IShopUI ui, IAudioService audio)
    {
        if (IsPurchased) return;
        
        if (wallet.SpendCoins(Price))
        {
            IsPurchased = true;
            ui.ShowMessage($"Куплено: {ItemName}");
            audio.PlaySound("Purchase");
        }
        else
        {
            ui.ShowMessage("Недостаточно монет!");
            audio.PlaySound("Error");
        }
    }
}

// PlayerWallet.cs
public class PlayerWallet
{
    public int Coins { get; private set; }
    
    public PlayerWallet(int startCoins)
    {
        Coins = startCoins;
    }
    
    public bool SpendCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            return true;
        }
        return false;
    }
    
    public void AddCoins(int amount) => Coins += amount;
}

// Интерфейсы для моков
public interface IShopUI
{
    void ShowMessage(string text);
}

public interface IAudioService
{
    void PlaySound(string soundName);
}
```

---

## 📋 Задачи для интеграционного тестирования:

1. Тест 1: Игрок с 100 монетами покупает предмет за 50 — проверьте, что:
- Монет стало 50
- `IsPurchased = true`
- UI получил сообщение о покупке
- Звук покупки был воспроизведён

2. Тест 2: Игрок с 20 монетами пытается купить предмет за 50 — проверьте, что:
- Монет осталось 20
- `IsPurchased = false`
- UI получил сообщение об ошибке
- Воспроизведён звук ошибки

3. Тест 3: Попытка купить уже купленный предмет — проверьте, что:
- Монеты не списываются повторно
- Сообщение UI не показывается повторно
- Звук не воспроизводится повторно

4. Тест 4 (со временем): Используя `[UnityTest]`, проверьте, что после покупки предмета у игрока обновляется инвентарь
   (допустим, есть метод `RefreshInventoryUI()`, который вызывается через `yield return new WaitForSeconds(0.5f)`)

---

## 🧰 Требования к реализации:
- Создайте ручные моки для `IShopUI` и `IAudioService` (Spy)
- Все тесты должны быть интеграционными — `ShopItem.Purchase()` вызывает реальные методы моков
- Используйте `[SetUp]` для создания нового экземпляра `PlayerWallet` перед каждым тестом
- Для Теста 4 используйте `[UnityTest]` и корутину

---

## 🧩 Пример Spy-мока:
```csharp
public class SpyShopUI : IShopUI
{
    public string LastMessage { get; private set; }
    public int ShowMessageCallCount { get; private set; }
    
    public void ShowMessage(string text)
    {
        LastMessage = text;
        ShowMessageCallCount++;
    }
}

public class SpyAudioService : IAudioService
{
    public string LastPlayedSound { get; private set; }
    public int PlaySoundCallCount { get; private set; }
    
    public void PlaySound(string soundName)
    {
        LastPlayedSound = soundName;
        PlaySoundCallCount++;
    }
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
