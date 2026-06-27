# 🎨 UI Toolkit: A New Way to Create UI in Unity (Web-Inspired Technology)
UI Toolkit is a modern UI creation system in Unity, inspired by web technologies. 
It replaces the old Canvas (uGUI) and offers separation of structure, style, and logic — similar to HTML, CSS, and JavaScript in web development.

> [!Important]
> Unity recommends using UI Toolkit for new projects, but uGUI and IMGUI remain relevant for certain scenarios and legacy project support.

---

## 1. Core Concepts of UI Toolkit
UI Toolkit is based on a retained mode system — unlike uGUI, where UI is redrawn every frame, here the visual tree is created once and stored in memory, with changes applied automatically.

### Key Components:

| Component | Description | Web Analogy |
| --- | --- | --- |
| Visual Tree | Tree of visual elements defining UI structure | DOM tree |
| UXML | Markup describing UI structure | HTML |
| USS | Style sheets for visual styling | CSS |
| C# Scripts | Logic and event handling | JavaScript |

UI Toolkit works in both Editor and runtime, allowing you to create game interfaces, editor extensions, and debugging tools.

---

## 2. UXML — UI Structure (like HTML)
UXML (Unity eXtensible Markup Language) is a markup language inspired by HTML and XML. 
It defines UI structure and allows creating reusable templates.

### 📝 Basic UXML Example:
```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement name="root-container">
        <ui:Label name="title" text="Welcome!" />
        <ui:Button name="start-button" text="Start Game" />
        <ui:TextField name="player-name" label="Player Name" />
    </ui:VisualElement>
</ui:UXML>
```

### 🔧 Creating UXML:
1. Manually: create a `.uxml` file
2. Via UI Builder: visual editor, similar to web page builders

---

## 3. USS — UI Styling (like CSS)
USS (Unity Style Sheets) are style sheets that apply visual styles and layout rules to UI. 
They support a subset of standard CSS properties and use the Flexbox layout model.

### 📝 Basic USS Example:
```css
#root-container {
    flex-grow: 1;
    background-color: rgb(30, 30, 45);
    padding: 20px;
}

#title {
    font-size: 32px;
    color: rgb(255, 200, 50);
    -unity-font-style: bold;
    margin-bottom: 10px;
}

.primary-button {
    background-color: rgb(50, 120, 255);
    border-radius: 8px;
    padding: 10px 20px;
    font-size: 18px;
    color: white;
}

.primary-button:hover {
    background-color: rgb(80, 150, 255);
}

.horizontal-container {
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
}
```

### 🎯 USS Selectors:
| Selector | Example | What It Selects |
| --- | --- | --- |
| Type | `Button` | All `Button` elements |
| Class | `.primary-button` | Elements with class `primary-button` |
| ID | `#submit-btn` | Element named `submit-btn` |
| Pseudo-class | `:hover`, `:active`, `:disabled` | Element states |

---

## 4. UIDocument — Connecting UI to the Scene
UIDocument is a component that connects UI Toolkit to a GameObject in the scene. It is analogous to the Canvas component in uGUI.

### 🎮 Scene Setup:
1. GameObject → UI Toolkit → UI Document
2. Assign Source Asset — your `.uxml` file in the Inspector
3. Optionally configure Panel Settings (resolution, scaling)

### 📝 Code Example:
```csharp
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private void OnEnable()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;
        
        Button startButton = root.Q<Button>("start-button");
        startButton.clicked += OnStartButtonClicked;
    }
    
    private void OnStartButtonClicked()
    {
        Debug.Log("Game starting!");
    }
}
```

---

## 5. Event Handling
Events in UI Toolkit work similarly to DOM events in browsers — they bubble up the tree and can be captured at different levels.

### 📝 Subscribing to Events:
```csharp
private void RegisterEvents()
{
    root.Query<Button>().ForEach(button => {
        button.RegisterCallback<ClickEvent>(OnButtonClick);
    });
}

private void OnButtonClick(ClickEvent evt)
{
    Button clicked = evt.currentTarget as Button;
    Debug.Log($"Button clicked: {clicked.text}");
}
```

---

## 6. Complete Example: Tabbed Menu
A simple three-tab menu using all three UI Toolkit components.

### 📄 `TabbedMenu.uxml` (structure):
```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <Style src="TabbedMenu.uss" />
    
    <ui:VisualElement name="tabs">
        <ui:Label name="LondonTab" text="London" class="tab currently-selected" />
        <ui:Label name="ParisTab" text="Paris" class="tab" />
        <ui:Label name="OttawaTab" text="Ottawa" class="tab" />
    </ui:VisualElement>
    
    <ui:VisualElement name="tab-content">
        <ui:Label name="LondonContent" text="London is the capital of England" />
        <ui:Label name="ParisContent" text="Paris is the capital of France" class="hidden" />
        <ui:Label name="OttawaContent" text="Ottawa is the capital of Canada" class="hidden" />
    </ui:VisualElement>
</ui:UXML>
```

### 🎨 `TabbedMenu.uss` (styles):
```css
#tabs {
    flex-direction: row;
    background-color: rgb(229, 223, 223);
    font-size: 14px;
}

.tab {
    flex-grow: 1;
    padding: 10px;
    text-align: center;
}

.currently-selected {
    background-color: rgb(173, 166, 166);
}

.hidden {
    display: none;
}
```

### 💻 `TabbedMenuController.cs` (logic):
```csharp
using UnityEngine;
using UnityEngine.UIElements;

public class TabbedMenu : MonoBehaviour
{
    private void OnEnable()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;
        
        root.Query<Label>(className: "tab").ForEach(tab => {
            tab.RegisterCallback<ClickEvent>(OnTabClick);
        });
    }
    
    private void OnTabClick(ClickEvent evt)
    {
        Label clickedTab = evt.currentTarget as Label;
        
        clickedTab.parent.Query<Label>().ForEach(tab => {
            tab.RemoveFromClassList("currently-selected");
        });
        
        clickedTab.AddToClassList("currently-selected");
        
        string contentName = clickedTab.name.Replace("Tab", "Content");
        VisualElement content = clickedTab.parent.parent.Q<Label>(contentName);
        
        clickedTab.parent.parent.Q<VisualElement>("tab-content")
            .Query<Label>().ForEach(label => {
                label.AddToClassList("hidden");
            });
        
        content.RemoveFromClassList("hidden");
    }
}
```

---

## 7. uGUI vs UI Toolkit Comparison

| Aspect | uGUI (Canvas) | UI Toolkit |
| --- | --- | --- |
| Structure | GameObjects + Components | Visual Tree (lightweight nodes) |
| Markup | Visual Designer | UXML (text-based) |
| Styling | Inspector | USS (CSS-like) |
| Performance | Good for simple UI | Better for complex UI |
| Editor UI | ❌ Not supported | ✅ Supported |
| Scaling | Canvas Scaler | Flexbox |
| Debugging | Inspector | UI Debugger (browser-like) |
| Data Binding | ❌ Manual update | ✅ Built-in system |

---

## 8. UI Toolkit Tools
### 🛠️ UI Builder:
Visual editor for creating UXML and USS without writing code. Available via Window → UI Toolkit → UI Builder.

### 🐛 UI Debugger:
Debugging tool similar to browser "Inspect Element". Allows exploring the visual tree, viewing applied styles, and checking element states.

Available via Window → UI Toolkit → Debugger.

### 📚 UI Samples:
Library of ready-to-use UI Toolkit examples. Available via Window → UI Toolkit → Samples.

---

## 9. Best Practices
### ✅ Recommendations:
1. Separate structure, style, and logic — UXML for markup, USS for styles, C# for logic
2. Use UI Builder for rapid prototyping
3. Learn Flexbox — key to responsive design in UI Toolkit
4. Apply CSS approach — reuse classes, use pseudo-classes (`:hover`, `:active`)
5. For complex UI, use Data Binding for automatic data updates

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Searching for elements at the wrong time
void Start()
{
    var button = GetComponent<UIDocument>().rootVisualElement.Q<Button>("btn");
}

// ✅ CORRECT: In OnEnable or after UI creation
private void OnEnable()
{
    var root = GetComponent<UIDocument>().rootVisualElement;
    var button = root.Q<Button>("btn");
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
