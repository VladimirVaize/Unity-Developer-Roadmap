# 🔍 Profiling in Unity: Finding Bottlenecks (CPU, GPU, Memory, Render)

This material covers Unity's built-in Profiler tool. It allows you to analyze your game's performance in real time, find bottlenecks, and optimize CPU, GPU, memory, and rendering.

## 🧠 What is profiling?
Profiling is the process of measuring the resources your game consumes: CPU time, GPU time, RAM, VRAM, and draw call counts. Without profiling, optimization is just guesswork.

---

## 📊 Main Profiler Tabs
Open Profiler via `Window → Analysis → Profiler` (or `Ctrl+7`).

### 1. CPU Usage 🖥️
#### What it shows:
The execution time of every method in your game: `Update()`, `FixedUpdate()`, `LateUpdate()`, physics, animation, UI, garbage collection (GC), etc.

#### How to use:
- Run the game (Play) and look at the timeline.
- Red spikes indicate frame drops.
- Expand the `CPU Usage` tab → select any frame → a hierarchical list of methods appears below with execution times (ms).
- Look for methods that take the most time (sort by `Total` or `Self`).

#### Bottleneck example:
Your `Update()` calls `FindObjectOfType()` every frame — that's slow. In the Profiler, 
`FindObjectOfType` takes 8 ms out of 16 ms per frame (target 60 FPS = ~16.6 ms/frame). Solution: cache the result in `Start()`.

### 2. GPU Usage 🎮
#### What it shows:
The time the GPU spends rendering a frame: vertex shaders, fragment shaders, fill rate, post‑effects.

#### How to use:
- Enable the `GPU Usage` tab (older Unity versions may call it `Rendering`).
- Run the game. If GPU time is higher than CPU time (e.g., GPU = 25 ms, CPU = 12 ms) — the bottleneck is the graphics card.
- Pay attention to `Draw Calls`, `Dynamic Batches`, `Shadow Casters`.

#### Bottleneck example:
You have 500 objects with different materials (500 draw calls) and complex shaders. GPU takes 30 ms per frame. 
Solution: combine materials (texture atlas), use GPU Instancing, or reduce shadow sources.

### 3. Memory 💾
#### What it shows:
Memory distribution: textures, meshes, animations, audio clips, script objects (Managed Memory). 
It also shows garbage collection (GC), which can cause freezes.

#### How to use:
- `Memory` tab → `Simple` or `Detailed` mode.
- Look at `Total Used Memory` and `GC Allocation`.
- Categories: `Assets` (textures, meshes) and `MonoHeap` (C# managed memory).
- Click `Take Sample` to capture a memory snapshot at a specific moment.

#### Bottleneck example:
Every time an enemy dies, you create a new `List<int>` inside an explosion. 
The Profiler shows frequent `GC.Alloc` spikes, then a `GarbageCollect` spike (50 ms freeze). 
Solution: use an Object Pool and avoid allocating garbage in `Update()`.

### 4. Rendering 🖼️
#### What it shows:
Detailed statistics about drawing: `SetPass Calls`, `Draw Calls`, `Batches`, `Triangles`, `Vertices`, shadow usage, and occlusion.

#### How to use:
- `Rendering` tab (sometimes merged with `GPU Usage`).
- Run the game and look at the number of `Draw Calls` and `Batches`.
- Lower is better. For mobile devices, target <100 Draw Calls.
- Check `Saved By Batching` (static/dynamic batching).

#### Bottleneck example:
The game has 2000 objects with the same material, but static batching is not enabled. 
Draw Calls = 2000. After enabling `Static Batching` (checkbox on objects) or `GPU Instancing`, Draw Calls drop to 20. FPS rises from 30 to 60.

---

## 🚦 How to find bottlenecks (practical algorithm)
1. Start the Profiler (Play) and record data<br>
   Make sure the `Record` button is on.

2. Find the slowest frame (red spike)<br>
   Click on it — details appear on the right.

3. Compare CPU vs GPU<br>
   If CPU is higher → optimize scripts, physics, GC. If GPU is higher → reduce graphics complexity (shaders, textures, shadows, geometry).

4. Look at deep profiling<br>
   Enable `Deep Profile` (slows down the game but gives full details). Find the method with the highest `Total ms`.

5. Check memory<br>
   Are there leaks (memory constantly growing)? Frequent GC spikes?

6. Use the Frame Debugger (`Window → Analysis → Frame Debugger`) together with the Profiler to see every draw call.

---

## 📌 Typical bottlenecks and solutions

| Component | Symptom in Profiler | Solution |
|--------|---------------|-----------------------------------|
| CPU | `Scripts` > 5 ms | Optimize code: remove `FindObjectOfType`, `GetComponent` in Update |
| CPU | `GarbageCollect` spikes | Object pooling, fixed‑size arrays |
| GPU | `Rendering` ≈ 20+ ms | Fewer shadows, LODs, smaller textures |
| Memory | `MonoHeap` grows forever | Leaks: unsubscribe from events, clear collections |
| Rendering | `Draw Calls` > 500 (mobile) | Batching, Atlas, GPU Instancing |

---

### ⭐ If this project was useful, put a star on GitHub!
