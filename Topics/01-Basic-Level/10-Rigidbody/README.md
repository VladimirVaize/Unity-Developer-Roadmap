# 🧱 Rigidbody: Forces, Gravity, and Kinematic Mode

> [!IMPORTANT]
> Rigidbody is one of the most important components in Unity — it enables physics for a GameObject.
> Without it, the object will not respond to gravity, collisions, or external forces.
> This material explains three key concepts: forces (AddForce), gravity, and kinematic mode (isKinematic).

---

## 🌍 Gravity

### What it is:
Gravity is a constant force pulling objects downward (along the Y-axis by default). 
In the real world, this is Earth's attraction; in Unity, it's a predefined vector `Physics.gravity = (0, -9.81, 0)`.

### Where to configure:
Inside the `Rigidbody` component → the `Use Gravity` checkbox. Enabled by default.

### How to use:
- ✅ `Use Gravity = true` — the object falls until it hits a collider (ground, floor).
- ❌ `Use Gravity = false` — the object does not fall, even if in mid-air (useful for spaceships, bullets that shouldn't drop).

### Example: 🍎
You create an apple with a `Rigidbody` and a collider. If `Use Gravity = true`, the apple falls to the ground. If `false` — it hangs in the air.

---

## 💥 Forces (AddForce)

### What it is:
`AddForce` is a method that applies a temporary or continuous force to an object with a `Rigidbody`. Force changes the object's velocity according to Newton's laws (F = m × a).

### How to use in code (C#):
```csharp
rigidbody.AddForce(Vector3 force, ForceMode mode);
```

### ForceMode options:
  - `ForceMode.Force` — regular force (takes mass into account). Good for pushes, hits.
  - `ForceMode.Impulse` — instant burst (takes mass into account). Ideal for jumps, explosions.
  - `ForceMode.VelocityChange` — changes velocity ignoring mass.
  - `ForceMode.Acceleration` — changes acceleration ignoring mass.

### Example: 🦘
Character jump:
```csharp
if (Input.GetKeyDown(KeyCode.Space))
{
    rigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
}
```

### Why it's used:
- Enemy movement (constant forward force)
- Knockback from explosions
- Car controls (gas = forward force)
- Jumps, shots, kicks

---

## 🔒 Kinematic Mode (isKinematic)

### What it is:
`isKinematic` is a switch that tells the physics engine: 
"*Do not apply gravity, forces, or automatic collisions to me, but I can still move the object manually (via Transform or animation) and I will still interact with collisions*".

### How to enable:
In the `Rigidbody` component → check `Is Kinematic`.<br>
Or in code: `rigidbody.isKinematic = true;`

Difference between regular and kinematic Rigidbody:

| Mode | Gravity | AddForce | Movement via Transform | Collisions with others | 
|-----------|------|------|------|-----------------|
| Regular Rigidbody | ✅ yes | ✅ yes | ❌ bad | ✅ yes (physical) |
| Kinematic (isKinematic = true) | ❌ no | ❌ no | ✅ good | ✅ yes (triggers & info only) |

### When to use:
- 🚪 Doors, elevators, killer platforms (move precisely but should push the player)
- 🎣 Hookshot or flying object that shouldn't change trajectory from hits
- 🎬 Animated objects that shouldn't fall due to physics (swinging pendulum)

### Example: 🚀
A platform moves back and forth via script (`transform.Translate`). 
If `isKinematic = false` — the player standing on it might push it or cause physics glitches. 
If `isKinematic = true` — the platform moves like on rails, but the player will still stand and move with it.

---

## 🧪 Combined Example
You create a soccer ball (`Rigidbody`, collider, mass = 0.5).
- By default: gravity enabled, `isKinematic = false`.
- A kick: `AddForce(direction * 10f, ForceMode.Impulse)`.
- The ball flies, then falls, bounces off walls.
- You want a "magic ball" that doesn't fall but still reacts to kicks: disable gravity, keep `isKinematic = false`.
- You want a decorative ball that never moves from physics and just floats in place: `isKinematic = true`.

---

## 📌 Quick Summary

| Concept | Key idea | 
|------------------|----------------------------------|
| Gravity (`Use Gravity`) | Does the object fall down? |
| `AddForce` | Apply a push, burst, or continuous force |
| `isKinematic` | Object is manually controlled; physics ignores it |

---

### ⭐ If this project was useful, put a star on GitHub!
