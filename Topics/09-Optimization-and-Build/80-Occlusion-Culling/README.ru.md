# 👁️ Окклюзия (Occlusion Culling) в Unity: Отключение рендера объектов, скрытых другими объектами
Окклюзия (Occlusion Culling) — это техника оптимизации рендеринга, 
при которой Unity отключает отрисовку объектов, полностью перекрытых другими объектами с точки зрения камеры. 
Это позволяет значительно снизить количество рисуемых полигонов и 
увеличить производительность, особенно в сценах с высоким уровнем детализации.

> [!Important]
> ⚠️ Важное отличие: Frustum Culling отключает объекты за пределами видимости камеры,
> а Occlusion Culling — объекты, которые находятся в пределах видимости, но скрыты другими объектами (например, комната за стеной).

---

## 1. Принцип работы Occlusion Culling
Occlusion Culling работает в два этапа:

| Этап | Описание |
| --- | --- |
| 1. Бейкинг (Baking) | Предварительный расчёт видимости для всей сцены. Unity разбивает сцену на ячейки и строит "граф видимости" между ними |
| 2. Runtime (выполнение) | В реальном времени камера проверяет, какие объекты должны быть видны, а какие — скрыты, и отключает рендер скрытых |

### 📊 Визуальный пример:
```text
Без Occlusion Culling:          С Occlusion Culling:
                               
    Камера                          Камера
      ↓                                ↓
   🏠 Стена                        🏠 Стена
      ↓                                ↓
   💣 Сундук (рисуется)            💣 Сундук (НЕ рисуется)
   👾 Монстр (рисуется)             👾 Монстр (НЕ рисуется)
```

---

## 2. Настройка Occlusion Culling в Unity
### 🛠️ Шаг 1: Настройка объектов для окклюзии
Чтобы объект закрывал другие (был окклюдером):
- Должен быть статическим (галочка `Static` в инспекторе)
- Иметь `MeshRenderer` с включённым `Cast Shadows`

Чтобы объект был скрыт (окклюди):
- Должен быть статическим
- Иметь `MeshRenderer`
```csharp
// Пример: динамическое переключение Static флага для окклюзии
using UnityEngine;

public class OcclusionHelper : MonoBehaviour
{
    void Start()
    {
        // Делаем объект статическим для участия в бейкинге окклюзии
        gameObject.isStatic = true;
        
        // Если объект должен быть окклюдером, включаем тени
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}
```

> [!Important]
> Только объекты с флагом `Static` участвуют в бейкинге окклюзии. Динамические объекты (движущиеся) не учитываются, но могут быть скрыты статическими окклюдерами.

### 🛠️ Шаг 2: Окно Occlusion Culling
Откройте: Window → Rendering → Occlusion Culling

Окно содержит 3 вкладки:
| Вкладка | Назначение |
| --- | --- |
| Object | Настройка параметров окклюзии для выбранных объектов |
| Bake | Настройки бейкинга и кнопка `Bake` |
| Visualization | Визуализация результатов бейкинга в сцене |

### 🛠️ Шаг 3: Настройка бейкинга (Bake)
Во вкладке Bake настройте параметры:
| Параметр | Описание | Рекомендация |
| --- | --- | --- |
| Smallest Occluder | Минимальный размер объекта, который может закрывать другие | 1-5 единиц |
| Smallest Hole | Минимальный размер "дырки", через которую можно увидеть объект | 0.25-1 единицы |
| Backface Threshold | Точность учёта задних граней | 100 (по умолчанию) |

```csharp
// Настройка параметров бейкинга через скрипт (Editor-only)
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class OcclusionSettings : MonoBehaviour
{
    [ContextMenu("Configure Occlusion Bake")]
    void ConfigureBake()
    {
        var staticOcclusionCulling = UnityEditor.Rendering.StaticOcclusionCulling;
        var settings = staticOcclusionCulling.occlusionCullingData;
        
        // Установка параметров
        settings.smallestOccluder = 2.5f;
        settings.smallestHole = 0.5f;
        settings.backfaceThreshold = 100;
        
        EditorUtility.SetDirty(settings);
    }
}
#endif
```

### 🛠️ Шаг 4: Запуск бейкинга
Нажмите кнопку Bake в окне Occlusion Culling. Процесс может занять от нескольких секунд до нескольких минут в зависимости от сложности сцены.

После бейкинга в папке `Assets/` появится файл `OcclusionCullingData.asset`.

---

## 3. Примеры использования
### 🏠 Пример 1: Лабиринт с комнатами
```csharp
using UnityEngine;

public class MazeOcclusion : MonoBehaviour
{
    void Start()
    {
        // Все стены лабиринта должны быть статическими
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            wall.isStatic = true;
        }
        
        // Враги в комнатах будут автоматически скрыты, если их не видно
        Debug.Log("Лабиринт настроен для Occlusion Culling");
    }
    
    void OnDrawGizmos()
    {
        // Визуализация ячеек окклюзии в редакторе
        #if UNITY_EDITOR
        if (UnityEditor.Rendering.StaticOcclusionCulling.occlusionCullingData != null)
        {
            Gizmos.color = Color.green;
            // Можно отобразить ячейки (сложно, требует низкоуровневого доступа)
        }
        #endif
    }
}
```

### 🎮 Пример 2: Большой открытый мир (с окклюзией)
```csharp
public class OpenWorldOcclusion : MonoBehaviour
{
    public Camera playerCamera;
    public float occlusionCheckInterval = 0.5f;
    
    private float lastCheckTime;
    
    void Update()
    {
        // Динамическое управление видимостью для нестатических объектов
        if (Time.time - lastCheckTime > occlusionCheckInterval)
        {
            lastCheckTime = Time.time;
            CheckDynamicOcclusion();
        }
    }
    
    void CheckDynamicOcclusion()
    {
        // Для динамических объектов, не участвующих в бейкинге,
        // можно написать свою простую окклюзию через Physics.Raycast
        GameObject[] dynamicObjects = GameObject.FindGameObjectsWithTag("DynamicProp");
        
        foreach (GameObject obj in dynamicObjects)
        {
            Vector3 direction = obj.transform.position - playerCamera.transform.position;
            float distance = direction.magnitude;
            
            // Проверяем, есть ли препятствие между камерой и объектом
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, direction.normalized, out hit, distance))
            {
                if (hit.collider.gameObject != obj)
                {
                    // Объект скрыт другим объектом
                    obj.SetActive(false);
                }
                else
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                obj.SetActive(true);
            }
        }
    }
}
```

### 🚪 Пример 3: Двери и динамические окклюдеры
```csharp
public class DynamicOccluderDoor : MonoBehaviour
{
    private bool isOpen = false;
    private MeshRenderer doorRenderer;
    
    void Start()
    {
        doorRenderer = GetComponent<MeshRenderer>();
        
        // Динамические объекты НЕ участвуют в стандартной окклюзии
        // Нужно вручную обновлять статический флаг при изменении
    }
    
    public void OpenDoor()
    {
        isOpen = true;
        transform.rotation = Quaternion.Euler(0, 90, 0);
        
        // При открытии двери, пространство за ней должно стать видимым
        // Обновляем окклюзию (требуется перебейкинг или решение через Unity)
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // В редакторе можно перебейкнуть
            UnityEditor.Rendering.StaticOcclusionCulling.GenerateInBackground();
        }
        #endif
    }
    
    public void CloseDoor()
    {
        isOpen = false;
        transform.rotation = Quaternion.identity;
        
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Rendering.StaticOcclusionCulling.GenerateInBackground();
        }
        #endif
    }
}
```

> [!Tip]
> Для динамических дверей лучше использовать Occlusion Portal или не полагаться на окклюзию полностью.

---

## 4. Occlusion Areas и Occlusion Portals
Unity предоставляет специальные компоненты для тонкой настройки окклюзии.

### 🗺️ Occlusion Area
Помечает область, где должна работать окклюзия. Это позволяет ограничить расчёты только определёнными зонами.
```csharp
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OcclusionAreaExample : MonoBehaviour
{
    void Start()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        col.isTrigger = true;
        
        // Область будет использоваться для бейкинга окклюзии
        Debug.Log($"Occlusion Area создана: {col.bounds.size}");
    }
}
```

Как настроить:
1. Добавьте компонент `Occlusion Area` к GameObject
2. Настройте размер области через Box Collider
3. В окне Occlusion Culling → Bake → `Occlusion Areas` должны быть включены

### 🚪 Occlusion Portal
Используется для "дверных проёмов" — позволяет контролировать, через какие отверстия камера может видеть.
```csharp
public class OcclusionPortalExample : MonoBehaviour
{
    public OcclusionPortal portal;
    public bool isOpen = true;
    
    void Start()
    {
        portal = GetComponent<OcclusionPortal>();
        if (portal != null)
        {
            portal.open = isOpen;
        }
    }
    
    void Update()
    {
        // Динамическое открытие/закрытие портала
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isOpen = !isOpen;
            portal.open = isOpen;
            Debug.Log($"Occlusion Portal {(isOpen ? "открыт" : "закрыт")}");
        }
    }
}
```

---

## 5. Проверка работы Occlusion Culling
### 🖥️ Визуализация в Scene View
1. Откройте Occlusion Culling окно
2. Перейдите на вкладку Visualization
3. Включите Visualize > Occlusion Culling
4. Выберите камеру в поле Camera
5. Цвета:
   - Зелёные объекты — видны (рендерятся)
   - Красные объекты — скрыты (не рендерятся)
  
### 📊 Профайлинг производительности
```csharp
using UnityEngine;

public class OcclusionProfiler : MonoBehaviour
{
    public Camera targetCamera;
    private int lastVisibleObjects = 0;
    
    void Update()
    {
        // Получение статистики (требуется доступ к внутренним API)
        int visibleObjects = CountVisibleObjects();
        
        if (visibleObjects != lastVisibleObjects)
        {
            Debug.Log($"Occlusion Culling: {visibleObjects} объектов видно (было {lastVisibleObjects})");
            lastVisibleObjects = visibleObjects;
        }
        
        // FPS индикатор
        float fps = 1.0f / Time.deltaTime;
        Debug.Log($"FPS: {fps:F1}, Видимых объектов: {visibleObjects}");
    }
    
    private int CountVisibleObjects()
    {
        // Примерная оценка (реальная статистика сложнее)
        var renderers = FindObjectsOfType<Renderer>();
        int visible = 0;
        
        foreach (var renderer in renderers)
        {
            if (renderer.isVisible)
                visible++;
        }
        
        return visible;
    }
}
```

### 🔍 Проверка через Frame Debugger
1. Window → Analysis → Frame Debugger
2. Нажмите Enable
3. Прокрутите список вызовов рендера
4. Объекты, скрытые окклюзией, будут отмечены `Occlusion culled`

---

## 6. Когда использовать Occlusion Culling
### ✅ Хорошо подходит для:
| Сценарий | Пример |
| --- | --- |
| Плотные городские среды | Улицы, где дома закрывают друг друга |
| Лабиринты и подземелья | Комнаты, соединённые коридорами |
| Архитектурные интерьеры | Офисы, замки, здания |
| Сцены с большим количеством перекрытий | Леса с густой листвой (частично) |

### ❌ Неэффективно для:
| Сценарий | Причина |
| --- | --- |
| Открытые пространства | Все объекты видны — окклюзия бесполезна | 
| Пустыни, равнины | Нет перекрытий |
| Динамически изменяемые сцены | Требуют перебейкинга |
| Очень мелкие объекты | Затраты на проверку больше выигрыша |

---

## 7. Ограничения и важные нюансы
| Ограничение | Описание | Решение |
| --- | --- | --- |
| Только статические объекты | Динамические объекты не участвуют в бейкинге | Использовать `Occlusion Portal` или кастомную логику |
| Занимает память | Данные видимости хранятся в памяти | Оптимизировать размер ячейки |
| Время бейкинга | Сложные сцены бейкатся долго | Разбивать сцену на части |
| Не работает со Skinned Mesh | Анимированные персонажи не закрывают пространство | Комбинировать со статической геометрией |
| Требует перебейкинга | При изменении геометрии | Настроить автоматический бейкинг |

```csharp
// Пример: автоматический перебейкинг при изменении сцены (Editor Script)
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoOcclusionBaker
{
    static AutoOcclusionBaker()
    {
        EditorSceneManager.sceneSaved += OnSceneSaved;
    }
    
    static void OnSceneSaved(UnityEngine.SceneManagement.Scene scene)
    {
        if (EditorUtility.DisplayDialog("Перебейкинг окклюзии", 
            "Пересчитать Occlusion Culling для этой сцены?", 
            "Да", "Нет"))
        {
            UnityEditor.Rendering.StaticOcclusionCulling.GenerateInBackground();
            Debug.Log($"Occlusion Culling перебейкен для сцены {scene.name}");
        }
    }
}
#endif
```

---

## 8. Полный пример: Оптимизация подземелья
```csharp
using UnityEngine;

public class DungeonOcclusionManager : MonoBehaviour
{
    [Header("Настройки")]
    public float bakeInterval = 10f; // Перебейкинг каждые 10 секунд (в редакторе)
    public bool enableDynamicPortals = true;
    
    private float lastBakeTime;
    
    void Start()
    {
        // Маркируем всю статическую геометрию подземелья
        MarkStaticGeometry();
        
        // Бейкинг окклюзии для стартовой сцены
        BakeOcclusion();
    }
    
    void MarkStaticGeometry()
    {
        // Находим все стены, полы, потолки и делаем их статическими
        GameObject[] staticProps = GameObject.FindGameObjectsWithTag("StaticEnvironment");
        foreach (GameObject obj in staticProps)
        {
            obj.isStatic = true;
        }
        
        Debug.Log($"Отмечено {staticProps.Length} статических объектов для окклюзии");
    }
    
    void BakeOcclusion()
    {
        #if UNITY_EDITOR
        if (enableDynamicPortals)
        {
            UnityEditor.Rendering.StaticOcclusionCulling.GenerateInBackground();
            Debug.Log("Запущен процесс бейкинга Occlusion Culling");
        }
        #endif
    }
    
    void Update()
    {
        #if UNITY_EDITOR
        // Автоматический перебейкинг при работе в редакторе
        if (Time.time - lastBakeTime > bakeInterval)
        {
            lastBakeTime = Time.time;
            BakeOcclusion();
        }
        #endif
    }
    
    // Визуализация эффективности
    void OnGUI()
    {
        int visibleObjects = CountVisibleRenderers();
        int totalObjects = FindObjectsOfType<Renderer>().Length;
        
        GUI.Box(new Rect(10, 10, 200, 60), "Occlusion Stats");
        GUI.Label(new Rect(20, 30, 180, 20), $"Visible: {visibleObjects} / {totalObjects}");
        GUI.Label(new Rect(20, 50, 180, 20), $"Culled: {totalObjects - visibleObjects}");
        
        float efficiency = (float)(totalObjects - visibleObjects) / totalObjects * 100f;
        GUI.Label(new Rect(20, 70, 180, 20), $"Efficiency: {efficiency:F1}%");
    }
    
    private int CountVisibleRenderers()
    {
        int count = 0;
        foreach (Renderer r in FindObjectsOfType<Renderer>())
        {
            if (r.isVisible) count++;
        }
        return count;
    }
}
```

---

## 9. Лучшие практики
| Практика | Почему важно |
| --- | --- |
| ✅ Маркируйте статические объекты | Без этого окклюзия не работает |
| ✅ Разбивайте большие меши | Мелкие объекты лучше окклюдятся |
| ✅ Используйте Occlusion Areas | Ограничивает зону расчётов, ускоряет бейкинг |
| ✅ Тестируйте на целевой платформе | Мобильные устройства выигрывают больше |
| ✅ Комбинируйте с Frustum Culling | Два метода дополняют друг друга |
| ❌ Не используйте для открытых пространств | Бесполезно и тратит память |
| ❌ Не делайте всё статическим | Динамика тоже нужна |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
