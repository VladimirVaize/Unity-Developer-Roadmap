# 🎯 Practical Task: Tags, Layers, and Physics Interactions

## 📝 Problem Statement
You are developing a 3D game where the player can shoot a laser gun at enemies. You need to implement the following logic using tags, layers, and layer masks in Raycast:

### 1. Objects and their tags/layers:
  - Player — tag `"Player"`, layer `Player`.
  - Enemy — tag `"Enemy"`, layer `Enemy`.
  - Laser beam — tag `"Laser"`, layer `Projectile`.
  - Wall — tag `"Wall"`, layer `Environment`.

### 2. Requirements:
  - Laser must destroy an enemy on hit.
  - Laser must NOT pass through walls (colliding with a wall destroys the laser).
  - Laser must NOT interact with the player (pass through without harm and without being destroyed).
  - When shooting, you must check if a wall is between the player and the enemy using Raycast (with a layer mask). If a wall blocks the line of sight, the shot should not reach the enemy.

### 3. Additional (Physics Matrix):
  - Configure the collision matrix so that:
    - Laser (layer `Projectile`) collides with Enemy (`Enemy`) and Wall (`Environment`).
    - Laser does NOT collide with Player (`Player`) or other lasers (`Projectile`).
   
---

## 🛠️ Steps to Complete

### Step 1: Create Layers
  - Open `Project Settings` → `Tags and Layers`.
  - Create layers: `Player`, `Enemy`, `Projectile`, `Environment` (starting from User Layer 8).

### Step 2: Assign Tags and Layers to Objects
  - Create cubes/spheres for the player, enemy, and wall.
  - Assign each object the corresponding tag and layer via the Inspector.

### Step 3: Configure Physics Matrix
  - `Project Settings` → `Physics` → `Layer Collision Matrix`.
  - Uncheck:
    - `Projectile` × `Player` (laser passes through player)
    - `Projectile` × `Projectile` (lasers don't collide with each other)
  - Keep checked:
    - `Projectile` × `Enemy`
    - `Projectile` × `Environment`
   
### Step 4: Laser Script (<a href="../Solution/Laser.cs"><code>Laser.cs</code></a>)
  - Store the laser speed (`speed`) and the laser lifetime (`lifetime`) in variables.
  - In `Start`, start the destruction (`Destroy`) laser after a `lifetime` sec.
  - Each frame (in the `Update`), set the movement through the `transform'.Translate`
  - In the `OnCollisionEnter`:
    - Checking the enemy tag (`CompareTag("Enemy")`)
      - Destroy the enemy in a collision
    - The laser is destroyed in any collision (wall, enemy)
   
### Step 5: Raycast Check Before Shooting (<a href="../Solution/PlayerShoot.cs"><code>PlayerShoot.cs</code></a>)
  - Store the laser prefab (`GameObject laserPrefab`), the starting position of the shot (`Transform firePoint`) and the maximum distance of the shot (`shootDistance = 50f`)
  - In `Update`, when pressing LMB (`Input.GetButtonDown("Fire1")`)
    - Creating a mask: exclude the `Player` layer, but include `Enemy` and `Environment`
    - We shoot the `Raycast` beam forward (don't forget to pass the mask parameter to the `Raycast`)
      - If the `Raycast` is in the `Enemy` (Do a check on the tag - `CompareTag("Enemy")`):
        - Creating a laser (`Instantiate(laserPrefab, firePoint.position, firePoint.rotation);`)
        - Display the message `"Shot at the enemy!"`
      - If the `Raycast` hits the `"Wall"`:
        - Display the message `"Path blocked by a wall. Cannot shoot."`
      - If the `Raycast` didn't hit anything:
        - Create a laser
        - Display the message `"Free shot"`
       
### Step 6: Test the Implementation
  - Place a wall between the player and the enemy. When you press the fire button, the Raycast should detect the wall and prevent laser creation.
  - Remove the wall — the shot should reach the enemy and destroy it.
  - The laser should pass through the player (no collision) but hit the wall or enemy.

---

### ⭐ If this project was useful, put a star on GitHub!
