# 🧩 Practical Task: Parallel Bullet Position Update Using Job System

## 🎯 Goal
Implement a bullet system in Unity where each bullet updates its position in a separate thread using `IJobParallelFor`, with no deadlock risk and maximum performance.

---

## 📝 Task Description
You have an array of 10,000 bullets flying forward. Each bullet must:
1. Have a current position (Vector3) and direction (Vector3)
2. Move each frame by `speed * Time.deltaTime`
3. If a bullet goes outside a boundary (e.g., farther than 1000 units), either destroy it or teleport it back

You cannot use a regular for‑loop on the main thread — that would cause frame drops. Instead, you must use the Job System.

---

## 🧠 What You Need to Implement
1. Create a job struct `BulletUpdateJob` implementing `IJobParallelFor`
2. Inside the job:
   - Accept `NativeArray<Vector3>` for positions
   - Accept `NativeArray<Vector3>` for directions
   - Accept `float deltaTime` and `float speed`
   - In `Execute(int i)`, update `positions[i]`
  
3. Inside a MonoBehavior (e.g., `BulletManager`), on every Update:
   - Allocate (or reuse) `NativeArray` with `Allocator.TempJob`
   - Fill the arrays with current bullet data
   - Create and schedule the job
   - Call `Complete()` and apply the results back to the bullets
   - Call `Dispose()` on the NativeArrays
  
---

## 🔍 Bonus (Optional, but a plus)
- Add `[BurstCompile]` for maximum performance
- Reuse arrays to avoid allocations every frame (use `Allocator.Persistent` + manual management)
- Implement boundary checking inside the job — if a bullet goes too far, reset it to a random position within a radius

---

## 📤 Expected Outcome
- Smooth frame rate with no spikes
- Stable ~60–200 FPS for 10,000 bullets
- No deadlocks or multi‑threading errors
- Clean code with comments on key parts

---

## 🚀 Hint
```csharp
[BurstCompile]
struct BulletUpdateJob : IJobParallelFor {
    public NativeArray<Vector3> positions;
    [ReadOnly] public NativeArray<Vector3> directions;
    public float deltaTime;
    public float speed;
    
    public void Execute(int i) {
        positions[i] += directions[i] * speed * deltaTime;
        // optional: boundary check
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
