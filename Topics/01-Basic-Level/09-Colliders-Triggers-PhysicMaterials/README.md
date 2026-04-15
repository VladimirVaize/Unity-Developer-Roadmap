# 🧱 Colliders, Triggers, and Physic Materials

> [!NOTE]
> This material explains how physical interaction between objects works in Unity.
> You will learn the difference between a regular Collision and a Trigger,
> and how to configure friction and bounce using Physic Materials.

---

## 📦 What is a Collider?
> A Collider is an invisible component added to a GameObject to define its shape for the physics engine. Without a collider, objects simply pass through each other.

### Common collider types:
- `Box Collider` — cube / cuboid.
- `Sphere Collider` — sphere.
- `Capsule Collider` — capsule (used for characters).
- `Mesh Collider` — exact shape based on the model's mesh (expensive for performance).

### How to add:
Select an object → `Add Component` → `Box Collider` (or another type).

> [!WARNING]
> For physics-based movement (forces, velocity, gravity), the object also needs a `Rigidbody` component.

---

## ⚔️ Collision vs Trigger — What's the difference?
> Every collider has an `Is Trigger` checkbox. This determines the type of interaction.

| Mode | `Is Trigger` = false (default) | `Is Trigger` = true |
|----------|------------------------------------|-----------------------|
| Behavior | Physical collision — objects bounce off, cannot pass through each other | Detector — objects pass through, but you receive entry events |
| Code events | `OnCollisionEnter`, `OnCollisionStay`, `OnCollisionExit` | `OnTriggerEnter`, `OnTriggerStay`, `OnTriggerExit` |
| Example | A ball bouncing off a wall | A door opens when the player enters an invisible zone |

---

## 🔨 How to use:

### Regular Collision:
1. Add a collider to both objects.
2. Add a `Rigidbody` to at least one (preferably the moving one).
3. Keep `Is Trigger = false`.
4. Write a script with `OnCollisionEnter`.
```csharp
void OnCollisionEnter(Collision collision)
{
    Debug.Log("Hit with: " + collision.gameObject.name);
}
```

### Trigger:
1. Add a collider.
2. Check `Is Trigger = true`.
3. Add a `Rigidbody` (can be `is Kinematic = true` if the object shouldn't move physically).
4. Use OnTriggerEnter.
```csharp
void OnTriggerEnter(Collider other)
{
    Debug.Log("Entered zone: " + other.gameObject.name);
}
```

> [!IMPORTANT]
> 🎯 Rule of thumb: For a trigger, only one of the two objects needs a `Rigidbody` (usually the player or active object). For collision, both need Rigidbody + colliders.

---

## 🧴 Physic Materials
> A Physic Material defines how surfaces interact in terms of friction and bounciness. Without it, objects slide forever or behave unnaturally.

### Properties:

| Parameter | What it does |
|--------------|-------------------------------|
| `Dynamic Friction` | Friction while moving (0 = ice, 1 = rubber) |
| `Static Friction` | Friction at rest (prevents objects from starting to slide on their own) |
| `Bounciness` | Bounce elasticity (0 = no bounce, 1 = perfect bounce) |
| `Friction Combine` | How to combine friction between two objects (Average, Minimum, Maximum, Multiply) |
| `Bounce Combine` | How to combine bounce between two objects |

### How to create and apply:
1. In the Project window → right-click → `Create` → `Physic Material`.
2. Name it, e.g., `Rubber` or `Ice`.
3. Adjust the parameters (`Bounciness = 0.8`, `Dynamic Friction = 0.1`).
4. Drag the material onto the Material field in any collider component.

### Example materials:
- 🧊 Ice: `Dynamic Friction = 0.05`, `Bounciness = 0.1`
- 🏀 Ball: `Dynamic Friction = 0.4`, `Bounciness = 0.8`
- 🧱 Brick: `Dynamic Friction = 0.9`, `Bounciness = 0.1`

---

## 🧠 Decision summary table

| What you need | What to use |
|--------------|--------------------------|
| Ball bounces off the floor | Collision + Physic Material with Bounciness |
| Player collects a coin | Trigger + `OnTriggerEnter` |
| Car brakes on asphalt | Collision + Physic Material with high friction |
| Invisible zone that opens a door | Trigger |
| Bullet bounces off a wall | Collision + Physic Material with Bounciness |
| Slippery ice floor | Collision + Physic Material with friction ~0 |

---

### ⭐ If this project was useful, put a star on GitHub!
