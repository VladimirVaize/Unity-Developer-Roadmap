# 📝 Task: Enemy Waves Configuration Window

## Goal:
Create an editor window that allows a game designer to quickly create, configure, and test enemy waves without writing code.

## Requirements:
1. Create a class `EnemyWaveWindow` inherited from `EditorWindow`.
   - Add a menu item `Tools/Enemy Waves Editor`.
  
2. Use EditorGUILayout to create the following fields:
   - `Enemy Prefab (ObjectField)` — which enemy will be spawned.
   - `Enemy Count (IntSlider)` — from 1 to 20.
   - `Spawn Delay (Slider)` — from 0.2 to 5 seconds.
   - `Spawn Radius (Slider)` — from 1 to 20 meters.
   - `Visualization Color (ColorField)` — to display the radius in Scene View.
  
3. Add two buttons:
   - `▶ Spawn Wave` — spawns the specified number of enemies at random positions within the spawn radius.
   - `❌ Delete All Enemies` — deletes all enemies in the scene (use the `Enemy` tag or `FindObjectsOfType`).
  
4. Add Scene View visualization:
   - Use `OnSceneGUI()` to draw a circle (WireDisc) with the spawn radius around the scene center or around a selected object (optional).
  
5. Save settings between sessions using `EditorPrefs` (enemy count, radius, delay, color).
6. Add validation: if the prefab is not assigned — show a warning (`HelpBox`) and disable the spawn button.

### Bonus (optional):
- Add a wave type dropdown (Enum): `Normal`, `Fast`, `Tank`.
- Add a `Wave Name` field (TextField) and save different parameter sets in a `ScriptableObject`.

---

### ⭐ If this project was useful, put a star on GitHub!
