# ⚙️ State Pattern: Implementing a Finite State Machine for Player, Enemy, or UI

This material covers the State Pattern — one of the key behavioral patterns in game development. 
You'll learn what a Finite State Machine (FSM) is, why it's needed for controlling players, enemies, 
and UI, and how to implement it in Unity using C#.

---

## 1. 🧠 What is the State Pattern?
### Purpose:
The State Pattern allows an object to change its behavior when its internal state changes. 
Instead of huge conditional constructs (`if` / `switch`), you extract each state into a separate class. 
This makes the code clean, extensible, and easy to debug.

### Real-life example:
A character can be in states: `Idle`, `Walk`, `Run`, `Jump`, `Attack`. 
In each state, it has different movement logic, available actions, and transitions.

---

## 2. 🎮 Finite State Machine (FSM)
### Definition:
An FSM is a behavior model that is exactly in one state at any given time from a finite set. 
Transitions between states occur based on events or conditions.

### Main components of an FSM:
| Component | Description |
| --- | --- |
| State | Specific behavior (e.g., `IdleState`, `RunState`). |
| Transition | Rule for when and to which state to switch (e.g., if speed > 0 → switch to `RunState`). |
| Event | Trigger for a transition (key press, reaching a target, taking damage). |
| State Machine | Container that holds the current state and manages transitions. |

---

## 3. 🧱 Implementation in Unity (basic example)
### 📁 Class structure
```csharp
// Base abstract class for all states
public abstract class IState
{
    public virtual void Enter() { }        // What to do on entering the state
    public virtual void Update() { }       // Logic every frame
    public virtual void FixedUpdate() { }  // For physics
    public virtual void Exit() { }         // What to do on exiting the state
}

// Simple state machine
public class StateMachine
{
    private IState _currentState;
    
    public void ChangeState(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }
    
    public void Update()
    {
        _currentState?.Update();
    }
    
    public void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }
}
```

### 👤 Example: player states
```csharp
public class PlayerController : MonoBehaviour
{
    private StateMachine _stateMachine;
    
    public IdleState IdleState;
    public WalkState WalkState;
    public JumpState JumpState;
    
    private void Awake()
    {
        _stateMachine = new StateMachine();
        
        // Create states (pass reference to player and state machine)
        IdleState = new IdleState(this, _stateMachine);
        WalkState = new WalkState(this, _stateMachine);
        JumpState = new JumpState(this, _stateMachine);
    }
    
    private void Start()
    {
        _stateMachine.ChangeState(IdleState);
    }
    
    private void Update()
    {
        _stateMachine.Update();
    }
    
    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }
}

// Concrete state: Idle
public class IdleState : IState
{
    private PlayerController _player;
    private StateMachine _fsm;
    
    public IdleState(PlayerController player, StateMachine fsm)
    {
        _player = player;
        _fsm = fsm;
    }
    
    public override void Enter()
    {
        Debug.Log("Entered Idle");
        // Stop movement animation
    }
    
    public override void Update()
    {
        float input = Input.GetAxis("Horizontal");
        
        if (Mathf.Abs(input) > 0.1f)
        {
            _fsm.ChangeState(_player.WalkState);
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            _fsm.ChangeState(_player.JumpState);
        }
    }
}

// WalkState, JumpState, etc. are implemented similarly
```

---

## 4. 🎯 Usage for different entities
### 🧝 Player
States: `Idle`, `Walk`, `Run`, `Jump`, `Fall`, `Attack`, `Hurt`, `Die`
```csharp
// Example transition by condition (in AttackState)
public override void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        _fsm.ChangeState(_player.AttackState);
    }
}
```

### 👾 Enemy
States: `Patrol`, `Chase`, `Attack`, `Hurt`, `Die`
```csharp
public class ChaseState : IState
{
    public override void Update()
    {
        // Move towards the player
        Vector3 direction = (_player.transform.position - _enemy.transform.position).normalized;
        _enemy.transform.position += direction * _enemy.speed * Time.deltaTime;
        
        // If close → attack
        if (Vector3.Distance(_enemy.transform.position, _player.transform.position) < _enemy.attackRange)
        {
            _fsm.ChangeState(_enemy.AttackState);
        }
        
        // If player is lost → return to patrol
        if (!_enemy.CanSeePlayer())
        {
            _fsm.ChangeState(_enemy.PatrolState);
        }
    }
}
```

### 🖥️ UI (menus, screens)
States: `MainMenu`, `SettingsMenu`, `PauseMenu`, `GameOverMenu`, `InGameUI`
```csharp
public class UIManager : MonoBehaviour
{
    private StateMachine _uiStateMachine;
    
    private void Start()
    {
        _uiStateMachine.ChangeState(new MainMenuState(this));
    }
    
    public void OnPausePressed()
    {
        _uiStateMachine.ChangeState(new PauseMenuState(this));
    }
}
```

---

## 5. 🚀 Advanced techniques
### 🔁 Event-driven transitions
Instead of checking in `Update()`, use events from other systems:
```csharp
// In HealthComponent
public event Action OnHealthZero;

void TakeDamage()
{
    health--;
    if (health <= 0) OnHealthZero?.Invoke();
}

// In HurtState
public override void Enter()
{
    _player.Health.OnHealthZero += () => _fsm.ChangeState(_player.DieState);
}
```

### 🧩 Hierarchical FSM
States can contain nested state machines. For example, `GroundState` (on ground) includes `Idle`, `Walk`, `Run`, while `AirState` includes `Jump`, `Fall`, `DoubleJump`.

### 📦 Ready-made solutions for Unity
| Name | Description |
| --- | --- |
| Unity Animator (Mecanim) | Built-in FSM for animations (parameters, transitions, layers). |
| State Machine Behaviour | Scripts attached to Animator states. |
| GitHub: Runtime State Machine | Popular implementations (e.g., `State Machine` by Inspiaaa). |

---

## 6. ✅ Pros and cons of the State Pattern
| Pros | Cons |
| --- | --- |
| ✅ Clean code (no spaghetti `if/else`) | ❌ Increased number of classes |
| ✅ Easy to add new states | ❌ Slight overhead from method calls | 
| ✅ Convenient unit testing of each state | ❌ May be overkill for simple objects |
| ✅ Clear architecture | ❌ Complexity with many transitions |

---

### ⭐ If this project was useful, put a star on GitHub!
