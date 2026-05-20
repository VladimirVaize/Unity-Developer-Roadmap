# 🛠️ Debug Visualization in Unity: Gizmos and Debug Drawing

> [!Note]
> When developing games, you often need to "see" the invisible: projectile trajectories, damage areas, attack radii, force directions, collider bounds, or AI behavior. 
> Unity provides powerful debug visualization tools that work directly in the Scene View (and sometimes Game View). 
> These are Gizmos and debug drawing methods. They don't affect the final build (if code is structured properly) and are indispensable during development.

---

## 🔷 1. Gizmos (Editor Drawing)

`Gizmos` is a class for drawing visual hints (spheres, cubes, rays, lines) in the Scene View. 
The code draws Gizmos only inside the Unity Editor (not in a built game) when using special event methods.

### 📌 Main Gizmos methods:

| Method                                                  | Description                             |
| ---                                                     | ---                                     |
| `Gizmos.DrawWireSphere(Vector3 position, float radius)` | Draws a wireframe sphere                |
| `Gizmos.DrawSphere(Vector3 position, float radius)`     | Draws a solid (semi-transparent) sphere |
| `Gizmos.DrawWireCube(Vector3 center, Vector3 size)`     | Wireframe cube                          |
| `Gizmos.DrawCube(Vector3 center, Vector3 size)`         | Solid cube                              |
| `Gizmos.DrawLine(Vector3 from, Vector3 to)`             | Line from point to point                |
| `Gizmos.DrawRay(Vector3 from, Vector3 direction)`       | Ray from a point in a direction         |
| `Gizmos.DrawIcon(Vector3 position, string iconName)`    | Draws an icon (e.g., "light.png")       |
| `Gizmos.DrawFrustum(...)`                               | Draws a camera's view frustum           |

### 🧠 The `OnDrawGizmos()` Event

This is a special method that Unity calls automatically every frame in the editor (not in play mode) for all scripts that define it.
```csharp
using UnityEngine;

public class EnemyGizmos : MonoBehaviour
{
    public float detectionRadius = 5f;
    public Color gizmoColor = Color.red;

    private void OnDrawGizmos()
    {
        // Draw a red wireframe sphere around the enemy
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
```

Result: You see a red circle around your enemy in the Scene View — this is its "player detection" zone.

### 🧠 The `OnDrawGizmosSelected()` Event
Draws Gizmos only when the object is selected. Useful to avoid cluttering the scene with thousands of hints.
```csharp
private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(transform.position + Vector3.up * 1f, new Vector3(2f, 2f, 2f));
}
```
Result: As long as you don't click on the object, the cube is invisible. When selected, a green cube appears above the object.

---

## 🔷 2. Debug Line and Ray Drawing (Runtime)

Unlike Gizmos, `Debug.DrawLine` and `Debug.DrawRay` work during Play Mode and are visible both in Scene View and sometimes in Game View (if Gizmos are enabled in Game). 
They are perfect for dynamic debugging: bullet trajectories, velocity direction, normal vectors, etc.

### 📌 Main Debug methods

| Method                                                                         | Description                                                                                                     |
| ---                                                                            | ---                                                                                                             |
| `Debug.DrawLine(Vector3 start, Vector3 end, Color color, float duration)`      | Draws a line from start to end for a given time (in seconds). If duration = 0, the line is visible for 1 frame. |
| `Debug.DrawRay(Vector3 start, Vector3 direction, Color color, float duration)` | Draws a ray from start in direction. Length of ray = magnitude(direction).                                      |
| `Debug.DrawRay(transform.position, transform.forward * 10, Color.blue)`        | Example: a blue ray of length 10 forward from the object.                                                       |

### 🧠 Example in a movement script
```csharp
void Update()
{
    Vector3 velocity = rb.velocity;
    // Draw a red ray of movement direction for 0.1 seconds
    Debug.DrawRay(transform.position, velocity, Color.red, 0.1f);
}
```
Result: During gameplay, a red ray shoots out of the object's center each frame, indicating the current velocity direction.

---

## 🔷 3. Visual Helpers in the Editor (Editor Scripting)
For complex tools, you can create custom editor scripts that draw Gizmos on any object. 
But the basic approach is using `OnDrawGizmos` inside `MonoBehaviour`.

### 💡 Useful settings:
- Gizmos in Game View: In the Game View dropdown → `Gizmos` → enable.
  Then lines and spheres will also be visible during gameplay right in the Game window (but be careful, this reduces performance).
- Colors: Use `Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);` — semi-transparent orange.
- Enable/disable: The `Gizmos` button in Scene View lets you hide all gizmos.

### 🧪 Summary: When to use which tool?

| Situation                                                        | Tool                               |
| ---                                                              | ---                                |
| Permanent visual hint in the editor (attack area, pickup radius) | `OnDrawGizmos`                     |
| Temporary line visible only when object is selected              | `OnDrawGizmosSelected`             |
| Runtime debugging (trajectories, rays, collisions)               | `Debug.DrawLine` / `Debug.DrawRay` |
| Custom icon in the editor                                        | `Gizmos.DrawIcon`                  |

---

### ⭐ If this project was useful, put a star on GitHub!
