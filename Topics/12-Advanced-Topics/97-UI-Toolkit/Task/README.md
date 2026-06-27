# 🎯 Task: «Creating a Character Panel Using UI Toolkit»
You are developing an RPG game and need to create a Character Panel using UI Toolkit. The panel should display:
1. Avatar (image)
2. Character Name (text field)
3. Level (number)
4. Health and Mana bars (progress bars)
5. "Level Up" button (increases level)

## 📝 What to Implement:
### Part 1: Creating UXML (structure)
Create a `CharacterPanel.uxml` file with the following structure:
```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <Style src="CharacterPanel.uss" />
    
    <ui:VisualElement name="panel">
        <ui:VisualElement name="header">
            <ui:VisualElement name="avatar" />
            <ui:VisualElement name="info">
                <ui:Label name="character-name" text="Hero" />
                <ui:Label name="level-label" text="Level: 1" />
            </ui:VisualElement>
        </ui:VisualElement>
        
        <ui:VisualElement name="stats">
            <ui:VisualElement name="health-bar" class="progress-bar">
                <ui:VisualElement name="health-fill" class="progress-fill health-fill" />
                <ui:Label name="health-text" text="100/100" />
            </ui:VisualElement>
            
            <ui:VisualElement name="mana-bar" class="progress-bar">
                <ui:VisualElement name="mana-fill" class="progress-fill mana-fill" />
                <ui:Label name="mana-text" text="50/50" />
            </ui:VisualElement>
        </ui:VisualElement>
        
        <ui:Button name="level-up-btn" text="Level Up" class="primary-button" />
    </ui:VisualElement>
</ui:UXML>
```

### Part 2: Creating USS (styles)
Create a `CharacterPanel.uss` file with styles:
- Panel: dark background, rounded corners, padding
- Avatar: 64×64 square, grey fill, rounded corners
- Health/Mana bars: width 100%, height 20px, dark background
- Health fill: red color (`#e74c3c`)
- Mana fill: blue color (`#3498db`)
- Button: green, with hover effect
- Use Flexbox for layout

### Part 3: Creating the Controller Script
Write `CharacterPanelController.cs`:
1. Gets references to all UI elements via `root.Q<>()` and `root.Query<>()`
2. Implements `UpdateUI()` method to refresh display
3. Handles "Level Up" button click
4. On click:
   - Increases level by 1
   - Increases max health by 10
   - Increases max mana by 5
   - Restores health and mana to maximum
   - Updates UI
  
### Part 4: Data Model
Create `CharacterData` class:
```csharp
[System.Serializable]
public class CharacterData
{
    public string characterName = "Hero";
    public int level = 1;
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int maxMana = 50;
    public int currentMana = 50;
}
```

### Part 5: Scene Integration
1. Add UI Document to a GameObject in the scene
2. Assign `CharacterPanel.uxml` as Source Asset
3. Attach `CharacterPanelController` script to the same GameObject

---

## 🧰 Implementation Requirements:
- Use all three UI Toolkit components: UXML, USS, C#
- Apply Flexbox for element layout
- Use pseudo-classes (`:hover`, `:active`) for the button
- Add event handling via `RegisterCallback<>`
- UI must be responsive (resize with window)

---

## 🔍 Verification:
1. Run the scene — panel should display correctly
2. Click "Level Up" button — level should increase
3. Verify health and mana bars update
4. Verify button changes color on hover
5. Open UI Debugger and inspect the visual tree

---

## 💡 Expected Result:
```text
=== Character Panel ===
Name: Hero
Level: 1 → 2 → 3 ...
Health: 100/100 → 110/110 → 120/120 ...
Mana: 50/50 → 55/55 → 60/60 ...

=== On "Level Up" click ===
- Level increases
- Bars fill to maximum
- Button gives visual feedback
```

---

## 🏆 Bonus Task (Optional):
1. Data Binding: bind the data model to UI automatically
2. Animations: smooth bar filling via USS Transitions
3. Themes: create light and dark themes, switchable via button

```css
/* Example CSS transition for smooth filling */
.health-fill {
    transition: width 0.3s ease;
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
