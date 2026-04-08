# 🧩 Transform in Unity: Position, Rotation, Scale, and Hierarchy

The Transform component is present on every GameObject in Unity. 
It determines where, how oriented, and how large the object is in the game world. 
Understanding Transform is the foundation of working with any scene.

---

## 📍 1. Position

### What it is:
The coordinates of the object's center in 3D (X, Y, Z) or 2D (X, Y) space.

### Where to set it:
In the Inspector window under the Transform component → `X`, `Y`, `Z` fields.

### How to use:
- Change the numbers directly in the Inspector to move the object.
- In a script: `transform.position = new Vector3(5, 0, 2);`
- In Scene View, move the object with the `Move` tool (hotkey `W`).

### Example:
You want to place a cube on a platform at a height of 2 meters. Set `Y = 2`. If you later move the platform, the cube will stay in place (unless it's a child). Use hierarchy for linking (see section 4).

---

## 🔄 2. Rotation

### 🔸 Euler Angles
### What it is:
Rotation around three axes (`X`, `Y`, `Z`) given in degrees (`0–360`). This is what you see in the Inspector: `Rotation X: 0, Y: 45, Z: 0`.

### How to use:
- In the Inspector: type `Y = 90` — the object rotates 90° around the vertical axis.
- In a script: `transform.eulerAngles = new Vector3(0, 90, 0);`

### ⚠️ Euler problem:
*Gimbal Lock* (loss of one degree of freedom). For example, if `X` becomes ±90°, the `Y` and `Z` axes start rotating the object identically. 
Euler works for most basic tasks, but for complex 3D animation or cameras, use Quaternion.

### 🔸 Quaternion
### What it is:
A mathematical representation of rotation without axis locking. You don't see it in the Inspector, but it's essential in code and for smooth interpolations.

### How to use in code:
- `transform.rotation = Quaternion.LookRotation(target.position - transform.position);` — rotate object to face a target.
- `transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);` — smooth rotation.

### Example (comparison):
You need a turret to always face an enemy. With Euler, this is complicated (many `if` statements). With Quaternion — one line: `LookRotation`. 
For manual adjustments in the Inspector, you still use Euler — Unity converts it to Quaternion internally.

---

## 📏 3. Scale

### What it is:
The object's size along each axis (`X`, `Y`, `Z`) relative to its original model (where `Scale = 1, 1, 1`).

### Where to set it:
Inspector → Transform → Scale.

### How to use:
- Increase `X` to `2` — the object becomes twice as wide.
- In a script: `transform.localScale = new Vector3(2, 1, 1);`
- Avoid changing scale on objects with complex colliders (Mesh Collider) — it may cause physics issues.

### Important:
If a parent has `Scale = (2, 2, 2)` and a child has `Scale = (0.5, 0.5, 0.5)`, then the child's actual scale is `(2*0.5, 2*0.5, 2*0.5) = (1, 1, 1)`.

---

## 👪 4. Hierarchy (Parent / Child)

### What it is:
A relationship where one object (parent) leads another (child). The child moves, rotates, and scales relative to the parent.

### How to create:
In the Hierarchy window, drag one object onto another. Or in a script: `transform.SetParent(parentTransform);`

### What changes:
- The child's position becomes local (relative to the parent's center). In the Inspector, you'll see `Position` (grayed text) — these are local coordinates.
- When the parent moves, the child moves with it.
- If you rotate the parent, the child rotates too (but maintains its local offset).

### Example:
Creating a character:
- Parent: `Character` (movement control).
- Children: `Body`, `Hat`, `Weapon`.
Now, moving `Character` moves the whole character as one. If you want to raise the hat, you only change the local `Y` of `Hat`.

### Access in script:
- `transform.position` — global position (world coordinates).
- `transform.localPosition` — position relative to parent.
- `transform.lossyScale` — global scale (including all parents).

---

## 🧠 Final Tips

- For simple objects and manual tweaking, use Euler (Inspector).
- For smooth rotations and precise orientation toward a target in code — use Quaternion.
- Change Scale carefully, especially with physics.
- Hierarchy is essential for grouping objects and building complex models from parts.

---

### ⭐ If this project was useful, put a star on GitHub!
