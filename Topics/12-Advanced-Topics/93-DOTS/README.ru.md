# ⚡ DOTS (Data-Oriented Technology Stack): Entities, ECS — архитектура для высокопроизводительных проектов
DOTS (Data-Oriented Technology Stack) — это новый подход к разработке в Unity, ориентированный на производительность. 
Вместо классического GameObject-подхода, DOTS использует ECS (Entity Component System) — архитектуру, основанную на данных, а не на объектах.

---

## 1. Что такое DOTS?
DOTS — это набор технологий, позволяющий создавать высокопроизводительные игры с миллионами объектов.

### 🧩 Основные компоненты DOTS:
| Компонент | Описание |
| --- | --- |
| Entities | Сущности (легковесные ID-объекты) |
| Components | Компоненты (данные без логики) |
| Systems | Системы (логика без данных) |
| Job System | Многопоточная обработка |
| Burst Compiler | Оптимизированный компилятор |

### 📊 Сравнение: GameObject vs DOTS
| Характеристика | GameObject | DOTS (ECS) |
| --- | --- | --- |
| Структура | Объектно-ориентированная | Data-Oriented (данные) |
| Производительность | Ограничена (1000-10000 объектов) | Высокая (1M+ объектов) |
| Память | Разбросанная (heap) | Последовательная (cache-friendly) |
| Многопоточность | Ограниченная | Полная (Job System) |
| Сложность | Низкая (легко начать) | Высокая (крутая кривая обучения) |

---

## 2. ECS — Entity Component System
ECS — это архитектурный паттерн, где:
| Компонент | Роль | Пример |
| --- | --- | --- |
| Entity | Идентификатор (int) | `Entity entity = ...` |
| Component | Данные (struct) | `struct Position { float3 Value; }` |
| System | Логика обработки | `class MovementSystem : SystemBase` |

### 🔄 Принцип работы ECS:
```text
   Entities           Components          Systems
   (ID)               (Data)              (Logic)
      ↓                   ↓                   ↓
   Entity 1          Position(10,5)     MovementSystem
   Entity 2          Velocity(2,0)      (обновляет позицию)
   Entity 3          Health(100)        HealthSystem
   ...               ...                (обновляет здоровье)
```

Ключевое отличие: Данные отделены от логики. Компоненты — это просто структуры с полями. Системы — это код, который обрабатывает эти данные.

---

## 3. Настройка DOTS в проекте
### 📦 Установка пакетов:
Через Package Manager:
1. Window → Package Manager
2. Включить "Preview Packages" (если нужно)
3. Установить:
   - `com.unity.entities`
   - `com.unity.jobs`
   - `com.unity.burst`
   - `com.unity.collections`
   - `com.unity.physics` (опционально)
  
Через manifest.json:
```json
{
  "dependencies": {
    "com.unity.entities": "1.0.0",
    "com.unity.jobs": "0.70.0",
    "com.unity.burst": "1.8.0",
    "com.unity.collections": "2.0.0",
    "com.unity.physics": "1.0.0"
  }
}
```

### 🏗️ Создание DOTS-сборки:
```csharp
// Добавить в Project Settings → Player → Scripting Define Symbols
ENABLE_DOTS
```

---

## 4. Компоненты (Components) — данные без логики
Компоненты — это структуры (struct), которые содержат только данные. Они должны быть blittable (не содержать ссылок на управляемые объекты).

### 📝 Создание компонента:
```csharp
using Unity.Entities;
using Unity.Mathematics;

// IComponentData — базовый интерфейс для компонентов
public struct PositionComponent : IComponentData
{
    public float3 Value;
}

public struct VelocityComponent : IComponentData
{
    public float3 Value;
}

public struct HealthComponent : IComponentData
{
    public int CurrentHealth;
    public int MaxHealth;
}

public struct IsDeadComponent : IComponentData, IEnableableComponent
{
    // Компонент-флаг (можно включать/выключать)
}

public struct AttackComponent : IComponentData
{
    public int Damage;
    public float Cooldown;
    public float CurrentCooldown;
}

// Компонент с буфером (для хранения списка)
public struct DamageHistoryComponent : IBufferElementData
{
    public int DamageAmount;
    public float Timestamp;
}
```

### 🗂️ Типы компонентов:
| Тип | Интерфейс | Описание |
| --- | --- | --- |
| Data Component | `IComponentData` | Основной тип данных |
| Tag Component | `IComponentData` (пустой) | Флаг для идентификации сущностей |
| Buffer Component | `IBufferElementData` | Динамический список данных |
| Enableable Component | `IEnableableComponent` | Компонент можно включать/выключать |
| Shared Component | `ISharedComponentData` | Данные, общие для группы сущностей |

### 🔍 Пример: Создание сущности с компонентами:
```csharp
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public GameObject prefab;  // Префаб для конвертации
    
    void Start()
    {
        // Получаем Entity Manager
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        // Создаём сущность
        Entity entity = entityManager.CreateEntity(
            typeof(PositionComponent),
            typeof(VelocityComponent),
            typeof(HealthComponent)
        );
        
        // Устанавливаем значения
        entityManager.SetComponentData(entity, new PositionComponent 
        { 
            Value = new float3(0, 0, 0) 
        });
        
        entityManager.SetComponentData(entity, new VelocityComponent 
        { 
            Value = new float3(1, 0, 0) 
        });
        
        entityManager.SetComponentData(entity, new HealthComponent 
        { 
            CurrentHealth = 100,
            MaxHealth = 100
        });
    }
}
```

---

## 5. Системы (Systems) — логика без данных
Системы обрабатывают компоненты. Они выполняются в определённом порядке и могут использовать Job System для многопоточности.

### 📝 Создание системы:
```csharp
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// SystemBase — базовый класс для систем
public partial class MovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        // Инициализация при создании системы
        Debug.Log("MovementSystem создана");
    }
    
    protected override void OnUpdate()
    {
        // Основная логика обновления
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // Запрос к данным (Query)
        Entities
            .ForEach((ref PositionComponent position, in VelocityComponent velocity) =>
            {
                // Обновляем позицию на основе скорости
                position.Value += velocity.Value * deltaTime;
            })
            .ScheduleParallel();  // Многопоточное выполнение
    }
    
    protected override void OnDestroy()
    {
        // Очистка при уничтожении системы
        Debug.Log("MovementSystem уничтожена");
    }
}
```

### 🔄 Иерархия систем:
```csharp
using Unity.Entities;

// Система может зависеть от других систем
[UpdateInGroup(typeof(SimulationSystemGroup))]        // Группа
[UpdateBefore(typeof(HealthSystem))]                  // Перед
[UpdateAfter(typeof(DamageSystem))]                   // После
public partial class MovementSystem : SystemBase
{
    // ...
}
```

### 📊 Группы систем:
| Группа | Описание |
| --- | --- |
| `InitializationSystemGroup` | Инициализация |
| `SimulationSystemGroup` | Основная игровая логика |
| `PresentationSystemGroup` | Рендеринг и UI |
| `FixedStepSimulationSystemGroup` | Фиксированный шаг (физика) |

### 🎯 Пример: Система здоровья
```csharp
public partial class HealthSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Обрабатываем все сущности с HealthComponent
        Entities
            .ForEach((ref HealthComponent health, in DamageComponent damage) =>
            {
                health.CurrentHealth -= damage.Amount;
                
                // Создаём флаг IsDead, если здоровье <= 0
                if (health.CurrentHealth <= 0)
                {
                    // Добавляем компонент-флаг
                    // (EntityCommandBuffer требуется для изменений)
                }
            })
            .ScheduleParallel();
    }
}
```

### 🧵 Работа с EntityCommandBuffer (ECB):
ECB используется для безопасного изменения сущностей из Job'ов.

```csharp
public partial class SpawnSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;
    
    protected override void OnCreate()
    {
        ecbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = ecbSystem.CreateCommandBuffer();
        var deltaTime = SystemAPI.Time.DeltaTime;
        var spawnTimer = 0f;
        
        Entities
            .ForEach((Entity entity, in SpawnerComponent spawner) =>
            {
                spawnTimer += deltaTime;
                
                if (spawnTimer >= spawner.Interval)
                {
                    spawnTimer = 0;
                    
                    // Создаём новую сущность через ECB
                    Entity newEntity = ecb.Instantiate(spawner.Prefab);
                    ecb.SetComponent(newEntity, new PositionComponent 
                    { 
                        Value = spawner.SpawnPosition 
                    });
                }
            })
            .ScheduleParallel();
        
        // Обязательно добавляем ECB в очередь
        ecbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
```

---

## 6. Job System — многопоточность
Job System позволяет выполнять код параллельно на всех доступных ядрах процессора.

### 📝 Создание Job'а:
```csharp
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

// IJob — однопоточный Job
public struct MovementJob : IJob
{
    public NativeArray<float3> positions;
    public NativeArray<float3> velocities;
    public float deltaTime;
    
    public void Execute()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] += velocities[i] * deltaTime;
        }
    }
}

// IJobParallelFor — многопоточный Job (для массивов)
public struct MovementParallelJob : IJobParallelFor
{
    public NativeArray<float3> positions;
    public NativeArray<float3> velocities;
    public float deltaTime;
    
    public void Execute(int index)
    {
        positions[index] += velocities[index] * deltaTime;
    }
}
```

### 🚀 Использование Job'ов в системе:
```csharp
public partial class JobMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Получаем данные из компонентов
        var positions = new NativeArray<float3>(1000, Allocator.TempJob);
        var velocities = new NativeArray<float3>(1000, Allocator.TempJob);
        
        // Создаём Job
        var job = new MovementJob
        {
            positions = positions,
            velocities = velocities,
            deltaTime = SystemAPI.Time.DeltaTime
        };
        
        // Запускаем Job
        JobHandle jobHandle = job.Schedule();
        
        // Ждём завершения Job'а
        jobHandle.Complete();
        
        // Применяем результаты обратно к компонентам
        // ...
        
        // Освобождаем память
        positions.Dispose();
        velocities.Dispose();
    }
}
```

---

## 7. Burst Compiler — оптимизация кода
Burst Compiler превращает C# код в высокооптимизированный машинный код.

### ⚡ Активация Burst:
```csharp
using Unity.Burst;

[BurstCompile]
public struct MovementJob : IJob
{
    // ...
    
    [BurstCompile]
    public void Execute()
    {
        // Этот код будет оптимизирован
    }
}

[BurstCompile(CompileSynchronously = true)]  // Синхронная компиляция (для отладки)
public struct DebugJob : IJob
{
    // ...
}
```

### 📊 Производительность Burst:
| Тип кода | Время выполнения (1M объектов) |
| --- | --- |
| MonoBehaviour (Update) | ~50-100 мс |
| ECS без Burst | ~10-20 мс |
| ECS с Burst | ~1-2 мс |

---

## 8. Полный пример: Система частиц (Particle System)
```csharp
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

// ============= КОМПОНЕНТЫ =============
public struct ParticleComponent : IComponentData
{
    public float Lifetime;
    public float MaxLifetime;
    public float3 Velocity;
    public float3 Acceleration;
    public float Size;
}

public struct ParticleSpawnerComponent : IComponentData
{
    public Entity Prefab;
    public int MaxParticles;
    public float SpawnRate;
    public float3 SpawnPosition;
}

public struct ParticleTag : IComponentData { }  // Тег-компонент

// ============= СИСТЕМЫ =============
[BurstCompile]
public partial class ParticleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        Entities
            .WithName("ParticleUpdate")
            .WithBurst()
            .ForEach((ref ParticleComponent particle, ref LocalTransform transform) =>
            {
                // Обновляем время жизни
                particle.Lifetime += deltaTime;
                
                // Если частица умерла
                if (particle.Lifetime >= particle.MaxLifetime)
                {
                    transform.Scale = 0f;  // Скрываем
                    return;
                }
                
                // Обновляем скорость
                particle.Velocity += particle.Acceleration * deltaTime;
                
                // Обновляем позицию
                transform.Position += particle.Velocity * deltaTime;
                
                // Уменьшаем размер (затухание)
                float lifeRatio = particle.Lifetime / particle.MaxLifetime;
                transform.Scale = particle.Size * (1f - lifeRatio);
            })
            .ScheduleParallel();
    }
}

[BurstCompile]
public partial class ParticleSpawnerSystem : SystemBase
{
    private float spawnTimer = 0f;
    
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        spawnTimer += deltaTime;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .ForEach((Entity entity, in ParticleSpawnerComponent spawner) =>
            {
                if (spawnTimer >= spawner.SpawnRate)
                {
                    spawnTimer = 0f;
                    
                    // Создаём новую частицу
                    Entity newParticle = ecb.Instantiate(spawner.Prefab);
                    
                    // Устанавливаем компоненты
                    ecb.SetComponent(newParticle, new LocalTransform
                    {
                        Position = spawner.SpawnPosition,
                        Rotation = quaternion.identity,
                        Scale = 1f
                    });
                    
                    ecb.SetComponent(newParticle, new ParticleComponent
                    {
                        Lifetime = 0f,
                        MaxLifetime = 2f,
                        Velocity = new float3(
                            UnityEngine.Random.Range(-2f, 2f),
                            UnityEngine.Random.Range(1f, 5f),
                            UnityEngine.Random.Range(-2f, 2f)
                        ),
                        Acceleration = new float3(0, -1f, 0),
                        Size = 0.5f
                    });
                }
            })
            .Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

// ============= МОНОБЕХАВИОР ДЛЯ СПАВНА =============
public class ParticleSpawnerAuthoring : MonoBehaviour
{
    public GameObject particlePrefab;
    public int maxParticles = 1000;
    public float spawnRate = 0.1f;
    
    class ParticleSpawnerBaker : Baker<ParticleSpawnerAuthoring>
    {
        public override void Bake(ParticleSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new ParticleSpawnerComponent
            {
                Prefab = GetEntity(authoring.particlePrefab, TransformUsageFlags.Dynamic),
                MaxParticles = authoring.maxParticles,
                SpawnRate = authoring.spawnRate,
                SpawnPosition = authoring.transform.position
            });
        }
    }
}
```

---

## 9. Преобразование GameObject → Entity (ConvertToEntity)
Unity предоставляет инструменты для конвертации GameObject в Entities.

### 🛠️ Использование ConvertToEntity:
```csharp
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ConvertToEntityExample : MonoBehaviour
{
    public GameObject prefab;
    
    void Start()
    {
        // 1. Используем GameObjectConversionUtility
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        
        // 2. Добавляем компоненты
        EntityManager.AddComponent<PositionComponent>(entity);
        EntityManager.SetComponentData(entity, new PositionComponent { Value = transform.position });
    }
}
```

### 🔄 Baker (Автоматическая конвертация):
```csharp
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float speed = 5f;
    public int health = 100;
    
    // Baker автоматически конвертирует MonoBehaviour в Entity
    class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            // Добавляем компоненты
            AddComponent(entity, new PlayerComponent
            {
                Speed = authoring.speed,
                Health = authoring.health
            });
            
            AddComponent<PlayerTag>(entity);  // Тег-компонент
        }
    }
}

public struct PlayerComponent : IComponentData
{
    public float Speed;
    public int Health;
}

public struct PlayerTag : IComponentData { }
```

---

## 10. Лучшие практики и частые ошибки
### ✅ Рекомендации:
1. Используйте структуры (struct), а не классы для компонентов
2. Разделяйте данные и логику — это основа ECS
3. Используйте Burst Compiler для критичных по производительности участков
4. Оптимизируйте запросы — запрашивайте только нужные компоненты
5. Используйте IEnableableComponent для включения/выключения сущностей
6. Работайте с EntityCommandBuffer для изменений из Job'ов

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Использование класса вместо структуры
public class HealthComponent : IComponentData { }  // Медленно!

// ✅ ПРАВИЛЬНО: struct
public struct HealthComponent : IComponentData { }

// ❌ ОШИБКА: Ссылки на управляемые объекты
public struct BadComponent : IComponentData
{
    public GameObject obj;  // Ошибка! Нельзя хранить ссылки
}

// ✅ ПРАВИЛЬНО: Entity или NativeContainer
public struct GoodComponent : IComponentData
{
    public Entity targetEntity;
}

// ❌ ОШИБКА: Модификация компонентов в Job'е без ECB
Entities.ForEach((Entity entity, ref HealthComponent health) =>
{
    // Нельзя создавать/удалять сущности здесь
    EntityManager.DestroyEntity(entity);  // Ошибка!
});

// ✅ ПРАВИЛЬНО: Использовать ECB
var ecb = new EntityCommandBuffer(Allocator.Temp);
Entities.ForEach((Entity entity) =>
{
    ecb.DestroyEntity(entity);
});
ecb.Playback(EntityManager);
ecb.Dispose();

// ❌ ОШИБКА: Забыли вызвать Dispose для Native Containers
var array = new NativeArray<float>(1000, Allocator.Temp);
// ... использование
// Нет Dispose! Утечка памяти

// ✅ ПРАВИЛЬНО:
array.Dispose();
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
