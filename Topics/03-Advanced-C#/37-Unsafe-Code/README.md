# 🧠 Unsafe Code and NativeArray: When and How to Use

This material covers working with unsafe code and Native containers (specifically `NativeArray`) in Unity. 
These tools are essential for high-performance computing, especially when using the Job System and Burst Compiler. 
We'll explore what they are, when their use is justified, and when it's not.

---

## 🚨 What is Unsafe Code?
In C# (and Unity), "safe" code runs under the Garbage Collector (GC) and performs bounds checking on arrays. Unsafe code allows you to:
- Use pointers (like in C/C++)
- Work with directly allocated memory (`stackalloc`, `Marshal.AllocHGlobal`)
- Disable array bounds checking for speed

### How to enable:
`Project Settings → Player → Allow 'unsafe' Code` (checkbox). In your script, add `unsafe` to the class or method.
```csharp
unsafe void FastCopy(float* source, float* dest, int count) 
{
    for (int i = 0; i < count; i++) dest[i] = source[i];
}
```

> [!Warning]
> Unsafe code is not managed by the GC and can lead to memory leaks, data corruption, and crashes. Use only when absolutely necessary!

---

## 📦 NativeArray and Other Native Containers
Native containers are structs that store data in "native" memory (not in the managed GC heap). 
They are needed to pass data into Jobs, because Jobs cannot access managed objects.

### Main container types:
| Container            | Purpose                                         |
| ---                  | ---                                             |
| `NativeArray<T>`     | Ordered list of elements (like a regular array) |
| `NativeList<T>`      | Dynamic list                                    |
| `NativeHashMap<K,V>` | Dictionary (hash table)                         |
| `NativeQueue<T>`     | Queue                                           |

### Creating a NativeArray:
```csharp
NativeArray<float> data = new NativeArray<float>(1000, Allocator.Temp);
// Allocator.Temp — fast, lasts up to 4 frames
// Allocator.Persistent — until manually disposed
// Allocator.TempJob — for short Jobs (4 frames)

// Always dispose!
data.Dispose();
```

### Usage in a Job:
```csharp
struct MyJob : IJob
{
    public NativeArray<float> results;
    public void Execute()
    {
        for (int i = 0; i < results.Length; i++)
            results[i] = results[i] * 2;
    }
}
```
---

## ⚖️ When Is Using Unsafe and NativeArray Justified?
### ✅ Justified (performance optimization):
- Processing thousands/millions of objects (particles, rays, vertex meshes)
- Real-time physics/animation (must run >60 FPS)
- Using Job System + Burst (NativeArray is required)
- Reading large binary files (custom level/model formats)
- Low-level algorithms (A* pathfinding, heat convection, fluid simulation)

### ❌ NOT justified (complexity without benefit):
- Simple scenarios with <100 objects
- UI logic, input, networking (bottleneck is not CPU there)
- Team projects without an experienced developer (risk of subtle bugs)
- Prototyping (development speed matters more than 20% performance gain)

### 💡 Golden Rule:
> First write safe managed code. If the Profiler shows that a specific section is a bottleneck, then refactor it to unsafe/NativeArray.

---

## 🔍 Example: Speed Comparison
```csharp
// Slow method (GC + bounds checks)
void ManagedProcess(float[] array)
{
    for (int i = 0; i < array.Length; i++)
        array[i] = Mathf.Sin(array[i]);
}

// Fast method (Unsafe + NativeArray)
unsafe void UnsafeProcess(NativeArray<float> array)
{
    float* ptr = (float*)array.GetUnsafePtr();
    for (int i = 0; i < array.Length; i++)
        ptr[i] = Mathf.Sin(ptr[i]);
}
```
Difference with 1 million elements: ~10-20 ms (managed) vs ~1-3 ms (unsafe + Burst). But for 100 elements, the difference is negligible.

---

## 🧹 Important: Memory Management
All Native containers must be manually disposed with `.Dispose()`, otherwise memory leaks occur. Use `using` or `try-finally` for safety:
```csharp
NativeArray<int> arr = new NativeArray<int>(100, Allocator.Temp);
try
{
    // work with arr
}
finally
{
    arr.Dispose();
}
```

Or use automatic `Dispose` with `DisposeHandle` in the Job System.

---

## 🧪 Summary
| Feature           | Safe Code         | Unsafe + NativeArray           |
| ---               | ---               | ---                            |
| Speed             | 🐢 Medium         | 🚀 Very high                  |
| Garbage Collector | 🟢 Safe           | 🔴 None, manual management    |
| Bounds checking   | ✅ Yes            | ❌ No (can go out of bounds)  |
| Complexity        | 📗 Low            | 📕 High                       |
| Where to use      | Almost everywhere | Only hot spots (5-10% of code) |

---

### ⭐ If this project was useful, put a star on GitHub!
