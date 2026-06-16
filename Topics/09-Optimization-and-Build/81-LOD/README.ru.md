# 🎨 LOD (Level of Detail) в Unity: Смена моделей в зависимости от расстояния до камеры

Level of Detail (LOD) — это техника оптимизации, при которой для одного объекта используются разные модели с возрастающей степенью детализации в зависимости от расстояния до камеры. 
Чем дальше объект — тем менее детализированная модель используется, что значительно снижает нагрузку на GPU.

---

## 1. Что такое LOD и зачем он нужен?
### 🎯 Цели использования LOD:
| Цель | Описание |
| --- | --- |
| Оптимизация производительности | Снижение количества полигонов в кадре |
| Экономия памяти | Менее детальные модели весят меньше |
| Увеличение FPS | Особенно важно на мобильных устройствах |
| Масштабируемость | Возможность отрисовки тысяч объектов |

### 📊 Принцип работы:
```text
Камера
   │
   ├── 0-10 м   → LOD 0 (High Poly)   10 000 полигонов
   ├── 10-20 м  → LOD 1 (Medium Poly) 5 000 полигонов
   ├── 20-40 м  → LOD 2 (Low Poly)    1 000 полигонов
   └── >40 м    → LOD 3 (Impostor/отключается)
```

> [!Important]
> LOD работает на основе расстояния между камерой и точкой pivot объекта (не центром рендеринга).

---

## 2. Компонент LOD Group
Unity предоставляет компонент LOD Group, который управляет переключением уровней детализации.

### 🧩 Структура LOD Group:
| Параметр | Описание |
| --- | --- |
| LOD 0 | Самый детализированный уровень (близко к камере) |
| LOD 1 | Средняя детализация |
| LOD 2 | Низкая детализация |
| LOD 3... | Дополнительные уровни |
| Culled | Объект не отрисовывается (дальше максимального расстояния) |

### 📐 Настройка процентов:
- Проценты рассчитываются от Reference Resolution (обычно ширина экрана в пикселях)
- Рекомендуемые значения: LOD0: 60%, LOD1: 30%, LOD2: 10%, Culled: 5%

```csharp
using UnityEngine;

public class LODGroupExample : MonoBehaviour
{
    private LODGroup lodGroup;
    
    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
        
        // Установка процентов вручную
        lodGroup.SetLODs(new LOD[] {
            new LOD(0.6f, new Renderer[] { GetComponent<Renderer>() }),  // LOD0: 60%
            new LOD(0.3f, new Renderer[] { GetComponent<Renderer>() }),  // LOD1: 30%
            new LOD(0.1f, null)  // LOD2: отключаем объект
        });
        
        // Включить анимацию перехода (кроссфейд)
        lodGroup.fadeMode = LODFadeMode.CrossFade;
        lodGroup.animateCrossFading = true;
    }
}
```

---

## 3. Настройка LOD Group в редакторе
### 🛠️ Пошаговое создание LOD:
1. Создайте разные модели для каждого LOD уровня:
   - `Tree_High.fbx` (10K полигонов)
   - `Tree_Medium.fbx` (5K полигонов)
   - `Tree_Low.fbx` (1K полигонов)
  
2. Добавьте компонент LOD Group на пустой GameObject:
   - `Add Component → Rendering → LOD Group`
  
3. Настройте уровни:
   - Нажмите `+` для добавления уровня
   - Перетащите модели в соответствующие слоты
   - Настройте ползунки процентов
  
4. Настройте переходы:
   - `Fade Mode`: Cross Fade (плавный переход) или Speed Tree (для деревьев)
   - `Cross Fade Width`: ширина зоны перехода (0.1-0.5)
  
### 📝 Пример готового LOD Group в инспекторе:
```csharp
// Аналог настройки через код
public class TreeLODSetup : MonoBehaviour
{
    void Awake()
    {
        LODGroup group = gameObject.AddComponent<LODGroup>();
        
        // Получаем рендереры для каждого уровня
        Renderer highRenderer = transform.Find("HighPoly").GetComponent<Renderer>();
        Renderer mediumRenderer = transform.Find("MediumPoly").GetComponent<Renderer>();
        Renderer lowRenderer = transform.Find("LowPoly").GetComponent<Renderer>();
        
        // Создаём уровни LOD
        LOD[] lods = new LOD[3];
        lods[0] = new LOD(0.6f, new Renderer[] { highRenderer });
        lods[1] = new LOD(0.3f, new Renderer[] { mediumRenderer });
        lods[2] = new LOD(0.1f, new Renderer[] { lowRenderer });
        
        group.SetLODs(lods);
        group.RecalculateBounds();  // Пересчитать границы
    }
}
```

---

## 4. Программное управление LOD
### 🎮 Управление через скрипты:
```csharp
public class DynamicLODController : MonoBehaviour
{
    private LODGroup lodGroup;
    private float originalReferenceResolution;
    
    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
        originalReferenceResolution = lodGroup.referenceResolution;
        
        // Получить текущие LOD
        LOD[] currentLODs = lodGroup.GetLODs();
        Debug.Log($"Количество LOD уровней: {currentLODs.Length}");
        
        // Изменить процент для LOD0
        currentLODs[0].screenRelativeTransitionHeight = 0.8f;
        lodGroup.SetLODs(currentLODs);
    }
    
    // Временное увеличение детализации (например, при прицеливании)
    public void IncreaseDetailTemporarily(float duration)
    {
        StartCoroutine(TempLODBoost(duration));
    }
    
    private System.Collections.IEnumerator TempLODBoost(float duration)
    {
        LOD[] lods = lodGroup.GetLODs();
        float originalLOD0 = lods[0].screenRelativeTransitionHeight;
        
        // Увеличиваем порог LOD0 (объект дольше остаётся детальным)
        lods[0].screenRelativeTransitionHeight = 1.0f;
        lodGroup.SetLODs(lods);
        
        yield return new WaitForSeconds(duration);
        
        // Возвращаем обратно
        lods[0].screenRelativeTransitionHeight = originalLOD0;
        lodGroup.SetLODs(lods);
    }
    
    // Адаптивный LOD в зависимости от FPS
    void Update()
    {
        float currentFPS = 1.0f / Time.deltaTime;
        
        if (currentFPS < 30)
        {
            // Низкий FPS — агрессивно снижаем детализацию
            lodGroup.SetLODs(GetAggressiveLODs());
        }
        else if (currentFPS > 50)
        {
            // Высокий FPS — повышаем качество
            lodGroup.SetLODs(GetHighQualityLODs());
        }
    }
    
    private LOD[] GetAggressiveLODs()
    {
        // LOD0: только 30% экрана
        return new LOD[] {
            new LOD(0.3f, new Renderer[] { GetRendererForLOD(0) }),
            new LOD(0.1f, new Renderer[] { GetRendererForLOD(1) }),
            new LOD(0.05f, null)  // Раннее отключение
        };
    }
    
    private LOD[] GetHighQualityLODs()
    {
        return new LOD[] {
            new LOD(0.8f, new Renderer[] { GetRendererForLOD(0) }),
            new LOD(0.5f, new Renderer[] { GetRendererForLOD(1) }),
            new LOD(0.2f, new Renderer[] { GetRendererForLOD(2) })
        };
    }
    
    private Renderer GetRendererForLOD(int level)
    {
        // Логика получения рендерера для уровня
        return transform.GetChild(level).GetComponent<Renderer>();
    }
}
```

### 🎭 Режимы перехода (Fade Modes):
```csharp
public class LODFadeExamples : MonoBehaviour
{
    private LODGroup lodGroup;
    
    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
        
        // Режим 1: Cross Fade — плавное исчезновение/появление
        lodGroup.fadeMode = LODFadeMode.CrossFade;
        lodGroup.animateCrossFading = true;  // Анимировать переход
        
        // Режим 2: Speed Tree — специальный режим для деревьев
        lodGroup.fadeMode = LODFadeMode.SpeedTree;
        
        // Режим 3: Нет перехода — резкое переключение
        lodGroup.fadeMode = LODFadeMode.None;
        
        // Настройка ширины зоны перехода
        LOD[] lods = lodGroup.GetLODs();
        lods[0].fadeTransitionWidth = 0.3f;  // 30% зона перехода
        lodGroup.SetLODs(lods);
    }
}
```

---

## 5. Создание LOD уровней в редакторе
### 🎨 Рекомендации по созданию моделей:
| LOD | Полигонов | Текстуры | Вершины | Использование |
| --- | --- | --- | --- | --- |
| LOD0 | 100% | 4K | Высокое | Близкие объекты |
| LOD1 | 50-60% | 2K | Среднее | Средняя дистанция |
| LOD2 | 20-25% | 1K | Низкое | Дальние объекты |
| LOD3 | 5-10% | 256x256 | Минимальное | Очень далеко |

### 🔧 Инструменты для создания LOD:
1. Unity LOD Generator (Built-in):
   - `Window → Rendering → LOD Generator`
   - Автоматически создаёт упрощённые версии
  
2. Blender Decimate Modifier:
   - Импорт → Decimate → Уменьшить полигоны → Экспорт
  
3. Simplygon (Плагин):
   - Профессиональное автоматическое упрощение
  
```csharp
// Пример использования LOD Generator через код (Editor Script)
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.LODGenerator;

public class LODGeneratorExample
{
    [MenuItem("Tools/Generate LODs for Selection")]
    static void GenerateLODs()
    {
        GameObject selected = Selection.activeGameObject;
        LODGeneratorUtility.GenerateLODs(selected, 50f, 25f, 10f);
        Debug.Log("LOD сгенерированы для " + selected.name);
    }
}
#endif
```

---

## 6. Оптимизация LOD
### ⚡ Лучшие практики:
```csharp
public class LODOptimization : MonoBehaviour
{
    void Start()
    {
        // 1. Отключаем Shadow Casting на дальних LOD
        DisableShadowsOnFarLODs();
        
        // 2. Настраиваем Occlusion Culling
        SetupOcclusionCulling();
        
        // 3. Используем Lightmap для дальних объектов
        UseLightmapForDistantLODs();
    }
    
    void DisableShadowsOnFarLODs()
    {
        LODGroup group = GetComponent<LODGroup>();
        LOD[] lods = group.GetLODs();
        
        for (int i = 1; i < lods.Length; i++)  // Начиная с LOD1
        {
            foreach (Renderer renderer in lods[i].renderers)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }
    }
    
    void SetupOcclusionCulling()
    {
        // Для LOD объектов важно точное окклюженное отсечение
        LODGroup group = GetComponent<LODGroup>();
        group.RecalculateBounds();  // Пересчитать границы для окклюжн кулинга
    }
    
    void UseLightmapForDistantLODs()
    {
        // Для LOD2+ используем Lightmap вместо реального освещения
        LODGroup group = GetComponent<LODGroup>();
        LOD[] lods = group.GetLODs();
        
        if (lods.Length > 2)
        {
            foreach (Renderer renderer in lods[2].renderers)
            {
                renderer.lightmapIndex = 1;  // Использовать подготовленный Lightmap
                renderer.lightmapScaleOffset = Vector4.one;
            }
        }
    }
}
```

### 📊 Настройка LOD Bias в Project Settings:
```csharp
// QualitySettings влияют на поведение LOD во всём проекте
public class QualityBasedLOD : MonoBehaviour
{
    void Start()
    {
        // Чем выше качество, тем дальше держим детализацию
        if (QualitySettings.GetQualityLevel() >= 2)  // High quality
        {
            QualitySettings.lodBias = 2.0f;   // LOD переключается в 2 раза дальше
        }
        else if (QualitySettings.GetQualityLevel() == 1)  // Medium quality
        {
            QualitySettings.lodBias = 1.0f;   // Стандартное расстояние
        }
        else  // Low quality
        {
            QualitySettings.lodBias = 0.5f;   // LOD переключается ближе
        }
        
        Debug.Log($"LOD Bias установлен на: {QualitySettings.lodBias}");
    }
}
```

---

## 7. Продвинутые техники LOD
### 🎭 Impostor LOD (биллборды):
```csharp
public class ImpostorLOD : MonoBehaviour
{
    private Camera mainCamera;
    private LODGroup lodGroup;
    private GameObject impostorBillboard;
    
    void Start()
    {
        mainCamera = Camera.main;
        lodGroup = GetComponent<LODGroup>();
        
        // Создаём биллборд для дальнего LOD
        CreateImpostorBillboard();
        
        // Настраиваем LOD с биллбордом
        SetupImpostorLOD();
    }
    
    void CreateImpostorBillboard()
    {
        // Создаём плоскую текстуру-биллборд
        impostorBillboard = new GameObject("Impostor");
        impostorBillboard.transform.parent = transform;
        impostorBillboard.transform.localPosition = Vector3.zero;
        
        var billboardRenderer = impostorBillboard.AddComponent<SpriteRenderer>();
        billboardRenderer.sprite = CaptureBillboardSprite();
        billboardRenderer.color = Color.white;
    }
    
    Sprite CaptureBillboardSprite()
    {
        // Захват текущего вида для создания биллборда
        // (упрощённая версия)
        return null;
    }
    
    void SetupImpostorLOD()
    {
        LOD[] lods = new LOD[4];
        lods[0] = new LOD(0.5f, GetComponent<Renderer>());  // Обычная модель
        lods[1] = new LOD(0.2f, GetComponent<Renderer>());  // Упрощённая
        lods[2] = new LOD(0.1f, impostorBillboard.GetComponent<Renderer>());  // Биллборд
        lods[3] = new LOD(0.05f, null);  // Отключение
        
        lodGroup.SetLODs(lods);
    }
}
```

### 🔄 Анимированный LOD (время жизни объекта):
```csharp
public class AnimatedLOD : MonoBehaviour
{
    private Animator animator;
    private LODGroup lodGroup;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        lodGroup = GetComponent<LODGroup>();
        
        // Отключаем анимацию на дальних LOD
        lodGroup.onLODChanged += OnLODLevelChanged;
    }
    
    void OnLODLevelChanged(int lodLevel)
    {
        if (lodLevel >= 2)  // Дальше LOD2
        {
            animator.enabled = false;  // Отключаем анимацию
        }
        else
        {
            animator.enabled = true;
        }
    }
    
    void OnDestroy()
    {
        lodGroup.onLODChanged -= OnLODLevelChanged;
    }
}
```

---

## 8. Частые ошибки и их решение
### ❌ Ошибки:
| Ошибка | Проблема | Решение |
| --- | --- | --- |
| Мерцание при переключении | Резкое переключение LOD | Использовать `Cross Fade` |
| Пустые рендереры | Слот LOD пуст | Убедиться, что есть хоть один рендерер |
| Неверные границы | LOD переключается неправильно | Вызвать `RecalculateBounds()` |
| Слишком частые переключения | Объект на границе LOD | Увеличить `fadeTransitionWidth` |

```csharp
// Пример исправления ошибок
public class LODErrorFix : MonoBehaviour
{
    void FixCommonLODIssues()
    {
        LODGroup group = GetComponent<LODGroup>();
        
        // 1. Пересчёт границ (исправляет неправильное переключение)
        group.RecalculateBounds();
        
        // 2. Установка плавного перехода
        group.fadeMode = LODFadeMode.CrossFade;
        group.animateCrossFading = true;
        
        // 3. Проверка наличия рендереров
        LOD[] lods = group.GetLODs();
        for (int i = 0; i < lods.Length; i++)
        {
            if (lods[i].renderers == null || lods[i].renderers.Length == 0)
            {
                Debug.LogWarning($"LOD уровень {i} не имеет рендереров!");
            }
        }
        
        // 4. Увеличение ширины перехода для LOD0
        lods[0].fadeTransitionWidth = 0.3f;
        group.SetLODs(lods);
    }
}
```

---

## 9. Полный пример: LOD для леса
```csharp
public class ForestLODManager : MonoBehaviour
{
    [Header("LOD Settings")]
    public float highQualityDistance = 30f;
    public float mediumQualityDistance = 60f;
    public float lowQualityDistance = 100f;
    
    private LODGroup[] allTrees;
    
    void Start()
    {
        // Находим все деревья в лесу
        allTrees = FindObjectsOfType<LODGroup>();
        
        // Настраиваем каждое дерево
        foreach (LODGroup tree in allTrees)
        {
            SetupTreeLOD(tree);
        }
    }
    
    void SetupTreeLOD(LODGroup tree)
    {
        // Получаем рендереры дерева
        Renderer[] allRenderers = tree.GetComponentsInChildren<Renderer>();
        
        // Создаём LOD уровни на основе расстояний
        LOD[] lods = new LOD[3];
        
        // LOD0: Высокое качество (кроссфейд 0.2)
        lods[0] = new LOD(highQualityDistance / GetMaxDistance(), allRenderers);
        lods[0].fadeTransitionWidth = 0.2f;
        
        // LOD1: Среднее качество (отключаем тени)
        Renderer[] medium = GetMediumRenderers(tree);
        lods[1] = new LOD(mediumQualityDistance / GetMaxDistance(), medium);
        
        // LOD2: Низкое качество (биллборд)
        Renderer[] low = GetLowRenderers(tree);
        lods[2] = new LOD(lowQualityDistance / GetMaxDistance(), low);
        
        tree.SetLODs(lods);
        tree.fadeMode = LODFadeMode.CrossFade;
    }
    
    float GetMaxDistance()
    {
        return lowQualityDistance + 20f;
    }
    
    Renderer[] GetMediumRenderers(LODGroup tree)
    {
        // Возвращаем упрощённые модели
        return tree.GetComponentsInChildren<Renderer>();
    }
    
    Renderer[] GetLowRenderers(LODGroup tree)
    {
        // Возвращаем биллборды или очень простые модели
        return tree.GetComponentsInChildren<Renderer>();
    }
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
