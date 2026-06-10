# 🎯 Task: «Patrolling Enemy with Player Reaction»

You are developing a stealth game. You need to implement AI for an enemy using a behavior tree. The enemy must:
1. Patrol between three predefined points (`Transform[] patrolPoints`).
2. React to the player: if the player comes within `5` units — switch to chase mode.
3. Chase the player while the distance is greater than `2` units (attack range).
4. If the player is lost (distance > 10 units) — return to patrolling.
5. Visual debugging: enemy color changes: green = patrol, yellow = chase, red = attack.

---

## 📝 Implementation Requirements:
- Implement the tree using one of three approaches (your choice):
  - NodeCanvas (if you have the asset) — create a visual tree.
  - Custom solution — write `Sequence`, `Selector`, `Condition`, `Action` nodes.
  - Hybrid — use Odin + NPBehave or any other library.
 
- Blackboard: use it to store `playerTransform`, `patrolIndex`, `currentState`.
- Nodes:
  - `Condition` — check distance to player.
  - `Action` — patrolling (MoveToNextPatrolPoint).
  - `Action` — chase (ChasePlayer).
  - `Decorator` — `RepeatUntilSuccess` or `Inverter` for cyclic patrolling.
 
- Timing: patrolling should have a `1 second` pause at each point before moving to the next.

---

## 🧰 Additional Materials:
Below are code examples to get started with a custom solution. You can use them as a foundation.

### Blackboard Class (Shared Memory):
```csharp
using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();
    
    public void Set<T>(string key, T value) => data[key] = value;
    public T Get<T>(string key) => data.ContainsKey(key) ? (T)data[key] : default;
    public bool HasKey(string key) => data.ContainsKey(key);
}
```

### Base Node Class:
```csharp
public enum NodeStatus { Success, Failure, Running }

public abstract class Node
{
    protected NodeStatus status;
    public NodeStatus Status => status;
    protected Blackboard blackboard;
    
    public Node(Blackboard bb) { blackboard = bb; }
    
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    
    public NodeStatus Evaluate()
    {
        if (status != NodeStatus.Running) OnEnter();
        status = OnUpdate();
        if (status != NodeStatus.Running) OnExit();
        return status;
    }
    
    protected abstract NodeStatus OnUpdate();
}
```

### Example Action Node:
```csharp
public class MoveToPosition : Node
{
    private Transform agent;
    private float speed;
    private string targetKey;
    
    public MoveToPosition(Blackboard bb, Transform agent, float speed, string targetKey) : base(bb)
    {
        this.agent = agent;
        this.speed = speed;
        this.targetKey = targetKey;
    }
    
    protected override NodeStatus OnUpdate()
    {
        Vector3 target = blackboard.Get<Vector3>(targetKey);
        agent.position = Vector3.MoveTowards(agent.position, target, speed * Time.deltaTime);
        
        if (Vector3.Distance(agent.position, target) < 0.1f)
            return NodeStatus.Success;
        
        return NodeStatus.Running;
    }
}
```

### 🔍 Expected Result:
- The enemy correctly patrols, pausing for 1 second at each point.
- When the player approaches, the enemy switches to chase mode.
- When the player moves away (>10 m), the enemy returns to patrolling.
- State change logs are printed to the console.
- Visual debugging works (color changes).

---

## 💡 Hints:
- For the patrol pause, use a `Wait` node.
- For distance checking, use a `Condition` node with a lambda or method.
- For cyclic execution, use a `Repeat` decorator or loop in the root.

---

### ⭐ If this project was useful, put a star on GitHub!
