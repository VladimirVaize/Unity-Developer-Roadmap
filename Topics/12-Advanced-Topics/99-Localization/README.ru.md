# 🌍 Локализация в Unity: Мультиязычность, адаптация UI под разные языки, работа с текстом и шрифтами

Локализация — это процесс адаптации игры для разных языков и культурных особенностей. 
В Unity нет встроенной системы локализации, но существует мощное официальное решение — Localization Package, а также множество альтернативных подходов. 
В этом руководстве мы разберём все аспекты локализации: от работы с текстом до адаптации шрифтов под разные языки.

---

## 1. Введение: Зачем нужна локализация?
Локализация — это не просто перевод текста. Это комплексная адаптация, включающая:

| Аспект | Описание |
| --- | --- |
| Перевод текста | Все UI-элементы, диалоги, подсказки |
| Адаптация UI | Изменение размеров, расположения элементов |
| Шрифты | Поддержка кириллицы, азиатских языков, RTL-языков |
| Форматы | Даты, время, валюта, числа |
| Культурные особенности | Цвета, символы, жесты |
| Аудио | Озвучка на разных языках |

> 💡 Статистика: Игры с локализацией имеют на 26% больше выручки и в 2 раза выше конверсия в установку.

---

## 2. Unity Localization Package — Официальное решение
Unity Localization Package — это официальный пакет от Unity, предоставляющий полноценную систему локализации.

### 📦 Установка:
```text
Window → Package Manager → Unity Registry → Localization → Install
```

### 🔧 Основные компоненты:
| Компонент | Описание |
| --- | --- |
| Locale | Представляет язык и регион (например, ru-RU, en-US) |
| String Table | Таблица строк (ключ → перевод) |
| Asset Table | Таблица ассетов (ключ → ассет) |
| Localized String | Компонент для локализации текста |
| Localized Asset | Компонент для локализации ассетов |

### 📄 Пример: Создание таблицы строк
1. Project Window → Create → Localization → String Table Collection
2. Назвать `GameTexts`
3. Добавить локали: `English (en)`, `Russian (ru)`, `German (de)`
4. Добавить записи:

| Key | English | Russian | German |
| --- | --- | --- | --- |
| `ui_start` | Start | Начать | Starten |
| `ui_settings` | Settings | Настройки | Einstellungen |
| `ui_quit` | Quit | Выйти | Beenden |
| `msg_welcome` | Welcome to the game! | Добро пожаловать в игру! | Willkommen im Spiel! |
| `msg_health` | Health: {0} | Здоровье: {0} | Gesundheit: {0} |

### 💻 Использование в коде:
```csharp
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    public LocalizedString localizedString;
    
    void Start()
    {
        // Получение перевода по ключу
        string key = "ui_start";
        string translated = LocalizationSettings.StringDatabase.GetLocalizedString("GameTexts", key);
        Debug.Log($"Перевод '{key}' на текущий язык: {translated}");
        
        // Использование LocalizedString
        localizedString.TableReference = "GameTexts";
        localizedString.TableEntryReference = "msg_welcome";
        localizedString.StringChanged += OnStringChanged;
    }
    
    void OnStringChanged(string value)
    {
        Debug.Log($"Обновлённый текст: {value}");
        // Обновить UI-элемент
        GetComponent<TextMeshProUGUI>().text = value;
    }
}
```

### 🎮 Пример: Локализация UI-элемента
```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class LocalizedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;
    [SerializeField] private LocalizeStringEvent _localizeStringEvent;
    
    void Start()
    {
        // Способ 1: Через LocalizeStringEvent (рекомендуется)
        _localizeStringEvent.SetTable("GameTexts");
        _localizeStringEvent.SetEntry("ui_start");
        _localizeStringEvent.RefreshString();
        
        // Способ 2: Ручное обновление через код
        string key = "ui_settings";
        _textMesh.text = LocalizationSettings.StringDatabase.GetLocalizedString("GameTexts", key);
    }
}
```

---

## 3. Смена языка в рантайме
Переключение языка без перезапуска игры — ключевая функция локализации.

### 🔄 Пример: Система смены языка
```csharp
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }
    
    private Locale _currentLocale;
    public Locale CurrentLocale => _currentLocale;
    
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        // Загрузка сохранённого языка
        string savedLocale = PlayerPrefs.GetString("Language", "en");
        SetLanguage(savedLocale);
    }
    
    public void SetLanguage(string localeCode)
    {
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code == localeCode)
            {
                _currentLocale = locale;
                LocalizationSettings.SelectedLocale = locale;
                PlayerPrefs.SetString("Language", localeCode);
                PlayerPrefs.Save();
                Debug.Log($"Язык изменён на: {locale.LocaleName}");
                return;
            }
        }
        
        Debug.LogWarning($"Язык '{localeCode}' не найден, используется английский");
    }
    
    public void ToggleLanguage()
    {
        string currentCode = _currentLocale.Identifier.Code;
        string nextCode = currentCode == "en" ? "ru" : "en";
        SetLanguage(nextCode);
    }
}

// Использование в UI:
public class LanguageUI : MonoBehaviour
{
    public void OnEnglishClicked() => LanguageManager.Instance.SetLanguage("en");
    public void OnRussianClicked() => LanguageManager.Instance.SetLanguage("ru");
    public void OnToggleClicked() => LanguageManager.Instance.ToggleLanguage();
}
```

---

## 4. Работа с текстом и шрифтами
Разные языки требуют разных шрифтов. Unity использует TextMeshPro для качественного текста.

### 🅰️ Основные проблемы с шрифтами:

| Проблема | Описание | Решение |
| --- | --- | --- |
| Отсутствие глифов | Шрифт не содержит нужных символов | Использовать Fallback шрифты |
| Азиатские языки | Тысячи символов (китайский, японский) | Специальные шрифты с поддержкой |
| RTL-языки | Арабский, иврит (текст справа налево) | RTL плагины или специальная настройка |
| Размер текста | Немецкий длиннее английского | Автоматическое масштабирование |

### 🖋️ Настройка TextMeshPro для локализации:
Шаг 1: Создание шрифта с поддержкой языков
1. В окне Font Asset Creator выбрать основной шрифт
2. Добавить Fallback Font Assets для недостающих символов
3. Для кириллицы добавить Character Set = `Unicode Range (Hex)`: `0400-04FF`
4. Для китайского/японского: `4E00-9FFF`, `3040-30FF`
```csharp
// Пример: Загрузка шрифта в зависимости от языка
public class FontManager : MonoBehaviour
{
    public TMP_FontAsset defaultFont;
    public TMP_FontAsset cyrillicFont;
    public TMP_FontAsset asianFont;
    
    public TMP_FontAsset GetFontForLanguage(string languageCode)
    {
        switch (languageCode)
        {
            case "ru": return cyrillicFont;
            case "ja": return asianFont;
            case "zh": return asianFont;
            default: return defaultFont;
        }
    }
    
    public void ApplyFontToAllText(TMP_FontAsset font)
    {
        foreach (var text in FindObjectsOfType<TextMeshProUGUI>())
        {
            text.font = font;
        }
    }
}
```

### 📏 Адаптация размера текста
Разные языки имеют разную длину слов. Например:

| Английский | Немецкий | Русский |
| --- | --- | --- |
| `Settings` (8) | `Einstellungen` (13) | `Настройки` (9) |
| `Play` (4) | `Spielen` (7) | `Играть` (6) |
| `Continue` (8) | `Fortsetzen` (10) | `Продолжить` (10) |

Решение: Автоматическое масштабирование
```csharp
using TMPro;
using UnityEngine;

public class AutoFitText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _minFontSize = 14;
    [SerializeField] private float _maxFontSize = 36;
    [SerializeField] private float _preferredWidth = 200f;
    
    void Start()
    {
        FitTextToWidth();
    }
    
    public void FitTextToWidth()
    {
        // Сохраняем исходный текст
        string originalText = _text.text;
        
        // Пробуем разные размеры от max к min
        for (float size = _maxFontSize; size >= _minFontSize; size -= 1f)
        {
            _text.fontSize = size;
            _text.ForceMeshUpdate();
            
            // Если ширина текста меньше предпочтительной — останавливаемся
            if (_text.preferredWidth <= _preferredWidth)
            {
                break;
            }
        }
        
        Debug.Log($"Итоговый размер шрифта: {_text.fontSize} для текста '{originalText}'");
    }
}
```

---

## 5. Поддержка RTL-языков (Арабский, Иврит)
RTL (Right-to-Left) языки требуют специальной обработки.

### 🧩 Решение: Arabic Support для Unity
```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArabicSupport; // Подключаем Arabic Support пакет

public class RTLTextFixer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private bool _useTashkeel = true;
    [SerializeField] private bool _useHinduNumbers = false;
    
    void Start()
    {
        if (IsRTL())
        {
            FixRTLText();
        }
    }
    
    public void FixRTLText()
    {
        string original = _text.text;
        string fixedText = ArabicFixer.Fix(original, _useTashkeel, _useHinduNumbers);
        _text.text = fixedText;
        
        // Для RTL текста нужно переключить выравнивание
        _text.alignment = TextAlignmentOptions.Right;
        _text.isRightToLeftText = true;
    }
    
    private bool IsRTL()
    {
        string currentLang = LocalizationSettings.SelectedLocale.Identifier.Code;
        return currentLang == "ar" || currentLang == "he";
    }
}
```

---

## 6. Локализация ассетов (изображения, звуки, видео)
Не только текст, но и графика, звуки могут зависеть от языка.

### 📁 Структура локализованных ассетов:
```text
Assets/
├── Localization/
│   ├── Assets/
│   │   ├── Images/
│   │   │   ├── en/
│   │   │   │   └── banner.png
│   │   │   ├── ru/
│   │   │   │   └── banner.png
│   │   │   └── de/
│   │   │       └── banner.png
│   │   └── Audio/
│   │       ├── en/
│   │       │   └── voiceover.wav
│   │       └── ru/
│   │           └── voiceover.wav
```

### 🎨 Локализация ассетов через Asset Table:
1. Project Window → Create → Localization → Asset Table Collection
2. Добавить локали
3. Добавить записи:

| Key | en-US | ru-RU | de-DE |
| --- | --- | --- | --- |
| `banner` | banner_en.png | banner_ru.png | banner_de.png |
| `voiceover` | voice_en.wav | voice_ru.wav | voice_de.wav |

### 💻 Использование в коде:
```csharp
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizedAssetLoader : MonoBehaviour
{
    [SerializeField] private Image _bannerImage;
    [SerializeField] private AudioSource _voiceSource;
    
    public void LoadLocalizedImage(string assetKey)
    {
        var op = LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<Sprite>("GameAssets", assetKey);
        op.Completed += (result) =>
        {
            _bannerImage.sprite = result.Result;
        };
    }
    
    public void PlayLocalizedVoice(string assetKey)
    {
        var op = LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<AudioClip>("GameAssets", assetKey);
        op.Completed += (result) =>
        {
            _voiceSource.clip = result.Result;
            _voiceSource.Play();
        };
    }
}
```

---

## 7. Локализация дат, времени, чисел и валют
Форматы дат, чисел и валют сильно отличаются в разных странах.

### 📅 Пример: Локализация даты
```csharp
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization;

public class DateFormatter : MonoBehaviour
{
    public string FormatDate(DateTime date)
    {
        var culture = GetCurrentCulture();
        return date.ToString("D", culture); // "D" — длинный формат
    }
    
    public string FormatDateShort(DateTime date)
    {
        var culture = GetCurrentCulture();
        return date.ToString("d", culture); // "d" — короткий формат
    }
    
    private CultureInfo GetCurrentCulture()
    {
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        
        switch (localeCode)
        {
            case "en-US": return new CultureInfo("en-US");
            case "ru-RU": return new CultureInfo("ru-RU");
            case "de-DE": return new CultureInfo("de-DE");
            case "ja-JP": return new CultureInfo("ja-JP");
            default: return new CultureInfo("en-US");
        }
    }
    
    void Start()
    {
        DateTime now = DateTime.Now;
        Debug.Log($"Текущая дата (локальная): {FormatDate(now)}");
        // en-US: Thursday, June 27, 2026
        // ru-RU: четверг, 27 июня 2026 г.
        // de-DE: Donnerstag, 27. Juni 2026
    }
}
```

### 💰 Локализация валют:
```csharp
public class CurrencyFormatter : MonoBehaviour
{
    public string FormatCurrency(float amount)
    {
        var culture = GetCurrentCulture();
        return amount.ToString("C2", culture);
    }
    
    void Start()
    {
        float price = 19.99f;
        Debug.Log($"Цена (локальная): {FormatCurrency(price)}");
        // en-US: $19.99
        // ru-RU: 19,99 ₽
        // de-DE: 19,99 €
        // ja-JP: ¥20
    }
}
```

---

## 8. Полный пример: Система локализации в игре
```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using TMPro;

[System.Serializable]
public class LanguageOption
{
    public string Code;
    public string Name;
    public TMP_FontAsset Font;
}

public class CompleteLocalizationSystem : MonoBehaviour
{
    public static CompleteLocalizationSystem Instance { get; private set; }
    
    [Header("Languages")]
    public List<LanguageOption> supportedLanguages = new List<LanguageOption>();
    
    [Header("UI Components")]
    public TextMeshProUGUI[] localizedTexts;
    public TextMeshProUGUI[] textWithArgs;
    
    private Dictionary<string, TMP_FontAsset> _fontMap = new Dictionary<string, TMP_FontAsset>();
    private string _currentLanguage = "en";
    
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Строим карту шрифтов
        foreach (var lang in supportedLanguages)
        {
            _fontMap[lang.Code] = lang.Font;
        }
    }
    
    void Start()
    {
        _currentLanguage = PlayerPrefs.GetString("GameLanguage", "en");
        SetLanguage(_currentLanguage);
    }
    
    public void SetLanguage(string languageCode)
    {
        _currentLanguage = languageCode;
        
        // 1. Смена локали
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code == languageCode)
            {
                LocalizationSettings.SelectedLocale = locale;
                break;
            }
        }
        
        // 2. Смена шрифта
        if (_fontMap.TryGetValue(languageCode, out TMP_FontAsset font))
        {
            ApplyFontToAllUI(font);
        }
        
        // 3. Обновление текстов с аргументами
        UpdateTextsWithArgs();
        
        // 4. Сохраняем выбор
        PlayerPrefs.SetString("GameLanguage", languageCode);
        PlayerPrefs.Save();
        
        Debug.Log($"Язык изменён на: {languageCode}");
    }
    
    private void ApplyFontToAllUI(TMP_FontAsset font)
    {
        foreach (var text in localizedTexts)
        {
            text.font = font;
        }
        foreach (var text in textWithArgs)
        {
            text.font = font;
        }
    }
    
    private void UpdateTextsWithArgs()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        int health = 100;
        string healthText = LocalizationSettings.StringDatabase.GetLocalizedString("GameTexts", "msg_health");
        string formatted = string.Format(healthText, health);
        
        foreach (var text in textWithArgs)
        {
            text.text = string.Format(healthText, health);
        }
    }
    
    // Шаблон для локализованных сообщений с аргументами
    public string GetLocalizedString(string table, string key, params object[] args)
    {
        string template = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
        return string.Format(template, args);
    }
    
    // Применение ко всем текстам на сцене
    public void RefreshAllTexts()
    {
        foreach (var text in FindObjectsOfType<TextMeshProUGUI>())
        {
            // Проверяем, есть ли у текста компонент LocalizeStringEvent
            var localizeEvent = text.GetComponent<LocalizeStringEvent>();
            if (localizeEvent != null)
            {
                localizeEvent.RefreshString();
            }
        }
    }
    
    // Пример использования:
    public void ShowPlayerStats(string playerName, int score)
    {
        string msg = GetLocalizedString("GameTexts", "msg_stats", playerName, score);
        Debug.Log(msg);
        // en: "Player 'John' scored 150 points!"
        // ru: "Игрок 'Джон' набрал 150 очков!"
        // de: "Spieler 'John' hat 150 Punkte erreicht!"
    }
}
```

---

## 9. Лучшие практики
### ✅ Рекомендации:
1. Используйте ключи вместо текста — никогда не хардкодьте текст
2. Разделяйте строки по категориям (UI, Dialogues, Items)
3. Тестируйте все языки — даже если не понимаете текст
4. Проверяйте длину текста — немецкий может быть на 30% длиннее
5. Используйте Fallback-язык — если перевод отсутствует
6. Держите шрифты в отдельных папках — для каждого языка

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Хардкод текста
Text.text = "Start Game";  // Не локализуется!

// ✅ ПРАВИЛЬНО: Использовать ключ
Text.text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "ui_start");

// ❌ ОШИБКА: Конкатенация строк
Text.text = "Health: " + health;  // Порядок слов может отличаться

// ✅ ПРАВИЛЬНО: Использовать плейсхолдеры
Text.text = string.Format(GetLocalizedString("UI", "msg_health"), health);

// ❌ ОШИБКА: Не учитывать RTL
// Арабский текст может отображаться задом наперёд
// ✅ ПРАВИЛЬНО: Использовать RTL-плагин

// ❌ ОШИБКА: Смена языка через перезагрузку сцены
// ✅ ПРАВИЛЬНО: Смена языка в рантайме без перезагрузки
```

---

## 10. Альтернативные решения
Помимо официального Localization Package, существуют и другие подходы:

| Решение | Описание | Плюсы | Минусы |
| --- | --- | --- | --- |
| Unity Localization Package | Официальное решение | Мощное, поддерживается Unity | Сложное, требует изучения |
| I2 Localization | Платный ассет на Asset Store | Очень мощный, множество функций | Платный (~$50) |
| Самописная система | CSV/JSON + Dictionary | Полный контроль, лёгкий вес | Требует написания кода |
| Google Sheets | Таблица как источник перевода | Удобно для переводчиков | Требует интеграции |

### 📄 Пример самописной локализации через JSON:
```csharp
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalizationData
{
    public string Key;
    public string English;
    public string Russian;
    public string German;
}

public class SimpleLocalization : MonoBehaviour
{
    public TextAsset localizationJson;
    private Dictionary<string, Dictionary<string, string>> _translations;
    
    void Start()
    {
        LoadTranslations();
        string translated = GetTranslation("ui_start", "ru");
        Debug.Log(translated);
    }
    
    void LoadTranslations()
    {
        var data = JsonUtility.FromJson<LocalizationData[]>(localizationJson.text);
        _translations = new Dictionary<string, Dictionary<string, string>>();
        
        foreach (var entry in data)
        {
            _translations[entry.Key] = new Dictionary<string, string>
            {
                ["en"] = entry.English,
                ["ru"] = entry.Russian,
                ["de"] = entry.German
            };
        }
    }
    
    public string GetTranslation(string key, string language)
    {
        if (_translations.TryGetValue(key, out var langs))
        {
            if (langs.TryGetValue(language, out string value))
                return value;
        }
        return $"[{key}]";
    }
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
