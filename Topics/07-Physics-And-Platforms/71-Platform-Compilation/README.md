# 🔧 Platform Compilation in Unity: Preprocessor Directives (UNITY_ANDROID, UNITY_IOS, UNITY_EDITOR)
Unity allows creating cross-platform games, but sometimes you need to execute different code depending on the target platform. 
For this, preprocessor directives (conditional compilation) are used. 
They tell the Unity compiler which code to include or exclude when building for a specific platform.

---

## 1. What are Preprocessor Directives?
Preprocessor directives are instructions that are processed before compilation begins. 

### They allow you to:
- Exclude platform-incompatible code
- Use platform-dependent APIs (e.g., `Application.OpenURL` behaves differently)
- Optimize performance for a specific platform
- Include debug code only in the editor

### Syntax:
```csharp
#if SYMBOL
    // Code compiles if SYMBOL is defined
#elif OTHER_SYMBOL
    // Code compiles for another symbol
#else
    // Code compiles if no symbol matched
#endif
```

> [!Important]
> Directives work at compile time. Code inside an incorrect directive never makes it into the final build.

---

## 2. Main Platform Symbols
### 📱 Mobile Platforms
| Symbol | Platform | When defined |
| --- | --- | --- |
| `UNITY_ANDROID` | Android | When building for Android |
| `UNITY_IOS` | iOS | When building for iOS |
| `UNITY_TVOS` | tvOS | When building for tvOS |

### 💻 Desktop Platforms
| Symbol | Platform | When defined |
| --- | --- | --- |
| `UNITY_STANDALONE_WIN` | Windows | Windows build |
| `UNITY_STANDALONE_OSX` | macOS | macOS build |
| `UNITY_STANDALONE_LINUX` | Linux | Linux build |
| `UNITY_STANDALONE` | Any desktop | Any desktop build |

### 🎮 Gaming Consoles
| Symbol | Platform |
| --- | --- |
| `UNITY_XBOXONE` | Xbox One |
| `UNITY_PS4` | PlayStation 4 |
| `UNITY_PS5` | PlayStation 5 |
| `UNITY_SWITCH` | Nintendo Switch |

### 🛠️ Editor & Development
| Symbol | Description |
| --- | --- |
| `UNITY_EDITOR` | Code runs inside Unity Editor |
| `UNITY_EDITOR_WIN` | Editor on Windows |
| `UNITY_EDITOR_OSX` | Editor on macOS |
| `DEVELOPMENT_BUILD` | Development Build flag enabled |
| `UNITY_DEBUG` | Defined in Debug builds |

### 🏷️ Unity Versions
```csharp
#if UNITY_2020_1_OR_NEWER
    // Code for Unity 2020.1 and newer
#endif

#if UNITY_2019_3_OR_NEWER && !UNITY_2020_1_OR_NEWER
    // Code only for versions between 2019.3 and 2020.0
#endif
```

---

## 3. Examples of Each Directive
### 📱 Example 1: UNITY_ANDROID — Save to Gallery
```csharp
using UnityEngine;

public class PlatformScreenshot : MonoBehaviour
{
    public void TakeScreenshot()
    {
        string filename = "screenshot.png";
        
        ScreenCapture.CaptureScreenshot(filename);
        
#if UNITY_ANDROID
        // On Android, save to gallery via Native API
        AndroidJavaClass mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection");
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        
        string path = Application.persistentDataPath + "/" + filename;
        mediaScanner.CallStatic("scanFile", currentActivity, new string[] { path }, null, null);
        
        Debug.Log("Screenshot saved to Android gallery");
#elif UNITY_IOS
        // iOS requires special permission
        Debug.Log("Screenshot saved to app folder");
#else
        Debug.Log("Screenshot saved: " + Application.persistentDataPath + "/" + filename);
#endif
    }
}
```

### 🍎 Example 2: UNITY_IOS — Push Notifications
```csharp
public class IOSNotificationHandler : MonoBehaviour
{
    void Start()
    {
#if UNITY_IOS
        // Import iOS namespace only for iOS builds
        using UnityEngine.iOS;
        
        NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
        
        var notification = NotificationServices.GetLastNotification();
        if (notification != null)
        {
            Debug.Log("Launched from notification: " + notification.alertBody);
            HandleNotification(notification);
        }
#endif
    }
    
#if UNITY_IOS
    private void HandleNotification(NotificationEventArgs args)
    {
        // Notification handling logic
    }
#endif
}
```

### 🖥️ Example 3: UNITY_EDITOR — Debugging Without Device
```csharp
using UnityEngine;

public class PlatformDebug : MonoBehaviour
{
    private void Update()
    {
#if UNITY_EDITOR
        // Emulate touch with mouse in editor
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Debug.Log($"[Editor] Mouse click at: {mousePos}");
            HandleTap(mousePos);
        }
#elif UNITY_ANDROID || UNITY_IOS
        // Real touch on device
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log($"[Device] Touch at: {touch.position}");
                HandleTap(touch.position);
            }
        }
#endif
    }
    
    private void HandleTap(Vector2 screenPos)
    {
        // Common tap handling logic
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Hit object: " + hit.collider.name);
        }
    }
}
```

### 💾 Example 4: Different Save Paths for Different Platforms
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
            Debug.Log($"Save directory created: {path}");
        }
    }
}
```

### 🎮 Example 5: Disable Back Button on Android
```csharp
public class AndroidBackButtonHandler : MonoBehaviour
{
    void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Only on real Android device
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowExitDialog();
        }
#elif UNITY_EDITOR
        // In editor, use Escape key for emulation
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[Editor] Emulating Android Back button");
            ShowExitDialog();
        }
#endif
    }
    
    private void ShowExitDialog()
    {
        Debug.Log("Showing exit confirmation dialog");
    }
}
```

### 🛡️ Example 6: Combining Multiple Directives
```csharp
public class AdvancedPlatformHandler : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR
        Debug.Log("=== RUNNING IN EDITOR ===");
        EnableDebugTools();
#elif DEVELOPMENT_BUILD
        Debug.Log("=== DEVELOPMENT BUILD ===");
        EnableErrorLogging();
#else
        Debug.Log("=== FINAL RELEASE BUILD ===");
        DisableDebugTools();
#endif

#if UNITY_ANDROID && (DEVELOPMENT_BUILD || UNITY_EDITOR)
        Debug.Log("Android debugging via ADB enabled");
        EnableADBLogging();
#endif

#if UNITY_IOS && !UNITY_EDITOR
        Debug.Log("Running on real iOS device");
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

## 4. Platform Tiling (Platform-Dependent Assets)
Sometimes code alone isn't enough — you need different resources (fonts, sounds, input controllers). 
### Unity provides Platform Tiling for this:
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

### Setup in Inspector:
1. Select the asset
2. Find Platform Settings section in the Inspector
3. Check the box for the desired platform

---

## 5. Custom Compilation Symbols
You can create your own symbols in Player Settings → Scripting Define Symbols.
```csharp
// File ProjectSettings/ProjectSettings.asset or through UI
// Add: USE_ANALYTICS;BETA_BUILD

#if USE_ANALYTICS
    Analytics.SendEvent("game_start");
#endif

#if BETA_BUILD
    Debug.Log("VERSION: Beta build");
    EnableBetaFeatures();
#endif
```

### Adding via code:
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

## 6. Best Practices and Common Mistakes
### ✅ Recommendations:
1. Don't duplicate code — extract platform-dependent code into separate methods
2. Use `#if UNITY_EDITOR` for debugging — don't clutter release builds
3. Test each platform separately — what works in the editor may not work on device
4. Document platform-specific blocks — comments help other developers
5. For complex logic, use the Adapter pattern instead of many directives

### ❌ Common Mistakes:
```csharp
// ❌ ERROR: Forgot to close directive
#if UNITY_ANDROID
    Debug.Log("Android");
    
// ❌ ERROR: Using undefined symbol
#if UNITY_SAMSUNG  // This symbol doesn't exist!

// ❌ ERROR: Mixing with runtime checks
if (Application.platform == RuntimePlatform.Android)  // Runtime, not compilation!
{
    // This code will end up in builds for ALL platforms
}
```

### ✅ Correct Approach:
```csharp
// ✓ Correct: Always close
#if UNITY_ANDROID
    Debug.Log("Android");
#endif

// ✓ Correct: Use only documented symbols
#if UNITY_ANDROID && !UNITY_EDITOR
    Debug.Log("Real Android device");
#endif

// ✓ Use runtime checks only when conditional compilation is impossible
#if UNITY_ANDROID
    // Platform-dependent code
#else
    // Fallback
    Debug.Log(Application.platform.ToString());
#endif
```

---

## 7. Symbol to Platform Mapping Table
| Platform | Main Symbol | Additional Symbols |
| --- | --- | --- |
| Android | `UNITY_ANDROID` | `UNITY_ANDROID`, `UNITY_ANDROID_API_LEVEL` |
| iOS | `UNITY_IOS` | `UNITY_IOS`, `UNITY_IPHONE` (deprecated) |
| Windows Standalone | `UNITY_STANDALONE_WIN` | `UNITY_STANDALONE`, `UNITY_WIN` |
| macOS Standalone | `UNITY_STANDALONE_OSX` | `UNITY_STANDALONE` |
| Linux Standalone | `UNITY_STANDALONE_LINUX` | `UNITY_STANDALONE` |
| WebGL | `UNITY_WEBGL` | - |
| Unity Editor | `UNITY_EDITOR` | `UNITY_EDITOR_WIN`, `UNITY_EDITOR_OSX` |

---

### ⭐ If this project was useful, put a star on GitHub!
