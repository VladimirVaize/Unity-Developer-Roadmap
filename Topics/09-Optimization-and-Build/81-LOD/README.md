# 🎨 LOD (Level of Detail) in Unity: Swapping Models Based on Camera Distance
Level of Detail (LOD) is an optimization technique where different models with increasing levels of detail are used for a single object based on its distance from the camera. 
The farther the object, the less detailed model is used, significantly reducing GPU load.

---

## 1. What is LOD and Why Do We Need It?
### 🎯 Goals of Using LOD:
| Goal | Description |
| --- | --- |
| Performance optimization | Reducing polygon count per frame |
| Memory saving | Less detailed models weigh less |
| FPS increase | Especially important on mobile devices |
| Scalability | Ability to render thousands of objects |

### 📊 How It Works:
```text
Camera
   │
   ├── 0-10 m   → LOD 0 (High Poly)   10,000 polygons
   ├── 10-20 m  → LOD 1 (Medium Poly) 5,000 polygons
   ├── 20-40 m  → LOD 2 (Low Poly)    1,000 polygons
   └── >40 m    → LOD 3 (Impostor/disabled)
```

> [!Important]
> LOD works based on distance between the camera and the object's pivot point (not render center).

---

## 2. LOD Group Component
Unity provides the LOD Group component that manages level of detail switching.

### 🧩 LOD Group Structure:
| Parameter | Description |
| --- | --- |
| LOD 0 | Most detailed level (close to camera) |
| LOD 1 | Medium detail |
| LOD 2 | Low detail |
| LOD 3... | Additional levels |
| Culled | Object not rendered (beyond max distance) |

### 📐 Percentage Settings:
- Percentages are calculated from Reference Resolution (usually screen width in pixels)
- Recommended values: LOD0: 60%, LOD1: 30%, LOD2: 10%, Culled: 5%

```csharp
using UnityEngine;

public class LODGroupExample : MonoBehaviour
{
    private LODGroup lodGroup;
    
    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
        
        // Set percentages manually
        lodGroup.SetLODs(new LOD[] {
            new LOD(0.6f, new Renderer[] { GetComponent<Renderer>() }),  // LOD0: 60%
            new LOD(0.3f, new Renderer[] { GetComponent<Renderer>() }),  // LOD1: 30%
            new LOD(0.1f, null)  // LOD2: disable object
        });
        
        // Enable cross-fade animation
        lodGroup.fadeMode = LODFadeMode.CrossFade;
        lodGroup.animateCrossFading = true;
    }
}
```

---

## 3. Setting Up LOD Group in the Editor
### 🛠️ Step-by-Step LOD Creation:
1. Create different models for each LOD level:
   - `Tree_High.fbx` (10K polygons)
   - `Tree_Medium.fbx` (5K polygons)
   - `Tree_Low.fbx` (1K polygons)
  
2. Add LOD Group component to an empty GameObject:
   - `Add Component → Rendering → LOD Group`
  
3. Configure levels:
   - Click `+` to add a level
   - Drag models to the appropriate slots
   - Adjust percentage sliders
  
4. Configure transitions:
   - `Fade Mode`: Cross Fade or Speed Tree
   - `Cross Fade Width`: transition zone width (0.1-0.5)
  
### 📝 Example LOD Group in Inspector (code equivalent):
```csharp
public class TreeLODSetup : MonoBehaviour
{
    void Awake()
    {
        LODGroup group = gameObject.AddComponent<LODGroup>();
        
        // Get renderers for each level
        Renderer highRenderer = transform.Find("HighPoly").GetComponent<Renderer>();
        Renderer mediumRenderer = transform.Find("MediumPoly").GetComponent<Renderer>();
        Renderer lowRenderer = transform.Find("LowPoly").GetComponent<Renderer>();
        
        // Create LOD levels
        LOD[] lods = new LOD[3];
        lods[0] = new LOD(0.6f, new Renderer[] { highRenderer });
        lods[1] = new LOD(0.3f, new Renderer[] { mediumRenderer });
        lods[2] = new LOD(0.1f, new Renderer[] { lowRenderer });
        
        group.SetLODs(lods);
        group.RecalculateBounds();
    }
}
```

---

## 4. Programmatic LOD Control
### 🎮 LOD Control via Scripts:
```csharp
public class DynamicLODController : MonoBehaviour
{
    private LODGroup lodGroup;
    private float originalReferenceResolution;
    
    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
        originalReferenceResolution = lodGroup.referenceResolution;
        
        // Get current LODs
        LOD[] currentLODs = lodGroup.GetLODs();
        Debug.Log($"Number of LOD levels: {currentLODs.Length}");
        
        // Change percentage for LOD0
        currentLODs[0].screenRelativeTransitionHeight = 0.8f;
        lodGroup.SetLODs(currentLODs);
    }
    
    // Temporary detail increase (e.g., when aiming)
    public void IncreaseDetailTemporarily(float duration)
    {
        StartCoroutine(TempLODBoost(duration));
    }
    
    private System.Collections.IEnumerator TempLODBoost(float duration)
    {
        LOD[] lods = lodGroup.GetLODs();
        float originalLOD0 = lods[0].screenRelativeTransitionHeight;
        
        // Increase LOD0 threshold (object stays detailed longer)
        lods[0].screenRelativeTransitionHeight = 1.0f;
        lodGroup.SetLODs(lods);
        
        yield return new WaitForSeconds(duration);
        
        // Revert
        lods[0].screenRelativeTransitionHeight = originalLOD0;
        lodGroup.SetLODs(lods);
    }
    
    // Adaptive LOD based on FPS
    void Update()
    {
        float currentFPS = 1.0f / Time.deltaTime;
        
        if (currentFPS < 30)
        {
            // Low FPS — aggressively reduce detail
            lodGroup.SetLODs(GetAggressiveLODs());
        }
        else if (currentFPS > 50)
        {
            // High FPS — increase quality
            lodGroup.SetLODs(GetHighQualityLODs());
        }
    }
    
    private LOD[] GetAggressiveLODs()
    {
        return new LOD[] {
            new LOD(0.3f, new Renderer[] { GetRendererForLOD(0) }),
            new LOD(0.1f, new Renderer[] { GetRendererForLOD(1) }),
            new LOD(0.05f, null)
        };
    }
    
    private LOD[] GetHighQualityLODs()
    {
        return new LOD[] {
            new LOD(0.8f, new Renderer[] { GetRendererForLOD(0) }),
            new LOD(0.5f, new Renderer[] { GetRendererForLOD(1) }),
            new LOD(0.2f, new Renderer[] { GetRendererForLOD(2) })
        };
    }
    
    private Renderer GetRendererForLOD(int level)
    {
        return transform.GetChild(level).GetComponent<Renderer>();
    }
}
```

### 🎭 Fade Modes:
```csharp
public class LODFadeExamples : MonoBehaviour
{
    private LODGroup lodGroup;
    
    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
        
        // Mode 1: Cross Fade — smooth fade in/out
        lodGroup.fadeMode = LODFadeMode.CrossFade;
        lodGroup.animateCrossFading = true;
        
        // Mode 2: Speed Tree — special mode for trees
        lodGroup.fadeMode = LODFadeMode.SpeedTree;
        
        // Mode 3: No transition — abrupt switching
        lodGroup.fadeMode = LODFadeMode.None;
        
        // Configure fade transition width
        LOD[] lods = lodGroup.GetLODs();
        lods[0].fadeTransitionWidth = 0.3f;
        lodGroup.SetLODs(lods);
    }
}
```

---

## 5. Creating LOD Levels in the Editor
### 🎨 Model Creation Recommendations:
| LOD | Polygons | Textures | Vertices | Usage |
| --- | --- | --- | --- | --- |
| LOD0 | 100% | 4K | High | Close objects |
| LOD1 | 50-60% | 2K | Medium | Medium distance |
| LOD2 | 20-25% | 1K | Low | Distant objects |
| LOD3 | 5-10% | 256x256 | Minimum | Very far |

### 🔧 Tools for Creating LOD:
1. Unity LOD Generator (Built-in):
   - `Window → Rendering → LOD Generator`
   - Automatically creates simplified versions
  
2. Blender Decimate Modifier:
   - Import → Decimate → Reduce polygons → Export
  
3. Simplygon (Plugin):
   - Professional automatic simplification
  
```csharp
// Example using LOD Generator via code (Editor Script)
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.LODGenerator;

public class LODGeneratorExample
{
    [MenuItem("Tools/Generate LODs for Selection")]
    static void GenerateLODs()
    {
        GameObject selected = Selection.activeGameObject;
        LODGeneratorUtility.GenerateLODs(selected, 50f, 25f, 10f);
        Debug.Log("LODs generated for " + selected.name);
    }
}
#endif
```

---

## 6. LOD Optimization
### ⚡ Best Practices:
```csharp
public class LODOptimization : MonoBehaviour
{
    void Start()
    {
        // 1. Disable Shadow Casting on far LODs
        DisableShadowsOnFarLODs();
        
        // 2. Configure Occlusion Culling
        SetupOcclusionCulling();
        
        // 3. Use Lightmap for distant objects
        UseLightmapForDistantLODs();
    }
    
    void DisableShadowsOnFarLODs()
    {
        LODGroup group = GetComponent<LODGroup>();
        LOD[] lods = group.GetLODs();
        
        for (int i = 1; i < lods.Length; i++)
        {
            foreach (Renderer renderer in lods[i].renderers)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }
    }
    
    void SetupOcclusionCulling()
    {
        LODGroup group = GetComponent<LODGroup>();
        group.RecalculateBounds();
    }
    
    void UseLightmapForDistantLODs()
    {
        LODGroup group = GetComponent<LODGroup>();
        LOD[] lods = group.GetLODs();
        
        if (lods.Length > 2)
        {
            foreach (Renderer renderer in lods[2].renderers)
            {
                renderer.lightmapIndex = 1;
                renderer.lightmapScaleOffset = Vector4.one;
            }
        }
    }
}
```

### 📊 Setting LOD Bias in Project Settings:
```csharp
public class QualityBasedLOD : MonoBehaviour
{
    void Start()
    {
        // Higher quality = keep detail farther
        if (QualitySettings.GetQualityLevel() >= 2)
        {
            QualitySettings.lodBias = 2.0f;
        }
        else if (QualitySettings.GetQualityLevel() == 1)
        {
            QualitySettings.lodBias = 1.0f;
        }
        else
        {
            QualitySettings.lodBias = 0.5f;
        }
        
        Debug.Log($"LOD Bias set to: {QualitySettings.lodBias}");
    }
}
```

---

## 7. Advanced LOD Techniques
### 🎭 Impostor LOD (Billboards):
```csharp
public class ImpostorLOD : MonoBehaviour
{
    private Camera mainCamera;
    private LODGroup lodGroup;
    private GameObject impostorBillboard;
    
    void Start()
    {
        mainCamera = Camera.main;
        lodGroup = GetComponent<LODGroup>();
        
        CreateImpostorBillboard();
        SetupImpostorLOD();
    }
    
    void CreateImpostorBillboard()
    {
        impostorBillboard = new GameObject("Impostor");
        impostorBillboard.transform.parent = transform;
        impostorBillboard.transform.localPosition = Vector3.zero;
        
        var billboardRenderer = impostorBillboard.AddComponent<SpriteRenderer>();
        billboardRenderer.sprite = CaptureBillboardSprite();
        billboardRenderer.color = Color.white;
    }
    
    Sprite CaptureBillboardSprite()
    {
        // Capture current view to create billboard
        return null;
    }
    
    void SetupImpostorLOD()
    {
        LOD[] lods = new LOD[4];
        lods[0] = new LOD(0.5f, GetComponent<Renderer>());
        lods[1] = new LOD(0.2f, GetComponent<Renderer>());
        lods[2] = new LOD(0.1f, impostorBillboard.GetComponent<Renderer>());
        lods[3] = new LOD(0.05f, null);
        
        lodGroup.SetLODs(lods);
    }
}
```

### 🔄 Animated LOD (object lifetime):
```csharp
public class AnimatedLOD : MonoBehaviour
{
    private Animator animator;
    private LODGroup lodGroup;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        lodGroup = GetComponent<LODGroup>();
        
        lodGroup.onLODChanged += OnLODLevelChanged;
    }
    
    void OnLODLevelChanged(int lodLevel)
    {
        if (lodLevel >= 2)
        {
            animator.enabled = false;
        }
        else
        {
            animator.enabled = true;
        }
    }
    
    void OnDestroy()
    {
        lodGroup.onLODChanged -= OnLODLevelChanged;
    }
}
```

---

## 8. Common Mistakes and Solutions
### ❌ Mistakes:
| Mistake | Problem | Solution |
| --- | --- | --- |
| Flickering on switch | Abrupt LOD switching | Use `Cross Fade` |
| Empty renderers | LOD slot empty | Ensure at least one renderer |
| Incorrect bounds | LOD switches incorrectly | Call `RecalculateBounds()` |
| Too frequent switching | Object on LOD boundary | Increase `fadeTransitionWidth` |

```csharp
public class LODErrorFix : MonoBehaviour
{
    void FixCommonLODIssues()
    {
        LODGroup group = GetComponent<LODGroup>();
        
        group.RecalculateBounds();
        
        group.fadeMode = LODFadeMode.CrossFade;
        group.animateCrossFading = true;
        
        LOD[] lods = group.GetLODs();
        for (int i = 0; i < lods.Length; i++)
        {
            if (lods[i].renderers == null || lods[i].renderers.Length == 0)
            {
                Debug.LogWarning($"LOD level {i} has no renderers!");
            }
        }
        
        lods[0].fadeTransitionWidth = 0.3f;
        group.SetLODs(lods);
    }
}
```

---

## 9. Complete Example: Forest LOD
```csharp
public class ForestLODManager : MonoBehaviour
{
    [Header("LOD Settings")]
    public float highQualityDistance = 30f;
    public float mediumQualityDistance = 60f;
    public float lowQualityDistance = 100f;
    
    private LODGroup[] allTrees;
    
    void Start()
    {
        allTrees = FindObjectsOfType<LODGroup>();
        
        foreach (LODGroup tree in allTrees)
        {
            SetupTreeLOD(tree);
        }
    }
    
    void SetupTreeLOD(LODGroup tree)
    {
        Renderer[] allRenderers = tree.GetComponentsInChildren<Renderer>();
        
        LOD[] lods = new LOD[3];
        
        lods[0] = new LOD(highQualityDistance / GetMaxDistance(), allRenderers);
        lods[0].fadeTransitionWidth = 0.2f;
        
        Renderer[] medium = GetMediumRenderers(tree);
        lods[1] = new LOD(mediumQualityDistance / GetMaxDistance(), medium);
        
        Renderer[] low = GetLowRenderers(tree);
        lods[2] = new LOD(lowQualityDistance / GetMaxDistance(), low);
        
        tree.SetLODs(lods);
        tree.fadeMode = LODFadeMode.CrossFade;
    }
    
    float GetMaxDistance()
    {
        return lowQualityDistance + 20f;
    }
    
    Renderer[] GetMediumRenderers(LODGroup tree)
    {
        return tree.GetComponentsInChildren<Renderer>();
    }
    
    Renderer[] GetLowRenderers(LODGroup tree)
    {
        return tree.GetComponentsInChildren<Renderer>();
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
