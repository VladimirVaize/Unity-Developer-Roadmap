# 💾 Система сохранений в Unity: JSON, шифрование, бинарные файлы
Сохранение и загрузка данных — одна из ключевых частей любой игры: 
прогресс игрока, настройки, инвентарь, достижения. В Unity существует несколько подходов к реализации системы сохранений. 
В этом README мы разберём три основных метода: JSON (текстовый формат), 
бинарные файлы (для сложных данных) и шифрование (для защиты от редактирования).

---

## 1. JSON — текстовый формат обмена данными
JSON (JavaScript Object Notation) — лёгкий текстовый формат, основанный на структуре «ключ-значение». 
Человеку легко читать и редактировать, машине — парсить.
### 📦 Варианты работы с JSON в Unity:
| Инструмент | Описание | Плюсы | Минусы |
| --- | --- | --- | --- |
| JsonUtility | Встроенный в Unity (минимальный функционал) | Не требует установки, работает быстро | Не поддерживает словари `Dictionary`, сложные вложенные структуры |
| Newtonsoft.JSON | Сторонняя библиотека (Json.NET) | Поддерживает всё (`Dictionary`, `List`, LINQ), гибкая настройка | Требует установки (UPM или DLL) |

### 🧪 JsonUtility — базовый пример
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
        // Путь: C:/Users/.../AppData/LocalLow/CompanyName/GameName/save.json (Windows)
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    public void SaveGame(PlayerSaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // true = красивое форматирование
        File.WriteAllText(savePath, json);
        Debug.Log($"Сохранено: {json}");
    }

    public PlayerSaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
            Debug.Log($"Загружено: {json}");
            return data;
        }
        else
        {
            Debug.LogWarning("Файл сохранения не найден");
            return null;
        }
    }
}
```

### 🧪 Newtonsoft.JSON — расширенный пример
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
    public Dictionary<string, int> inventory; // JsonUtility НЕ поддерживает Dictionary!
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
        // Настройка форматирования (красивый JSON)
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

### 📝 Когда использовать:
| JsonUtility | Newtonsoft.JSON |
| --- | --- |
| Простые данные (примитивы, массивы, списки) | Сложные данные (`Dictionary`, LINQ) |
| Не нужна совместимость со старыми версиями | Тонкая настройка сериализации |
| Быстрая разработка без лишних зависимостей | Работа с JSON из внешних API |

---

## 2. Бинарные файлы (Binary Serialization)
Бинарные файлы хранят данные в нечитаемом для человека виде. 
Это ускоряет загрузку/сохранение и усложняет ручное редактирование, 
но не является полноценной защитой.

### 🛠️ Два подхода к бинарной сериализации:
| Подход | Описание | Плюсы | Минусы |
| --- | --- | --- | --- |
| BinaryFormatter | Встроенный в .NET (устаревший) | Простой, не требует установки | Небезопасен, не рекомендуется Unity (может быть удалён) |
| System.IO + BinaryWriter | Ручная запись полей | Полный контроль, безопасно | Много ручного кода |

> [!Important]
> BinaryFormatter объявлен устаревшим и небезопасным.
> В современных проектах рекомендуется использовать ручную сериализацию.

### 📝 Пример ручной бинарной сериализации:
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
        Debug.Log("Сохранено в бинарный файл");
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

### 📝 Пример с версионированием (для совместимости обновлений):
```csharp
public class VersionedBinarySave : MonoBehaviour
{
    private const int SAVE_VERSION = 2;
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "game_versioned.bin");
    }

    public void SaveGame(PlayerData data)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            writer.Write(SAVE_VERSION); // Сначала пишем версию
            writer.Write(data.playerName);
            writer.Write(data.level);
            writer.Write(data.experience);
        }
    }

    public PlayerData LoadGame()
    {
        if (!File.Exists(savePath)) return null;

        using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            int version = reader.ReadInt32();
            PlayerData data = new PlayerData();

            if (version == 1)
            {
                // Старая версия (только имя и уровень)
                data.playerName = reader.ReadString();
                data.level = reader.ReadInt32();
                data.experience = 0; // значение по умолчанию
            }
            else if (version == 2)
            {
                // Новая версия
                data.playerName = reader.ReadString();
                data.level = reader.ReadInt32();
                data.experience = reader.ReadInt32();
            }
            return data;
        }
    }
}
```

---

## 3. Шифрование данных
Шифрование защищает файлы сохранений от редактирования пользователем (читай: читерства). 
Оно не делает файл нечитаемым на 100% (опытный взломщик обойдёт любое шифрование), но отпугивает 99% обычных игроков.

### 🛡️ Популярные методы шифрования:
| Метод | Описание | Когда использовать |
| --- | --- | --- |
| Base64 | Кодирование (не шифрование!) в текст | Для передачи через текстовые протоколы |
| AES (симметричное) | Надёжное шифрование с одним ключом | Сохранения игрока, настройки |
| RSA (асимметричное) | Пара ключей (публичный + приватный) | Лицензии, покупки (редко в сохранениях) |
| XOR (простейшее) | Побитовая операция | Быстрое «запутывание» данных |

### 🔐 Пример AES-шифрования (рекомендуемый):
```csharp
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AESEncryption : MonoBehaviour
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("01234567890123456789012345678901"); // 32 байта (256 бит)
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("0123456789012345"); // 16 байт (128 бит)

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

### 🔐 Пример простого XOR-шифрования (быстрое, но слабое):
```csharp
public class XOREncryption
{
    private static byte[] xorKey = Encoding.UTF8.GetBytes("MySecretKey123");

    public static byte[] Encrypt(byte[] data)
    {
        byte[] result = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            result[i] = (byte)(data[i] ^ xorKey[i % xorKey.Length]);
        }
        return result;
    }

    public static byte[] Decrypt(byte[] data)
    {
        // XOR симметричен: повторное применение восстанавливает данные
        return Encrypt(data);
    }
}

// Использование:
byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
byte[] encrypted = XOREncryption.Encrypt(jsonBytes);
File.WriteAllBytes(savePath, encrypted);
```

---

## 4. Полная система сохранений (комбинированный подход)
Объединяем всё лучшее: JSON для удобства + AES для защиты + Binary для производительности (по необходимости).
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
        // 1. Сериализуем в JSON
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        
        // 2. Шифруем
        byte[] encrypted = EncryptAES(json);
        
        // 3. Сохраняем на диск
        File.WriteAllBytes(savePath, encrypted);
        Debug.Log("Игра сохранена с шифрованием");
    }

    public T LoadGame<T>() where T : new()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("Файл сохранения не найден, создаём новый");
            return new T();
        }

        // 1. Читаем зашифрованные байты
        byte[] encrypted = File.ReadAllBytes(savePath);
        
        // 2. Расшифровываем
        string json = DecryptAES(encrypted);
        
        // 3. Десериализуем из JSON
        return JsonConvert.DeserializeObject<T>(json);
    }

    private byte[] EncryptAES(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = aesKey;
            aes.IV = aesIV;
            ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }
    }

    private string DecryptAES(byte[] cipherBytes)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = aesKey;
            aes.IV = aesIV;
            ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
```

---

## 5. Где хранить файлы? `Application.persistentDataPath`
Unity предоставляет специальный путь `Application.persistentDataPath`, который гарантированно доступен для чтения и записи на всех платформах.

| Платформа | Путь |
| --- | --- |
| Windows | `C:\Users\<user>\AppData\LocalLow\<company>\<product>\` |
| macOS | `~/Library/Application Support/<company>/<product>/` |
| iOS | `/var/mobile/Containers/Data/Application/<app-id>/Documents/` |
| Android | `/storage/emulated/0/Android/data/<package-name>/files/` |
| WebGL | `IndexedDB` (специфичная реализация) |

```csharp
string saveFolder = Application.persistentDataPath;
string fullPath = Path.Combine(saveFolder, "saves", "slot_1.json");

// Создаём вложенную папку, если её нет
Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
```

---

## 6. Рекомендации
1. Всегда проверяйте существование файла перед загрузкой
2. Используйте `try-catch` при работе с файловой системой (ошибки прав доступа, повреждённые файлы)
3. Храните ключи шифрования в защищённом месте (не в коде, если возможно, через `ScriptableObject` или удалённый сервер)
4. Делайте резервные копии старых сохранений перед перезаписью
5. Версионируйте формат сохранений — добавьте поле `version`
6. Для больших объёмов данных используйте бинарную сериализацию + сжатие (GZip)

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
