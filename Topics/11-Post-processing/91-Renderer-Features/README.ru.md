# 🎨 Renderer Features в URP: Создание кастомных проходов рендеринга и full-screen эффектов
Renderer Features в Universal Render Pipeline (URP) — это мощный механизм для расширения конвейера рендеринга. 
Он позволяет разработчикам вставлять собственные проходы рендеринга (Render Passes) в определённые точки пайплайна, 
создавая кастомные пост-эффекты, специальную визуализацию объектов и многое другое.

---

## 1. Основные понятия
| Понятие | Описание |
| --- | --- |
| Scriptable Renderer Feature | Компонент, который добавляется к Renderer'у и управляет проходами рендеринга. |
| Scriptable Render Pass | Непосредственно проход рендеринга, содержащий логику отрисовки. Выполняется в определённый момент. |
| Injection Point | Точка вставки прохода в конвейер рендеринга. |
| Full Screen Pass | Готовый тип Renderer Feature для создания полноэкранных эффектов без написания кода. |
| Render Graph | Современная система управления ресурсами рендеринга в URP. |

### 📁 Создание Renderer Feature
1. Создайте C# скрипт, наследующий от `ScriptableRendererFeature`.
2. Реализуйте методы `Create()` и `AddRenderPasses()`.
3. Создайте Render Pass, наследующий от `ScriptableRenderPass`.
4. Добавьте Feature в URP Renderer Asset через Inspector.

---

## 2. Full Screen Pass Renderer Feature — готовое решение для пост-эффектов
Unity предоставляет готовый Full Screen Pass Renderer Feature, который позволяет создавать полноэкранные эффекты без написания C# кода.

### 🎯 Особенности:
| Особенность | Описание |
| --- | --- |
| Pass Material | Материал с Fullscreen Shader Graph. |
| Injection Point | Точка вставки: до/после прозрачных объектов, до/после пост-обработки. |
| Requirements | Требуемые проходы: Depth, Normal, Color, Motion. |
| Pass Index | Индекс прохода в шейдере (для сложных эффектов). |

### 📝 Пример: Создание Grayscale эффекта
Шаг 1: Создание Fullscreen Shader Graph
1. В Project Window: Create → Shader Graph → URP → Fullscreen Shader Graph.
2. Добавьте узлы:
   - URP Sample Buffer (Source Buffer → BlitSource).
   - Vector 3 со значениями (0.2126, 0.7152, 0.0722) — коэффициенты яркости.
   - Dot Product для вычисления яркости.
  
3. Соедините: Sample Buffer → Dot Product A, Vector 3 → Dot Product B, Dot Product → Fragment Base Color.
4. Создайте Material и примените Shader Graph.

Шаг 2: Добавление в Renderer
1. Выберите Universal Renderer Asset.
2. В Inspector: Add Renderer Feature → Full Screen Pass Renderer Feature.
3. Установите:
   - Pass Material → созданный Material.
   - Injection Point → `After Rendering Post Processing`.
   - Requirements → `Color` (для доступа к _BlitTexture).
  
> 💡 Результат: сцена становится чёрно-белой!

---

## 3. Создание кастомного Scriptable Renderer Feature
Если готового Full Screen Pass недостаточно, можно создать полностью кастомный Renderer Feature.

### 🏗️ Базовая структура:
```csharp
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MyRendererFeature : ScriptableRendererFeature
{
    // Поля для настройки в Inspector
    public Material material;
    public float intensity = 1.0f;

    // Ссылка на Render Pass
    private MyRenderPass renderPass;

    // Вызывается при загрузке или изменении свойств [citation:5][citation:6]
    public override void Create()
    {
        renderPass = new MyRenderPass(material);
        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox; // [citation:6]
    }

    // Вызывается каждый кадр для каждого камера [citation:5][citation:6]
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Проверяем, что это игровая камера [citation:6][citation:8]
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(renderPass);
        }
    }

    // Очистка ресурсов
    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(material);
    }
}
```

### 🎮 Пример: BlurRendererFeature с Render Graph
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

### BlurRenderPass.cs (с Render Graph API):
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

        // Создаём временный RenderTexture
        RenderTextureDescriptor desc = srcCamColor.GetDescriptor(renderGraph);
        desc.name = k_BlurTextureName;
        desc.depthBufferBits = 0;
        TextureHandle dst = renderGraph.CreateTexture(desc);

        // Обновляем параметры шейдера
        material.SetFloat(horizontalBlurId, defaultSettings.horizontalBlur);
        material.SetFloat(verticalBlurId, defaultSettings.verticalBlur);

        // Добавляем проходы для вертикального и горизонтального блюра [citation:5]
        RenderGraphUtils.BlitMaterialParameters paraVertical = new(srcCamColor, dst, material, 0);
        renderGraph.AddBlitPass(paraVertical, "VerticalBlurRenderPass");

        RenderGraphUtils.BlitMaterialParameters paraHorizontal = new(dst, srcCamColor, material, 1);
        renderGraph.AddBlitPass(paraHorizontal, "HorizontalBlurRenderPass");
    }
}
```

---

## 4. Full-Screen Blit (Совместимый режим)
Для проектов, где Render Graph не используется (Compatibility Mode), применяется подход с `CommandBuffer` и `Blitter.BlitCameraTexture`.

### 📱 Пример: Tint Effect (Green Screen)
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
            // Используйте Blitter.BlitCameraTexture вместо cmd.Blit [citation:8]
            Blitter.BlitCameraTexture(cmd, cameraColorTarget, cameraColorTarget, material, 0);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
```

### ColorBlitRendererFeature.cs:
```csharp
internal class ColorBlitRendererFeature : ScriptableRendererFeature
{
    public Shader shader;
    public float intensity = 1.5f;

    private Material material;
    private ColorBlitPass renderPass;

    public override void Create()
    {
        material = CoreUtils.CreateEngineMaterial(shader);
        renderPass = new ColorBlitPass(material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
            renderer.EnqueuePass(renderPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            renderPass.SetTarget(renderer.cameraColorTargetHandle, intensity);
        }
    }
}
```

### ColorBlit.shader:
```h1s1
Shader "ColorBlit"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "ColorBlitPass"
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            float _Intensity;

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float4 color = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, input.texcoord);
                return color * float4(0, _Intensity, 0, 1);
            }
            ENDHLSL
        }
    }
}
```

---

## 5. Точки вставки (Injection Points)
| Injection Point | Описание |
| --- | --- |
| `BeforeRenderingTransparents` | После Skybox, до прозрачных объектов |
| `BeforeRenderingPostProcessing` | После прозрачных, до пост-обработки |
| `AfterRenderingPostProcessing` | После стандартной пост-обработки |

Важно: Выбирайте Injection Point в зависимости от того, когда должен применяться эффект.

---

## 6. Requirements (Требования)
| Requirement | Описание |
| --- | --- |
| `Depth` | Добавляет Depth Prepass для использования глубины |
| `Normal` | Обеспечивает доступ к нормалям |
| `Color` | Копирует цвет экрана в `_BlitTexture` |
| `Motion` | Обеспечивает векторы движения |
| `Everything` | Включает все вышеперечисленное |

---

## 7. Лучшие практики
### ✅ Рекомендации:
1. Используйте Render Graph API для новых проектов.
2. Оптимизируйте шейдеры — избегайте сложных вычислений в каждом пикселе.
3. Управляйте ресурсами через `Dispose()`.
4. Проверяйте `CameraType` перед вставкой прохода.
5. Используйте `ProfilingSampler` для дебага производительности.
6. Для XR проектов используйте `Blitter` вместо `cmd.Blit`.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
