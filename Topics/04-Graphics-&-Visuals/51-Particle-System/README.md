# ✨ Particle System in Unity: Complete 2D Animation Guide

The Particle System is a powerful Unity component for creating dynamic 2D/3D effects: smoke, fire, sparks, rain, leaves, magic, etc. 
In 2D games, it's often used for background atmosphere, hit effects, explosions, and interactions.

> 📌 All parameters are found in the Inspector window when you select a GameObject with a `Particle System` component.

---

## 🧩 1. System Modules
Modules are logical blocks you can enable/disable. Each adds specific behavior.

| Module | Purpose |
| --- | --- |
| Main | Core settings: duration, looping, start speed, max particles. |
| Emission | Controls particles emitted per second (rate) or in bursts. |
| Shape | The shape of the area where particles are born (sphere, cone, rectangle, edge, etc.). |
| Velocity over Lifetime | Particle velocity changes over time. |
| Color over Lifetime | Particle color changes smoothly. |
| Collision | Particles collide with scene objects (solid surfaces, colliders). |
| Sub Emitters | Spawns another particle system on events (birth, death, collision). |
| Renderer | How each particle is drawn (material, sorting, lighting). |

---

## 🚀 2. Emission
Defines how and when particles appear.

- Rate over Time – particles per second (e.g., 30 particles/sec).
- Rate over Distance – particles per meter the system moves (useful for wheels, trails).
- Bursts – instant emission of a fixed number of particles at a specific moment (explosion).

📌 *Example*: For continuous smoke, use `Rate over Time = 20;` for an explosion, use a single Burst of 100 particles at time 0.

---

## 🔘 3. Shape
Defines the geometry of the particle source.

| Shape | 2D Usage |
| --- | --- |
| Rectangle | Particles born in a rectangle – perfect for rain or falling leaves. |
| Circle | Spells, auras, sparks from a point source. |
| Edge | Particles along a line – fire strip, sword slash. |
| Mesh | Complex shapes (rare in 2D). |

> ⚙️ In 2D, `Shape: Rectangle` with width/height set in world units is common.

---

## 💥 4. Collision
Allows particles to interact with physics objects (walls, platforms, enemies).

- Type:
  - `Planes` – infinite planes (fast but coarse).
  - `World` – scene object colliders.
 
- Mode:
  - `Collision` – particle bounces or disappears.
  - `Kill` – particle is removed on contact (e.g., spark against a wall).
 
- Dampen – speed loss after impact.
- Bounce – elasticity.

🔧 For 2D games, enable `2D Colliders` in collision settings so particles work with `BoxCollider2D`, `CircleCollider2D`.

---

## 🌿 5. Sub‑Emitters
Creates a nested particle system that triggers in response to an event in the parent system.

| Event | What happens |
| --- | --- |
| Birth | On each particle birth, a child effect is created. | 
| Collision | On particle collision, a new effect spawns (sparks, smoke). |
| Death | On particle death, another effect triggers (explosion, debris). |

📌 **Example**: A spark particle hits a wall → on `Collision`, a "smoke puff" subsystem is created, which fades quickly.

To configure: in the `Sub Emitters` module, click `+`, select an event, and assign another Particle System (prefab or child object).

---

## ⚡ 6. Particle Optimization (Performance)
Too many particles kill performance, especially on mobile devices.

### ✅ Practical tips:

| Problem | Solution |
| --- | --- |
| Too many living particles | Reduce `Max Particles` (Main module). |
| Long-lived particles | Use `Limit Velocity` over Lifetime and shorten `Start Lifetime`. |
| Complex collisions | For distant or small effects, disable `Collision` or use simple `Planes`. |
| Many Sub‑Emitters | Avoid chains of 3+ levels; combine effects into one prefab. | 
| Rendering | Use simple materials (Mobile/Particles‑additive) and small textures. |
| Burst spikes | Spread bursts over several frames (reduce particles per Burst). |

> 🔁 Use the **Particle System Profiler** (Profiler window → Particles) to find bottlenecks.

---

## 🧪 7. Quick Setup Example (for a 2D game)
Task: Create a falling autumn leaves effect.

1. Create a `Particle System` → in Main: Loop = true, Start Lifetime = 3 sec, Start Speed = 0.5 (downward).
2. Emission: Rate over Time = 15.
3. Shape: Rectangle (width 10, height 1) at the top of the screen.
4. Color over Lifetime: yellow → red → transparent.
5. Texture Sheet Animation: assign a leaf sprite (leaf atlas).
6. Collision: World + 2D Colliders, Dampen = 0.5 (leaves bounce slightly off the ground).
7. Sub Emitters: on particle death (Death), trigger a small "pollen" effect.

---

### ⭐ If this project was useful, put a star on GitHub!
