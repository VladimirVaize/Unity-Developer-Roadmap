# 🎯 Task: «Optimizing a City with LOD»
You are developing a city scene with many buildings. You need to implement an LOD system for 4 building types that will:
1. Automatically swap models based on distance from the camera
2. Smoothly transition between detail levels
3. Adapt to performance (Dynamic LOD)
4. Use billboards for the farthest buildings

## 📝 What to Implement:
### Part 1: Creating LOD Group for a Building
1. Create a building prefab named `Building_LOD_Template`:
   - `LOD0`: detailed model (10,000 polygons, 4K textures)
   - `LOD1`: medium model (5,000 polygons, 2K textures)
   - `LOD2`: simple model (1,000 polygons, 512x512 textures)
   - `LOD3`: billboard (flat sprite with building texture)
  
2. Configure the LOD Group component:
   - LOD0: 70% (0-15 meters)
   - LOD1: 40% (15-40 meters)
   - LOD2: 15% (40-80 meters)
   - LOD3: 5% (80-120 meters)
   - Culled: <5% (>120 meters)
  
### Part 2: LOD Control Scripts
3. Write script `DynamicCityLOD.cs`:
   - When FPS is low (<30), automatically reduce LOD thresholds by 20%
   - When FPS is high (>55), increase thresholds by 15%
   - Log changes to console (Development Build only)
  
4. Write script `BuildingLODController.cs`:
   - Add method `SetForcedLOD(int level)` for forced switching
   - Save the last active LOD when exiting the game
   - Implement `OnLODChanged` event
  
### Part 3: Adaptive LOD
5. Implement "important" buildings system:
   - Buildings with tag `ImportantBuilding` should have higher LOD (shift up one level)
   - Example: LOD2 becomes LOD1, LOD1 becomes LOD0
  
6. Add camera "focus" effect:
   - If a building is in screen center for more than 2 seconds — temporarily increase LOD
  
### Part 4: Visual Feedback
7. Create debug mode (key `F1`):
   - Color buildings based on active LOD:
     - LOD0: Green
     - LOD1: Yellow
     - LOD2: Orange
     - LOD3: Red
     - Culled: Black
    
8. Display on-screen (GUI) information:
   - Total buildings
   - Number of buildings at each LOD level
   - Current FPS
   - LOD Bias
  
---

## 🧰 Implementation Requirements:
- Use at least 3 LOD levels + billboard
- Must use Cross Fade for smooth transitions
- Implement Dynamic LOD based on FPS
- Add Impostor (billboard) for the farthest level
- Use `RecalculateBounds()` for correct occlusion culling

---

## 🔍 Verification:
1. Verify that models switch when the camera moves
2. Verify no flickering during switching (Cross Fade works)
3. Artificially lower FPS (add load) — verify LOD thresholds change
4. Mark a building as ImportantBuilding — it should render in more detail
5. In debug mode, colors should match the active LOD

---

## 🏆 Bonus Task (Optional):
Implement Billboard Baking System:
- Script automatically renders the building from all sides
- Creates an atlas texture (8-16 angles)
- Saves as an asset for use in LOD3
- Use `Camera.RenderToCubemap` or Impostor Baker asset

---

### ⭐ If this project was useful, put a star on GitHub!
