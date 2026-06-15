# 🎯 Task: «Dungeon Optimization via Occlusion Culling»
You are developing a 3D game in the "dungeon quest" genre. The scene consists of 50+ rooms connected by corridors. 
Without optimization, FPS drops to 15-20. Your task is to implement Occlusion Culling to improve performance.

### 📝 Initial Data:
- Scene contains: walls (occluders), chests, enemies, decorative objects
- Camera is first-person, moves through the dungeon
- Player can open doors and activate portals

---

## 📋 Part 1: Scene Preparation (5 steps)
### 1. Mark static objects:
- All walls, floors, ceilings, columns — enable `Static` flag
- Ensure they have `MeshRenderer` with shadows enabled (`Cast Shadows = On`)

### 2. Dynamic objects:
- Chests, enemies, lamps — leave NON-static (`Static` flag disabled)

### 3. Create Occlusion Areas:
- For each large room, add an `Occlusion Area`
- Adjust area size to match the room size

### 4. Add Occlusion Portals:
- For each doorway, add an `Occlusion Portal` component
- Adjust rotation so the portal "looks" into the opening

### 5. Check tags:
- Occluders (walls) → assign tag `Occluder`
- Occludees (decor) → assign tag `Occludee`

---

## 📋 Part 2: Baking Configuration
6. Open Occlusion Culling window (Window → Rendering → Occlusion Culling)
7. Set baking parameters:
```text
Smallest Occluder = 1.5
Smallest Hole = 0.5
Backface Threshold = 100
```
8. Click Bake
9. After baking, verify that `OcclusionCullingData.asset` appears in the `Assets/` folder

---

## 📋 Part 3: Scripting Control (write code)
You need to implement the following scripts:
### 🔧 Script 1: `OcclusionDebugger.cs`
Outputs statistics to the console every 2 seconds:
```text
[Occlusion] Visible objects: 45 / 320 (14% culled)
[Occlusion] Current FPS: 58.2
```

Requirements:
- Use `Renderer.isVisible` for counting
- Use platform directive `#if UNITY_EDITOR` for editor output

### 🚪 Script 2: `DynamicPortalController.cs`
Controls the Occlusion Portal on a door:
- When door opens — portal opens
- When door closes — portal closes
- Log when portal toggles
```csharp
// Structure hint:
public class DynamicPortalController : MonoBehaviour
{
    public OcclusionPortal portal;
    public KeyCode openKey = KeyCode.E;
    
    void Update()
    {
        // TODO: On key press, open/close portal
        // TODO: Log "Portal opened/closed"
    }
}
```

### 🏆 Script 3: `RoomOcclusionTrigger.cs`
Triggers when entering a room:
- Enables dynamic objects in this room
- Disables dynamic objects in other rooms (distance > 20 meters)

Algorithm:
1. When entering room trigger, get all `GameObject` with tag `RoomObject` in this room
2. For each object: `SetActive(true)`
3. For objects in other rooms: `SetActive(false)` (except where player is)

---

## 📋 Part 4: Visualization and Testing
10. In Occlusion Culling window → Visualization tab, enable Visualize → Occlusion Culling
11. Select the player camera in the Camera field
12. Verify:
    - In the corridor — are red objects (hidden) visible behind walls?
    - In a room with an enemy — enemy should be green (visible)
    - When leaving a room — enemy should become red
   
13. Measure FPS before and after:
```text
Without OC: 18 FPS
With OC:    52 FPS
```

---

## 📋 Part 5: Advanced Task (Bonus)
Implement adaptive baking for dynamically changing scenes:
- When a secret door opens, rebake the area around it
- Use `StaticOcclusionCulling.GenerateInBackground()`
- Show a loading indicator (UI spinner)
```csharp
public class AdaptiveBaking : MonoBehaviour
{
    public void OnSecretDoorOpened()
    {
        #if UNITY_EDITOR
        // Run rebaking in background
        UnityEditor.Rendering.StaticOcclusionCulling.GenerateInBackground();
        StartCoroutine(ShowBakingProgress());
        #endif
    }
    
    System.Collections.IEnumerator ShowBakingProgress()
    {
        // TODO: Show UI indicator
        yield return new WaitForSeconds(2f);
        Debug.Log("Baking complete!");
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
