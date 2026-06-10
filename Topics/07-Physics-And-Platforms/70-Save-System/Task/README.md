# 🎯 Task: «RPG Game Progress Save System»
You are developing an RPG game. You need to implement a complete save/load system with JSON (Newtonsoft.JSON), AES encryption, and binary slots.

## 📝 Save Data Structure:
```csharp
public class RPGPlayerData
{
    public string playerName;
    public int level;
    public int experience;
    public float currentHealth;
    public float maxHealth;
    public List<string> unlockedAbilities;
    public Dictionary<string, int> inventory; // itemId -> quantity
    public Vector3 lastPosition;
    public float playTimeSeconds;
}
```

---

## 📋 Tasks to Implement:
1. Serialization — save `RPGPlayerData` object to JSON using Newtonsoft.JSON
2. Encryption — encrypt the JSON string using AES algorithm (use key/IV from the example above)
3. Binary Save — write encrypted bytes to file with `.rpgdata` extension
4. Loading — read file, decrypt, deserialize to object
5. Save Slots — support up to 3 slots (slot0, slot1, slot2) in `Application.persistentDataPath`
6. Screenshot — take a screenshot before saving and save it as PNG (in separate `screenshots/` folder)

---

## 🧰 Implementation Requirements:
- Create a class `RPGSaveManager` with methods:
  - `SaveGame(int slot, RPGPlayerData data)`
  - `RPGPlayerData LoadGame(int slot)`
  - `bool HasSaveInSlot(int slot)`
  - `DeleteSave(int slot)`
  - `CaptureAndSaveScreenshot(int slot)` (takes 256x256 screenshot)
 
- Handle all errors (missing file, corrupted data) with `try-catch`
- On load, verify the file is not corrupted (decryption is valid)
- Store a hash (MD5 or SHA256) of the save for integrity checking

---

## 📁 Expected File Structure:
```text
persistentDataPath/
├── saves/
│   ├── slot0.rpgdata
│   ├── slot1.rpgdata
│   └── slot2.rpgdata
└── screenshots/
    ├── slot0.png
    ├── slot1.png
    └── slot2.png
```

---

## 🎮 Bonus Challenge (Optional):
Add autosave every 60 seconds to the current active slot, but not during combat (flag `isInCombat`).

---

## 💡 Hints:
- For screenshots, use `ScreenCapture.CaptureScreenshot()` or `Texture2D.ReadPixels()` + `EncodeToPNG()`
- For hashing: `System.Security.Cryptography.MD5.Create().ComputeHash()`
- If a save is corrupted, create a new object with default values

---

### ⭐ If this project was useful, put a star on GitHub!
