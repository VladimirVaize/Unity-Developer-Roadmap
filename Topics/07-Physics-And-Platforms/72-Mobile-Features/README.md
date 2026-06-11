# 📱 Mobile Features in Unity: Push Notifications, Touches, Battery Optimization
Developing for mobile platforms (Android and iOS) requires consideration of specific device capabilities and limitations. 
This guide covers three key topics: touch handling, push notifications, and battery optimization.

---

## 1. Touches (Input.touches) — Working with Touch Screen
Mobile devices don't have a mouse, so all navigation is based on touches, gestures, and multi-touch.

### 📌 Main Structures
| Structure | Description |
| --- | --- |
| `Touch` | Stores information about a single touch |
| `TouchPhase` | Touch phase (Began, Moved, Stationary, Ended, Canceled) |
| `Input.touches` | Array of all active touches |
| `Input.touchCount` | Number of active touches |

### 🔄 Touch Phases
```csharp
public enum TouchPhase
{
    Began,      // Finger just touched the screen
    Moved,      // Finger is moving
    Stationary, // Finger is stationary but still on screen
    Ended,      // Finger lifted from screen
    Canceled    // Touch canceled by system (e.g., screen lock)
}
```

### 📝 Basic Example: Single Touch Handling
```csharp
using UnityEngine;

public class TouchHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Debug.Log($"Touch began at: {touch.position}");
                    OnTouchBegan(touch.position);
                    break;
                    
                case TouchPhase.Moved:
                    Debug.Log($"Movement: {touch.deltaPosition}");
                    OnTouchMoved(touch.deltaPosition);
                    break;
                    
                case TouchPhase.Ended:
                    Debug.Log("Touch ended");
                    OnTouchEnded();
                    break;
                    
                case TouchPhase.Canceled:
                    Debug.Log("Touch canceled by system");
                    OnTouchCanceled();
                    break;
            }
        }
    }
    
    private void OnTouchBegan(Vector2 position) { /* Touch start logic */ }
    private void OnTouchMoved(Vector2 delta) { /* Movement logic */ }
    private void OnTouchEnded() { /* End logic */ }
    private void OnTouchCanceled() { /* Cancel logic */ }
}
```

### 🖱️ Multi-touch: Handling Multiple Fingers
```csharp
public class MultiTouchHandler : MonoBehaviour
{
    void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            
            Debug.Log($"Finger {touch.fingerId} - phase: {touch.phase}");
            
            if (Input.touchCount == 2)
            {
                HandlePinchZoom();
            }
        }
    }
    
    private void HandlePinchZoom()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);
        
        Vector2 prevPos0 = touch0.position - touch0.deltaPosition;
        Vector2 prevPos1 = touch1.position - touch1.deltaPosition;
        
        float prevDistance = Vector2.Distance(prevPos0, prevPos1);
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);
        
        float delta = currentDistance - prevDistance;
        
        Camera.main.orthographicSize -= delta * 0.01f;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2f, 10f);
    }
}
```

### 👆 Swipes and Gestures
```csharp
public class SwipeDetector : MonoBehaviour
{
    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;
    private float _minSwipeDistance = 50f;
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                _startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _endTouchPosition = touch.position;
                DetectSwipe();
            }
        }
    }
    
    private void DetectSwipe()
    {
        Vector2 swipeVector = _endTouchPosition - _startTouchPosition;
        float swipeDistance = swipeVector.magnitude;
        
        if (swipeDistance < _minSwipeDistance) return;
        
        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            if (swipeVector.x > 0)
                Debug.Log("Swipe RIGHT");
            else
                Debug.Log("Swipe LEFT");
        }
        else
        {
            if (swipeVector.y > 0)
                Debug.Log("Swipe UP");
            else
                Debug.Log("Swipe DOWN");
        }
    }
}
```

---

## 2. Push Notifications
Push notifications allow the app to interact with users even when it's not active.

### 🏗️ Push Notification Architecture
```text
[Your Server] → [FCM (Android) / APNs (iOS)] → [User Device] → [Your App]
```

### 🔧 Setup in Unity (Mobile Notifications Package)
```csharp
using UnityEngine;
using Unity.Notifications;
using Unity.Notifications.Android;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
```

### 📱 Android: Setting Up and Sending Notifications
```csharp
public class AndroidNotificationManager : MonoBehaviour
{
    [SerializeField] private string _channelId = "game_channel";
    [SerializeField] private string _channelName = "Game Notifications";
    [SerializeField] private string _channelDescription = "Important game events";
    
    void Start()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = _channelId,
            Name = _channelName,
            Description = _channelDescription,
            Importance = Importance.High,
            EnableVibration = true,
            EnableLights = true
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        
        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        if (notificationIntentData != null)
        {
            Debug.Log($"App opened from notification: {notificationIntentData.Notification.Title}");
            HandleNotification(notificationIntentData.Notification);
        }
    }
    
    public void SendNotification(string title, string text, int delaySeconds)
    {
        var notification = new AndroidNotification()
        {
            Title = title,
            Text = text,
            FireTime = System.DateTime.Now.AddSeconds(delaySeconds),
            SmallIcon = "icon_0",
            LargeIcon = "icon_1",
            ShouldAutoCancel = true,
            ShowTimestamp = true,
            Color = Color.red
        };
        
        var id = AndroidNotificationCenter.SendNotification(notification, _channelId);
        Debug.Log($"Notification sent with ID: {id}");
    }
    
    public void CancelAllNotifications()
    {
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        Debug.Log("All notifications canceled");
    }
    
    private void HandleNotification(AndroidNotification notification) { }
}
```

### 🍎 iOS: Setting Up Notifications
```csharp
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class IOSNotificationManager : MonoBehaviour
{
    void Start()
    {
#if UNITY_IOS
        iOSNotificationCenter.RequestAuthorization(AuthorizationOptions.Alert | AuthorizationOptions.Badge | AuthorizationOptions.Sound);
        
        var notification = iOSNotificationCenter.GetLastRespondedNotification();
        if (notification != null)
        {
            Debug.Log($"Opened from notification: {notification.Identifier}");
            HandleNotification(notification);
        }
#endif
    }
    
    public void SendLocalNotification(string title, string body, int delaySeconds)
    {
#if UNITY_IOS
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = delaySeconds,
            Repeats = false
        };
        
        var notification = new iOSNotification()
        {
            Identifier = System.Guid.NewGuid().ToString(),
            Title = title,
            Body = body,
            Subtitle = "Game is waiting for you!",
            Trigger = timeTrigger,
            Sound = "default",
            Badge = 1,
            ShowInForeground = true
        };
        
        iOSNotificationCenter.ScheduleNotification(notification);
        Debug.Log("iOS notification scheduled");
#endif
    }
    
    public void ClearBadge()
    {
#if UNITY_IOS
        iOSNotificationCenter.ApplicationBadge = 0;
#endif
    }
    
    private void HandleNotification(iOSNotification notification) { }
}
```

### 🔄 Cross-platform Notification Manager
```csharp
public class CrossPlatformNotificationManager : MonoBehaviour
{
    public static CrossPlatformNotificationManager Instance;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    public void ScheduleNotification(string title, string message, int delaySeconds)
    {
#if UNITY_ANDROID
        GetComponent<AndroidNotificationManager>().SendNotification(title, message, delaySeconds);
#elif UNITY_IOS
        GetComponent<IOSNotificationManager>().SendLocalNotification(title, message, delaySeconds);
#else
        Debug.Log($"Notification (skipped in editor): {title} - {message}");
#endif
    }
    
    public void CancelAll()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
    }
}
```

---

## 3. Battery Optimization 🔋
Mobile devices run on batteries, so energy efficiency is critically important.

### ⚡ Key Energy Saving Principles
| Factor | Impact | Solution |
| --- | --- | --- |
| CPU | High | Optimize Update(), use Object Pooling |
| GPU | Very High | Reduce polygon count, draw calls |
| Network | Medium | Batch requests, use WebSockets |
| Animation | Medium | Reduce animation FPS in background |
| Screen | Very High | Reduce brightness, frame rate in background |

### 📉 Frame Rate Optimization in Background
```csharp
public class BatteryOptimizer : MonoBehaviour
{
    private bool _isInBackground = false;
    
    void OnApplicationPause(bool pauseStatus)
    {
        _isInBackground = pauseStatus;
        
        if (pauseStatus)
        {
            Application.targetFrameRate = 15;
            QualitySettings.vSyncCount = 0;
            Debug.Log("Entering power saving mode");
        }
        else
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            Debug.Log("Returning to full performance");
        }
    }
    
    void Update()
    {
        if (_isInBackground) return;
        // Main logic
    }
}
```

### 🎮 Optimizing Update() and Coroutines
```csharp
// ❌ BAD: heavy operations every frame
void Update()
{
    for (int i = 0; i < 1000; i++)
    {
        // Searching objects, calculations...
    }
}

// ✅ GOOD: throttling operations
private float _nextUpdateTime = 0f;
private float _updateInterval = 0.1f; // 10 times per second

void Update()
{
    if (Time.time >= _nextUpdateTime)
    {
        _nextUpdateTime = Time.time + _updateInterval;
        PerformHeavyOperation();
    }
}

// ✅ EVEN BETTER: use coroutines with yield
IEnumerator OptimizedUpdate()
{
    while (true)
    {
        PerformHeavyOperation();
        yield return new WaitForSeconds(0.1f);
    }
}

void Start()
{
    StartCoroutine(OptimizedUpdate());
}
```

### 🌙 Sleep Mode and Screen Lock
```csharp
public class ScreenPowerManager : MonoBehaviour
{
    [SerializeField] private bool _keepScreenOn = true;
    
    void Start()
    {
        if (_keepScreenOn)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Debug.Log("Screen will not turn off");
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else if (_keepScreenOn)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
```

### 📦 Object Pooling for CPU/GC Savings
```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _poolSize = 20;
    
    private Queue<GameObject> _pool = new Queue<GameObject>();
    
    void Start()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_prefab);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
    
    public GameObject GetObject()
    {
        if (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(_prefab);
            obj.SetActive(true);
            return obj;
        }
    }
    
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}
```

### 🌐 Network Request Optimization
```csharp
public class NetworkOptimizer : MonoBehaviour
{
    private float _lastRequestTime = 0f;
    private float _requestInterval = 5f;
    
    void Update()
    {
        if (Time.time - _lastRequestTime >= _requestInterval)
        {
            _lastRequestTime = Time.time;
            SendBatchRequest();
        }
    }
    
    private void SendBatchRequest()
    {
        Debug.Log("Sending batch request");
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            _requestInterval = 30f;
        }
        else
        {
            _requestInterval = 5f;
        }
    }
}
```

### 🧪 Profiling Tools
| Tool | Purpose |
| --- | --- |
| Unity Profiler | CPU/GPU/Memory/Rendering |
| Frame Debugger | Draw call analysis |
| Memory Profiler | Memory leaks |
| Android Profiler (Android Studio) | CPU/GPU/Network/Battery |
| Xcode Instruments (iOS) | Energy consumption, thermal monitoring |

### 📊 Battery Optimization Recommendations
1. Reduce FPS in background — 30 frames is enough for menus, 60 only for active gameplay
2. Use GPU Instancing for identical objects
3. Disable unnecessary components (Rigidbody, ParticleSystem) in background
4. Batch network requests — each request drains the battery
5. Avoid frequent `Camera.main` calls — cache references
6. Use sprite atlases to reduce draw calls
7. Disable physics for off-camera objects (`Rigidbody.Sleep()`)

---

### ⭐ If this project was useful, put a star on GitHub!
