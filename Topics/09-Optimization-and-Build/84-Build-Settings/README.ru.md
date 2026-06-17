# 📦 Сборка проекта в Unity: Player Settings, настройка иконок, сплеш-скринов, разрешений
Сборка (Build) — это процесс преобразования вашего проекта в исполняемое приложение для целевой платформы. 
Правильная настройка Player Settings критически важна для внешнего вида, функциональности и соответствия требованиям магазинов приложений.

---

## 1. Player Settings — главный центр управления сборкой
Player Settings — это окно, где настраиваются все параметры финального приложения: от названия до разрешений.

### 🗂️ Доступ к Player Settings:
```text
Edit → Project Settings → Player
```
Или через Build Settings → кнопка Player Settings.

### 📋 Основные разделы Player Settings:
| Раздел | Описание |
| --- | --- |
| Company Name | Название компании (отображается в системе) |
| Product Name | Название приложения (отображается пользователю) |
| Version | Версия приложения (например, 1.0.0) |
| Default Icon | Иконка приложения |
| Splash Screen | Заставка при запуске |
| Resolution | Разрешение экрана, полноэкранный режим |
| Run In Background | Работает ли игра в фоне |
| Scripting Backend | IL2CPP или Mono |
| API Compatibility | .NET Standard 2.0 / .NET 4.x |
| Android / iOS Settings | Платформо-специфичные настройки |

---

## 2. Настройка иконок (Icons)
Иконка — это лицо вашего приложения в магазине и на рабочем столе. 
Unity позволяет загружать иконки разных размеров для разных платформ.

### 🖼️ Настройка иконок для Android:
Путь: Player Settings → Android → Icon
| Размер (px) | Назначение |
| --- | --- |
| 512×512 | Иконка в Google Play |
| 192×192 | Высокое разрешение (адаптивная иконка) |
| 144×144 | XXHDPI |
| 96×96 | XHDPI |
| 72×72 | HDPI |
| 48×48 | MDPI |

### Адаптивные иконки (Android 8+):
В Player Settings → Android → Icon → Adaptive Icon

Установите два слоя:
1. Foreground — изображение (маска обязательна)
2. 2. Background — цвет или изображение

### 🍎 Настройка иконок для iOS:
Путь: Player Settings → iOS → Icon

| Размер (px) | Назначение |
| --- | --- |
| 180×180 | iPhone (60pt @3x) |
| 120×120 | iPhone (60pt @2x) |
| 152×152 | iPad (76pt @2x) |
| 76×76 | iPad (76pt @1x) |
| 167×167 | iPad Pro (83.5pt @2x) |
| 1024×1024 | App Store |

### 📝 Пример автоматической установки иконок через скрипт:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class IconSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup App Icons")]
    public static void SetupIcons()
    {
        Texture2D[] icons = new Texture2D[]
        {
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Icons/icon_512.png"),
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Icons/icon_192.png"),
            // ... все размеры
        };
        
        // Получаем текущие настройки
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, icons);
        
        Debug.Log("Icons applied successfully!");
    }
}
#endif
```

> [!Tip]
> Используйте генератор иконок (например, Adaptive Icon Generator в Unity 2022+) для автоматического создания всех размеров из одного изображения.

---

## 3. Настройка сплеш-скрина (Splash Screen)
Splash Screen — экран-заставка, который показывается во время загрузки приложения.

### 🎨 Настройка сплеш-скрина:
Путь: Player Settings → Splash Screen

| Параметр | Описание |
| --- | --- |
| Show Splash Screen | Включить/выключить заставку |
| Splash Style | Dark / Light (фон) |
| Animation | Анимация логотипа (Center / Scaling / Custom) |
| Logo | Изображение логотипа (PNG с прозрачностью) |
| Background | Цвет фона или градиент |
| Draw Mode | Unity Logo / All Sequential / All Simultaneous |

### 📱 Пример настройки кастомного сплеш-скрина:
В Player Settings → Splash Screen:
1. Установите Splash Style: Dark
2. 2. Загрузите свой логотип (логотип компании)
3. Установите Background Color: #1a1a2e
4. Отключите "Show Unity Logo" (если у вас Pro лицензия)

### ⏱️ Программное управление сплеш-скрином:
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SplashScreenController : MonoBehaviour
{
    void Start()
    {
        // Если вы используете кастомный сплеш-скрин (не Unity-стандартный)
        StartCoroutine(ShowCustomSplash());
    }
    
    IEnumerator ShowCustomSplash()
    {
        // Показываем свой UI-сплеш (Canvas)
        GameObject splashUI = GameObject.Find("SplashScreenCanvas");
        splashUI.SetActive(true);
        
        // Ждём 3 секунды
        yield return new WaitForSeconds(3f);
        
        // Скрываем сплеш
        splashUI.SetActive(false);
        
        // Загружаем основную сцену
        SceneManager.LoadScene("MainMenu");
    }
}
```

### 🏪 Сплеш-скрин для магазинов:
Google Play:
- Сплеш должен быть одиночным (не анимированным)
- Минимальное время показа — 1.5 секунды
- Не используйте звук

App Store:
- Требуется Launch Screen (системный, не через Unity)
- Используйте Unity Splash Screen только для логотипа

---

## 4. Настройка разрешений (Permissions)
Разрешения — это доступ приложения к функциям устройства (камера, микрофон, GPS и т.д.). 
Для Android и iOS это обязательная часть настройки.

### 🤖 Разрешения для Android:
Путь: Player Settings → Android → Publishing Settings → Permissions

Основные разрешения:
| Разрешение | Ключ | Использование |
| --- | --- | --- |
| Internet | `INTERNET` | Доступ в интернет (обязательно) |
| Camera | `CAMERA` | Использование камеры |
| Microphone | `RECORD_AUDIO` | Запись звука |
| GPS | `ACCESS_FINE_LOCATION` | GPS-координаты |
| Storage | `READ_EXTERNAL_STORAGE` / `WRITE_EXTERNAL_STORAGE` | Доступ к файлам |
| Vibrate | `VIBRATE` | Вибрация |

Добавление кастомного разрешения в манифест:
```xml
<!-- Assets/Plugins/Android/AndroidManifest.xml -->
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <uses-permission android:name="android.permission.VIBRATE" />
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.CAMERA" />
    
    <!-- Для Android 13+ (API 33) -->
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
    
    <application android:icon="@drawable/app_icon">
        <!-- ... -->
    </application>
</manifest>
```

### 🍎 Разрешения для iOS:
Путь: Player Settings → iOS → Other Settings

Настройка разрешений в Info.plist:
```xml
<!-- Xcode → Info.plist или через Unity Player Settings -->
<key>NSCameraUsageDescription</key>
<string>Используем камеру для сканирования QR-кодов</string>

<key>NSMicrophoneUsageDescription</key>
<string>Запись голоса для чата в игре</string>

<key>NSLocationWhenInUseUsageDescription</key>
<string>Для поиска игроков рядом с вами</string>

<key>NSPhotoLibraryUsageDescription</key>
<string>Сохранение скриншотов в галерею</string>
```

Пример запроса разрешений в коде:
```csharp
using UnityEngine;
using UnityEngine.Android;

public class PermissionManager : MonoBehaviour
{
    void Start()
    {
        CheckAndRequestPermissions();
    }
    
    public void CheckAndRequestPermissions()
    {
        #if UNITY_ANDROID
        // Проверяем разрешение на камеру
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        
        // Проверяем разрешение на микрофон
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        
        // Для Android 13+ запрос на уведомления
        if (SystemInfo.operatingSystem.Contains("Android 13"))
        {
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
        }
        #endif
        
        #if UNITY_IOS
        // iOS разрешения запрашиваются через плагины или Native API
        // Например, через Unity iOS Support
        #endif
    }
    
    // Проверка статуса разрешений
    public bool IsPermissionGranted(string permission)
    {
        #if UNITY_ANDROID
        return Permission.HasUserAuthorizedPermission(permission);
        #elif UNITY_IOS
        // iOS логика
        return true;
        #else
        return true;
        #endif
    }
}
```

---

## 5. Дополнительные настройки сборки
### 🔧 Scripting Backend (Бэкенд скриптов)
| Backend | Описание | Когда использовать |
| --- | --- | --- |
| Mono | Более быстрая компиляция, больше размер | Разработка, тестирование |
| IL2CPP | Оптимизированный код, лучше производительность | Релизные сборки |

Настройка: Player Settings → Scripting Backend

### ⚡ Managed Stripping Level (Уровень обрезки)
| Уровень | Описание |
| --- | --- |
| Disabled | Без обрезки (максимальный размер) |
| Low | Минимальная обрезка |
| Medium | Средняя обрезка (рекомендуется) |
| High | Агрессивная обрезка (экономия размера, риск ошибок) |

### 🎮 Other Settings (Прочее)
| Параметр | Описание |
| --- | --- |
| Run In Background | Продолжать выполнение в фоне |
| Fullscreen Mode | Полноэкранный режим / Windowed |
| Default Screen Width/Height | Разрешение окна |
| Color Space | Gamma / Linear (для графики) |
| Graphics API | DirectX, Vulkan, OpenGL, Metal |

---

## 6. Процесс сборки (Build Process)
📦 Стандартный процесс:
1. File → Build Settings
2. Выбрать платформу (Android / iOS / Standalone)
3. Нажать Player Settings для дополнительной настройки
4. Нажать Build и выбрать папку
5. Дождаться завершения сборки

### 🤖 Особенности сборки под Android:
Необходимые настройки:
| Параметр | Значение |
| --- | --- |
| Build System | Gradle (рекомендуется) |
| Minimum API Level | Android 6.0 (API 23) или выше |
| Target API Level | Последняя доступная (API 33+) |
| Key Store | Подпись APK (обязательно для Google Play) |
| IL2CPP Code Generation | Faster (smaller) / Faster (runtime) |

Генерация ключа (Keystore):
1. Player Settings → Android → Publishing Settings
2. Keystore Manager → Create New
3. Заполнить поля:
   - Keystore name: `mygame.keystore`
   - Password: `******`
   - Alias: `mygame_alias`
   - Validity (years): 25
  
### 🍎 Особенности сборки под iOS:
| Параметр | Значение |
| --- | --- |
| Target SDK | Device SDK (для релиза) |
| Target Device | iPhone + iPad |
| Bundle Identifier | com.company.appname (уникальный) |
| Automatic Signing | Включить (для разработки) |
| Scripting Backend | IL2CPP |

Процесс:
1. Unity генерирует Xcode проект
2. Открыть в Xcode
3. Настроить подписи (Signing)
4. Собрать .ipa файл

---

## 7. Пример: Полный скрипт автоматической сборки
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AutoBuilder : MonoBehaviour
{
    [MenuItem("Build/Build Android")]
    public static void BuildAndroid()
    {
        // Настройка Player Settings
        PlayerSettings.companyName = "MyGameStudio";
        PlayerSettings.productName = "DungeonQuest";
        PlayerSettings.bundleVersion = "1.0.0";
        
        // Android специфичные настройки
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.mygamestudio.dungeonquest");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_Standard_2_0);
        
        // Настройка Android Keystore
        PlayerSettings.Android.keystorePass = "password123";
        PlayerSettings.Android.keyaliasName = "game_alias";
        PlayerSettings.Android.keyaliasPass = "password123";
        
        // Сборка
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/MainScene.unity" },
            locationPathName = "Builds/DungeonQuest.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;
        
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build успешно завершён: {summary.totalSize} bytes");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError($"Build провален: {summary.totalErrors} errors");
        }
    }
    
    [MenuItem("Build/Build iOS")]
    public static void BuildIOS()
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.mygamestudio.dungeonquest");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/MainScene.unity" },
            locationPathName = "Builds/iOS",
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };
        
        BuildPipeline.BuildPlayer(buildOptions);
        Debug.Log("iOS Xcode проект создан в Builds/iOS");
    }
}
#endif
```

---

## 8. Лучшие практики
✅ Рекомендации:
1. Используйте Version Control для PlayerSettings — отслеживайте изменения
2. Сохраняйте Keystore в безопасном месте — не теряйте ключи для обновлений
3. Тестируйте сборки на реальных устройствах — эмулятор не даёт полной картины
4. Оптимизируйте размер сборки — используйте Stripping и сжатие текстур
5. Настраивайте разрешения по необходимости — не запрашивайте лишнего

❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Забыли настроить Bundle Identifier
// На Android: com.Company.ProductName
// На iOS: com.company.appname (должен быть уникальным)

// ❌ ОШИБКА: Слишком большой размер сборки
// Используйте Texture Compression (ASTC для Android, PVRTC для iOS)

// ❌ ОШИБКА: Игнорирование разрешений
// Если не запросить разрешение, приложение упадёт при попытке доступа

// ❌ ОШИБКА: Неправильный Spash Screen
// Для Google Play запрещена анимация и звук

// ❌ ОШИБКА: Использование Mono для релиза
// Всегда используйте IL2CPP для финальных сборок
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
