# 🛠️ IDE Setup and Debugging in Unity

This material covers how to set up your Integrated Development Environment (IDE) 
for Unity and how to use built-in debugging tools: breakpoints, Immediate Window, and Attach to Unity. 
Proper debugging allows you to find and fix logic errors in your game without disrupting the workflow.

---

## 1. Choosing and Configuring an IDE 🖥️
Unity officially supports Visual Studio (Community/Professional) and Rider by JetBrains. Both IDEs have free versions for non-commercial use.

### Visual Studio
- Installation: Unity Hub will offer to install Visual Studio with the "Game development with Unity" workload.
  You can also install it manually via Visual Studio Installer by selecting the Game development with Unity workload.

- Configuration: In Unity: `Edit → Preferences → External Tools → External Script Editor` → select `Visual Studio`.
  Make sure the `Visual Studio Editor` package is installed in the Package Manager.

### Rider
- Installation: Download from the official JetBrains website.
  Rider offers a free license for students and open-source projects, as well as a 30-day trial.

- Configuration: In Unity: `Edit → Preferences → External Tools → External Script Editor` → select `Rider`.
  Rider will automatically import the Unity project and recognize all scripts.

### 🔧 Connection check
Open any C# script from Unity (double-click). The chosen IDE should launch. If it doesn't, check the path in External Tools settings.

---

## 2. Breakpoints 🛑
Purpose: A breakpoint is a marker in your code at which program execution stops. 
You can then inspect variable values, object states, and step through code line by line.

### How to use:
- Setting: In the IDE, click in the left margin next to a line number (a red circle appears).
- Removing: Click the red circle again.
- Conditional breakpoints: Right-click the red circle → `Conditions` → enter a condition (e.g., `i == 5`).
- Execution stops only when the condition is true.

### Example:
```csharp
void Update()
{
    float speed = 10f;
    transform.Translate(Vector3.forward * speed * Time.deltaTime);
}
```

Place a breakpoint on the `transform.Translate...` line. Start the game (Play in Unity). 
When the object begins moving, execution will pause. Now hover over the `speed` variable to see its value.

---

## 3. Immediate Window ⌨️
Purpose: The Immediate Window allows you to execute C# code while paused at a breakpoint. 
This is a powerful tool for testing hypotheses, modifying variables, or calling methods on the fly.

### How to use:
- Visual Studio: `Debug → Windows → Immediate` (or `Ctrl + Alt + I`)
- Rider: `View → Tool Windows → Immediate Window` (or search with `Ctrl + Shift + A`)

### Example (continued):
The game is paused at a breakpoint inside `Update()`. In the Immediate Window, type:
```csharp
speed = 50;
```

Press Enter. The `speed` variable changes immediately during execution. 
Press `Continue` (F5) — the object now moves faster, without recompiling.

---

## 4. Attach to Unity 🔗
Purpose: Allows you to debug code without launching the IDE from Unity. 
You've already started the game in the editor but forgot to set breakpoints? Simply "attach" to the Unity process.

### How to use:
- Visual Studio: `Debug → Attach Unity Debugger` → select your Unity editor (usually `Unity Editor (your_version)`).
- Rider: `Attach to Unity Process` button (green play triangle with a plus) or `Run → Attach to Unity Process`.
- Automatic attachment: In IDE settings, you can enable "Always attach" for the current project.

> [!Important]
> Unity must be running at the moment of attachment (playing the game is not required). 
> After attaching, you can set breakpoints and they will trigger on the next pass of the code.

### Example:
You started the game, and the character teleports strangely. Without closing the game, in Rider you click `Attach to Unity Process`. 
You open the movement script, set a breakpoint in `Update()` — on the next frame, execution will stop, and you'll see why coordinates are changing incorrectly.

---

## 🔄 Typical debugging workflow
1. Write code in the IDE.
2. Run the game in Unity.
3. If an error or unexpected behavior occurs — set a breakpoint.
4. Stop the game, restart with the debugger attached (or use `Attach to Unity`).
5. When the breakpoint triggers — inspect variables using the Immediate Window.
6. Fix the code, recompile (Unity recompiles automatically), and continue.

---

## 📌 Useful hotkeys

| Action                        | Visual Studio  | Rider                              |
| ---                           | ---            | ---                                |
| Set/remove breakpoint         | F9             | Ctrl + F8                          |
| Start debugging (with launch) | F5             | Alt + Shift + F5                   |
| Continue to next breakpoint   | F5             | F5                                 |
| Step Into                     | F11            | F7                                 |
| Step Over                     | F10            | F8                                 |
| Open Immediate Window         | Ctrl + Alt + I | Ctrl + Alt + Shift + I (or search) |

---

### ⭐ If this project was useful, put a star on GitHub!
