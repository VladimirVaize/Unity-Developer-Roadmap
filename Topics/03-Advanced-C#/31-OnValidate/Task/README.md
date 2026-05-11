# 🎯 Practical Task: Using `OnValidate` to Configure a Weapon

You are creating an editor tool for configuring a weapon prefab. 
Use `OnValidate` to automatically maintain valid values.

---

## 📝 Requirements
Create a script <a href="../Solution/Weapon.cs"><code>Weapon.cs</code></a> with fields:
- `damage` (int, from 1 to 100)
- `range` (float, from 0.5 to 50)
- `fireRate` (float, shots per second, from 1 to 20)
- `ammo` (int, current ammo)
- `maxAmmo` (int, maximum ammo)

---

## ⚙️ `OnValidate` Rules
1. If `damage` is outside [1, 100] — clamp to the nearest limit.
2. If `range` is outside [0.5, 50] — clamp.
3. If `fireRate` < 1 → set to 1. If > 20 → set to 20.
4. If `ammo` > `maxAmmo` → `ammo` = `maxAmmo`
5. If `maxAmmo` < 1 → set `maxAmmo = 1`
6. After correction, print to the console:
   `"Weapon validated: Dmg={damage}, Range={range}, Rate={fireRate}, Ammo={ammo}/{maxAmmo}"`

---

## 🧠 Bonus (optional)
Add automatic calculation of `reloadTime = 1 / fireRate` (also print it to the console, but do not make it a serialized field).

---

## 🔍 Verification
1. Attach the script to any GameObject in the editor.
2. Change values in the Inspector to invalid ones (damage = 150, range = 0, ammo greater than max).
3. Verify that `OnValidate` corrects them instantly.

---

### ⭐ If this project was useful, put a star on GitHub!
