# 🗜️ Asset Compression and Optimization in Unity: Texture Importer, Audio Importer, Model Importer, Presets
Asset optimization is one of the key stages of game development. 
Proper import settings for textures, audio, and models can reduce the final build size by 2-5 times and significantly improve performance.

---

## 1. Texture Importer — Texture Import Settings
Texture Importer is the settings window for each texture asset. Here you configure compression, size, format, and platform-dependent settings.

### 📂 Accessing Texture Importer:
1. Select a texture in the Project Window
2. The Texture Importer opens in the Inspector

### 🎯 Key Texture Importer Parameters:
| Parameter | Description | Recommendation |
| --- | --- | --- |
| Texture Type | Texture type (Sprite, Normal Map, UI, etc.) | Choose based on purpose |
| Sprite Mode | Single / Multiple / Polygon | For sprites — Multiple |
| Pixels Per Unit | Pixels per unit | 100 (standard) |
| Mesh Type | Tight / Rectangular | For sprites — Tight |
| Wrap Mode | Repeat / Clamp / Mirror | Depends on texture |
| Filter Mode | Point / Bilinear / Trilinear | For pixel art — Point |
| Compression | None / Low / Normal / High | High for most textures |
| Format | RGBA Compressed, DXT, ASTC, etc. | Choose for platform |

### 🖼️ Configuring Compression for Different Platforms:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class TextureOptimizer : MonoBehaviour
{
    [MenuItem("Tools/Optimize Textures")]
    public static void OptimizeTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Textures" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer == null) continue;
            
            // Android settings
            TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
            androidSettings.overridden = true;
            androidSettings.format = TextureImporterFormat.ASTC_6x6;
            androidSettings.compressionQuality = 50;
            importer.SetPlatformTextureSettings(androidSettings);
            
            // iOS settings
            TextureImporterPlatformSettings iosSettings = importer.GetPlatformTextureSettings("iPhone");
            iosSettings.overridden = true;
            iosSettings.format = TextureImporterFormat.ASTC_6x6;
            iosSettings.compressionQuality = 50;
            importer.SetPlatformTextureSettings(iosSettings);
            
            // Standalone settings
            TextureImporterPlatformSettings standaloneSettings = importer.GetPlatformTextureSettings("Standalone");
            standaloneSettings.overridden = true;
            standaloneSettings.format = TextureImporterFormat.DXT5;
            importer.SetPlatformTextureSettings(standaloneSettings);
            
            importer.textureCompression = TextureImporterCompression.Compressed;
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log($"Optimized texture: {path}");
        }
    }
}
#endif
```

### 📊 Compression Format Table:
| Format | Platform | Quality | Size | When to Use |
| --- | --- | --- | --- | --- |
| ASTC 4x4 | Android, iOS | Excellent | Medium | High-quality games |
| ASTC 6x6 | Android, iOS | Good | Small | Mobile games (recommended) |
| ASTC 8x8 | Android, iOS | Medium | Very small | High-resolution textures |
| DXT1 | Standalone | Good | Small | No alpha channel |
| DXT5 | Standalone | Excellent | Medium | With alpha channel |
| ETC2 | Android | Good | Medium | Older devices |
| PVRTC | iOS | Medium | Small | Older iOS devices |
| RGBA 32-bit | All | Perfect | Huge | Only for UI and critical textures |

### 🎨 Sprite Configuration:
```csharp
public class SpriteSetupExample : MonoBehaviour
{
    void ConfigureSprite(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 100;
        importer.spriteMeshType = SpriteMeshType.Tight;
        
        importer.spriteIsPolygon = false;
        importer.spriteExtrude = 1;
        importer.spriteGenerateFallbackPhysicsShape = true;
        
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.npotScale = TextureImporterNPOTScale.None;
        
        AssetDatabase.ImportAsset(path);
    }
}
```

---

## 2. Audio Importer — Audio Import Settings
Audio Importer manages compression and quality settings for audio files.

### 🔉 Key Audio Importer Parameters:
| Parameter | Description | Recommendation |
| --- | --- | --- |
| Force To Mono | Convert to mono | Yes for most sounds |
| Normalize | Normalize volume | Yes for SFX, No for music |
| Load In Background | Load in background | Yes for large files |
| Ambisonic | 3D sound for VR | Only for VR |
| Compression Format | Compression format | Depends on audio type |

### 📊 Audio Compression Formats:
| Format | Quality | Size | Usage |
| --- | --- | --- | --- |
| PCM | Perfect | Huge | Only for critical sounds |
| ADPCM | Good | Small | SFX, short sounds |
| Vorbis | Good | Small | Music, long sounds |
| MP3 | Medium | Very small | Background music |
| HEVAG | Good | Small | Japanese consoles |

### 🎵 Audio Importer Configuration Example:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AudioOptimizer : MonoBehaviour
{
    [MenuItem("Tools/Optimize Audio")]
    public static void OptimizeAudio()
    {
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
            
            if (importer == null) continue;
            
            // SFX settings
            if (path.Contains("SFX"))
            {
                importer.forceToMono = true;
                importer.normalize = true;
                importer.loadInBackground = false;
                
                importer.defaultSampleSettings = new AudioImporterSampleSettings
                {
                    compressionFormat = AudioCompressionFormat.ADPCM,
                    sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate,
                    sampleRateOverride = 22050,
                    quality = 0.5f
                };
            }
            
            // Music settings
            if (path.Contains("Music"))
            {
                importer.forceToMono = false;
                importer.normalize = false;
                importer.loadInBackground = true;
                
                importer.defaultSampleSettings = new AudioImporterSampleSettings
                {
                    compressionFormat = AudioCompressionFormat.Vorbis,
                    sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate,
                    quality = 0.7f
                };
            }
            
            // Mobile settings
            AudioImporterSampleSettings androidSettings = importer.GetOverrideSampleSettings("Android");
            androidSettings.compressionFormat = AudioCompressionFormat.Vorbis;
            androidSettings.quality = 0.5f;
            importer.SetOverrideSampleSettings("Android", androidSettings);
            
            AudioImporterSampleSettings iosSettings = importer.GetOverrideSampleSettings("iPhone");
            iosSettings.compressionFormat = AudioCompressionFormat.MP3;
            iosSettings.quality = 0.6f;
            importer.SetOverrideSampleSettings("iPhone", iosSettings);
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log($"Optimized audio: {path}");
        }
    }
}
#endif
```

---

## 3. Model Importer — 3D Model Import Settings
Model Importer manages the import of 3D models (FBX, OBJ, DAE, etc.).

### 📐 Key Model Importer Parameters:
| Tab | Parameter | Description |
| --- | --- | --- |
| Model | Scale Factor | Model scale |
| | Use File Scale | Use scale from file |
| | Bake Axis Conversion | Axis conversion |
| Rig | Animation Type | None / Humanoid / Generic |
| | Avatar Definition | Create From This Model / Copy From Other |
| Animation | Import Animation | Import animation |
| | Anim. Compression | Off / Keyframe Reduction / Optimal |
| Materials | Import Materials | Import materials |
| | Material Naming | By Base Textures / From Model |

### 🎮 Model Importer Configuration Example:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ModelOptimizer : MonoBehaviour
{
    [MenuItem("Tools/Optimize Models")]
    public static void OptimizeModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { "Assets/Models" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            
            if (importer == null) continue;
            
            importer.importMaterials = true;
            importer.materialName = ModelImporterMaterialName.BasedOnTextureName;
            importer.materialSearch = ModelImporterMaterialSearch.Local;
            
            importer.animationType = ModelImporterAnimationType.Generic;
            importer.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
            
            importer.importAnimation = true;
            importer.animationCompression = ModelImporterAnimationCompression.Optimal;
            
            importer.optimizeGameObjects = true;
            importer.extraUserProperties = false;
            
            importer.importLOD = true;
            importer.meshCompression = ModelImporterMeshCompression.Medium;
            
            importer.keepQuads = false;
            importer.weldVertices = true;
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log($"Optimized model: {path}");
        }
    }
}
#endif
```

---

## 4. Presets — Settings Templates
Presets allow you to save import settings and apply them to multiple assets.

### 📁 Creating a Preset:
1. Configure a Texture Importer for one asset
2. In the Inspector top right → Preset → Save Preset
3. Name it, e.g., `MobileTexture.preset`
4. The Preset is saved to Assets/Presets/

### 🔄 Applying Presets to Assets:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

public class PresetManager : MonoBehaviour
{
    [MenuItem("Tools/Apply Presets")]
    public static void ApplyPresets()
    {
        Preset texturePreset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/MobileTexture.preset");
        Preset audioPreset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/MobileAudio.preset");
        Preset modelPreset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/MobileModel.preset");
        
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D");
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && texturePreset.CanBeAppliedTo(importer))
            {
                texturePreset.ApplyTo(importer);
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log("All Presets applied!");
    }
}
#endif
```

---

## 5. Full Optimization Script
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AssetOptimizerFull : AssetPostprocessor
{
    [MenuItem("Tools/Full Asset Optimization")]
    public static void OptimizeAllAssets()
    {
        OptimizeTextures();
        OptimizeAudio();
        OptimizeModels();
        AssetDatabase.Refresh();
        Debug.Log("=== Asset optimization completed! ===");
    }
    
    static void OptimizeTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        int count = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;
            
            if (path.Contains("/UI/"))
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                continue;
            }
            
            TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
            androidSettings.overridden = true;
            androidSettings.format = TextureImporterFormat.ASTC_6x6;
            androidSettings.maxTextureSize = 2048;
            importer.SetPlatformTextureSettings(androidSettings);
            
            importer.mipmapEnabled = true;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.maxTextureSize = 4096;
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            count++;
        }
        
        Debug.Log($"Textures optimized: {count}");
    }
    
    static void OptimizeAudio()
    {
        string[] guids = AssetDatabase.FindAssets("t:AudioClip");
        int count = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
            if (importer == null) continue;
            
            if (path.Contains("/SFX/"))
            {
                importer.forceToMono = true;
                importer.defaultSampleSettings = new AudioImporterSampleSettings
                {
                    compressionFormat = AudioCompressionFormat.ADPCM,
                    sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate,
                    sampleRateOverride = 22050
                };
            }
            else if (path.Contains("/Music/"))
            {
                importer.defaultSampleSettings = new AudioImporterSampleSettings
                {
                    compressionFormat = AudioCompressionFormat.Vorbis,
                    sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate,
                    quality = 0.6f
                };
            }
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            count++;
        }
        
        Debug.Log($"Audio optimized: {count}");
    }
    
    static void OptimizeModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model");
        int count = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null) continue;
            
            importer.meshCompression = ModelImporterMeshCompression.Medium;
            importer.optimizeGameObjects = true;
            importer.weldVertices = true;
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            count++;
        }
        
        Debug.Log($"Models optimized: {count}");
    }
}
#endif
```

---

## 6. Best Practices
### ✅ Recommendations:
| Asset | Recommendation |
| --- | --- |
| Textures | Use ASTC on mobile, DXT on desktop |
| | Max size 2048 or 1024 for mobile |
| | Enable Mipmaps for 3D objects | 
| | Use Point filter for pixel art |
| Audio | Compress SFX in ADPCM, music in Vorbis/MP3 |
| | Use mono for SFX, stereo for music | 
| | Lower sample rate for SFX (22050 Hz) |
| Models | Use mesh compression (Medium) |
| | Optimize skeleton (remove unused bones) |
| | Disable animation for environments |

---

### ⭐ If this project was useful, put a star on GitHub!
