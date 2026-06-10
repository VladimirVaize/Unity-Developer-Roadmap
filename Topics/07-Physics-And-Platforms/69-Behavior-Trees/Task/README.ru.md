# 🎯 Задача: «Патрулирующий враг с реакцией на игрока»

Вы разрабатываете stealth-игру. Вам нужно реализовать AI для врага, используя поведенческое дерево. Враг должен:
1. Патрулировать между тремя заданными точками (`Transform[] patrolPoints`).
2. Реагировать на игрока: если игрок подошёл ближе чем на `5` единиц — переключиться в режим преследования.
3. Преследовать игрока, пока расстояние больше `2` единиц (атака).
4. Если потерял игрока (расстояние > 10 единиц) — вернуться к патрулированию.
5. Визуальная отладка: цвет врага меняется: зелёный = патруль, жёлтый = преследование, красный = атака.

---

## 📝 Требования к реализации:
- Реализуйте дерево, используя один из трёх подходов (на ваш выбор):
  - NodeCanvas (если есть ассет) — создайте визуальное дерево.
  - Самописное решение — напишите код узлов `Sequence`, `Selector`, `Condition`, `Action`.
  - Гибрид — используйте Odin + NPBehave или любую другую библиотеку.
 
- Blackboard: используйте для хранения `playerTransform`, `patrolIndex`, `currentState`.
- Узлы:
  - `Condition` — проверка расстояния до игрока.
  - `Action` — патрулирование (MoveToNextPatrolPoint).
  - `Action` — преследование (ChasePlayer).
  - `Decorator` — `RepeatUntilSuccess` или `Inverter` для циклического патрулирования.
 
- Управление временем: патруль должен иметь паузу `1 секунду` на точке перед движением к следующей.

---

## 🧰 Дополнительные материалы:
Ниже приведены примеры кода для старта самописного решения. Вы можете использовать их как основу.

### Класс Blackboard (общая память):
```csharp
using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();
    
    public void Set<T>(string key, T value) => data[key] = value;
    public T Get<T>(string key) => data.ContainsKey(key) ? (T)data[key] : default;
    public bool HasKey(string key) => data.ContainsKey(key);
}
```

### Базовый класс узла:
```csharp
public enum NodeStatus { Success, Failure, Running }

public abstract class Node
{
    protected NodeStatus status;
    public NodeStatus Status => status;
    protected Blackboard blackboard;
    
    public Node(Blackboard bb) { blackboard = bb; }
    
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    
    public NodeStatus Evaluate()
    {
        if (status != NodeStatus.Running) OnEnter();
        status = OnUpdate();
        if (status != NodeStatus.Running) OnExit();
        return status;
    }
    
    protected abstract NodeStatus OnUpdate();
}
```

### Пример узла действия (Action):
```csharp
public class MoveToPosition : Node
{
    private Transform agent;
    private float speed;
    private string targetKey;
    
    public MoveToPosition(Blackboard bb, Transform agent, float speed, string targetKey) : base(bb)
    {
        this.agent = agent;
        this.speed = speed;
        this.targetKey = targetKey;
    }
    
    protected override NodeStatus OnUpdate()
    {
        Vector3 target = blackboard.Get<Vector3>(targetKey);
        agent.position = Vector3.MoveTowards(agent.position, target, speed * Time.deltaTime);
        
        if (Vector3.Distance(agent.position, target) < 0.1f)
            return NodeStatus.Success;
        
        return NodeStatus.Running;
    }
}
```

---

## 🔍 Ожидаемый результат:
- Враг корректно патрулирует, останавливаясь на 1 секунду на каждой точке.
- При приближении игрока враг переключается на преследование.
- При удалении игрока (>10 м) враг возвращается к патрулированию.
- В консоль выводятся логи смены состояний.
- Визуальная отладка работает (изменение цвета).

---

## 💡 Подсказки:
- Для паузы в патруле используйте `Wait` узел.
- Для проверки расстояния — `Condition` узел с лямбдой или методом.
- Для циклического выполнения используйте `Repeat` декоратор или зацикливание в корне.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
