# 🧪 Practical Task: Player Settings Panel

## 📝 Task Description
You need to create a small settings panel (UI Canvas) in Unity that allows the player to adjust character parameters in real time. Use the following UI elements:

- **Button** – to save settings and exit.
- **Slider** – to adjust the character's movement speed.
- **Toggle** – to enable/disable infinite health.
- **ScrollRect** – to display a list of available skins (colors) for the character.

---

## 🎯 Requirements

1. Speed Slider
   - Range: from 2 to 10.
   - A `Text` next to the Slider should display the current speed value (e.g., "Speed: 5.5").
   - When the slider changes, the character's speed must change in real time.
  
2. Health Toggle
   - If the Toggle is ON (`isOn == true`), the character does NOT lose health (simply log `"God mode ON"` to the console).
   - If off, the character is vulnerable (`"God mode OFF"`).
  
3. ScrollRect with Skins
   - Inside the `ScrollRect`, create 5 buttons with color names: `"Red"`, `"Green"`, `"Blue"`, `"Yellow"`, `"Purple"`.
   - When a skin button is clicked, the character's color (e.g., an `Image` or `SpriteRenderer`) changes to the corresponding color.
   - The list must be scrollable (vertically or horizontally – your choice).
  
4. "Save and Exit" Button
   - Saves all current settings to `PlayerPrefs`.
   - Logs to the console: `"Settings saved: Speed = X, GodMode = Y, Skin = Z"`.
   - After that, disables the settings panel (or returns to the main menu).
  
5. Load settings on start
   - When the scene starts, settings should be loaded from `PlayerPrefs` (if they exist) and applied to the character and UI elements.
  
---

## 🧱 What you need to create
- A scene containing:
  - A "Player" game object (at least a `Cube` with an `Image` or `SpriteRenderer` to visualize the skin).
  - A UI Canvas with a settings panel (`Panel`).
  - A separate script `PlayerSettingsUI` that controls all the logic.
 
- References in the script to all necessary UI elements and to the player.

---

## ⭐ Bonus (optional)
- Add button click animations.
- Make the selected skin button highlight in the ScrollRect.
- Add a second Toggle – "Invert controls" – and show its effect via the console.

---

### ⭐ If this project was useful, put a star on GitHub!
