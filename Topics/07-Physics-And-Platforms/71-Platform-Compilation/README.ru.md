# 🔧 Платформенная компиляция в Unity: Директивы препроцессора (UNITY_ANDROID, UNITY_IOS, UNITY_EDITOR)

Unity позволяет создавать кроссплатформенные игры, 
но иногда требуется выполнять разный код в зависимости от целевой платформы. 
Для этого используются директивы препроцессора (условная компиляция). 
Они указывают компилятору Unity, какой код включать или исключать при сборке под конкретную платформу.

---

## 1. Что такое директивы препроцессора?
Директивы препроцессора — это инструкции, которые обрабатываются до начала компиляции.

### Они позволяют:
- Исключать код, несовместимый с платформой
- Использовать платформо-зависимые API (например, `Application.OpenURL` ведёт себя по-разному)
- Оптимизировать производительность под конкретную платформу
- Включать отладочный код только в редакторе

### Синтаксис:
```csharp
#if СИМВОЛ
    // Код компилируется, если СИМВОЛ определён
#elif ДРУГОЙ_СИМВОЛ
    // Код компилируется для другого символа
#else
    // Код компилируется, если ни один символ не подошёл
#endif
```

> [!Important]
> Директивы работают на этапе компиляции. Код внутри неверной директивы даже не попадает в финальную сборку.

---

## 2. Основные платформенные символы
### 📱 Мобильные платформы
| Символ | Платформа | Когда определён |
| --- | --- | --- |
| `UNITY_ANDROID` | Android | При сборке под Android |
| `UNITY_IOS` | iOS | При сборке под iOS |
| `UNITY_TVOS` | tvOS | При сборке под tvOS |

### 💻 Десктопные платформы
| Символ | Платформа | Когда определён |
| --- | --- | --- |
| `UNITY_STANDALONE_WIN` | Windows | Сборка под Windows |
| `UNITY_STANDALONE_OSX` | macOS | Сборка под macOS | 
| `UNITY_STANDALONE_LINUX` | Linux | Сборка под Linux |
| `UNITY_STANDALONE` | Любой десктоп | Любая десктопная сборка |

### 🎮 Игровые консоли
| Символ | Платформа |
| --- | --- |
| `UNITY_XBOXONE` | Xbox One |
| `UNITY_PS4` | PlayStation 4 |
| `UNITY_PS5` | PlayStation 5 |
| `UNITY_SWITCH` | Nintendo Switch |

### 🛠️ Редактор и разработка
| Символ | Описание |
| --- | --- |
| `UNITY_EDITOR` | Код выполняется внутри Unity Editor |
| `UNITY_EDITOR_WIN` | Редактор под Windows |
| `UNITY_EDITOR_OSX` | Редактор под macOS |
| `DEVELOPMENT_BUILD` | Сборка с флагом Development Build |
| `UNITY_DEBUG` | Определён в Debug-сборках |

### 🏷️ Версии Unity
```csharp
#if UNITY_2020_1_OR_NEWER
    // Код для Unity 2020.1 и новее
#endif

#if UNITY_2019_3_OR_NEWER && !UNITY_2020_1_OR_NEWER
    // Код только для версий между 2019.3 и 2020.0
#endif
```

---

## 3. Примеры использования каждой директивы
### 📱 Пример 1: UNITY_ANDROID — сохранение в галерею
```csharp
using UnityEngine;

public class PlatformScreenshot : MonoBehaviour
{
    public void TakeScreenshot()
    {
        string filename = "screenshot.png";
        
        ScreenCapture.CaptureScreenshot(filename);
        
#if UNITY_ANDROID
        // На Android сохраняем в галерею через Native API
        AndroidJavaClass mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection");
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        
        string path = Application.persistentDataPath + "/" + filename;
        mediaScanner.CallStatic("scanFile", currentActivity, new string[] { path }, null, null);
        
        Debug.Log("Скриншот сохранён в галерею Android");
#elif UNITY_IOS
        // iOS требует специального разрешения
        Debug.Log("Скриншот сохранён в папку приложения");
        // Здесь можно вызвать Native iOS метод для сохранения в фотоальбом
#else
        Debug.Log("Скриншот сохранён: " + Application.persistentDataPath + "/" + filename);
#endif
    }
}
```

### 🍎 Пример 2: UNITY_IOS — обработка push-уведомлений
```csharp
public class IOSNotificationHandler : MonoBehaviour
{
    void Start()
    {
#if UNITY_IOS
        // Импортируем пространство имён iOS только для сборки под iOS
        using UnityEngine.iOS;
        
        // Регистрация на уведомления
        NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
        
        // Проверка запуска из уведомления
        var notification = NotificationServices.GetLastNotification();
        if (notification != null)
        {
            Debug.Log("Запущено из уведомления: " + notification.alertBody);
            HandleNotification(notification);
        }
#endif
    }
    
#if UNITY_IOS
    private void HandleNotification(NotificationEventArgs args)
    {
        // Логика обработки уведомления
    }
#endif
}
```

### 🖥️ Пример 3: UNITY_EDITOR — отладка без устройства
```csharp
using UnityEngine;

public class PlatformDebug : MonoBehaviour
{
    private void Update()
    {
#if UNITY_EDITOR
        // Эмуляция касания мышкой в редакторе
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Debug.Log($"[Editor] Клик мышкой по координатам: {mousePos}");
            HandleTap(mousePos);
        }
#elif UNITY_ANDROID || UNITY_IOS
        // Реальное касание на устройстве
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log($"[Device] Касание по координатам: {touch.position}");
                HandleTap(touch.position);
            }
        }
#endif
    }
    
    private void HandleTap(Vector2 screenPos)
    {
        // Общая логика обработки касания
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Попали в объект: " + hit.collider.name);
        }
    }
}
```

### 💾 Пример 4: Разные пути сохранения для разных платформ
```csharp
public class PlatformPaths : MonoBehaviour
{
    public static string GetSavePath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Saves/";
#elif UNITY_ANDROID
        return Application.persistentDataPath + "/Saves/";
#elif UNITY_IOS
        return Application.persistentDataPath + "/Documents/Saves/";
#elif UNITY_STANDALONE_WIN
        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/MyGame/Saves/";
#elif UNITY_STANDALONE_OSX
        return Application.persistentDataPath + "/Saves/";
#else
        return Application.persistentDataPath + "/Saves/";
#endif
    }
    
    void Awake()
    {
        string path = GetSavePath();
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
            Debug.Log($"Создана директория сохранений: {path}");
        }
    }
}
```

### 🎮 Пример 5: Отключение кнопки "Назад" на Android
```csharp
public class AndroidBackButtonHandler : MonoBehaviour
{
    void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Только на реальном Android устройстве
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Показываем диалог подтверждения выхода
            ShowExitDialog();
        }
#elif UNITY_EDITOR
        // В редакторе используем кнопку Escape для эмуляции
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[Editor] Эмуляция кнопки Назад на Android");
            ShowExitDialog();
        }
#endif
    }
    
    private void ShowExitDialog()
    {
        // Показываем UI-диалог
        Debug.Log("Показ диалога выхода из игры");
        // Здесь код для показа окна подтверждения
    }
}
```

### 🛡️ Пример 6: Комбинация нескольких директив
```csharp
public class AdvancedPlatformHandler : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR
        Debug.Log("=== ЗАПУСК В РЕДАКТОРЕ ===");
        // Включаем дополнительные отладочные инструменты
        EnableDebugTools();
#elif DEVELOPMENT_BUILD
        Debug.Log("=== DEVELOPMENT BUILD ===");
        // Включить логирование ошибок
        EnableErrorLogging();
#else
        Debug.Log("=== ФИНАЛЬНАЯ СБОРКА ===");
        // Отключаем все отладочные функции
        DisableDebugTools();
#endif

#if UNITY_ANDROID && (DEVELOPMENT_BUILD || UNITY_EDITOR)
        // Только Android разработка
        Debug.Log("Включена Android-отладка через ADB");
        EnableADBLogging();
#endif

#if UNITY_IOS && !UNITY_EDITOR
        // Только iOS без редактора (реальное устройство)
        Debug.Log("Запущено на реальном iOS устройстве");
        RequestiOSPermissions();
#endif
    }
    
    private void EnableDebugTools() { /* ... */ }
    private void EnableErrorLogging() { /* ... */ }
    private void DisableDebugTools() { /* ... */ }
    private void EnableADBLogging() { /* ... */ }
    private void RequestiOSPermissions() { /* ... */ }
}
```

---

## 4. Платформенные ассеты (Platform Tiling)
Иногда одного кода недостаточно — нужны разные ресурсы (шрифты, звуки, контроллеры ввода).

### Для этого Unity предоставляет механизм Platform Tiling:
```text
Assets/
├── Textures/
│   ├── Icon_Android.png (Platform: Android)
│   └── Icon_iOS.png (Platform: iOS)
├── Plugins/
│   ├── Android/
│   │   └── libNative.so
│   └── iOS/
│       └── NativePlugin.mm
```

### Настройка в инспекторе:
1. Выберите ассет
2. В инспекторе найдите раздел Platform Settings
3. Установите галочку для нужной платформы

---

## 5. Пользовательские символы компиляции
Вы можете создавать свои символы в Player Settings → Scripting Define Symbols.
```csharp
// Файл ProjectSettings/ProjectSettings.asset или через UI
// Добавить: USE_ANALYTICS;BETA_BUILD

#if USE_ANALYTICS
    Analytics.SendEvent("game_start");
#endif

#if BETA_BUILD
    Debug.Log("ВЕРСИЯ: Бета-билд");
    EnableBetaFeatures();
#endif
```

### Добавление через код:
```csharp
using UnityEditor;
using UnityEditor.Build;

public class DefineSymbolsHelper
{
    [MenuItem("Tools/Add Beta Symbol")]
    public static void AddBetaSymbol()
    {
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        if (!symbols.Contains("BETA"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbols + ";BETA");
        }
    }
}
```

---

## 6. Лучшие практики и частые ошибки
### ✅ Рекомендации:
1. Не дублируйте код — выносите платформо-зависимый код в отдельные методы
2. Используйте `#if UNITY_EDITOR` для отладки — не засоряйте релизный билд
3. Тестируйте каждую платформу отдельно — то, что работает в редакторе, может не работать на устройстве
4. Документируйте платформенные блоки — комментарии помогут другим разработчикам
5. Для сложной логики используйте паттерн "Адаптер" вместо кучи директив

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Забыли закрыть директиву
#if UNITY_ANDROID
    Debug.Log("Android");
    
// ❌ ОШИБКА: Использование неопределённого символа
#if UNITY_SAMSUNG  // Такого символа не существует!

// ❌ ОШИБКА: Смешивание с runtime-проверками
if (Application.platform == RuntimePlatform.Android)  // Runtime, не компиляция!
{
    // Этот код попадёт в билд под все платформы
}
```

### ✅ Правильный подход:
```csharp
// ✓ Правильно: всегда закрывайте
#if UNITY_ANDROID
    Debug.Log("Android");
#endif

// ✓ Правильно: используйте только задокументированные символы
#if UNITY_ANDROID && !UNITY_EDITOR
    Debug.Log("Real Android device");
#endif

// ✓ Используйте runtime-проверки только когда условная компиляция невозможна
#if UNITY_ANDROID
    // Платформо-зависимый код
#else
    // Fallback
    Debug.Log(Application.platform.ToString());
#endif
```

---

## 7. Таблица соответствия символов и платформ

| Платформа | Основной символ | Дополнительные символы |
| --- | --- | --- |
| Android | `UNITY_ANDROID` | `UNITY_ANDROID`, `UNITY_ANDROID_API_LEVEL` |
| iOS | `UNITY_IOS` | `UNITY_IOS`, `UNITY_IPHONE` (устаревший) |
| Windows Standalone | `UNITY_STANDALONE_WIN` | `UNITY_STANDALONE`, `UNITY_WIN` |
| macOS Standalone | `UNITY_STANDALONE_OSX` | `UNITY_STANDALONE` |
| Linux Standalone | `UNITY_STANDALONE_LINUX` | `UNITY_STANDALONE` |
| WebGL | `UNITY_WEBGL` | - |
| Unity Editor | `UNITY_EDITOR` | `UNITY_EDITOR_WIN`, `UNITY_EDITOR_OSX` |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
