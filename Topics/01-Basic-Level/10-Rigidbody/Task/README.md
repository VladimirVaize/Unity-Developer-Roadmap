# 🎯 Practical Task: Basketball Simulator

## 📝 Task Description
You need to create a simple scene containing:
  - 🏀 A ball (sphere) with a `Rigidbody` component
  - 🧱 A hoop (empty object with a trigger collider shaped like a ring)
  - 🕹️ A throw button (press `Space` or click a UI button)
  - 📦 A kinematic platform (a moving floor that pushes the ball)

---

## ✅ What you must implement:

### 1. Throw the ball toward the hoop
On `Space` press, the ball receives an upward and forward force (`AddForce` with `ForceMode.Impulse`). Tune the values so the ball flies in an arc toward the hoop.

### 2. Gravity
The ball must fall down if it misses the hoop. Use `Use Gravity = true`.

### 3. Kinematic platform
Create a platform that moves left and right (e.g., using `transform.Translate`). Give it a `Rigidbody` with `isKinematic = true`. The ball, if it lands on the platform, should move with it and not fall through.

### 4. Score detection
When the ball enters the hoop's trigger:
  - Print `"Score!"` to the console
  - Reset the ball to its starting position
  - Zero out its velocity (`velocity = Vector3.zero`)

### 🧠 Bonus challenge (harder)
- Add a second ball with `isKinematic = true`.
  It should not react to the throw or gravity, but when hit by the normal ball,
  it should push that ball away (check how collision works between a kinematic and a regular Rigidbody).
- Experiment with different ForceMode values for the throw (e.g., Force vs Impulse) and observe the difference.

---

## 🔧 Hints
- To get a reference to the `Rigidbody` in code:<br>
  `GetComponent<Rigidbody>()` or `public Rigidbody rb;` (drag in the Inspector).
- To reset velocity:<br>
  `rb.velocity = Vector3.zero;`
- To move the platform:<br>
  `transform.Translate(Vector3.right * speed * Time.deltaTime);` and bounce off boundaries.

---

### ⭐ If this project was useful, put a star on GitHub!
