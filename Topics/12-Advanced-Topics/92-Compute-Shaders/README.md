# ⚡ Compute Shaders in Unity: Using GPU for Parallel Computing (Physics, Procedural Generation)
Compute Shaders are programs that run on the GPU (Graphics Processing Unit) and are designed for massive parallel computations. 
They can execute thousands of operations simultaneously, making them ideal for particle physics, 
procedural generation, image processing, and other computationally intensive tasks.

---

## 1. What is a Compute Shader and Why Use It?
### 🧠 CPU vs GPU
| Characteristic | CPU | GPU |
| --- | --- | --- |
| Number of cores | 4-32 | 1000-10000+ |
| Parallelism | Sequential | Massively parallel |
| Floating-point performance | ~100 GFLOPS | ~10-30 TFLOPS |
| Usage | Control, logic | Math, graphics |
| Best for | AI, UI, networking | Physics, particles, rendering |

### 🎯 When to Use Compute Shaders:
- ✅ Particle physics — thousands of objects with gravity and collisions
- ✅ Procedural generation — terrain, textures, meshes
- ✅ Image processing — post-effects, filters, recognition
- ✅ Fluid/cloth simulation — complex physical models
- ✅ AI/neural networks — matrix multiplication, forward/backward pass
- ✅ Data visualization — large data volumes

### ❌ Don't use for:
- UI and input logic
- Small number of objects (< 100)
- Operations with branching (if/else)

---

## 2. Compute Shader Structure
### 📁 Creating a Compute Shader:
```text
Assets/ → Create → Shader → Compute Shader
```

### 📄 Basic Structure:
```hlsl
// ==========================================
// 1. Version and directives
// ==========================================
#pragma kernel CSMain          // Main function (kernel)
#pragma kernel CSSecondary     // Secondary function

// ==========================================
// 2. Buffers (data)
// ==========================================
RWStructuredBuffer<float3> resultBuffer;   // Writable buffer
StructuredBuffer<float3> inputBuffer;      // Readable buffer
RWTexture2D<float4> outputTexture;         // Writable texture

// ==========================================
// 3. Constants (input parameters)
// ==========================================
int width;
int height;
float time;

// ==========================================
// 4. Main kernel function
// ==========================================
[numthreads(8, 8, 1)]          // Group size: 8x8x1 = 64 threads
void CSMain(uint3 id : SV_DispatchThreadID)
{
    // Get current thread index
    uint index = id.x + id.y * width;
    
    // Perform computation
    float3 result = inputBuffer[index] * 2.0f;
    
    // Write result
    resultBuffer[index] = result;
}
```

---

## 3. Key Compute Shader Concepts
### 🧵 Thread Groups
```text
Dispatch(10, 10, 1)           // Total groups: 10x10x1 = 100 groups
[numthreads(8, 8, 1)]         // Each group: 8x8x1 = 64 threads

Total threads: 100 * 64 = 6400
```

### 📐 Thread Indices:
| Index | Description |
| --- | --- |
| `SV_GroupID` | Group ID (0 to Dispatch-1) |
| `SV_GroupThreadID` | Thread ID within group (0 to numthreads-1) |
| `SV_DispatchThreadID` | Global thread ID = `GroupID * numthreads + GroupThreadID` |
| `SV_GroupIndex` | Thread index in group (0 .. numthreads-1) |

```hlsl
[numthreads(8, 8, 1)]
void CSMain(uint3 groupID : SV_GroupID,
            uint3 groupThreadID : SV_GroupThreadID,
            uint3 dispatchID : SV_DispatchThreadID,
            uint groupIndex : SV_GroupIndex)
{
    // dispatchID = groupID * 8 + groupThreadID
    int index = dispatchID.x + dispatchID.y * width;
}
```

---

## 4. Buffers in Compute Shaders
### 📦 Buffer Types:
| Type | Description | Use Case |
| --- | --- | --- |
| `RWStructuredBuffer<T>` | Read/write structured data | Particle positions |
| `StructuredBuffer<T>` | Read-only | Input data |
| `RWTexture2D<T>` | Read/write to texture | Texture generation |
| `Texture2D<T>` | Read-only texture | Input image |
| `RWByteAddressBuffer` | Raw byte read/write | Low-level data |

### 📝 Defining Structures:
```hlsl
// In Compute Shader
struct Particle
{
    float3 position;
    float3 velocity;
    float mass;
    float lifetime;
};

RWStructuredBuffer<Particle> particles;

[numthreads(64, 1, 1)]
void UpdateParticles(uint3 id : SV_DispatchThreadID)
{
    Particle p = particles[id.x];
    p.position += p.velocity * deltaTime;
    particles[id.x] = p;
}
```

```csharp
// In C# script
[System.Serializable]
public struct ParticleData
{
    public Vector3 position;
    public Vector3 velocity;
    public float mass;
    public float lifetime;
}

private ComputeBuffer particleBuffer;
private ParticleData[] particles;
```

---

## 5. Example 1: Procedural Texture Generation
### 📄 Compute Shader: `TextureGenerator.compute`

```hlsl
#pragma kernel CSMain

RWTexture2D<float4> result;
float time;
int textureWidth;
int textureHeight;

[numthreads(8, 8, 1)]
void CSMain(uint3 id : SV_DispatchThreadID)
{
    int x = id.x;
    int y = id.y;
    
    float2 uv = float2(x / (float)textureWidth, y / (float)textureHeight);
    
    float wave = sin(uv.x * 20 + time) * cos(uv.y * 20 + time * 0.7);
    float value = (wave + 1) * 0.5;
    
    float3 color = float3(value, value * 0.8, 1.0 - value);
    
    result[uint2(x, y)] = float4(color, 1.0);
}
```

### 🎮 C# Script:
```csharp
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    public ComputeShader computeShader;
    public Renderer targetRenderer;
    
    private RenderTexture renderTexture;
    private int kernelIndex;
    
    void Start()
    {
        renderTexture = new RenderTexture(512, 512, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        
        kernelIndex = computeShader.FindKernel("CSMain");
        
        computeShader.SetTexture(kernelIndex, "result", renderTexture);
        computeShader.SetInt("textureWidth", 512);
        computeShader.SetInt("textureHeight", 512);
        
        targetRenderer.material.mainTexture = renderTexture;
    }
    
    void Update()
    {
        computeShader.SetFloat("time", Time.time);
        
        int threadGroupsX = Mathf.CeilToInt(512 / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(512 / 8.0f);
        computeShader.Dispatch(kernelIndex, threadGroupsX, threadGroupsY, 1);
    }
    
    void OnDestroy()
    {
        renderTexture.Release();
    }
}
```

---

## 6. Example 2: GPU Particle Simulation
### 📄 Compute Shader: `ParticleSystem.compute`
```hlsl
#pragma kernel UpdateParticles
#pragma kernel RenderParticles

struct Particle
{
    float3 position;
    float3 velocity;
    float mass;
    float lifetime;
};

RWStructuredBuffer<Particle> particles;
RWStructuredBuffer<float3> particleColors;
StructuredBuffer<float3> initialPositions;

float deltaTime;
float time;
int particleCount;
float3 gravity;

[numthreads(64, 1, 1)]
void UpdateParticles(uint3 id : SV_DispatchThreadID)
{
    int index = id.x;
    if (index >= particleCount) return;
    
    Particle p = particles[index];
    
    p.velocity += gravity * deltaTime;
    p.position += p.velocity * deltaTime;
    p.lifetime -= deltaTime;
    
    if (p.lifetime <= 0)
    {
        p.position = initialPositions[index] + float3(0, 1, 0);
        p.velocity = float3(RandomRange(-1, 1), RandomRange(2, 5), RandomRange(-1, 1));
        p.lifetime = RandomRange(1, 3);
    }
    
    particles[index] = p;
}

float RandomRange(float min, float max)
{
    float r = frac(sin(dot(float2(index, time), float2(12.9898, 78.233))) * 43758.5453);
    return min + r * (max - min);
}
```

---

## 7. Example 3: Conway's Game of Life on GPU
### 📄 Compute Shader: `GameOfLife.compute`
```hlsl
#pragma kernel UpdateLife

RWTexture2D<float4> cells;
RWTexture2D<float4> nextCells;
int width;
int height;

[numthreads(8, 8, 1)]
void UpdateLife(uint3 id : SV_DispatchThreadID)
{
    int x = id.x;
    int y = id.y;
    
    if (x >= width || y >= height) return;
    
    int neighbors = 0;
    for (int dx = -1; dx <= 1; dx++)
    {
        for (int dy = -1; dy <= 1; dy++)
        {
            if (dx == 0 && dy == 0) continue;
            
            int nx = (x + dx + width) % width;
            int ny = (y + dy + height) % height;
            
            if (cells[uint2(nx, ny)].r > 0.5f)
                neighbors++;
        }
    }
    
    bool alive = cells[uint2(x, y)].r > 0.5f;
    bool nextAlive = false;
    
    if (alive && (neighbors == 2 || neighbors == 3))
        nextAlive = true;
    else if (!alive && neighbors == 3)
        nextAlive = true;
    else
        nextAlive = false;
    
    nextCells[uint2(x, y)] = float4(nextAlive ? 1.0f : 0.0f, 0, 0, 1);
}
```

---

## 8. Best Practices and Common Mistakes
### 🚀 Optimization Tips:
1. Group Size:
   - Use `[numthreads(64, 1, 1)]` for 1D data
   - Use `[numthreads(8, 8, 1)]` for 2D data
   - Always multiple of 4 (preferably 8, 16, 32, 64)
  
2. Avoid Branching:
   - All threads in a group execute synchronously
   - `if/else` with divergent paths hurts performance
   - Use ternary operators: `value = condition ? a : b;`
  
3. Minimize Data Transfers:
   - Transfer data to GPU once
   - Retrieve only when necessary
   - Use `ComputeBuffer` for large data
  
4. Synchronization:
   - Avoid `Atomic` operations inside shader
   - Use separate passes if synchronization is needed
  
5. Profiling:
   - Use Unity Profiler → GPU
   - Check thread count and execution time
  
### ⚠️ Common Mistakes:
```hlsl
// ❌ ERROR: Branching inside thread
if (someCondition)
{
    // Long operation
}
else
{
    // Another long operation
}

// ✅ CORRECT: Use ternary operator
float result = someCondition ? DoSomething() : DoSomethingElse();

// ❌ ERROR: Memory access without bounds checking
float3 pos = positions[index + 10000]; // Out of bounds

// ✅ CORRECT: Bounds checking
if (index < bufferSize)
{
    float3 pos = positions[index];
}

// ❌ ERROR: Too small group
[numthreads(1, 1, 1)] // 1 thread per group — inefficient

// ✅ CORRECT: Minimum 64 threads
[numthreads(64, 1, 1)]
```

---

### ⭐ If this project was useful, put a star on GitHub!
