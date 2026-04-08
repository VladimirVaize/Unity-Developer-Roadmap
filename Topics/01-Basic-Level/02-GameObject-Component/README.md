# 🧩 GameObject and Component: Container vs Behavior

In Unity, everything that exists in a scene is a GameObject. 
But a GameObject by itself is an empty box. All functionality is given to it by Components. 
Understanding this difference is the key to flexible, confusion-free architecture.

---

## 📦 What is a GameObject?

### A GameObject is a container. It has no properties like "speed", "health", or "model". It only has:
- A name (`name`)
- An active state (`activeSelf` / `SetActive()`)
- Position, rotation, scale (via the mandatory `Transform` component)
- A list of attached components

> 🧠 Think of a GameObject as an empty plastic box with a name sticker. The box itself does nothing.

### Examples of GameObjects:
- Empty object (`Create Empty`)
- Cube, sphere, capsule (actually GameObject + `MeshFilter` + `MeshRenderer` components)
- Character, wall, light source, camera

---

## ⚙️ What is a Component?

### A Component is a behavior, data, or capability that you add to a GameObject. A component:
- Cannot exist on its own (always attached to a GameObject)
- Gives the object concrete properties: physics, visuals, sound, logic
- Can be removed, added, or temporarily disabled

> 🧠 Think of components as tools you put inside the box: an engine (Rigidbody), paint (MeshRenderer), a microphone (AudioSource), a brain (your script).

### Examples of components:
- `Transform` (exists on every GameObject, cannot be removed)
- `MeshRenderer` — to make the object visible
- `Rigidbody` — to apply gravity and physics
- `AudioSource` — to play sound
- Your own script `PlayerHealth.cs` — to store health and react to damage

---

## 🔄 Key Difference (short)

| Feature | GameObject | Component |
|--------------|------------------------|------------------------|
| Essence | Container | Behavior / Data |
| Can exist alone? | ✅ Yes | ❌ No (only inside a GameObject) | 
| Added / removed | Create/destroy object | ✅ Can be added and removed |
| Example | "Character", "Door" | "Movement script", "Collider" |

---

## 🛠️ How to use in practice

### 1️⃣ Create an empty GameObject
Hierarchy → Right-click → `Create Empty` → name it `Enemy`

### 2️⃣ Add components via the Inspector
Select `Enemy` → click `Add Component`:

- `Mesh Filter` → choose a mesh (e.g., sphere)
- `Mesh Renderer` → assign a material (color)
- `Capsule Collider` → to enable collisions
- `Rigidbody` → so it falls under gravity
- Your script `EnemyAI` (write it) → to make the enemy move toward the player

### 3️⃣ Disable components during play

Uncheck `Rigidbody` in the Inspector → gravity disappears.<br>
Uncheck `Mesh Renderer` → the enemy becomes invisible (but still runs its logic).

---

## 🧪 Typical beginner mistake

❌ "My script cannot see the Speed variable of another object"<br>
✅ You need to understand: the variable is not inside the GameObject, but inside a specific component (e.g., `PlayerMovement`). Access it like this:

```csharp
GameObject player = GameObject.Find("Player");
PlayerMovement movement = player.GetComponent<PlayerMovement>();
float speed = movement.speed;
```

---

## 🧠 The Golden Rule of Unity

> The GameObject defines WHAT exists. The Component defines WHAT IT DOES and HOW IT LOOKS.

---

### ⭐ If this project was useful, put a star on GitHub!
