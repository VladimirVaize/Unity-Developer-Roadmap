# 🛠️ Practical Task: «Hero Inventory»
You are creating an RPG game. You need to implement an inventory system where each item has a name, weight, and price. 
Use your knowledge of serialization to properly show and hide fields in the Inspector.

## 📝 Requirements
1. Create a struct <a href="../Solution/InventoryItem.cs"><code>InventoryItem</code></a> with the following fields:
   - `itemName` (string, public) — the item's name.
   - `weight` (float, private, but should be visible in the Inspector) — the item's weight.
   - `price` (int, public, but should be hidden in the Inspector) — the item's price.
  
2. Create an <a href="../Solution/Inventory.cs"><code>Inventory</code></a> class (MonoBehaviour) that contains:
   - A public field `items` — an array of `InventoryItem`.
   - A private field `maxWeight` (float) that should be displayed in the Inspector.
   - A private field `ownerName` (string) that should not be displayed in the Inspector.
  
3. Write a method `GetTotalWeight()` inside `Inventory` that returns the sum of all items' weights.
4. In the `Start()` method, check whether the total weight exceeds `maxWeight`. If it does, print a warning to the console: `"Inventory of [ownerName] is overloaded!"`.

---

## ✅ What to use
- `[System.Serializable]` for the struct.
- `[SerializeField]` for private fields that should be visible in the Inspector.
- `[HideInInspector]` for a public field that should be hidden.

---

## 🧪 Verification
- Open Unity, create an empty GameObject, and attach the `Inventory` script.
- In the Inspector, you should see:
  - An `Items` array, and inside each element — `Item Name` and `Weight` (but not `Price`).
  - A `Max Weight` field.
  - No `Owner Name` field.
 
- Fill in a few items, set `maxWeight`. Run the scene and check the Console.

> 💡 Hint: To hide `price` inside the struct, use `[HideInInspector]` public int price;. Despite being hidden, the value will be saved and accessible from code.

---

### ⭐ If this project was useful, put a star on GitHub!
