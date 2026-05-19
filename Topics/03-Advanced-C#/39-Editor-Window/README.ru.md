# 🧰 Editor Window: Создание собственных окон в редакторе Unity

> [!Note]
> Этот README посвящён расширению возможностей Unity Editor. Вы узнаете, как создавать собственные окна (Editor Windows), 
> использовать EditorGUILayout для автоматической вёрстки и делать визуальные инструменты для дизайнеров (и других членов команды), 
> чтобы они могли настраивать игру без погружения в код.

> [!Important]
> Весь код для редакторских окон должен лежать в папке `Editor` (например, `Assets/Editor/`). Скрипты в этой папке не попадают в финальную сборку игры.

---

## 1. Создание собственного окна (EditorWindow)
### Для чего нужно:
Стандартные окна Unity (Inspector, Project, Hierarchy) не всегда покрывают специфические задачи вашей игры. Создавая своё окно, вы можете:
- Сгруппировать часто используемые настройки (баланс уровней, параметры врагов, диалоги).
- Автоматизировать рутинные действия (генерация уровней, импорт ассетов).
- Предоставить дизайнерам/художникам простой интерфейс без необходимости править префабы вручную.

### Как использовать:
1. Создайте C# скрипт в папке `Editor` (например, `MyToolsWindow.cs`).
2. Унаследуйте класс от `EditorWindow`.
3. Добавьте статический метод с атрибутом `[MenuItem("Tools/Моё окно")]` — он добавит пункт в верхнее меню Unity.
4. В методе вызовите `GetWindow<YourWindowType>()`.
5. Переопределите метод `OnGUI()` — здесь будет вся отрисовка интерфейса окна.

### Пример:
```csharp
using UnityEditor;
using UnityEngine;

public class MyToolsWindow : EditorWindow
{
    [MenuItem("Tools/Мой простой инструмент")]
    public static void ShowWindow()
    {
        // Открыть окно (или сфокусироваться, если уже открыто)
        GetWindow<MyToolsWindow>("Мой инструмент");
    }

    private void OnGUI()
    {
        GUILayout.Label("Привет, это моё окно!", EditorStyles.boldLabel);
        if (GUILayout.Button("Нажми меня"))
        {
            Debug.Log("Кнопка нажата!");
        }
    }
}
```

После сохранения в меню `Tools → Мой простой инструмент` появится окно с кнопкой.

---

## 2. EditorGUILayout — автоматическая вёрстка
### Для чего нужно:
`EditorGUILayout` — это набор методов для автоматического размещения UI-элементов один под другим (вертикально) или в строку (горизонтально). 
Это аналог GUILayout, но заточенный под редактор с поддержкой полей для сериализуемых данных (полей классов, которые можно сохранять между перезапусками Unity).

### Как использовать (основные методы):
- `EditorGUILayout.LabelField("Текст")` — обычная надпись.
- `EditorGUILayout.TextField("Имя:", переменная)` — строковое поле.
- `EditorGUILayout.IntField("Количество:", переменная)` — целочисленное поле.
- `EditorGUILayout.Slider("Громкость:", 0f, 1f)` — слайдер.
- `EditorGUILayout.ObjectField("Объект:", obj, typeof(GameObject), true)` — поле для перетаскивания ассета/объекта.
- `EditorGUILayout.Space()` — вертикальный отступ.
- `EditorGUILayout.BeginHorizontal()` / `EndHorizontal()` — расположить элементы в одну строку.

### Пример (окно настроек врага):
```csharp
private string enemyName = "Гоблин";
private int enemyHealth = 100;
private float enemySpeed = 3.5f;
private GameObject enemyPrefab;

private void OnGUI()
{
    EditorGUILayout.LabelField("Параметры врага", EditorStyles.boldLabel);
    
    enemyName = EditorGUILayout.TextField("Имя врага:", enemyName);
    enemyHealth = EditorGUILayout.IntField("Здоровье:", enemyHealth);
    enemySpeed = EditorGUILayout.Slider("Скорость:", enemySpeed, 0f, 10f);
    enemyPrefab = (GameObject)EditorGUILayout.ObjectField("Префаб врага:", enemyPrefab, typeof(GameObject), false);
    
    if (GUILayout.Button("Применить к выбранному врагу"))
    {
        // Логика применения параметров к объекту в сцене
        Debug.Log($"Применено к {enemyName}");
    }
}
```

---

## 3. Визуальные инструменты для дизайнеров
### Для чего нужно:
Дизайнеры, художники и геймдизайнеры часто не пишут код. 
Визуальные инструменты позволяют им настраивать игру через удобные интерфейсы 
(кнопки, поля, ползунки, цветовые палитры) прямо в редакторе.

Полезные приёмы для дизайнеров:
### 🔸 Цветовые поля
```csharp
private Color gizmoColor = Color.red;
gizmoColor = EditorGUILayout.ColorField("Цвет области:", gizmoColor);
```

### 🔸 Выпадающие списки (Enum)
```csharp
public enum Difficulty { Easy, Medium, Hard }
private Difficulty currentDifficulty = Difficulty.Medium;
currentDifficulty = (Difficulty)EditorGUILayout.EnumPopup("Сложность:", currentDifficulty);
```

### 🔸 Toggle (вкл/выкл)
```csharp
private bool enableSpawners = true;
enableSpawners = EditorGUILayout.Toggle("Включить спавнеры:", enableSpawners);
```

### 🔸 Создание кнопок для действий «Очистить всё», «Сгенерировать уровень», «Выдать всем здоровье»
```csharp
if (GUILayout.Button("Создать 10 врагов на сцене"))
{
    for (int i = 0; i < 10; i++)
        Instantiate(enemyPrefab, Random.insideUnitSphere * 10f, Quaternion.identity);
}
```

### 🔸 Сохранение данных между сессиями (EditorPrefs или SerializedObject)
```csharp
// Сохранить значение
EditorPrefs.SetFloat("MyVolume", volume);
// Загрузить значение
volume = EditorPrefs.GetFloat("MyVolume", 0.5f);
```

---

## 4. Полный пример: «Окно настройки спавна предметов»
```csharp
using UnityEditor;
using UnityEngine;

public class SpawnToolWindow : EditorWindow
{
    private GameObject itemPrefab;
    private int spawnCount = 5;
    private float spawnRadius = 10f;
    private Color gizmoColor = Color.green;

    [MenuItem("Tools/Окно спавна предметов")]
    public static void ShowWindow() => GetWindow<SpawnToolWindow>("Спавн предметов");

    private void OnEnable()
    {
        // Загружаем сохранённые значения (EditorPrefs)
        spawnCount = EditorPrefs.GetInt("SpawnCount", 5);
        spawnRadius = EditorPrefs.GetFloat("SpawnRadius", 10f);
    }

    private void OnDisable()
    {
        // Сохраняем при закрытии окна
        EditorPrefs.SetInt("SpawnCount", spawnCount);
        EditorPrefs.SetFloat("SpawnRadius", spawnRadius);
    }

    private void OnGUI()
    {
        GUILayout.Label("⚙️ Генератор предметов", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        itemPrefab = (GameObject)EditorGUILayout.ObjectField("Префаб предмета:", itemPrefab, typeof(GameObject), false);
        spawnCount = EditorGUILayout.IntSlider("Количество предметов:", spawnCount, 1, 50);
        spawnRadius = EditorGUILayout.Slider("Радиус спавна:", spawnRadius, 1f, 30f);
        gizmoColor = EditorGUILayout.ColorField("Цвет визуализации:", gizmoColor);

        EditorGUILayout.Space();

        if (itemPrefab == null)
        {
            EditorGUILayout.HelpBox("Перетащите префаб предмета в поле выше", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("🎲 Случайный спавн"))
        {
            SpawnItemsRandomly();
        }

        if (GUILayout.Button("🗑️ Удалить все предметы на сцене"))
        {
            DeleteAllSpawnedItems();
        }
    }

    private void SpawnItemsRandomly()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            Instantiate(itemPrefab, randomPos, Quaternion.identity);
        }
        Debug.Log($"Создано {spawnCount} предметов");
    }

    private void DeleteAllSpawnedItems()
    {
        // Находит все объекты с тегом "Item" (предварительно назначьте тег)
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in items)
            DestroyImmediate(item);
        Debug.Log($"Удалено {items.Length} предметов");
    }

    // Рисуем визуализацию радиуса в Scene View (для дизайнера)
    private void OnSceneGUI()
    {
        Handles.color = gizmoColor;
        Handles.DrawWireDisc(Vector3.zero, Vector3.up, spawnRadius);
    }
}
```

---

## 🔁 Как дизайнеры используют такие окна
- Открывают через Tools → ...
- Перетаскивают префабы, крутят слайдеры, нажимают кнопки.
- Мгновенно видят изменения в сцене (спавн, удаление, настройка параметров).
- Данные сохраняются между перезапусками Unity.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
