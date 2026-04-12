# 🎮 Input Handling in Unity: Old Input Manager (GetKey, GetAxis, GetMouseButton)

> [!NOTE]
> This material covers the old Input Manager in Unity — the built-in input system for keyboard, mouse, and joystick.
> Despite the new Input System, the old manager remains simple and convenient for learning, prototyping, and many 2D/3D projects.

---

## 🧠 Main Input Methods

## 1. GetKey / GetKeyDown / GetKeyUp

### Purpose:
Handling specific keyboard keys.

### How to use:
- `Input.GetKey(KeyCode.Space)` – while held, returns `true` every frame.
- `Input.GetKeyDown(KeyCode.W)` – only on the frame the key was pressed.
- `Input.GetKeyUp(KeyCode.LeftShift)` – only on the frame the key was released.

### Example:
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
    {
        Jump(); // jump exactly once per press
    }

    float speed = Input.GetKey(KeyCode.LeftShift) ? 10f : 5f;
    Move(speed);
}
```

## 2. GetMouseButton / GetMouseButtonDown / GetMouseButtonUp

### Purpose:
Handling mouse buttons.

### Button indices:
- `0` – left button
- `1` – right button
- `2` – middle (scroll wheel)

### How to use:
- `Input.GetMouseButton(0)` – while left button is held.
- `Input.GetMouseButtonDown(1)` – single press of the right button.
- `Input.GetMouseButtonUp(2)` – middle button released.

### Example:
```csharp
void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Shoot(); // shoot on click
    }

    if (Input.GetMouseButton(1))
    {
        Aim(); // aiming while right button is held
    }
}
```

## 3. GetAxis – Smooth Control

### Purpose:
Smooth values for movement, rotation, camera.<br>
Returns a value from -1 to 1.

### Main axes:
- `"Horizontal"` – A / D or ← / →
- `"Vertical"` – W / S or ↑ / ↓
- `"Mouse X"` – horizontal mouse movement
- `"Mouse Y"` – vertical mouse movement

### How to use:
```csharp
void Update()
{
    float moveX = Input.GetAxis("Horizontal");
    float moveZ = Input.GetAxis("Vertical");
    Vector3 movement = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
    transform.Translate(movement);
}
```

### Important:
- `GetAxis` – smooth change with acceleration/gravity.
- `GetAxisRaw` – instant (-1, 0, 1) for precise platformers.

---

## ⚙️ Configuring Axes (Input Manager)

Window: `Edit → Project Settings → Input Manager`

### You can:
- Add a new axis (`Size++`)
- Set keys (`Positive Button`, `Negative Button`)
- Change sensitivity and gravity

### Example custom axis for jump:
- Name: `Jump`
- Positive Button: `space`
- Type: `Key or Mouse Button`

### Usage in code:
```csharp
if (Input.GetButtonDown("Jump")) { ... }
```

---

## 🧪 Combining Methods

```csharp
void Update()
{
    // Movement via axes
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");
    Move(h, v);

    // Mouse shooting
    if (Input.GetMouseButtonDown(0))
        Fire();

    // Scroll weapon switch
    float scroll = Input.GetAxis("Mouse ScrollWheel");
    if (scroll != 0)
        SwitchWeapon(scroll > 0 ? 1 : -1);
}
```

---

## ⚠️ When to use the old Input Manager

### ✅ Good for:
- Learning and simple games
- Prototypes (jam, solo dev)
- 2D platformers, arcades, casual games
- Keyboard + mouse without complex combos

### ❌ Disadvantages:
- Complex gamepad support
- Hard to rebind keys at runtime
- Axis conflicts with multiple joysticks

---

### ⭐ If this project was useful, put a star on GitHub!
