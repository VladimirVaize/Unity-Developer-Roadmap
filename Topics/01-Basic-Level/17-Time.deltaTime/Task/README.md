# 🧪 Task: Slow-motion System with Correct Physics

## 📌 Requirements
You need to implement a "bullet time" (slow-motion) mechanic for a scene with falling spheres.

### Requirements:

1. Scene: 3 spheres (Sphere) with a `Rigidbody` component, falling from a height of 10 meters onto a plane (Plane) with a collider.
2. Script for each sphere: When a sphere hits the plane, it should be destroyed (`Destroy(gameObject)`) and increase a score counter by +1.
3. Controls:
   - Press `Q` — slow down time to `0.2` (bullet time).
   - Press `E` — return to normal speed (`1.0`).
   - Press `Space` — pause/resume.
4. Correct physics behavior: When slowing down or speeding up, the physics (falling spheres) should slow down/speed up proportionally.
   Use the correct methods (`Update` vs `FixedUpdate`) and `Time.deltaTime` / `Time.fixedDeltaTime`.
5. UI (optional): On-screen text showing:
   - Current `Time.timeScale` (rounded to 1 decimal)
   - Current score
  
---

## 🎯 Goal of the task

Learn to properly use `Time.deltaTime` for movement (if spheres were moved manually), 
`Time.timeScale` to control global time, and understand how `Time.fixedDeltaTime` automatically adjusts to time scale for correct physics.

---

## 📝 What to implement in code
1. Create a `TimeManager.cs` script (on an empty GameObject) to handle input and display UI.
2. Create a `DestroyOnCollision.cs` script for spheres that destroys the sphere and increases the score upon collision with the plane (using a static variable or a reference to TimeManager).
3. Ensure that sphere movement (gravity) correctly reacts to `Time.timeScale` — thanks to `FixedUpdate`, no extra work is needed.
4. Add text (UI Text or TextMeshPro) to display `Time.timeScale` and the score.

---

## ✅ Evaluation Criteria
- Pressing `Q` slows down the falling spheres; pressing `E` returns to normal.
- Pressing `Space` freezes everything, but the UI (score and text) may continue updating (if you use `unscaledDeltaTime` — optional).
- Physics remains stable (spheres do not fall through the floor).
- Score increases each time a sphere is destroyed.

---

### ⭐ If this project was useful, put a star on GitHub!
