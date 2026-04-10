# 📐 Vector Mathematics in Unity: Fundamentals for Game Logic

Vectors are the foundation of movement, direction, distances, and countless calculations in a game engine. 
Unity most commonly uses **Vector2** (for 2D) and **Vector3** (for 3D), along with key operations: **Distance**, **Lerp**, **Dot Product**, and **Cross Product**. 
Understanding these tools turns chaotic movement into controllable and predictable game mechanics.

---

## 1. Vector2 and Vector3 📦

### What it is:
Structures that store coordinates in space.
- `Vector2` → `(x, y)` — for UI, 2D games, screen coordinates.
- `Vector3` → `(x, y, z)` — for the 3D world, physics, Transform.position.

### Where to use:
- Object position: `transform.position = new Vector3(5, 0, 10);`
- Movement direction: `Vector3 direction = target - transform.position;`
- Scale: `transform.localScale = new Vector3(2, 2, 2);`

### Useful constants:
- `Vector3.zero` → (0,0,0)
- `Vector3.one` → (1,1,1)
- `Vector3.up` → (0,1,0)
- `Vector3.forward` → (0,0,1)
- `Vector3.right` → (1,0,0)

### Example:
Move an enemy upward by 2 units:
```csharp
transform.position += Vector3.up * 2;
```

---

## 2. Distance 📏

### What it is:
The distance between two points in space. Returns a `float`.

### Formula (inside Unity):
`Vector3.Distance(A, B) = length of vector (B - A)`

### Where to use:
- Check if the player has entered the enemy's attack radius.
- Activate dialogue when approaching an NPC.
- Destroy a bullet that has traveled its maximum distance.

### Example:
Enemy attacks if the player is within 5 meters:

```csharp
if (Vector3.Distance(transform.position, player.position) < 5f)
{
    Attack();
}
```

---

## 3. Lerp (Linear Interpolation) 🔄

### What it is:
Smooth transition from one value to another over time.<br>
`Vector3.Lerp(start, end, t)` where `t` ranges from 0 to 1.

- `t = 0` → `start`
- `t = 1` → `end`
- `t = 0.5` → exactly in the middle

### Where to use:
- Smooth camera follow (lagging behind the player).
- Smooth color, scale, or position changes.
- Moving an object along an arc.

### Example:
Smoothly move an object from point A to point B over 2 seconds:

```csharp
float t = Time.time / 2f; // from 0 to 1 over 2 seconds
transform.position = Vector3.Lerp(startPoint, endPoint, t);
```

Tip: For constant speed, use `MoveTowards`. For smooth damping, use `Lerp` with `Time.deltaTime`:

```csharp
transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * smoothSpeed);
```

---

## 4. Dot Product ⚫

### What it is:
A measure of how much two directions align.<br>
Returns a `float` from -1 to 1:
- `1` → pointing exactly in the same direction.
- `0` → perpendicular.
- `-1` → exactly opposite.

Formula: `dot = Vector3.Dot(a.normalized, b.normalized)`

### Where to use:
- Determine if an enemy is facing the player (value > 0.7).
- Check if an object is in front or behind.
- Calculate the angle between two vectors: `angle = Mathf.Acos(dot) * Mathf.Rad2Deg`.
- Lighting systems (the higher the dot with the surface normal, the brighter).

### Example:
Check if an enemy sees the player in front (not necessarily line-of-sight, but direction of gaze):

```csharp
Vector3 toPlayer = (player.position - transform.position).normalized;
float dot = Vector3.Dot(transform.forward, toPlayer);
if (dot > 0.5f)
{
    Debug.Log("Player is in front and within view!");
}
```

---

## 5. Cross Product ✖

### What it is:
A vector perpendicular to two input vectors. In 3D, it allows you to get a "left/right" direction relative to a forward direction.

### Properties:
- `Vector3.Cross(a, b)` → result is perpendicular to both a and b.
- The length of the result = area of the parallelogram formed by a and b.
- Direction follows the right-hand rule.

### Where to use:
- Determine whether to turn left or right to face a target.
- Calculate the axis of rotation.
- Build a custom coordinate system (e.g., for a custom camera).

### Example:
Find out if the player is to the left or right of the enemy (for "circle left" behavior):

```csharp
Vector3 toPlayer = player.position - transform.position;
Vector3 cross = Vector3.Cross(transform.forward, toPlayer);
if (cross.y > 0)
    Debug.Log("Player is to the right");
else if (cross.y < 0)
    Debug.Log("Player is to the left");
```

---

## 🧠 Cheat Sheet for Game Logic

| Task | Tool | Example Usage |
|----------|--------|----------------------|
| Object position | Vector3 | `transform.position` |
| Distance to target | Distance | `if(Distance < 5) Attack()` |
| Smooth movement | Lerp | `pos = Lerp(pos, target, dt)` | 
| Is it facing me? | Dot > 0.7 | `if(Dot(forward, toTarget) > 0.7f)` |
| Left or right? | Cross.y | `float side = Cross(forward, toTarget).y` |

---

### ⭐ If this project was useful, put a star on GitHub!
