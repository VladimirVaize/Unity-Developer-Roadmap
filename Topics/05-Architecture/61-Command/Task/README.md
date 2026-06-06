# 🎯 Practical Task: Command System for a Drawing App with Undo/Redo and Replay
## 📋 Task Description
You need to create a simple drawing application in Unity where the player can click on a grid to place/remove pixels. Use the Command Pattern to implement:
- Undo and Redo functionality
- Recording action sequences to a file
- Playing back replays from a file

---

## 🧱 Task Structure
### 📁 Part 1: Base Command
Create an interface `ICommand` with `Execute()` and `Undo()` methods.

Create a concrete command `PlacePixelCommand` that stores:
- `GridManager grid` — reference to the grid manager
- `int x, int y` — pixel coordinates
- `Color color` — pixel color
- `bool wasOccupied` — whether the pixel was occupied before execution (for Undo)

### 📁 Part 2: GridManager
Create a `GridManager : MonoBehaviour` that:
- Has a 2D array `Color?[,] grid` (10×10 or 16×16)
- Method `SetPixel(int x, int y, Color color)` — sets the color
- Method `ClearPixel(int x, int y)` — removes the pixel
- Method `bool IsOccupied(int x, int y)` — checks if a cell is occupied
- Visualizes the grid using `OnDrawGizmos` or creates sprites/cubes

### 📁 Part 3: CommandManager
Create a `CommandManager` that:
- Has two stacks: `undoStack` and `redoStack`
- Method `ExecuteCommand(ICommand cmd)` — executes command and pushes to `undoStack`
- Methods `Undo()` and `Redo()`
- Clears `redoStack` on new `ExecuteCommand()`

### 📁 Part 4: Drawing via UI
Create a script `DrawingController` that:
- Responds to clicks on grid cells (can use `OnMouseDown` on each tile or Raycast)
- When clicking an empty cell, creates a `PlacePixelCommand` and executes it
- When clicking an occupied cell — removes the pixel (another command or the same `PlacePixelCommand` with different behavior)

### 📁 Part 5: Saving and Replay (⭐ main challenge)
Add to `DrawingController` or a separate `ReplayManager`:
- `StartRecording()` — clears recorded commands list and starts recording
- `StopRecording()` — stops recording, saves commands to a file (JSON)
- `LoadReplay(string filename)` — loads commands from file
- `StartReplay()` — resets grid to initial state, then sequentially executes all commands from file with delays (e.g., using `Invoke` or `Coroutine`)

### 📁 Part 6: Hotkeys
Add hotkeys:
- `Ctrl+Z` — Undo
- `Ctrl+Y` — Redo
- `F1` — start recording
- `F2` — stop recording and save
- `F3` — load and start replay

---

## ✅ Completion Criteria
1. Command Pattern is correctly implemented: each drawing/erasing operation is a separate command object
2. Undo/Redo system works sequentially (undo recent actions and redo them)
3. Replay plays back all actions in correct order, considering time between actions (or just sequentially with a fixed delay, e.g., 0.5 seconds)
4. Save and load from file implemented via JSON serialization

---

## 🧩 Bonus Task (⭐⭐)
1. Add clear entire grid as a single command that can also be undone
2. Add a color palette (3-5 colors) — the command must store the selected color
3. Implement multiplayer replay: two players take turns drawing, replay shows both action layers

---

## 🧪 Expected Result
After completing the task:
- Clicking on the grid creates colored squares/dots
- `Ctrl+Z` undoes the last placed or removed pixel
- `Ctrl+Y` redoes the undone action
- Pressing `F1`, then performing several actions, then `F2` — creates a `replay.json` file
- Pressing `F3` — the grid clears and actions play back automatically (like a video)
- Console logs show command execution, Undo, Redo, replay start/end

---

### ⭐ If this project was useful, put a star on GitHub!
