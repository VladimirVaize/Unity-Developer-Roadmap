# 🎬 Animation in Unity: Animator Controller and State Machine

> [!Note]
> This material covers Unity's animation system, specifically the Animator Controller, State Machine, and Parameters.
> These tools allow you to create complex, responsive character and object behavior — from simple switching between
> "Idle" and "Run" to dynamic transitions influenced by speed, health, or player actions.

---

## 🧠 What is an Animator Controller?
An Animator Controller is a graphical asset that manages an object's animations. It contains:
- A State Machine — a set of states and transitions between them.
- Parameters — variables that control transitions (speed, health, jump trigger, etc.).

To use an Animator Controller, attach an `Animator` component to your GameObject and assign the controller to it.

---

## 🔁 State Machine
A State Machine is a diagram where each State is one animation clip (e.g., `Idle`, `Walk`, `Run`, `Jump`), and Transitions define when and how one state changes to another.

### Key concepts:
- Any State — a special state that can transition to any other state from any current state (useful for events like `Death` or `Jump`).
- Entry — the entry point into the state machine (usually leads to the default state, e.g., `Idle`).
- Exit — exit point (rarely used, mostly for sub-state machines).

### How to create and configure:
1. Create an Animator Controller: right-click in the `Project` window → `Create` → `Animator Controller`.
2. Double-click it to open the `Animator` window.
3. Drag animation clips (e.g., `Idle`, `Walk`, `Run`) from the `Project` window into the `Animator` window — they become states (rectangles with names).
4. Set the default state (orange color): right-click on a state → `Set as Layer Default State`.
5. Create a transition: right-click on a state → `Make Transition`, then click on the target state.

### Example:
You have `Idle` and `Walk`. You want the character to walk when speed > 0.1. 
Create a transition from `Idle` to `Walk` and set the condition `Speed > 0.1`. 
Also, create a transition back from `Walk` to `Idle` with the condition `Speed < 0.1`.

---

## 🎛️ Parameters
Parameters are variables that you can set from scripts (using `Animator.SetFloat()`, `SetBool()`, `SetInteger()`, `SetTrigger()`). They define transition conditions.

### Parameter types:
| Type | Description | Example usage |
|-------|--------------|-----------------------|
| Int | Integer number (0, 1, 2, -5) | Weapon type (0 — sword, 1 — bow, 2 — staff) |
| Float | Floating-point number | Movement speed, rotation angle |
| Bool | True or false | IsGrounded, IsDead |
| Trigger | Like Bool, but auto-resets to false after use | Jump, Attack, TakeDamage |

### How to create a parameter:
1. In the `Animator` window, go to the `Parameters` tab (left panel) and click `+`.
2. Choose the type (`Float`, `Int`, `Bool`, `Trigger`).
3. Name it (e.g., `Speed`, `IsJumping`, `Attack`).

---

## 🔀 Transitions and Conditions
A transition defines: from which state → to which state. A transition has Conditions — checks on parameters.

### How to configure a transition:
1. Click on the transition arrow between two states.
2. In the Inspector, find the `Conditions` section.
3. Click `+`, select a parameter, and set a value.
   - For `Float`: `Greater` / `Less` (e.g., `Speed > 0.1`)
   - For `Int`: `Equals` / `Not Equal` (e.g., `WeaponType Equals 1`)
   - For `Bool`: `True` / `False`
   - For `Trigger`: just having the trigger (no value)
  
### Additional transition settings:
- Has Exit Time — if enabled, the transition will only occur after the animation finishes (clip length). Often disabled for responsive transitions like Idle→Run.
- Transition Duration — cross‑fade time between animations (default ~0.1s for smoothness).
- Conditions — a list of conditions. If multiple, all must be true for the transition to happen.

### Trigger example:
1. Create a `Jump` parameter of type `Trigger`.
2. In your script when jumping, call `animator.SetTrigger("Jump")`.
3. Create a transition from `Any State` to the `Jump` state.
4. In the transition's condition, select `Jump`.
5. Now at any moment (Idle, Walk, Run) when the trigger is called, the jump animation will play.

---

## 🧩 Practical example structure
Imagine a character with animations: `Idle`, `Walk`, `Run`, `Jump`, `Attack`.

### Parameters:
- `Speed` (Float) — from 0 to 10 (movement speed)
- `IsGrounded` (Bool) — whether standing on ground
- `Attack` (Trigger) — attack
- `Jump` (Trigger) — jump

### Transition logic:
- `Idle` → `Walk`: `Speed > 0.1`
- `Walk` → `Idle`: `Speed < 0.1`
- `Walk` → `Run`: `Speed > 5.0`
- `Run` → `Walk`: `Speed < 5.0`
- `Any State` → `Jump`: `Jump` (Trigger)
- `Any State` → `Attack`: `Attack` (Trigger)
- `Jump` → `Idle`: `IsGrounded == true` (after landing)

---

## 💡 Tips
- Use Any State with caution — it interrupts the current animation instantly, which is good for jumps but may be undesirable for attacks (better to transition from specific states).
- For blending animations (e.g., head turning), use Layers and Avatar Masks — an advanced topic.
- In scripts, update parameters in `Update()`: `animator.SetFloat("Speed", currentSpeed);`

---

### ⭐ If this project was useful, put a star on GitHub!
