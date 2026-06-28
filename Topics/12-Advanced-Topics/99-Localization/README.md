# 🌍 Localization in Unity: Multilingual Support, UI Adaptation, Text and Font Management

Localization is the process of adapting a game for different languages and cultural specifics. 
Unity doesn't have a built-in localization system, but there is a powerful official solution — the Localization Package, as well as many alternative approaches. 
This guide covers all aspects of localization: from text handling to font adaptation for different languages.

---

## 1. Introduction: Why Localization Matters?
Localization is not just text translation. It's comprehensive adaptation including:

| Aspect | Description |
| --- | --- |
| Text translation | All UI elements, dialogues, tooltips |
| UI adaptation | Changing sizes, positions of elements |
| Fonts | Support for Cyrillic, Asian, RTL languages |
| Formats | Dates, time, currency, numbers |
| Cultural specifics | Colors, symbols, gestures |
| Audio | Voiceover in different languages |

> 💡 Statistics: Games with localization have 26% more revenue and 2x higher conversion to install.

---

## 2. Unity Localization Package — Official Solution
Unity Localization Package is Unity's official package providing a full-featured localization system.

### 📦 Installation:
```text
Window → Package Manager → Unity Registry → Localization → Install
```

### 🔧 Key Components:

| Component | Description |
| --- | --- |
| Locale | Represents a language and region (e.g., ru-RU, en-US) |
| String Table | String table (key → translation) |
| Asset Table | Asset table (key → asset) |
| Localized String | Component for text localization |
| Localized Asset | Component for asset localization |

### 📄 Example: Creating a String Table
1. Project Window → Create → Localization → String Table Collection
2. Name it `GameTexts`
3. Add locales: `English (en)`, `Russian (ru)`, `German (de)`
4. Add entries:

| Key | English | Russian | German |
| --- | --- | --- | --- |
| `ui_start` | Start | Начать | Starten |
| `ui_settings` | Settings | Настройки | Einstellungen |
| `ui_quit` | Quit | Выйти | Beenden |
| `msg_welcome` | Welcome to the game! | Добро пожаловать в игру! | Willkommen im Spiel! |
| `msg_health` | Health: {0} | Здоровье: {0} | Gesundheit: {0} |

### 💻 Usage in Code:
```csharp
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    public LocalizedString localizedString;
    
    void Start()
    {
        string key = "ui_start";
        string translated = LocalizationSettings.StringDatabase.GetLocalizedString("GameTexts", key);
        Debug.Log($"Translation of '{key}' in current language: {translated}");
        
        localizedString.TableReference = "GameTexts";
        localizedString.TableEntryReference = "msg_welcome";
        localizedString.StringChanged += OnStringChanged;
    }
    
    void OnStringChanged(string value)
    {
        Debug.Log($"Updated text: {value}");
        GetComponent<TextMeshProUGUI>().text = value;
    }
}
```

---

## 3. Runtime Language Switching
Switching language without restarting the game is a key localization feature.

### 🔄 Example: Language Switching System
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
                Debug.Log($"Language changed to: {locale.LocaleName}");
                return;
            }
        }
        
        Debug.LogWarning($"Language '{localeCode}' not found, using English");
    }
    
    public void ToggleLanguage()
    {
        string currentCode = _currentLocale.Identifier.Code;
        string nextCode = currentCode == "en" ? "ru" : "en";
        SetLanguage(nextCode);
    }
}
```

---

## 4. Text and Font Management
Different languages require different fonts. Unity uses TextMeshPro for high-quality text.

### 🅰️ Main Font Issues:

| Issue | Description | Solution |
| --- | --- | --- |
| Missing glyphs | Font doesn't contain needed characters | Use Fallback fonts |
| Asian languages | Thousands of characters (Chinese, Japanese) | Special fonts with support |
| RTL languages | Arabic, Hebrew (right-to-left text) | RTL plugins or special config |
| Text length | German is longer than English | Auto-scaling |

### 🖋️ Configuring TextMeshPro for Localization:
Step 1: Creating font with language support
1. In Font Asset Creator window, select main font
2. Add Fallback Font Assets for missing characters
3. For Cyrillic, add Character Set = `Unicode Range (Hex)`: `0400-04FF`
4. For Chinese/Japanese: `4E00-9FFF`, `3040-30FF`
```csharp
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
}
```

### 📏 Text Size Adaptation
Different languages have different word lengths. Example:

| English | German | Russian |
| --- | --- | --- |
| `Settings` (8) | `Einstellungen` (13) | `Настройки` (9) |
| `Play` (4) | `Spielen` (7) | `Играть` (6) |

Solution: Auto-scaling
```csharp
using TMPro;
using UnityEngine;

public class AutoFitText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _minFontSize = 14;
    [SerializeField] private float _maxFontSize = 36;
    [SerializeField] private float _preferredWidth = 200f;
    
    public void FitTextToWidth()
    {
        string originalText = _text.text;
        
        for (float size = _maxFontSize; size >= _minFontSize; size -= 1f)
        {
            _text.fontSize = size;
            _text.ForceMeshUpdate();
            
            if (_text.preferredWidth <= _preferredWidth)
            {
                break;
            }
        }
    }
}
```

---

## 5. RTL Language Support (Arabic, Hebrew)
RTL (Right-to-Left) languages require special handling.

### 🧩 Solution: Arabic Support for Unity
```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArabicSupport;

public class RTLTextFixer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private bool _useTashkeel = true;
    [SerializeField] private bool _useHinduNumbers = false;
    
    public void FixRTLText()
    {
        string original = _text.text;
        string fixedText = ArabicFixer.Fix(original, _useTashkeel, _useHinduNumbers);
        _text.text = fixedText;
        
        _text.alignment = TextAlignmentOptions.Right;
        _text.isRightToLeftText = true;
    }
}
```

---

## 6. Asset Localization (Images, Audio, Video)
Not just text, but graphics and audio can be language-dependent.

### 📁 Localized Asset Structure:
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

---

## 7. Date, Time, Number and Currency Localization
Date, number and currency formats vary greatly across countries.

### 📅 Example: Date Localization
```csharp
using System;
using System.Globalization;
using UnityEngine.Localization;

public class DateFormatter : MonoBehaviour
{
    public string FormatDate(DateTime date)
    {
        var culture = GetCurrentCulture();
        return date.ToString("D", culture);
    }
    
    private CultureInfo GetCurrentCulture()
    {
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        
        switch (localeCode)
        {
            case "en-US": return new CultureInfo("en-US");
            case "ru-RU": return new CultureInfo("ru-RU");
            case "de-DE": return new CultureInfo("de-DE");
            default: return new CultureInfo("en-US");
        }
    }
}
```

---

## 8. Complete Localization Example
```csharp
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
        
        foreach (var lang in supportedLanguages)
        {
            _fontMap[lang.Code] = lang.Font;
        }
    }
    
    public void SetLanguage(string languageCode)
    {
        _currentLanguage = languageCode;
        
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code == languageCode)
            {
                LocalizationSettings.SelectedLocale = locale;
                break;
            }
        }
        
        if (_fontMap.TryGetValue(languageCode, out TMP_FontAsset font))
        {
            ApplyFontToAllUI(font);
        }
        
        UpdateTextsWithArgs();
        PlayerPrefs.SetString("GameLanguage", languageCode);
        PlayerPrefs.Save();
    }
    
    private void ApplyFontToAllUI(TMP_FontAsset font)
    {
        foreach (var text in localizedTexts) text.font = font;
        foreach (var text in textWithArgs) text.font = font;
    }
    
    public string GetLocalizedString(string table, string key, params object[] args)
    {
        string template = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
        return string.Format(template, args);
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
