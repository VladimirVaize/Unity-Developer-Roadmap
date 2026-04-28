# 🎨 Unity UI Components: RectTransform, Anchors, Layout Groups

> [!Note]
> This material covers creating a responsive user interface (UI) in Unity.
> Unlike regular 3D objects, UI elements are managed via RectTransform, and their position and size relative to the screen are determined by Anchors and Layout Groups.

---

## 📐 RectTransform
### Purpose:
`RectTransform` is a special transformation component for UI elements (buttons, panels, text, images). 
It extends the regular Transform with the concepts of Width, Height, Anchors, and Pivot.

### How to use:
- Select any UI element (e.g., an Image) in the Hierarchy. You will see the RectTransform component in the Inspector.
- Width / Height — set the size of the element's rectangle.
- Pos X, Pos Y — position relative to anchors or parent.
- Pivot — the point for rotation and scaling (0,0 = bottom-left corner, 1,1 = top-right corner).
- Blue arrows/squares in the Scene view allow you to resize and rotate visually.

### Example:
🟢 You create a `Button`. In RectTransform, set `Width = 160`, `Height = 60`. Leave Pivot at (0.5, 0.5) — center. Now the button rotates and scales from its center.

---

## ⚓ Anchors
### Purpose:
Anchors define which part of the parent element (or screen) the UI element is attached to. 
This is the foundation of responsiveness across different screen resolutions. 
Every RectTransform has 4 anchors (Min X, Max X, Min Y, Max Y) that you can move.

### How to use:
- In the Scene view, select a UI element. Small white "radial" handles appear at its corners — these are the anchors.
- The fastest method: in the Inspector, click the square Anchor Presets icon (left of the word "RectTransform").
  - Stretch to full parent: Shift + click the bottom‑right preset (arrows pointing in all directions).
  - Attach to left edge: select the left‑aligned preset.
  - Attach to top‑right corner: pick the top‑right preset.
- You can also manually enter Min/Max X/Y values in the Inspector.

### Example:
📱 A `Pause` button should always stay in the top‑right corner, regardless of screen width.
→ Choose the Anchor Preset → top‑right (arrow pointing right/up). 
Now Pos X/Y are measured from the top‑right corner of the parent (usually the Canvas).

---

## 🧩 Layout Groups (Containers)
### Purpose:
`Horizontal Layout Group`, `Vertical Layout Group`, and `Grid Layout Group` automatically arrange child UI elements. 
They eliminate manual positioning and sizing.

### How to use:
1. Create an empty GameObject (e.g., `MenuPanel`) and add a `Horizontal Layout Group` component.
2. Add several child elements (buttons, icons) inside it.
3. Configure the Layout Group parameters:
   - Padding — inner margins.
   - Spacing — distance between elements.
   - Child Alignment — vertical/horizontal alignment.
   - Child Controls Size — allows the container to control children's width/height.
   - Child Force Expand — forces children to stretch to fill available space.
  
### Example:
🎮 An inventory panel needs 4 cells in a row.
→ Create `InventoryGrid`, add a `Grid Layout Group`. Set `Cell Size = 80x80`, `Spacing = 10`. 
Add 4 child Images — they will automatically align into a grid with equal sizes and gaps.

---

## 🧠 Putting It All Together
Most often you use all three tools together:
1. The main Canvas contains a container (Vertical Layout Group) for the settings screen.
2. Inside the container are buttons with a fixed RectTransform height.
3. Each button is anchored so that it maintains the desired width when the container stretches.
4. When the screen resolution changes, everything adjusts automatically.

---

### ⭐ If this project was useful, put a star on GitHub!
