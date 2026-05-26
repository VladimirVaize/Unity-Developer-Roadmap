# ✨ VFX Graph: Modern Particle System for High-Performance Effects

VFX Graph (Visual Effect Graph) is a powerful tool in Unity based on compute shaders that leverages GPU computational capabilities. 
Unlike the classic Shuriken Particle System, VFX Graph allows you to create hundreds of thousands to millions of particles with high performance and complex behavior logic.

---

## 🎯 When to use VFX Graph?

| Scenario | Classic System | VFX Graph |
| --- | --- | --- |
| Simple effects (smoke, sparks, trails) | ✅ Yes | ❌ Overkill |
| Mass effects (bird flock, crowd, pollen) | ❌ Low performance | ✅ High performance |
| Visual spectacles (magic, explosions, fluid simulation) | 🟡 Limited | ✅ Unlimited |
| Interactive effects (response to player, physics) | 🟡 Difficult | ✅ Visual programming |

---

## 🧠 Key Concepts of VFX Graph
### 1. Graph
A visual editor where you create effects by connecting Blocks and Nodes. 
Blocks define what happens to particles (movement, color, shape), while nodes manage the flow of data.

### 2. Contexts
These are the main stages of an effect's life, arranged left to right in the graph:
- Initialize — initial particle values (position, velocity, color, lifetime)
- Update — particle behavior each frame (forces, gravity, rotation, collisions)
- Output — how and where particles are rendered (material, shape, blending)

### 3. Blocks
Drag them into contexts. For example:
- `Set Position (Cone)` — particles spawn from a cone shape
- `Add Velocity (Random)` — random initial velocity
- `Gravity` — downward pull
- `Collide with Scene` — collision with scene geometry

### 4. Particle Attributes
Each particle has built-in attributes: `position`, `velocity`, `color`, `lifetime`, `age`, `size`, `angle`, `alive`, etc. 
You can modify these using blocks.

---

## ⚙️ Step-by-Step Example: Creating a Fire Explosion
### 1. Create a VFX Graph asset:
`Assets → Create → Visual Effects → Visual Effect Graph`

### 2. Create an object in the scene:
`GameObject → Visual Effects → Visual Effect`

### 3. Open the graph and configure the contexts:
Initialize:
- `Set Position (Sphere)` — radius 2
- `Set Lifetime (Random)` — 1 to 2 seconds
- `Set Color (Gradient)` — yellow to red
- `Set Size (Random)` — 0.2 to 0.8

Update:
- `Add Velocity (Radial)` — bursts outward from center
- `Gravity` — falls downward
- `Drag` — slows down over time

Output:
- Choose `Quad Output` (flat sprites)
- Assign a material with a fire texture and additive blending

### 4. Use Exposed Parameters
Create a `SpawnRate` parameter (float) and bind it to the `Rate` in the spawn block. 
Now you can adjust explosion intensity from the Inspector during gameplay!

---

## 🚀 Advanced Features
- Visual Programming: create custom logic with nodes (`Lerp`, `Noise`, `Sample Curve`, `Branch`)
- Scene Interaction: effects react to colliders, triggers, and objects (via `Block: Collide with Scene` and `Trigger Event`)
- Events: trigger effects from scripts (`Send Event` on the Visual Effect component)
- Blackboard: categorized parameters, textures, meshes, and gradients

---

## 📌 Tip for Beginners

> Start with templates: `Create → Visual Effects → VFX Graph Templates`.
> Explore ready-made effects (fire, smoke, sparks), modify blocks, and observe how particle behavior changes.

---

### ⭐ If this project was useful, put a star on GitHub!
