# 🎯 Task: «Optimizing a Forest with 10,000 Trees»
You are developing an open-world RPG. You need to place 10,000 trees in the scene, 
but performance drops to 5-10 FPS. Your task is to optimize rendering using Static Batching and GPU Instancing.

## 📝 What to Implement:
### Part 1: Creating a Test Scene
1. Create a simple tree mesh (Cylinder + Sphere or import a model)
2. Create a material for the tree (enable GPU Instancing)
3. Create a script `TreeSpawner.cs`

### Part 2: Three Rendering Modes
Implement the ability to switch between three modes:
| Mode | Description |
| --- | --- |
| Standard | Regular creation of 10,000 GameObjects with MeshRenderer |
| Static Batching | Same objects marked as Static and combined | 
| GPU Instancing | Using `Graphics.DrawMeshInstanced` without GameObjects |

### Part 3: Functionality
The `TreeSpawner` script must:
1. Generate trees at random positions on a plane (size 500x500)
2. Switch modes using keys `1`, `2`, `3`
3. Show statistics on UI:
   - FPS
   - Draw Calls (via `UnityStats.drawCalls`)
   - Number of trees
   - Current mode
  
4. Tree animation (only for GPU Instancing):
   - Each tree slightly sways in the wind (via shader)
   - Use `MaterialPropertyBlock` to pass animation phase
  
### Part 4: Optimized Shader
Create a shader for GPU Instancing that supports:
- Different colors (green → yellow → brown) for variety
- Wind sway animation (use `_WindStrength` and `_Time`)

### Part 5: Performance Analysis
Run each mode and record:
- Average FPS
- Number of Draw Calls
- Memory usage (RAM)

Log conclusions to console when switching modes.

---

## 🧰 Implementation Requirements:
- Use one material for all trees
- In GPU Instancing mode, use `MaterialPropertyBlock` to set color and animation phase
- Static objects must be marked as `isStatic = true`
- For static batching, call `StaticBatching.Combine()`
- Add UI buttons for switching modes

---

## 🔍 Verification:
1. Standard mode — 10,000 separate objects → FPS < 15
2. Static Batching mode — 1 Draw Call → FPS ~ 60
3. GPU Instancing mode — ~10 Draw Calls (10,000 / 1023) → FPS ~ 50-60
4. Visually, trees should look the same in all modes
5. Wind animation works only in GPU Instancing mode

---

## 💡 Expected Console Output:
```text
[TreeSpawner] Spawned 10000 trees
[TreeSpawner] Mode: Standard -> FPS: 12, Draw Calls: 10001
[TreeSpawner] Mode: Static Batching -> FPS: 58, Draw Calls: 1, Memory: +15MB
[TreeSpawner] Mode: GPU Instancing -> FPS: 54, Draw Calls: 10, Memory: +2MB
[TreeSpawner] Best performance: GPU Instancing (best memory, good FPS)
```

---

## 🏆 Bonus Task:
Add LOD (Level of Detail) for GPU Instancing:
- Trees close to camera — full detail
- Distant trees — simplified mesh (e.g., a cross of 2 planes)
- Use `Graphics.DrawMeshInstanced` with different meshes for different distances

---

### ⭐ If this project was useful, put a star on GitHub!
