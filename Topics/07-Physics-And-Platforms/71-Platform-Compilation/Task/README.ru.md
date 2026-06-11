# 🎯 Задача: «Кроссплатформенный менеджер ввода (InputManager)»
Вы разрабатываете игру, которая должна работать на Android, iOS и в Unity Editor. 
На мобильных устройствах управление происходит через касания экрана, а в редакторе — через мышь. 
Вам нужно реализовать менеджер ввода, который автоматически адаптируется под платформу.

## 📝 Шаблон кода (дополнить):
```csharp
using UnityEngine;
using UnityEngine.Events;

public class CrossPlatformInputManager : MonoBehaviour
{
    [System.Serializable]
    public class InputEvent : UnityEvent<Vector2> { }
    
    public InputEvent OnTap;          // Событие при нажатии
    public InputEvent OnDrag;         // Событие при перетаскивании
    
    private bool _isDragging = false;
    private Vector2 _lastDragPosition;
    
    void Update()
    {
        // ========== TODO: Добавить платформенную обработку ввода ==========
        // 1. Для UNITY_EDITOR: обработать Input.GetMouseButtonDown(0) и Input.GetMouseButton(0)
        // 2. Для UNITY_ANDROID и UNITY_IOS: обработать Input.touchCount и Input.GetTouch(0)
        // 3. При нажатии вызывать OnTap?.Invoke(position)
        // 4. При перетаскивании вызывать OnDrag?.Invoke(delta) и OnDrag?.Invoke(position)
        // =================================================================
    }
}
```

---

## 📋 Конкретные задачи для реализации:
1. Добавьте платформенные директивы для разделения кода редактора и мобильных устройств.
2. Реализуйте обработку в редакторе (`UNITY_EDITOR`):
   - Левый клик мыши = касание
   - Зажатая кнопка + движение = перетаскивание
   - Отпускание кнопки = завершение перетаскивания
  
3. Реализуйте обработку на мобильных устройствах (`UNITY_ANDROID` и `UNITY_IOS`):
   - Первое касание = основное взаимодействие
   - Обрабатывайте фазы касания: `Began`, `Moved`, `Ended`
  
4. Добавьте условную компиляцию для отладки:
   - В `DEVELOPMENT_BUILD` выводите подробные логи в консоль
   - В финальной сборке (`!DEVELOPMENT_BUILD`) логи должны отсутствовать
  
5. Добавьте проверку на платформу для путей сохранения:
   - На Android сохраняйте настройки в `Application.persistentDataPath`
   - На iOS используйте `Application.persistentDataPath + "/Documents/"`
   - В редакторе сохраняйте в папку `Application.dataPath`
  
6. Создайте пользовательский символ `USE_TACTILE_FEEDBACK`:
   - Если символ определён, при каждом касании вызывайте вибрацию (только на мобильных)
   - На Android используйте: `Handheld.Vibrate()`
   - На iOS используйте: `UnityEngine.iOS.Device.PlayVibration()`
   - В редакторе просто выводите лог "Vibration would play here"
  
---

## 🧰 Требования к реализации:
- Используйте минимум 4 различные платформенные директивы (`UNITY_EDITOR`, `UNITY_ANDROID`, `UNITY_IOS`, `DEVELOPMENT_BUILD`)
- Создайте пользовательский символ компиляции через Player Settings
- Все общие методы должны быть вынесены вне директив
- Добавьте комментарии на русском языке к каждому блоку `#if`

---

## 🔍 Проверка работоспособности:
1. Запустите в Unity Editor — должно работать с мышью
2. Соберите Android-версию — должно работать с касаниями
3. Соберите iOS-версию — должно работать с касаниями
4. Проверьте, что в финальной сборке нет отладочных логов
5. Проверьте, что вибрация работает (если включён `USE_TACTILE_FEEDBACK`)

---

## 💡 Ожидаемый вывод в консоли (DEVELOPMENT_BUILD):
```text
[Editor] Input initialized
[Editor] Tap at position: (342, 156)
[Editor] Dragging from (342, 156) to (345, 160)
[Editor] Drag ended
```

```text
[Android] Input initialized
[Android] Touch began at: (120, 340)
[Android] Vibration played (USE_TACTILE_FEEDBACK enabled)
[Android] Touch moved to: (125, 342)
[Android] Touch ended
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
