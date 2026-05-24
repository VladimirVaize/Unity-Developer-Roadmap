# 💡 Lighting in Unity: Light Types and Lightmapping
This material explains three main light types — Directional, Point, Spot — as well as the key concept of Baked vs Realtime lighting and Lightmaps.

---

## 🎯 1. Directional Light
### Purpose:
Simulates the sun or any distant light source (moon, far spotlight). Rays travel parallel to each other, so an object is lit the same way regardless of distance.

### How to use:
- By default, present in every new Unity scene.
- Control rotation (Transform) — tilt determines time of day.
- Change Color and Intensity in Inspector → Light.
- Enable shadows (Shadow Type: Hard/Soft).

### Example:
Rotate Directional Light down by 90° — you get noon (shadows straight down). Tilt toward the horizon — long shadows, like at sunset.

---

## 🔴 2. Point Light
### Purpose:
Shines in all directions from a single point, like a light bulb, candle, or torch. Intensity fades with distance.

### How to use:
- Create: `GameObject → Light → Point Light`.
- `Range` — radius of effect (no light beyond it).
- `Intensity` — light strength.
- `Color` — e.g., orange for a torch.

### Example:
You hang a lamp over a table. Place a Point Light inside the lamp model, set Range = 5, Intensity = 1.5, color = warm yellow. Now the lamp only lights the table area.

---

## 🟡 3. Spot Light
### Purpose:
A cone of light, like a flashlight, stage spotlight, car headlight, or street lamp.

### How to use:
- Create: `GameObject → Light → Spot Light`.
- `Spot Angle` — cone width (narrow or wide beam).
- `Range` — distance.
- Aim by rotating Transform.

### Example:
You're making a horror level. Place a Spot Light on the character, point it forward, Spot Angle = 30°, Range = 10. The player only sees what's in the flashlight beam — everything else is dark.

---

## 🧱 Baked vs Realtime Lightmap
## ⚡ Realtime Lighting
### How it works:
The light source recalculates lighting and shadows every frame. You can change color, intensity, position, turn on/off lights during gameplay.

### When to use:
- Moving sources (flashlight in hand, flying sparks).
- Changing conditions (real-time day/night cycle).
- Interactive objects (turn on a lamp — light appears).

### Downsides:
Heavy CPU/GPU cost, especially with multiple lights and soft shadows.

---

## 🧱 Baked Lighting + Lightmap
### How it works:
Unity pre-calculates (at edit time, before game runs) how light falls on static objects and saves the result into a texture — a Lightmap. 
In the game, lighting is simply "applied" to objects like a texture. This is very fast.

### When to use:
- Static scenes (buildings, ground, walls, furniture that doesn't move).
- Mobile games and VR — need performance.
- Beautiful, predictable lighting with no runtime cost.

### How to set up:
1. Select an object (e.g., floor, wall).
2. In Inspector → Static dropdown → check `Contribute GI` or mark object as `Static`.
3. For the light source, set Mode = `Baked`.
4. Window → Rendering → Lighting → Generate Lighting (or enable Auto Generate).

### Example:
You build a room (walls, floor, ceiling, pillars — all Static). Place a Directional Light (Mode = Baked) and a Point Light (Mode = Baked) in a chandelier. 
Unity creates a Lightmap — a texture with shadows and bright spots. In the game, no lighting calculations — high performance, beautiful image.

---

## 🔄 Mixed Lighting
### Mixed Mode:
A Mixed light gives baked lighting (from Lightmap) to static objects but can also dynamically light moving objects (characters, cars) in real time. The best compromise for most games.

---

## 🧠 Quick Summary Table

| Light Type | Shape | Dynamic? | Example |
| --- | --- | --- | --- |
| Directional | Parallel rays | Yes/No | Sun |
| Point | Sphere | Yes/No | Bulb |
| Spot | Cone | Yes/No | Flashlight |

| Mode | Performance | Changeable at runtime | Best for |
| --- | --- | --- | --- |
| Realtime | Heavy | ✅ Yes | Moving lights |
| Baked | Very fast | ❌ No | Static scenes |
| Mixed | Medium | Partially | Static + dynamic |

---

### ⭐ If this project was useful, put a star on GitHub!
