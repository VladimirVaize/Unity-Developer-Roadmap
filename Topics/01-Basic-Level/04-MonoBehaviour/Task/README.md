# 🧪 Practical Task: MonoBehaviour Lifecycle

## 🎯 Goal

Create a script that demonstrates the order of lifecycle method calls and correctly distributes movement, physics, and camera logic between `Update()`, `FixedUpdate()`, and `LateUpdate()`.

---

## 🧱 Given
### You have:
- 🟦 A Cube (Player) with a `Rigidbody` component.
- 🎥 A Camera that is NOT a child of the cube.
- ⌨️ Controls: Arrow keys / WASD to move the cube on the plane (X and Z axes).
- ⬆️ Spacebar — jump.

---

## ✅ Tasks

### 1. Create a script `PlayerMovement` and attach it to the cube.
### 2. Implement the correct structure:
  - `Awake()` — get the `Rigidbody` component and store a reference.
  - `Start()` — set the cube's initial position to `(0, 1, 0)`.
  - `Update()` — read input (horizontal/vertical and spacebar).
  - `FixedUpdate()` — apply movement via `rigidbody.MovePosition()` (or `AddForce`) and jump via `AddForce`.
  - `LateUpdate()` — make the camera follow the cube with a slight delay (smooth follow).
### 3. Add `Debug.Log` to each method to see the call order in the console.

---

## 🔍 Expected Behavior

- The cube moves smoothly and does not fall through the floor (if the floor has a `Collider`).
- Jump only works when the cube is on the ground (use `OnCollisionEnter` or an additional ground check).
- The camera moves after the cube, with no jitter.
- The console shows the sequence: `Awake` → `Start` → (loop `FixedUpdate` → `Update` → `LateUpdate`).

---

## 🚀 Bonus Task (optional)

Add a UI Button that disables and then re-enables the `PlayerMovement` script, and observe how `OnEnable()` and `OnDisable()` are called.

---

## 📤 Self-check

- Full code of `PlayerMovement.cs`.

---

### ⭐ If this project was useful, put a star on GitHub!
