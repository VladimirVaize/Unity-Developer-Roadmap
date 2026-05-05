# TextMeshPro (TMP): Modern Typography in Unity

TextMeshPro (often abbreviated TMP) is a powerful text rendering solution for Unity that replaces the legacy `Text` component. 
TMP delivers superior quality, flexibility, and performance using SDF (Signed Distance Field) technology.

---

## ✨ Advantages over Legacy Text

| Feature | Legacy Text | TextMeshPro |
|------------------|--------------------|-----------------------|
| Quality when scaling | Pixelation, blur | Sharp, crisp at any size |
| Anti-aliasing | Limited | Excellent via SDF |
| Shadows & outlines | Require extra objects or shaders | Built-in styles (Outline, Shadow, Bevel) |
| Icon library | None | Built-in sprites (Emoji, icons) |
| Rich Text tags | Basic (bold, italic, color, size) | Extended (gradients, rotation, shadows, sprites) |
| Performance | Good for simple text | Great for complex text (single mesh for all text) |
| Wrapping & alignment | Basic | Advanced (flexible word breaking, indents) |

Key advantage: SDF technology allows vector font information to live inside a texture, so text stays sharp and smooth when scaled, rotated, or even during camera movement.

---

## 🔤 Font Assets
TMP does not use system fonts directly. Instead, you create a Font Asset — a special Unity resource generated from a `.ttf` or `.otf` file.

### How to create a Font Asset:
1. Import a `.ttf` or `.otf` file into your `Assets` folder.
2. Right-click the file → `Create` → `TextMeshPro` → `Font Asset`.
3. Adjust settings (texture size, character set, padding) and click `Generate Font Atlas`.

### Key parameters:
- Atlas Resolution — size of the texture map (512x512, 1024x1024...)
- Character Set — which characters to include (ASCII, Unicode, or manual)
- Padding — spacing between characters in the atlas (affects outline sharpness)
- Rendering Mode — SDF mode (usually `SDFAA` or `SDFAA_HINTED`)

Usage example: Create a Font Asset for your favorite font, and it will look equally sharp on a 4K screen and on a mobile device.

---

## 🎨 Materials
Each TMP component uses a material that determines how the text is rendered. Unity provides several built-in TMP materials:

| Material | Effect |
|-------------|------------------|
| `TMPro Material` | Basic text with SDF support |
| `TMPro Material Outline` | Adds an outline around letters |
| `TMPro Material Glow` | Glow effect |
| `TMPro Material Bevel` | 3D bevel / emboss effect |
| `TMPro Material Gradient` | Gradient fill |

Configuration example: Select a TMP component → in the `Material` field, choose `TMPro Material Outline`. 
Then in the Inspector, adjust `Outline Width = 0.2` and `Outline Color = red` — each letter will get a neat red outline.

---

## 🎬 Text Animation
TMP does not have a built-in animation component, but you can animate text:

1. Via code: Change `color`, `fontSize`, `vertexPosition` properties per character (using `TMP_TextInfo`).
2. Via Vertex Shader: Advanced approach — write a custom shader that animates text vertices (wave, ripple, offset).
3. Via Rich Text + AnimationCurve: Combine tags and script-driven animation.

### Simple script example (C#):
```csharp
// Text size pulsation
float pulse = Mathf.Sin(Time.time * 5f) * 0.2f + 1f;
textComponent.fontSize = 36 * pulse;
```

---

## 🏷️ Rich Text Tags (Extended Markup)
TMP supports all Legacy tags plus many unique ones. Tags are inserted directly into the text string.

### Basic tags (work in both Legacy and TMP):

| Tag | Description | Example |
|---------------|-----------------|-----------------------|
| `<b>...</b>` | Bold | `<b>Bold text</b>` |
| `<i>...</i>` | Italic | `<i>Italic</i>` |
| `<size=30>...</size>` | Font size | `<size=50>Large</size> normal` |
| `<color=red>...</color>` | Color (by name or #RRGGBB) | `<color=#FF0000>Red</color>` |

### Extended TMP tags (unique):

| Tag | Description | Example |
|-------------|------------------|-----------------------|
| `<alpha=#AA>` | Transparency (HEX) | `<alpha=#88>Semi-transparent text</alpha>` |
| `<cspace=5>` | Character spacing | `<cspace=10>S p a c e d</cspace>` |
| `<voffset=10>` | Vertical offset | Normal text `<voffset=15>raised</voffset>` |
| `<rotate=15>` | Character rotation | `<rotate=10>Slanted</rotate>` |
| `<pos=100px>` | Absolute positioning | Text `<pos=200px>shifted right` |
| `<sprite index=0>` | Inline sprite (icon) | Click `<sprite name="icon_play">` to start |
| `<gradient>` | Gradient (must be defined in material) | `<gradient>Rainbow text</gradient>` |
| `<link="myid">...</link>` | Creates a clickable region | `<link="shop">Buy</link>` (handled via `OnPointerClick`) |

### Practical example string with tags:
```text
This is <b>bold</b> and <i>italic</i> text.
<size=40>Huge text</size> with a <color=yellow>yellow</color> accent.
<alpha=#66>Semi-transparent</alpha> and <rotate=10>rotated</rotate>.
Press <sprite name="next"> to continue.
```

### Creating a custom sprite for the `<sprite>` tag
1. Prepare a PNG icon.
2. Import into Unity, set `Sprite Mode = Multiple` (if needed).
3. Open `Window → TextMeshPro → Sprite Asset`.
4. Create a new Sprite Asset and add your sprites to it.
5. In the TMP component, assign this Sprite Asset to the `Sprite Asset` field.
6. Use the tag: `<sprite name="myIcon">`.

---

### ⭐ If this project was useful, put a star on GitHub!
