# 🎯 Task: «Smart Enemy System with Three States»
You are developing a stealth action game. You need to implement an AI enemy with three behavior modes:
1. 🧘 Patrol — Enemy moves between designated points on the NavMesh
2. 🔍 Investigate — Enemy moves to where the player made a noise (e.g., dropped an item)
3. 🏃‍♂️ Chase — Enemy pursues the player upon sight

### 📁 Scene Structure:
- Terrain: Maze of walls with NavMesh Surface (baked)
- Player: Object with Collider and script that creates "noise" when pressing Space
- Enemy: Cube with NavMesh Agent and your `SmartEnemy` script
- Patrol Points: 4 empty objects (Waypoint1-4)

### 📝 SmartEnemy Class — What to Implement:
```csharp
public enum EnemyState { Patrol, Investigate, Chase }

public class SmartEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 3f;
    
    [Header("Chase Settings")]
    public float chaseSpeed = 6f;
    public float chaseRange = 12f;
    public float lostPlayerTime = 5f;
    
    [Header("Investigate Settings")]
    public float investigateTime = 4f;
    public float investigateSpeed = 4f;
    
    [Header("References")]
    public Transform player;
    
    private NavMeshAgent agent;
    private EnemyState currentState;
    private Vector3 lastKnownPlayerPosition;
    private Vector3 investigatePoint;
    private float stateTimer;
    private int currentPatrolIndex;
}
```

### 📋 State Requirements:
| State | Entry Condition | Behavior |
| --- | --- | --- |
| Patrol | Default / investigate timer expired / lost player | Moves to waypoints in order |
| Investigate | Received "noise" event from player | Moves to noise point, waits, returns to patrol |
| Chase | Player enters `chaseRange` | Chases player; if hidden > `lostPlayerTime` → return to patrol |

### 🧰 Additional Requirements:
1. Patrol waypoint switching — after reaching the current waypoint, switch to the next one
2. Noise event — create `public void ReceiveNoise(Vector3 noisePosition)` that transitions to `Investigate`
3. Visual debugging — log the current state and actions to the console
4. Dynamic obstacle — add a moving crate with `NavMeshObstacle` that the enemy must avoid

### 🔍 Expected Behavior:
```text
[Enemy] State: Patrol → Moving to Waypoint 1
[Enemy] State: Patrol → Moving to Waypoint 2
[Player] Made noise at position (5, 0, 3)
[Enemy] State: Investigate → Moving to noise position (5, 0, 3)
[Enemy] State: Investigate → Arrived, investigating...
[Enemy] State: Patrol → Returning to patrol route
[Enemy] State: Chase → Player detected, chasing!
[Player] Left chase range, Enemy searching...
[Enemy] State: Chase → Lost player, returning to patrol
```

### 💡 Hints:
- Use `agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending` to check destination arrival
- Use `stateTimer` and `Time.deltaTime` for waiting in `Investigate` state
- Check priority for interrupting states: Chase > Investigate > Patrol

---

### ⭐ If this project was useful, put a star on GitHub!
