# 🎮 Паттерн Команда (Command) в Unity: Undo/Redo, сохранение ввода и реплеи

Этот материал покрывает применение паттерна Команда в Unity: 
реализацию систем отмены/повтора (Undo/Redo), сохранение пользовательского ввода, запись и воспроизведение реплеев. 
Паттерн Команда превращает запросы (действия) в объекты, позволяя передавать их, ставить в очередь, логировать и откатывать.

---

## 📖 1. Что такое паттерн Команда?
### 🎯 Для чего нужно:
Паттерн Команда инкапсулирует действие в отдельный объект. 
Этот объект содержит всю необходимую информацию для выполнения 
действия и (опционально) для его отмены. Это позволяет:

- Реализовать Undo/Redo (отмену и повтор действий)
- Записывать действия игрока для реплеев
- Сохранять ввод в файл и воспроизводить его позже
- Создавать очереди действий (макросы, турнирные режимы)

### ⚙️ Базовая структура:
```csharp
// Интерфейс команды
public interface ICommand
{
    void Execute();   // Выполнить действие
    void Undo();      // Отменить действие
}

// Конкретная команда: движение игрока
public class MoveCommand : ICommand
{
    private Player player;
    private Vector3 direction;
    private float distance;
    
    private Vector3 previousPosition; // Для отмены
    
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

## ↩️ 2. Система Undo/Redo (Отмена/Повтор)
### 🎯 Для чего нужно:
Позволяет игроку отменять последние действия (например, в редакторе уровней, стратегии или головоломке) и возвращать их обратно.

### ⚙️ Как реализовать:
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
        redoStack.Clear(); // Новое действие очищает Redo-стек
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

### 📌 Пример из реальной жизни:
Редактор уровня в стиле "Строитель замков". Игрок размещает стены, башни, двери. 
Каждое действие — отдельная команда. Нажатие `Ctrl+Z` (Undo) убирает последний объект, `Ctrl+Y` (Redo) возвращает его.

---

## 💾 3. Сохранение ввода (Input Recording)
### 🎯 Для чего нужно:
Запись действий игрока в список команд с сохранением времени. 
Это позволяет позже воспроизвести точную последовательность действий для тестирования,
обучения или создания демо-записей.

### ⚙️ Как реализовать:
```csharp
// Команда с временной меткой
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

// Система записи
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
        // Сериализация recordedCommands в JSON/бинарный файл
        string json = JsonUtility.ToJson(new CommandWrapper(recordedCommands));
        System.IO.File.WriteAllText(Application.dataPath + "/" + filename, json);
    }
}
```

### 📌 Пример из реальной жизни:
В гоночной игре вы записываете свой лучший заезд. Все нажатия газа, тормоза, повороты руля сохраняются во времени. 
Другой игрок может загрузить этот файл и наблюдать "призрака" — точную копию вашего заезда.

---

## 🎥 4. Реплеи (Replays)
### 🎯 Для чего нужно:
Реплей позволяет проиграть записанную последовательность команд точно так, как она выполнялась. 
Это может быть показ лучшего времени, обучение, анализ ошибок или просто красивая демонстрация.

### ⚙️ Как реализовать:
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
        
        // Сброс всех объектов в начальное состояние
        ResetGameState();
        
        currentCommandIndex = 0;
        replayStartTime = Time.time;
        isPlaying = true;
    }
    
    void Update()
    {
        if (!isPlaying) return;
        
        float currentReplayTime = Time.time - replayStartTime;
        
        // Воспроизводим все команды, чья временная метка <= текущему времени
        while (currentCommandIndex < replayData.Count && 
               replayData[currentCommandIndex].timestamp <= currentReplayTime)
        {
            replayData[currentCommandIndex].command.Execute();
            currentCommandIndex++;
        }
        
        if (currentCommandIndex >= replayData.Count)
        {
            isPlaying = false;
            Debug.Log("Реплей завершён");
        }
    }
    
    void ResetGameState()
    {
        // Возвращаем всех игроков, врагов, позиции и т.д. в исходное состояние
        // Для этого удобно сохранить начальный снапшот или использовать Undo
    }
}

// Обёртка для сериализации
[System.Serializable]
public class CommandWrapper
{
    public List<TimedCommand> commands;
    public CommandWrapper(List<TimedCommand> commands) => this.commands = commands;
}
```

### 📌 Пример из реальной жизни:
В шахматах после партии можно просмотреть реплей всех ходов. 
Паттерн Команда хранит каждый ход (откуда, куда, какая фигура). 
Реплей просто вызывает `Execute()` для каждой команды с правильной задержкой.

---

## 🧠 Полный пример: Движение игрока с Undo/Redo и записью
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
        // Получаем ввод
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
            
        // Запись реплея
        if (Input.GetKeyDown(KeyCode.F1)) inputRecorder.StartRecording();
        if (Input.GetKeyDown(KeyCode.F2)) inputRecorder.SaveToFile("replay.json");
        if (Input.GetKeyDown(KeyCode.F3)) replaySystem.LoadReplay("replay.json");
        if (Input.GetKeyDown(KeyCode.F4)) replaySystem.StartReplay();
    }
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
