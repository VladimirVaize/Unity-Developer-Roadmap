# 🎯 Practical Task: Quest Window Using TextMeshPro

Objective: Create a UI panel displaying a quest text, demonstrating key TextMeshPro features: custom font, outline material, animation, and Rich Text tags.

## 📝 Task
Create a Unity scene with a UI panel that displays a quest description. Use TextMeshPro for all text elements.

### Requirements:
1.  Font 🖋️
   - Download a free font (e.g., Open Sans or Montserrat).
   - Import it into Unity.
   - Create a Font Asset for this font (atlas size: 1024x1024, character set: Unicode).
   - Apply this Font Asset to your TMP text.
2. Material with Effects ✨
   - Do NOT use the default TMP material.
   - Create a new material based on the `TextMeshPro/Mobile/Distance Field Outline` shader.
   - Configure:
     - Outline Width = 0.2
     - Outline Color = gold (`#FFD700`)
     - Face Color (fill color) = dark blue (`#1A2A4A`)
3. Text Formatting with Rich Text Tags 🏷️

The quest text must include:
- A bold quest title (`<b>`)
- An italic goal description (`<i>`)
- At least one colored fragment (`<color>`)
- An inline coin icon (`<sprite>`) — create a simple coin sprite or use a built-in one
- An increased font size for an important number (`<size>`)
- A slight rotation (`<rotate>`) for one word (e.g., "special")

4. Text Animation (minimal) 🎬
   - Write a simple C# script that animates the text in one of the following ways:
     - Alpha pulsation of the entire text (from 1 to 0.5 and back) — cycle every 2 seconds.
     - OR size wobble (`fontSize` +- 10%) for a word wrapped in a `<link>` tag.
    
---

## 🌟 Bonus (optional):
- Add click handling for the `<link>` tag (print a message to the console when clicked).

---

### ⭐ If this project was useful, put a star on GitHub!
