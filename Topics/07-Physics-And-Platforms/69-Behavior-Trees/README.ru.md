# 🌳 Поведенческие деревья (Behavior Trees) в Unity: NodeCanvas, Odin + NPBehave, самописные решения
Поведенческие деревья — это мощный инструмент для создания AI в играх. 
В отличие от конечных автоматов (FSM), они модульны, 
легко масштабируются и позволяют создавать сложное, предсказуемое поведение без «лапши» из кода. 
В этой статье разберём три подхода: использование готовых ассетов (NodeCanvas), 
связку Odin Inspector + NPBehave для визуального редактирования, и написание самописного решения с нуля.

---

## 1. Что такое Behavior Tree? Основные понятия
Поведенческое дерево — это иерархическая структура узлов (nodes). 
Каждый узел возвращает своему родителю один из трёх статусов выполнения:

| Статус | Описание |
| --- | --- |
| Success | Узел выполнил свою задачу успешно |
| Failure | Узел не смог выполнить задачу |
| Running | Узлу нужно несколько кадров для выполнения (движение, анимация, ожидание) |

### Типы узлов:
| Тип | Описание | Примеры |
| --- | --- | --- |
| Composite | Имеют несколько детей, управляют порядком их выполнения | `Sequence`, `Selector`, `Parallel` |
| Decorator | Имеют одного ребёнка, изменяют его результат или поведение | `Inverter`, `Repeat`, `UntilFail` |
| Leaf (Action / Condition) | Листовые узлы, выполняют действие или проверку | `MoveTo`, `Attack`, `IsHealthLow` |
| Root | Корневой узел, точка входа в дерево | `Root` |

### Основные композиты:
- `Sequence` (И) : Выполняет детей по порядку. Если ребёнок вернул `Failure` — вся последовательность прерывается и возвращает `Failure`. Если все дети вернули `Success` — возвращает `Success`.
- `Selector` (ИЛИ) : Выполняет детей по порядку. Если ребёнок вернул `Success` — весь `Selector` завершается с `Success`. Если все дети вернули `Failure` — возвращает `Failure`.
- `Parallel` (Параллельный) : Запускает всех детей одновременно. Завершается, когда все дети завершились (или по первому успеху/ошибке — в зависимости от настроек).

---

## 2. Ассеты: NodeCanvas (Платный, де-факто стандарт)
NodeCanvas — самый популярный платный ассет для визуального создания поведения. 
Поддерживает Behavior Trees, Finite State Machines и Dialogs. Стоит около $45–60.

### Ключевые особенности NodeCanvas:
| Особенность | Описание |
| --- | --- |
| Визуальный редактор | Drag-and-drop узлы, создание связей, инспектор для настройки параметров |
| Blackboard | Хранилище данных (переменные), доступное всем узлам дерева (Int, Float, Vector3, GameObject, и др.) |
| Dynamic (Reactive) | Узлы могут прерывать выполнение при изменении условий (реактивное поведение) |
| Graph Reference | Возможность вкладывать одно дерево в другое (Nested BT) |
| Debugging | Цветовая индикация статуса узлов в реальном времени (зелёный — Success, красный — Failure, жёлтый — Running) |

### Пример настройки в NodeCanvas:
1. Добавьте на объект компонент `BehaviourTreeOwner`.
2. Нажмите `Open Behaviour Tree` и создайте узлы.
3. В `Blackboard` добавьте переменную `TargetPosition` (Vector3).
4. Постройте дерево:
   - `Sequence` → `Conditional` (Check Health) → `Action` (Move To Position, используя Blackboard переменную)
  
5. В `Action` узле выберите `Use Blackboard Variable` для Target Position.
6. Включите опцию `Dynamic` на `Sequence`, чтобы дерево мгновенно реагировало на изменение условий.

### Пример кастомного ActionTask (скрипт для NodeCanvas):
```csharp
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("MyActions")]
public class MoveToTarget : ActionTask<Transform>
{
    public BBParameter<Vector3> targetPosition; // Связь с Blackboard
    public float speed = 5f;

    protected override void OnExecute()
    {
        // Вызывается при старте узла
    }

    protected override void OnUpdate()
    {
        // Вызывается каждый кадр, пока узел в статусе Running
        Vector3 newPos = Vector3.MoveTowards(agent.position, targetPosition.value, speed * Time.deltaTime);
        agent.position = newPos;

        if (Vector3.Distance(agent.position, targetPosition.value) < 0.1f)
        {
            EndAction(true); // Success
        }
    }
}
```

---

## 3. Связка Odin Inspector + NPBehave (Бесплатно, визуальный редактор)
NPBehave — бесплатная библиотека поведенческих деревьев на C# (код-ориентированный подход). 
Odin Inspector — платный ассет для расширения редактора Unity. 
Их связка позволяет создать визуальный редактор для NPBehave.

### Почему Odin + NPBehave?
- NPBehave лёгкий, производительный, поддерживает черные доски (Blackboards) и всё необходимое для BT.
- Odin позволяет создавать кастомные окна редактора, инспекторы и сериализацию.
- Вместе — получаем визуальный редактор поведения без покупки NodeCanvas.

### Реализация (концепт):
1. Расширение класса NodeData: Создайте базовый класс для хранения данных узла.
2. Canvas UI: Используйте Odin для отрисовки узлов на кастомном окне редактора.
3. Сериализация: Сохраняйте дерево в JSON или ScriptableObject.
4. Генерация кода или Runtime парсинг: При старте игры парсите данные и стройте NPBehave дерево.

```csharp
// Пример структуры данных для узла (Odin-совместимая)
[System.Serializable]
public class NodeData
{
    public string nodeName;
    public Vector2 position;
    public List<NodeData> children = new List<NodeData>();
}

[CreateAssetMenu(fileName = "BehaviourTree", menuName = "AI/Behaviour Tree")]
public class BehaviourTreeAsset : ScriptableObject
{
    public List<NodeData> nodes = new List<NodeData>();
}
```

> [!Important]
> Готового ассета «Odin + NPBehave» в Asset Store нет — это архитектурный подход, который вы реализуете сами.
> Либо используйте готовые решения типа «NodeCanvas» или «Behaviour Tree Designer».

---

## 4. Самописное решение на C# (Бесплатно, полный контроль)
Написание собственного фреймворка BT даёт 100% контроль над производительностью и функционалом.

### Базовая архитектура:
```csharp
public enum NodeStatus
{
    Success,
    Failure,
    Running
}

public abstract class Node
{
    protected NodeStatus status;
    public NodeStatus Status => status;
    
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    
    public NodeStatus Evaluate()
    {
        if (status != NodeStatus.Running)
            OnEnter();
        
        status = OnUpdate();
        
        if (status != NodeStatus.Running)
            OnExit();
        
        return status;
    }
    
    protected abstract NodeStatus OnUpdate();
}
```

### Композитные узлы:

### Sequence (И):
```csharp
public class Sequence : Node
{
    private List<Node> children = new List<Node>();
    private int currentChild = 0;
    
    public Sequence(params Node[] nodes)
    {
        children.AddRange(nodes);
    }
    
    protected override NodeStatus OnUpdate()
    {
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return NodeStatus.Success;
        }
        
        NodeStatus childStatus = children[currentChild].Evaluate();
        
        if (childStatus == NodeStatus.Failure)
        {
            currentChild = 0;
            return NodeStatus.Failure;
        }
        
        if (childStatus == NodeStatus.Success)
        {
            currentChild++;
            return currentChild < children.Count ? NodeStatus.Running : NodeStatus.Success;
        }
        
        return NodeStatus.Running;
    }
}
```

### Selector (ИЛИ):
```csharp
public class Selector : Node
{
    private List<Node> children = new List<Node>();
    private int currentChild = 0;
    
    public Selector(params Node[] nodes)
    {
        children.AddRange(nodes);
    }
    
    protected override NodeStatus OnUpdate()
    {
        while (currentChild < children.Count)
        {
            NodeStatus childStatus = children[currentChild].Evaluate();
            
            if (childStatus == NodeStatus.Success)
            {
                currentChild = 0;
                return NodeStatus.Success;
            }
            
            if (childStatus == NodeStatus.Running)
                return NodeStatus.Running;
            
            currentChild++;
        }
        
        currentChild = 0;
        return NodeStatus.Failure;
    }
}
```

### Действие (Action):
```csharp
public class MoveTo : Node
{
    private Transform agent;
    private Vector3 target;
    private float speed;
    
    public MoveTo(Transform agent, Vector3 target, float speed)
    {
        this.agent = agent;
        this.target = target;
        this.speed = speed;
    }
    
    protected override NodeStatus OnUpdate()
    {
        agent.position = Vector3.MoveTowards(agent.position, target, speed * Time.deltaTime);
        
        if (Vector3.Distance(agent.position, target) < 0.1f)
            return NodeStatus.Success;
        
        return NodeStatus.Running;
    }
}
```

### Использование:
```csharp
public class EnemyAI : MonoBehaviour
{
    private Node behaviourTree;
    
    void Start()
    {
        behaviourTree = new Sequence(
            new Condition(() => Health > 0),
            new Selector(
                new Condition(() => IsPlayerInRange),
                new MoveTo(transform, player.position, 5f)
            ),
            new Attack()
        );
    }
    
    void Update()
    {
        behaviourTree.Evaluate();
    }
}
```

### Blackboard (Чёрная доска) — общая память для узлов:
```csharp
public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();
    
    public void Set<T>(string key, T value) => data[key] = value;
    public T Get<T>(string key) => data.ContainsKey(key) ? (T)data[key] : default;
    public bool HasKey(string key) => data.ContainsKey(key);
}
```

---

## 5. Сравнение подходов

| Критерий | NodeCanvas | Odin + NPBehave | Самописное решение |
| --- | --- | --- | --- |
| Цена | $45–60 | $45 (Odin) + бесплатно | Бесплатно |
| Визуальный редактор | ✅ Мощный, готовый | 🔧 Нужно делать самим | ❌ Нет (только код) |
| Готовность к использованию | ✅ Из коробки | 🔧 Требует настройки | 🔧 Требует написания |
| Производительность | Хорошая | Отличная | Отличная (оптимизированная) |
| Гибкость | Ограничена API | Высокая (полный код) | Максимальная | 
| Debugging | ✅ Визуальный, цвета | 🔧 Нужно реализовать | 🔧 Нужно реализовать |
| Поддержка | Официальная, документация | Сообщества NPBehave | Своя |

---

## 6. Рекомендации по выбору
- NodeCanvas — если нужен мощный визуальный редактор «из коробки» и вы готовы платить.
- Odin + NPBehave — если у вас уже есть Odin, или вам нужен визуальный редактор за минимальную цену (только время на разработку).
- Самописное решение — если вы хотите полный контроль, не хотите платить, или вам нужна максимальная производительность.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
