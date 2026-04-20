# 🧪 Practical Task: «Settings and High Scores with PlayerPrefs»

## 🎯 Objective

Create a small game application in Unity that uses `PlayerPrefs` to:
  - Save the best high score
  - Save music volume settings
  - Save the last player name

All data must persist between game launches.

---

## 📝 Task Description
You need to implement a scene with the following UI elements:
1. Score text — increases by 10 each time you click the `Click!` button.
2. High Score text — updates if the current score exceeds the saved high score.
3. `Click!` button — adds 10 points to the current score.
4. `Reset Score` button — resets the current score to 0 (does not affect the high score).
5. Slider for music volume — range 0..1, the value must be saved and applied to `AudioListener.volume`.
6. InputField for player name — saves automatically when focus is lost or Enter is pressed.
7. `Clear All Data` button — calls `PlayerPrefs.DeleteAll()` and resets all UI elements to their initial state.

---

## ✅ Requirements
- On first launch: high score = 0, volume = 0.5, name = `"Player"`.
- When a new high score is achieved, show a message in the console or on screen.
- Use `PlayerPrefs.Save()` after every data change.
- In the `Start()` method, load all saved values and apply them to the UI.

---

## 📦 What to submit
- Scene file (`.unity`)
- Script file (`.cs`) with PlayerPrefs and UI logic
- Brief description (or code comments) explaining how each save/load works

---

## 🚀 Bonus Task (optional)
Add a second high score — «Best Click Streak» (Max Streak), which increases while you keep clicking without resetting the score. 
If `Reset Score` is pressed, the current streak resets to 0, but the record streak is preserved unless a new streak beats it.

---

## Solution

- <a href="../Solution/PlayerPrefsDemo.cs">PlayerPrefsDemo.cs</a> script.

---

### ⭐ If this project was useful, put a star on GitHub!
