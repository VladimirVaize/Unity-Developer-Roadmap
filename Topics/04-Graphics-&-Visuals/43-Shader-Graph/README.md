# 🎨 Shader Graph: Visual Shader Creation Without Code

> [!Note]
> Shader Graph is a powerful visual tool in Unity that allows you to create shaders by connecting nodes in a graph, instead of writing HLSL/GLSL code.
> It's ideal for artists, designers, and programmers who want to quickly prototype materials with advanced graphics.

> [!Important]
> ⚠️ Requirement: Shader Graph only works with Universal Render Pipeline (URP) or High Definition Render Pipeline (HDRP). Built-in render pipeline is not supported.

---

## 📦 Shader Types in Shader Graph
In Shader Graph, you choose a Master Stack template when creating a shader. Three main types:

### 1. Lit Shader 💡
- Purpose: Objects that interact with light (sources, shadows, reflections). Most game objects.
- Features: Reacts to light direction, color, and intensity in the scene. Has parameters: Albedo (color/texture), Normal Map (surface detail), Metallic, Smoothness, Emission.
- Use case example: Characters, environment (rocks, trees), vehicles, weapons.

### 2. Unlit Shader 🌟
- Purpose: Objects that should NOT react to scene lighting. They always look the same regardless of lights.
- Features: Only color/texture, emission, transparency. Very performant.
- Use case example: UI elements, sprites (2D), flat/stylized effects, holograms, fixed-color screens.

### 3. VFX Shader ✨
- Purpose: Specialized shaders for particles (Particle System) or effects that don't need complex lighting.
- Features: Often use transparency, additive blending, texture animation over time, dissolve effects.
- Use case example: Explosions, fire, smoke, magic, sparks, UI "flashes".

---

## 🧠 Basic Nodes to Start With
Nodes are the "building blocks" you connect. Here are the most important:

| Node | What it does | Output |
| --- | --- | --- |
| `Texture 2D` | Allows loading a texture | Color (RGBA) |
| `Sample Texture 2D` | Reads a texture with UV coordinates | Color, alpha |
| `Multiply` | Multiplies two values (colors or numbers) | Multiplication result |
| `Add` | Adds values | Sum |
| `Lerp` (Linear Interpolate) | Blends two values based on a third (alpha) | Smooth transition |
| `Time` | Returns current game time | Sine, Cosine, Delta Time |
| `UV` | Object's texture coordinates (0-1) | Vector2 (U, V) |
| `Color` | Sets a constant color | RGBA |
| `Normal Vector` | Allows distorting normals (for surface detail) | Vector3 |
| `Polar Coordinates` / `Rotate` | Transforms UV for circular effects | UV |

---

## 🛠️ How to Create Your First Shader in Shader Graph
### Step-by-step example: "Pulsing Color" (Unlit)

1. Create the shader: In the `Project` window → right-click → `Create` → `Shader Graph` → `URP` → `Unlit Shader Graph`. Name it `PulseColor`.
2. Open the editor: Double-click the `.shadergraph` file.
3. Add parameters: In the `Blackboard` (left panel) click `+` → `Color` → name it `BaseColor`. Add `Vector1` (Float) → name it `Speed`.
4. Build the graph:
   - Drag `BaseColor` and `Speed` into the graph.
   - Add a `Time` node → take its `Sine Time` output.
   - Multiply (`Multiply`) `Sine Time` by 0.5 and add (`Add`) 0.5 (so the value stays between 0 and 1).
   - Create a second color (e.g., red) using a `Color` node.
   - Blend (`Lerp`) `BaseColor` and the red color using the computed value as `T` (alpha).
   - Connect the `Lerp` result to the Master Stack → `Base Color`.
  
5. Save: `Save Asset` (Ctrl+S).
6. Apply: Create a Material from this shader (right-click on the shader → `Create` → `Material`).
   Assign the material to any object in the scene. The color will pulse smoothly!

---

## 💡 Tips and Best Practices
- 🏷️ Name parameters in Blackboard meaningfully (`Speed`, `GlowIntensity`) so they're easy to tweak in the material.
- 🔍 Use Preview (click on a node → small window) to see the output of a node without connecting it to the final output.
- ⚡ Unlit is faster than Lit. For mass objects (hundreds of particles), prefer Unlit if complex lighting isn't needed.
- 📂 Keep the graph tidy: arrange nodes neatly, group them using `Group` (select several → right-click → `Group`).
- 🎥 Visualize UV: plug UV coordinates directly into `Base Color` — you'll see a color gradient, useful for debugging.

---

## 🔗 Where to Learn More
- Official Unity documentation: <a href="https://docs.unity3d.com/Packages/com.unity.shadergraph@17.6/manual/index.html">Shader Graph</a>
- Free node search: `Ctrl+G` to search nodes by name.
- Examples: Look for "Shader Graph Demo" by Unity in the Asset Store.

---

### ⭐ If this project was useful, put a star on GitHub!
