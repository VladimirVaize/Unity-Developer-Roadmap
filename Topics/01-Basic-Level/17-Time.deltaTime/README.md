# ⏱️ Time.deltaTime and Time Scale in Unity

> [!Note]
> This material explains how to work with time in Unity so your games run consistently on devices with different frame rates (FPS).
> You will learn about `Time.deltaTime`, frame rate dependence, pausing and slow-motion using `Time.timeScale`, and `Time.fixedDeltaTime` for physics.

---

## 🎞️ Frame Rate Dependence

### The Problem:
If you move an object like this:
```csharp
transform.Translate(5, 0, 0); // +5 meters on X every frame
```
- On a powerful PC (60 FPS): the object moves `5 * 60 = 300` meters per second.
- On a weak device (30 FPS): `5 * 30 = 150` meters per second.
- Result: Speed depends on performance — unacceptable for a game.

### The Solution:
Use the time between frames — `Time.deltaTime`.

---

## ⏲️ Time.deltaTime

### What it is:
The time in seconds since the last frame. In `Update()`, it represents the duration of the previous frame.

### Purpose:
Make movement and any continuous changes independent of frame rate.

### How to use:
```csharp
void Update()
{
    // Speed 5 meters per second (not per frame)
    float speed = 5f;
    transform.Translate(speed * Time.deltaTime, 0, 0);
}
```

- At 60 FPS: `deltaTime ≈ 0.0167`, per second `5 * 0.0167 * 60 ≈ 5` meters.
- At 30 FPS: `deltaTime ≈ 0.0333`, per second `5 * 0.0333 * 30 ≈ 5` meters.
- Result: same distance per second at any FPS.

### Use cases:
- Character movement: `velocity * Time.deltaTime`
- Rotation: `transform.Rotate(0, 90 * Time.deltaTime, 0)`
- Smooth color/alpha changes
- Countdown timers:
```csharp
timer -= Time.deltaTime;
if (timer <= 0) { /* action */ }
```

---

## ⏸️ Time.timeScale (Time Scale)

### What it is:
A global multiplier for time speed. Default is `1.0`.

### Values:
- `1.0` — normal speed
- `0.5` — 2x slowdown
- `2.0` — 2x speedup
- `0.0` — complete stop (pause)

> [!Important]
> `Time.timeScale` affects `Time.deltaTime` but does not affect `Time.unscaledDeltaTime` (real time without scaling).

### How to use:
```csharp
// Pause
Time.timeScale = 0f;

// Slow-motion (bullet time)
Time.timeScale = 0.3f;

// Return to normal
Time.timeScale = 1f;
```

### Example: Pause with Space key
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
    {
        // Toggle pause
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;  // Resume
        else
            Time.timeScale = 0f;  // Pause
    }
}
```

### Ignoring Time.timeScale (pause menu UI)
If you need UI (pause menu, cursor animation) to keep working during pause:
```csharp
// Use unscaledDeltaTime
void Update()
{
    float realTime = Time.unscaledDeltaTime;
    // Button animations, cursor blinking, etc.
}
```

---

## ⚙️ Time.fixedDeltaTime (Fixed Time Step)

### What it is:
The interval between calls to `FixedUpdate()`. Default is `0.02` seconds (50 times per second).

### Purpose:
`FixedUpdate()` is used for physics (Rigidbody, collisions, forces). Unlike `Update()`, it is called at consistent intervals regardless of FPS.

### Relationship with Time.timeScale:
`Time.fixedDeltaTime` is automatically multiplied by `Time.timeScale` so that physics also slows down/speeds up.

### Important rule:
- Movement, rotation, animation, timers → `Update()` + `Time.deltaTime`
- Physics (forces, Rigidbody velocity, impulses) → `FixedUpdate()` + `Time.fixedDeltaTime`

### Example: Character movement with physics
```csharp
public float moveSpeed = 5f;
private Rigidbody rb;

void Start()
{
    rb = GetComponent<Rigidbody>();
}

void FixedUpdate()
{
    float horizontal = Input.GetAxis("Horizontal");
    Vector3 movement = new Vector3(horizontal, 0, 0) * moveSpeed;
    
    // For physics, use fixedDeltaTime
    rb.velocity = movement * Time.fixedDeltaTime;
}
```

### Manually changing Time.fixedDeltaTime
If you drastically change `Time.timeScale` (e.g., slowdown x0.1), Unity automatically adjusts `fixedDeltaTime`. But if you need to do it manually:
```csharp
Time.fixedDeltaTime = 0.02f; // reset to default
```
Or when dynamically changing timescale:
```csharp
Time.timeScale = 0.5f;
Time.fixedDeltaTime = 0.02f * Time.timeScale;
```

---

## 📊 Summary Table

| Parameter | Used in | Affected by timeScale | Purpose |
|------------|----------|------|------------------------|
| `Time.deltaTime` | `Update()` | ✅ Yes | Smooth movement, timers, animation |
| `Time.unscaledDeltaTime` | `Update()` | ❌ No | UI, effects during pause |
| `Time.fixedDeltaTime` | `FixedUpdate()` | ✅ Yes | Physics, forces, velocity |
| `Time.timeScale` | Global | — | Pause, slow-motion/speedup |

---

## 🎮 Practical example (all together)

Imagine a racing game. Press `R` to enable slow-motion (bullet time), and `P` to pause:
```csharp
void Update()
{
    // Normal movement (affected by timeScale)
    float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
    transform.Translate(move, 0, 0);
    
    // Slow-motion
    if (Input.GetKeyDown(KeyCode.R))
        Time.timeScale = 0.3f;
    
    // Return to normal
    if (Input.GetKeyDown(KeyCode.T))
        Time.timeScale = 1f;
    
    // Pause
    if (Input.GetKeyDown(KeyCode.P))
    {
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;
        else
            Time.timeScale = 0f;
    }
}

void FixedUpdate()
{
    // Car physics (will automatically slow down when timeScale = 0.3)
    rb.AddForce(transform.forward * engineForce * Time.fixedDeltaTime);
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
