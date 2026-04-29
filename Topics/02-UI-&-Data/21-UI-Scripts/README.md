# 🎮 UI Scripts in Unity: Button, Slider, Toggle, ScrollRect

> [!Note]
> This material focuses on working with basic UI elements through scripts in Unity.
> You will learn how to handle button clicks, read slider values, track toggle states, and control scrolling.
> These skills are essential for creating menus, settings panels, inventories, and any user interfaces.

---

## 🖱️ Button.onClick

### Purpose:
The `Button` is the primary interaction element. The `.onClick` event allows you to execute your code when the user presses and releases the button.

### How to use in code:
1. Get a reference to the `Button` component.
2. Add a listener using `onClick.AddListener(MethodName)`.
3. Write the method that will be called.
```csharp
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        Debug.Log("Game started!");
        // Load a scene, start the game, etc.
    }
}
```

### 🔧 Setup without code (for simple cases):
In the Button's Inspector, there's an `OnClick()` list. Drag an object there and select its method (e.g., `SceneManager.LoadScene`). Useful for quick prototypes.

### Example:
An `OptionsButton` opens the settings panel. On click – the panel becomes active (`SetActive(true)`).

---

## 📊 Slider

### Purpose:
A `Slider` allows the user to select a value from a range (e.g., volume, mouse sensitivity, font size).

### How to use in code:
1. Get a reference to the `Slider`.
2. Read its `.value` property.
3. React to changes using `onValueChanged.AddListener`.
```csharp
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioSource backgroundMusic;

    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        backgroundMusic.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }
}
```

### 🔧 Slider properties in the Inspector:
- `Min Value` / `Max Value` – the minimum and maximum values.
- `Whole Numbers` – if enabled, the value will be an integer (e.g., 1, 2, 3).
- `Value` – the current value (can be set manually).

### Example:
A volume slider from 0 to 1. As the user drags the handle, the music volume changes in real time, and the value is saved to `PlayerPrefs`.

---

## ✅ Toggle

### Purpose:
A `Toggle` is a checkbox that can be `true` (checked) or `false` (unchecked). Used for on/off settings: fullscreen mode, UI sounds, camera inversion.

### How to use in code:
1. Get a reference to the `Toggle`.
2. Read its `.isOn` property.
3. Subscribe to the `onValueChanged` event.
```csharp
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Toggle fullscreenToggle;

    void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
    }

    void OnFullscreenToggled(bool isOn)
    {
        Screen.fullScreen = isOn;
        Debug.Log("Fullscreen mode: " + isOn);
    }
}
```

### 🔧 Additional:
A `Toggle` can be part of a `Toggle Group`, where enabling one automatically disables others. Useful for selecting one option from several (e.g., difficulty: Easy / Medium / Hard).

### Example:
A checkbox "Disable music" – when checked, music volume becomes 0; when unchecked, it returns to the saved value.

---

## 📜 ScrollRect

### Purpose:
A `ScrollRect` is used for lists, inventories, long texts, or any content that doesn't fit on the screen.

### How to use in code:
Although `ScrollRect` is often configured in the Inspector, scripts allow you to control scrolling programmatically:
```csharp
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Button scrollToTopButton;

    void Start()
    {
        scrollToTopButton.onClick.AddListener(ScrollToTop);
    }

    void ScrollToTop()
    {
        scrollRect.verticalNormalizedPosition = 1f; // 1 = top, 0 = bottom
    }
    
    public float GetScrollPosition()
    {
        return scrollRect.verticalNormalizedPosition;
    }
}
```

### 🔧 Inspector setup for ScrollRect (important!):
1. `Content` – the object with `RectTransform` that holds the list items. Its size determines the scrollable area.
2. `Viewport` – the "window" through which the content is visible.
3. `Horizontal` / `Vertical` – allow horizontal/vertical scrolling.
4. `Scroll Sensitivity` – mouse wheel sensitivity.

### Example:
An inventory with 20 items. You dynamically create 20 buttons as children of `Content`, and `ScrollRect` automatically adds a scroll bar – the player can scroll through the entire inventory.

---

## 🔗 Combining Elements in a Real Project

### UI elements often work together:
- **Slider** changes a `Text` value (displaying current volume as a percentage).
- **Toggle** enables/disables a GameObject (e.g., an advanced settings panel).
- **Button** saves all settings or closes the menu.
- **ScrollRect** contains a list of levels, and clicking a button in the list loads that level.

### Example code for combining:
```csharp
public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle musicToggle;
    public Text volumePercentText;
    public Button saveButton;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(v => volumePercentText.text = Mathf.Round(v * 100) + "%");
        musicToggle.onValueChanged.AddListener(OnMusicToggled);
        saveButton.onClick.AddListener(SaveSettings);
    }

    void OnMusicToggled(bool isOn) { /* Turn background music off/on */ }
    void SaveSettings() { /* Save everything to PlayerPrefs */ }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
