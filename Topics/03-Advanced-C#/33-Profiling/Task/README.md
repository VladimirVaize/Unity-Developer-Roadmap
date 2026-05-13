# 🧪 Practical Task: Optimizing a "Laggy Scene"
You have received a Unity project where a scene with a spaceship and 500 asteroids runs very slowly (~15 FPS). 
Your task is to use the Profiler and Frame Debugger to find bottlenecks and fix them.

## 🎯 Goal
Achieve stable 60 FPS on an average‑performance desktop (or 30 FPS on mobile if specified).

---

## 📁 Initial data (imagine you have it)
- Scene `SpaceScene.unity`
- 1 player (spaceship) with a ~500‑triangle mesh
- 500 asteroids, each a separate GameObject with a 200‑triangle mesh
- Each asteroid has its own material (500 different materials)
- Each asteroid has an `AsteroidRotate.cs` script with an `Update()` method that calls `transform.Rotate` and `FindObjectOfType<Player>()` to check distance
- One Directional Light with hard shadows on all objects
- Bloom post‑effect (camera with `Post-Processing Volume`)

---

## 🔍 Step 1: Profiling (diagnosis)
Enable the Profiler (`Window → Analysis → Profiler`), run the scene, and answer these questions:
1. CPU Usage: Which method or system takes the most time? Are there `GarbageCollect` spikes?
2. GPU Usage: What is the GPU time? Compare it to CPU.
3. Rendering: How many `Draw Calls` and `Batches`? How many `SetPass Calls`?
4. Memory: Are there constant allocations (GC) every frame? Which scripts cause them?

---

## 🛠️ Step 2: Fixing bottlenecks (optimization)
Based on the Profiler data, propose and implement (mentally or in code) the following improvements:

### Issue 1: CPU due to `AsteroidRotate` script
- Cause: `FindObjectOfType<Player>()` is called 500 times per frame.

<details>
<summary>Solution:</summary>
Get a reference to the player once in <code>Start()</code> and cache it. If the player can be destroyed/recreated, use an event or a Singleton.

</details>

### Issue 2: GPU and Rendering — too many draw calls
- Cause: 500 asteroids with different materials + shadows from the Directional Light result in 500+ draw calls.

<details>
<summary>Solution (pick one or combine):</summary>
  A) Combine all asteroids into 1 material (Texture Atlas).<br>
  B) Enable <code>GPU Instancing</code> on the asteroid material.<br>
  C) Make asteroids static and enable <code>Static Batching</code>.<br>
  D) Lower shadow quality (Hard Shadows, Shadow Resolution = Low).

</details>

### Issue 3: Bloom post‑effect eats GPU
- Cause: Bloom scales the whole frame.

<details>
<summary>Solution:</summary>
Lower Bloom resolution (Half Resolution) in Post‑Processing settings, or disable it for older devices.

</details>

### Issue 4: Memory (if leaks or GC exist)
- Cause: Each asteroid creates garbage (e.g., `new List<Vector3>()` in `Update`).

<details>
<summary>Solution:</summary>
Move collection creation to <code>Start()</code> or <code>Awake()</code>, or use array pooling.

</details>

---

## ✅ Step 3: Verify the result
After each change, run the Profiler again and compare:
- FPS (should rise to 50–60)
- Draw Calls (should drop to <100)
- CPU/GPU time (should be ≤16 ms for 60 FPS)
- GC Allocation (should be 0 bytes per frame)

---

## 💡 Hint
> If you don't know where to start — first enable Deep Profile and find the longest call in CPU Usage.
> Then switch to Rendering and count draw calls. 80% of problems are in these two areas.

---

### ⭐ If this project was useful, put a star on GitHub!
