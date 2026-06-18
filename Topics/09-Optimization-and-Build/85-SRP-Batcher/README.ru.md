# ⚡ SRP Batcher: Механизм батчинга для Scriptable Render Pipelines, отличие от стандартного батчинга
SRP Batcher — это система оптимизации рендеринга, встроенная в Scriptable Render Pipelines (URP и HDRP). 
Она значительно ускоряет рендеринг за счёт оптимизации обновления свойств материалов, особенно в сценах с большим количеством объектов, использующих одинаковые шейдеры, но разные материалы.

---

## 1. Что такое батчинг (Batching) в рендеринге?
Батчинг — это объединение нескольких вызовов рендеринга (Draw Calls) в один. 
Это снижает нагрузку на CPU и увеличивает производительность.

### 📊 Сравнение с и без батчинга:
```text
БЕЗ БАТЧИНГА:           С БАТЧИНГОМ:
Object 1 → Draw Call    Object 1 ─┐
Object 2 → Draw Call    Object 2 ─┼→ Draw Call
Object 3 → Draw Call    Object 3 ─┘
Object 4 → Draw Call    Object 4 ─┐
Object 5 → Draw Call    Object 5 ─┼→ Draw Call
...                     Object 6 ─┘
(100+ Draw Calls)       (10 Draw Calls)
```

Цель: Минимизировать количество Draw Calls для повышения FPS.

---

## 2. Стандартный батчинг в Unity (Built-in Render Pipeline)
Встроенный рендеринг (Built-in RP) имеет два типа батчинга:

### 🔄 2.1 Dynamic Batching (Динамический батчинг)
| Характеристика | Описание |
| --- | --- |
| Принцип | Объединяет мелкие объекты (вершин < 300) с одинаковым материалом |
| Как работает | Преобразует вершины в мировое пространство на CPU |
| Ограничения | Только объекты < 300 вершин, один материал, без скелетной анимации |
| Плюсы | Простота использования, автоматически |
| Минусы | Нагружает CPU, неэффективен для сложных сцен |

```text
// Включение в Built-in RP:
// Edit → Project Settings → Player → Other Settings → Dynamic Batching ✅
```

### 🗂️ 2.2 Static Batching (Статический батчинг)
| Характеристика | Описание |
| --- | --- |
| Принцип | Объединяет статичные объекты (не движущиеся) в один меш |
| Как работает | Объединяет меши на этапе сборки или в редакторе |
| Ограничения | Только статичные объекты, требует памяти для объединённого меша |
| Плюсы | Очень эффективен для статичных сцен (города, окружение) |
| Минусы | Увеличивает использование памяти, не работает с движущимися объектами |

```text
// Включение в Built-in RP:
// Объект → Static флаг ✅ (Lightmap Static + Batching Static)
```

---

## 3. SRP Batcher — новый подход для URP и HDRP
SRP Batcher — это эволюция батчинга, созданная специально для Scriptable Render Pipelines (URP, HDRP). 
Она решает главную проблему стандартного батчинга: неэффективное обновление свойств материалов.

### 🧠 Ключевая идея:
В стандартном батчинге каждый объект с разными материалами требует отдельного Draw Call, даже если у них одинаковый шейдер. 
SRP Batcher разделяет данные объекта (матрицы трансформации) и данные материала (свойства шейдера).

### 📐 Архитектура SRP Batcher:
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

### ⚡ Преимущества SRP Batcher:
| Преимущество | Описание |
| --- | --- |
| Меньше Draw Calls | Объекты с одинаковым шейдером группируются в один Draw Call |
| Меньше CPU нагрузки | Данные материалов обновляются только при изменении |
| Лучшая производительность | Особенно заметно на мобильных устройствах |
| Поддержка сложных сцен | Тысячи объектов с разными материалами |
| Автоматическая работа | Включается одним флажком |

### 🚫 Ограничения SRP Batcher:
| Ограничение | Почему |
| --- | --- |
| Только URP/HDRP | Не работает в Built-in RP |
| Только определённые шейдеры | Шейдеры должны быть совместимы с SRP Batcher (Shader Graph, Lit, Unlit) |
| Не совместим с MaterialPropertyBlocks | Используйте `[PerRendererData]` вместо |
| Ограниченное количество свойств | До 32 свойств на материал |

---

## 4. Как работает SRP Batcher (под капотом)
### 🔍 Пошаговый процесс:
1. Сборка кадра (Frame Assembly):
   - Определение всех объектов для рендеринга
   - Группировка по Shader и Material
  
2. Подготовка данных (Data Preparation):
   - Per-Object Data: Матрицы трансформации, Lightmap UV, цвета
   - Shared Data: Текстуры, значения свойств шейдера
  
3. Загрузка в GPU (GPU Upload):
   - Per-Object данные загружаются в GPU Buffer (CBUFFER)
   - Shared данные загружаются отдельно
  
4. Рендеринг (Rendering):
   - Для каждой группы с одинаковым материалом:
     - Установка Shared Data (один раз)
     - Цикл по объектам, обновление только Per-Object Data
     - Один Draw Call на группу
    
### 💡 Пример на псевдокоде:
```csharp
// БЕЗ SRP Batcher (стандартный подход)
foreach (var obj in allObjects)
{
    SetMaterial(obj.material);        // Медленно: обновление многих свойств
    SetTransform(obj.transform);      // Медленно: каждый раз заново
    Draw(obj);                        // Draw Call
}

// С SRP Batcher (оптимизированный)
foreach (var materialGroup in groupedByMaterial)
{
    SetSharedMaterialData(materialGroup.material);  // Один раз!
    foreach (var obj in materialGroup.objects)
    {
        SetPerObjectData(obj.transform);           // Быстро: только трансформ
        Draw(obj);                                  // Один Draw Call на группу
    }
}
```

---

## 5. Сравнение: Standard Batching vs SRP Batcher
| Характеристика | Dynamic Batching | Static Batching | SRP Batcher |
| --- | --- | --- | --- |
| Поддерживаемые RP | Built-in | Built-in | URP / HDRP |
| Поддержка движущихся объектов | ✅ Да | ❌ Нет | ✅ Да |
| Ограничение по вершинам | < 300 | Нет | Нет |
| CPU нагрузка | Высокая | Низкая | Низкая |
| GPU нагрузка | Низкая | Средняя | Низкая |
| Обновление свойств | Полное | Полное | Только Per-Object |
| Поддержка разных материалов | ❌ Нет | ❌ Нет | ✅ Да (если одинаковый шейдер) |
| Автоматическое включение | ✅ Да | ❌ Вручную | ✅ Да (один флаг) |
| Память | Низкая | Высокая | Средняя |

---

## 6. Настройка SRP Batcher
### 🎯 Включение SRP Batcher:
1. URP:
   - Создать/открыть Universal Render Pipeline Asset
   - В инспекторе найти SRP Batcher → включить ✅
  
2. HDRP:
   - Создать/открыть HDRenderPipelineAsset
   - В инспекторе: Rendering → SRP Batcher → включить ✅
  
```csharp
// Управление из кода (URP)
using UnityEngine.Rendering.Universal;

public class SRPBatcherController : MonoBehaviour
{
    public UniversalRenderPipelineAsset urpAsset;
    
    void Start()
    {
        if (urpAsset != null)
        {
            urpAsset.useSRPBatcher = true;
            Debug.Log("SRP Batcher включен");
        }
    }
}
```

### 🔍 Проверка работы SRP Batcher:
1. Открыть Frame Debugger (Window → Analysis → Frame Debugger)
2. Выбрать кадр
3. Найти секцию SRP Batcher
4. Смотреть количество Draw Calls и групп

```csharp
// Статистика в коде (для отладки)
using UnityEngine.Rendering;

public class SRPStats : MonoBehaviour
{
    void Update()
    {
        if (UnityEngine.Rendering.RenderPipelineManager.currentPipeline != null)
        {
            // SRP Batcher статистика доступна через RenderPipeline
            // В URP можно использовать UniversalRenderPipeline
            Debug.Log($"SRP Batcher активен: {GraphicsSettings.useScriptableRenderPipelineBatching}");
        }
    }
}
```

---

## 7. Совместимость шейдеров с SRP Batcher
### ✅ Совместимые шейдеры:
| Тип шейдера | Совместимость |
| --- | --- |
| Shader Graph (URP) | ✅ Полная совместимость |
| Lit / Unlit (URP) | ✅ Полная совместимость |
| Complex Lit / Lit (HDRP) | ✅ Полная совместимость |
| Кастомные шейдеры (SRP) | ✅ Если использовать `CBUFFER` правильно |

### ❌ Несовместимые шейдеры:
| Тип шейдера | Причина |
| --- | --- |
| Built-in шейдеры | Не поддерживают SRP Batcher |
| Шейдеры с MaterialPropertyBlock | Не совместимы (используйте `[PerRendererData]`) |
| Шейдеры с `[MaterialToggle]` | Могут вызвать ошибки |

### 📝 Пример кастомного шейдера для SRP Batcher:
```hlsl
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
                UNITY_VERTEX_INPUT_INSTANCE_ID  // ВАЖНО: для SRP Batcher
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID  // ВАЖНО: для SRP Batcher
            };
            
            // SRP Batcher использует CBUFFER для Per-Object данных
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
                // ... рендеринг
                return color;
            }
            ENDHLSL
        }
    }
}
```

---

## 8. Примеры оптимизации с SRP Batcher
### 🏙️ Пример 1: Сцена с 1000 деревьями (разные цвета)
```csharp
public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrefab;
    public Material treeMaterial;
    public int count = 1000;
    
    void Start()
    {
        // Каждое дерево имеет свой цвет
        for (int i = 0; i < count; i++)
        {
            GameObject tree = Instantiate(treePrefab);
            tree.transform.position = Random.insideUnitSphere * 100f;
            
            // Создаём экземпляр материала (SRP Batcher оптимизирует)
            Material mat = new Material(treeMaterial);
            mat.color = Random.ColorHSV();
            tree.GetComponent<Renderer>().material = mat;
        }
        
        // При включённом SRP Batcher:
        // Draw Calls ≈ количество групп материалов (не объектов!)
        // Если цвета уникальны → каждый объект = отдельная группа
        // Рекомендация: использовать ограниченный набор цветов
    }
}
```

### 🎨 Пример 2: Использование [PerRendererData] вместо MaterialPropertyBlock
```csharp
// ❌ НЕ СОВМЕСТИМО С SRP BATCHER
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

// ✅ СОВМЕСТИМО С SRP BATCHER
public class GoodPerObjectColor : MonoBehaviour
{
    void Start()
    {
        // Используем [PerRendererData] в шейдере
        GetComponent<Renderer>().material.SetColor("_Color", Random.ColorHSV());
    }
}
```

### 🔄 Пример 3: Сравнение производительности
```csharp
using UnityEngine;
using UnityEngine.Rendering;

public class PerformanceTest : MonoBehaviour
{
    public int objectCount = 5000;
    public GameObject prefab;
    
    void Start()
    {
        // Проверка SRP Batcher
        if (GraphicsSettings.useScriptableRenderPipelineBatching)
        {
            Debug.Log("SRP Batcher активен! 🚀");
        }
        else
        {
            Debug.LogWarning("SRP Batcher НЕ активен! Проверьте настройки.");
        }
        
        // Создаём объекты для теста
        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.position = Random.insideUnitSphere * 200f;
            
            // У каждого объекта свой оттенок (проверяем батчинг)
            var renderer = obj.GetComponent<Renderer>();
            renderer.material.color = new Color(
                Random.Range(0.5f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(0.5f, 1f)
            );
        }
        
        // Используйте Profiler и Frame Debugger для сравнения
    }
}
```

---

## 9. Лучшие практики
### ✅ Рекомендации:
1. Всегда включайте SRP Batcher в URP/HDRP проектах
2. Используйте одинаковые шейдеры для схожих объектов
3. Ограничьте количество уникальных материалов (группируйте)
4. Используйте `[PerRendererData]` вместо MaterialPropertyBlock
5. Оптимизируйте количество свойств в шейдере (< 32)
6. Проверяйте Frame Debugger для анализа батчинга

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Использование MaterialPropertyBlock
// Разрушает батчинг, т.к. каждый объект становится уникальным
var block = new MaterialPropertyBlock();
block.SetColor("_Color", color);
renderer.SetPropertyBlock(block);

// ✅ ПРАВИЛЬНО: Использовать PerRendererData в шейдере
// И установить цвет напрямую в материал
renderer.material.SetColor("_Color", color);

// ❌ ОШИБКА: Создание множества материалов
for (int i = 0; i < 1000; i++)
{
    material = new Material(baseMaterial); // 1000 материалов!
}

// ✅ ПРАВИЛЬНО: Использовать свойства материала для вариаций
// (SRP Batcher разделяет Per-Object и Shared данные)
renderer.material.SetColor("_Color", color);
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
