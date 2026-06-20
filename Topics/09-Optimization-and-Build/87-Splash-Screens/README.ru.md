# 🚀 Сплит-скрины и стартовые сцены в Unity: Splash Screen, настройка первой сцены, Build Pipeline
Процесс запуска игры — это первое впечатление пользователя. Правильная организация стартовых экранов, 
последовательности загрузки и автоматизация сборки критически важны для профессионального проекта. 
В этом руководстве мы разберём все аспекты: от сплеш-скрина до продвинутого Build Pipeline.

---

## 1. Splash Screen (Заставка) — первое, что видит пользователь
Splash Screen — это экран-заставка, который отображается сразу после запуска приложения, до загрузки первой сцены.
### 🎨 Типы сплеш-скринов в Unity:
| Тип | Описание | Когда использовать |
| --- | --- | --- |
| Unity Splash Screen | Встроенная заставка Unity с логотипом | Бесплатная версия (обязательно) |
| Custom Splash Screen | Собственный экран загрузки | Pro версия или кастомная реализация |
| System Launch Screen | Системная заставка (iOS/Android) | Для мобильных платформ |

### 🛠️ Настройка Unity Splash Screen:
Путь: Player Settings → Splash Screen

```text
// Основные параметры
Show Splash Screen: true/false
Splash Style: Dark / Light
Animation: Center / Scaling / Custom
Logo: Texture2D (логотип компании)
Background: Color или Gradient
Draw Mode: Unity Logo / All Sequential / All Simultaneous
```

### 📝 Пример: Продвинутая настройка сплеш-скрина
Player Settings → Splash Screen
1. Включаем Show Splash Screen
2. Устанавливаем Splash Style: Dark
3. Добавляем логотип компании (PNG с прозрачностью)
4. Устанавливаем фон: градиент #0f0c29 → #302b63 → #24243e
5. Отключаем "Show Unity Logo" (только для Pro лицензии)
6. Устанавливаем Animation: Center (плавное появление)

### ⏱️ Программное управление сплеш-скрином:
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    [Header("Splash Settings")]
    [SerializeField] private float minimumDisplayTime = 2f;
    [SerializeField] private string nextSceneName = "MainMenu";
    
    private float startTime;
    
    void Start()
    {
        startTime = Time.realtimeSinceStartup;
        
        // Если используем кастомный UI-сплеш
        StartCoroutine(ShowCustomSplash());
    }
    
    IEnumerator ShowCustomSplash()
    {
        // Активируем UI сплеш-скрина
        GameObject splashUI = GameObject.Find("SplashScreenCanvas");
        if (splashUI != null)
            splashUI.SetActive(true);
        
        // Показываем анимацию появления (через Animator)
        Animator animator = splashUI?.GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("Show");
        
        // Ждём минимальное время + пока анимация не завершится
        float elapsed = 0f;
        while (elapsed < minimumDisplayTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Скрываем сплеш
        if (animator != null)
            animator.SetTrigger("Hide");
        
        yield return new WaitForSeconds(0.5f); // Время анимации скрытия
        
        if (splashUI != null)
            splashUI.SetActive(false);
        
        // Загружаем следующую сцену
        SceneManager.LoadScene(nextSceneName);
    }
}
```

### 📱 Сплеш-скрин для мобильных платформ:
Android (Google Play):
```xml
<!-- Разрешения для сплеш-скрина -->
<!-- В AndroidManifest.xml -->
<application android:theme="@android:style/Theme.Light.NoTitleBar.Fullscreen">
    <!-- или -->
    <application android:theme="@style/UnitySplashTheme">
        <!-- ... -->
    </application>
</application>
```

iOS (App Store):
- Используйте Launch Screen Storyboard (системная заставка)
- Не используйте Unity Splash Screen для iOS (только для Android)
- Настройка в Player Settings → iOS → Splash Screen

---

## 2. Настройка первой сцены (First Scene)
Первая сцена определяет, что увидит пользователь после сплеш-скрина. Это может быть главное меню, экран загрузки или сразу игровой процесс.
### 📋 Стратегии организации стартовых сцен:
| Стратегия | Описание | Плюсы | Минусы |
| --- | --- | --- | --- |
| Меню | Загружается главное меню | Простота, быстрый старт | Задержка перед игрой |
| Загрузочная сцена | Отдельная сцена загрузки | Можно показывать прогресс | Дополнительная сцена |
| Сцена-менеджер | Пустая сцена для инициализации | Гибкость | Сложнее настройка |
| Прямой запуск | Сразу игровой уровень | Быстрый вход в игру | Нет меню настроек |

### 🎯 Настройка первой сцены в Build Settings:
1. File → Build Settings
2. В разделе Scenes In Build добавьте сцены
3. Первая сцена в списке — будет загружаться первой
4. Перетаскивайте сцены для изменения порядка

```csharp
// Пример: программная установка первой сцены
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneSetup : MonoBehaviour
{
    [MenuItem("Tools/Set Startup Scene")]
    public static void SetStartupScene()
    {
        // Устанавливаем первую сцену в списке Build Settings
        EditorSceneManager.OpenScene("Assets/Scenes/SplashScene.unity");
        
        // Добавляем сцены в Build Settings
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/SplashScene.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/GameScene.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/LoadingScene.unity", true)
        };
        
        Debug.Log("Startup scene set to SplashScene");
    }
}
#endif
```

### 🔄 Пример: Сцена-загрузчик с прогресс-баром
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text progressText;
    [SerializeField] private Text statusText;
    
    [Header("Loading Settings")]
    [SerializeField] private string targetSceneName = "GameScene";
    [SerializeField] private bool loadAdditive = false;
    
    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }
    
    IEnumerator LoadSceneAsync()
    {
        statusText.text = "Loading...";
        
        // Создаём операцию загрузки
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
        
        if (loadAdditive)
            operation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        
        // Запрещаем активацию сцены (чтобы показать прогресс)
        operation.allowSceneActivation = false;
        
        float progress = 0f;
        
        while (!operation.isDone)
        {
            // Прогресс загрузки (0-0.9)
            progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            // Обновляем UI
            progressSlider.value = progress;
            progressText.text = $"{progress * 100:F0}%";
            
            // Смена статуса
            if (progress < 0.3f)
                statusText.text = "Loading assets...";
            else if (progress < 0.6f)
                statusText.text = "Preparing world...";
            else if (progress < 0.8f)
                statusText.text = "Starting game...";
            else
                statusText.text = "Almost ready!";
            
            // Когда загрузка почти завершена (0.9)
            if (progress >= 0.9f)
            {
                // Активируем сцену
                operation.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        statusText.text = "Loading complete!";
        progressSlider.value = 1f;
        progressText.text = "100%";
        
        yield return new WaitForSeconds(0.5f);
        
        // Если загружали аддитивно, выгружаем эту сцену
        if (loadAdditive)
            SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
```

---

## 3. Build Pipeline (Конвейер сборки)
Build Pipeline — это процесс автоматизации сборки проекта. 
В Unity это реализуется через Build Pipeline API и скрипты в папке `Editor`.

### 🏗️ Основные компоненты Build Pipeline:

| Компонент | Описание |
| --- | --- |
| BuildPipeline.BuildPlayer() | Главный метод сборки |
| BuildPlayerOptions | Настройки сборки |
| BuildReport | Отчёт о сборке |
| PostProcessBuild | Действия после сборки |
| PreProcessBuild | Действия перед сборкой |

### 📝 Базовый пример сборки:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class SimpleBuildPipeline : MonoBehaviour
{
    [MenuItem("Build/Android/Release")]
    public static void BuildAndroidRelease()
    {
        // Настройки для релизной Android сборки
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = GetEnabledScenes();
        options.locationPathName = "Builds/Android/MyGame_v1.0.apk";
        options.target = BuildTarget.Android;
        options.targetGroup = BuildTargetGroup.Android;
        options.options = BuildOptions.None;
        
        BuildReport report = BuildPipeline.BuildPlayer(options);
        
        // Анализ результата
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"✅ Build успешен! Размер: {report.summary.totalSize} bytes");
            Debug.Log($"⏱️ Время сборки: {report.summary.totalTime}");
            
            // Пост-обработка
            OnBuildSuccess(report);
        }
        else
        {
            Debug.LogError($"❌ Build провален! Ошибок: {report.summary.totalErrors}");
            OnBuildFailed(report);
        }
    }
    
    private static string[] GetEnabledScenes()
    {
        // Получаем все включённые сцены из Build Settings
        var scenes = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }
    
    private static void OnBuildSuccess(BuildReport report)
    {
        // Действия после успешной сборки
        // Например: копирование APK в другую папку
        Debug.Log("Post-build processing...");
    }
    
    private static void OnBuildFailed(BuildReport report)
    {
        // Действия при неудачной сборке
        Debug.LogError("Build failed! Check console for errors.");
    }
}
#endif
```

### 🔧 Продвинутый Build Pipeline с настройками:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AdvancedBuildPipeline : MonoBehaviour
{
    // Настройки билда
    private const string BUILD_PATH = "Builds/";
    private const string ANDROID_APK = "MyGame.apk";
    private const string IOS_XCODE = "iOS_Project";
    
    // Методы сборки
    public enum BuildType
    {
        Development,
        Release
    }
    
    public enum Platform
    {
        Android,
        iOS,
        WebGL,
        Windows,
        Mac
    }
    
    [MenuItem("Build/Android/Development")]
    public static void BuildAndroidDevelopment()
    {
        BuildForPlatform(Platform.Android, BuildType.Development);
    }
    
    [MenuItem("Build/Android/Release")]
    public static void BuildAndroidRelease()
    {
        BuildForPlatform(Platform.Android, BuildType.Release);
    }
    
    [MenuItem("Build/iOS/Development")]
    public static void BuildIOSDevelopment()
    {
        BuildForPlatform(Platform.iOS, BuildType.Development);
    }
    
    [MenuItem("Build/iOS/Release")]
    public static void BuildIOSRelease()
    {
        BuildForPlatform(Platform.iOS, BuildType.Release);
    }
    
    [MenuItem("Build/Build All Platforms")]
    public static void BuildAllPlatforms()
    {
        BuildForPlatform(Platform.Android, BuildType.Release);
        BuildForPlatform(Platform.iOS, BuildType.Release);
        BuildForPlatform(Platform.Windows, BuildType.Release);
        Debug.Log("✅ All platforms built successfully!");
    }
    
    private static void BuildForPlatform(Platform platform, BuildType buildType)
    {
        // Настройка платформы
        BuildTarget target = GetBuildTarget(platform);
        BuildTargetGroup targetGroup = GetBuildTargetGroup(platform);
        
        // Настройка опций
        BuildOptions options = GetBuildOptions(buildType);
        
        // Путь для сборки
        string buildPath = GetBuildPath(platform, buildType);
        
        // Настройка Player Settings
        ConfigurePlayerSettings(targetGroup, buildType);
        
        // Сборка
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = buildPath,
            target = target,
            targetGroup = targetGroup,
            options = options
        };
        
        Debug.Log($"🚀 Начинаем сборку для {platform} ({buildType})...");
        Debug.Log($"📂 Путь: {buildPath}");
        
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"✅ Сборка {platform} ({buildType}) успешна! Размер: {report.summary.totalSize} bytes");
            Debug.Log($"⏱️ Время: {report.summary.totalTime}");
            
            // Дополнительные действия
            PostBuildActions(platform, buildType, buildPath);
        }
        else
        {
            Debug.LogError($"❌ Сборка {platform} ({buildType}) провалена!");
            Debug.LogError($"Ошибок: {report.summary.totalErrors}");
        }
    }
    
    private static BuildTarget GetBuildTarget(Platform platform)
    {
        switch (platform)
        {
            case Platform.Android: return BuildTarget.Android;
            case Platform.iOS: return BuildTarget.iOS;
            case Platform.WebGL: return BuildTarget.WebGL;
            case Platform.Windows: return BuildTarget.StandaloneWindows64;
            case Platform.Mac: return BuildTarget.StandaloneOSX;
            default: return BuildTarget.StandaloneWindows64;
        }
    }
    
    private static BuildTargetGroup GetBuildTargetGroup(Platform platform)
    {
        switch (platform)
        {
            case Platform.Android: return BuildTargetGroup.Android;
            case Platform.iOS: return BuildTargetGroup.iOS;
            case Platform.WebGL: return BuildTargetGroup.WebGL;
            case Platform.Windows: return BuildTargetGroup.Standalone;
            case Platform.Mac: return BuildTargetGroup.Standalone;
            default: return BuildTargetGroup.Standalone;
        }
    }
    
    private static BuildOptions GetBuildOptions(BuildType buildType)
    {
        switch (buildType)
        {
            case BuildType.Development:
                return BuildOptions.Development | BuildOptions.AllowDebugging;
            case BuildType.Release:
                return BuildOptions.None;
            default:
                return BuildOptions.None;
        }
    }
    
    private static string GetBuildPath(Platform platform, BuildType buildType)
    {
        string folder = $"{BUILD_PATH}{platform}/";
        string filename = "";
        
        switch (platform)
        {
            case Platform.Android:
                filename = ANDROID_APK;
                break;
            case Platform.iOS:
                filename = IOS_XCODE;
                break;
            case Platform.Windows:
                filename = "MyGame.exe";
                break;
            case Platform.WebGL:
                filename = "WebGL";
                break;
            default:
                filename = "MyGame";
                break;
        }
        
        if (buildType == BuildType.Development)
            filename = filename.Replace(".", "_Dev.");
        
        return folder + filename;
    }
    
    private static void ConfigurePlayerSettings(BuildTargetGroup targetGroup, BuildType buildType)
    {
        // Общие настройки
        PlayerSettings.companyName = "MyGameStudio";
        PlayerSettings.productName = "MyGame";
        PlayerSettings.bundleVersion = "1.0.0";
        
        // Платформо-специфичные настройки
        if (targetGroup == BuildTargetGroup.Android)
        {
            PlayerSettings.SetApplicationIdentifier(targetGroup, "com.mygamestudio.mygame");
            PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel30;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            
            if (buildType == BuildType.Release)
            {
                PlayerSettings.Android.keystorePass = "secure_password";
                PlayerSettings.Android.keyaliasName = "mygame_alias";
                PlayerSettings.Android.keyaliasPass = "secure_password";
            }
        }
        else if (targetGroup == BuildTargetGroup.iOS)
        {
            PlayerSettings.SetApplicationIdentifier(targetGroup, "com.mygamestudio.mygame");
            PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.targetOSVersionString = "13.0";
        }
    }
    
    private static void PostBuildActions(Platform platform, BuildType buildType, string buildPath)
    {
        // Копирование в архив
        string archivePath = $"Archive/{platform}_{buildType}_{System.DateTime.Now:yyyy-MM-dd_HH-mm}/";
        System.IO.Directory.CreateDirectory(archivePath);
        
        // Копируем файлы
        if (platform == Platform.Android)
        {
            System.IO.File.Copy(buildPath, archivePath + "MyGame.apk", true);
        }
        else if (platform == Platform.iOS)
        {
            // Архивируем Xcode проект
            string zipPath = archivePath + "iOS_Project.zip";
            System.IO.Compression.ZipFile.CreateFromDirectory(buildPath, zipPath);
        }
        
        Debug.Log($"📦 Файлы скопированы в архив: {archivePath}");
    }
    
    private static string[] GetEnabledScenes()
    {
        var scenes = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }
}
#endif
```

### 🔌 Pre-Build и Post-Build Callbacks:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildCallbacksHandler : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;
    
    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log($"🔧 Pre-Build: {report.summary.platform} (Build {report.summary.buildStartTime})");
        
        // Проверки перед сборкой
        CheckForErrors();
        ValidateAssets();
        UpdateBuildNumber();
    }
    
    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"🎉 Post-Build: Success! {report.summary.totalSize} bytes");
            
            // Отправка уведомления
            SendBuildNotification(report);
            
            // Запуск тестов
            RunPostBuildTests();
        }
        else
        {
            Debug.LogError($"💀 Post-Build: Failed! {report.summary.totalErrors} errors");
            LogErrorDetails(report);
        }
    }
    
    private void CheckForErrors()
    {
        // Проверка на ошибки в консоли
        Debug.Log("Checking for errors...");
    }
    
    private void ValidateAssets()
    {
        // Валидация ассетов
        Debug.Log("Validating assets...");
    }
    
    private void UpdateBuildNumber()
    {
        // Автоматическое обновление номера билда
        string version = PlayerSettings.bundleVersion;
        string[] parts = version.Split('.');
        int buildNumber = int.Parse(parts[parts.Length - 1]);
        buildNumber++;
        parts[parts.Length - 1] = buildNumber.ToString();
        PlayerSettings.bundleVersion = string.Join(".", parts);
        Debug.Log($"📌 Build number updated to: {PlayerSettings.bundleVersion}");
    }
    
    private void SendBuildNotification(BuildReport report)
    {
        // Отправка уведомления (например, в Telegram)
        Debug.Log($"📨 Sending notification: Build {report.summary.platform} completed");
    }
    
    private void RunPostBuildTests()
    {
        // Запуск автоматических тестов
        Debug.Log("🧪 Running post-build tests...");
    }
    
    private void LogErrorDetails(BuildReport report)
    {
        foreach (var step in report.steps)
        {
            if (step.result == BuildResult.Failed)
            {
                Debug.LogError($"Step failed: {step.name}");
                foreach (var log in step.messages)
                {
                    Debug.LogError($"  {log.content}");
                }
            }
        }
    }
}
#endif
```

---

## 4. Оптимизация времени загрузки
### ⚡ Стратегии оптимизации:
| Стратегия | Описание | Влияние |
| --- | --- | --- |
| Addressables | Загрузка ассетов по требованию | Сильное |
| Asset Bundles | Разделение контента на пакеты | Сильное |
| Scene Streaming | Загрузка сцены по частям | Среднее |
| Pre-loading | Загрузка в фоне во время сплеша | Среднее |
| LOD System | Уровни детализации | Слабое |

### 📝 Пример: Минимизация времени загрузки:
```csharp
public class OptimizedLoader : MonoBehaviour
{
    void Awake()
    {
        // Включаем быструю загрузку
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        
        // Настройка качества текстур
        QualitySettings.masterTextureLimit = 0; // Максимальное качество
        
        // Кэширование шейдеров
        Shader.WarmupAllShaders();
    }
    
    void Start()
    {
        StartCoroutine(LoadWithAddressables());
    }
    
    IEnumerator LoadWithAddressables()
    {
        #if UNITY_ADDRESSABLES
        // Загрузка критических ресурсов
        var handle = Addressables.LoadAssetsAsync<GameObject>("player_prefabs", null);
        yield return handle;
        
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperationStatus.Succeeded)
        {
            Debug.Log("✅ Critical assets loaded successfully");
        }
        #endif
    }
}
```

---

## 5. Лучшие практики
### ✅ Рекомендации:
1. Минимальное время сплеш-скрина — 2 секунды (но не более 5)
2. Используйте прогресс-бар для загрузочных экранов
3. Автоматизируйте сборку через Build Pipeline
4. Версионируйте билды (например, 1.0.0.123)
5. Сохраняйте архивы билдов для отката
6. Тестируйте стартовые сцены на слабых устройствах

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Забыли добавить сцену в Build Settings
SceneManager.LoadScene("NewScene"); // Ошибка! Сцены нет в билде

// ✅ ПРАВИЛЬНО: Проверяем перед загрузкой
#if UNITY_EDITOR
SceneManager.LoadScene("NewScene");
#else
// В билде сцена должна быть в Build Settings
#endif

// ❌ ОШИБКА: Долгая инициализация в Awake/Start
void Start() { LoadAllAssets(); } // Зависание

// ✅ ПРАВИЛЬНО: Асинхронная загрузка
IEnumerator Start() 
{ 
    yield return StartCoroutine(LoadAllAssetsAsync()); 
}

// ❌ ОШИБКА: Игнорирование разрешений для сплеш-скрина
// На Android сплеш-скрин требует Fullscreen Theme

// ✅ ПРАВИЛЬНО: Настройка темы в AndroidManifest.xml
<application android:theme="@style/UnitySplashTheme">
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
