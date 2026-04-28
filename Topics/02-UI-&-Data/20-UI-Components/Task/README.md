# 🧪 Practical Task: Adaptive Main Menu Panel

### Goal:
Create a main menu panel that works on any screen resolution (from smartphone to wide monitor) and uses all three learned components: RectTransform, Anchors, and Layout Groups.

---

## 📋 Requirements
### You need to implement a Unity UI that contains:
- A title (text "Menu") – pinned to the top edge, horizontally centered.
- Three buttons ("Play", "Settings", "Quit") – arranged vertically one below another in the center of the screen.
- An info panel – appears when hovering over any button (a tooltip‑like description to the right of the button).
  This panel must be anchored to the right edge of the screen and automatically change its height depending on the length of the text.

### Implementation requirements:
1. Use `Vertical Layout Group` for vertical button alignment.
2. Configure the title's anchors so that it stays pinned to the top and remains centered when the width changes.
3. The info panel must be created as a child of the Canvas, with its `RectTransform` anchored to the right edge (Anchors: right, top + bottom stretch, or right + top but with dynamic height).
4. Inside the info panel, use a `Content Size Fitter` (Vertical Fit = Preferred Size) to automatically change the panel's height.
5. All buttons must have the same width, stretching to fill the container's width, but with left and right margins.

---

## 🔧 Expected Outcome
- At 1920x1080 resolution, the menu panel looks proportional.
- At 800x600 resolution, the buttons are not clipped, the title stays at the top, and the tooltip panel does not go off screen.
- When a long tooltip is added (e.g., "Opens the save game loading screen and profile settings"), the info panel increases in height and the text inside wraps properly.

---

### ⭐ If this project was useful, put a star on GitHub!
