# ⚡ Static Batching and GPU Instancing: Combining Meshes to Reduce Draw Calls
A Draw Call is a command from CPU to GPU to render one set of graphical primitives. 
Fewer draw calls means higher performance, especially on mobile devices. 
Unity provides two main mechanisms to reduce them: Static Batching and GPU Instancing.

---

## 1. Core Concepts
| Term | Description |
| --- | --- |
| Draw Call | One rendering command for one mesh with one material |
| Batch | A group of objects rendered in a single Draw Call |
| Static Batching | Combining static objects into one large mesh at build or load time |
| Dynamic Batching | Automatic combining of small moving objects (limited to 300 vertices) |
| GPU Instancing | Rendering many identical meshes with different parameters in one Draw Call |

### 📊 Method Comparison:
| Feature | Static Batching | GPU Instancing | Dynamic Batching |
| --- | --- | --- | --- |
| Objects can move | ❌ No | ✅ Yes | ✅ Yes |
| Vertex limit | ❌ No | ❌ No | ✅ < 300 vertices |
| Different materials | ❌ Same only | ❌ Same only | ❌ Same only |
| Memory (RAM/VRAM) | 🔺 High | 🔻 Low | 🔻 Low |
| Skinned mesh support | ❌ No | ❌ No | ❌ No |

---

## 2. Static Batching
Static Batching combines several static objects into one large mesh before the game starts. 
This is the most efficient method for static geometry (buildings, roads, terrain).

### 🛠️ How to Enable Static Batching:
1. Select an object in the scene
2. In the inspector, check the Static checkbox (or select Batching Static from the dropdown)
3. Unity will automatically combine all static objects with the same material

### 🧩 Code Setup:
```csharp
using UnityEngine;

public class StaticBatchingSetup : MonoBehaviour
{
    void Start()
    {
        // Mark object as static for batching
        gameObject.isStatic = true;
        
        // Force combine all static objects in the scene
        StaticBatching.Combine(gameObject.scene);
    }
}
```

### 📐 Example: City Building
```csharp
using UnityEngine;

public class CityBuilder : MonoBehaviour
{
    public GameObject buildingPrefab;
    public int gridSize = 10;
    
    void BuildCity()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = new Vector3(x * 10, 0, z * 10);
                GameObject building = Instantiate(buildingPrefab, position, Quaternion.identity);
                
                building.isStatic = true;
                building.tag = "Building";
            }
        }
        
        StaticBatching.Combine(gameObject.scene);
        Debug.Log($"Built {gridSize * gridSize} buildings. All will be combined into one Draw Call.");
    }
}
```

### ⚠️ Important Limitations:
```csharp
// ❌ Static Batching does NOT work if:
// 1. Objects use different materials
// 2. Objects move (transform changes)
// 3. Objects have different lightmap settings (Lightmap Index)
// 4. Objects use skinned animation (SkinnedMeshRenderer)

// ✅ Static Batching works if:
// 1. Objects are marked as Static
// 2. Objects use the SAME material
// 3. Objects don't move during gameplay
```

---

## 3. GPU Instancing
GPU Instancing allows rendering many identical meshes in one Draw Call, 
while each object can have its own parameters (color, position, scale). 
The magic happens on the GPU.

### 🛠️ How to Enable GPU Instancing:
1. Select a material
2. In the inspector, check Enable GPU Instancing
3. Use the same material for all objects

### 🎲 Example 1: Flower Field (GPU Instancing)
```csharp
using UnityEngine;

public class FlowerField : MonoBehaviour
{
    public Mesh flowerMesh;
    public Material flowerMaterial;
    public int flowerCount = 10000;
    public float fieldSize = 100f;
    
    private Matrix4x4[] matrices;
    private Vector4[] colors;
    private MaterialPropertyBlock propertyBlock;
    
    void Start()
    {
        if (!flowerMaterial.enableInstancing)
        {
            Debug.LogError("Enable GPU Instancing on the flower material!");
            return;
        }
        
        matrices = new Matrix4x4[flowerCount];
        colors = new Vector4[flowerCount];
        propertyBlock = new MaterialPropertyBlock();
        
        for (int i = 0; i < flowerCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-fieldSize, fieldSize),
                0,
                Random.Range(-fieldSize, fieldSize)
            );
            
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Vector3 scale = Vector3.one * Random.Range(0.8f, 1.2f);
            
            matrices[i] = Matrix4x4.TRS(position, rotation, scale);
            
            colors[i] = new Vector4(
                Random.Range(0.5f, 1f),
                Random.Range(0.2f, 0.8f),
                Random.Range(0.2f, 0.8f),
                1f
            );
        }
        
        Debug.Log($"Created {flowerCount} flowers with GPU Instancing → 1 Draw Call!");
    }
    
    void Update()
    {
        for (int i = 0; i < flowerCount; i += 1023)
        {
            int batchSize = Mathf.Min(1023, flowerCount - i);
            Matrix4x4[] batchMatrices = new Matrix4x4[batchSize];
            System.Array.Copy(matrices, i, batchMatrices, 0, batchSize);
            
            for (int j = 0; j < batchSize; j++)
            {
                propertyBlock.SetVector("_Color", colors[i + j]);
                Graphics.DrawMeshInstanced(flowerMesh, 0, flowerMaterial, batchMatrices, batchSize, propertyBlock);
            }
        }
    }
}
```

### 🔴 Example 2: Enemies with Different Colors (MaterialPropertyBlock)
```csharp
using UnityEngine;

public class EnemyInstancing : MonoBehaviour
{
    public Mesh enemyMesh;
    public Material enemyMaterial;
    public int enemyCount = 500;
    
    private Matrix4x4[] matrices;
    private MaterialPropertyBlock propertyBlock;
    private float[] healthPercent;
    private Vector3[] velocities;
    
    void Start()
    {
        if (!enemyMaterial.enableInstancing)
        {
            Debug.LogError("Enable GPU Instancing on the enemy material!");
            return;
        }
        
        matrices = new Matrix4x4[enemyCount];
        healthPercent = new float[enemyCount];
        velocities = new Vector3[enemyCount];
        propertyBlock = new MaterialPropertyBlock();
        
        for (int i = 0; i < enemyCount; i++)
        {
            matrices[i] = Matrix4x4.TRS(
                Random.insideUnitSphere * 20f,
                Random.rotation,
                Vector3.one
            );
            healthPercent[i] = Random.Range(0.2f, 1f);
            velocities[i] = Random.insideUnitSphere * 2f;
        }
    }
    
    void Update()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            velocities[i].y -= 9.81f * Time.deltaTime;
            Vector3 newPos = matrices[i].GetPosition() + velocities[i] * Time.deltaTime;
            
            if (newPos.y < -5f)
            {
                newPos.y = 5f;
                velocities[i].y = Mathf.Abs(velocities[i].y) * 0.8f;
            }
            
            matrices[i] = Matrix4x4.TRS(newPos, matrices[i].rotation, Vector3.one);
        }
        
        const int batchSize = 1023;
        int batches = Mathf.CeilToInt((float)enemyCount / batchSize);
        
        for (int b = 0; b < batches; b++)
        {
            int start = b * batchSize;
            int count = Mathf.Min(batchSize, enemyCount - start);
            
            Matrix4x4[] batchMatrices = new Matrix4x4[count];
            System.Array.Copy(matrices, start, batchMatrices, 0, count);
            
            propertyBlock.Clear();
            for (int i = 0; i < count; i++)
            {
                Color color = Color.Lerp(Color.red, Color.green, healthPercent[start + i]);
                propertyBlock.SetColor("_Color", color);
            }
            
            Graphics.DrawMeshInstanced(enemyMesh, 0, enemyMaterial, batchMatrices, count, propertyBlock);
        }
    }
}
```

### 🚀 Example 4: Performance Comparison
```csharp
using UnityEngine;
using UnityEngine.UI;

public class PerformanceComparison : MonoBehaviour
{
    public GameObject cubePrefab;
    public int objectCount = 1000;
    public Text fpsText;
    
    private GameObject[] objects;
    private float deltaTime = 0f;
    
    public enum RenderMode { Standard, StaticBatching, GPUInstancing }
    public RenderMode mode = RenderMode.Standard;
    
    void Start()
    {
        objects = new GameObject[objectCount];
        
        switch (mode)
        {
            case RenderMode.Standard:
                CreateStandard();
                break;
            case RenderMode.StaticBatching:
                CreateStaticBatched();
                break;
            case RenderMode.GPUInstancing:
                CreateGPUInstanced();
                break;
        }
    }
    
    void CreateStandard()
    {
        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = Instantiate(cubePrefab, Random.insideUnitSphere * 20f, Quaternion.identity);
            objects[i] = obj;
        }
        Debug.Log($"Standard: {objectCount} objects → {objectCount} Draw Calls");
    }
    
    void CreateStaticBatched()
    {
        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = Instantiate(cubePrefab, new Vector3(i % 100, 0, i / 100), Quaternion.identity);
            obj.isStatic = true;
            objects[i] = obj;
        }
        StaticBatching.Combine(gameObject.scene);
        Debug.Log($"Static Batching: {objectCount} objects → ~1 Draw Call (if same material)");
    }
    
    void CreateGPUInstanced()
    {
        Mesh mesh = cubePrefab.GetComponent<MeshFilter>().sharedMesh;
        Material mat = cubePrefab.GetComponent<Renderer>().sharedMaterial;
        mat.enableInstancing = true;
        
        Matrix4x4[] matrices = new Matrix4x4[objectCount];
        for (int i = 0; i < objectCount; i++)
        {
            matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * 20f, Random.rotation, Vector3.one);
        }
        
        Graphics.DrawMeshInstanced(mesh, 0, mat, matrices, objectCount);
        Debug.Log($"GPU Instancing: {objectCount} objects → 1 Draw Call");
    }
    
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"FPS: {fps:F1}\nDraw Calls: ~{GetApproximateDrawCalls()}\nMode: {mode}";
    }
    
    int GetApproximateDrawCalls()
    {
        #if UNITY_EDITOR
        return UnityEditor.UnityStats.drawCalls;
        #else
        return 0;
        #endif
    }
}
```

---

## 4. Method Selection Guide
### 📋 When to use what:
| Situation | Best Method | Why |
| --- | --- | --- |
| Static buildings, roads, terrain | Static Batching | Combine once and forget |
| Grass, trees, particles | GPU Instancing | They move but are identical |
| Enemy crowds with different colors | GPU Instancing + MPB | Different parameters, same mesh |
| Small moving objects (<300 vertices) | Dynamic Batching | Automatic (but unreliable) |
| Skinned animation objects | None ❌ | Neither batching nor instancing works |

### 🔬 Experiment: 10,000 Objects
```csharp
// Results on a typical mobile device:
// Standard: 10,000 draw calls → ~10 FPS (very bad)
// Static Batching: 1 draw call → ~60 FPS (great for static)
// GPU Instancing: 10 draw calls (10,000 / 1023) → ~55 FPS (good)
// Dynamic Batching: WON'T work (>300 vertices) → ~10 FPS
```

---

## 5. Diagnostics and Debugging
### 🛠️ Tools for Checking Batching:
1. Frame Debugger (Window → Analysis → Frame Debugger)
2. Stats Window (Game View → Stats)
3. Profiler (Window → Analysis → Profiler)
```csharp
using UnityEngine;
using UnityEngine.UI;

public class BatchingDebugger : MonoBehaviour
{
    public Text debugText;
    
    void Update()
    {
        #if UNITY_EDITOR
        int batches = UnityEditor.UnityStats.batches;
        int drawCalls = UnityEditor.UnityStats.drawCalls;
        int dynamicBatched = UnityEditor.UnityStats.dynamicBatches;
        int staticBatched = UnityEditor.UnityStats.staticBatches;
        int instanced = UnityEditor.UnityStats.instancedBatches;
        
        debugText.text = $"Batches: {batches}\n" +
                        $"Draw Calls: {drawCalls}\n" +
                        $"Dynamic: {dynamicBatched}\n" +
                        $"Static: {staticBatched}\n" +
                        $"Instanced: {instanced}";
        #endif
    }
}
```

---

## 6. Best Practices
### ✅ Recommendations:
1. Always enable Static Batching for static geometry
2. Use GPU Instancing for repeating objects (coins, enemies, bullets)
3. Combine textures into an atlas to use a single material
4. Limit the number of materials — fewer materials = fewer draw calls
5. On mobile devices, disable Dynamic Batching — it consumes CPU

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Moving object marked as Static
movingObject.isStatic = true; // This breaks batching and will rebuild the mesh every frame!

// ✅ CORRECT: Only immovable objects
building.isStatic = true; // Building doesn't move
movingObject.isStatic = false;

// ❌ ERROR: Different materials on same mesh objects
object1.GetComponent<Renderer>().material = materialRed;
object2.GetComponent<Renderer>().material = materialBlue;
// → 2 draw calls, even with GPU Instancing

// ✅ CORRECT: One material + MaterialPropertyBlock
Material sharedMaterial = materialBase;
MaterialPropertyBlock props = new MaterialPropertyBlock();
props.SetColor("_Color", Color.red);
Graphics.DrawMeshInstanced(mesh, 0, sharedMaterial, matrices, count, props);
```

---

### ⭐ If this project was useful, put a star on GitHub!
