# ⚡ Статическое батчинг (Static Batching) и GPU Instancing: Объединение мешей для снижения draw calls

Draw Call — это команда от CPU к GPU на отрисовку одного набора графических примитивов. 
Чем меньше draw calls, тем выше производительность, особенно на мобильных устройствах. 
Unity предоставляет два основных механизма для их снижения: Static Batching и GPU Instancing.

---

## 1. Основные понятия
| Термин | Описание |
| --- | --- |
| Draw Call | Один вызов отрисовки одного меша с одним материалом |
| Batch | Группа объектов, отрисованных за один Draw Call |
| Static Batching | Объединение статических объектов в один большой меш на этапе сборки или загрузки |
| Dynamic Batching | Автоматическое объединение маленьких движущихся объектов (ограничено 300 вершинами) |
| GPU Instancing | Отрисовка множества одинаковых мешей с разными параметрами за один Draw Call |

### 📊 Сравнение методов:
| Характеристика | Static Batching | GPU Instancing | Dynamic Batching |
| --- | --- | --- | --- |
| Объекты могут двигаться | ❌ Нет | ✅ Да | ✅ Да |
| Ограничение по вершинам | ❌ Нет | ❌ Нет | ✅ < 300 вершин |
| Разные материалы | ❌ Только одинаковые | ❌ Только одинаковые | ❌ Только одинаковые |
| Память (RAM/VRAM) | 🔺 Высокая | 🔻 Низкая | 🔻 Низкая |
| Поддержка скинов | ❌ Нет | ❌ Нет | ❌ Нет |

---

## 2. Статическое батчинг (Static Batching)
Static Batching объединяет несколько статических объектов в один большой меш до начала работы игры. 
Это наиболее эффективный метод для статической геометрии (здания, дороги, ландшафт).

### 🛠️ Как включить Static Batching:
1. Выберите объект в сцене
2. В инспекторе отметьте флажок Static (или выберите Batching Static в выпадающем меню)
3. Unity автоматически объединит все статические объекты с одинаковым материалом

### 🧩 Настройка через код:
```csharp
using UnityEngine;

public class StaticBatchingSetup : MonoBehaviour
{
    void Start()
    {
        // Помечаем объект как статический для батчинга
        gameObject.isStatic = true;
        
        // Принудительно объединяем все статические объекты в сцене
        StaticBatching.Combine(gameObject.scene);
    }
}
```

### 📐 Пример: Строительство города
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
                
                // Помечаем как статический для батчинга
                building.isStatic = true;
                building.tag = "Building";
            }
        }
        
        // Объединяем все здания в один меш
        StaticBatching.Combine(gameObject.scene);
        Debug.Log($"Построено {gridSize * gridSize} зданий. Все они будут объединены в один Draw Call.");
    }
}
```

### ⚠️ Важные ограничения Static Batching:
```csharp
// ❌ Static Batching НЕ работает если:
// 1. Объекты используют разные материалы
// 2. Объекты двигаются (transform изменяется)
// 3. Объекты имеют разные настройки освещения (Lightmap Index)
// 4. Объекты используют скиновую анимацию (SkinnedMeshRenderer)

// ✅ Static Batching работает если:
// 1. Объекты помечены как Static
// 2. Объекты используют ОДИН и тот же материал
// 3. Объекты не двигаются во время игры
```

### 📈 Производительность и память:
```csharp
public class BatchingMemoryCheck : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.Label($"Draw Calls: {UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline}");
        
        // Получение статистики батчинга (только в редакторе)
        #if UNITY_EDITOR
        GUILayout.Label($"Saved by batching: {UnityEditor.UnityStats.batches}");
        #endif
    }
}
```

---

## 3. GPU Instancing
GPU Instancing позволяет отрисовать множество одинаковых мешей за один Draw Call, 
но при этом каждый объект может иметь свои параметры (цвет, позицию, масштаб). 
Вся магия происходит на GPU.

### 🛠️ Как включить GPU Instancing:
1. Выберите материал
2. В инспекторе отметьте флажок Enable GPU Instancing
3. Используйте один материал для всех объектов

### 🎲 Пример 1: Поле цветов (GPU Instancing)
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
            Debug.LogError("Включите GPU Instancing в материале цветка!");
            return;
        }
        
        matrices = new Matrix4x4[flowerCount];
        colors = new Vector4[flowerCount];
        propertyBlock = new MaterialPropertyBlock();
        
        // Генерируем позиции и цвета для каждого цветка
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
            
            // Случайный цвет
            colors[i] = new Vector4(
                Random.Range(0.5f, 1f),
                Random.Range(0.2f, 0.8f),
                Random.Range(0.2f, 0.8f),
                1f
            );
        }
        
        Debug.Log($"Создано {flowerCount} цветков с GPU Instancing → 1 Draw Call!");
    }
    
    void Update()
    {
        // Отрисовка всех цветков одним вызовом
        for (int i = 0; i < flowerCount; i += 1023) // Максимум 1023 матрицы за вызов
        {
            int batchSize = Mathf.Min(1023, flowerCount - i);
            Matrix4x4[] batchMatrices = new Matrix4x4[batchSize];
            System.Array.Copy(matrices, i, batchMatrices, 0, batchSize);
            
            // Устанавливаем цвета для этой партии
            for (int j = 0; j < batchSize; j++)
            {
                propertyBlock.SetVector("_Color", colors[i + j]);
                Graphics.DrawMeshInstanced(flowerMesh, 0, flowerMaterial, batchMatrices, batchSize, propertyBlock);
            }
        }
    }
}
```

### 🔴 Пример 2: Враги с разными цветами (MaterialPropertyBlock)
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
            Debug.LogError("Включите GPU Instancing в материале врага!");
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
        // Обновление позиций врагов
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
        
        // Отрисовка всех врагов одним вызовом GPU Instancing
        const int batchSize = 1023;
        int batches = Mathf.CeilToInt((float)enemyCount / batchSize);
        
        for (int b = 0; b < batches; b++)
        {
            int start = b * batchSize;
            int count = Mathf.Min(batchSize, enemyCount - start);
            
            Matrix4x4[] batchMatrices = new Matrix4x4[count];
            System.Array.Copy(matrices, start, batchMatrices, 0, count);
            
            // Устанавливаем цвет на основе здоровья для каждого врага
            propertyBlock.Clear();
            for (int i = 0; i < count; i++)
            {
                // Health: 1 = зеленый, 0.5 = желтый, 0 = красный
                Color color = Color.Lerp(Color.red, Color.green, healthPercent[start + i]);
                propertyBlock.SetColor("_Color", color);
                // Примечание: MaterialPropertyBlock применяется ко всем объектам в вызове
                // Для разных цветов нужно использовать отдельные вызовы или шейдеры с instanced свойствами
            }
            
            Graphics.DrawMeshInstanced(enemyMesh, 0, enemyMaterial, batchMatrices, count, propertyBlock);
        }
    }
}
```

### 🎨 Пример 3: Шейдер с поддержкой Instanced свойств
```hlsl
// Instanced шейдер для GPU Instancing
Shader "Custom/InstancedColored"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            }
            ENDCG
        }
    }
}
```

### 🚀 Пример 4: Сравнение производительности
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
        Debug.Log($"Standard: {objectCount} объектов → {objectCount} Draw Calls");
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
        Debug.Log($"Static Batching: {objectCount} объектов → ~1 Draw Call (если материал один)");
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
        Debug.Log($"GPU Instancing: {objectCount} объектов → 1 Draw Call");
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

## 4. Сравнение и выбор метода
### 📋 Когда что использовать:
| Ситуация | Лучший метод | Почему |
| --- | --- | --- |
| Статические здания, дороги, ландшафт | Static Batching | Один раз объединить и забыть |
| Трава, деревья, частицы | GPU Instancing | Движутся, но одинаковые |
| Толпы врагов с разными цветами | GPU Instancing + MPB | Разные параметры, но один меш |
| Маленькие движущиеся объекты (<300 вершин) | Dynamic Batching | Автоматически (но не надейтесь) |
| Объекты с анимацией (скелетной) | Ничего ❌ | Ни батчинг, ни инстансинг не работают |

### 🔬 Эксперимент: 10,000 объектов
```csharp
// Результаты на типичном мобильном устройстве:
// Standard: 10,000 draw calls → ~10 FPS (очень плохо)
// Static Batching: 1 draw call → ~60 FPS (отлично для статики)
// GPU Instancing: 10 draw calls (10,000 / 1023) → ~55 FPS (хорошо)
// Dynamic Batching: НЕ сработает (вершин > 300) → ~10 FPS
```

---

## 5. Продвинутые техники
### 🔄 Комбинация Static Batching и LOD
```csharp
using UnityEngine;

public class LODWithBatching : MonoBehaviour
{
    public GameObject[] lod0Buildings;
    public GameObject[] lod1Buildings;
    
    void Start()
    {
        // Помечаем все LOD объекты как статические
        foreach (var building in lod0Buildings)
            building.isStatic = true;
        foreach (var building in lod1Buildings)
            building.isStatic = true;
        
        // Объединяем каждый LOD уровень отдельно
        StaticBatching.Combine(gameObject.scene);
    }
}
```

### 🎭 GPU Instancing с анимацией (Vertex Shader)
```csharp
using UnityEngine;

public class AnimatedInstancing : MonoBehaviour
{
    public Mesh mesh;
    public Material materialWithAnimation;
    public int instanceCount = 1000;
    
    private Matrix4x4[] matrices;
    private float[] animationOffsets;
    
    void Start()
    {
        matrices = new Matrix4x4[instanceCount];
        animationOffsets = new float[instanceCount];
        
        for (int i = 0; i < instanceCount; i++)
        {
            matrices[i] = Matrix4x4.TRS(
                Random.insideUnitSphere * 30f,
                Random.rotation,
                Vector3.one * Random.Range(0.5f, 1.5f)
            );
            animationOffsets[i] = Random.Range(0f, Mathf.PI * 2);
        }
        
        // Передаем анимационные оффсеты через MaterialPropertyBlock
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        for (int i = 0; i < instanceCount; i += 1023)
        {
            int batchSize = Mathf.Min(1023, instanceCount - i);
            Matrix4x4[] batch = new Matrix4x4[batchSize];
            System.Array.Copy(matrices, i, batch, 0, batchSize);
            
            // В реальном проекте нужно передать массив оффсетов в шейдер
            // props.SetFloatArray("_Offsets", animationOffsets);
            
            Graphics.DrawMeshInstanced(mesh, 0, materialWithAnimation, batch, batchSize, props);
        }
    }
}
```

---

## 6. Диагностика и отладка
### 🛠️ Инструменты для проверки батчинга:
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

## 7. Лучшие практики
### ✅ Рекомендации:
1. Всегда включайте Static Batching для статической геометрии
2. Используйте GPU Instancing для повторяющихся объектов (монеты, враги, пули)
3. Объединяйте текстуры в атлас (Texture Atlas) для использования одного материала
4. Ограничьте количество материалов — меньше материалов = меньше draw calls
5. На мобильных устройствах отключайте Dynamic Batching — он потребляет CPU

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Движущийся объект помечен как Static
movingObject.isStatic = true; // Это сломает батчинг и будет пересобирать меш каждый кадр!

// ✅ ПРАВИЛЬНО: Только неподвижные объекты
building.isStatic = true; // Здание не двигается
movingObject.isStatic = false;

// ❌ ОШИБКА: Разные материалы у объектов с одним мешем
object1.GetComponent<Renderer>().material = materialRed;
object2.GetComponent<Renderer>().material = materialBlue;
// → 2 draw calls, даже с GPU Instancing

// ✅ ПРАВИЛЬНО: Один материал + MaterialPropertyBlock
Material sharedMaterial = materialBase;
MaterialPropertyBlock props = new MaterialPropertyBlock();
props.SetColor("_Color", Color.red);
Graphics.DrawMeshInstanced(mesh, 0, sharedMaterial, matrices, count, props);
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
