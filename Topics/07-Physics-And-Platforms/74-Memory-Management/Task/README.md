# 🎯 Task: «Texture Memory Optimizer for Mobile RPG»
You are developing a mobile open-world RPG game. 
The game uses hundreds of textures for various purposes: UI, characters, environment, effects. 
Many testers report crashes on devices with 2GB RAM. 
Your task is to create a texture memory management system.

## 📝 Initial Data:
The game has the following texture categories:

| Category | Texture count | Original size | Current format | Mipmaps |
| --- | --- | --- | --- | --- |
| UI icons | 50 | 256x256 | RGBA32 | No |
| UI backgrounds | 10 | 2048x2048 | RGBA32 | No |
| Characters | 20 | 2048x2048 | RGBA32 | Yes |
| Environment | 100 | 1024x1024 | RGBA32 | Yes |
| Effects | 30 | 512x512 | RGBA32 | No |

Current memory consumption (approximate):
- UI icons: 50 × (256×256×32/8) = 50 × 262KB ≈ 13 MB
- UI backgrounds: 10 × (2048×2048×32/8) = 10 × 16 MB = 160 MB
- Characters: 20 × (2048×2048×32/8 × 1.33 with mipmaps) = 20 × 21.3 MB ≈ 426 MB
- Environment: 100 × (1024×1024×32/8 × 1.33) = 100 × 5.33 MB ≈ 533 MB
- Effects: 30 × 512×512×32/8 = 30 × 1 MB = 30 MB

TOTAL: ~1162 MB (textures only!)

---

## 📋 Optimization Tasks:
### 1. Apply Texture Compression (Most Important)
For each category, choose the optimal compression format:

| Category | Format | Why |
| --- | --- | --- |
| UI icons | ASTC 4x4 | Maximum quality for small details |
| UI backgrounds | ASTC 6x6 | Good quality for large areas | 
| Characters | ASTC 5x5 | Balance of quality and memory savings |
| Environment | ASTC 8x8 | Memory savings for background objects |
| Effects | ASTC 6x6 | Good for particles |

Calculate new memory after compression (write the formula)

### 2. Optimize Texture Resolutions
Some textures have excessive resolution. Reduce them:
- UI backgrounds: from 2048x2048 to 1024x1024 (no need larger than screen)
- Environment: distant objects can be reduced to 512x512
- Characters: for non-playable NPCs — down to 1024x1024

### 3. Create Sprite Atlases
Combine into atlases:
- All UI icons into one 2048x2048 atlas (ASTC 4x4)
- UI backgrounds into separate atlases (one background per 1024x1024 atlas)
- Particle effects into one 2048x2048 atlas

### 4. Implement Texture LOD System (Programmatically)
Write code that:
- Detects device RAM amount
- On < 2GB RAM, loads textures at half resolution
- On > 3GB RAM, loads at full quality
- On memory pressure, unloads textures of distant objects

```csharp
public class TextureMemoryOptimizer : MonoBehaviour
{
    // TODO: Implement:
    // 1. Detect device memory via SystemInfo.systemMemorySize
    // 2. Dictionary with texture categories and priorities
    // 3. Method to unload textures by priority on memory pressure
    // 4. Method to load textures at target resolution (via AssetBundle or Resources)
}
```

### 5. Configure Mipmaps Correctly
Fix mipmap settings for each category:
- UI: Disable mipmaps (save 33% memory)
- Characters: Enable (they move around)
- Environment: Enable (different distances)
- Effects: Disable (usually close to camera)

### 6. Write Memory Monitoring
Create a script that every 30 seconds logs:
- Total texture memory
- Memory per category
- Warning when exceeding 300 MB

---

## 📊 Expected Results (Goal):
After all optimizations, texture memory should be < 150 MB instead of 1162 MB.

---

## 🧰 Implementation Requirements:
- Create `TextureMemoryOptimizer` script with all logic
- Use `[Serializable]` for category configuration in Inspector
- Add conditional compilation for `UNITY_ANDROID` and `UNITY_IOS`
- Implement editor emulation (via `#if UNITY_EDITOR`)
- Add buttons in Inspector for manual testing (`[ContextMenu]`)

---

## 💡 Hints:
- Use formula for memory calculation: `width * height * bitsPerPixel / 8 / 1048576` (in MB)
- For ASTC compression ratio: for 6x6 — 3.56 bits/pixel
- In Unity Editor, you can emulate memory via `UnityEditor.EditorPrefs`
- Use `Resources.UnloadUnusedAssets()` for forced unloading

---

## 🔍 Final Checklist:
- All textures converted to ASTC with appropriate block sizes
- Texture resolutions optimized for mobile devices
- Sprite atlases created for UI and effects
- Texture LOD system based on device memory implemented
- Mipmaps configured correctly
- Memory monitoring with logging added
- Tested on real device with 2GB RAM (no crashes)

---

### ⭐ If this project was useful, put a star on GitHub!
