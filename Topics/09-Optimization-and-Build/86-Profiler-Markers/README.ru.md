# 🔬 Profiler маркеры в Unity: Кастомные маркеры в коде для точного профилирования

Profiler — это основной инструмент Unity для анализа производительности. 
Но встроенные маркеры не всегда покрывают ваш специфический код. 
Кастомные маркеры (ProfilerMarkers) позволяют добавлять свои собственные метки в Profiler, 
чтобы точно измерять время выполнения критических участков кода.

---

## 1. Что такое Profiler маркеры?
ProfilerMarker — это инструмент для измерения времени выполнения определенных участков кода. 
Он работает как "секундомер": вы ставите начало и конец измерения, и Unity записывает это в Profiler.

### 🎯 Зачем нужны кастомные маркеры?
| Задача | Как помогают маркеры |
| --- | --- |
| Оптимизация кода | Найти самые медленные методы |
| Сравнение алгоритмов | Измерить, что быстрее |
| Отладка производительности | Увидеть, где происходят просадки FPS |
| Командная работа | Понять, кто за какой код отвечает |
| CI/CD | Автоматическое тестирование производительности |

### 📊 Сравнение с другими методами:

| Метод | Точность | Влияние на производительность | В Profiler |
| --- | --- | --- | --- |
| `Stopwatch` | ⭐⭐⭐⭐⭐ | Высокое (замедляет код) | ❌ Нет |
| `Debug.Log` | ⭐ | Очень высокое | ❌ Нет |
| ProfilerMarker | ⭐⭐⭐⭐⭐ | Минимальное | ✅ Да |
| Unity Profiler (встроенный) | ⭐⭐⭐⭐ | Отсутствует | ✅ Да |

---

## 2. Старый способ: Begin/End (устаревший)
Раньше использовался подход с `Profiler.BeginSample` и `Profiler.EndSample`:
```csharp
// ❌ УСТАРЕВШИЙ ПОДХОД (не рекомендуется)
using UnityEngine.Profiling;

void Update()
{
    Profiler.BeginSample("My Method");
    // ... ваш код ...
    Profiler.EndSample();
}
```

Проблемы:
- Строковые параметры создают аллокации
- Нужно обязательно закрывать (`EndSample`), иначе ошибки
- Несколько медленнее, чем новый подход

---

## 3. Новый подход: ProfilerMarker (рекомендуемый)
```csharp
using Unity.Profiling; // ВАЖНО: новое пространство имён!

public class PerformanceTester : MonoBehaviour
{
    // Создаём маркер один раз (статический или инстансный)
    private static readonly ProfilerMarker s_UpdateMarker = 
        new ProfilerMarker("Player.Update");
    
    private static readonly ProfilerMarker s_AIUpdateMarker = 
        new ProfilerMarker("AI.Update");
    
    private static readonly ProfilerMarker s_RenderMarker = 
        new ProfilerMarker("Render.Prepare");

    void Update()
    {
        // Использование маркера
        using (s_UpdateMarker.Auto())
        {
            // Весь код внутри этого блока будет профилироваться
            UpdatePlayer();
            UpdateCamera();
        }
        
        // Или вручную
        s_AIUpdateMarker.Begin();
        UpdateAI();
        s_AIUpdateMarker.End();
        
        // Вложенные маркеры
        using (s_RenderMarker.Auto())
        {
            PrepareRenderData();
        }
    }
}
```

---

## 4. Типы маркеров и их создание
### 🏗️ Создание маркера:
```csharp
using Unity.Profiling;

public class MarkerExamples : MonoBehaviour
{
    // 1. Статический маркер (один на весь проект)
    private static readonly ProfilerMarker StaticMarker = 
        new ProfilerMarker("Static.Marker");
    
    // 2. Инстансный маркер (один на каждый объект)
    private readonly ProfilerMarker InstanceMarker = 
        new ProfilerMarker("Instance.Marker");
    
    // 3. Маркер с категорией (для фильтрации)
    private static readonly ProfilerMarker CategoryMarker = 
        new ProfilerMarker(ProfilerCategory.Scripts, "Category.Marker");
    
    // 4. Маркер с динамическим именем (через интерполяцию)
    public void DynamicMarker(string objectName)
    {
        using (new ProfilerMarker($"Dynamic.Object.{objectName}").Auto())
        {
            // Код для конкретного объекта
        }
    }
}
```

### 📂 Доступные категории:
```csharp
// ProfilerCategory — встроенные категории
ProfilerCategory.Scripts      // Скрипты
ProfilerCategory.Render       // Рендеринг
ProfilerCategory.Physics      // Физика
ProfilerCategory.Animation    // Анимация
ProfilerCategory.Audio        // Аудио
ProfilerCategory.Memory       // Память
ProfilerCategory.Network      // Сеть
ProfilerCategory.UI           // UI
ProfilerCategory.Video        // Видео
ProfilerCategory.Lighting     // Освещение
ProfilerCategory.Gui          // Старый GUI
ProfilerCategory.System       // Системные вызовы
```

---

## 5. Практические примеры
### 📊 Пример 1: Профилирование загрузки сцены
```csharp
using Unity.Profiling;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static readonly ProfilerMarker LoadSceneMarker = 
        new ProfilerMarker("Scene.Load");
    
    private static readonly ProfilerMarker LoadResourcesMarker = 
        new ProfilerMarker("Scene.LoadResources");
    
    private static readonly ProfilerMarker InitObjectsMarker = 
        new ProfilerMarker("Scene.InitObjects");
    
    public void LoadScene(string sceneName)
    {
        using (LoadSceneMarker.Auto())
        {
            // Загрузка сцены
            SceneManager.LoadScene(sceneName);
            
            using (LoadResourcesMarker.Auto())
            {
                // Загрузка ресурсов
                LoadAllResources();
            }
            
            using (InitObjectsMarker.Auto())
            {
                // Инициализация объектов
                InitializeGameObjects();
                SpawnEnemies();
            }
        }
    }
    
    private void LoadAllResources() { /* ... */ }
    private void InitializeGameObjects() { /* ... */ }
    private void SpawnEnemies() { /* ... */ }
}
```

### 🔄 Пример 2: Профилирование алгоритмов
```csharp
using Unity.Profiling;

public class AlgorithmTester : MonoBehaviour
{
    private static readonly ProfilerMarker BubbleSortMarker = 
        new ProfilerMarker("Algorithms.BubbleSort");
    
    private static readonly ProfilerMarker QuickSortMarker = 
        new ProfilerMarker("Algorithms.QuickSort");
    
    private static readonly ProfilerMarker MergeSortMarker = 
        new ProfilerMarker("Algorithms.MergeSort");
    
    public void CompareSorts(int[] data)
    {
        // Тестируем пузырьковую сортировку
        int[] bubbleData = (int[])data.Clone();
        using (BubbleSortMarker.Auto())
        {
            BubbleSort(bubbleData);
        }
        
        // Тестируем быструю сортировку
        int[] quickData = (int[])data.Clone();
        using (QuickSortMarker.Auto())
        {
            QuickSort(quickData, 0, quickData.Length - 1);
        }
        
        // Тестируем сортировку слиянием
        int[] mergeData = (int[])data.Clone();
        using (MergeSortMarker.Auto())
        {
            MergeSort(mergeData, 0, mergeData.Length - 1);
        }
    }
    
    private void BubbleSort(int[] arr) { /* ... */ }
    private void QuickSort(int[] arr, int left, int right) { /* ... */ }
    private void MergeSort(int[] arr, int left, int right) { /* ... */ }
}
```

### 🎮 Пример 3: Профилирование игровой логики
```csharp
using Unity.Profiling;
using UnityEngine.AI;

public class GameLoopProfiler : MonoBehaviour
{
    private static readonly ProfilerMarker UpdateEnemiesMarker = 
        new ProfilerMarker("GameLoop.UpdateEnemies");
    
    private static readonly ProfilerMarker UpdatePlayerMarker = 
        new ProfilerMarker("GameLoop.UpdatePlayer");
    
    private static readonly ProfilerMarker UpdateProjectilesMarker = 
        new ProfilerMarker("GameLoop.UpdateProjectiles");
    
    private static readonly ProfilerMarker PathfindingMarker = 
        new ProfilerMarker("GameLoop.Pathfinding");
    
    void Update()
    {
        // Игровой цикл с детальным профилированием
        using (UpdatePlayerMarker.Auto())
        {
            UpdatePlayer();
        }
        
        using (UpdateEnemiesMarker.Auto())
        {
            foreach (var enemy in enemies)
            {
                using (PathfindingMarker.Auto())
                {
                    enemy.NavAgent.SetDestination(playerPosition);
                }
                enemy.UpdateBehavior();
            }
        }
        
        using (UpdateProjectilesMarker.Auto())
        {
            UpdateProjectiles();
        }
    }
}
```

### 🎨 Пример 4: Профилирование рендеринга
```csharp
using Unity.Profiling;
using UnityEngine.UI;

public class UIRenderProfiler : MonoBehaviour
{
    private static readonly ProfilerMarker UIMarker = 
        new ProfilerMarker(ProfilerCategory.UI, "UI.Render");
    
    private static readonly ProfilerMarker TextUpdateMarker = 
        new ProfilerMarker("UI.TextUpdate");
    
    private static readonly ProfilerMarker ImageUpdateMarker = 
        new ProfilerMarker("UI.ImageUpdate");
    
    void LateUpdate()
    {
        using (UIMarker.Auto())
        {
            // Обновление всех UI элементов
            using (TextUpdateMarker.Auto())
            {
                UpdateAllTexts();
            }
            
            using (ImageUpdateMarker.Auto())
            {
                UpdateAllImages();
            }
            
            // Canvas будет пересобран автоматически
        }
    }
    
    private void UpdateAllTexts()
    {
        // Сложные операции с текстом
        foreach (var text in FindObjectsOfType<Text>())
        {
            text.text = GetUpdatedText(text.name);
        }
    }
    
    private void UpdateAllImages()
    {
        // Обновление спрайтов
        foreach (var image in FindObjectsOfType<Image>())
        {
            image.sprite = GetUpdatedSprite(image.name);
        }
    }
}
```

---

## 6. Продвинутые техники
### 🔄 Вложенные маркеры (Nested Markers)
```csharp
public class NestedMarkers : MonoBehaviour
{
    private static readonly ProfilerMarker OuterMarker = 
        new ProfilerMarker("Outer.Method");
    
    private static readonly ProfilerMarker InnerMarker = 
        new ProfilerMarker("Inner.Method");
    
    void ComplexMethod()
    {
        using (OuterMarker.Auto())
        {
            // Внешний код
            
            using (InnerMarker.Auto())
            {
                // Внутренний код (будет показан внутри Outer)
                DoHeavyWork();
            }
            
            // Ещё внешний код
        }
    }
}
```

### ⏱️ Условное профилирование
```csharp
public class ConditionalProfiling : MonoBehaviour
{
    private static readonly ProfilerMarker HeavyMarker = 
        new ProfilerMarker("Heavy.Operation");
    
    private static readonly ProfilerMarker LightMarker = 
        new ProfilerMarker("Light.Operation");
    
    public bool isDebugMode = false;
    
    void Update()
    {
        if (isDebugMode)
        {
            using (HeavyMarker.Auto())
            {
                // Только в debug режиме
                HeavyOperation();
            }
        }
        else
        {
            using (LightMarker.Auto())
            {
                LightOperation();
            }
        }
    }
}
```

### 📈 Сбор статистики через ProfilerRecorder
```csharp
using Unity.Profiling;
using Unity.Profiling.LowLevel;

public class ProfilerRecorderExample : MonoBehaviour
{
    private ProfilerRecorder _recorder;
    private float _averageTime;
    
    void Start()
    {
        // Создаём рекордер для маркера
        _recorder = ProfilerRecorder.StartNew(
            ProfilerCategory.Scripts,
            "My.Custom.Marker",
            30 // Количество семплов для усреднения
        );
    }
    
    void Update()
    {
        // Получаем среднее время выполнения
        if (_recorder.Valid)
        {
            _averageTime = _recorder.LastValue / 1000f; // микросекунды → миллисекунды
            Debug.Log($"Average time: {_averageTime:F2} ms");
        }
    }
    
    void OnDestroy()
    {
        _recorder.Dispose();
    }
}
```

### 🧵 Профилирование в многопоточном коде
```csharp
using Unity.Profiling;
using System.Threading.Tasks;

public class MultithreadedProfiling : MonoBehaviour
{
    private static readonly ProfilerMarker ThreadMarker = 
        new ProfilerMarker("Thread.Work");
    
    void Start()
    {
        // Профилирование в параллельных задачах
        Parallel.For(0, 10, i =>
        {
            using (ThreadMarker.Auto())
            {
                // Каждая задача будет профилироваться отдельно
                DoWork(i);
            }
        });
    }
    
    void DoWork(int id)
    {
        // Имитация работы
        System.Threading.Thread.Sleep(100);
    }
}
```

---

## 7. Отладка маркеров
### 🖥️ Просмотр маркеров в Profiler
1. Открыть Window → Analysis → Profiler
2. Выбрать вкладку CPU Usage
3. В Hierarchy будут видны ваши маркеры:
   - Player.Update
   - GameLoop.UpdateEnemies
   - Algorithms.QuickSort
   - и т.д.
  
### 🔍 Фильтрация по категориям
```csharp
// Маркеры с категорией автоматически группируются
private static readonly ProfilerMarker PhysicsMarker = 
    new ProfilerMarker(ProfilerCategory.Physics, "Physics.Custom");
```

### 📱 Просмотр на мобильных устройствах
```csharp
// Включаем профилирование для мобильных сборок
#if UNITY_ANDROID || UNITY_IOS
    // Активируем маркеры даже в релизных сборках
    // (нужно в Player Settings включить Development Build)
#endif
```

---

## 8. Лучшие практики
### ✅ Рекомендации:
1. Создавайте маркеры статически — избегайте создания новых маркеров в Update
2. Используйте `using` блоки — они автоматически закрывают маркер
3. Давайте осмысленные имена — иерархия с точками: `Category.Subcategory.Method`
4. Используйте категории — для фильтрации в Profiler
5. Не профилируйте микро-операции — маркеры имеют накладные расходы
6. Удаляйте маркеры перед релизом — или используйте условную компиляцию

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Создание маркера в Update (аллокации)
void Update()
{
    var marker = new ProfilerMarker("Update"); // ПЛОХО!
    using (marker.Auto()) { /* ... */ }
}

// ✅ ПРАВИЛЬНО: Статический маркер
private static readonly ProfilerMarker UpdateMarker = 
    new ProfilerMarker("Update");

// ❌ ОШИБКА: Забыли закрыть маркер
void BadMethod()
{
    Profiler.BeginSample("Bad"); // Устаревший метод
    // ... если здесь exception, маркер не закроется
    Profiler.EndSample();
}

// ✅ ПРАВИЛЬНО: using блок или try/finally
void GoodMethod()
{
    using (var marker = new ProfilerMarker("Good").Auto())
    {
        // Автоматически закроется
    }
}

// ❌ ОШИБКА: Слишком детальное профилирование
void Update()
{
    using (new ProfilerMarker("EveryFrame").Auto()) // Каждый кадр!
    {
        // Сотни маркеров в кадре замедляют игру
    }
}

// ✅ ПРАВИЛЬНО: Профилируйте только важные участки
void Update()
{
    if (someCondition) // Профилируем только при необходимости
    {
        using (ConditionalMarker.Auto()) { /* ... */ }
    }
}
```

### 🏷️ Конвенции именования:
| Шаблон | Пример | Применение |
| --- | --- | --- |
| `Category.Method` | `AI.Pathfinding` | Простое разделение |
| `Module/Subsystem.Method` | `Gameplay/Combat.CalculateDamage` | Иерархия модулей |
| `ObjectName.Method` | `PlayerController.Update` | Для конкретных объектов |
| `System.Threading` | `Physics.Async` | Для многопоточности |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
