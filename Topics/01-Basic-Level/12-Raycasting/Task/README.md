# 🎯 Practical Task: "Interactive Ray" System

## 📝 Task Description
You need to create a simple but functional interaction system using a raycast. 
The player controls a first-person camera, aims a crosshair (center of the screen) at interactive objects, and interacts with them using the `E` key.

---

## ✅ Requirements
1. **Ray from camera center** — every time `E` is pressed, cast a ray from the screen center (as in shooters).
2. **LayerMask** — the ray must interact only with objects on the `"Interactable"` layer. All other objects (walls, floor, triggers) are ignored.
3. **RaycastHit** — upon a hit, the ray must:
   - Get the `IInteractable` component from the object.
   - Call the `Interact()` method.
   - Print to the console: `"Interacted with {object name} at distance {distance}"`.
4. **Visualization** — draw the ray using `Debug.DrawRay`:
   - Green on hit (duration 0.5 sec).
   - Red on miss (duration 0.5 sec).
5. **IInteractable interface** — implement it in two types of objects:
   - `Door` (door): opens/closes (any logic: `Debug.Log` or animation).
   - `LightSwitch` (switch): turns the light on/off (changes the intensity of a point light).
  
---

## 🧱 Starter Code (Template)
```csharp
// IInteractable.cs
public interface IInteractable
{
    void Interact();
}

// Interactor.cs (attached to the camera)
public class Interactor : MonoBehaviour
{
    public float maxDistance = 5f;
    public LayerMask interactableLayer; // Assign the "Interactable" layer in the Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CastInteractionRay();
        }
    }

    void CastInteractionRay()
    {
        // 1. Create a ray from the center of the screen
        
        // 2. Raycast with LayerMask
        
        // 3. If hit — get IInteractable and call Interact()
        
        // 4. Debug.DrawRay (green/red)
    }
}

// Door.cs
public class Door : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    
    public void Interact()
    {
        isOpen = !isOpen;
        Debug.Log($"Door is {(isOpen ? "open" : "closed")}");
        // Additional: rotate the door, play sound, etc.
    }
}

// LightSwitch.cs
public class LightSwitch : MonoBehaviour, IInteractable
{
    public Light targetLight;
    private bool isOn = true;
    
    public void Interact()
    {
        isOn = !isOn;
        targetLight.intensity = isOn ? 1f : 0f;
        Debug.Log($"Light is {(isOn ? "on" : "off")}");
    }
}
```

---

## 🧪 Testing the Functionality
1. Create a scene: floor, walls, camera (FPS controller).
2. Add two cubes: one as `Door`, the second as `LightSwitch`.
3. Assign them the `"Interactable"` layer.
4. Attach the appropriate scripts (`Door`, `LightSwitch`) and set up references.
5. Run the scene, aim at the objects (crosshair in the center of the screen) and press `E`.
6. Check the console and ray visualization in the Scene View.

---

## 🔥 Bonus Task (Optional)
- Add highlighting to the object the crosshair is over (change material or add an Outline).
- Implement interaction sound (`AudioSource.PlayOneShot`).
- Make `IInteractable` have a `string InteractionPrompt` property (e.g., `"Press E to open the door"`) and display it on the UI.

---

### ⭐ If this project was useful, put a star on GitHub!
