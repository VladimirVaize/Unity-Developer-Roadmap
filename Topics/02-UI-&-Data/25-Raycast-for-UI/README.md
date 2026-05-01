# 🎯 Raycast for UI: Click Blocking and Graphic Raycaster

> [!Note]
> In Unity, the user interface (UI) operates by special rules.
> A standard physics Raycast does not interact with UI elements.
> To handle clicks on buttons, panels, images, and to prevent clicks from passing through the UI,
> the Graphic Raycaster system and the concept of click blocking are used.

---

## 📦 What is Graphic Raycaster?
Graphic Raycaster is a component added to a Canvas that is responsible for:
- Detecting which UI element (Image, Button, Text, etc.) the user clicked on.
- Sending events (e.g., `IPointerClickHandler`) to those elements.
- Blocking physics rays (or camera rays) so that a click doesn't pass through the UI into the 3D/2D world.

> Without Graphic Raycaster, buttons and other UI elements will not respond to clicks.

---

## 🛡️ Click Blocking (How to Prevent Clicks from Passing Through the UI)
When an inventory or pause window is displayed over the game world, the click should be absorbed by the UI instead of hitting 3D objects.

### How it works:
1. On a mouse click, Unity checks if there is a UI element under the cursor.
2. The Graphic Raycaster on the Canvas processes all UI objects and determines the topmost one.
3. If a UI element is found, the click event is not forwarded to the physics Raycast (e.g., from `Physics.Raycast`).
4. Thus, you cannot shoot through an inventory panel.

### Configuration example:
- The Canvas already has a `Canvas Scaler` and `Graphic Raycaster` component.
- Each UI element (Button, Image) has an `Image` component with the `Raycast Target` checkbox enabled.
  - If you uncheck this, the element becomes "transparent" to rays — clicks will pass through it.
 
---

## 🧪 Why this is needed (usage examples)

### 1. Blocking world interaction while UI is open
Scenario: The player opens the inventory (a `Canvas` with `Graphic Raycaster`). 
While the inventory is open, they should not shoot or pick up items.

#### How to use:
- Simply place a UI panel covering the entire screen (e.g., semi-transparent background).
- The Graphic Raycaster will intercept all clicks.
- In your script, check `EventSystem.current.IsPointerOverGameObject()` — if `true`, the click is on the UI, so you ignore world actions.

### 2. Preventing clicks from passing through buttons
Scenario: You have a large "Start" button. If the user clicks slightly outside it, that click should not go into the game world.

#### How to use:
- Set `Raycast Target = true` on the background panel (or on the button itself).
- Ensure there are no other interactive UI elements under the button unless necessary.

### 3. Conditional blocking by element types
Scenario: You want a click on an empty area of the UI to close the window, but to pass through a certain decorative icon.

#### How to use:
- Uncheck `Raycast Target` on the decorative icon.
- Then a click will pass through it and can be caught by a panel underneath or even the 3D world.

---

## 🔧 Key components and settings

| Component / Parameter | Purpose |
|-----------------|------------------------------|
| `Graphic Raycaster` (on Canvas) | Enables UI raycasting. Without it, the UI is not clickable. |
| `Raycast Target` (on UI element) | Determines whether this element should intercept clicks. |
| `EventSystem` (in the scene) | Handles input (mouse, touches). Without it, raycasts won't work. |
| `EventSystem.current.IsPointerOverGameObject()` | A method in scripts to check if the UI is currently being clicked. |

---

## 💡 Common mistake
### Mistake: 
- The UI does not respond to clicks, even though the Canvas and buttons exist.

### Cause: 
- The Canvas is missing the `Graphic Raycaster` component, or there is no `EventSystem` in the scene.

### Solution:
- Select Canvas → `Add Component` → `Graphic Raycaster`.
- If there's no `EventSystem`, create one: `GameObject` → `UI` → `Event System`.

---

## 🧠 Summary
- `Graphic Raycaster` is a mandatory component for UI click functionality.
- It automatically blocks clicks from passing through to 3D objects if the click hits the UI.
- Control the "transparency" of individual elements to rays via the `Raycast Target` flag.
- For checking in code, use `EventSystem.current.IsPointerOverGameObject()`.

---

### ⭐ If this project was useful, put a star on GitHub!
