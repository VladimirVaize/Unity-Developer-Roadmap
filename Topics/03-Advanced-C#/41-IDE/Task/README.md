# 🧪 Practical Task: Debugging a Movement Script with Breakpoints

## 📌 Task Description
You have created a simple movement script for a character in Unity. 
The script works, but the character moves too fast and goes through walls due to logic errors. 
Your task is to use breakpoints, the Immediate Window, and Attach to Unity to find and fix the errors.

### Original code (with bugs)
```csharp
using UnityEngine;

public class BuggyMovement : MonoBehaviour
{
    public float speed = 50f;       // Value too high
    public float jumpForce = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Bug: rb could be null if the component is missing
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(move, 0, 0);

        if (Input.GetButtonDown("Jump"))
        {
            // Bug: jump is applied every frame (should be in FixedUpdate)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
```

### Expected behavior
- The character moves at a comfortable speed (e.g., 10 units per second).
- The jump occurs once without accumulating multiple times.
- The character does not pass through walls (colliders work).

### Current (buggy) behavior
- The character is way too fast.
- The jump may trigger multiple times per single press.
- The character passes through walls due to using `transform.Translate` instead of physics.

---

## 🎯 Goal
1. Configure your IDE (Visual Studio or Rider) for debugging with Unity.
2. Set breakpoints in problematic parts of the code.
3. Use the Immediate Window to change variables during a pause.
4. Apply Attach to Unity (debugging without restarting the game).
5. Fix the errors.

---

## 📝 Step-by-step instructions
### Step 1. Preparation
- Create a new 2D or 3D project in Unity.
- Add a Cube to the scene — this will be your character.
- Add a `Rigidbody` component (for physics).
- Create a wall (another Cube with a collider) in the character's path.
- Attach the `BuggyMovement` script to the Cube.

### Step 2. IDE configuration
- Make sure `Edit → Preferences → External Tools` points to your editor (VS or Rider).
- Open the script from Unity (double-click it in the Project window).

### Step 3. Run and observe (without debugging)
- Press Play in Unity.
- Try moving with arrow keys and jumping (spacebar).
- Note the problems: speed too high, strange jump, passing through the wall.

### Step 4. Debug with breakpoints
- Stop the game.
- Place a breakpoint on the line `transform.Translate(...)`.
- Run the game again (Play). When the character moves, execution will pause.
- Hover over the `move` variable — check its value.

### Step 5. Immediate Window
- Open the Immediate Window (`Ctrl + Alt + I` in VS, search for it in Rider).
- While the game is paused (breakpoint active), type:
  - `speed = 10;`
  - Press Enter.
 
- Press F5 (Continue) — the character will now move slower while the game is still running.

### Step 6. Attach to Unity (optional)
- Close the IDE (leave Unity open).
- Start the game in Unity.
- Open the IDE again, but do not let it open the project automatically.
- Choose `Debug → Attach to Unity Process` (or the equivalent in Rider).
- Now set a breakpoint in the jump (`rb.AddForce...`).
- Press space in the game — the breakpoint will hit, even though you didn't start debugging from the IDE.

### Step 7. Fix the errors
Correct the code so it works properly:
```csharp
using UnityEngine;

public class FixedMovement : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Rigidbody missing!");
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + Vector3.right * move);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
```
- Drag the corrected script onto the Cube (remove the old one).
- Make sure the wall has the tag `Ground` (create the tag in Unity).
- Run — the character now moves using physics, does not pass through walls, and jumps once.

---

## ✅ Success criteria
- You successfully set a breakpoint and the game paused.
- You changed the `speed` variable via the Immediate Window and saw the effect.
- You used `Attach to Unity` to debug without restarting.
- The final script works correctly: speed ≈10, single jump, no wall penetration.

---

### ⭐ If this project was useful, put a star on GitHub!
