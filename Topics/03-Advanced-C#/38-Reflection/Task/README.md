# 🎯 Practical Task: Buff/Debuff System using Reflection

## 📋 Task Description
You need to create a flexible buff/debuff system (temporary effects) that uses Reflection to modify any field in any script, 
interfaces to apply effects, `[RequireComponent]` to automatically add necessary components, and `[ContextMenu]` for debugging.

---

## 🧱 Task Structure
### 📁 Part 1: Base Classes and Interfaces
1. Create an interface `IBuffable` with method `ApplyBuff(BuffData data)`
2. Create a class `BuffData` that contains:
   - `string fieldName` — name of the field to modify
   - `float valueModifier` — modifier value (e.g., 1.2 for 20% increase)
   - `float duration` — effect duration in seconds
  
### 📁 Part 2: Buff Script
Create a script `BuffReceiver` with `[RequireComponent(typeof(IBuffable))]`. It should:
- Store active buffs in a list/dictionary
- Have a method `AddBuff(BuffData buff)` that applies the change via Reflection
- Restore the original field value after the duration (use `Coroutine`)

### 📁 Part 3: Test Class
Create a class `PlayerStats : MonoBehaviour, IBuffable` with public fields:
- `float speed = 5.0f`
- `int damage = 10`
- `float jumpPower = 8.0f`

### 📁 Part 4: Applying Buffs
Create a script `BuffTester` with a `[ContextMenu("Apply Speed Buff")]` method that:
- Finds the `IBuffable` component on the same object
- Creates a `BuffData` to modify `speed` by 1.5 (50% speed increase) for 5 seconds
- Applies the buff

---

## ✅ Completion Criteria
1. Use `System.Reflection` to read/modify a field by name (`GetType().GetField()`)
2. `GetComponents<IBuffable>()` should find all components implementing the interface
3. `[RequireComponent(typeof(IBuffable))]` should automatically require the interface
> (hint: you cannot directly require an interface via RequireComponent — figure out a workaround by requiring a concrete component that implements the interface)

4. `[ContextMenu]` is used for quick testing of buffs from the editor

---

## 🧩 Bonus Task (⭐)
Add buff stacking support:
- If a second buff is applied to the same field, either replace the previous one (reset timer) OR stack multiplicatively (×1.2, then ×1.3 = ×1.56)

---

## 🧪 Expected Result
After adding `BuffTester` to an object with `PlayerStats`:
- Right-click on the `BuffTester` component in Inspector → "Apply Speed Buff"
- Player's `speed` increases from 5 to 7.5 for 5 seconds
- After 5 seconds, speed returns to 5
- Logs are printed to the console when buff is applied and when it ends

---

### ⭐ If this project was useful, put a star on GitHub!
