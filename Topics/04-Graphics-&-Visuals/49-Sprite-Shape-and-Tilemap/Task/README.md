# 🎯 Practical Task: Platformer Cave
## 📝 Task Description
Your task is to create a small level for a 2D platformer using Tilemap for the main ground and walls, and Sprite Shape for an organic cave arch or a smooth hill.

## 🔧 Requirements
1. Unity Scene (2D)
   - Create a new 2D scene.
   - Add a `Grid` object and one `Tilemap` for the ground (main floor).
  
2. Tilemap part
   - Paint the floor and walls (minimum 10×5 cells).
   - Add `Tilemap Collider 2D` + `Composite Collider 2D` for physics.
   - Use any tiles (you can use default ones from the `2D Tilemap Extras` package or your own simple sprites).
  
3. Sprite Shape part
   - Create a `Sprite Shape Profile` (Closed or Open Shape).
   - Add a `Sprite Shape Controller` to the scene.
   - Create a smooth curve that overhangs part of the Tilemap (like a cave ceiling) or a smooth hill that the player can jump onto.
   - Set up collision (`Polygon Collider 2D` or `Sprite Shape Collider`).
  
4. Character (for testing)
   - Add a simple square player with `Rigidbody 2D`, `Box Collider 2D`, and a script for left/right movement and jumping.
   - Ensure the player can:
     - Walk on the Tilemap (floor).
     - Jump onto the Sprite Shape (hill/arch).
     - Not fall through the physics of either object.
    
5. Visual differences
   - Paint the Tilemap in one color (or use ground sprites).
   - Paint the Sprite Shape in a different color or texture (e.g., grass/stone).
  
---

## ✨ Evaluation criteria
- Correct collision setup on both types of objects.
- Seamless combination of Tilemap and Sprite Shape in one level.
- Functioning physics (the player does not fall through platforms).
- At least one smooth curve (Bezier point) in the Sprite Shape.

---

## 🧪 Bonus (optional)
- Add a camera that follows the player (Cinemachine 2D or a simple script).
- Make the Sprite Shape use Fill (only for Closed Shape).

---

## 📦 Useful links
- Built-in packages: `Window → Package Manager → 2D Tilemap Extras`, `2D Sprite Shape`.
- Use your own simple player movement script (or standard examples).

---

### ⭐ If this project was useful, put a star on GitHub!
