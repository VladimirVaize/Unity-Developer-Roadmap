# 🎯 Task: Weapon Type System & Global Score Manager

### Goal:
Create a small Unity project that uses `ScriptableObject` for:
1. Storing configuration for different weapon types (damage, attack speed, name, icon).
2. Replacing a singleton — a global player score storage accessible from any scene and any script without a `static Instance`.
3. Integration — weapons increase the score through the global storage.

---

## 🧱 Steps to implement
1. Create `WeaponSO` ScriptableObject<br>
   Fields: `weaponName` (string), `damage` (int), `attackSpeed` (float), `icon` (Sprite), `prefab` (GameObject — reference to a visual weapon model).
2. Create `GlobalScoreSO` ScriptableObject<br>
   Fields: `score` (int).<br>
   Methods: `AddScore(int value)`, `ResetScore()`.
3. Create several weapon assets via `Create → Game/Weapon`:
   - Sword (damage=10, attackSpeed=1.0)
   - Bow (damage=7, attackSpeed=1.5)
   - Staff (damage=15, attackSpeed=0.8)
4. Create `WeaponPickup` script (MonoBehaviour)
   - Has a public field `WeaponSO weaponData`.
   - In `OnTriggerEnter` (or on click) when picking up the weapon:
     - Logs to the console: "Picked up {weaponName}, damage {damage}".
     - Adds the weapon's damage to the global score: `GlobalScoreSO.AddScore(weaponData.damage)`.
     - Destroys the scene object.
5. Create `ScoreUI` script (MonoBehaviour)
   - Reference to `GlobalScoreSO`.
   - In `Update()` (or via an event) updates a UI Text field: `Score: {score}`.
6. Scene setup:
   - Place 3 different pickup items on the scene (prefabs or simple cubes with colliders).
   - Assign each its own `WeaponSO` (Sword, Bow, Staff).
   - Add a simple UI Text to display the score.
   - Put `ScoreUI` and `GlobalScoreSO` on an empty GameObject.
7. Verify singleton replacement
   - Ensure there is no `public static GlobalScoreSO Instance` anywhere.
   - Instead, the reference to `GlobalScoreSO` is manually dragged into the Inspector of any script that needs the score.
  
---

## ✅ Success Criteria
- Picking up a sword increases the score by 10.
- Picking up a bow increases it by 7, etc.
- When reloading the scene (or moving to a new scene), the score does NOT reset automatically (unless you call `ResetScore()`).
- All weapon configurations live in separate `.asset` files and can be changed without rewriting code.

---

## 🧠 Bonus (optional)
- Add a `WeaponEventSO` (another ScriptableObject) as an event channel.
  When a weapon is picked up, it fires an event, and `ScoreUI` listens to it — then `ScoreUI` doesn't need an `Update()` call every frame.
- Create a "Reset Score" button that calls `GlobalScoreSO.ResetScore()`.

---

## Solution:

### 🧩 1. ScriptableObject: Weapon (<a href="../Solution/ScriptableObjects/WeaponSO.cs"><code>WeaponSO.cs</code></a>)
### 🧩 2. ScriptableObject: Global Score (<a href="../Solution/ScriptableObjects/GlobalScoreSO.cs"><code>GlobalScoreSO.cs</code></a>)
### 🧩 3. Weapon Pickup Component (<a href="../Solution/Pickup/WeaponPickup.cs"><code>WeaponPickup.cs</code></a>)
### 🧩 4. Score UI Display (<a href="../Solution/UI/ScoreUI.cs"><code>ScoreUI.cs</code></a>)
### 🧩 5. (Bonus) Reset Score Button Script (<a href="../Solution/ResetScoreButton.cs"><code>ResetScoreButton.cs</code></a>)
---

### ⭐ If this project was useful, put a star on GitHub!
