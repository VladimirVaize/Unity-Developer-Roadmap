# 🎯 Practical Task: Creating an Animator Controller for a Character with Run, Jump, and Attack

## Objective
Learn how to create an Animator Controller, configure a State Machine, and use all parameter types (Float, Bool, Trigger) to control character animations.

---

## Description
You need to create an animation controller for a simple character with three basic actions: Idle, Run, Jump, and Attack.

---

## Requirements

### 1. Animation Clips
Assume the following animation clips are already imported into your project:
- `Idle` — character standing still
- `Run` — character running
- `Jump` — character jumping
- `Attack` — character performing an attack

### 2. Animator Controller Parameters
Create the following parameters in your controller:
| Parameter Name | Type | Purpose |
|------------|-------|-----------------|
| `Speed` | Float | Movement speed (0 to 10) |
| `IsGrounded` | Bool | Whether the character is on the ground |
| `Jump` | Trigger | Initiates a jump |
| `Attack` | Trigger | Initiates an attack |

### 3. Transition Logic (configure in the State Machine)
#### Basic transitions:
- `Idle` → `Run`: when `Speed > 0.1`
- `Run` → `Idle`: when `Speed < 0.1`

#### Jump (from any state):
- `Any State` → `Jump`: condition `Jump` (Trigger)
- `Jump` → `Idle`: condition `IsGrounded == true`

#### Attack (only from Idle and Run):
- `Idle` → `Attack`: condition `Attack` (Trigger)
- `Run` → `Attack`: condition `Attack` (Trigger)
- `Attack` → `Idle`: after the attack animation finishes (enable `Has Exit Time`)

### 4. Optional (extra challenge)
- If the character attacks while airborne (`IsGrounded == false`), the attack should not interrupt the jump (make the `Attack` transition only from `Idle` and `Run`, not from `Jump`).
- Add a slight delay or use `Transition Duration` to make the attack feel responsive.

---

## What you need to do in Unity
1. Create an Animator Controller named `PlayerAnimator`.
2. Add states `Idle`, `Run`, `Jump`, `Attack` (by dragging the clips into the Animator window).
3. Create the parameters (`Speed`, `IsGrounded`, `Jump`, `Attack`).
4. Create all transitions according to the logic above.
5. Write a simple C# script that:
   - Stores a reference to the `Animator`.
   - In `Update()`, reads keyboard input:
     - `W, S, A, D` or `Horizontal` / `Vertical` — changes `Speed`.
     - Spacebar — calls `SetTrigger("Jump")`.
     - Left Ctrl or mouse — calls `SetTrigger("Attack")`.
    
   - Example code:
  
```csharp
void Update()
{
    float moveInput = Input.GetAxis("Vertical");
    float speed = Mathf.Abs(moveInput) * 10f;
    animator.SetFloat("Speed", speed);
    
    if (Input.GetButtonDown("Jump"))
        animator.SetTrigger("Jump");
    
    if (Input.GetKeyDown(KeyCode.LeftControl))
        animator.SetTrigger("Attack");
        
    // For demonstration, hardcode IsGrounded = true (if no physics)
    animator.SetBool("IsGrounded", true);
}
```

6. Attach the `Animator` component to your character and assign the created controller.
7. Run the scene and verify:
   - Pressing `W/S` switches between Idle and Run.
   - Pressing Space plays Jump and returns to Idle.
   - Pressing Ctrl plays Attack from Idle or Run and returns back.
  
---

## Success Criteria
- ✅ All states switch correctly.
- ✅ Parameters affect transitions.
- ✅ The `Jump` and `Attack` triggers fire when called from the script (and do not repeat continuously).
- ✅ The `Jump → Idle` transition only happens after `IsGrounded == true`.

---

## Expected Result
An animated character that responds to keyboard input, switching animations smoothly without errors.

---

### ⭐ If this project was useful, put a star on GitHub!
