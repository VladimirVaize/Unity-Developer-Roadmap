# 🎮 Command Pattern in Unity: Undo/Redo, Input Recording, Replays

This material covers applying the Command Pattern in Unity: implementing Undo/Redo systems, saving user input, recording and playing back replays. 
The Command Pattern turns requests (actions) into objects, allowing them to be passed around, queued, logged, and rolled back.

---

## 📖 1. What is the Command Pattern?
### 🎯 Purpose:
The Command Pattern encapsulates an action into a separate object. 
This object contains all the information needed to perform the action and (optionally) to undo it. This enables:
- Undo/Redo functionality
- Recording player actions for replays
- Saving input to a file and playing it back later
- Creating action queues (macros, tournament modes)

### ⚙️ Basic structure:
```csharp
// Command interface
public interface ICommand
{
    void Execute();   // Perform the action
    void Undo();      // Undo the action
}

// Concrete command: player movement
public class MoveCommand : ICommand
{
    private Player player;
    private Vector3 direction;
    private float distance;
    
    private Vector3 previousPosition; // For undo
    
    public MoveCommand(Player player, Vector3 direction, float distance)
    {
        this.player = player;
        this.direction = direction;
        this.distance = distance;
    }
    
    public void Execute()
    {
        previousPosition = player.transform.position;
        player.Move(direction * distance);
    }
    
    public void Undo()
    {
        player.transform.position = previousPosition;
    }
}
```

---

## ↩️ 2. Undo/Redo System
### 🎯 Purpose:
Allows the player to undo recent actions (e.g., in a level editor, strategy game, or puzzle) and redo them.

### ⚙️ How to implement:
```csharp
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();
    
    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear(); // New action clears redo stack
    }
    
    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            ICommand command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
        }
    }
    
    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            ICommand command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }
    }
}
```

### 📌 Real-life example:
A castle-building level editor. The player places walls, towers, and doors. 
Each action is a separate command. Pressing `Ctrl+Z` (Undo) removes the last object, `Ctrl+Y` (Redo) brings it back.

---

## 💾 3. Input Recording
### 🎯 Purpose:
Recording player actions into a list of commands with timestamps. 
This allows replaying the exact sequence of actions for testing, tutorials, or demo recordings.

### ⚙️ How to implement:
```csharp
// Command with timestamp
public class TimedCommand : ICommand
{
    public float timestamp;
    public ICommand command;
    
    public TimedCommand(float timestamp, ICommand command)
    {
        this.timestamp = timestamp;
        this.command = command;
    }
    
    public void Execute() => command.Execute();
    public void Undo() => command.Undo();
}

// Recording system
public class InputRecorder : MonoBehaviour
{
    private List<TimedCommand> recordedCommands = new List<TimedCommand>();
    private float startTime;
    private bool isRecording = false;
    
    public void StartRecording()
    {
        recordedCommands.Clear();
        startTime = Time.time;
        isRecording = true;
    }
    
    public void RecordCommand(ICommand command)
    {
        if (!isRecording) return;
        
        float currentTime = Time.time - startTime;
        recordedCommands.Add(new TimedCommand(currentTime, command));
    }
    
    public void SaveToFile(string filename)
    {
        // Serialize recordedCommands to JSON/binary file
        string json = JsonUtility.ToJson(new CommandWrapper(recordedCommands));
        System.IO.File.WriteAllText(Application.dataPath + "/" + filename, json);
    }
}
```

### 📌 Real-life example:
In a racing game, you record your best lap. 
All gas, brake, and steering inputs are saved with timing. 
Another player can load this file and watch a "ghost" — an exact copy of your lap.

---

## 🎥 4. Replays
### 🎯 Purpose:
A replay plays back a recorded sequence of commands exactly as they were executed. 
This can be for showing best times, tutorials, error analysis, or just cool demonstrations.

### ⚙️ How to implement:
```csharp
public class ReplaySystem : MonoBehaviour
{
    private List<TimedCommand> replayData;
    private int currentCommandIndex = 0;
    private float replayStartTime;
    private bool isPlaying = false;
    
    public void LoadReplay(string filename)
    {
        string json = System.IO.File.ReadAllText(Application.dataPath + "/" + filename);
        replayData = JsonUtility.FromJson<CommandWrapper>(json).commands;
    }
    
    public void StartReplay()
    {
        if (replayData == null || replayData.Count == 0) return;
        
        // Reset all objects to initial state
        ResetGameState();
        
        currentCommandIndex = 0;
        replayStartTime = Time.time;
        isPlaying = true;
    }
    
    void Update()
    {
        if (!isPlaying) return;
        
        float currentReplayTime = Time.time - replayStartTime;
        
        // Play all commands whose timestamp <= current time
        while (currentCommandIndex < replayData.Count && 
               replayData[currentCommandIndex].timestamp <= currentReplayTime)
        {
            replayData[currentCommandIndex].command.Execute();
            currentCommandIndex++;
        }
        
        if (currentCommandIndex >= replayData.Count)
        {
            isPlaying = false;
            Debug.Log("Replay finished");
        }
    }
    
    void ResetGameState()
    {
        // Reset all players, enemies, positions, etc. to initial state
        // A snapshot or Undo system works well here
    }
}

// Wrapper for serialization
[System.Serializable]
public class CommandWrapper
{
    public List<TimedCommand> commands;
    public CommandWrapper(List<TimedCommand> commands) => this.commands = commands;
}
```

### 📌 Real-life example:
In chess, after a match you can watch a replay of all moves. 
The Command Pattern stores each move (from, to, which piece). 
The replay simply calls `Execute()` for each command with proper timing.

---

## 🧠 Complete example: Player movement with Undo/Redo and recording
```csharp
public class Player : MonoBehaviour
{
    public float speed = 5f;
    private CommandManager commandManager;
    private InputRecorder inputRecorder;
    
    void Start()
    {
        commandManager = FindObjectOfType<CommandManager>();
        inputRecorder = FindObjectOfType<InputRecorder>();
    }
    
    void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 moveDelta = new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;
        
        if (moveDelta != Vector3.zero)
        {
            ICommand moveCommand = new MoveCommand(this, moveDelta);
            commandManager.ExecuteCommand(moveCommand);
            inputRecorder.RecordCommand(moveCommand);
        }
        
        // Undo/Redo
        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
            commandManager.Undo();
        if (Input.GetKeyDown(KeyCode.Y) && Input.GetKey(KeyCode.LeftControl))
            commandManager.Redo();
            
        // Replay recording
        if (Input.GetKeyDown(KeyCode.F1)) inputRecorder.StartRecording();
        if (Input.GetKeyDown(KeyCode.F2)) inputRecorder.SaveToFile("replay.json");
        if (Input.GetKeyDown(KeyCode.F3)) replaySystem.LoadReplay("replay.json");
        if (Input.GetKeyDown(KeyCode.F4)) replaySystem.StartReplay();
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
