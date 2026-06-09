# 🧭 Navigation System in Unity: NavMesh Surface, NavMesh Agent, NavMesh Obstacle
NavMesh (Navigation Mesh) is Unity's AI system that allows characters to autonomously 
navigate the game world, avoid obstacles, and find optimal routes. It is based on three key components.

---

## 1. NavMesh Surface — Navigation Mesh Generation
NavMesh Surface is responsible for creating the navigation mesh — the "blue zone" where agents can move. This mesh is generated (baked) based on the scene geometry.

### 🔧 Key Parameters:
| Parameter | Description |
| --- | --- |
| `Agent Type` | Agent type (size affects passability) |
| `Default Area` | Area type (Walkable, Not Walkable, Jump) |
| `Use Geometry` | Where to take geometry from: Render Meshes or Physics Colliders |
| `Layers` | Which layers participate in baking |
| `Voxel Size` | Mesh accuracy (smaller = more accurate, but slower) |

### 📝 Usage Example:
```csharp
using UnityEngine.AI;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface surface;

    void Start()
    {
        // Bake NavMesh at start (for procedurally generated levels)
        surface.BuildNavMesh();
    }

    void Update()
    {
        // Re-bake when level changes (e.g., wall destruction)
        if (Input.GetKeyDown(KeyCode.B))
        {
            surface.RemoveActiveSurfaces();
            surface.BuildNavMesh();
            Debug.Log("NavMesh rebuilt!");
        }
    }
}
```

> [!Tip]
> Static objects (walls, floors) should participate in baking. Dynamic objects use NavMeshObstacle.

---

## 2. NavMesh Agent — Character Movement Control
NavMesh Agent is a component added to a character that controls its movement across the NavMesh. 
It automatically calculates path, speed, acceleration, and avoids other agents and obstacles.

### 🔧 Key Parameters:
| Parameter | Description |
| --- | --- |
| `Radius` / `Height` | Agent collider size for passability |
| `Speed` | Maximum movement speed | 
| `Angular Speed` | Rotation speed | 
| `Acceleration` | Acceleration |
| `Stopping Distance` | Distance to target at which agent stops |
| `Auto Braking` | Automatic braking before target |
| `Area Mask` | Which area types are accessible to the agent |

### 📝 Example of Controlling an Agent:
```csharp
using UnityEngine;
using UnityEngine.AI;

public class AgentMover : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform target;
    public Camera playerCamera;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 5f;
        agent.stoppingDistance = 0.5f;
    }

    void Update()
    {
        // Move to the specified target
        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        // Mouse click movement
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }

        // Display movement status
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("Agent reached target!");
        }
    }

    public void StopAgent()
    {
        agent.isStopped = true;
    }

    public void ResumeAgent()
    {
        agent.isStopped = false;
    }
}
```

### 📝 Example with Animation:
```csharp
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class AnimatedAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("SpeedPercent", speedPercent);
    }
}
```

---

## 3. NavMesh Obstacle — Dynamic Obstacles
NavMesh Obstacle is used for objects that can move, appear, or disappear during gameplay (crates, doors, enemies). 
These objects must be considered by agents during navigation.

### 🔧 Two Operating Modes:
| Mode | Description | When to Use |
| --- | --- | --- |
| Blocking (Carve: off) | Agents dynamically avoid the obstacle (RVO) | Constantly moving objects (other agents) |
| Carving (Carve: on) | The obstacle cuts a "hole" in the NavMesh | Objects that may become stationary (crates, gates) |

### 📝 NavMeshObstacle Setup Example:
```csharp
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class MovingObstacle : MonoBehaviour
{
    private NavMeshObstacle obstacle;
    private Vector3 startPosition;
    public float moveDistance = 3f;
    public float speed = 2f;
    private bool movingRight = true;

    void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        startPosition = transform.position;
        
        // Enable carving for stationary obstacles
        obstacle.carving = true;
        obstacle.carvingMoveThreshold = 0.5f;
    }

    void Update()
    {
        float newX = startPosition.x + (movingRight ? moveDistance : -moveDistance);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        
        if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance)
        {
            movingRight = !movingRight;
        }
    }

    void OnDisable()
    {
        if (NavMeshSurface.activeSurface != null)
            NavMeshSurface.activeSurface.UpdateNavMesh();
    }
}
```

---

## 4. NavMesh Link and OffMesh Link — Special Connections
### 🚪 OffMesh Link
Used to connect two points where there is no normal path: jumping over chasms, climbing stairs, opening doors.

Setup: Create an empty GameObject → add OffMesh Link component → set Start and End points.
```csharp
using UnityEngine.AI;

public class DoorLink : MonoBehaviour
{
    public OffMeshLink doorLink;
    public bool isOpen = false;

    void Update()
    {
        doorLink.activated = isOpen;
    }
}
```

### 🌉 NavMesh Link
Connects multiple separate NavMesh Surfaces (for large open worlds or dungeons).

---

## 5. Complete Example: Patrolling Enemy
```csharp
using UnityEngine;
using UnityEngine.AI;

public class PatrollingEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    public float patrolSpeed = 3f;
    public float chaseSpeed = 7f;
    public float chaseRange = 10f;
    public Transform player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToNextWaypoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer < chaseRange)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.speed = patrolSpeed;
            
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNextWaypoint();
            }
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }
}
```

---

## 🎯 Optimization Tips
1. For large levels use NavMeshSurface with `Tile Size` configured for faster baking.
2. Dynamic obstacles — enable `Carve` only if the object can be stationary for some time.
3. Area Mask — limit area types for different agents (e.g., enemies can't enter water).
4. Checking goal arrival — use `agent.remainingDistance <= agent.stoppingDistance`, but also check `agent.pathPending == false` and `agent.hasPath == true`.

---

### ⭐ If this project was useful, put a star on GitHub!
