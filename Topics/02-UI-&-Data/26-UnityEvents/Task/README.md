# 🛠️ Practice Task: UnityEvents for Loose Coupling

## 🎯 Task: Trap and Reward Activation System
You are developing a 2D platformer. Create a scene containing:
- Player (a cube or sprite with `Rigidbody2D` and `BoxCollider2D`)
- Finish trigger (`BoxCollider2D` with `IsTrigger` flag)
- Trap (e.g., a falling spike or enemy)
- Secret door (an object that should disappear or open)
- Audio manager (an object with an `AudioSource` component)

---

## 📝 Implementation Requirements
### Write a script `PlayerTriggerHandler.cs` with a single UnityEvent:
```csharp
public UnityEvent OnPlayerReachedFinish;
```

This event should be invoked when the player enters the finish trigger.

Do not add any direct references to other objects (Light, Door, Audio, etc.) in this script.

---

## ⚙️ What to configure in the Inspector (without changing code):
### Using only the Inspector window, bind the following actions to the `OnPlayerReachedFinish` event:
1. Play victory sound (call `AudioSource.PlayOneShot()` or `Play()` on the audio manager)
2. Activate the trap — enable a component that makes the trap dangerous (e.g., `Behaviour.enabled` or `GameObject.SetActive(true)`)
3. Deactivate the secret door — disable its `GameObject` (or play an opening animation)
4. Display text "Level complete!" (use `TextMeshPro` or `UI.Text`)

---

## 🔍 Bonus task (with a star)
Create a second script `ScoreCounter.cs` with a public method `AddScore(int value)`. Bind it to the same event, but pass a parameter of `5` points.

Hint: UnityEvent allows you to select a method with an int parameter — a value input field will appear in the Inspector.

---

## ✅ Success criteria:
- The `PlayerTriggerHandler` script has no variables of type `Light`, `Door`, `AudioSource`, `Text`, etc.
- All reactions to the event are configured only through the Inspector.
- When the player enters the finish trigger, simultaneously:
  - A sound plays
  - The trap activates
  - The door disappears / opens
  - Text appears (and points are added if the bonus task is completed)
 
---

## 💡 Hint
Don't forget to click "+" in the event's Inspector field for each action. You can bind multiple methods to a single event — they will execute from top to bottom.

---

### ⭐ If this project was useful, put a star on GitHub!
