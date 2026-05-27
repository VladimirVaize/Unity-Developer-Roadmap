# 🧪 Practical Task: URP + Render Feature "Highlight on Hover"

Goal: Set up URP in a project, create a simple scene with several cubes, and implement a highlight effect when hovering over an object without writing a shader – using only Render Features and standard materials.

---

## 📌 Task (step by step)
### 🔧 Part 1: URP Setup
1. Create a new Unity project (3D template) or use an existing one.
2. Create a URP Asset and assign it in Project Settings (as described in the theory).
3. Upgrade scene materials if needed.

### 🧱 Part 2: Create a scene
4. Create 3–5 cubes (`GameObject → 3D Object → Cube`) at different positions.
5. Assign each cube a different standard colored material (e.g., red, blue, green) – use URP Lit Shader.
6. Add a `Camera` (already there) and a `Directional Light`.

### ✨ Part 3: Create a "Highlight On Hover" Render Feature
Task: When hovering the mouse over a cube – the cube should be highlighted with a bright white outline or fill without changing its original material.

#### 🛠️ Implementation using Render Objects Feature:
1. Select your Forward Renderer Asset (usually inside the URP Asset).
2. Click `Add Render Feature` → choose `Render Objects`.
3. Name it `Highlight Feature`.
4. Configure:
   - `Event` = `AfterRenderingOpaques`.
   - `Filters` → `Layer Mask` = create a new layer `Highlightable` and assign it to all cubes.
   - `Overrides` → `Material` = create a new material (URP Lit), white color, `Surface Type` = `Opaque`, `Emission` = white (or just bright color).
   - `Render Queue` = `Geometry+1` (to draw on top).
  
5. Leave the feature disabled for now (uncheck the checkbox next to its name).

### 🖱️ Part 4: Scripting (toggling the feature)
6. Create a C# script `HighlightOnHover.cs` and attach it to the cubes.
```csharp
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HighlightOnHover : MonoBehaviour
{
    private UniversalAdditionalCameraData cameraData;
    private ScriptableRendererFeature highlightFeature;

    void Start()
    {
        cameraData = Camera.main.GetUniversalAdditionalCameraData();
        // Find our Render Feature by name (replace "Highlight Feature" with your name)
        var renderer = cameraData.scriptableRenderer as UniversalRenderer;
        if (renderer != null)
        {
            highlightFeature = renderer.rendererFeatures.Find(f => f.name == "Highlight Feature");
        }
    }

    void OnMouseEnter()
    {
        if (highlightFeature != null) highlightFeature.SetActive(true);
    }

    void OnMouseExit()
    {
        if (highlightFeature != null) highlightFeature.SetActive(false);
    }
}
```

> [!Note]
> Accessing Render Features via code requires a bit of extra work (or a public static field).
> For simplicity, you could instead temporarily change the object's material – but the goal is to learn Render Features.

#### Alternative simple approach (without finding the feature):
In `OnMouseEnter()`, create a temporary duplicate object with a white material; in `OnMouseExit()`, destroy it. But that's a hack. The proper way is enabling the feature.

---

## 🎯 Expected result:
- When hovering the mouse over any cube, the entire cube becomes highlighted with a bright color (white/yellow).
- When the mouse leaves, the highlight disappears.
- The cubes' original materials remain unchanged.
- The effect works without performance loss.

---

## ⭐ Bonus task (advanced):
- Make only the outline of the object glow using a Render Feature with an `Unlit/Texture` material and Stencil buffer.
- Add a second Render Feature – `Motion Blur` for the entire camera, but only when the player holds down the Shift key.

---

### ⭐ If this project was useful, put a star on GitHub!
