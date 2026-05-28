# 🎮 Sprite Shape and Tilemap: Creating 2D Levels (Platformers)

This material covers two powerful Unity tools for building 2D levels: Tilemap 
(for grid-based, pixel-perfect, crisp levels) and Sprite Shape (for smooth, organic, curved forms). 
They are perfect for platformers, RPGs, Metroidvanias, and other 2D games.

---

## 🧱 Tilemap — for "Tile-Based" Levels
### What is it?
Tilemap is a system that lets you paint levels using "tiles" — like a mosaic or pixel art. 
Each tile is a small sprite (e.g., a piece of ground, grass, wall). 
You work on a grid, giving you perfect control over collisions and physics.

### 📦 Main Components
- Grid — an invisible grid that holds everything.
- Tilemap — a layer with tiles (one layer = one "material": ground, grass, water).
- Tile Palette — an editor window where you store and select tiles.
- Tilemap Collider 2D + Composite Collider 2D — create physical boundaries matching the shape of painted tiles.

### 🛠️ How to Use (Step by Step)
1. Create a Grid and Tilemap<br>
   `RMB in Hierarchy → 2D Object → Tilemap → Rectangular Tilemap`

2. Open the Tile Palette<br>
   `Window → 2D → Tile Palette`<br>
   Click `Create New Palette`, choose cell size (usually 16x16 or 32x32 pixels).

3. Add Tiles<br>
   Drag sprites (cut into cells) into the `Tile Palette` window. Unity will ask how to slice them — choose `Grid by Cell Size`.

4. Paint your level<br>
   Select a brush in the palette, click and drag in the scene to paint tiles.<br>
   Use the Eraser Brush to delete.

5. Set up collisions<br>
   Select the Tilemap in Hierarchy → `Add Component` → `Tilemap Collider 2D` → then `Composite Collider 2D`.<br>
   Check `Used by Composite` on `Tilemap Collider 2D`. Physics will perfectly match the tile shapes.

### ✅ What Tilemap is good for
- Precise platformers (like Celeste, Super Mario)
- Dungeons (Roguelike)
- Towns / houses / rooms with clear boundaries

---

## 🎢 Sprite Shape — for Smooth and Curved Levels
### What is it?
Sprite Shape is a tool for creating flexible, organic forms: curved platforms, hills, caves, rounded walls. 
You define control points, and Unity automatically fills the space with sprites, smoothly connecting them.

### 📦 Main Components
- Sprite Shape Profile — settings: which sprites to use for borders, corners, fill.
- Sprite Shape Controller — the component on an object that draws the shape from your points.
- Open / Closed Shape — an open line (platform) or a closed shape (island, cave).

### 🛠️ How to Use (Step by Step)
1. Create a Sprite Shape Profile<br>
   `RMB in Project → Create → 2D → Sprite Shape Profile → Open Shape` (for platform) or `Closed Shape` (for island).

2. Configure sprites in the Profile
   - Border — the main line sprite.
   - Fill — sprite for the interior (for Closed Shape).
   - Corners — optional, for sharp turns.
  
3. Create a controller in the scene<br>
   `RMB in Hierarchy → 2D Object → Sprite Shape → Open Shape`<br>
   An empty curved line appears.

4. Draw the shape<br>
   In `Edit Spline` mode (click `Edit Spline` on the controller in the Inspector), move points (Ctrl + click — add point).<br>
   Points can toggle between `Linear` (angle) and `Bezier` (smooth curve).

5. Add collisions<br>
   On the Sprite Shape object, add components:<br>
   `Polygon Collider 2D` (for closed shape) or `Edge Collider 2D` (for open line).<br>
   Or use `Sprite Shape Collider` — automatic physics matching the shape.

### ✅ What Sprite Shape is good for
- Smooth platformers (Ori and the Blind Forest)
- Organic caves, hills, clouds
- Backgrounds (distant mountains, curved horizons)

---

## ⚔️ Tilemap + Sprite Shape: Combining Both
#### Nothing stops you from using both tools in one game:
- Tilemap — for the main level: ground blocks, ladders, room walls.
- Sprite Shape — for decorations: tree roots, winding paths, background hills.

#### Platformer example:
The floor of a room — Tilemap (crisp tiles). A smooth earthen ledge hanging over a chasm — Sprite Shape. The player can walk on both if you add colliders.

---

## 🧠 Summary

| Tool | Grid | Shape | Style | Best for |
| --- | --- | --- | --- | --- |
| Tilemap | Yes | Rectangular | Pixel-perfect, crisp | Walls, floors, dungeons |
| Sprite Shape | No | Arbitrary / smooth | Organic, smooth | Hills, caves, arched platforms |

---

### ⭐ If this project was useful, put a star on GitHub!
