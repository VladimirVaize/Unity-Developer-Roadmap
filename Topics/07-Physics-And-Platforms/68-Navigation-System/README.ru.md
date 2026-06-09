# 🧭 Система навигации в Unity: NavMesh Surface, NavMesh Agent, NavMesh Obstacle

NavMesh (Navigation Mesh) — это система искусственного интеллекта в Unity, 
позволяющая персонажам автономно перемещаться по игровому миру, 
обходить препятствия и находить оптимальные маршруты. В её основе лежат три ключевых компонента.

---

## 1. NavMesh Surface — Генерация навигационной сетки
NavMesh Surface отвечает за создание навигационной сетки — «синей зоны», 
по которой могут перемещаться агенты. Эта сетка генерируется (запекается) на основе геометрии сцены.

### 🔧 Ключевые параметры:

| Параметр | Описание |
| --- | --- |
| `Agent Type` | Тип агента (размер влияет на проходимость) |
| `Default Area` | Тип области (Walkable, Not Walkable, Jump) |
| `Use Geometry` | Откуда брать геометрию: Render Meshes или Physics Colliders |
| `Layers` | Какие слои участвуют в запекании |
| `Voxel Size` | Точность сетки (чем меньше, тем точнее, но дольше) |

### 📝 Пример использования:
```csharp
using UnityEngine.AI;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface surface;

    void Start()
    {
        // Бейк NavMesh при старте (для процедурно генерируемых уровней)
        surface.BuildNavMesh();
    }

    void Update()
    {
        // Перебейк при изменении уровня (например, разрушении стены)
        if (Input.GetKeyDown(KeyCode.B))
        {
            surface.RemoveActiveSurfaces();
            surface.BuildNavMesh();
            Debug.Log("NavMesh перестроен!");
        }
    }
}
```

> [!Tip]
> Статические объекты (стены, полы) должны участвовать в запекании. Динамические объекты используют NavMeshObstacle.

---

## 2. NavMesh Agent — Управление движением персонажа
NavMesh Agent — компонент, который добавляется к персонажу и управляет его перемещением по NavMesh. 
Он автоматически рассчитывает путь, скорость, ускорение и избегает других агентов и препятствий.

### 🔧 Ключевые параметры:
| Параметр | Описание |
| --- | --- |
| `Radius` / `Height` | Размер коллайдера агента для проходимости |
| `Speed` | Максимальная скорость движения |
| `Angular Speed` | Скорость поворота |
| `Acceleration` | Ускорение |
| `Stopping Distance` | Расстояние до цели, на котором агент останавливается |
| `Auto Braking` | Автоматическое торможение перед целью |
| `Area Mask` | Какие типы областей доступны агенту |

### 📝 Пример управления агентом:
```csharp
using UnityEngine;
using UnityEngine.AI;

public class AgentMover : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform target;          // Цель для движения
    public Camera playerCamera;       // Камера для Raycast

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Настройка скорости под анимацию
        agent.speed = 5f;
        agent.stoppingDistance = 0.5f;
    }

    void Update()
    {
        // Движение к указанной цели
        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        // Клик мышью для перемещения
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }

        // Отображение статуса движения
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("Агент достиг цели!");
        }
    }

    // Остановка агента
    public void StopAgent()
    {
        agent.isStopped = true;
    }

    // Возобновление движения
    public void ResumeAgent()
    {
        agent.isStopped = false;
    }
}
```

### 📝 Пример с анимацией:
```csharp
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class AnimatedAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Передаём скорость в аниматор для Blend Tree
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("SpeedPercent", speedPercent);
    }
}
```

---

## 3. NavMesh Obstacle — Динамические препятствия
NavMesh Obstacle используется для объектов, которые могут двигаться, 
появляться или исчезать во время игры (ящики, двери, враги). 
Эти объекты должны учитываться агентами при навигации.

### 🔧 Два режима работы:
| Режим | Описание | Когда использовать |
| --- | --- | --- |
| Блокировка (Carve: off) | Агенты обходят препятствие динамически (RVO) | Постоянно движущиеся объекты (другие агенты) |
| Вырезание (Carve: on) | Препятствие вырезает "дыру" в NavMesh | Объекты, которые могут остановиться (ящики, ворота) |

### 📝 Пример настройки NavMeshObstacle:
```csharp
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class MovingObstacle : MonoBehaviour
{
    private NavMeshObstacle obstacle;
    private Vector3 startPosition;
    public float moveDistance = 3f;
    public float speed = 2f;
    private bool movingRight = true;

    void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        startPosition = transform.position;
        
        // Включаем вырезание для стационарных препятствий
        obstacle.carving = true;
        obstacle.carvingMoveThreshold = 0.5f; // Перемещение >0.5 ед. = обновление carving
    }

    void Update()
    {
        // Движение препятствия туда-обратно
        float newX = startPosition.x + (movingRight ? moveDistance : -moveDistance);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        
        if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance)
        {
            movingRight = !movingRight;
        }
    }

    void OnDisable()
    {
        // Если объект уничтожен, carving остаётся — нужно перебейк
        if (NavMeshSurface.activeSurface != null)
            NavMeshSurface.activeSurface.UpdateNavMesh();
    }
}
```

---

## 4. NavMesh Link и OffMesh Link — Специальные соединения
### 🚪 OffMesh Link
Используется для соединения двух точек, между которыми нет обычного пути: прыжки через пропасть, подъём по лестнице, открытие двери.

Настройка: создаётся пустой GameObject → добавляется компонент OffMesh Link → задаются Start и End точки.
```csharp
using UnityEngine.AI;

public class DoorLink : MonoBehaviour
{
    public OffMeshLink doorLink;
    public bool isOpen = false;

    void Update()
    {
        // При открытии двери включаем переход
        doorLink.activated = isOpen;
    }
}
```

### 🌉 NavMesh Link
Соединяет несколько отдельных NavMesh Surface (для больших открытых миров или подземелий).

---

## 5. Полный пример: Патрулирующий враг
```csharp
using UnityEngine;
using UnityEngine.AI;

public class PatrollingEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    public float patrolSpeed = 3f;
    public float chaseSpeed = 7f;
    public float chaseRange = 10f;
    public Transform player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToNextWaypoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer < chaseRange)
        {
            // Режим преследования
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            // Режим патрулирования
            agent.speed = patrolSpeed;
            
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNextWaypoint();
            }
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }
}
```

---

## 🎯 Рекомендации по оптимизации
1. Для больших уровней используйте NavMeshSurface с настройкой `Tile Size`, чтобы бейк был быстрее.
2. Динамические препятствия — включайте `Carve` только если объект может быть статическим какое-то время.
3. Area Mask — ограничьте типы областей для разных агентов (например, враги не могут заходить в воду).
4. Проверка достижения цели — используйте `agent.remainingDistance <= agent.stoppingDistance`, но проверяйте `agent.pathPending == false` и `agent.hasPath == true`.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
