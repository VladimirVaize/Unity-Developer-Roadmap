# ⚡ Burst Compiler: High-Performance Compiler for the Job System

Burst Compiler is an optimizing AOT (Ahead-Of-Time) compiler for Unity that converts managed C# code — written under specific constraints (inside `jobs`) — into highly efficient native code. 
Combined with the Job System, it allows you to harness the full power of modern multi-core CPUs without the complexity of manual thread management.

---

## 🎯 Why do you need Burst?
- 🚀 Massive performance boost — 5-100x speedup compared to regular C# for compute-heavy tasks.
- 🔁 Automatic SIMD (Single Instruction, Multiple Data) — the compiler vectorizes loops.
- 🔒 Safety — Burst works only inside `unsafe` or specially marked code (`[BurstCompile]`), preventing memory management errors.
- 🧠 Readable assembly output — for profiling and extreme optimization.
- 🎮 Perfect for games — procedural generation, particle physics, AI (massive unit counts), mesh processing, simulations.

---

## 🧠 How it works with the Job System?

1. You write a struct implementing one of the interfaces: `IJob`, `IJobParallelFor`, `IJobChunk`, etc.
2. Mark the `Execute()` method with the `[BurstCompile]` attribute.
3. Schedule the job using `Schedule()`.
4. Burst compiles it before the first execution into the fastest possible native code.
5. The Job System distributes execution across worker threads.

---

## 📦 What do you need to use Burst?
1. Install the packages:
   - `com.unity.jobs`
   - `com.unity.burst`
   - `com.unity.collections` (for `NativeArray<T>` and similar)
  
2. Import the required namespaces:
   ```csharp
   using Unity.Burst;
   using Unity.Jobs;
   using Unity.Collections;
   ```

3. Write a job using only allowed types:
   - `NativeArray<T>`, `NativeList<T>` (but not regular `List<T>`)
   - `float`, `int`, `bool`, `Vector3`, `Mathf` (but not .NET `Math`)
   - No reference types (`class`, `string`), virtual methods, or exceptions.
  
4. Ensure the Burst option is enabled in the project settings (Editor → Project Settings → Burst AOT Settings).

---

## ✅ Example of a simple job with Burst
```csharp
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
public struct VelocityJob : IJobParallelFor
{
    public NativeArray<Vector3> positions;
    public NativeArray<Vector3> velocities;
    public float deltaTime;

    public void Execute(int i)
    {
        positions[i] += velocities[i] * deltaTime;
    }
}
```
### Usage:
```csharp
NativeArray<Vector3> positions = new NativeArray<Vector3>(1000, Allocator.TempJob);
NativeArray<Vector3> velocities = new NativeArray<Vector3>(1000, Allocator.TempJob);
// ... fill data
VelocityJob job = new VelocityJob
{
    positions = positions,
    velocities = velocities,
    deltaTime = Time.deltaTime
};
JobHandle handle = job.Schedule(positions.Length, 64); // 64 = batch count
handle.Complete();
positions.Dispose();
velocities.Dispose();
```

---

## ⚠️ Limitations and pitfalls
- ❌ Cannot use `Debug.Log` inside a Burst-compiled job.
- ❌ Cannot access `GameObject` or MonoBehaviour components.
- ❌ Must use `Unity.Mathematics` instead of standard math functions for maximum performance.
- ✅ Can use `float3`, `float4x4`, `math.sqrt()`, etc.
- ✅ Burst does not work with virtual methods or interfaces inside the job.

---

## 🧪 When is Burst NOT needed?
- Small data sets (less than 100-500 iterations) — compilation/scheduling overhead does not pay off.
- Code runs very infrequently (once every few seconds).
- You heavily use reflection, exceptions, or complex object models.

---

## 💡 Conclusion
Burst Compiler + Job System is a "silver bullet" for compute-intensive tasks in Unity. 
They turn C# code into near-C++ performance and efficiently load all CPU cores without thread-management headaches.

---

### ⭐ If this project was useful, put a star on GitHub!
