# 🛠️ Custom Editor / PropertyDrawer: Расширяем редактор Unity под свои задачи (Editor API)

Unity Editor — мощный инструмент, но его стандартные интерфейсы не всегда удобны для сложных или повторяющихся компонентов. 
Механики Custom Editor и PropertyDrawer позволяют создавать собственные визуальные элементы в окне Inspector. 
Это ускоряет разработку, уменьшает ошибки настройки и даёт полный контроль над пользовательским опытом внутри редактора.

> Весь код для этих расширений должен лежать в папке `Editor` (или её подпапках), чтобы Unity не включала его в финальную сборку игры.

---

## 📦 1. PropertyDrawer — кастомизация отображения одного поля
### Когда использовать:
Вы хотите изменить то, как одно поле (или простой тип, например `Vector2`) отображается в инспекторе, или добавить валидацию, кнопку, слайдер.

### Базовый принцип:
Создаётся класс, наследующий `PropertyDrawer`. С помощью атрибута `[CustomPropertyDrawer(typeof(Тип))]` он связывается с определённым типом данных. 
Затем переопределяется метод `OnGUI()` для рисования поля.

### 🔧 Пример: Drawer для `Vector2` с дополнительной меткой «Direction»
```csharp
// Файл: Assets/Scripts/Editor/DirectionVectorDrawer.cs
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Vector2))]
public class DirectionVectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Начинаем изменения
        EditorGUI.BeginProperty(position, label, property);
        
        // Находим дочерние поля x и y
        SerializedProperty xProp = property.FindPropertyRelative("x");
        SerializedProperty yProp = property.FindPropertyRelative("y");
        
        // Создаём горизонтальную линию
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label.text + " (направление)");
        
        // Смещаемся вниз
        Rect fieldsRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.BeginChangeCheck();
        
        float oldX = xProp.floatValue;
        float oldY = yProp.floatValue;
        
        Vector2 newValue = EditorGUI.Vector2Field(fieldsRect, "", new Vector2(oldX, oldY));
        
        if (EditorGUI.EndChangeCheck())
        {
            xProp.floatValue = newValue.x;
            yProp.floatValue = newValue.y;
        }
        
        EditorGUI.EndProperty();
    }
    
    // Сколько места займёт этот drawer в инспекторе (высота)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2; // два ряда
    }
}
```

Результат: Поле `Vector2` в инспекторе будет занимать две строки: сверху подпись «… (направление)», снизу поля X и Y.

---

## 🧩 2. Custom Editor — полная перестройка инспектора для компонента
### Когда использовать:
Вы хотите полностью изменить интерфейс инспектора для конкретного компонента (MonoBehaviour). 
Например, добавить кнопки, вкладки, предупреждения, скрыть ненужные поля или динамически менять видимость свойств.

### Базовый принцип:
Класс наследует `Editor`. Атрибут `[CustomEditor(typeof(МойКомпонент))]` привязывает его. Переопределяется `OnInspectorGUI()` — здесь рисуется весь интерфейс.

### 🧪 Пример: Инспектор для компонента игрока с кнопкой прыжка
```csharp
// Файл: Assets/Scripts/Player.cs (обычный MonoBehaviour)
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public bool isGrounded = true;
}

// Файл: Assets/Scripts/Editor/PlayerEditor.cs
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Получаем ссылку на компонент
        Player player = (Player)target;
        
        // Стандартные поля можно нарисовать по-отдельности, чтобы контролировать порядок
        EditorGUILayout.LabelField("⚙️ Основные настройки", EditorStyles.boldLabel);
        player.speed = EditorGUILayout.FloatField("Скорость", player.speed);
        player.jumpForce = EditorGUILayout.FloatField("Сила прыжка", player.jumpForce);
        
        // Рисуем поле isGrounded, но только для чтения (обычно его меняет код, не редактор)
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("На земле", player.isGrounded);
        EditorGUI.EndDisabledGroup();
        
        // Добавляем разделитель
        EditorGUILayout.Space(10);
        
        // Пользовательская кнопка — вызывает метод прыжка прямо в редакторе (тестовый режим)
        if (GUILayout.Button("🏃‍♂️ Тестовый прыжок (в редакторе)"))
        {
            Debug.Log($"{player.name} прыгнул бы с силой {player.jumpForce}");
            // Здесь можно добавить эффекты или анимацию в Scene View
        }
        
        // Предупреждение, если сила прыжка равна 0
        if (Mathf.Approximately(player.jumpForce, 0f))
        {
            EditorGUILayout.HelpBox("Сила прыжка равна 0 — персонаж не сможет прыгать!", MessageType.Warning);
        }
        
        // Сохраняем изменения, чтобы Unity их запомнила
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
```

### Результат:
- Поля скорости и силы прыжка редактируются.
- Поле `isGrounded` только для чтения.
- Появляется кнопка, которая пишет в консоль.
- Если `jumpForce == 0`, показывается жёлтое предупреждение.

---

## 🧰 3. Дополнительные возможности Editor API

| Функция | Описание |
|---------------------------|-------------------------------------------|
| `EditorGUILayout.PropertyField()` | Нарисовать стандартное поле из сериализованного свойства (SerializedProperty) |
| `EditorGUILayout.Slider()` | Слайдер для float с min/max |
| `EditorGUILayout.ObjectField()` | Поле для назначения ссылки на другой объект/ассет |
| `EditorGUILayout.EnumPopup()` | Выпадающий список для enum |
| `EditorGUILayout.HelpBox()` | Информационный блок (info/warning/error) |
| `GUILayout.Button()` + `SceneView` API | Рисовать кнопки в сцене (например, «Создать врага здесь») |
| `[InitializeOnLoad]` | Статический конструктор, вызываемый при запуске редактора |
| `EditorApplication.playModeStateChanged` | Реакция на вход/выход из Play Mode |

---

## 🧠 Рекомендации
1. Всегда проверяйте `EditorGUI.changed` и вызывайте `EditorUtility.SetDirty()` — иначе Unity может не сохранить изменения.
2. Используйте `SerializedProperty` вместо прямого доступа к полям — это обеспечивает поддержку Undo/Redo и множественного выделения.
3. Не кладите Editor-код в папки, которые попадают в сборку (только в `Editor`).
4. Не забывайте про высоту (GetPropertyHeight) в PropertyDrawer, если ваше поле занимает больше одной строки.

---

## 🔗 Где искать дальше
- Официальная документация Unity: <a href="https://docs.unity3d.com/Manual/editor-CustomEditors.html">Custom Editors</a>
- <a href="https://docs.unity3d.com/ScriptReference/PropertyDrawer.html">PropertyDrawer</a>

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
