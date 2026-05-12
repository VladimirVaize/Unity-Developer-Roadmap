# 📝 Задача: Инспектор для «Умной двери» (SmartDoor)

### Контекст:
У вас есть компонент `SmartDoor`, который может открываться вручную, автоматически при приближении игрока или по таймеру. 
В стандартном инспекторе все поля видны всегда — это сбивает с толку. Вам нужно создать Custom Editor, который:
1. Покажет поле выбора `doorMode` (режим двери: ручной, по приближению, таймер).
2. Динамически будет отображать только релевантные поля в зависимости от выбранного режима.
3. Добавит кнопку «Предпросмотр в Scene View» — при нажатии она выделяет дверь и выводит в консоль её режим.
4. Если скорость открытия (`openSpeed`) меньше или равна нулю, показывает красную ошибку.

### Структура `SmartDoor` (упрощённая):
```csharp
public enum DoorMode { Manual, Proximity, Timed }

public class SmartDoor : MonoBehaviour
{
    public DoorMode doorMode = DoorMode.Manual;
    
    // Для ручного режима — не используется, для остальных — да
    public float openSpeed = 2f;
    
    // Для режима Proximity
    public float detectionRadius = 3f;
    
    // Для режима Timed
    public float autoCloseDelay = 5f;
}
```

### Требования:
- Режим Manual → показывать только `doorMode` (и скрыть `openSpeed`, `detectionRadius`, `autoCloseDelay`).
- Режим Proximity → показывать `doorMode`, `openSpeed`, `detectionRadius`.
- Режим Timed → показывать `doorMode`, `openSpeed`, `autoCloseDelay`.
- Кнопка «Предпросмотр в Scene View» — под полями.
- Предупреждение (HelpBox) красного цвета, если `openSpeed <= 0` и при этом `doorMode != DoorMode.Manual`.

Подсказка: Используйте `EditorGUILayout.PropertyField()` для рисования полей и метод `serializedObject.FindProperty()`. 
Чтобы скрыть поля, можно использовать `EditorGUILayout.PropertyField(...)` только при нужных условиях, либо `EditorGUI.indentLevel` для улучшения внешнего вида.

После реализации убедитесь, что Undo/Redo работает (благодаря `SerializedObject` и `ApplyModifiedProperties()`).

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
