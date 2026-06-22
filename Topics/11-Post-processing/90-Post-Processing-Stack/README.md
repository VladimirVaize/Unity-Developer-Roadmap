# 🎨 Post-Processing Stack in Unity: Bloom, Depth of Field, Color Grading, Anti-aliasing (Volume Framework)
Post-Processing is the final rendering stage where visual effects are applied to the rendered image. 
Unity provides a powerful Volume Framework for configuring post-effects both globally and locally (depending on camera position).

---

## 1. Post-Processing Basics in Unity
### 📦 Installing Post-Processing:
1. Window → Package Manager
2. Find Post Processing
3. Install the package (version 3.x or newer)

### 🏗️ Basic System Setup:
```csharp
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingSetup : MonoBehaviour
{
    public PostProcessVolume volume;
    public PostProcessProfile profile;
    
    void Start()
    {
        if (volume == null)
        {
            GameObject volumeObj = new GameObject("PostProcessVolume");
            volume = volumeObj.AddComponent<PostProcessVolume>();
            volume.isGlobal = true;
        }
        
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<PostProcessProfile>();
            volume.sharedProfile = profile;
        }
    }
}
```

---

## 2. Bloom — Glow Effect
Bloom creates a "glow" effect around bright objects. Simulates light scattering in the eye or camera lens.

### 🎛️ Bloom Parameters:
| Parameter | Description | Value |
| --- | --- | --- |
| Intensity | Glow intensity | 0-10 (usually 0.5-2) |
| Threshold | Brightness threshold | 0-1 (0.8-1.0) |
| Scatter | Scattering (glow width) | 0-1 |
| Tint | Glow color | Color (white/yellow) |
| Dirt Texture | Lens dirt texture | Texture2D |

### 🌟 Dynamic Bloom (e.g., explosion):
```csharp
public class ExplosionBloom : MonoBehaviour
{
    public PostProcessVolume volume;
    private Bloom bloom;
    private float originalIntensity;
    
    void Start()
    {
        volume.profile.TryGetSettings(out bloom);
        originalIntensity = bloom.intensity.value;
    }
    
    public void TriggerExplosion()
    {
        StartCoroutine(FlashBloom(3f, 2f));
    }
    
    System.Collections.IEnumerator FlashBloom(float peak, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float intensity = Mathf.Lerp(originalIntensity, peak, Mathf.Sin(t * Mathf.PI));
            bloom.intensity.value = intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = originalIntensity;
    }
}
```

---

## 3. Depth of Field (DOF)
Depth of Field simulates camera focus: objects in focus are sharp, foreground and background are blurred.

### 🎛️ DOF Parameters:
| Parameter | Description | Value |
| --- | --- | --- |
| Focus Distance | Distance to focused object | 0-100 (meters) |
| Aperture | Aperture (smaller = more blur) | 1-32 |
| Focal Length | Focal length | 1-300 mm |
| Kernel Size | Blur quality | Small/Medium/Large |
| Max Blur Size | Maximum blur | 0-10 |

### 🎮 Dynamic DOF for RPG:
```csharp
public class DynamicDOF : MonoBehaviour
{
    public PostProcessVolume volume;
    public Transform player;
    public Transform currentTarget;
    private DepthOfField dof;
    private float smoothVelocity = 0f;
    
    void Start()
    {
        volume.profile.TryGetSettings(out dof);
    }
    
    void Update()
    {
        if (currentTarget != null)
        {
            float targetDistance = Vector3.Distance(player.position, currentTarget.position);
            dof.focusDistance.value = Mathf.SmoothDamp(
                dof.focusDistance.value,
                targetDistance,
                ref smoothVelocity,
                0.3f
            );
        }
    }
    
    public void FocusOnTarget(Transform target)
    {
        currentTarget = target;
    }
    
    public void ResetFocus()
    {
        currentTarget = null;
        dof.focusDistance.value = 10f;
    }
}
```

---

## 4. Color Grading
Color Grading allows changing the image color palette, creating a specific mood.

### 🎛️ Color Grading Parameters:
| Parameter | Description | Value |
| --- | --- | --- |
| Post-exposure | Overall exposure | -10 to +10 EV |
| Contrast | Contrast | -100 to +100 |
| Saturation | Saturation | -100 to +100 |
| Color Filter | Color filter | Color |
| Hue Shift | Hue shift | -180 to +180 |
| Temperature | Temperature (blue/yellow) | -100 to +100 |
| Tint | Tint (green/magenta) | -100 to +100 |
| Lift | Shadows (color) | Color |
| Gamma | Midtones (color) | Color |
| Gain | Highlights (color) | Color |

### 🎬 Color Grading Examples:
```csharp
public void ApplyHorrorMode()
{
    grading.temperature.value = -30f;     // Cold
    grading.tint.value = 20f;             // Magenta tint
    grading.saturation.value = -30f;      // Desaturated
    grading.contrast.value = 30f;         // High contrast
    grading.postExposure.value = -1f;     // Darker
    grading.gain.value = new Color(0.8f, 0.7f, 0.9f); // Cold highlights
}

public void ApplyNostalgiaMode()
{
    grading.temperature.value = 15f;
    grading.saturation.value = -20f;
    grading.contrast.value = -10f;
    grading.gain.value = new Color(1.1f, 1.0f, 0.8f); // Yellow highlights
    grading.lift.value = new Color(0.1f, 0.05f, 0f);  // Warm shadows
}
```

---

## 5. Anti-aliasing
Anti-aliasing eliminates "jaggies" on diagonal lines and object edges.

### 🎛️ Anti-aliasing Types:
| Type | Description | Performance |
| --- | --- | --- |
| FXAA | Fast, lower quality | 🟢 Very fast |
| SMAA | Good quality, medium speed | 🟡 Medium |
| TAA | Best quality, possible artifacts | 🔴 Demanding |

```csharp
public void SetAntiAliasing(string mode)
{
    PostProcessLayer layer = Camera.main.GetComponent<PostProcessLayer>();
    if (layer != null)
    {
        switch (mode)
        {
            case "FXAA":
                layer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                break;
            case "SMAA":
                layer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                break;
            case "TAA":
                layer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                break;
        }
    }
}
```

---

## 6. Local Volumes
Volume Framework allows applying effects based on camera position (zones).

### 🎯 Creating a Local Volume:
1. GameObject → Volume → Box/Sphere Volume
2. Configure Trigger
3. Add PostProcessVolume component
4. Set Is Global = false
5. Configure Blend Distance

```csharp
public class VolumeZone : MonoBehaviour
{
    public PostProcessVolume volume;
    public Color zoneColor = Color.red;
    
    void Start()
    {
        volume.isGlobal = false;
        
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
        
        ColorGrading grading;
        if (volume.profile.TryGetSettings(out grading))
        {
            grading.colorFilter.value = zoneColor;
            grading.colorFilter.overrideState = true;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            volume.weight = 1f;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            volume.weight = 0f;
        }
    }
}
```

---

## 7. Complete Example: Switching Profiles
```csharp
public class PostProcessingManager : MonoBehaviour
{
    [Header("Volumes")]
    public PostProcessVolume globalVolume;
    public PostProcessVolume underwaterVolume;
    public PostProcessVolume cinematicVolume;
    
    [Header("Settings")]
    public float transitionTime = 1f;
    
    private PostProcessVolume currentVolume;
    
    public void SwitchToUnderwater()
    {
        StartCoroutine(SwitchWithBlend(underwaterVolume));
    }
    
    public void SwitchToCinematic()
    {
        StartCoroutine(SwitchWithBlend(cinematicVolume));
    }
    
    public void SwitchToDefault()
    {
        StartCoroutine(SwitchWithBlend(globalVolume));
    }
    
    private IEnumerator SwitchWithBlend(PostProcessVolume target)
    {
        float elapsed = 0f;
        float startWeight = currentVolume != null ? currentVolume.weight : 0f;
        
        while (elapsed < transitionTime)
        {
            float t = elapsed / transitionTime;
            if (currentVolume != null)
                currentVolume.weight = Mathf.Lerp(startWeight, 0f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (currentVolume != null)
            currentVolume.weight = 0f;
        
        currentVolume = target;
        target.weight = 0f;
        target.gameObject.SetActive(true);
        
        elapsed = 0f;
        while (elapsed < transitionTime)
        {
            float t = elapsed / transitionTime;
            target.weight = Mathf.Lerp(0f, 1f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        target.weight = 1f;
    }
}
```

---

## 8. Best Practices
### ✅ Recommendations:
1. Use Profiles — reuse settings across scenes
2. Don't enable all effects at once — it's expensive
3. TAA + Motion Blur — gives a beautiful cinematic look
4. Test on target devices — post-effects are performance-heavy
5. Use LUT for color grading — faster than parameter tuning

---

### ⭐ If this project was useful, put a star on GitHub!
