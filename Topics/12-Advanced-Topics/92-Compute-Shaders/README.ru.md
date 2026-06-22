# ⚡ Compute Shaders в Unity: Использование GPU для параллельных вычислений (физика, процедурная генерация)
Compute Shaders — это программы, выполняющиеся на GPU (графическом процессоре) и предназначенные для массовых параллельных вычислений. 
Они позволяют выполнять тысячи операций одновременно, что делает их идеальными для физики частиц, процедурной генерации, обработки изображений и других вычислительно-ёмких задач.

---

## 1. Что такое Compute Shader и зачем он нужен?
### 🧠 CPU vs GPU
| Характеристика | CPU | GPU |
| --- | --- | --- |
| Количество ядер | 4-32 | 1000-10000+ |
| Параллелизм | Последовательный | Массово-параллельный |
| Скорость операций с плавающей точкой | ~100 GFLOPS | ~10-30 TFLOPS |
| Использование | Управление, логика | Математика, графика |
| Подходит для | AI, UI, сетевое взаимодействие | Физика, частицы, рендеринг |

### 🎯 Когда использовать Compute Shader:
- ✅ Физика частиц — тысячи объектов с гравитацией и столкновениями
- ✅ Процедурная генерация — террейн, текстуры, меши
- ✅ Обработка изображений — пост-эффекты, фильтры, распознавание
- ✅ Симуляция жидкостей/тканей — сложные физические модели
- ✅ AI/нейросети — умножение матриц, forward/backward pass
- ✅ Визуализация данных — большие объёмы данных

### ❌ Не использовать для:
- Логики UI и ввода
- Небольшого количества объектов (< 100)
- Операций с ветвлениями (if/else)

---

## 2. Структура Compute Shader
### 📁 Создание Compute Shader:
```text
Assets/ → Create → Shader → Compute Shader
```

### 📄 Базовая структура:
```hlsl
// ==========================================
// 1. Настройка версии и директивы
// ==========================================
#pragma kernel CSMain          // Главная функция (kernel)
#pragma kernel CSSecondary     // Вторичная функция

// ==========================================
// 2. Буферы (данные)
// ==========================================
RWStructuredBuffer<float3> resultBuffer;   // Записываемый буфер
StructuredBuffer<float3> inputBuffer;      // Читаемый буфер
RWTexture2D<float4> outputTexture;         // Текстура для записи

// ==========================================
// 3. Константы (входные параметры)
// ==========================================
int width;
int height;
float time;

// ==========================================
// 4. Основная функция (kernel)
// ==========================================
[numthreads(8, 8, 1)]          // Размер группы: 8x8x1 = 64 потока
void CSMain(uint3 id : SV_DispatchThreadID)
{
    // Получаем индекс текущего потока
    uint index = id.x + id.y * width;
    
    // Выполняем вычисления
    float3 result = inputBuffer[index] * 2.0f;
    
    // Записываем результат
    resultBuffer[index] = result;
}
```

---

## 3. Основные понятия Compute Shader
### 🧵 Группы потоков (Thread Groups)
```text
Dispatch(10, 10, 1)           // Всего групп: 10x10x1 = 100 групп
[numthreads(8, 8, 1)]         // Каждая группа: 8x8x1 = 64 потока

Общее количество потоков: 100 * 64 = 6400
```

### 📐 Индексы потоков:
| Индекс | Описание |
| --- | --- |
| `SV_GroupID` | ID группы (от 0 до Dispatch-1) |
| `SV_GroupThreadID` | ID потока внутри группы (от 0 до numthreads-1) |
| `SV_DispatchThreadID` | Глобальный ID потока = `GroupID * numthreads + GroupThreadID` |
| `SV_GroupIndex` | Индекс потока в группе (0 .. numthreads-1) |

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

## 4. Буферы в Compute Shader
### 📦 Типы буферов:

| Тип | Описание | Пример использования |
| --- | --- | --- |
| `RWStructuredBuffer<T>` | Чтение/запись структурированных данных | Позиции частиц |
| `StructuredBuffer<T>` | Только чтение | Входные данные |
| `RWTexture2D<T>` | Чтение/запись в текстуру | Генерация текстур |
| `Texture2D<T>` | Только чтение текстуры | Входное изображение |
| `RWByteAddressBuffer` | Чтение/запись сырых байтов | Низкоуровневые данные |

### 📝 Определение структур:
```hlsl
// В Compute Shader
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
// В C# скрипте
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

## 5. Пример 1: Процедурная генерация текстур
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
    
    // Нормализованные координаты
    float2 uv = float2(x / (float)textureWidth, y / (float)textureHeight);
    
    // Создаём волны
    float wave = sin(uv.x * 20 + time) * cos(uv.y * 20 + time * 0.7);
    float value = (wave + 1) * 0.5;
    
    // Цвет
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
        // Создаём RenderTexture
        renderTexture = new RenderTexture(512, 512, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        
        // Находим Kernel
        kernelIndex = computeShader.FindKernel("CSMain");
        
        // Устанавливаем параметры
        computeShader.SetTexture(kernelIndex, "result", renderTexture);
        computeShader.SetInt("textureWidth", 512);
        computeShader.SetInt("textureHeight", 512);
        
        // Назначаем текстуру на материал
        targetRenderer.material.mainTexture = renderTexture;
    }
    
    void Update()
    {
        // Обновляем время
        computeShader.SetFloat("time", Time.time);
        
        // Запускаем Compute Shader
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

## 6. Пример 2: Симуляция частиц на GPU
### 📄 Compute Shader: `ParticleSystem.compute`
```hlsl
#pragma kernel UpdateParticles
#pragma kernel RenderParticles

// ==========================================
// Данные
// ==========================================
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

// ==========================================
// Обновление частиц
// ==========================================
[numthreads(64, 1, 1)]
void UpdateParticles(uint3 id : SV_DispatchThreadID)
{
    int index = id.x;
    if (index >= particleCount) return;
    
    Particle p = particles[index];
    
    // Симуляция физики
    p.velocity += gravity * deltaTime;
    p.position += p.velocity * deltaTime;
    p.lifetime -= deltaTime;
    
    // Если частица умерла — респавним
    if (p.lifetime <= 0)
    {
        p.position = initialPositions[index] + float3(0, 1, 0);
        p.velocity = float3(RandomRange(-1, 1), RandomRange(2, 5), RandomRange(-1, 1));
        p.lifetime = RandomRange(1, 3);
    }
    
    particles[index] = p;
}

// ==========================================
// Случайные числа (штатный метод)
// ==========================================
float RandomRange(float min, float max)
{
    // Простая реализация случайного числа
    float r = frac(sin(dot(float2(index, time), float2(12.9898, 78.233))) * 43758.5453);
    return min + r * (max - min);
}
```

### 🎮 C# Script:
```csharp
using UnityEngine;

public class GPUParticleSystem : MonoBehaviour
{
    public ComputeShader computeShader;
    public int particleCount = 10000;
    public Mesh particleMesh;
    public Material particleMaterial;
    
    private ComputeBuffer particleBuffer;
    private ComputeBuffer colorBuffer;
    private ParticleData[] particles;
    private int updateKernel;
    private int particleCountPerDraw = 1024;
    
    [System.Serializable]
    public struct ParticleData
    {
        public Vector3 position;
        public Vector3 velocity;
        public float mass;
        public float lifetime;
    }
    
    void Start()
    {
        // Инициализация буферов
        particles = new ParticleData[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            particles[i].position = Random.insideUnitSphere * 10;
            particles[i].velocity = Random.insideUnitSphere * 2;
            particles[i].mass = 1;
            particles[i].lifetime = Random.Range(1f, 3f);
        }
        
        particleBuffer = new ComputeBuffer(particleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(ParticleData)));
        particleBuffer.SetData(particles);
        
        // Настройка Compute Shader
        updateKernel = computeShader.FindKernel("UpdateParticles");
        computeShader.SetBuffer(updateKernel, "particles", particleBuffer);
        computeShader.SetInt("particleCount", particleCount);
        computeShader.SetVector("gravity", new Vector3(0, -9.81f, 0));
    }
    
    void Update()
    {
        // Обновление параметров
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetFloat("time", Time.time);
        
        // Запуск обновления
        int threadGroups = Mathf.CeilToInt(particleCount / 64.0f);
        computeShader.Dispatch(updateKernel, threadGroups, 1, 1);
        
        // Получение данных обратно (если нужно на CPU)
        // particleBuffer.GetData(particles);
    }
    
    void OnRenderObject()
    {
        // Рендеринг частиц через GPU Instancing
        particleMaterial.SetBuffer("_ParticleBuffer", particleBuffer);
        Graphics.DrawMeshInstancedProcedural(
            particleMesh, 0, particleMaterial,
            new Bounds(Vector3.zero, Vector3.one * 100),
            particleCount
        );
    }
    
    void OnDestroy()
    {
        particleBuffer?.Release();
    }
}
```

---

## 7. Пример 3: Игра "Жизнь" (Conway's Game of Life) на GPU
### 📄 Compute Shader: `GameOfLife.compute`
```hlsl
#pragma kernel UpdateLife

RWTexture2D<float4> cells;      // Текущее состояние (1.0 = жива, 0.0 = мертва)
RWTexture2D<float4> nextCells;  // Следующее состояние
int width;
int height;

[numthreads(8, 8, 1)]
void UpdateLife(uint3 id : SV_DispatchThreadID)
{
    int x = id.x;
    int y = id.y;
    
    // Проверяем границы
    if (x >= width || y >= height) return;
    
    // Подсчёт соседей
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
    
    // Правила игры "Жизнь"
    if (alive && (neighbors == 2 || neighbors == 3))
        nextAlive = true;
    else if (!alive && neighbors == 3)
        nextAlive = true;
    else
        nextAlive = false;
    
    nextCells[uint2(x, y)] = float4(nextAlive ? 1.0f : 0.0f, 0, 0, 1);
}
```

### 🎮 C# Script:
```csharp
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public ComputeShader computeShader;
    public int size = 256;
    
    private RenderTexture cells;
    private RenderTexture nextCells;
    private int kernelIndex;
    
    void Start()
    {
        // Создаём две текстуры
        cells = new RenderTexture(size, size, 0);
        cells.enableRandomWrite = true;
        cells.Create();
        
        nextCells = new RenderTexture(size, size, 0);
        nextCells.enableRandomWrite = true;
        nextCells.Create();
        
        // Инициализация случайными клетками
        InitializeRandom();
        
        // Настройка Compute Shader
        kernelIndex = computeShader.FindKernel("UpdateLife");
        computeShader.SetTexture(kernelIndex, "cells", cells);
        computeShader.SetTexture(kernelIndex, "nextCells", nextCells);
        computeShader.SetInt("width", size);
        computeShader.SetInt("height", size);
    }
    
    void InitializeRandom()
    {
        // Создаём текстуру со случайными значениями
        Texture2D temp = new Texture2D(size, size);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float value = Random.value > 0.7f ? 1f : 0f;
                temp.SetPixel(x, y, new Color(value, 0, 0, 1));
            }
        }
        temp.Apply();
        Graphics.Blit(temp, cells);
        Destroy(temp);
    }
    
    void Update()
    {
        // Запуск Compute Shader
        int groupsX = Mathf.CeilToInt(size / 8.0f);
        int groupsY = Mathf.CeilToInt(size / 8.0f);
        computeShader.Dispatch(kernelIndex, groupsX, groupsY, 1);
        
        // Меняем текстуры местами
        RenderTexture temp = cells;
        cells = nextCells;
        nextCells = temp;
        
        // Обновляем параметры для следующего кадра
        computeShader.SetTexture(kernelIndex, "cells", cells);
        computeShader.SetTexture(kernelIndex, "nextCells", nextCells);
    }
    
    void OnGUI()
    {
        // Отображение результата
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), cells);
    }
    
    void OnDestroy()
    {
        cells?.Release();
        nextCells?.Release();
    }
}
```

---

## 8. Пример 4: GPU-акселерация для физики
### 📄 Compute Shader: `Physics.compute`
```hlsl
#pragma kernel ApplyGravity
#pragma kernel ResolveCollisions

struct Body
{
    float3 position;
    float3 velocity;
    float mass;
    float radius;
};

RWStructuredBuffer<Body> bodies;
int bodyCount;
float deltaTime;
float3 gravityCenter;
float gravityStrength;

// ==========================================
// Применение гравитации (N-Body)
// ==========================================
[numthreads(64, 1, 1)]
void ApplyGravity(uint3 id : SV_DispatchThreadID)
{
    int i = id.x;
    if (i >= bodyCount) return;
    
    Body a = bodies[i];
    float3 force = float3(0, 0, 0);
    
    // Вычисляем гравитацию от всех других тел (O(N²))
    for (int j = 0; j < bodyCount; j++)
    {
        if (i == j) continue;
        
        Body b = bodies[j];
        float3 dir = b.position - a.position;
        float dist = length(dir);
        
        // Избегаем сингулярности
        dist = max(dist, 1.0f);
        dir = normalize(dir);
        
        // F = G * m1 * m2 / r²
        float G = 0.001f;
        float f = G * a.mass * b.mass / (dist * dist);
        force += dir * f;
    }
    
    // Обновляем скорость и позицию
    a.velocity += force / a.mass * deltaTime;
    a.position += a.velocity * deltaTime;
    
    // Притяжение к центру (если нужно)
    // a.velocity += (gravityCenter - a.position) * gravityStrength * deltaTime;
    
    bodies[i] = a;
}

// ==========================================
// Разрешение столкновений
// ==========================================
[numthreads(64, 1, 1)]
void ResolveCollisions(uint3 id : SV_DispatchThreadID)
{
    int i = id.x;
    if (i >= bodyCount) return;
    
    Body a = bodies[i];
    
    for (int j = i + 1; j < bodyCount; j++)
    {
        Body b = bodies[j];
        float3 dir = b.position - a.position;
        float dist = length(dir);
        float minDist = a.radius + b.radius;
        
        if (dist < minDist && dist > 0)
        {
            // Разрешаем столкновение
            dir = normalize(dir);
            float overlap = minDist - dist;
            
            // Отталкиваем тела
            a.position -= dir * overlap * 0.5f;
            b.position += dir * overlap * 0.5f;
            
            // Обмен скоростей (упругий удар)
            float3 relVel = a.velocity - b.velocity;
            float impulse = dot(relVel, dir);
            if (impulse < 0)
            {
                float e = 0.5f; // Коэффициент восстановления
                float j = -(1 + e) * impulse / (1 / a.mass + 1 / b.mass);
                a.velocity += dir * j / a.mass;
                b.velocity -= dir * j / b.mass;
            }
            
            bodies[j] = b;
        }
    }
    
    bodies[i] = a;
}
```

---

## 9. Оптимизация и лучшие практики
### 🚀 Советы по оптимизации:
1. Размер группы:
   - Используйте `[numthreads(64, 1, 1)]` для 1D данных
   - Используйте `[numthreads(8, 8, 1)]` для 2D данных
   - Всегда кратно 4 (лучше 8, 16, 32, 64)
  
2. Избегайте ветвлений:
   - В GPU все потоки в группе выполняются синхронно
   - Если есть `if/else`, и разные потоки идут по разным веткам, производительность падает
   - Используйте тернарные операторы: `value = condition ? a : b;`
  
3. Минимизируйте передачу данных:
   - Передавайте данные на GPU один раз
   - Получайте обратно только при необходимости
   - Используйте `ComputeBuffer` для больших данных
  
4. Блокировка синхронизации:
   - Избегайте `Atomic` операций внутри шейдера
   - Если нужна синхронизация, используйте отдельные проходы
  
5. Профилирование:
   - Используйте Unity Profiler → GPU
   - Проверяйте количество потоков и время выполнения
  
### ⚠️ Частые ошибки:
```hlsl
// ❌ ОШИБКА: Ветвление внутри потока
if (someCondition)
{
    // Долгая операция
}
else
{
    // Другая долгая операция
}

// ✅ ПРАВИЛЬНО: Использовать тернарный оператор
float result = someCondition ? DoSomething() : DoSomethingElse();

// ❌ ОШИБКА: Доступ к памяти без проверки границ
float3 pos = positions[index + 10000]; // Выход за пределы

// ✅ ПРАВИЛЬНО: Проверка границ
if (index < bufferSize)
{
    float3 pos = positions[index];
}

// ❌ ОШИБКА: Слишком маленькая группа
[numthreads(1, 1, 1)] // 1 поток на группу — неэффективно

// ✅ ПРАВИЛЬНО: Минимум 64 потока
[numthreads(64, 1, 1)]
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
