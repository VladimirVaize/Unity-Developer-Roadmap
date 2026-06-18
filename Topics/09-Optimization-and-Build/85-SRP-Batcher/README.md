# ⚡ SRP Batcher: Batching Mechanism for Scriptable Render Pipelines, Difference from Standard Batching
SRP Batcher is a rendering optimization system built into Scriptable Render Pipelines (URP and HDRP). 
It significantly speeds up rendering by optimizing material property updates, especially in scenes with many objects using the same shader but different materials.

---

## 1. What is Batching in Rendering?
Batching is combining multiple draw calls into one. This reduces CPU load and increases performance.

### 📊 Comparison: With vs Without Batching:
```text
WITHOUT BATCHING:        WITH BATCHING:
Object 1 → Draw Call    Object 1 ─┐
Object 2 → Draw Call    Object 2 ─┼→ Draw Call
Object 3 → Draw Call    Object 3 ─┘
Object 4 → Draw Call    Object 4 ─┐
Object 5 → Draw Call    Object 5 ─┼→ Draw Call
...                      Object 6 ─┘
(100+ Draw Calls)        (10 Draw Calls)
```
Goal: Minimize Draw Calls to increase FPS.

---

## 2. Standard Batching in Unity (Built-in Render Pipeline)
Built-in RP has two types of batching:

### 🔄 2.1 Dynamic Batching
| Characteristic | Description |
| --- | --- |
| Principle | Combines small objects (< 300 vertices) with same material |
| How it works | Transforms vertices to world space on CPU |
| Limitations | Only < 300 vertices, one material, no skeletal animation |
| Pros | Easy to use, automatic | 
| Cons | CPU-heavy, inefficient for complex scenes |

```text
// Enable in Built-in RP:
// Edit → Project Settings → Player → Other Settings → Dynamic Batching ✅
```

### 🗂️ 2.2 Static Batching
| Characteristic | Description | 
| --- | --- |
| Principle | Combines static (non-moving) objects into one mesh |
| How it works | Merges meshes at build time or in the editor |
| Limitations | Only static objects, requires memory for combined mesh |
| Pros | Very efficient for static scenes (cities, environments) |
| Cons | Increases memory usage, doesn't work with moving objects |

```text
// Enable in Built-in RP:
// Object → Static flag ✅ (Lightmap Static + Batching Static)
```

---

## 3. SRP Batcher — The New Approach for URP and HDRP
SRP Batcher is an evolution of batching, created specifically for Scriptable Render Pipelines (URP, HDRP). 
It solves the main problem of standard batching: inefficient material property updates.

### 🧠 Key Idea:
In standard batching, each object with a different material requires a separate draw call, even if they have the same shader. 
SRP Batcher separates object data (transformation matrices) from material data (shader properties).

### 📐 SRP Batcher Architecture:
```text
┌──────────────────────────────────────────────────────────┐
│                   SRP Batcher                            │
├──────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐    │
│  │  Per-Object  │  │  Per-Object  │  │  Per-Object  │    │
│  │    Data      │  │    Data      │  │    Data      │    │
│  │ (Transform,  │  │ (Transform,  │  │ (Transform,  │    │
│  │  LightmapUV) │  │  LightmapUV) │  │  LightmapUV) │    │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘    │
│         │                │                │             │
│         └────────────────┼────────────────┘             │
│                          │                              │
│  ┌───────────────────────▼──────────────────────────┐  │
│  │           Shared Material Data                   │  │
│  │      (Shader Properties, Textures)              │  │
│  └───────────────────────┬──────────────────────────┘  │
│                          │                              │
│  ┌───────────────────────▼──────────────────────────┐  │
│  │               GPU Buffer                         │  │
│  │   (Uploaded once per frame, reused)             │  │
│  └──────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────┘
```

### ⚡ SRP Batcher Advantages:
| Advantage | Description |
| --- | --- |
| Fewer Draw Calls | Objects with the same shader are grouped into one Draw Call |
| Less CPU Load | Material data is updated only when changed |
| Better Performance | Especially noticeable on mobile devices |
| Support for Complex Scenes | Thousands of objects with different materials |
| Automatic | Enabled with one flag |

### 🚫 SRP Batcher Limitations:
| Limitation | Why |
| --- | --- |
| URP/HDRP Only | Doesn't work in Built-in RP |
| Only Specific Shaders | Shaders must be SRP Batcher compatible (Shader Graph, Lit, Unlit) |
| Not Compatible with MaterialPropertyBlocks | Use `[PerRendererData]` instead |
| Limited Number of Properties | Up to 32 properties per material |

---

## 4. How SRP Batcher Works (Under the Hood)
### 🔍 Step-by-Step Process:
1. Frame Assembly:
   - Identify all objects to render
   - Identify all objects to render
  
2. Data Preparation:
   - Per-Object Data: Transformation matrices, Lightmap UV, colors
   - Shared Data: Textures, shader property values
  
3. GPU Upload:
   - Per-Object data uploaded to GPU Buffer (CBUFFER)
   - Shared data uploaded separately
  
4. Rendering:
   - For each group with the same material:
     - Set Shared Data (once)
     - Loop through objects, update only Per-Object Data
     - One Draw Call per group
    
### 💡 Pseudo-code Example:
```csharp
// WITHOUT SRP Batcher (standard approach)
foreach (var obj in allObjects)
{
    SetMaterial(obj.material);        // Slow: updating many properties
    SetTransform(obj.transform);      // Slow: repeated every time
    Draw(obj);                        // Draw Call
}

// WITH SRP Batcher (optimized)
foreach (var materialGroup in groupedByMaterial)
{
    SetSharedMaterialData(materialGroup.material);  // Once!
    foreach (var obj in materialGroup.objects)
    {
        SetPerObjectData(obj.transform);           // Fast: only transform
        Draw(obj);                                  // One Draw Call per group
    }
}
```

---

## 5. Comparison: Standard Batching vs SRP Batcher
| Feature | Dynamic Batching | Static Batching | SRP Batcher |
| --- | --- | --- | --- |
| Supported RP | Built-in | Built-in | URP / HDRP |
| Moving Objects Support | ✅ Yes | ❌ No | ✅ Yes |
| Vertex Limit | < 300 | None | None |
| CPU Load | High | Low | Low |
| GPU Load | Low | Medium | Low |
| Property Update | Full | Full | Per-Object Only |
| Different Materials Support | ❌ No | ❌ No | ✅ Yes (if same shader) |
| Automatic Enable | ✅ Yes | ❌ Manual | ✅ Yes (one flag) |
| Memory | Low | High | Medium |

---

## 6. Configuring SRP Batcher
### 🎯 Enabling SRP Batcher:
1. URP:
   - Create/open Universal Render Pipeline Asset
   - In the inspector, find SRP Batcher → enable ✅
  
2. HDRP:
   - Create/open HDRenderPipelineAsset
   - In the inspector: Rendering → SRP Batcher → enable ✅
  
```csharp
// Code control (URP)
using UnityEngine.Rendering.Universal;

public class SRPBatcherController : MonoBehaviour
{
    public UniversalRenderPipelineAsset urpAsset;
    
    void Start()
    {
        if (urpAsset != null)
        {
            urpAsset.useSRPBatcher = true;
            Debug.Log("SRP Batcher enabled");
        }
    }
}
```

### 🔍 Verifying SRP Batcher:
1. Open Frame Debugger (Window → Analysis → Frame Debugger)
2. Select a frame
3. Find the SRP Batcher section
4. Check Draw Call and group counts

```csharp
// Statistics in code (for debugging)
using UnityEngine.Rendering;

public class SRPStats : MonoBehaviour
{
    void Update()
    {
        if (UnityEngine.Rendering.RenderPipelineManager.currentPipeline != null)
        {
            Debug.Log($"SRP Batcher active: {GraphicsSettings.useScriptableRenderPipelineBatching}");
        }
    }
}
```

---

## 7. Shader Compatibility with SRP Batcher
### ✅ Compatible Shaders:
| Shader Type | Compatibility |
| --- | --- |
| Shader Graph (URP) | ✅ Full compatibility |
| Lit / Unlit (URP) | ✅ Full compatibility |
| Complex Lit / Lit (HDRP) | ✅ Full compatibility |
| Custom SRP Shaders | ✅ If using CBUFFER correctly |

### ❌ Incompatible Shaders:
| Shader Type | Reason |
| --- | --- |
| Built-in shaders | Don't support SRP Batcher |
| Shaders with MaterialPropertyBlock | Not compatible (use `[PerRendererData]`) |
| Shaders with `[MaterialToggle]` | May cause errors |

### 📝 Example Custom Shader for SRP Batcher:
```h1s1
Shader "Custom/SRPBatcherShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID  // IMPORTANT: for SRP Batcher
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID  // IMPORTANT: for SRP Batcher
            };
            
            // SRP Batcher uses CBUFFER for Per-Object data
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);      // SRP Batcher
                UNITY_TRANSFER_INSTANCE_ID(input, output); // SRP Batcher
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input); // SRP Batcher
                half4 color = _Color;
                // ... rendering
                return color;
            }
            ENDHLSL
        }
    }
}
```

---

## 8. Optimization Examples with SRP Batcher
### 🏙️ Example 1: Scene with 1000 Trees (Different Colors)
```csharp
public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab;
    public Material treeMaterial;
    public int count = 1000;
    
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject tree = Instantiate(treePrefab);
            tree.transform.position = Random.insideUnitSphere * 100f;
            
            Material mat = new Material(treeMaterial);
            mat.color = Random.ColorHSV();
            tree.GetComponent<Renderer>().material = mat;
        }
        
        // With SRP Batcher enabled:
        // Draw Calls ≈ number of material groups (not objects!)
        // If colors are unique → each object = separate group
        // Recommendation: use a limited set of colors
    }
}
```

### 🎨 Example 2: Using [PerRendererData] Instead of MaterialPropertyBlock
```csharp
// ❌ NOT COMPATIBLE WITH SRP BATCHER
public class BadPerObjectColor : MonoBehaviour
{
    private MaterialPropertyBlock propertyBlock;
    
    void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_Color", Random.ColorHSV());
        GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }
}

// ✅ COMPATIBLE WITH SRP BATCHER
public class GoodPerObjectColor : MonoBehaviour
{
    void Start()
    {
        // Use [PerRendererData] in shader
        GetComponent<Renderer>().material.SetColor("_Color", Random.ColorHSV());
    }
}
```

### 🔄 Example 3: Performance Comparison
```csharp
using UnityEngine;
using UnityEngine.Rendering;

public class PerformanceTest : MonoBehaviour
{
    public int objectCount = 5000;
    public GameObject prefab;
    
    void Start()
    {
        if (GraphicsSettings.useScriptableRenderPipelineBatching)
        {
            Debug.Log("SRP Batcher is active! 🚀");
        }
        else
        {
            Debug.LogWarning("SRP Batcher is NOT active! Check settings.");
        }
        
        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.position = Random.insideUnitSphere * 200f;
            
            var renderer = obj.GetComponent<Renderer>();
            renderer.material.color = new Color(
                Random.Range(0.5f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(0.5f, 1f)
            );
        }
        
        // Use Profiler and Frame Debugger for comparison
    }
}
```

---

## 9. Best Practices
### ✅ Recommendations:
1. Always enable SRP Batcher in URP/HDRP projects
2. Use the same shader for similar objects
3. Limit the number of unique materials (group them)
4. Use `[PerRendererData]` instead of MaterialPropertyBlock
5. Optimize shader property count (< 32)
6. Check Frame Debugger for batching analysis

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Using MaterialPropertyBlock
// Breaks batching because each object becomes unique
var block = new MaterialPropertyBlock();
block.SetColor("_Color", color);
renderer.SetPropertyBlock(block);

// ✅ CORRECT: Use PerRendererData in shader
// And set color directly on the material
renderer.material.SetColor("_Color", color);

// ❌ ERROR: Creating many materials
for (int i = 0; i < 1000; i++)
{
    material = new Material(baseMaterial); // 1000 materials!
}

// ✅ CORRECT: Use material properties for variations
// (SRP Batcher separates Per-Object and Shared data)
renderer.material.SetColor("_Color", color);
```

---

### ⭐ If this project was useful, put a star on GitHub!
