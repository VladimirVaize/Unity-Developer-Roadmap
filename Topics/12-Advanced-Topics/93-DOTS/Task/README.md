# 🎯 Task: «Creating a High-Performance Particle System with DOTS»
You are developing a space simulation game with thousands of asteroids. 
You need to create a particle (asteroid) system using DOTS (ECS) that can handle 100,000+ objects with smooth performance.

## 📝 What to Implement:
### Part 1: Components (Data)
1. Create components:

| Component | Fields | Purpose |
| --- | --- | --- |
| `AsteroidComponent` | `float Size`, `float RotationSpeed`, `float3 Velocity` | Asteroid data |
| `HealthComponent` | `int CurrentHealth`, `int MaxHealth` | Asteroid health |
| `DestroyComponent` | `float Timer` | Timer until destruction |
| `AsteroidTag` | (empty) | Tag for identification |

2. Create buffer component `DamageHistoryBuffer`:
   - Stores last 10 received damages
   - Fields: `int Damage`, `float Timestamp`
  
### Part 2: Systems (Logic)
3. Create `AsteroidMovementSystem`:
   - Updates position based on velocity
   - Rotates asteroid (changes rotation)
   - Uses `BurstCompile` and `ScheduleParallel`
  
4. Create `AsteroidHealthSystem`:
   - Checks health (if <= 0 → adds `DestroyComponent`)
   - Processes damage taken
   - Adds entry to `DamageHistoryBuffer`
  
5. Create `AsteroidDestroySystem`:
   - Increments `DestroyComponent` timer
   - When timer reaches limit → destroys entity
   - Spawns 3-5 "fragments" (small asteroids)
  
6. Create `AsteroidSpawnSystem`:
   - Spawns new asteroids at a set rate
   - Uses `EntityCommandBuffer`
   - Distributes randomly around the center
  
### Part 3: Job System
7. Create Job for parallel update:
   - `IAsteroidUpdateJob` — IJobParallelFor
   - Updates 100,000+ asteroids simultaneously
   - Uses NativeArrays for data
  
8. Create Job for collision detection:
   - `CollisionDetectionJob` — IJobParallelFor
   - Checks distance between asteroids
   - Deals damage to both on collision
  
### Part 4: Burst Compiler
9. Apply `[BurstCompile]` to all Jobs
10. Use `[BurstCompile(CompileSynchronously = true)]` for debugging
11. Verify Burst works (check via Profiler)

### Part 5: GameObject → Entity Conversion
12. Create Authoring component:
```csharp
public class AsteroidAuthoring : MonoBehaviour
{
    public float size = 1f;
    public float rotationSpeed = 1f;
    public int health = 10;
    public GameObject prefab;
    
    class AsteroidBaker : Baker<AsteroidAuthoring>
    {
        public override void Bake(AsteroidAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            // Add components
        }
    }
}
```

13. Asteroid prefab should contain:
    - Mesh (sphere/asteroid)
    - `AsteroidAuthoring` script
    - Unity Physics (optional)
   
### Part 6: Performance Testing
14. Create spawn scenarios:
    - 10,000 asteroids
    - 50,000 asteroids
    - 100,000 asteroids
   
15. Measure FPS for each scenario via Profiler
16. Compare performance:
    - GameObject version (for control)
    - DOTS version without Burst
    - DOTS version with Burst
   
---

## 🧰 Implementation Requirements:
- Use at least 3 systems (Movement, Health, Destroy/Spawn)
- Use at least 2 Jobs (Update, Collision)
- All systems must use `BurstCompile`
- Use `EntityCommandBuffer` for spawning/destroying
- All components — struct, not class
- Use `IEnableableComponent` to disable destroyed asteroids

---

## 🔍 Verification:
1. Verify that asteroids move and rotate
2. Verify that health decreases when damaged
3. Verify that when health <= 0, the asteroid is destroyed (or explodes)
4. Verify that new asteroids spawn automatically
5. Verify that in Profiler, DOTS version shows > 100 FPS for 100K asteroids
6. Verify that the system works in the build (IL2CPP + Burst)

---

## 💡 Expected Result:
```text
=== Performance Test ===
GameObject (10K): 30-40 FPS
GameObject (50K): 5-10 FPS
GameObject (100K): 1-2 FPS

DOTS without Burst (10K): 60 FPS
DOTS without Burst (50K): 30-40 FPS
DOTS without Burst (100K): 10-15 FPS

DOTS + Burst (10K): 120+ FPS
DOTS + Burst (50K): 90-100 FPS
DOTS + Burst (100K): 60-80 FPS

Result: DOTS + Burst is 60-80 times faster than GameObject approach! 🚀
```

---

## 🏆 Bonus Task (Optional):
Implement Visual Effect Graph with DOTS:
- Use `VFXGraph` for rendering asteroids
- Instead of Mesh Renderer, use VFX Particle System
- Achieve performance of 1,000,000+ asteroids

---

### ⭐ If this project was useful, put a star on GitHub!
