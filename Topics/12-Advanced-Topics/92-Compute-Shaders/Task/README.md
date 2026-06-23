# 🎯 Task: «GPU-Accelerated Star Cluster Simulation»
You are developing a space simulator "StarCluster" that needs to simulate gravitational interaction of thousands of stars in real-time. 
The CPU can't handle N-Body simulation (> 10,000 objects). Use Compute Shaders for gravity calculation, movement, and star rendering.

## 📝 What to Implement:
### Part 1: Structures and Buffers
1. Create `Star` struct in Compute Shader:
```hlsl
struct Star
{
    float3 position;
    float3 velocity;
    float mass;
    float luminosity;
    float temperature;
    uint color;  // Packed color (RGBA)
};
```

2. Create two buffers in C#:
   - `starBuffer` — RWStructuredBuffer for read/write
   - `colorBuffer` — output buffer for colors (or direct rendering)
  
### Part 2: Gravity Simulation
3. Implement `ApplyGravity` kernel:
   - Calculate gravitational force from all stars to each star (N-Body)
   - Use softening: `dist = max(dist, 0.01f)` to avoid singularity
   - Update velocity and position
  
4. Implement `ResolveCollisions` kernel:
   - Check collisions between stars
   - On collision, merge stars or bounce them apart
   - Use restitution coefficient `0.3f`
  
### Part 3: Rendering
5. Implement star rendering via GPU Instancing:
   - Use `Graphics.DrawMeshInstancedProcedural`
   - Pass positions and colors through buffers
   - Star size depends on mass
  
6. Create `StarShader.shader`:
   - Receives positions and colors from buffer
   - Renders star as a point with glow
   - Color depends on temperature (from red to blue)
  
### Part 4: Control and Interaction
7. Implement camera controls:
   - Orbit around the cluster center
   - Zoom in/out
   - Add stars on click (new objects to buffer)
  
8. Add two simulation models:
   - `Spiral Galaxy` — stars distributed in spiral arms
   - `Globular Cluster` — spherical cluster
  
### Part 5: Optimization
9. Optimize N-Body simulation:
    - Use Barnes-Hut or Particle Mesh for acceleration
    - Or use approximation: divide into cells and calculate forces from groups
  
10. Implement Level of Detail (LOD):
    - Close stars — detailed rendering
    - Far stars — point rendering
   
### Part 6: Profiling and Debugging
11. Display in UI:
    - Star count
    - FPS
    - Compute Shader execution time per frame
   
12. Add data export capability:
    - Save star positions to JSON or CSV
    - Load saved configuration
   
---

## 🧰 Implementation Requirements:
- Minimum stars: 10,000
- Maximum stars: 100,000+
- Frame Rate: 60 FPS on modern GPUs
- Use ComputeBuffer with flag `ComputeBufferType.Structured`
- Use GPU Random for initialization (not CPU)
- Implement model switching without reloading

---

## 🔍 Verification:
1. Run simulation with 10,000 stars
2. Verify FPS > 30 on average GPU
3. Verify stars move smoothly and interact
4. Verify clicking adds a new star
5. Verify model switching works correctly
6. Run Unity Profiler and check GPU time

---

## 🏆 Bonus Task (Optional):
Implement A Pathfinding for Ships* on GPU:
- 100 ships flying to different targets
- Use Compute Shader for grid-based pathfinding
- Visualize movement trajectories

---

### ⭐ If this project was useful, put a star on GitHub!
