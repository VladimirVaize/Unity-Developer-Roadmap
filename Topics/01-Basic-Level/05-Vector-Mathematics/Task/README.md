# 🧪 Practical Task: "Pursuing Turret with Smooth Aiming"

Goal: Create enemy (turret) behavior that uses all the vector operations covered: `Distance`, `Lerp`, `Dot Product`, and `Cross Product`.

---

## Task Description:

### You have:
- A Turret (object A) with its forward direction `transform.forward`.
- A Player (object B) moving around the scene.

### Implement the following mechanics (all in one script `Turret.cs`):

### 1. Activation by Distance
The turret starts "asleep". As soon as the player comes within 10 units, the turret activates (starts working).<br>
Use: `Vector3.Distance`

### 2. Smooth Rotation Toward the Player
The turret should not rotate instantly. It should smoothly rotate toward the player at a speed of `rotationSpeed = 120° per second`.<br>
Use: `Quaternion.LookRotation` + `Quaternion.RotateTowards` or `Vector3.Lerp` for direction.

### 3. Field of View Check (Dot Product)
The turret only fires when the player is within a 90° sector in front of the turret 
(i.e., the angle between the turret's forward direction and the direction to the player is less than 45° on each side → `dot > 0.7`).<br>
Use: `Vector3.Dot`

### 4. Side Detection (Cross Product)
In debug mode, log to the console whether the player is to the left or to the right of the turret.<br>
Use: `Vector3.Cross`

### 5. Shooting (simplified)
If the turret is activated, the player is within the field of view (dot > 0.7), and the distance is < 10 → every 1 second, log `"Pew! Pew!"` to the console.

### Bonus task (optional):
Make a projectile move smoothly from the turret to the player's position using `Vector3.Lerp`.

### Expected result:
You will write a script that demonstrates an understanding of all five vector operations working together. 
The turret reacts naturally, without sharp rotations, and only fires within the forward-facing sector.

---

## Solution:
- <a href="../Solution/Turret.cs"><code>Turret.cs</code></a>
- Movement for Player can be taken from <code>04-MonoBehaviour</code> topic. -> <a href="../../04-MonoBehaviour/Solution/PlayerMovement.cs"><code>PlayerMovement.cs</code></a>

---

### ⭐ If this project was useful, put a star on GitHub!
