# 🧱 Collision Events in Unity: OnCollisionEnter and OnTriggerEnter

> [!Important]
> In Unity, physical interactions between objects are handled through two main types of events:
> Collision (real impact with bounce) and Trigger (zone detection without physical response).
> They are called automatically if the objects have `Collider` components and (optionally) a `Rigidbody`.

---

## 🎯 Main Differences

| Feature | OnCollisionEnter | OnTriggerEnter |
|-------------|------------------------|----------------------|
| Physical response | ✅ Yes (objects bounce) | ❌ No (objects pass through) |
| Rigidbody required | At least on one object | At least on one object (usually the moving one) |
| Method parameter | `Collision collision` | `Collider other` |
| Collider mode | `Collider` (IsTrigger = false) | `Collider` with IsTrigger = true |
| Typical use | Ball collisions, cars, falling objects | Damage zones, item pickups, trigger doors, checkpoints |

---

## ⚙️ Order of Event Calls

For two objects A and B that collide:

### 1. OnCollisionEnter / OnTriggerEnter — called once at the moment contact begins.
### 2. OnCollisionStay / OnTriggerStay — called every frame while contact continues.
### 3. OnCollisionExit / OnTriggerExit — called once when objects stop touching.

> [!note]
> 💡 If both objects have `Rigidbody` and both can move, events are called on both objects.

---

## 🚫 Limitations and Important Rules

### For OnCollisionEnter:
- Both objects must have a Collider (not Trigger).
- At least one object must have a non-kinematic `Rigidbody` (`isKinematic = false`).
- If both objects are kinematic (`isKinematic = true`), the event will not fire.
- The event is not called if objects only intersect via triggers.

### For OnTriggerEnter:
- At least one object's `Collider` must have IsTrigger = true.
- At least one object must have a `Rigidbody` (kinematic or not — doesn't matter).
- If both colliders are triggers, the event still fires (if a Rigidbody exists).
- A trigger does not generate forces, but you can manually change positions, health, destroy objects, etc.

---

## 📝 Code Examples
```csharp
// OnCollisionEnter example (deal damage on impact)
void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Enemy"))
    {
        health -= 10;
        Debug.Log("I was hit by an enemy!");
    }
}

// OnTriggerEnter example (pick up an item)
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Coin"))
    {
        Destroy(other.gameObject);
        score += 100;
    }
}
```

---

## 🔁 Summary
- **Collision** → physics + bounce + precise impact.
- **Trigger** → zone detection without physics.
- A Rigidbody is required on at least one participant.
- Always check tags to avoid reacting to every object.

---

### ⭐ If this project was useful, put a star on GitHub!
