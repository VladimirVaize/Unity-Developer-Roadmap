# 🎯 Task: «Integration Testing an RPG Shop»
You are developing an RPG where players can buy items from a shop using coins. 
You need to write integration tests to verify purchase scenarios using mock objects for dependencies (UI, audio, saving).

## 📝 Classes Under Test (Simplified):
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
            ui.ShowMessage($"Purchased: {ItemName}");
            audio.PlaySound("Purchase");
        }
        else
        {
            ui.ShowMessage("Not enough coins!");
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

// Interfaces for mocks
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

## 📋 Integration Testing Tasks:
1. Test 1: Player with 100 coins buys an item for 50 — verify that:
   - Coins become 50
   - `IsPurchased = true`
   - UI received a purchase message
   - Purchase sound was played
  
2. Test 2: Player with 20 coins tries to buy an item for 50 — verify that:
   - Coins remain 20
   - `IsPurchased = false`
   - UI received an error message
   - Error sound was played
  
3. Test 3: Attempt to buy an already purchased item — verify that:
   - Coins are not deducted again
   - UI message is not shown again
   - Sound is not played again
  
4. Test 4 (with timing): Using `[UnityTest]`, verify that after purchasing an item, the player's inventory updates
5. (assume there is a `RefreshInventoryUI()` method that is called after `yield return new WaitForSeconds(0.5f)`)

---

## 🧰 Implementation Requirements:
- Create hand-written mocks for `IShopUI` and `IAudioService` (Spy pattern)
- All tests must be integration tests — `ShopItem.Purchase()` calls real methods on mocks
- Use `[SetUp]` to create a new `PlayerWallet` instance before each test
- For Test 4, use `[UnityTest]` and a coroutine

---

## 🧩 Example Spy Mock Implementation:
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

### ⭐ If this project was useful, put a star on GitHub!
