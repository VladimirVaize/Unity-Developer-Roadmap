# 🎯 Задача: «Система умного врага с тремя состояниями»
Вы разрабатываете стелс-экшен. Вам нужно реализовать AI врага, который имеет три режима поведения:
1. 🧘 Патрулирование (Patrol) — враг перемещается между заданными точками по NavMesh
2. 🔍 Поиск (Investigate) — враг идёт к месту, где игрок издал шум (например, бросил предмет)
3. 🏃‍♂️ Преследование (Chase) — враг преследует игрока, если увидел его

### 📁 Структура сцены:
- Территория: Лабиринт из стен с NavMesh Surface (бейк выполнен)
- Игрок: Объект с Collider и скриптом, который создаёт «шум» при нажатии Space
- Враг: Кубик с NavMesh Agent и вашим скриптом `SmartEnemy`
- Точки патрулирования: 4 пустых объекта (Waypoint1-4)

### 📝 Класс SmartEnemy — что нужно реализовать:
```csharp
public enum EnemyState { Patrol, Investigate, Chase }

public class SmartEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 3f;
    
    [Header("Chase Settings")]
    public float chaseSpeed = 6f;
    public float chaseRange = 12f;
    public float lostPlayerTime = 5f;   // Время поиска после потери игрока
    
    [Header("Investigate Settings")]
    public float investigateTime = 4f;  // Время изучения места шума
    public float investigateSpeed = 4f;
    
    [Header("References")]
    public Transform player;
    
    private NavMeshAgent agent;
    private EnemyState currentState;
    private Vector3 lastKnownPlayerPosition;
    private Vector3 investigatePoint;
    private float stateTimer;
    private int currentPatrolIndex;
}
```

### 📋 Требования к логике:
| Состояние | Условие входа | Что делает |
| --- | --- | --- |
| Patrol | По умолчанию / когда таймер investigate истёк / потерял игрока | Движется к точкам по порядку |
| Investigate | Получен «шум» от игрока (событие) | Идёт к точке шума, ждёт, затем возврат к патрулю |
| Chase | Игрок вошёл в `chaseRange` | Преследует игрока; если игрок скрылся > `lostPlayerTime` → возврат к патрулю |

### 🧰 Дополнительные требования:
1. Переключение между точками патруля — после достижения текущей точки переключаться на следующую
2. Событие шума — создайте метод `public void ReceiveNoise(Vector3 noisePosition)`, который переводит врага в режим `Investigate`
3. Визуальная отладка — выводите в консоль текущее состояние врага и его действия
4. Динамическое препятствие — добавьте на сцену движущийся ящик с `NavMeshObstacle`, который враг должен обходить

### 🔍 Ожидаемое поведение:
```text
[Enemy] State: Patrol → Moving to Waypoint 1
[Enemy] State: Patrol → Moving to Waypoint 2
[Player] Made noise at position (5, 0, 3)
[Enemy] State: Investigate → Moving to noise position (5, 0, 3)
[Enemy] State: Investigate → Arrived, investigating...
[Enemy] State: Patrol → Returning to patrol route
[Enemy] State: Chase → Player detected, chasing!
[Player] Left chase range, Enemy searching...
[Enemy] State: Chase → Lost player, returning to patrol
```

### 💡 Подсказки:
- Для проверки достижения цели используйте `agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending`
- Для ожидания в состоянии `Investigate` используйте `stateTimer` и `Time.deltaTime`
- Для прерывания состояний — проверяйте приоритеты: Chase > Investigate > Patrol

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
