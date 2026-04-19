# 🎮 PlayerPrefs: Simple Data Saving in Unity

> [!Note]
> PlayerPrefs is Unity's built-in mechanism for saving simple user data between game sessions.
> It is ideal for storing settings (volume, mouse sensitivity, language) and high scores (best score, unlocked levels).
> Data is stored in the Windows registry, `.plist` files on macOS, or the application's storage on mobile devices.

---

## 🔧 Supported Data Types
PlayerPrefs works with only three simple types:

| Type | Save Method | Load Method |
|---------|------------------------|-------------------|
| `int` | `SetInt(string key, int value)` | `GetInt(string key)` |
| `float` | `SetFloat(string key, float value)` | `GetFloat(string key)` |
| `string` | `SetString(string key, string value)` | `GetString(string key)` |

> [!Important]
> 💡 The `key` is an identifier string, e.g., `"Volume"`, `"HighScore"`, `"PlayerName"`.

---

## 📥 Saving Data (Set)
```csharp
// Save settings
PlayerPrefs.SetInt("HighScore", 2500);
PlayerPrefs.SetFloat("MusicVolume", 0.75f);
PlayerPrefs.SetString("PlayerName", "Hero");
PlayerPrefs.Save(); // immediate write to disk (optional but recommended)
```
- Calling `Save()` is optional — Unity auto-saves when the app closes, but it's safer to call it manually.

---

## 📤 Loading Data (Get)
```csharp
// Load data (returns default value if key doesn't exist)
int score = PlayerPrefs.GetInt("HighScore", 0);
float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
string name = PlayerPrefs.GetString("PlayerName", "Player");
```
- The second parameter is the default value. If the key was never saved, this default is returned.

---

## ✅ Checking if a Key Exists
```csharp
if (PlayerPrefs.HasKey("HighScore"))
{
    // Key exists — safe to load
    int best = PlayerPrefs.GetInt("HighScore");
}
else
{
    // First launch — show welcome message
    Debug.Log("Welcome!");
}
```

---

## 🗑️ Deleting Data
```csharp
// Delete a single key
PlayerPrefs.DeleteKey("HighScore");

// Delete ALL data for the application
PlayerPrefs.DeleteAll();
```

---

## 🎯 Typical Use Cases

### 1. Saving a High Score
```csharp
int currentScore = 3200;
int bestScore = PlayerPrefs.GetInt("HighScore", 0);

if (currentScore > bestScore)
{
    PlayerPrefs.SetInt("HighScore", currentScore);
    PlayerPrefs.Save();
    Debug.Log("New high score!");
}
```

### 2. Saving Volume Settings with a Slider
```csharp
// When the slider changes
public void OnVolumeChanged(float value)
{
    PlayerPrefs.SetFloat("Volume", value);
    AudioListener.volume = value;
}

// When the scene starts
void Start()
{
    float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);
    AudioListener.volume = savedVolume;
    // update UI slider value
    volumeSlider.value = savedVolume;
}
```

### 3. Saving Unlocked Levels
```csharp
// When the player completes level 3
PlayerPrefs.SetInt("Level3Unlocked", 1);
PlayerPrefs.Save();

// When checking if a level is unlocked
bool IsLevelUnlocked(int levelIndex)
{
    return PlayerPrefs.GetInt("Level" + levelIndex + "Unlocked", 0) == 1;
}
```

---

## ⚠️ Limitations and Warnings

| ❌ Don't use PlayerPrefs for... | ✅ Use for... |
| -------------------------------|-------------------------------|
| Large data (lists, arrays, textures) | Small numbers and strings (up to ~1 MB) |
| Secret data (passwords, tokens) — stored in plain text | Settings and high scores (non-critical info) |
| Complex structures (classes, object lists) | Simple key-value pairs (int, float, string) |

> [!Note]
> 💡 To save arrays or complex objects, you can use `JsonUtility` + `PlayerPrefs.SetString()`, but for large data, files (`Application.persistentDataPath`) are better.

---

## 🔁 Complete Lifecycle (Code Example)
```csharp
public class GameSettings : MonoBehaviour
{
    void Start()
    {
        // Load on startup
        LoadSettings();
    }

    void OnApplicationQuit()
    {
        // Save on exit
        SaveSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("HighScore", GameManager.Score);
        PlayerPrefs.SetFloat("Sensitivity", mouseSensitivity);
        PlayerPrefs.SetString("LastPlayer", playerName);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        GameManager.Score = PlayerPrefs.GetInt("HighScore", 0);
        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        playerName = PlayerPrefs.GetString("LastPlayer", "Guest");
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
