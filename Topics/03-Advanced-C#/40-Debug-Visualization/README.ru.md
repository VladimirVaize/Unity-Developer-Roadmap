# 🛠️ Визуализация отладки в Unity: Gizmos и отладочная отрисовка

> [!Note]
> При разработке игр часто нужно «увидеть» невидимое: траектории полёта снарядов, зоны поражения, радиусы атак, направления сил, границы коллайдеров или работу ИИ.
> Unity предоставляет мощные инструменты визуальной отладки, которые работают прямо в окне Scene View (и иногда в Game View).
> Это Gizmos и методы отрисовки отладочных линий. Они не влияют на финальную сборку (если код правильно организован) и незаменимы при разработке.

---

## 🔷 1. Gizmos (Рисование в редакторе)

`Gizmos` — это класс для рисования визуальных подсказок (сфер, кубов, лучей, линий) в окне Scene View. 
Код рисует Gizmos только внутри редактора Unity (не в собранной игре), если вы используете методы в специальных событиях.

### 📌 Основные методы Gizmos:

| Метод                                                   | Описание                               |
| ---                                                     | ---                                    |
| `Gizmos.DrawWireSphere(Vector3 position, float radius)` | Рисует прозрачную (каркасную) сферу    |
| `Gizmos.DrawSphere(Vector3 position, float radius)`     | Рисует сплошную сферу (полупрозрачную) |
| `Gizmos.DrawWireCube(Vector3 center, Vector3 size)`     | Каркасный куб                          |
| `Gizmos.DrawCube(Vector3 center, Vector3 size)`         | Сплошной куб                           |
| `Gizmos.DrawLine(Vector3 from, Vector3 to)`             | Линия от точки до точки                |
| `Gizmos.DrawRay(Vector3 from, Vector3 direction)`       | Луч от точки в направлении             |
| `Gizmos.DrawIcon(Vector3 position, string iconName)`    | Рисует иконку (например, "light.png")  |
| `Gizmos.DrawFrustum(...)`                               | Рисует пирамиду обзора камеры          |

### 🧠 Событие `OnDrawGizmos()`
Это специальный метод, который Unity вызывает автоматически каждый кадр в редакторе (не в игре!) для всех скриптов, у которых он определён.
```csharp
using UnityEngine;

public class EnemyGizmos : MonoBehaviour
{
    public float detectionRadius = 5f;
    public Color gizmoColor = Color.red;

    private void OnDrawGizmos()
    {
        // Рисуем красную каркасную сферу вокруг врага
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
```

Результат: Вы видите в Scene View красную окружность вокруг вашего врага — это зона его «обнаружения» игрока.

### 🧠 Событие `OnDrawGizmosSelected()`
Рисует Gizmos только когда объект выделен. Это полезно, чтобы не засорять сцену тысячами подсказок.
```csharp
private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(transform.position + Vector3.up * 1f, new Vector3(2f, 2f, 2f));
}
```

Результат: Пока вы не кликнете на объект — куб не виден. Как выделили — появился зелёный куб над объектом.

---

## 🔷 2. Отладочная отрисовка линий и лучей (Runtime)
В отличие от Gizmos, методы `Debug.DrawLine` и `Debug.DrawRay` работают во время выполнения игры (Play Mode) и видны и в Scene View, и иногда в Game View (если включены Gizmos в Game). 
Они идеальны для динамической отладки: траектории полёта пули, направление скорости, вектор нормали и т.д.

### 📌 Основные методы Debug

| Метод                                                                          | Описание                                                                                            |
| ---                                                                            | ---                                                                                                 |
| `Debug.DrawLine(Vector3 start, Vector3 end, Color color, float duration)`      | Рисует линию от start до end на заданное время (в секундах). Если duration = 0, линия видна 1 кадр. |
| `Debug.DrawRay(Vector3 start, Vector3 direction, Color color, float duration)` | Рисует луч от start в направлении direction. Длина луча = magnitude(direction).                     |
| `Debug.DrawRay(transform.position, transform.forward * 10, Color.blue)`        | Пример: луч синего цвета длиной 10 вперёд от объекта.                                               |

### 🧠 Пример в скрипте движения
```csharp
void Update()
{
    Vector3 velocity = rb.velocity;
    // Рисуем красный луч направления движения на 0.1 секунды
    Debug.DrawRay(transform.position, velocity, Color.red, 0.1f);
}
```
Результат: Во время игры из центра объекта каждый кадр вылетает красный луч, указывающий текущее направление скорости.

---

## 🔷 3. Визуальные помощники в редакторе (Editor Scripting)
Для сложных инструментов вы можете создавать собственные редакторские скрипты, которые рисуют Gizmos на любых объектах. 
Но базовый способ — использовать `OnDrawGizmos` внутри `MonoBehaviour`.

### 💡 Полезные настройки:
- Gizmos в Game View: В выпадающем меню Game View → `Gizmos` → включите, тогда линии и сферы будут видны и во время игры прямо в окне Game (но осторожно, это снижает производительность).
- Цвета: Используйте `Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);` — полупрозрачный оранжевый.
- Включение/выключение: Кнопка `Gizmos` в Scene View позволяет скрыть все гигзомы.

### 🧪 Итог: когда что использовать?

| Ситуация                                                               | Инструмент                         |
| ---                                                                    | ---                                |
| Постоянная визуальная подсказка в редакторе (зона атаки, радиус сбора) | `OnDrawGizmos`                     |
| Временная линия, видимая только когда объект выделен                   | `OnDrawGizmosSelected`             |
| Отладка во время игры (траектории, лучи, столкновения)                 | `Debug.DrawLine` / `Debug.DrawRay` |
| Кастомная иконка в редакторе                                           | `Gizmos.DrawIcon`                  |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
