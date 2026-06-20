# 🚀 Splash Screens and Startup Scenes in Unity: Splash Screen, First Scene Setup, Build Pipeline
The game startup process is the user's first impression. Proper organization of startup screens, 
loading sequences, and build automation is critical for a professional project. 
In this guide, we'll cover all aspects: from splash screen to advanced Build Pipeline.

---

## 1. Splash Screen — The First Thing Users See
Splash Screen is the startup screen displayed immediately after the application launches, before the first scene loads.

### 🎨 Types of Splash Screens in Unity:
| Type | Description | When to Use |
| --- | --- | --- |
| Unity Splash Screen | Built-in Unity splash with logo | Free version (mandatory) |
| Custom Splash Screen | Your own loading screen | Pro version or custom implementation |
| System Launch Screen | System splash (iOS/Android) | For mobile platforms |

### 🛠️ Configuring Unity Splash Screen:
Path: Player Settings → Splash Screen
```text
// Key parameters
Show Splash Screen: true/false
Splash Style: Dark / Light
Animation: Center / Scaling / Custom
Logo: Texture2D (company logo)
Background: Color or Gradient
Draw Mode: Unity Logo / All Sequential / All Simultaneous
```

### 📝 Example: Advanced Splash Screen Configuration
Player Settings → Splash Screen
1. Enable Show Splash Screen
2. Set Splash Style: Dark
3. Add company logo (PNG with transparency)
4. Set background: gradient #0f0c29 → #302b63 → #24243e
5. Disable "Show Unity Logo" (Pro license only)
6. Set Animation: Center (smooth fade-in)

### ⏱️ Programmatic Splash Screen Control:
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
        StartCoroutine(ShowCustomSplash());
    }
    
    IEnumerator ShowCustomSplash()
    {
        GameObject splashUI = GameObject.Find("SplashScreenCanvas");
        if (splashUI != null)
            splashUI.SetActive(true);
        
        Animator animator = splashUI?.GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("Show");
        
        float elapsed = 0f;
        while (elapsed < minimumDisplayTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (animator != null)
            animator.SetTrigger("Hide");
        
        yield return new WaitForSeconds(0.5f);
        
        if (splashUI != null)
            splashUI.SetActive(false);
        
        SceneManager.LoadScene(nextSceneName);
    }
}
```

---

## 2. First Scene Configuration
The first scene determines what users see after the splash screen. 
It could be a main menu, loading screen, or directly the gameplay.

### 📋 Startup Scene Strategies:
| Strategy | Description | Pros | Cons |
| --- | --- | --- | --- |
| Menu | Loads main menu | Simple, fast start | Delay before game |
| Loading Scene | Separate loading scene | Can show progress | Extra scene |
| Manager Scene | Empty scene for initialization | Flexible | Harder to configure |
| Direct Launch | Directly to game level | Quick entry | No settings menu |

### 🎯 Setting the First Scene in Build Settings:
1. File → Build Settings
2. In Scenes In Build, add scenes
3. The first scene in the list — loads first
4. Drag to reorder scenes

```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneSetup : MonoBehaviour
{
    [MenuItem("Tools/Set Startup Scene")]
    public static void SetStartupScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SplashScene.unity");
        
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

### 🔄 Example: Loading Scene with Progress Bar
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
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
        
        if (loadAdditive)
            operation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        
        operation.allowSceneActivation = false;
        
        float progress = 0f;
        
        while (!operation.isDone)
        {
            progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            progressSlider.value = progress;
            progressText.text = $"{progress * 100:F0}%";
            
            if (progress < 0.3f)
                statusText.text = "Loading assets...";
            else if (progress < 0.6f)
                statusText.text = "Preparing world...";
            else if (progress < 0.8f)
                statusText.text = "Starting game...";
            else
                statusText.text = "Almost ready!";
            
            if (progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        statusText.text = "Loading complete!";
        progressSlider.value = 1f;
        progressText.text = "100%";
        
        yield return new WaitForSeconds(0.5f);
        
        if (loadAdditive)
            SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
```

---

## 3. Build Pipeline
Build Pipeline is the automation process for project building. 
In Unity, this is implemented through the Build Pipeline API and scripts in the `Editor` folder.

### 🏗️ Key Build Pipeline Components:
| Component | Description |
| --- | --- |
| BuildPipeline.BuildPlayer() | Main build method |
| BuildPlayerOptions | Build configuration |
| BuildReport | Build report |
| PostProcessBuild | Actions after build |
| PreProcessBuild | Actions before build |

### 📝 Basic Build Example:
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
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = GetEnabledScenes();
        options.locationPathName = "Builds/Android/MyGame_v1.0.apk";
        options.target = BuildTarget.Android;
        options.targetGroup = BuildTargetGroup.Android;
        options.options = BuildOptions.None;
        
        BuildReport report = BuildPipeline.BuildPlayer(options);
        
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"✅ Build successful! Size: {report.summary.totalSize} bytes");
            Debug.Log($"⏱️ Build time: {report.summary.totalTime}");
            OnBuildSuccess(report);
        }
        else
        {
            Debug.LogError($"❌ Build failed! Errors: {report.summary.totalErrors}");
            OnBuildFailed(report);
        }
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
    
    private static void OnBuildSuccess(BuildReport report)
    {
        Debug.Log("Post-build processing...");
    }
    
    private static void OnBuildFailed(BuildReport report)
    {
        Debug.LogError("Build failed! Check console for errors.");
    }
}
#endif
```

### 🔧 Advanced Build Pipeline:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AdvancedBuildPipeline : MonoBehaviour
{
    private const string BUILD_PATH = "Builds/";
    private const string ANDROID_APK = "MyGame.apk";
    private const string IOS_XCODE = "iOS_Project";
    
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
        BuildTarget target = GetBuildTarget(platform);
        BuildTargetGroup targetGroup = GetBuildTargetGroup(platform);
        BuildOptions options = GetBuildOptions(buildType);
        string buildPath = GetBuildPath(platform, buildType);
        
        ConfigurePlayerSettings(targetGroup, buildType);
        
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = buildPath,
            target = target,
            targetGroup = targetGroup,
            options = options
        };
        
        Debug.Log($"🚀 Starting build for {platform} ({buildType})...");
        Debug.Log($"📂 Path: {buildPath}");
        
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"✅ Build {platform} ({buildType}) successful! Size: {report.summary.totalSize} bytes");
            Debug.Log($"⏱️ Time: {report.summary.totalTime}");
            PostBuildActions(platform, buildType, buildPath);
        }
        else
        {
            Debug.LogError($"❌ Build {platform} ({buildType}) failed!");
            Debug.LogError($"Errors: {report.summary.totalErrors}");
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
        PlayerSettings.companyName = "MyGameStudio";
        PlayerSettings.productName = "MyGame";
        PlayerSettings.bundleVersion = "1.0.0";
        
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
        string archivePath = $"Archive/{platform}_{buildType}_{System.DateTime.Now:yyyy-MM-dd_HH-mm}/";
        System.IO.Directory.CreateDirectory(archivePath);
        
        if (platform == Platform.Android)
        {
            System.IO.File.Copy(buildPath, archivePath + "MyGame.apk", true);
        }
        else if (platform == Platform.iOS)
        {
            string zipPath = archivePath + "iOS_Project.zip";
            System.IO.Compression.ZipFile.CreateFromDirectory(buildPath, zipPath);
        }
        
        Debug.Log($"📦 Files archived: {archivePath}");
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

---

## 4. Optimization Strategies
### ⚡ Loading Time Optimization:
| Strategy | Description | Impact |
| --- | --- | --- |
| Addressables | Load assets on demand | High |
| Asset Bundles | Split content into packages | High |
| Scene Streaming | Load scene in parts | Medium |
| Pre-loading | Load in background during splash | Medium |
| LOD System | Level of detail | Low |

### 📝 Example: Minimizing Load Time:
```csharp
public class OptimizedLoader : MonoBehaviour
{
    void Awake()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        QualitySettings.masterTextureLimit = 0;
        Shader.WarmupAllShaders();
    }
    
    void Start()
    {
        StartCoroutine(LoadWithAddressables());
    }
    
    IEnumerator LoadWithAddressables()
    {
        #if UNITY_ADDRESSABLES
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

## 5. Best Practices
### ✅ Recommendations:
1. Minimum splash screen time — 2 seconds (but not more than 5)
2. Use progress bars for loading screens
3. Automate builds using Build Pipeline
4. Version builds (e.g., 1.0.0.123)
5. Keep build archives for rollbacks
6. Test startup scenes on low-end devices

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Forgot to add scene to Build Settings
SceneManager.LoadScene("NewScene"); // Error! Scene not in build

// ✅ CORRECT: Check before loading
#if UNITY_EDITOR
SceneManager.LoadScene("NewScene");
#else
// Scene must be in Build Settings
#endif

// ❌ ERROR: Long initialization in Awake/Start
void Start() { LoadAllAssets(); } // Freeze

// ✅ CORRECT: Async loading
IEnumerator Start() 
{ 
    yield return StartCoroutine(LoadAllAssetsAsync()); 
}

// ❌ ERROR: Ignoring splash screen permissions
// Android splash requires Fullscreen Theme

// ✅ CORRECT: Theme configuration in AndroidManifest.xml
<application android:theme="@style/UnitySplashTheme">
```

---

### ⭐ If this project was useful, put a star on GitHub!
