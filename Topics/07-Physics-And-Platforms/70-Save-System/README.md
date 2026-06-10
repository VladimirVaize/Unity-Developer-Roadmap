# 💾 Save System in Unity: JSON, Encryption, Binary Files
Saving and loading data is a core part of any game: player progress, settings, inventory, achievements. 
Unity offers several approaches to implementing a save system. 
In this README, we'll cover three main methods: JSON (text format), 
binary files (for complex data), and encryption (to protect against editing).

---

## 1. JSON — Text-Based Data Exchange Format
JSON (JavaScript Object Notation) is a lightweight text format based on key-value structures. 
It's human-readable and machine-parseable.

### 📦 JSON Options in Unity:
| Tool | Description | Pros | Cons |
| --- | --- | --- | --- |
| JsonUtility | Built into Unity (minimal functionality) | No installation, fast | No Dictionary support, limited complex structures |
| Newtonsoft.JSON | Third-party library (Json.NET) | Supports everything (Dictionary, List, LINQ), flexible | Requires installation (UPM or DLL) |

### 🧪 JsonUtility — Basic Example
```csharp
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerSaveData
{
    public string playerName;
    public int level;
    public float health;
    public Vector3 position;
}

public class JsonUtilityExample : MonoBehaviour
{
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    public void SaveGame(PlayerSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Saved: {json}");
    }

    public PlayerSaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<PlayerSaveData>(json);
        }
        return null;
    }
}
```

### 🧪 Newtonsoft.JSON — Advanced Example
```csharp
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

[System.Serializable]
public class PlayerSaveDataAdvanced
{
    public string playerName;
    public int level;
    public Dictionary<string, int> inventory; // JsonUtility does NOT support Dictionary!
    public List<float> highScores;
}

public class NewtonsoftExample : MonoBehaviour
{
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save_advanced.json");
    }

    public void SaveGame(PlayerSaveDataAdvanced data)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };
        
        string json = JsonConvert.SerializeObject(data, settings);
        File.WriteAllText(savePath, json);
    }

    public PlayerSaveDataAdvanced LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonConvert.DeserializeObject<PlayerSaveDataAdvanced>(json);
        }
        return null;
    }
}
```

---

## 2. Binary Files (Binary Serialization)
Binary files store data in a non-human-readable format. 
This speeds up saving/loading and complicates manual editing, but is not full protection.

### 🛠️ Two Approaches to Binary Serialization:
| Approach | Description | Pros | Cons |
| --- | --- | --- | --- |
| BinaryFormatter | Built into .NET (deprecated) | Simple, no installation | Unsafe, may be removed from Unity |
| System.IO + BinaryWriter | Manual field writing | Full control, safe | Lots of manual code |

### 📝 Manual Binary Serialization Example:
```csharp
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    public int score;
    public float timePlayed;
    public string levelName;
}

public class BinarySaveSystem : MonoBehaviour
{
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "game.bin");
    }

    public void SaveGame(GameData data)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            writer.Write(data.score);
            writer.Write(data.timePlayed);
            writer.Write(data.levelName);
        }
    }

    public GameData LoadGame()
    {
        if (!File.Exists(savePath)) return null;

        using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            GameData data = new GameData();
            data.score = reader.ReadInt32();
            data.timePlayed = reader.ReadSingle();
            data.levelName = reader.ReadString();
            return data;
        }
    }
}
```

---

## 3. Data Encryption
Encryption protects save files from user editing (cheating). 
It doesn't make the file 100% unreadable (a determined hacker can bypass anything), 
but deters 99% of regular players.

### 🛡️ Popular Encryption Methods:
| Method | Description | When to Use |
| --- | --- | --- |
| Base64 | Encoding (not encryption!) | For transmission via text protocols |
| AES (symmetric) | Strong encryption with single key | Player saves, settings |
| RSA (asymmetric) | Key pair (public + private) | Licenses, purchases (rare for saves) |
| XOR (simplest) | Bitwise operation | Fast data "obfuscation" |

### 🔐 AES Encryption Example (Recommended):
```csharp
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AESEncryption : MonoBehaviour
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("01234567890123456789012345678901");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("0123456789012345");

    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save_encrypted.dat");
    }

    public void SaveEncrypted(string jsonData)
    {
        byte[] dataToEncrypt = Encoding.UTF8.GetBytes(jsonData);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            using (var encryptor = aes.CreateEncryptor())
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                cryptoStream.FlushFinalBlock();
                byte[] encryptedData = memoryStream.ToArray();
                File.WriteAllBytes(savePath, encryptedData);
            }
        }
    }

    public string LoadEncrypted()
    {
        if (!File.Exists(savePath)) return null;

        byte[] encryptedData = File.ReadAllBytes(savePath);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            using (var decryptor = aes.CreateDecryptor())
            using (var memoryStream = new MemoryStream(encryptedData))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cryptoStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
```

---

## 4. Complete Save System (Combined Approach)
Combine the best: JSON for convenience + AES for protection + Binary for performance (if needed).
```csharp
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public class CompleteSaveSystem : MonoBehaviour
{
    private string savePath;
    private readonly byte[] aesKey = Encoding.UTF8.GetBytes("32ByteLongKeyForAES256Encrypt!!");
    private readonly byte[] aesIV = Encoding.UTF8.GetBytes("16ByteIVForAES!!");

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "game_save.dat");
    }

    public void SaveGame<T>(T saveData)
    {
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        byte[] encrypted = EncryptAES(json);
        File.WriteAllBytes(savePath, encrypted);
    }

    public T LoadGame<T>() where T : new()
    {
        if (!File.Exists(savePath)) return new T();

        byte[] encrypted = File.ReadAllBytes(savePath);
        string json = DecryptAES(encrypted);
        return JsonConvert.DeserializeObject<T>(json);
    }

    private byte[] EncryptAES(string plainText) { /* ... */ }
    private string DecryptAES(byte[] cipherBytes) { /* ... */ }
}
```

---

## 5. Where to Store Files? `Application.persistentDataPath`
Unity provides `Application.persistentDataPath`, guaranteed to be readable/writable on all platforms.

| Platform | Path |
| --- | --- |
| Windows | `C:\Users\<user>\AppData\LocalLow\<company>\<product>\` |
| macOS | `~/Library/Application Support/<company>/<product>/` |
| iOS | `/var/mobile/Containers/Data/Application/<app-id>/Documents/` |
| Android | `/storage/emulated/0/Android/data/<package-name>/files/` |
| WebGL | `IndexedDB` (implementation-specific) |

---

## 6. Recommendations
1. Always check if file exists before loading
2. Use try-catch for file system operations
3. Store encryption keys securely (not in code if possible)
4. Back up old saves before overwriting
5. Version your save format — add a `version` field
6. For large data, use binary serialization + compression (GZip)

---

### ⭐ If this project was useful, put a star on GitHub!
