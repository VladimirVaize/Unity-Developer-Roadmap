# 🎯 Practical Task: "Collector with Obstacles"

## 📌 Task Description

reate a simple 3D scene in Unity where the player controls a sphere (Player), collects coins (Coins), and avoids or takes damage from enemies (Enemies). 
The task reinforces the use of OnCollisionEnter and OnTriggerEnter, their differences, and limitations.

---

## 🧱 What to Do

### 1. Scene Setup
- Create a `Plane` — the ground.
- Add a `Sphere` — the player. Assign to it:
  - `Rigidbody` (non-kinematic, `isKinematic = false`)
  - `Collider` (leave `IsTrigger = false`)
  - A script `PlayerController` for movement with arrow keys/WASD (or use standard `MoveTowards`).
- Add 5–10 cubes with the tag `"Coin"`, colored green. They should have:
  - `Collider` with IsTrigger = true
  - An empty script (or just the tag).
- Add 3 red cubes with the tag `"Enemy"`. They should have:
  - `Collider` (IsTrigger = false)
  - `Rigidbody` (kinematic or not — your choice, but at least one Rigidbody must exist in the scene for collisions).
 
### 2. Player Script (`PlayerController` + events)
Write a script `PlayerCollisionHandler` that contains:
```csharp
public int score = 0;
public int health = 3;

void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Coin"))
    {
        Destroy(other.gameObject);
        score++;
        Debug.Log("Coin collected! Score: " + score);
    }
}

void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Enemy"))
    {
        health--;
        Debug.Log("Damage! Health: " + health);
        if (health <= 0)
        {
            Debug.Log("Game Over!");
            // you can disable controls or reload the scene
        }
    }
}
```
### 3. Additional Requirements
- Upon colliding with an enemy, the player should bounce back (use `Rigidbody.AddForce`).
- Play a simple sound when collecting a coin (optional).
- Add a wall with a trigger zone that teleports the player to another point on `OnTriggerEnter` (use `Transform.position`).

---

## 🧠 Self-Check Questions
- Why is `OnTriggerEnter` used for collecting coins instead of `OnCollisionEnter`?
- What would happen if you removed the `Rigidbody` from a coin? What if you made its collider non-trigger?
- Why is `OnCollisionEnter` used for collision with an enemy?

---

## 🏆 Goal
Write working code, set up the scene, and ensure that:
- Coins disappear on touch and increase the score.
- Enemies reduce health and push the player away.
- The trigger zone teleports the player.

---

### ⭐ If this project was useful, put a star on GitHub!
