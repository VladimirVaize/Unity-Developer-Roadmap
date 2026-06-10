# 🌳 Behavior Trees in Unity: NodeCanvas, Odin + NPBehave, Custom Solutions

Behavior Trees are a powerful tool for creating game AI. Unlike Finite State Machines (FSM), 
they are modular, easily scalable, and allow you to create complex, predictable behavior without "spaghetti code". 
This article covers three approaches: using ready-made assets (NodeCanvas), 
the Odin Inspector + NPBehave combo for visual editing, and writing a custom solution from scratch.

---

## 1. What is a Behavior Tree? Key Concepts
A Behavior Tree is a hierarchical structure of nodes. Each node returns one of three execution statuses to its parent:
| Status | Description |
| --- | --- |
| Success | The node completed its task successfully |
| Failure | The node failed to complete its task |
| Running | The node needs multiple frames to complete (movement, animation, waiting) |


### Node Types:
| Type | Description | Examples |
| --- | --- | --- |
| Composite | Have multiple children, control execution order | `Sequence`, `Selector`, `Parallel` |
| Decorator | Have one child, modify its result or behavior | `Inverter`, `Repeat`, `UntilFail` |
| Leaf (Action / Condition) | Leaf nodes that perform an action or check | `MoveTo`, `Attack`, `IsHealthLow` |
| Root | Root node, entry point of the tree | `Root` |

### Main Composites:
- `Sequence` (AND): Executes children in order. If a child returns `Failure` — the whole sequence stops and returns `Failure`. If all children return `Success` — returns `Success`.
- `Selector` (OR): Executes children in order. If a child returns `Success` — the Selector finishes with `Success`. If all children return `Failure` — returns `Failure`.
- `Parallel`: Runs all children simultaneously. Completes when all children are done (or on first success/failure, depending on settings).

---

## 2. Asset: NodeCanvas (Paid, De Facto Standard)
NodeCanvas is the most popular paid asset for visual behavior authoring. Supports Behavior Trees, Finite State Machines, and Dialogs. Costs about $45–60.
### Key NodeCanvas Features:
| Feature | Description |
| --- | --- |
| Visual Editor | Drag-and-drop nodes, create connections, inspector for parameters |
| Blackboard | Data storage (variables) accessible to all tree nodes (Int, Float, Vector3, GameObject, etc.) |
| Dynamic (Reactive) | Nodes can interrupt execution when conditions change (reactive behavior) |
| Graph Reference | Ability to nest one tree inside another (Nested BT) |
| Debugging | Real-time color-coded node status (green — Success, red — Failure, yellow — Running) |

### NodeCanvas Setup Example:
1. Add the `BehaviourTreeOwner` component to your GameObject.
2. Click `Open Behaviour Tree` and create nodes.
3. In `Blackboard`, add a `TargetPosition` (Vector3) variable.
4. Build the tree:
   - `Sequence` → `Conditional` (Check Health) → `Action` (Move To Position, using Blackboard variable)
  
5. In the `Action` node, select `Use Blackboard Variable` for Target Position.
6. Enable the `Dynamic` option on the `Sequence` to make the tree react immediately to condition changes.

### Custom ActionTask Example (NodeCanvas Script):
```csharp
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("MyActions")]
public class MoveToTarget : ActionTask<Transform>
{
    public BBParameter<Vector3> targetPosition; // Link to Blackboard
    public float speed = 5f;

    protected override void OnExecute()
    {
        // Called when the node starts
    }

    protected override void OnUpdate()
    {
        // Called every frame while the node is Running
        Vector3 newPos = Vector3.MoveTowards(agent.position, targetPosition.value, speed * Time.deltaTime);
        agent.position = newPos;

        if (Vector3.Distance(agent.position, targetPosition.value) < 0.1f)
        {
            EndAction(true); // Success
        }
    }
}
```

---

## 3. Odin Inspector + NPBehave Combo (Free, Visual Editor Potential)
NPBehave is a free behavior tree library in C# (code-driven approach). 
Odin Inspector is a paid Unity editor extension. 
Their combination allows you to create a visual editor for NPBehave.

### Why Odin + NPBehave?
- NPBehave is lightweight, performant, supports Blackboards, and has everything needed for BT.
- Odin allows you to create custom editor windows, inspectors, and serialization.
- Together — you get a visual behavior editor without purchasing NodeCanvas.

### Implementation Concept:
1. Extend NodeData Class: Create a base class for storing node data.
2. Canvas UI: Use Odin to draw nodes in a custom editor window.
3. Serialization: Save the tree as JSON or ScriptableObject.
4. Code Generation or Runtime Parsing: Parse the data at game start and build the NPBehave tree.

```csharp
// Example node data structure (Odin-compatible)
[System.Serializable]
public class NodeData
{
    public string nodeName;
    public Vector2 position;
    public List<NodeData> children = new List<NodeData>();
}

[CreateAssetMenu(fileName = "BehaviourTree", menuName = "AI/Behaviour Tree")]
public class BehaviourTreeAsset : ScriptableObject
{
    public List<NodeData> nodes = new List<NodeData>();
}
```

> [!Important]
> There is no ready-made "Odin + NPBehave" asset in the Asset Store — this is an architectural approach you implement yourself.
> Alternatively, use ready-made solutions like "NodeCanvas" or "Behaviour Tree Designer".

---

## 4. Custom C# Solution (Free, Full Control)
Writing your own BT framework gives you 100% control over performance and functionality.

### Basic Architecture:
```csharp
public enum NodeStatus
{
    Success,
    Failure,
    Running
}

public abstract class Node
{
    protected NodeStatus status;
    public NodeStatus Status => status;
    
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    
    public NodeStatus Evaluate()
    {
        if (status != NodeStatus.Running)
            OnEnter();
        
        status = OnUpdate();
        
        if (status != NodeStatus.Running)
            OnExit();
        
        return status;
    }
    
    protected abstract NodeStatus OnUpdate();
}
```

### Composite Nodes:

### Sequence (AND):
```csharp
public class Sequence : Node
{
    private List<Node> children = new List<Node>();
    private int currentChild = 0;
    
    public Sequence(params Node[] nodes)
    {
        children.AddRange(nodes);
    }
    
    protected override NodeStatus OnUpdate()
    {
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return NodeStatus.Success;
        }
        
        NodeStatus childStatus = children[currentChild].Evaluate();
        
        if (childStatus == NodeStatus.Failure)
        {
            currentChild = 0;
            return NodeStatus.Failure;
        }
        
        if (childStatus == NodeStatus.Success)
        {
            currentChild++;
            return currentChild < children.Count ? NodeStatus.Running : NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}
```

### Selector (OR):
```csharp
public class Selector : Node
{
    private List<Node> children = new List<Node>();
    private int currentChild = 0;
    
    public Selector(params Node[] nodes)
    {
        children.AddRange(nodes);
    }
    
    protected override NodeStatus OnUpdate()
    {
        while (currentChild < children.Count)
        {
            NodeStatus childStatus = children[currentChild].Evaluate();
            
            if (childStatus == NodeStatus.Success)
            {
                currentChild = 0;
                return NodeStatus.Success;
            }
            
            if (childStatus == NodeStatus.Running)
                return NodeStatus.Running;
            
            currentChild++;
        }
        
        currentChild = 0;
        return NodeStatus.Failure;
    }
}
```

### Action Node:
```csharp
public class MoveTo : Node
{
    private Transform agent;
    private Vector3 target;
    private float speed;
    
    public MoveTo(Transform agent, Vector3 target, float speed)
    {
        this.agent = agent;
        this.target = target;
        this.speed = speed;
    }
    
    protected override NodeStatus OnUpdate()
    {
        agent.position = Vector3.MoveTowards(agent.position, target, speed * Time.deltaTime);
        
        if (Vector3.Distance(agent.position, target) < 0.1f)
            return NodeStatus.Success;
        
        return NodeStatus.Running;
    }
}
```

### Usage Example:
```csharp
public class EnemyAI : MonoBehaviour
{
    private Node behaviourTree;
    
    void Start()
    {
        behaviourTree = new Sequence(
            new Condition(() => Health > 0),
            new Selector(
                new Condition(() => IsPlayerInRange),
                new MoveTo(transform, player.position, 5f)
            ),
            new Attack()
        );
    }
    
    void Update()
    {
        behaviourTree.Evaluate();
    }
}
```

### Blackboard — Shared Memory for Nodes:
```csharp
public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();
    
    public void Set<T>(string key, T value) => data[key] = value;
    public T Get<T>(string key) => data.ContainsKey(key) ? (T)data[key] : default;
    public bool HasKey(string key) => data.ContainsKey(key);
}
```

---

## 5. Approach Comparison
| Criteria | NodeCanvas | Odin + NPBehave | Custom Solution |
| --- | --- | --- | --- |
| Price | $45–60 | $45 (Odin) + free | Free |
| Visual Editor | ✅ Powerful, ready | 🔧 Need to build yourself	| ❌ No (code only) |
| Ready to Use | ✅ Out of the box | 🔧 Requires setup | 🔧 Requires coding |
| Performance | Good | Great | Great (optimized) |
| Flexibility | Limited by API | High (full code) | Maximum |
| Debugging	| ✅ Visual, color-coded | 🔧 Need to implement | 🔧 Need to implement |
| Support | Official, documentation | NPBehave community | Your own |

---

## 6. Recommendations
- NodeCanvas — if you need a powerful visual editor "out of the box" and are willing to pay.
- Odin + NPBehave — if you already own Odin, or want a visual editor at minimal cost (only your time).
- Custom Solution — if you want full control, don't want to pay, or need maximum performance.

---

### ⭐ If this project was useful, put a star on GitHub!
