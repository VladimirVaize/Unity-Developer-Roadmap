# 🎯 Practical Task: «Build a Robot from Components»

## Goal

Learn to distinguish between a GameObject (container) and Components (behaviors/parts). Create a working object by adding and removing components.

## Task

### Part 1. Create an empty container

1. Open Unity (any 2020+ version)
2. In the Hierarchy window → Right-click → `Create Empty`
3. Name the object `MyRobot`
4. Look in the Inspector: what components are already there? (only `Transform`)

### Part 2. Add visual behavior (components)

1. Select `MyRobot`
2. Click `Add Component` → `Mesh Filter` → choose `Cube` (or `Sphere`)
3. Add `Mesh Renderer` (if not added automatically)
4. Create a material: Right-click in Project → `Create` → `Material`, name it `RobotMat`, color it red
5. Drag `RobotMat` onto `MyRobot` (or assign it in `Mesh Renderer` → `Element 0`)

➡️ Now `MyRobot` is visible. But it doesn't move or interact.

### Part 3. Add physics behavior

1. `Add Component` → `Rigidbody`
2. `Add Component` → `Box Collider` (automatically fits the cube's size)

➡️ Press Play. The object will fall down due to gravity. Stop the game.

### Part 4. Add logical behavior (script)

1. Create a C# script: Right-click in Project → `Create` → `C# Script`, name it `Rotator`
2. Open the script (double-click) and write:

```csharp
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 30f;

    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
```

3. Save the script. Drag it onto `MyRobot` in the Inspector (or onto the object in Hierarchy)

### Part 5. Experiments (checking understanding)

Perform each step one by one and observe the result:

1. Disable `Mesh Renderer` (uncheck the box) → the object becomes invisible but still rotates (if Play is on) — because `Rotator` is still working.
2. Disable `Rotator` → rotation stops.
3. Remove `Rigidbody` (three dots → Remove Component) → gravity and physics are gone.
4. Create a second object (`Create Empty` → name it `MyRobot2`) and drag the existing `Rotator` script from the Project window onto it. It will also rotate.

### Part 6. Answer the questions (verbally or as code comments)

- How many GameObjects are in the scene after Part 5? (hint: every object in Hierarchy is a GameObject)
- Can you add two identical `Rigidbody` components to one GameObject? (try it)
- What happens if you delete the `Transform` component?

> ✅ Success criteria: You can create an object, make it visible, physical, with rotation logic — and explain which part is the container (GameObject) and which are components.

---

### ⭐ If this project was useful, put a star on GitHub!
