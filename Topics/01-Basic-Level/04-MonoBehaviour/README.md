# 🎬 MonoBehaviour: Script Lifecycle

Every script in Unity that inherits from `MonoBehaviour` follows a specific order of method calls. This order is called the lifecycle. 
Understanding when each method is called is critical for handling physics, input, animation, and optimization correctly.

---

## ⏱️ Method Execution Order (Core Methods)

Unity automatically calls these methods in a strict sequence:

```
Awake → OnEnable → Start → Per-frame updates → Fixed updates → Late updates → OnDisable → OnDestroy
```

### 1. `Awake()` — Awakening 🟢
- Called once when the object is loaded into the scene (even if the script is disabled).
- Used for initializing references between components (`GetComponent`, finding other objects).
- Guaranteed to run before `Start()`.

### 2. `OnEnable()` — Enabling 🟡
- Called every time the object becomes active (toggled in hierarchy or script).
- Good place to subscribe to events.

### 3. `Start()` — Start 🔵
- Called once before the first `Update()`, but only if the script is enabled.
- Used for logic that depends on other objects that have already been initialized in `Awake()`.

### 4. `Update()` — Frame Update 🟠
- Called every frame (frequency depends on FPS: 30, 60, 144+ times per second).
- Used for: input handling (`Input.GetKey`), non-physics movement, animation, timers (`Time.deltaTime`).
- Not suitable for physics-related work!

### 5. `FixedUpdate()` — Fixed Update 🔴
- Called at constant time intervals (default 0.02 sec = 50 times per second), independent of FPS.
- Used for everything related to physics: `Rigidbody.AddForce`, movement via `MovePosition`, changing velocity.
- The interval can be changed in `Project Settings → Time → Fixed Timestep`.

### 6. `LateUpdate()` — Late Update 🟣
- Called every frame right after `Update()`.
- Used for: camera following the player (so the camera moves after the player has updated its position), procedural animation.

### 7. `OnDisable()` — Disabling ⚫
- Called every time the object becomes inactive.
- Unsubscribe from events, stop coroutines.

### 8. `OnDestroy()` — Destruction ⚪
- Called once when the object is destroyed (`Destroy()`) or when the scene/application closes.
- Final cleanup (if needed).

---

## 📊 Visual Diagram (single object, script enabled):

```
Scene loads:
  Awake() → OnEnable() → Start()

Then every frame:
  FixedUpdate() → [physics] → Update() → LateUpdate()
  (FixedUpdate may be called multiple times between Updates if FPS is low)

When object is disabled:
  OnDisable()

On destruction / exit:
  OnDestroy()
```

---

## 🔧 Usage Examples

| Method | When to use | What NOT to do |
|---------|--------------------------------|--------------------------------|
| `Awake` | Find a component: <br>`GetComponent<Rigidbody>()` | Access other objects that might not have woken up yet |
| `Start` | Set initial health, enable AI | Rely on another object's `Awake` not having run |
| `Update` | Movement: <br>`transform.Translate(Input.GetAxis("Horizontal") * speed * Time.deltaTime)` | Apply forces to Rigidbody (that goes in FixedUpdate) |
| `FixedUpdate` | Jump: <br>`rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse)` | Use `Time.deltaTime` (use `Time.fixedDeltaTime` instead) |
| `LateUpdate` | Camera follows player: <br>`cameraTransform.position = playerTransform.position + offset` | Change the physical position of a Rigidbody |

---

## ⚠️ Important Notes

- If the script is disabled (checkbox unchecked in the Inspector), `Start()`, `Update()`, `FixedUpdate()`, `LateUpdate()` are NOT called, but `Awake()` and `OnEnable()` are called (if the object is active).
- If the object is disabled in the hierarchy — nothing from the lifecycle is called.
- `Time.deltaTime` — time between frames (for `Update`).
- `Time.fixedDeltaTime` — fixed interval (for `FixedUpdate`).

---

### ⭐ If this project was useful, put a star on GitHub!
