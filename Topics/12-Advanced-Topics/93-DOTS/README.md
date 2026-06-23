# ⚡ DOTS (Data-Oriented Technology Stack): Entities, ECS — Architecture for High-Performance Projects
DOTS (Data-Oriented Technology Stack) is a new approach to development in Unity focused on performance. 
Instead of the classic GameObject approach, DOTS uses ECS (Entity Component System) — an architecture based on data rather than objects.

---

## 1. What is DOTS?
DOTS is a set of technologies that allows creating high-performance games with millions of objects.

### 🧩 Core DOTS Components:
| Component | Description |
| --- | --- |
| Entities | Entities (lightweight ID objects) |
| Components | Components (data without logic) |
| Systems | Systems (logic without data) |
| Job System | Multithreaded processing |
| Burst Compiler | Optimized compiler |

### 📊 Comparison: GameObject vs DOTS
| Feature | GameObject | DOTS (ECS) |
| --- | --- | --- |
| Structure | Object-oriented | Data-Oriented |
| Performance | Limited (1000-10000 objects) | High (1M+ objects) |
| Memory | Scattered (heap) | Sequential (cache-friendly) |
| Multithreading | Limited | Full (Job System) |
| Complexity | Low (easy to start) | High (steep learning curve) |

---

## 2. ECS — Entity Component System
ECS is an architectural pattern where:
| Component | Role | Example |
| --- | --- | --- |
| Entity | Identifier (int) | `Entity entity = ...` |
| Component | Data (struct) | `struct Position { float3 Value; }` |
| System | Processing logic | `class MovementSystem : SystemBase` |

### 🔄 How ECS Works:
```text
   Entities           Components          Systems
   (ID)               (Data)              (Logic)
      ↓                   ↓                   ↓
   Entity 1          Position(10,5)     MovementSystem
   Entity 2          Velocity(2,0)      (updates position)
   Entity 3          Health(100)        HealthSystem
   ...               ...                (updates health)
```
Key Difference: Data is separated from logic. Components are just structs with fields. Systems are code that processes this data.

---

## 3. Setting Up DOTS in a Project
### 📦 Installing Packages:
Via Package Manager:
1. Window → Package Manager
2. Enable "Preview Packages" (if needed)
3. Install:
   - `com.unity.entities`
   - `com.unity.jobs`
   - `com.unity.burst`
   - `com.unity.collections`
   - `com.unity.physics` (optional)

Via manifest.json:
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

### 🏗️ Creating DOTS Build:
```csharp
// Add to Project Settings → Player → Scripting Define Symbols
ENABLE_DOTS
```

---

## 4. Components — Data Without Logic
Components are structs that contain only data. They must be blittable (no references to managed objects).

### 📝 Creating a Component:
```csharp
using Unity.Entities;
using Unity.Mathematics;

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
    // Flag component (can be enabled/disabled)
}

public struct AttackComponent : IComponentData
{
    public int Damage;
    public float Cooldown;
    public float CurrentCooldown;
}

public struct DamageHistoryComponent : IBufferElementData
{
    public int DamageAmount;
    public float Timestamp;
}
```

### 🗂️ Types of Components:
| Type | Interface | Description |
| --- | --- | --- |
| Data Component | `IComponentData` | Main data type |
| Tag Component | `IComponentData` (empty) | Flag to identify entities |
| Buffer Component | `IBufferElementData` | Dynamic list of data |
| Enableable Component | `IEnableableComponent` | Can be enabled/disabled |
| Shared Component | `ISharedComponentData` | Data shared by entity group |

### 🔍 Example: Creating Entity with Components:
```csharp
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public GameObject prefab;
    
    void Start()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        Entity entity = entityManager.CreateEntity(
            typeof(PositionComponent),
            typeof(VelocityComponent),
            typeof(HealthComponent)
        );
        
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

## 5. Systems — Logic Without Data
Systems process components. They execute in a specific order and can use the Job System for multithreading.

### 📝 Creating a System:
```csharp
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class MovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        Debug.Log("MovementSystem created");
    }
    
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        Entities
            .ForEach((ref PositionComponent position, in VelocityComponent velocity) =>
            {
                position.Value += velocity.Value * deltaTime;
            })
            .ScheduleParallel();
    }
    
    protected override void OnDestroy()
    {
        Debug.Log("MovementSystem destroyed");
    }
}
```

### 🔄 System Hierarchy:
```csharp
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(HealthSystem))]
[UpdateAfter(typeof(DamageSystem))]
public partial class MovementSystem : SystemBase
{
    // ...
}
```

### 📊 System Groups:
| Group | Description |
| --- | --- |
| `InitializationSystemGroup` | Initialization |
| `SimulationSystemGroup` | Main game logic |
| `PresentationSystemGroup` | Rendering and UI | 
| `FixedStepSimulationSystemGroup` | Fixed step (physics) |

---

## 6. Job System — Multithreading
The Job System allows executing code in parallel on all available CPU cores.

### 📝 Creating a Job:
```csharp
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

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

### 🚀 Using Jobs in Systems:
```csharp
public partial class JobMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var positions = new NativeArray<float3>(1000, Allocator.TempJob);
        var velocities = new NativeArray<float3>(1000, Allocator.TempJob);
        
        var job = new MovementJob
        {
            positions = positions,
            velocities = velocities,
            deltaTime = SystemAPI.Time.DeltaTime
        };
        
        JobHandle jobHandle = job.Schedule();
        jobHandle.Complete();
        
        // Apply results back to components
        // ...
        
        positions.Dispose();
        velocities.Dispose();
    }
}
```

---

## 7. Burst Compiler — Code Optimization
The Burst Compiler turns C# code into highly optimized machine code.

### ⚡ Activating Burst:
```csharp
using Unity.Burst;

[BurstCompile]
public struct MovementJob : IJob
{
    // ...
    
    [BurstCompile]
    public void Execute()
    {
        // This code will be optimized
    }
}

[BurstCompile(CompileSynchronously = true)]
public struct DebugJob : IJob
{
    // ...
}
```

### 📊 Burst Performance:
| Code Type | Execution Time (1M objects) |
| --- | --- |
| MonoBehaviour (Update) | ~50-100 ms |
| ECS without Burst | ~10-20 ms |
| ECS with Burst | ~1-2 ms |

---

## 8. Full Example: Particle System
```csharp
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

// ============= COMPONENTS =============
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

public struct ParticleTag : IComponentData { }

// ============= SYSTEMS =============
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
                particle.Lifetime += deltaTime;
                
                if (particle.Lifetime >= particle.MaxLifetime)
                {
                    transform.Scale = 0f;
                    return;
                }
                
                particle.Velocity += particle.Acceleration * deltaTime;
                transform.Position += particle.Velocity * deltaTime;
                
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
                    
                    Entity newParticle = ecb.Instantiate(spawner.Prefab);
                    
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

// ============= MONOBEHAVIOR FOR SPAWNING =============
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

## 9. GameObject → Entity Conversion (ConvertToEntity)
Unity provides tools for converting GameObjects to Entities.

### 🛠️ Using ConvertToEntity:
```csharp
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ConvertToEntityExample : MonoBehaviour
{
    public GameObject prefab;
    
    void Start()
    {
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        
        EntityManager.AddComponent<PositionComponent>(entity);
        EntityManager.SetComponentData(entity, new PositionComponent { Value = transform.position });
    }
}
```

### 🔄 Baker (Automatic Conversion):
```csharp
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float speed = 5f;
    public int health = 100;
    
    class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new PlayerComponent
            {
                Speed = authoring.speed,
                Health = authoring.health
            });
            
            AddComponent<PlayerTag>(entity);
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

## 10. Best Practices and Common Mistakes
### ✅ Recommendations:
1. Use structs, not classes for components
2. Separate data and logic — this is the foundation of ECS
3. Use Burst Compiler for performance-critical sections
4. Optimize queries — only request the components you need
5. Use IEnableableComponent for enabling/disabling entities
6. Use EntityCommandBuffer for changes from Jobs

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Using class instead of struct
public class HealthComponent : IComponentData { }  // Slow!

// ✅ CORRECT: struct
public struct HealthComponent : IComponentData { }

// ❌ ERROR: References to managed objects
public struct BadComponent : IComponentData
{
    public GameObject obj;  // Error! Can't store references
}

// ✅ CORRECT: Entity or NativeContainer
public struct GoodComponent : IComponentData
{
    public Entity targetEntity;
}

// ❌ ERROR: Modifying components in Job without ECB
Entities.ForEach((Entity entity, ref HealthComponent health) =>
{
    EntityManager.DestroyEntity(entity);  // Error!
});

// ✅ CORRECT: Use ECB
var ecb = new EntityCommandBuffer(Allocator.Temp);
Entities.ForEach((Entity entity) =>
{
    ecb.DestroyEntity(entity);
});
ecb.Playback(EntityManager);
ecb.Dispose();

// ❌ ERROR: Forgot to Dispose Native Containers
var array = new NativeArray<float>(1000, Allocator.Temp);
// No Dispose! Memory leak

// ✅ CORRECT:
array.Dispose();
```

---

### ⭐ If this project was useful, put a star on GitHub!
