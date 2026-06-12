# 🧠 Memory Management on Mobile Devices: Texture Compression, ASTC, Resolutions, Sprite Atlases
On mobile devices, GPU memory (VRAM) and RAM are critical resources. 
Textures are among the biggest memory consumers. 
Poor texture management can lead to crashes (OutOfMemory), frame drops, 
and high power consumption. In this guide, we'll cover key optimization techniques.

---

## 1. How Textures Consume Memory?
Texture memory size is calculated by the formula:
```text
Memory (bytes) = Width × Height × Bit Depth / 8
```

Example for a 2048×2048 texture in RGBA32 format (32 bits per pixel):
```text
2048 × 2048 × 32 / 8 = 16,777,216 bytes ≈ 16 MB
```

If you have 100 such textures → 1.6 GB of memory, which will kill any mobile device.
> [!Important]
> This is only video memory. Each texture also occupies RAM during loading.

### 🎯 Main ways to reduce texture memory:
| Method | Savings | Quality Loss |
| --- | --- | --- |
| Reduce resolution | High | High |
| Texture compression (ASTC/ETC2) | Very high | Medium |
| Sprite atlases | Moderate (fewer draw calls) | None |
| Mipmaps | Extra memory (optional) | Improves anti-aliasing |

---

## 2. Texture Compression
Compression works like JPEG but for GPU: the texture is stored compressed and decompressed in hardware during rendering.

### 📊 Comparison of compression formats for mobile:
| Format | Platforms | Bits per pixel | Size (1024x1024) | Quality |
| --- | --- | --- | --- | --- |
| ASTC 4x4 | Modern Android, iOS | 8 bpp | 1 MB | Excellent |
| ASTC 6x6 | Android, iOS | 3.56 bpp | 456 KB | Good |
| ASTC 8x8 | Android, iOS | 2 bpp | 256 KB | Medium |
| ETC2 RGB | All modern Android | 4 bpp | 512 KB | Good |
| ETC2 RGBA | Android | 8 bpp | 1 MB | Good |
| PVRTC | Old iOS devices | 2-4 bpp | 256-512 KB | Medium |
| RGBA32 (uncompressed) | All | 32 bpp | 4 MB | Perfect |

> 🏆 ASTC (Adaptive Scalable Texture Compression) — the best choice for modern mobile devices.
> It supports arbitrary block sizes from 4x4 to 12x12.

### 🛠️ Unity configuration example:
1. Select texture in Project Window
2. In Inspector, find Platform Settings
3. Override settings for Android and iOS:

```csharp
// Example code for checking texture format at runtime
public class TextureFormatChecker : MonoBehaviour
{
    void Start()
    {
        Texture2D tex = GetComponent<Renderer>().material.mainTexture as Texture2D;
        Debug.Log($"Texture format: {tex.format}");
        
        switch (tex.format)
        {
            case TextureFormat.ASTC_4x4:
            case TextureFormat.ASTC_6x6:
            case TextureFormat.ASTC_8x8:
                Debug.Log("✓ Using ASTC compression (optimal)");
                break;
            case TextureFormat.ETC2_RGB:
            case TextureFormat.ETC2_RGBA8:
                Debug.Log("✓ Using ETC2 compression (good)");
                break;
            case TextureFormat.RGBA32:
                Debug.Log("⚠ WARNING: Uncompressed texture! Huge memory usage!");
                break;
        }
    }
}
```

### 📝 ASTC in detail:
ASTC block selection:
| Block | Bits/pixel | When to use |
| --- | --- | --- |
| 4x4 | 8 bpp | Icons, UI, text (max quality) |
| 5x4 | 6.4 bpp | Main environment textures |
| 5x5 | 5.12 bpp | Characters, medium props |
| 6x6 | 3.56 bpp | Background textures, terrain |
| 8x8 | 2 bpp | Very large textures (sky, far background) |

ASTC setup in Unity:
```text
Texture → Inspector → Android → Override
Format: ASTC (6x6 block size)
Compression Quality: High (slower build, better quality)
```

---

## 3. Texture Resolutions
Golden rule: use the smallest necessary resolution. Mobile screens have limited sharpness.

### 📱 Recommended resolutions for different texture types:
| Texture Type | Max resolution | Why |
| --- | --- | --- |
| UI icons | 128×128 - 256×256 | They are small on screen |
| UI backgrounds | 1024×1024 | Stretch across entire screen |
| Character (3rd person) | 1024×1024 | Occupies 30-50% of screen |
| Character (1st person - hands/weapons) | 2048×2048 | Close to camera |
| Environment | 512×512 - 1024×1024 | Many objects |
| Skybox | 2048×2048 | Stretched across entire screen |
| Sprites (2D game) | 512×512 - 1024×1024 | Depends on screen size |

### 🛠️ Automatic resolution management:
```csharp
using UnityEngine;

public class AdaptiveTextureManager : MonoBehaviour
{
    [System.Serializable]
    public class TextureProfile
    {
        public Texture2D texture;
        public int maxResolution = 1024;
    }

    public TextureProfile[] textures;
    
    void Start()
    {
        int qualityLevel = GetDeviceQualityLevel();
        
        foreach (var profile in textures)
        {
            int targetResolution = Mathf.Min(profile.maxResolution, GetResolutionByQuality(qualityLevel));
            Debug.Log($"Texture {profile.texture.name} will be capped at {targetResolution}x{targetResolution}");
        }
    }
    
    private int GetDeviceQualityLevel()
    {
        // 0: low, 1: medium, 2: high
        if (SystemInfo.systemMemorySize < 2048) return 0;
        if (SystemInfo.systemMemorySize < 4096) return 1;
        return 2;
    }
    
    private int GetResolutionByQuality(int quality)
    {
        switch (quality)
        {
            case 0: return 512;   // Low
            case 1: return 1024;  // Medium
            default: return 2048; // High
        }
    }
}
```

> [!Tip]
> Mobile games rarely need textures larger than 2048x2048. The exception is cinematic cutscenes.

---

## 4. Sprite Atlases
A sprite atlas is a single large texture containing many small sprites. 
This dramatically reduces the number of draw calls.

### 📊 Atlas advantages:
| Metric | Without atlas | With atlas |
| --- | --- | --- |
| Draw calls (100 sprites) | 100 | 1-2 |
| GPU memory | Sum of all textures | One texture |
| Load time | 100 separate files | 1 file |

### 🛠️ Creating an atlas in Unity (Sprite Atlas V2):
```csharp
// Example atlas configuration via script
using UnityEngine.U2D;
using UnityEditor.U2D;
using UnityEngine;

#if UNITY_EDITOR
public class SpriteAtlasBuilder : MonoBehaviour
{
    [MenuItem("Tools/Build UI Atlas")]
    public static void BuildUIAtlas()
    {
        SpriteAtlas atlas = ScriptableObject.CreateInstance<SpriteAtlas>();
        
        atlas.SetPlatformSettings(new SpriteAtlasPlatformSettings
        {
            name = "Android",
            format = TextureImporterFormat.ASTC_6x6,
            maxTextureSize = 2048
        });
        
        string folderPath = "Assets/Sprites/UI";
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(folderPath);
        
        atlas.Add(sprites);
        
        AssetDatabase.CreateAsset(atlas, "Assets/Atlas/UIAtlas.spriteatlasv2");
        AssetDatabase.SaveAssets();
    }
}
#endif
```

### 📝 Manual atlas setup:
1. Window → 2D → Sprite Atlas
2. Click Create New Sprite Atlas
3. Add sprites to Objects for Packing
4. Settings:
   - Allow Rotation: Off (better compression)
   - Tight Packing: Off (if precise bounds not needed)
   - Padding: 2-4 pixels (prevents neighbor artifacts)
  
### 🧪 Using atlas in code:
```csharp
using UnityEngine;
using UnityEngine.U2D;

public class SpriteFromAtlasLoader : MonoBehaviour
{
    [SerializeField] private SpriteAtlas uiAtlas;
    [SerializeField] private string spriteName = "button_play";
    
    void Start()
    {
        Sprite mySprite = uiAtlas.GetSprite(spriteName);
        
        if (mySprite != null)
        {
            GetComponent<Image>().sprite = mySprite;
            Debug.Log($"Sprite {spriteName} loaded from atlas");
        }
        else
        {
            Debug.LogError($"Sprite {spriteName} not found in atlas");
        }
    }
    
    public void ShowIcons(string[] iconNames)
    {
        foreach (string name in iconNames)
        {
            Sprite icon = uiAtlas.GetSprite(name);
            if (icon != null)
                CreateIconObject(icon);
        }
    }
    
    private void CreateIconObject(Sprite sprite) { /* ... */ }
}
```

### ⚠️ Atlas pitfalls:
| Problem | Solution |
| --- | --- |
| Artifacts on sprite edges | Increase Padding to 4-8 |
| Atlas too large (4096+) | Split into multiple atlases by category (UI, characters, environment) |
| Frequently updated sprites | Put them in a separate atlas (updating one sprite requires rebuilding entire atlas) |
| Sprites with different compression | Use separate atlases for different settings |

---

## 5. Mipmaps — Distance-Based Quality Management
Mipmaps are a series of reduced copies of a texture. They are needed to prevent shimmering on distant objects.

### 📊 Memory for mipmaps:
```text
Total size with mipmaps = Original size × (1 + 1/4 + 1/16 + 1/64 + ...) ≈ Original size × 1.33
```
Example: 1024×1024 texture (1 MB) with mipmaps takes ~1.33 MB.

### 🛠️ When to enable mipmaps:
| Object type | Mipmaps | Reason |
| --- | --- | --- |
| Ground/terrain | ✅ Yes | Visible both close and far |
| Building walls | ✅ Yes | Seen from different distances |
| UI elements | ❌ No | Always at the same distance |
| 3D character | ✅ Yes | Can move away |
| 2D sprites | ❌ No | Camera doesn't zoom |

```csharp
// Checking and managing mipmaps in code
public class MipmapManager : MonoBehaviour
{
    void Start()
    {
        Texture texture = GetComponent<Renderer>().material.mainTexture;
        
        if (texture != null)
        {
            Debug.Log($"Mipmaps enabled: {texture.mipmapCount > 1}");
            Debug.Log($"Mipmap count: {texture.mipmapCount}");
            
            texture.mipMapBias = -0.5f; // Shift toward sharper textures
        }
    }
}
```

---

## 6. Practical Texture Memory Checklist
### ✅ Optimization for Android:
- All textures compressed to ASTC (or ETC2 for older devices)
- Texture sizes: powers of two (2, 4, 8, 16, 32... 2048)
- UI textures: Generate Mipmaps = OFF
- 3D textures: Generate Mipmaps = ON
- Sprites combined into Sprite Atlases (max 2048x2048)
- Uncompressed textures used only where quality is critical

### ✅ Optimization for iOS:
- All textures compressed to ASTC (on newer devices) or PVRTC (on older ones)
- Use Override for iOS in import settings
- Check memory via XCode Memory Debugger

### 🛠️ Monitoring memory consumption:
```csharp
using UnityEngine.Profiling;

public class TextureMemoryMonitor : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            long totalMemory = Profiler.GetTotalAllocatedMemory();
            long textureMemory = Profiler.GetAllocatedMemoryForGraphicsDriver();
            
            Debug.Log($"Total memory: {totalMemory / 1048576} MB");
            Debug.Log($"Texture memory: {textureMemory / 1048576} MB");
            
            if (textureMemory > 300 * 1048576) // >300 MB
                Debug.LogWarning("⚠ High video memory usage!");
        }
    }
}
```

---

## 7. Common Mistakes and Solutions
| Mistake | Why it's bad | Solution |
| --- | --- | --- |
| RGBA32 texture | 4 MB per 1024x1024 vs 0.5 MB with compression | Switch to ASTC |
| 4096×4096 atlas | Crashes on older devices | Reduce to 2048×2048 |
| Mipmaps on UI | Wasted memory (33% extra) | Turn off mipmaps |
| 300×300 texture | Not power of two → decompression & extra memory | Crop to 256×256 or 512×512 | 
| Huge sprites | 2048×2048 for 32×32 icon | Reduce to 128×128 + atlas |

---

### ⭐ If this project was useful, put a star on GitHub!
