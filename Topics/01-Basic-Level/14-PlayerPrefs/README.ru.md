# 🎮 PlayerPrefs: Простое сохранение данных в Unity

> [!NOTE]
> **PlayerPrefs** — это встроенный в Unity механизм для сохранения простых пользовательских данных между сессиями игры.
> Он идеально подходит для хранения настроек (громкость, чувствительность мыши, язык) и рекордов (лучший счёт, пройденные уровни).
> Данные сохраняются в реестре Windows, в файлах `.plist` на macOS или в хранилище приложения на мобильных устройствах.

---

## 🔧 Типы данных, которые можно сохранять
PlayerPrefs работает только с тремя простыми типами:

| Тип | Метод сохранения | Метод загрузки |
|---------|---------------------------|-----------------|
| `int` | `SetInt(string key, int value)` | `GetInt(string key)` |
| `float` | `SetFloat(string key, float value)` | `GetFloat(string key)` |
| `string` | `SetString(string key, string value)` | `GetString(string key)` |

> [!Important]
> 💡 Ключ (`key`) — это строка-идентификатор, например, `"Volume"`, `"HighScore"`, `"PlayerName"`.

---

## 📥 Сохранение данных (Set)

```csharp
// Сохраняем настройки
PlayerPrefs.SetInt("HighScore", 2500);
PlayerPrefs.SetFloat("MusicVolume", 0.75f);
PlayerPrefs.SetString("PlayerName", "Hero");
PlayerPrefs.Save(); // немедленная запись на диск (необязательно, но рекомендуется)
```
- `Save()` вызывать необязательно — Unity автоматически сохраняет при завершении приложения, но для надёжности лучше вызывать вручную.

---

## 📤 Загрузка данных (Get)

```csharp
// Загружаем данные (если ключа нет, вернётся значение по умолчанию)
int score = PlayerPrefs.GetInt("HighScore", 0);
float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
string name = PlayerPrefs.GetString("PlayerName", "Player");
```
- Второй параметр — значение по умолчанию. Если ключ никогда не сохранялся, вернётся именно оно.

---

## ✅ Проверка существования ключа

```csharp
if (PlayerPrefs.HasKey("HighScore"))
{
    // Ключ существует — можно безопасно загружать
    int best = PlayerPrefs.GetInt("HighScore");
}
else
{
    // Первый запуск — показываем приветствие
    Debug.Log("Добро пожаловать!");
}
```

---

## 🗑️ Удаление данных

```csharp
// Удалить один ключ
PlayerPrefs.DeleteKey("HighScore");

// Удалить ВСЕ данные приложения
PlayerPrefs.DeleteAll();
```

---

## 🎯 Типичные примеры использования

### 1. Сохранение рекорда (High Score)
```csharp
int currentScore = 3200;
int bestScore = PlayerPrefs.GetInt("HighScore", 0);

if (currentScore > bestScore)
{
    PlayerPrefs.SetInt("HighScore", currentScore);
    PlayerPrefs.Save();
    Debug.Log("Новый рекорд!");
}
```

### 2. Сохранение настроек громкости с ползунком (Slider)
```csharp
// При изменении слайдера
public void OnVolumeChanged(float value)
{
    PlayerPrefs.SetFloat("Volume", value);
    AudioListener.volume = value;
}

// При запуске сцены
void Start()
{
    float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);
    AudioListener.volume = savedVolume;
    // обновить значение слайдера UI
    volumeSlider.value = savedVolume;
}
```

### 3. Сохранение пройденных уровней
```csharp
// Когда игрок прошёл уровень 3
PlayerPrefs.SetInt("Level3Unlocked", 1);
PlayerPrefs.Save();

// При проверке, открыт ли уровень
bool IsLevelUnlocked(int levelIndex)
{
    return PlayerPrefs.GetInt("Level" + levelIndex + "Unlocked", 0) == 1;
}
```

---

## ⚠️ Ограничения и предупреждения

| ❌ Не используйте PlayerPrefs для... | ✅ Используйте для... |
|------------------------------------|---------------------------------------|
| Больших объёмов данных (списки, массивы, текстуры) | Небольших чисел и строк (до ~1 МБ) |
| Секретных данных (пароли, токены) — они хранятся в открытом виде | Настроек и рекордов (некритичная информация) |
| Сложных структур (классы, списки объектов) | Простых ключ-значение (int, float, string) |

> [!Note]
> 💡 Для сохранения массивов или сложных объектов можно использовать `JsonUtility` + `PlayerPrefs.SetString()`, но для больших данных лучше подойдут файлы (`Application.persistentDataPath`).

---

## 🔁 Полный жизненный цикл (код-пример)

```csharp
public class GameSettings : MonoBehaviour
{
    void Start()
    {
        // Загрузка при запуске
        LoadSettings();
    }

    void OnApplicationQuit()
    {
        // Сохранение при выходе
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

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
