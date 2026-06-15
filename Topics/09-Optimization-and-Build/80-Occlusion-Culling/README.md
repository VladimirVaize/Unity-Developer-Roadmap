# 👁️ Occlusion Culling in Unity: Disabling Rendering of Objects Hidden by Other Objects
Occlusion Culling is a rendering optimization technique where Unity disables rendering of objects completely blocked by other objects from the camera's perspective. 
This significantly reduces the number of drawn polygons and increases performance, especially in scenes with high detail levels.

> [!Important]
> ⚠️ Important Difference: Frustum Culling disables objects outside the camera's view, while Occlusion Culling disables objects within view but hidden behind other objects (e.g., a room behind a wall).

---

## 1. How Occlusion Culling Works
Occlusion Culling works in two stages:
| Stage | Description |
| --- | --- |
| 1. Baking | Precomputes visibility for the entire scene. Unity divides the scene into cells and builds a "visibility graph" between them |
| 2. Runtime | In real-time, the camera checks which objects should be visible and which are hidden, then disables rendering of hidden objects |

### 📊 Visual Example:
```text
Without Occlusion Culling:          With Occlusion Culling:
                               
    Camera                          Camera
      ↓                                ↓
   🧱 Wall                           🧱 Wall
      ↓                                ↓
   💣 Chest (rendered)               💣 Chest (NOT rendered)
   👾 Monster (rendered)             👾 Monster (NOT rendered)
```

---

## 2. Setting Up Occlusion Culling in Unity
### 🛠️ Step 1: Configure Objects for Occlusion
For an object to block others (be an occluder):
- Must be static (`Static` checkbox in Inspector)
- Must have a `MeshRenderer` with `Cast Shadows` enabled

For an object to be hidden (occludee):
- Must be static
- Must have a `MeshRenderer`

```csharp
// Example: Dynamically setting Static flag for occlusion
using UnityEngine;

public class OcclusionHelper : MonoBehaviour
{
    void Start()
    {
        // Make object static to participate in occlusion baking
        gameObject.isStatic = true;
        
        // If the object should be an occluder, enable shadows
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}
```

> [!Important]
> Only objects with the `Static` flag participate in occlusion baking. Dynamic (moving) objects are not considered but can be hidden by static occluders.

### 🛠️ Step 2: Occlusion Culling Window
Open: Window → Rendering → Occlusion Culling

The window has 3 tabs:
| Tab | Purpose |
| --- | --- |
| Object | Configure occlusion parameters for selected objects |
| Bake | Baking settings and `Bake` button |
| Visualization | Visualize baking results in the scene |

### 🛠️ Step 3: Configure Baking
In the Bake tab, configure the parameters:
| Parameter | Description | Recommendation |
| --- | --- | --- |
| Smallest Occluder | Minimum size of an object that can block others | 1-5 units |
| Smallest Hole | Minimum "hole" size through which an object can be seen | 0.25-1 units |
| Backface Threshold | Accuracy of backface consideration | 100 (default) |

```csharp
// Configuring bake parameters via script (Editor-only)
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class OcclusionSettings : MonoBehaviour
{
    [ContextMenu("Configure Occlusion Bake")]
    void ConfigureBake()
    {
        var staticOcclusionCulling = UnityEditor.Rendering.StaticOcclusionCulling;
        var settings = staticOcclusionCulling.occlusionCullingData;
        
        // Set parameters
        settings.smallestOccluder = 2.5f;
        settings.smallestHole = 0.5f;
        settings.backfaceThreshold = 100;
        
        EditorUtility.SetDirty(settings);
    }
}
#endif
```

### 🛠️ Step 4: Run Baking
Click the Bake button in the Occlusion Culling window. The process can take from a few seconds to several minutes depending on scene complexity.

After baking, the file `OcclusionCullingData.asset` appears in the `Assets/` folder.

---

## 3. Usage Examples
### 🏠 Example 1: Maze with Rooms
```csharp
using UnityEngine;

public class MazeOcclusion : MonoBehaviour
{
    void Start()
    {
        // All maze walls must be static
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            wall.isStatic = true;
        }
        
        // Enemies in rooms will be automatically hidden if not visible
        Debug.Log("Maze configured for Occlusion Culling");
    }
    
    void OnDrawGizmos()
    {
        // Visualize occlusion cells in the editor
        #if UNITY_EDITOR
        if (UnityEditor.Rendering.StaticOcclusionCulling.occlusionCullingData != null)
        {
            Gizmos.color = Color.green;
            // Can display cells (complex, requires low-level access)
        }
        #endif
    }
}
```

### 🎮 Example 2: Large Open World (with occlusion)
```csharp
public class OpenWorldOcclusion : MonoBehaviour
{
    public Camera playerCamera;
    public float occlusionCheckInterval = 0.5f;
    
    private float lastCheckTime;
    
    void Update()
    {
        // Dynamic visibility control for non-static objects
        if (Time.time - lastCheckTime > occlusionCheckInterval)
        {
            lastCheckTime = Time.time;
            CheckDynamicOcclusion();
        }
    }
    
    void CheckDynamicOcclusion()
    {
        // For dynamic objects not participating in baking,
        // write your own simple occlusion using Physics.Raycast
        GameObject[] dynamicObjects = GameObject.FindGameObjectsWithTag("DynamicProp");
        
        foreach (GameObject obj in dynamicObjects)
        {
            Vector3 direction = obj.transform.position - playerCamera.transform.position;
            float distance = direction.magnitude;
            
            // Check if there's an obstacle between camera and object
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, direction.normalized, out hit, distance))
            {
                if (hit.collider.gameObject != obj)
                {
                    // Object is hidden by another object
                    obj.SetActive(false);
                }
                else
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                obj.SetActive(true);
            }
        }
    }
}
```

### 🚪 Example 3: Doors and Dynamic Occluders
```csharp
public class DynamicOccluderDoor : MonoBehaviour
{
    private bool isOpen = false;
    private MeshRenderer doorRenderer;
    
    void Start()
    {
        doorRenderer = GetComponent<MeshRenderer>();
        
        // Dynamic objects DO NOT participate in standard occlusion
        // Need to manually update static flag when changing
    }
    
    public void OpenDoor()
    {
        isOpen = true;
        transform.rotation = Quaternion.Euler(0, 90, 0);
        
        // When door opens, space behind it should become visible
        // Update occlusion (requires rebaking or Unity solution)
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Rendering.StaticOcclusionCulling.GenerateInBackground();
        }
        #endif
    }
    
    public void CloseDoor()
    {
        isOpen = false;
        transform.rotation = Quaternion.identity;
        
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Rendering.StaticOcclusionCulling.GenerateInBackground();
        }
        #endif
    }
}
```

> [!Tip]
> For dynamic doors, use `Occlusion Portal` or don't rely entirely on occlusion.

---

## 4. Occlusion Areas and Occlusion Portals
Unity provides special components for fine-tuning occlusion.

### 🗺️ Occlusion Area
Marks an area where occlusion should work. This allows limiting calculations to specific zones.
```csharp
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OcclusionAreaExample : MonoBehaviour
{
    void Start()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        col.isTrigger = true;
        
        Debug.Log($"Occlusion Area created: {col.bounds.size}");
    }
}
```

How to set up:
1. Add `Occlusion Area` component to a GameObject
2. Adjust size via Box Collider
3. In Occlusion Culling → Bake → `Occlusion Areas` must be enabled

### 🚪 Occlusion Portal
Used for "door openings" — allows controlling which openings the camera can see through.
```csharp
public class OcclusionPortalExample : MonoBehaviour
{
    public OcclusionPortal portal;
    public bool isOpen = true;
    
    void Start()
    {
        portal = GetComponent<OcclusionPortal>();
        if (portal != null)
        {
            portal.open = isOpen;
        }
    }
    
    void Update()
    {
        // Dynamic opening/closing of portal
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isOpen = !isOpen;
            portal.open = isOpen;
            Debug.Log($"Occlusion Portal {(isOpen ? "opened" : "closed")}");
        }
    }
}
```

---

## 5. Verifying Occlusion Culling
### 🖥️ Visualization in Scene View
1. Open Occlusion Culling window
2. Go to Visualization tab
3. Enable Visualize > Occlusion Culling
4. Select a camera in the Camera field
5. Colors:
   - Green objects — visible (rendered)
   - Red objects — hidden (not rendered)
  
### 📊 Performance Profiling
```csharp
using UnityEngine;

public class OcclusionProfiler : MonoBehaviour
{
    public Camera targetCamera;
    private int lastVisibleObjects = 0;
    
    void Update()
    {
        // Get statistics (requires access to internal APIs)
        int visibleObjects = CountVisibleObjects();
        
        if (visibleObjects != lastVisibleObjects)
        {
            Debug.Log($"Occlusion Culling: {visibleObjects} objects visible (was {lastVisibleObjects})");
            lastVisibleObjects = visibleObjects;
        }
        
        // FPS indicator
        float fps = 1.0f / Time.deltaTime;
        Debug.Log($"FPS: {fps:F1}, Visible objects: {visibleObjects}");
    }
    
    private int CountVisibleObjects()
    {
        // Rough estimate (real statistics are more complex)
        var renderers = FindObjectsOfType<Renderer>();
        int visible = 0;
        
        foreach (var renderer in renderers)
        {
            if (renderer.isVisible)
                visible++;
        }
        
        return visible;
    }
}
```

### 🔍 Check via Frame Debugger
1. Window → Analysis → Frame Debugger
2. Click Enable
3. Scroll through the render call list
4. Objects hidden by occlusion will be marked `Occlusion culled`

---

## 6. When to Use Occlusion Culling
### ✅ Good for:
| Scenario | Example |
| --- | --- |
| Dense urban environments | Streets where buildings block each other |
| Mazes and dungeons | Rooms connected by corridors |
| Architectural interiors | Offices, castles, buildings |
| Scenes with many occlusions | Forests with dense foliage (partially) |

### ❌ Inefficient for:
| Scenario | Reason |
| --- | --- |
| Open spaces | All objects are visible — occlusion is useless |
| Deserts, plains | No occlusions |
| Dynamically changing scenes | Requires rebaking |
| Very small objects | Overhead exceeds benefit |

---

## 7. Limitations and Important Nuances
| Limitation | Description | Solution |
| --- | --- | --- |
| Static objects only | Dynamic objects don't participate in baking | Use `Occlusion Portal` or custom logic |
| Memory usage | Visibility data stored in memory | Optimize cell size |
| Baking time | Complex scenes bake slowly | Split scene into parts |
| No Skinned Mesh support | Animated characters don't occlude space | Combine with static geometry |
| Requires rebaking | When geometry changes | Set up automatic baking |

```csharp
// Example: automatic rebaking when scene changes (Editor Script)
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoOcclusionBaker
{
    static AutoOcclusionBaker()
    {
        EditorSceneManager.sceneSaved += OnSceneSaved;
    }
    
    static void OnSceneSaved(UnityEngine.SceneManagement.Scene scene)
    {
        if (EditorUtility.DisplayDialog("Rebake Occlusion", 
            "Recalculate Occlusion Culling for this scene?", 
            "Yes", "No"))
        {
            UnityEditor.Rendering.StaticOcclusionCulling.GenerateInBackground();
            Debug.Log($"Occlusion Culling rebaked for scene {scene.name}");
        }
    }
}
#endif
```

---

## 8. Complete Example: Dungeon Optimization
```csharp
using UnityEngine;

public class DungeonOcclusionManager : MonoBehaviour
{
    [Header("Settings")]
    public float bakeInterval = 10f; // Rebake every 10 seconds (in editor)
    public bool enableDynamicPortals = true;
    
    private float lastBakeTime;
    
    void Start()
    {
        // Mark all static dungeon geometry
        MarkStaticGeometry();
        
        // Bake occlusion for the starting scene
        BakeOcclusion();
    }
    
    void MarkStaticGeometry()
    {
        // Find all walls, floors, ceilings and make them static
        GameObject[] staticProps = GameObject.FindGameObjectsWithTag("StaticEnvironment");
        foreach (GameObject obj in staticProps)
        {
            obj.isStatic = true;
        }
        
        Debug.Log($"Marked {staticProps.Length} static objects for occlusion");
    }
    
    void BakeOcclusion()
    {
        #if UNITY_EDITOR
        if (enableDynamicPortals)
        {
            UnityEditor.Rendering.StaticOcclusionCulling.GenerateInBackground();
            Debug.Log("Started Occlusion Culling baking process");
        }
        #endif
    }
    
    void Update()
    {
        #if UNITY_EDITOR
        // Automatic rebaking while working in the editor
        if (Time.time - lastBakeTime > bakeInterval)
        {
            lastBakeTime = Time.time;
            BakeOcclusion();
        }
        #endif
    }
    
    // Visualize efficiency
    void OnGUI()
    {
        int visibleObjects = CountVisibleRenderers();
        int totalObjects = FindObjectsOfType<Renderer>().Length;
        
        GUI.Box(new Rect(10, 10, 200, 60), "Occlusion Stats");
        GUI.Label(new Rect(20, 30, 180, 20), $"Visible: {visibleObjects} / {totalObjects}");
        GUI.Label(new Rect(20, 50, 180, 20), $"Culled: {totalObjects - visibleObjects}");
        
        float efficiency = (float)(totalObjects - visibleObjects) / totalObjects * 100f;
        GUI.Label(new Rect(20, 70, 180, 20), $"Efficiency: {efficiency:F1}%");
    }
    
    private int CountVisibleRenderers()
    {
        int count = 0;
        foreach (Renderer r in FindObjectsOfType<Renderer>())
        {
            if (r.isVisible) count++;
        }
        return count;
    }
}
```

---

## 9. Best Practices
| Practice | Why It Matters |
| --- | --- |
| ✅ Mark static objects | Occlusion won't work without this |
| ✅ Split large meshes | Smaller objects occlude better |
| ✅ Use Occlusion Areas | Limits calculation area, speeds up baking |
| ✅ Test on target platform | Mobile devices benefit more |
| ✅ Combine with Frustum Culling | Both methods complement each other |
| ❌ Don't use for open spaces | Useless and wastes memory |
| ❌ Don't make everything static | Dynamics are also needed |

---

### ⭐ If this project was useful, put a star on GitHub!
