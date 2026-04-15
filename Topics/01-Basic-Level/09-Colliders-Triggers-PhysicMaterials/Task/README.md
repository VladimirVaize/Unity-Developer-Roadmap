# 🎯 Task: «Magic Floor and Moving Platform»

Create a scene where the player (a sphere) is controlled with keyboard input and interacts with three different zones using colliders, triggers, and physic materials.

---

## 📌 Requirements:
### 1. Regular Floor (Collision + Physic Material)
  - The floor (plane) has a regular collider (`Is Trigger = false`).
  - Assign a «Rubber» physic material (high friction: `Dynamic Friction = 0.8`, `Bounciness = 0.2`).
  - The player should walk on the floor and not slide.

### 2. Ice Zone (Collision + Physic Material)
  - Create a separate platform colored blue.
  - Add an «Ice» physic material (`Dynamic Friction = 0.05`, `Bounciness = 0.1`).
  - When the player enters this zone, they should slide heavily while moving.

### 3. Trigger Trap (Trigger)
  - Create an invisible zone (a cube with `Is Trigger = true`).
  - Upon entering the zone (`OnTriggerEnter`), a message should appear in the console: «Careful, a trap!».
  - The player should pass through the zone without physical bouncing.

### 4. Moving Bouncy Platform (Collision + Bounciness)
  - Create a platform that moves up and down (animation or simple script).
  - Assign a «Trampoline» physic material (`Bounciness = 0.9`, `Dynamic Friction = 0.3`).
  - Upon colliding with the platform, the player should jump high.

### 🧩 Bonus:
  - When colliding with the "ice zone", change the player's color to light blue (via script).
  - Add walls with colliders so the player cannot leave the scene boundaries.

---

## 🧠 Check questions (after completion):
- Why doesn't a trigger need physical bouncing?
- What would happen if you removed the `Rigidbody` from the player?
- How would the behavior change if you disabled `Is Trigger` on the "trampoline"?

---

### ⭐ If this project was useful, put a star on GitHub!
