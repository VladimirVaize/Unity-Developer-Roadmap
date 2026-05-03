# 🎯 UnityEvents: Configuring Events in the Inspector for Loose Coupling

## 📘 What is UnityEvent?

UnityEvent is a special variable type in Unity that allows you to bind method calls from different components directly in the Inspector. 
This is a powerful tool for creating loose coupling between objects and scripts.

Loose coupling means that one script does not directly depend on another via references 
(`GetComponent`, public fields of concrete types). Instead, it simply "declares an event," and who responds to it is configured visually in the editor.

---

## 🧠 Why use it?
- ✅ Separation of concerns: The script that generates an event (e.g., a button pressed, player touched a trigger) does not need to know what actions will follow.
- ✅ Flexibility: A designer or programmer can change reactions to an event without modifying code — just by dragging objects in the Inspector.
- ✅ Reusability: The same script can be used in different scenes and projects with different listeners.
- ✅ Visual setup and debugging: Everything is configured via the editor UI, which is convenient for non-programmers.

---

## 🔧 How to use UnityEvent (step by step)
### 1. Declare the event in your script
```csharp
using UnityEngine;
using UnityEngine.Events;

public class ButtonPresser : MonoBehaviour
{
    // Declare a public event — it will appear in the Inspector
    public UnityEvent OnButtonPressed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Invoke the event. All bound methods will execute automatically
            OnButtonPressed.Invoke();
        }
    }
}
```

### 2. Configure the event in Unity's Inspector
- Attach the script to any GameObject (e.g., an empty `EventSystem` or `Manager`).
- In the Inspector you will see an On `Button Pressed` field (or your event's name).
- Click "+" at the bottom of the event list.
- Drag any GameObject from your scene into the empty field (target object).
- From the dropdown, select a component, then a specific method (e.g., `Light.enabled = true` or `Animator.Play("Jump")`).

### 3. Supported method types
#### UnityEvent can call:
- Parameterless methods (`void MyMethod()`)
- Methods with one parameter (`void MyMethod(bool arg)`, `void MyMethod(int arg)`, `void MyMethod(string arg)`, `void MyMethod(float arg)`, etc.)
- Static methods
- Methods inherited from `MonoBehaviour`

---

## 💡 Usage Examples
### Example 1: Door opening on trigger
#### Script `DoorTrigger.cs`:
```csharp
public class DoorTrigger : MonoBehaviour
{
    public UnityEvent OnPlayerEnter;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnPlayerEnter.Invoke();
    }
}
```

#### Inspector setup:
- Bind `OnPlayerEnter` to the door animation → `Animator.Play("Open")`
- Also add a second method → `AudioSource.Play()` for opening sound

### Example 2: UI button controlling multiple actions
#### Script `UIManager.cs`:
```csharp
public class UIManager : MonoBehaviour
{
    public UnityEvent OnRestartButtonClicked;
    
    public void RestartGame() // public method called from UI Button
    {
        OnRestartButtonClicked.Invoke();
    }
}
```

#### Setup:
- Bind `OnRestartButtonClicked` to reloading the scene (`SceneManager.LoadScene`)
- And to resetting the score (`ScoreCounter.ResetScore()`)
- And to playing a sound (`Audio.PlayOneShot`)

Not a single line of code inside `RestartGame` except invoking the event!

---

## 🆚 UnityEvent vs. Direct References

| Direct References (tight coupling) | UnityEvent (loose coupling) |
|---------------------------------|-------------------------------------|
| `public Light myLight;`<br>`myLight.enabled = true;` | `public UnityEvent OnReachedGoal;`<br>`... OnReachedGoal.Invoke();` |
| Script knows exact type (`Light`) | Script knows nothing about listeners |
| To change behavior — modify code | To change behavior — drag objects in Inspector |
| Cannot call multiple methods without code | Can bind 10 methods without code |
| Hard to reuse | Easy to reuse |

---

## ⚠️ Important Notes
- UnityEvents don't persist broken bindings — if you delete a listener object, the Inspector reference becomes empty (no runtime error, just nothing happens).
- Performance — for events that happen very often (every frame), use standard C# events (`public event Action ...`).
  UnityEvent is slightly slower, but for UI and gameplay events (door opening, player death) it's unnoticeable.
- Serialization — UnityEvent fully supports saving in prefabs and scenes.

---

## 🎓 Conclusion
UnityEvent is the ideal tool when:
- You want a level designer to be able to configure behavior without access to code.
- You have one event source (e.g., a button) but many possible reactions.
- You are building a system that is easy to extend without touching existing classes.

Use UnityEvents for loose coupling, and your project will become flexible, clear, and team-friendly!

---

### ⭐ If this project was useful, put a star on GitHub!
