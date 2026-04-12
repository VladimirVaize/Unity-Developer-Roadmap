# 🧪 Practical Task: Movement, Jump, and Shooting via Old Input Manager

## 📌 Goal
Write a script for a simple player cube that uses only the old Input Manager:
- Forward/back (W/S) and left/right (A/D) movement
- Jump on spacebar press
- Shoot a sphere on left mouse button click (in the camera's forward direction)

---

## 🧩 Requirements

### 1. Movement
- Use `Input.GetAxis("Vertical")` and `Input.GetAxis("Horizontal")`
- Multiply by speed and `Time.deltaTime`
- Move the cube via `transform.Translate`

### 2. Jump
- Only when the player is grounded (`isGrounded` check)
- Use `Input.GetKeyDown(KeyCode.Space)`
- Add a `Rigidbody` and apply upward force (`AddForce`)

### 3. Shooting
- Instantiate a sphere (prefab) in front of the player
- On left mouse button click (`Input.GetMouseButtonDown(0)`)
- Give the sphere forward velocity via `Rigidbody.velocity`

### 4. Bonus (optional)
- Add sprint while holding `LeftShift` using `Input.GetKey`

---

## 📁 Structure (example)
```plaintext
PlayerController.cs  (main script)
Bullet.prefab        (sphere + collider + Rigidbody)
Ground               (plane with collider)
```

---

## ✅ Success Criteria
- Cube moves smoothly and doesn't fall through the ground
- Jump works only on ground
- Left mouse click spawns a ball flying forward
- No console errors

---

### ⭐ If this project was useful, put a star on GitHub!
