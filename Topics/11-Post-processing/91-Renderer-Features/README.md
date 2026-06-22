# 🎨 Renderer Features in URP: Custom Render Passes and Full-Screen Effects
Renderer Features in Universal Render Pipeline (URP) are a powerful mechanism for extending the rendering pipeline. 
They allow developers to insert custom render passes at specific points in the pipeline, creating custom post-effects, special object visualization, and more.

---

## 1. Core Concepts
| Concept | Description |
| --- | --- |
| Scriptable Renderer Feature | Component added to a Renderer that manages render passes. |
| Scriptable Render Pass | The actual render pass containing rendering logic. Executes at a specific point. |
| Injection Point | The insertion point of the pass in the rendering pipeline. |
| Full Screen Pass | Ready-made Renderer Feature for creating full-screen effects without code. |
| Render Graph | Modern resource management system for rendering in URP. |

### 📁 Creating a Renderer Feature
1. Create a C# script inheriting from `ScriptableRendererFeature`.
2. Implement `Create()` and `AddRenderPasses()` methods.
3. Create a Render Pass inheriting from `ScriptableRenderPass`.
4. Add the Feature to the URP Renderer Asset via Inspector.

---

## 2. Full Screen Pass Renderer Feature — Ready-Made Solution
Unity provides a ready-made Full Screen Pass Renderer Feature for creating full-screen effects without writing C# code.

### 🎯 Features:
| Feature | Description |
| --- | --- |
| Pass Material | Material with Fullscreen Shader Graph. |
| Injection Point | Insertion point: before/after transparents, before/after post-processing. |
| Requirements | Required passes: Depth, Normal, Color, Motion. |
| Pass Index | Pass index in shader (for complex effects). |

### 📝 Example: Creating a Grayscale Effect
Step 1: Create Fullscreen Shader Graph
1. In Project Window: Create → Shader Graph → URP → Fullscreen Shader Graph.
2. Add nodes:
   - URP Sample Buffer (Source Buffer → BlitSource).
   - Vector 3 with values (0.2126, 0.7152, 0.0722) — luminance coefficients.
   - Dot Product for calculating luminance.
  
3. Connect: Sample Buffer → Dot Product A, Vector 3 → Dot Product B, Dot Product → Fragment Base Color.
4. Create a Material and apply the Shader Graph.

Step 2: Add to Renderer
1. Select Universal Renderer Asset.
2. In Inspector: Add Renderer Feature → Full Screen Pass Renderer Feature.
3. Set:
   - Pass Material → created Material.
   - Injection Point → `After Rendering Post Processing`.
   - Requirements → `Color` (for access to _BlitTexture).
  
> 💡 Result: the scene becomes black and white!

---

## 3. Creating a Custom Scriptable Renderer Feature
If the ready-made Full Screen Pass isn't sufficient, you can create a fully custom Renderer Feature.

### 🏗️ Basic Structure:
```csharp
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MyRendererFeature : ScriptableRendererFeature
{
    public Material material;
    public float intensity = 1.0f;

    private MyRenderPass renderPass;

    public override void Create()
    {
        renderPass = new MyRenderPass(material);
        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox; // [citation:6]
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game) // [citation:6][citation:8]
        {
            renderer.EnqueuePass(renderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(material);
    }
}
```

### 🎮 Example: BlurRendererFeature with Render Graph
```csharp
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader shader;
    [SerializeField] private BlurSettings settings;

    private Material material;
    private BlurRenderPass blurRenderPass;

    [System.Serializable]
    public class BlurSettings
    {
        [Range(0, 0.4f)] public float horizontalBlur;
        [Range(0, 0.4f)] public float verticalBlur;
    }

    public override void Create()
    {
        if (shader == null) return;
        material = new Material(shader);
        blurRenderPass = new BlurRenderPass(material, settings);
        blurRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(blurRenderPass);
    }
}
```

### BlurRenderPass.cs (with Render Graph API):
```csharp
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class BlurRenderPass : ScriptableRenderPass
{
    private Material material;
    private BlurSettings defaultSettings;
    private static readonly int horizontalBlurId = Shader.PropertyToID("_HorizontalBlur");
    private static readonly int verticalBlurId = Shader.PropertyToID("_VerticalBlur");
    private const string k_BlurTextureName = "_BlurTexture";

    public BlurRenderPass(Material material, BlurSettings defaultSettings)
    {
        this.material = material;
        this.defaultSettings = defaultSettings;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) // [citation:5]
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        if (resourceData.isActiveTargetBackBuffer) return;

        TextureHandle srcCamColor = resourceData.activeColorTexture;
        RenderTextureDescriptor desc = srcCamColor.GetDescriptor(renderGraph);
        desc.name = k_BlurTextureName;
        desc.depthBufferBits = 0;
        TextureHandle dst = renderGraph.CreateTexture(desc);

        material.SetFloat(horizontalBlurId, defaultSettings.horizontalBlur);
        material.SetFloat(verticalBlurId, defaultSettings.verticalBlur);

        RenderGraphUtils.BlitMaterialParameters paraVertical = new(srcCamColor, dst, material, 0);
        renderGraph.AddBlitPass(paraVertical, "VerticalBlurRenderPass");

        RenderGraphUtils.BlitMaterialParameters paraHorizontal = new(dst, srcCamColor, material, 1);
        renderGraph.AddBlitPass(paraHorizontal, "HorizontalBlurRenderPass");
    }
}
```

---

## 4. Full-Screen Blit (Compatibility Mode)
For projects not using Render Graph (Compatibility Mode), use `CommandBuffer` and `Blitter.BlitCameraTexture`.

### 📱 Example: Tint Effect (Green Screen)
### ColorBlitPass.cs:
```csharp
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class ColorBlitPass : ScriptableRenderPass
{
    private ProfilingSampler profilingSampler = new ProfilingSampler("ColorBlit");
    private Material material;
    private RTHandle cameraColorTarget;
    private float intensity;

    public ColorBlitPass(Material material)
    {
        this.material = material;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing; // [citation:8]
    }

    public void SetTarget(RTHandle colorHandle, float intensity)
    {
        this.cameraColorTarget = colorHandle;
        this.intensity = intensity;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (material == null || renderingData.cameraData.cameraType != CameraType.Game) return;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, profilingSampler))
        {
            material.SetFloat("_Intensity", intensity);
            Blitter.BlitCameraTexture(cmd, cameraColorTarget, cameraColorTarget, material, 0); // [citation:8]
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
```

---

## 5. Injection Points
| Injection Point | Description |
| --- | --- |
| `BeforeRenderingTransparents` | After Skybox, before transparents |
| `BeforeRenderingPostProcessing` | After transparents, before post-processing |
| `AfterRenderingPostProcessing` | After standard post-processing |

---

## 6. Requirements
| Requirement | Description |
| --- | --- |
| `Depth` | Adds Depth Prepass for using depth |
| `Normal` | Provides access to normals |
| `Color` | Copies screen color to `_BlitTexture` |
| `Motion` | Provides motion vectors |
| `Everything` | Enables all of the above |

---

### ⭐ If this project was useful, put a star on GitHub!
