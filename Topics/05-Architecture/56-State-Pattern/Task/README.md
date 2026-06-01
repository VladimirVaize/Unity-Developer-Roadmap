# 🧪 Practical Task: Finite State Machine for a Guard Enemy

Goal: Reinforce the implementation of the State Pattern (State Machine) using an enemy AI example. 
You will create a Finite State Machine for a guard with three states: `Patrol`, `Chase`, and `Attack`.

---

## 📥 Initial data
- A Unity 2D or 3D project (your choice).
- A simple cube or sprite for the player.
- A simple cube or sprite for the enemy.
- A `Rigidbody2D` / `Rigidbody` component for the enemy (optional).

---

## 🎯 Tasks
### Step 1: Create the basic State Machine structure
1. Create an `IState` script (abstract class or interface) with `Enter()`, `Update()`, `Exit()` methods.
2. Create a `StateMachine` script with `ChangeState()`, `UpdateCurrentState()` methods.
3. Create an `EnemyController` script that contains references to all states and an instance of `StateMachine`.

### Step 2: Implement the `Patrol` state
Requirements:
- The enemy moves between two points (or randomly around the scene).
- If distance to the player < 5 units → switch to `ChaseState`.
- Log "Patrolling" (or play an animation).

### Step 3: Implement the `Chase` state
Requirements:
- The enemy moves toward the player at 1.5x speed.
- If distance to the player < 1.5 units → switch to `AttackState`.
- If distance to the player > 8 units → switch back to `PatrolState`.

### Step 4: Implement the `Attack` state
Requirements:
- The enemy stops and "attacks" (logs `Attack!` to the console and deals 10 damage to the player every second).
- If the player dies (health ≤ 0) → switch to `PatrolState` or `IdleState`.
- If the player moves farther than 2 units → switch back to `ChaseState`.

### Step 5: Health system
1. Add a health component to the player (e.g., `PlayerHealth` with an int `health = 30` field).
2. In the `AttackState`, call `player.TakeDamage(10)` when attacking.
3. Implement a `DieState` (optional) that disables the enemy or logs a message.

---

## 🔧 Bonus requirements (⭐)
1. Animations: Add simple animations for each state (via `Animator` or sprite color changes).
2. Debugging: Visualize the current state using `TextMeshPro` or by changing the enemy's color.
3. UI: Create a player health bar.

---

## ✅ Success criteria
- The enemy starts in `PatrolState` and moves between points.
- When the player approaches, the enemy switches to `ChaseState` and speeds up.
- Upon contact, the enemy switches to `AttackState` and deals damage every second.
- When the player moves away, the enemy returns to `PatrolState`.
- (⭐) Animations change depending on the state.
- (⭐) The player's health bar updates when attacked.

---

## 💡 Hints
- Use `Vector3.Distance(transform.position, player.position)` to check distance.
- In `ChaseState`, you can use `MoveTowards` or `Rigidbody.velocity`.
- Remember to call `Exit()` to clean up timers and coroutines when switching states.
- If attack needs a delay, use `InvokeRepeating` or a `Coroutine` (but cancel them in `Exit()`).

---

### ⭐ If this project was useful, put a star on GitHub!
