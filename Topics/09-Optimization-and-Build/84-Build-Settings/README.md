# 📦 Unity Build Settings: Player Settings, Icons, Splash Screens, Permissions

Build is the process of converting your project into an executable application for the target platform. 
Proper Player Settings configuration is critical for the app's appearance, functionality, and compliance with app store requirements.

---

## 1. Player Settings — The Main Control Center
Player Settings is the window where all final application parameters are configured: from name to permissions.

### 🗂️ Accessing Player Settings:
```text
Edit → Project Settings → Player
```
Or via Build Settings → Player Settings button.

### 📋 Main Player Settings Sections:
| Section | Description |
| --- | --- |
| Company Name | Company name (displayed in the system) |
| Product Name | Application name (displayed to users) |
| Version | Application version (e.g., 1.0.0) |
| Default Icon | Application icon |
| Splash Screen | Startup splash screen |
| Resolution | Screen resolution, fullscreen mode |
| Run In Background | Whether the game runs in the background |
| Scripting Backend | IL2CPP or Mono |
| API Compatibility | .NET Standard 2.0 / .NET 4.x |
| Android / iOS Settings | Platform-specific settings |

---

## 2. Configuring Icons
The icon is the face of your application in the store and on the desktop. 
Unity allows uploading icons of different sizes for different platforms.

### 🖼️ Configuring Icons for Android:
Path: Player Settings → Android → Icon

| Size (px) | Purpose |
| --- | --- |
| 512×512 | Google Play icon |
| 192×192 | High resolution (adaptive icon) |
| 144×144 | XXHDPI |
| 96×96 | XHDPI |
| 72×72 | HDPI |
| 48×48 | MDPI |

#### Adaptive Icons (Android 8+):
In Player Settings → Android → Icon → Adaptive Icon

Set two layers:
1. Foreground — image (mask required)
2. Background — color or image
  
### 🍎 Configuring Icons for iOS:
Path: Player Settings → iOS → Icon

| Size (px) | Purpose |
| --- | --- |
| 180×180 | iPhone (60pt @3x) |
| 120×120 | iPhone (60pt @2x) |
| 152×152 | iPad (76pt @2x) |
| 76×76 | iPad (76pt @1x) |
| 167×167 | iPad Pro (83.5pt @2x) |
| 1024×1024 | App Store |

### 📝 Example: Automatic Icon Setup via Script:
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
            // ... all sizes
        };
        
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, icons);
        
        Debug.Log("Icons applied successfully!");
    }
}
#endif
```

> [!Tip]
> Use an icon generator (e.g., Adaptive Icon Generator in Unity 2022+) to automatically create all sizes from a single image.

---

## 3. Configuring Splash Screen
Splash Screen is the startup screen shown while the application loads.

### 🎨 Configuring the Splash Screen:
Path: Player Settings → Splash Screen
| Parameter | Description |
| --- | --- |
| Show Splash Screen | Enable/disable the splash screen |
| Splash Style | Dark / Light (background) |
| Animation | Logo animation (Center / Scaling / Custom) |
| Logo | Logo image (PNG with transparency) |
| Background | Background color or gradient |
| Draw Mode | Unity Logo / All Sequential / All Simultaneous |

### 📱 Example: Custom Splash Screen Setup:
In Player Settings → Splash Screen:
1. Set Splash Style: Dark
2. Upload your logo (company logo)
3. Set Background Color: #1a1a2e
4. Disable "Show Unity Logo" (if you have Pro license)

### ⏱️ Programmatic Splash Screen Control:
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SplashScreenController : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ShowCustomSplash());
    }
    
    IEnumerator ShowCustomSplash()
    {
        GameObject splashUI = GameObject.Find("SplashScreenCanvas");
        splashUI.SetActive(true);
        
        yield return new WaitForSeconds(3f);
        
        splashUI.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
}
```

### 🏪 Splash Screen for Stores:
Google Play:
- Splash must be static (not animated)
- Minimum display time — 1.5 seconds
- No sound

App Store:
- Requires Launch Screen (system-provided, not via Unity)
- Use Unity Splash Screen only for logo

---

## 4. Configuring Permissions
Permissions are the app's access to device functions (camera, microphone, GPS, etc.). For Android and iOS, this is mandatory.

### 🤖 Android Permissions:
Path: Player Settings → Android → Publishing Settings → Permissions

Common Permissions:
| Permission | Key | Usage |
| --- | --- | --- |
| Internet | `INTERNET` | Internet access (mandatory) |
| Camera | `CAMERA` | Using the camera |
| Microphone | `RECORD_AUDIO` | Audio recording | 
| GPS | `ACCESS_FINE_LOCATION` | GPS coordinates |
| Storage | `READ_EXTERNAL_STORAGE` / `WRITE_EXTERNAL_STORAGE` | File access |
| Vibrate | `VIBRATE` | Vibration |

Adding Custom Permission to Manifest:
```xml
<!-- Assets/Plugins/Android/AndroidManifest.xml -->
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <uses-permission android:name="android.permission.VIBRATE" />
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.CAMERA" />
    
    <!-- For Android 13+ (API 33) -->
    <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
    
    <application android:icon="@drawable/app_icon">
        <!-- ... -->
    </application>
</manifest>
```

### 🍎 iOS Permissions:
Path: Player Settings → iOS → Other Settings

Configuring Permissions in Info.plist:
```xml
<!-- Xcode → Info.plist or via Unity Player Settings -->
<key>NSCameraUsageDescription</key>
<string>Using camera to scan QR codes</string>

<key>NSMicrophoneUsageDescription</key>
<string>Voice recording for in-game chat</string>

<key>NSLocationWhenInUseUsageDescription</key>
<string>To find players near you</string>

<key>NSPhotoLibraryUsageDescription</key>
<string>Saving screenshots to gallery</string>
```

Example: Requesting Permissions in Code:
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
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        
        if (SystemInfo.operatingSystem.Contains("Android 13"))
        {
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
        }
        #endif
        
        #if UNITY_IOS
        // iOS permissions are requested via plugins or Native API
        #endif
    }
    
    public bool IsPermissionGranted(string permission)
    {
        #if UNITY_ANDROID
        return Permission.HasUserAuthorizedPermission(permission);
        #elif UNITY_IOS
        return true;
        #else
        return true;
        #endif
    }
}
```

---

## 5. Additional Build Settings
### 🔧 Scripting Backend
| Backend | Description | When to Use |
| --- | --- | --- |
| Mono | Faster compilation, larger size | Development, testing |
| IL2CPP | Optimized code, better performance | Release builds |

Path: Player Settings → Scripting Backend

### ⚡ Managed Stripping Level
| Level | Description |
| --- | --- |
| Disabled | No stripping (maximum size) |
| Low | Minimal stripping |
| Medium | Medium stripping (recommended) |
| High | Aggressive stripping (size savings, risk of errors) |

### 🎮 Other Settings
| Parameter | Description |
| --- | --- |
| Run In Background | Continue execution in background |
| Fullscreen Mode | Fullscreen / Windowed |
| Default Screen Width/Height | Window resolution |
| Color Space | Gamma / Linear (for graphics) |
| Graphics API | DirectX, Vulkan, OpenGL, Metal |

---

## 6. Build Process
### 📦 Standard Process:
1. File → Build Settings
2. Choose platform (Android / iOS / Standalone)
3. Click Player Settings for additional configuration
4. Click Build and select a folder
5. Wait for the build to complete

### 🤖 Android Build Specifics:
Required Settings:

| Parameter | Value |
| --- | --- |
| Build System | Gradle (recommended) |
| Minimum API Level | Android 6.0 (API 23) or higher |
| Target API Level | Latest available (API 33+) |
| Key Store | APK signing (required for Google Play) |
| IL2CPP Code Generation | Faster (smaller) / Faster (runtime) |

Generating a Keystore:
1. Player Settings → Android → Publishing Settings
2. Keystore Manager → Create New
3. Fill in fields:
   - Keystore name: `mygame.keystore`
   - Password: `******`
   - Alias: `mygame_alias`
   - Validity (years): 25
  
### 🍎 iOS Build Specifics:
| Parameter | Value |
| --- | --- |
| Target SDK | Device SDK (for release) |
| Target Device | iPhone + iPad |
| Bundle Identifier | com.company.appname (unique) |
| Automatic Signing | Enable (for development) |
| Scripting Backend | IL2CPP |

Process:
1. Unity generates an Xcode project
2. Open in Xcode
3. Configure Signing
4. Build .ipa file

---

## 7. Example: Full Automated Build Script
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
        PlayerSettings.companyName = "MyGameStudio";
        PlayerSettings.productName = "DungeonQuest";
        PlayerSettings.bundleVersion = "1.0.0";
        
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.mygamestudio.dungeonquest");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_Standard_2_0);
        
        PlayerSettings.Android.keystorePass = "password123";
        PlayerSettings.Android.keyaliasName = "game_alias";
        PlayerSettings.Android.keyaliasPass = "password123";
        
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
            Debug.Log($"Build succeeded: {summary.totalSize} bytes");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError($"Build failed: {summary.totalErrors} errors");
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
        Debug.Log("iOS Xcode project created in Builds/iOS");
    }
}
#endif
```

---

## 8. Best Practices
✅ Recommendations:
1. Use Version Control for PlayerSettings — track changes
2. Store Keystore in a safe place — don't lose keys for updates
3. Test builds on real devices — emulators don't show the full picture
4. Optimize build size — use Stripping and texture compression
5. Configure permissions as needed — don't ask for unnecessary permissions

❌ Common Mistakes:
```text
// ❌ ERROR: Forgot to set Bundle Identifier
// On Android: com.Company.ProductName
// On iOS: com.company.appname (must be unique)

// ❌ ERROR: Build size too large
// Use Texture Compression (ASTC for Android, PVRTC for iOS)

// ❌ ERROR: Ignoring permissions
// If you don't request a permission, the app will crash when trying to access it

// ❌ ERROR: Incorrect Splash Screen
// Google Play prohibits animation and sound

// ❌ ERROR: Using Mono for release
// Always use IL2CPP for final builds
```

---

### ⭐ If this project was useful, put a star on GitHub!
