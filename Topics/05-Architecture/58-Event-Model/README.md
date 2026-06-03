# 📡 Event Model in Unity: Global Event System

This material covers creating a global event system in Unity for loosely coupled (decoupled) code. 
You'll learn about three main approaches: Delegate, Action (built-in delegates), 
and ScriptableObject Channels. Each approach has its advantages and use cases.

---

## 1. 🧩 Why do we need a global event system?
In a typical game project, objects often need to exchange information:
- Player scores points → UI should update.
- Character takes damage → health system must react.
- Enemy dies → spawner should create a new one.

### Problem with direct references:
```csharp
public class Player : MonoBehaviour {
    public UIManager uiManager; // Hard coupling
    void OnScore() {
        uiManager.UpdateScore(); // Depends on specific UI
    }
}
```

### What the event model provides:
- 📉 Reduced coupling — sender doesn't know about receivers.
- 🔧 Easier maintenance — add new listeners without changing sender.
- 🧪 Testability — subsystems can be tested in isolation.

---

## 2. 📜 Delegate — the classic approach
### What it is:
A delegate is a type that holds a reference to a method with a specific signature. It's the base event mechanism in C#.

### How to use:
```csharp
// 1. Declare delegate
public delegate void ScoreChangedDelegate(int newScore);

// 2. Create event based on delegate
public class ScoreManager : MonoBehaviour {
    public event ScoreChangedDelegate OnScoreChanged;
    
    private int score;
    
    public void AddScore(int points) {
        score += points;
        OnScoreChanged?.Invoke(score);
    }
}

// 3. Subscribe in another class
public class UIManager : MonoBehaviour {
    public ScoreManager scoreManager;
    
    void Start() {
        scoreManager.OnScoreChanged += UpdateScoreUI;
    }
    
    void UpdateScoreUI(int newScore) {
        Debug.Log($"Score: {newScore}");
    }
    
    void OnDestroy() {
        scoreManager.OnScoreChanged -= UpdateScoreUI; // UNSUBSCRIBE IS IMPORTANT!
    }
}
```

### ✅ Pros:
- Built into C# (no dependencies).
- Supports multiple subscribers.
- Good performance.

### ❌ Cons:
- Manual subscription/unsubscription (memory leak risk).
- Direct references still required.
- Not convenient for global use (needs singleton).

---

## 3. 🚀 Action / Func / UnityEvent — built-in delegates
### What it is:
`Action<T>` is a ready-made delegate from Microsoft. `UnityEvent` is a serializable version visible in the Inspector.

### 3.1 Action (System.Action)
```csharp
public class ScoreManager : MonoBehaviour {
    public static Action<int> OnScoreChanged; // Global static event
    
    private int score;
    
    void AddScore(int points) {
        score += points;
        OnScoreChanged?.Invoke(score);
    }
}

// Any class can subscribe without direct reference
public class AchievementSystem : MonoBehaviour {
    void Start() {
        ScoreManager.OnScoreChanged += UnlockAchievements;
    }
    
    void UnlockAchievements(int score) {
        if(score >= 100) Debug.Log("Achievement: 100 points!");
    }
    
    void OnDestroy() {
        ScoreManager.OnScoreChanged -= UnlockAchievements;
    }
}
```

### 3.2 UnityEvent (serializable)
```csharp
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour {
    public UnityEvent<int, int> OnDamageTaken; // Visible in Inspector!
    
    public void TakeDamage(int damage) {
        int newHealth = currentHealth - damage;
        OnDamageTaken?.Invoke(damage, newHealth);
    }
}
```

### ✅ Pros:
- `Action` — minimal, no extra code.
- `UnityEvent` — visible in Inspector, configurable via UI.

### ❌ Cons:
- Static events can cause memory leaks (easy to forget to unsubscribe).
- Harder to debug (who subscribed? who raised?).

---

## 4. 🎯 ScriptableObject Channels — recommended for medium/large projects
### What it is:
A ScriptableObject used as a channel to pass events between objects without direct references. This is an Event Bus pattern based on ScriptableObject.

### How to build your own system:
Step 1: Base event channel
```csharp
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/IntEventChannel")]
public class IntEventChannelSO : ScriptableObject {
    public UnityAction<int> OnEventRaised;
    
    public void RaiseEvent(int value) {
        OnEventRaised?.Invoke(value);
    }
}
```

Step 2: Create channels in the Editor
- Right-click in `Assets` → `Create` → `Events` → `IntEventChannelSO`
- Name it: `ScoreChangedChannel`, `PlayerHealthChannel`, etc.

Step 3: Publisher (Sender)
```csharp
public class ScoreManager : MonoBehaviour {
    [SerializeField] private IntEventChannelSO scoreChannel;
    
    private int score;
    
    public void AddScore(int points) {
        score += points;
        scoreChannel.RaiseEvent(score);
    }
}
```

Step 4: Subscriber (Receiver)
```csharp
public class UIManager : MonoBehaviour {
    [SerializeField] private IntEventChannelSO scoreChannel;
    
    void Start() {
        scoreChannel.OnEventRaised += UpdateScoreUI;
    }
    
    void UpdateScoreUI(int newScore) {
        scoreText.text = $"Score: {newScore}";
    }
    
    void OnDestroy() {
        scoreChannel.OnEventRaised -= UpdateScoreUI;
    }
}
```

Step 5: Configure in Inspector
- Drag `ScoreChangedChannel` into the `scoreChannel` field of both `ScoreManager` and `UIManager`.

### 🎁 Advanced: Generic channel
```csharp
public class EventChannelSO<T> : ScriptableObject {
    public UnityAction<T> OnEventRaised;
    public void RaiseEvent(T value) => OnEventRaised?.Invoke(value);
}

// Create concrete types:
// IntEventChannel : EventChannelSO<int>
// StringEventChannel : EventChannelSO<string>
// GameObjectEventChannel : EventChannelSO<GameObject>
```

### ✅ Pros of ScriptableObject Channels:
- 🧵 No global static variables — each channel lives as an asset.
- 🎨 Visual Inspector setup — change connections without code.
- 🔍 Easier to debug — see who's subscribed to a channel.
- 🧩 Loose coupling — sender knows nothing about receiver.
- ♻️ Reusable — one channel can be used across multiple scenes.

### ❌ Cons:
- Need to create many assets (one per event type).
- Slightly more complex for beginners.

---

## 5. 📊 Comparison of approaches
| Feature | Delegate | Action | UnityEvent | ScriptableObject Channel |
| --- | --- | --- | --- | --- |
| Complexity | Medium | Low | Low | Medium |
| Visible in Inspector | ❌ | ❌ | ✅ | ✅ |
| Global reach | Via static | Via static | ❌ | ✅ (as asset) |
| Memory leak risk | High | High | Medium | Low |
| Reusable across scenes | ❌ | ❌ | ❌ | ✅ |
| Recommended for | Small projects | Small/Medium | UI/Inspector | Medium/Large projects |

---

## 6. 🎮 Recommended architecture
For a medium-sized game project, use a combination:

### 1. ScriptableObject Channels — for global game events:
- `PlayerDeathChannel`, `ScoreChangedChannel`, `LevelCompletedChannel`

### 2. UnityEvent — for local events inside prefabs:
- UI button → animation

### 3. Action — for simple temporary connections inside a single script

### Example structure:
```text
Assets/
  Scripts/
    Events/
      Channels/
        IntEventChannelSO.cs
        StringEventChannelSO.cs
      Listeners/
        IntEventListener.cs (optional)
    Game/
      ScoreManager.cs (publisher)
      UIManager.cs (subscriber)
```

---

### ⭐ If this project was useful, put a star on GitHub!
