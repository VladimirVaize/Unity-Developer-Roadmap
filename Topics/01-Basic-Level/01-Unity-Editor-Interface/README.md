# Unity Editor Interface: Core Windows

This material covers the four key windows (panels) of the Unity editor that form the foundation of working on any project: **Hierarchy**, **Project**, **Inspector**, and **Scene / Game view**. Understanding their functions and interactions is the first step to efficient development in Unity.

---

## 1. Hierarchy

### Purpose:

The Hierarchy window displays all GameObjects present in the currently active scene. It serves as a "table of contents" for your scene, showing the structure of all objects — from cameras and lights to characters, walls, and empty parent objects.

### How to use:

- **Selecting an object**: Click any object in the Hierarchy — it becomes selected in the Scene view, and its components appear in the Inspector.
- **Organizing**: Drag one object onto another to create parent-child relationships (Parenting). When moving the parent, all child objects move with it.
- **Creating and deleting**: Right-click in the window → Create Empty (or any other object). Press Delete to remove the selected object.
- **Searching**: Use the search field at the top to find objects by name or component type.

### Example:

You are building a room. In the Hierarchy, you have: `Camera`, `Directional Light`, `Floor`, `Walls` (a parent object for four child walls), `Player`. If you move `Walls` in the scene, all four walls move together while maintaining their relative positions.

---

## 2. Project Window

### Purpose:

This window represents all the files on your project's disk (assets: textures, models, audio, scenes, scripts, prefabs, etc.) inside the Unity editor. It is independent of the current scene and serves as the "library" of the entire project.

### How to use:

- **Navigation**: Double-click a folder to enter it. Use the `One Column / Two Columns` button to change the view.
- **Importing**: Drag files (PNG, FBX, WAV, CS) directly from your file explorer into this window. Unity will automatically import them as assets.
- **Creating**: Right-click in an empty area → `Create` → `C# Script`, `Material`, `Folder`, etc.
- **Search and filters**: Use the search bar and file type buttons (textures, models, prefabs).
- **Prefabs**: Drag a ready-made GameObject from the scene into the Project window — this creates a prefab (template). Later, you can drag that prefab from the Project window into any scene.

### Example:

You have a `Sword.fbx` model. You drag it into the `Assets/Models` folder. Now you can drag it from the Project window into the Scene view — a sword appears in the game. If you create an `Enemy` prefab (with movement, health, etc.), you can place 10 enemies by simply dragging that prefab from the Project into the Hierarchy.

---

## 3. Inspector

### Purpose:

The Inspector window displays all properties and components of the selected GameObject. This is where you configure how the object looks, moves, interacts, and responds to logic.

### How to use:

- **Viewing properties**: Select an object in the Hierarchy or Scene view. The Inspector shows: the object's name, an active checkbox (enabled/disabled), and all components (Transform, Mesh Renderer, Collider, scripts, etc.).
- **Editing**: Change numbers, colors, references to other objects. For example, a `Speed` field in a movement script — change 5 to 10, and the object will run faster directly in the editor.
- **Adding components**: Click the `Add Component` button at the bottom of the Inspector. Choose, for example, `Rigidbody` for physics or `Audio Source` for sound.
- **Removing / adjusting**: Click the three dots (…) or the gear icon on a component → `Remove Component`. You can also reset, copy, and paste component values.

### Example:

You have a `Player` object with a `Rigidbody` component and your `PlayerMovement` script. In the Inspector, you see a `jumpForce = 5` field. During playtesting, the jump feels too weak. You pause the game, change `jumpForce` to 12 directly in the Inspector — you see the result immediately without recompiling.

---

## 4. Scene View and Game View

### Purpose:

- **Scene View**: The editor's "workshop." You can freely move the camera (QWE + right mouse button), place objects, rotate, scale, paint terrain. This is NOT what the player will see.
- **Game View**: A simulation of what the player's camera sees during gameplay. You see the final frame as it would appear in a finished application.

### How to use (Scene View):

- **Navigation**: Hold the right mouse button and move with WASD — fly through the scene. Click an object and press `F` to focus the camera on it.
- **Tools (top-left panel)**: `Q` (Pan), `W` (Move), `E` (Rotate), `R` (Scale), `T` (Rect Transform for UI).
- **Display modes**: The `Shaded` button (or `Wireframe`, `Shaded Wireframe`) — for example, to view polygons.
- **Gizmos**: Enable/disable icons for lights, colliders, audio sources (the Gizmos button in the upper-right corner of the Scene view).

### How to use (Game View):

- **Running the game**: Press the `Play` button (triangle) in the center of the editor's top toolbar. The Game View comes to life, showing the exact picture as in a build.
- **Screen resolution**: Use the dropdown in Game View (e.g., `Standalone (1080p)`, `iPhone X`) to test UI responsiveness.
- **Pause and frame-by-frame debugging**: Use the `Pause` and `Step` buttons next to Play. While paused, you can switch to Scene View, orbit the camera, and inspect objects in the Inspector — all during the paused state.

### Example:

You are building a level. In Scene View, you move platforms (tool `W`), rotate obstacles (`E`), and place enemies from prefabs. Then you press **Play — in Game View**, you see the character jumping across platforms. If something is off (an enemy is too far away), you stop the game (Play → Stop), move the enemy closer in Scene View, and run it again.

---

## Interaction Between Windows (The Workflow)

### No window works in isolation. A typical workflow cycle:

1. In **Project**, you select an Enemy `prefab`.
2. Drag it into **Hierarchy** (it appears in the scene).
3. Select the enemy — in **Inspector**, you see its health, speed, model.
4. In **Scene View**, move the enemy to the desired position.
5. Press **Play** and watch **Game View** to see how the enemy attacks.
6. While paused, change the attack parameters in Inspector and immediately check the result.

---

### ⭐ If this project was useful, put a star on GitHub!
