# 🎯 Practical Task: Power-up System with Visual Effects

## 📝 Task Description
You need to implement a temporary power-up system for a game character using coroutines. 
The power-up is collected on the scene, activates for 5 seconds, gives the player a speed boost, and then deactivates.

---

## ✅ Requirements

### 1. <a href="../Solution/PowerUp.cs"><code>PowerUp</code></a> Script (attached to the power-up object on the scene):
  - Upon collision with the player (`OnTriggerEnter`), the power-up should disappear (visually and the collider) and call the activation method on the player.
  - After activation, the power-up should not be visible (can be destroyed or disabled).

### 2. <a href="../Solution/PlayerPowerUp.cs"><code>PlayerPowerUp</code></a> Script (attached to the player):
  - Method `ActivateSpeedBoost(float duration)` — starts the power-up coroutine.
  - The coroutine must:
    - Increase the player's speed (e.g., `moveSpeed *= 2`).
    - Start a visual effect (change sprite color to blue or increase scale by 20%).
    - Wait for `duration` seconds using `WaitForSeconds`.
    - Restore speed to its original value.
    - Disable the visual effect (restore color/scale).
  - If the player picks up a second power-up while one is active — the old effect should clean up properly, and the new one should start (timer reset). (Hint: use `StopCoroutine` before starting a new one)

### 3. Bonus (optional but recommended):
  - Add flickering to the power-up object (blinking light or alpha change) using a second coroutine.
  - Play a sound when the power-up is activated (via `AudioSource.PlayOneShot`).

---

## 🧪 Expected Result
- The player runs around the scene.
- Picks up a power-up object.
- Speed increases for 5 seconds.
- During the power-up, the character visually changes (blue color / larger size).
- After 5 seconds, the character returns to normal.

---

### ⭐ If this project was useful, put a star on GitHub!
