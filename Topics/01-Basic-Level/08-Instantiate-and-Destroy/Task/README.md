# 🎯 Practical Task: Falling Objects Spawner with Destruction

## 📝 Task Description
You need to create a simple scene where objects (e.g., rocks or coins) fall from the sky. The player controls a platform (or character) at the bottom of the screen and must collect coins while avoiding rocks.

---

## ✅ Requirements
### 1. Create prefabs
  - Create two prefabs: `Coin` (green cube or sphere) and `Rock` (gray cube).
  - Add a `Collider` component (e.g., Box Collider) to each prefab, and optionally a simple script for color or rotation.

### 2. Spawner script (`Spawner`)
  - The spawner object is located at the top of the scene (or just an empty GameObject).
  - Every 1–2 seconds, randomly create (`Instantiate`) either a coin or a rock.
  - Objects appear at a random horizontal (X) position within given limits.
  - Each created object is automatically destroyed after 4 seconds if not collected (to avoid cluttering the scene).

### 3. Collectible object script (`Collectible`)
  - When the player touches a coin — destroy the coin (`Destroy`) and increase the score.
  - When the player touches a rock — the game ends (or loses a life).

### 4. Player script (`Player`)
  - Horizontal movement (left/right arrows or A/D).
  - Displays the current score on the screen (using `TextMeshPro` or simple `GUI`).
  - When colliding with a rock, call `GameOver()` (stop the spawner, show a message).

### 5. Destroy objects that fall out of bounds (optional)
  - If an object falls below the platform and was not collected — destroy it (so it doesn't linger in empty space).

---

## 🧠 What to use
- `Instantiate()` — to create coins and rocks.
- `Destroy()` — to remove coins, rocks, and also for the player's "death" (you can reload the scene).
- `OnTriggerEnter()` or `OnCollisionEnter()` — to detect collisions.
- `Destroy(object, delay)` — to automatically remove objects that have fallen.

---

## 🏁 Goal
Write a working object spawning and destruction system that demonstrates the full lifecycle of GameObjects during runtime.

---

### A comparison <a href="../Solution">solution</a>

---

### ⭐ If this project was useful, put a star on GitHub!
