# 📱 Мобильные особенности в Unity: Пуш-уведомления, касания, оптимизация батареи
Разработка под мобильные платформы (Android и iOS) требует учёта специфических возможностей и ограничений устройств. 
В этом руководстве разбираются три ключевые темы: обработка касаний, отправка и получение push-уведомлений, а также оптимизация энергопотребления.

---

## 1. Касания (Input.touches) — работа с сенсорным экраном
На мобильных устройствах нет мыши, поэтому вся навигация строится на касаниях, жестах и мультитач.

### 📌 Основные структуры
| Структура | Описание |
| --- | --- |
| `Touch` | Хранит информацию об одном касании |
| `TouchPhase` | Фаза касания (Began, Moved, Stationary, Ended, Canceled) |
| `Input.touches` | Массив всех активных касаний |
| `Input.touchCount` | Количество активных касаний |

### 🔄 Фазы касания (TouchPhase)
```csharp
public enum TouchPhase
{
    Began,      // Палец только коснулся экрана
    Moved,      // Палец перемещается
    Stationary, // Палец неподвижен, но всё ещё на экране
    Ended,      // Палец оторван от экрана
    Canceled    // Касание отменено (системой, например, при блокировке экрана)
}
```

### 📝 Базовый пример: обработка одного касания
```csharp
using UnityEngine;

public class TouchHandler : MonoBehaviour
{
    void Update()
    {
        // Проверяем, есть ли касания
        if (Input.touchCount > 0)
        {
            // Берём первое касание
            Touch touch = Input.GetTouch(0);
            
            // Обрабатываем в зависимости от фазы
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Debug.Log($"Касание началось в позиции: {touch.position}");
                    OnTouchBegan(touch.position);
                    break;
                    
                case TouchPhase.Moved:
                    Debug.Log($"Перемещение: {touch.deltaPosition}");
                    OnTouchMoved(touch.deltaPosition);
                    break;
                    
                case TouchPhase.Ended:
                    Debug.Log($"Касание завершено");
                    OnTouchEnded();
                    break;
                    
                case TouchPhase.Canceled:
                    Debug.Log("Касание отменено системой");
                    OnTouchCanceled();
                    break;
            }
        }
    }
    
    private void OnTouchBegan(Vector2 position) { /* Логика начала касания */ }
    private void OnTouchMoved(Vector2 delta) { /* Логика перемещения */ }
    private void OnTouchEnded() { /* Логика завершения */ }
    private void OnTouchCanceled() { /* Логика отмены */ }
}
```

### 🖱️ Мультитач: обработка нескольких пальцев
```csharp
public class MultiTouchHandler : MonoBehaviour
{
    void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            
            // Каждый палец имеет уникальный ID
            Debug.Log($"Палец {touch.fingerId} - фаза: {touch.phase}");
            
            // Пример: масштабирование двумя пальцами (pinch)
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
        
        // Расстояние между пальцами в предыдущем и текущем кадре
        Vector2 prevPos0 = touch0.position - touch0.deltaPosition;
        Vector2 prevPos1 = touch1.position - touch1.deltaPosition;
        
        float prevDistance = Vector2.Distance(prevPos0, prevPos1);
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);
        
        float delta = currentDistance - prevDistance;
        
        // Изменяем масштаб камеры
        Camera.main.orthographicSize -= delta * 0.01f;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2f, 10f);
    }
}
```

### 👆 Свайпы и жесты
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
        
        // Определяем направление
        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            // Горизонтальный свайп
            if (swipeVector.x > 0)
                Debug.Log("Свайп ВПРАВО");
            else
                Debug.Log("Свайп ВЛЕВО");
        }
        else
        {
            // Вертикальный свайп
            if (swipeVector.y > 0)
                Debug.Log("Свайп ВВЕРХ");
            else
                Debug.Log("Свайп ВНИЗ");
        }
    }
}
```

---

## 2. Пуш-уведомления (Push Notifications)
Пуш-уведомления позволяют приложению взаимодействовать с пользователем, даже когда оно не активно.

### 🏗️ Архитектура пуш-уведомлений
```text
[Ваш сервер] → [FCM (Android) / APNs (iOS)] → [Устройство пользователя] → [Ваше приложение]
```

### 🔧 Настройка в Unity (пакет Mobile Notifications)
```csharp
using UnityEngine;
using Unity.Notifications;
using Unity.Notifications.Android;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
```

### 📱 Android: настройка и отправка уведомлений
```csharp
public class AndroidNotificationManager : MonoBehaviour
{
    [SerializeField] private string _channelId = "game_channel";
    [SerializeField] private string _channelName = "Game Notifications";
    [SerializeField] private string _channelDescription = "Important game events";
    
    void Start()
    {
        // Создаём канал уведомлений (Android 8.0+)
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
        
        // Обработка уведомления, по которому открыли приложение
        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        if (notificationIntentData != null)
        {
            Debug.Log($"Приложение открыто из уведомления: {notificationIntentData.Notification.Title}");
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
            SmallIcon = "icon_0",     // Имя иконки в папке res/drawable
            LargeIcon = "icon_1",
            ShouldAutoCancel = true,   // Исчезает после нажатия
            ShowTimestamp = true,
            Color = Color.red
        };
        
        var id = AndroidNotificationCenter.SendNotification(notification, _channelId);
        Debug.Log($"Уведомление отправлено с ID: {id}");
    }
    
    public void CancelAllNotifications()
    {
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        Debug.Log("Все уведомления отменены");
    }
    
    private void HandleNotification(AndroidNotification notification)
    {
        // Переход на нужную сцену, загрузка контента и т.д.
    }
}
```

### 🍎 iOS: настройка уведомлений
```csharp
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class IOSNotificationManager : MonoBehaviour
{
    void Start()
    {
#if UNITY_IOS
        // Запрос разрешения
        iOSNotificationCenter.RequestAuthorization(AuthorizationOptions.Alert | AuthorizationOptions.Badge | AuthorizationOptions.Sound);
        
        // Получение уведомления, по которому открыли приложение
        var notification = iOSNotificationCenter.GetLastRespondedNotification();
        if (notification != null)
        {
            Debug.Log($"Открыто из уведомления: {notification.Identifier}");
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
            Subtitle = "Игра ждёт вас!",
            Trigger = timeTrigger,
            Sound = "default",
            Badge = 1,
            ShowInForeground = true
        };
        
        iOSNotificationCenter.ScheduleNotification(notification);
        Debug.Log("iOS уведомление запланировано");
#endif
    }
    
    public void ClearBadge()
    {
#if UNITY_IOS
        iOSNotificationCenter.ApplicationBadge = 0;
#endif
    }
    
    private void HandleNotification(iOSNotification notification)
    {
        // Обработка нажатия на уведомление
    }
}
```

### 🔄 Кроссплатформенный менеджер уведомлений
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
        Debug.Log($"Уведомление (пропущено в редакторе): {title} - {message}");
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

## 3. Оптимизация батареи 🔋
Мобильные устройства работают от батареи, поэтому энергоэффективность критически важна.

### ⚡ Основные принципы энергосбережения
| Фактор | Влияние | Решение |
| --- | --- | --- |
| CPU | Высокое | Оптимизировать Update(), использовать Object Pooling |
| GPU | Очень высокое | Уменьшить количество полигонов, дроу-коллов |
| Сеть | Среднее | Пакетировать запросы, использовать WebSockets |
| Анимация | Среднее | Уменьшить FPS анимации в фоне |
| Экран | Очень высокое | Уменьшать яркость, частоту кадров в фоне |

### 📉 Оптимизация частоты кадров в фоне
```csharp
public class BatteryOptimizer : MonoBehaviour
{
    private bool _isInBackground = false;
    
    void OnApplicationPause(bool pauseStatus)
    {
        _isInBackground = pauseStatus;
        
        if (pauseStatus)
        {
            // Уходим в фон: снижаем нагрузку
            Application.targetFrameRate = 15;
            QualitySettings.vSyncCount = 0;
            Debug.Log("Переход в энергосберегающий режим");
        }
        else
        {
            // Возвращаемся в игру
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            Debug.Log("Возврат к полноценной работе");
        }
    }
    
    void Update()
    {
        if (_isInBackground)
        {
            // Не выполняем тяжёлые вычисления в фоне
            return;
        }
        
        // Основная логика
    }
}
```

### 🎮 Оптимизация Update() и корутин
```csharp
// ❌ ПЛОХО: тяжёлые операции в каждом кадре
void Update()
{
    for (int i = 0; i < 1000; i++)
    {
        // Поиск объектов, расчёты...
    }
}

// ✅ ХОРОШО: разреживание операций
private float _nextUpdateTime = 0f;
private float _updateInterval = 0.1f; // 10 раз в секунду

void Update()
{
    if (Time.time >= _nextUpdateTime)
    {
        _nextUpdateTime = Time.time + _updateInterval;
        PerformHeavyOperation();
    }
}

// ✅ ЕЩЁ ЛУЧШЕ: использовать корутины с yield
IEnumerator OptimizedUpdate()
{
    while (true)
    {
        PerformHeavyOperation();
        yield return new WaitForSeconds(0.1f); // Пауза 100 мс
    }
}

void Start()
{
    StartCoroutine(OptimizedUpdate());
}
```

### 🌙 Режим сна и блокировка экрана
```csharp
public class ScreenPowerManager : MonoBehaviour
{
    [SerializeField] private bool _keepScreenOn = true;
    
    void Start()
    {
        // Удерживать экран включённым
        if (_keepScreenOn)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Debug.Log("Экран не будет выключаться");
        }
        else
        {
            // Использовать системные настройки
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // Приложение не в фокусе — разрешаем спать
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else if (_keepScreenOn)
        {
            // Вернулись в приложение — снова не даём спать
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
```

### 📦 Object Pooling (пул объектов) для экономии CPU/GC
```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _poolSize = 20;
    
    private Queue<GameObject> _pool = new Queue<GameObject>();
    
    void Start()
    {
        // Создаём пул
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
            // Пул пуст — создаём новый (или можно расширить пул)
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

### 🌐 Оптимизация сетевых запросов
```csharp
public class NetworkOptimizer : MonoBehaviour
{
    private float _lastRequestTime = 0f;
    private float _requestInterval = 5f; // Запрос раз в 5 секунд
    
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
        // Собираем данные за интервал и отправляем одним пакетом
        Debug.Log("Отправка пакетного запроса");
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Увеличиваем интервал в фоне
            _requestInterval = 30f;
        }
        else
        {
            _requestInterval = 5f;
        }
    }
}
```

### 🧪 Инструменты профилирования
| Инструмент | Назначение |
| --- | --- |
| Unity Profiler | CPU/GPU/Memory/Rendering |
| Frame Debugger | Анализ дроу-коллов |
| Memory Profiler | Утечки памяти |
| Android Profiler (Android Studio) | CPU/GPU/Network/Battery |
| Xcode Instruments (iOS) | Энергопотребление, тепловой мониторинг |

### 📊 Рекомендации по оптимизации батареи
1. Снижайте FPS в фоне — 30 кадров достаточно для меню, 60 только для активного геймплея
2. Используйте GPU Instancing для одинаковых объектов
3. Отключайте ненужные компоненты (Rigidbody, ParticleSystem) в фоне
4. Объединяйте сетевые запросы — каждый запрос разряжает батарею
5. Избегайте частых вызовов `Camera.main` — кешируйте ссылки
6. Используйте спрайтовые атласы для снижения дроу-коллов
7. Отключайте физику для объектов вне камеры (`Rigidbody.Sleep()`)

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
