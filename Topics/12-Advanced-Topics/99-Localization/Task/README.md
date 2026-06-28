# 🎯 Task: «Localizing a Game for 4 Languages»
You are developing the game "Word Wizard" — an educational puzzle game for children. 
The game must support 4 languages: English (en), Russian (ru), German (de), Spanish (es).

## 📝 Part 1: Setting Up Localization
1. Install Unity Localization Package
2. Create a String Table Collection named `UI_Texts`
3. Add all 4 languages
4. Add the following entries:

| Key | en | ru | de | es |
| --- | --- | --- | --- | --- |
| `title` | Word Wizard | Словесный Маг | Wortzauberer | Mago de Palabras |
| `play` | Play | Играть | Spielen | Jugar |
| `settings` | Settings | Настройки | Einstellungen | Ajustes |
| `score` | Score: {0} | Счёт: {0} | Punktestand: {0} | Puntuación: {0} |
| `time` | Time: {0}s | Время: {0}с | Zeit: {0}s | Tiempo: {0}s |
| `hint` | Hint | Подсказка | Hinweis | Pista |
| `victory` | Victory! | Победа! | Sieg! | ¡Victoria! |
| `defeat` | Try again | Попробуй снова | Versuch es nochmal | Inténtalo de nuevo |

## 📝 Part 2: Text and Font Management
5. Choose fonts for each language:
   - English / Spanish: `Roboto` or `Arial`
   - Russian: `Roboto` with Cyrillic support
   - German: `Roboto` (standard, with umlauts)
  
6. Create Font Assets for TextMeshPro for each font
7. Configure Fallback Fonts for special characters
8. Write `FontManager.cs` script that:
   - Listens for language changes
   - Automatically changes fonts on all text elements
   - Supports different sizes for different languages

## 📝 Part 3: UI Adaptation
9. Create language settings menu:
    - 4 buttons with flags (🇺🇸, 🇷🇺, 🇩🇪, 🇪🇸)
    - On click → change language without reloading
  
10. Implement auto-scaling text:
    - German words are often longer than English
    - Use `Content Size Fitter` or custom scaling
   
11. Adapt UI layout for different languages:
    - Buttons should expand for longer text
    - Use `Horizontal Layout Group` with `Control Child Size`
   
## 📝 Part 4: Asset Localization
12. Create an Asset Table Collection named `Images`
13. Add localized images:
    - `banner_en.png`, `banner_ru.png`, `banner_de.png`, `banner_es.png`
    - Each image contains the game name in its language
   
14. Create `LocalizedBanner.cs` script that:
    - Loads the correct banner based on language
    - Updates when language changes
   
## 📝 Part 5: Date and Number Formats
15. Implement localized timer:
    - Display format: `Time: 45s` (en), `Время: 45с` (ru)
   
16. Implement localized score:
    - `Score: 1,234` (en), `Счёт: 1 234` (ru), `Punktestand: 1.234` (de)
   
## 📝 Part 6: Localization Testing
17. Create a `LocalizationTest` scene with all UI elements
18. Add a language switcher in the top corner
19. Verify:
    - All texts display correctly
    - Fonts change properly
    - Text fits in UI elements
    - Time and score format correctly
   
---

## 🧰 Implementation Requirements:
- Use Unity Localization Package
- All texts must be in tables (no hardcoding)
- Language switching must work at runtime
- Fonts must support all characters of the language
- Code must be commented

---

## 🔍 Verification:
1. Run the scene in each language
2. Verify all buttons, labels, and dialogues are translated
3. Verify long German words don't overflow boundaries
4. Verify score and time format according to language rules
5. Verify banner updates on language change

---

## 💡 Expected Result:
```text
=== Localization ============
✅ 4 languages supported
✅ 9 keys translated
✅ 3 fonts configured
✅ 3 UI elements adapted
✅ Assets localized

=== Tests ==================
🏁 English: all texts correct
🏁 Russian: Cyrillic displays correctly
🏁 German: umlauts display, text fits
🏁 Spanish: special characters display

=== Result =================
✅ Localization works correctly
✅ UI adapts to different languages
✅ Fonts support all characters
✅ Ready for release!
```

---

## 🏆 Bonus Task (Optional):
Implement automatic system language detection:
- On first launch, detect OS language
- Automatically set the corresponding language in the game
- If language is not supported — use English
```csharp
public class AutoLanguageDetector : MonoBehaviour
{
    void Start()
    {
        string systemLanguage = Application.systemLanguage.ToString();
        string mappedLanguage = MapSystemLanguage(systemLanguage);
        LanguageManager.Instance.SetLanguage(mappedLanguage);
    }
    
    private string MapSystemLanguage(string systemLang)
    {
        return systemLang switch
        {
            "English" => "en",
            "Russian" => "ru",
            "German" => "de",
            "Spanish" => "es",
            _ => "en"
        };
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
