# 🎯 Practical Task: Visualizing Damage Radius and Projectile Trajectory

You are developing a Tower Defense game. 
You have a turret (`Turret`) that rotates toward the nearest enemy and shoots projectiles (`Projectile`). 
Your task is to implement debug visualization using Gizmos and Debug drawing.

---

## 📌 Task Requirements
1. Create a `Turret.cs` script attached to the turret. It should have public fields:
   - `float range` = 10f — enemy detection radius.
   - `Transform firePoint` — the point from which projectiles are fired.
   - `GameObject projectilePrefab` — the projectile prefab.
  
2. Implement in `Turret.cs`:
   - In `OnDrawGizmosSelected()`, draw a semi-transparent green wire sphere with radius `range` around the turret, only when the turret is selected.
   - In `OnDrawGizmos()` (always), draw a red ray from `firePoint` in the direction of `transform.forward` with length 3 units — this indicates the firing direction.
  
3. Create a `Projectile.cs` script for the projectile. In `Update()`, it moves forward (`transform.Translate(Vector3.forward * speed * Time.deltaTime)`).
4. Add trajectory debugging in `Projectile.cs`:
   - Every frame, draw a yellow line from the projectile's previous position to its current position using `Debug.DrawLine` (line lives for 0.05 seconds).
   - Upon collision (you can use `OnCollisionEnter` conditionally), draw a red ray from the hit point in a random direction with length 2 units (simulating shrapnel) that lasts for 1 second.
  
5. Optional: In `Turret.cs` inside `Update()`, use `Debug.DrawRay` to draw a green ray from the turret to the nearest enemy (if an enemy is found).

---

## 🧪 Expected Outcome
- In Scene View: when the turret is selected, a semi-transparent green wire sphere (vision radius) is visible. The red firing direction ray is always visible.
- During Play Mode: projectiles leave yellow lines behind them (trajectory). On impact, red "shrapnel" rays appear.
- If an enemy is within range, a green ray extends from the turret to that enemy.

---

## 🔧 Implementation Tips
- Use a `Vector3 previousPosition` in `Projectile` and update it at the end of `Update()`.
- For impact ray: `Debug.DrawRay(hitPoint, Random.onUnitSphere * 2f, Color.red, 1f);`
- Don't forget to enable Gizmo display in Scene View and Game View (for `Debug.Draw` methods).

---

### ⭐ If this project was useful, put a star on GitHub!
