# ⚙️ Паттерн «Состояние» (State Machine): Реализация конечного автомата для игрока, врага или UI

Этот материал посвящён паттерну "Состояние" (State Pattern) — одному из ключевых поведенческих паттернов в разработке игр. 
Вы узнаете, что такое конечный автомат (Finite State Machine, FSM), зачем он нужен для управления игроком, врагами и UI, 
а также как реализовать его в Unity на C#.

---

## 1. 🧠 Что такое паттерн «Состояние»?
### Для чего нужно:
Паттерн «Состояние» позволяет объекту менять своё поведение при изменении внутреннего состояния. 
Вместо огромных условных конструкций (`if` / `switch`) вы выделяете каждое состояние в отдельный класс. 
Это делает код чистым, расширяемым и легко отлаживаемым.

### Пример из жизни:
Персонаж может быть в состояниях: `Idle` (стоит), `Walk` (идёт), `Run` (бежит), `Jump` (прыгает), `Attack` (атакует). 
В каждом состоянии у него разная логика движения, доступные действия и переходы.

---

## 2. 🎮 Конечный автомат (Finite State Machine, FSM)
### Определение:
FSM — это модель поведения, которая в каждый момент времени находится ровно в одном состоянии из конечного набора. 
Переходы между состояниями происходят по событиям или условиям.

### Основные компоненты FSM:

| Компонент | Описание |
| --- | --- |
| Состояние (State) | Конкретное поведение (например, `IdleState`, `RunState`). |
| Переход (Transition) | Правило, когда и в какое состояние перейти (например, если скорость > 0 → перейти в `RunState`). |
| Событие (Event) | Триггер перехода (нажатие клавиши, достижение цели, получение урона). |
| Машина состояний (State Machine) | Контейнер, который хранит текущее состояние и управляет переходами. |

---

## 3. 🧱 Реализация в Unity (базовый пример)
### 📁 Структура классов
```csharp
// Базовый абстрактный класс для всех состояний
public abstract class IState
{
    public virtual void Enter() { }        // Что делать при входе в состояние
    public virtual void Update() { }       // Логика каждый кадр
    public virtual void FixedUpdate() { }  // Для физики
    public virtual void Exit() { }         // Что делать при выходе из состояния
}

// Простая машина состояний
public class StateMachine
{
    private IState _currentState;
    
    public void ChangeState(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }
    
    public void Update()
    {
        _currentState?.Update();
    }
    
    public void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }
}
```

### 👤 Пример: состояния игрока
```csharp
public class PlayerController : MonoBehaviour
{
    private StateMachine _stateMachine;
    
    public IdleState IdleState;
    public WalkState WalkState;
    public JumpState JumpState;
    
    private void Awake()
    {
        _stateMachine = new StateMachine();
        
        // Создаём состояния (передаём ссылку на игрока и машину)
        IdleState = new IdleState(this, _stateMachine);
        WalkState = new WalkState(this, _stateMachine);
        JumpState = new JumpState(this, _stateMachine);
    }
    
    private void Start()
    {
        _stateMachine.ChangeState(IdleState);
    }
    
    private void Update()
    {
        _stateMachine.Update();
    }
    
    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }
}

// Конкретное состояние: Idle
public class IdleState : IState
{
    private PlayerController _player;
    private StateMachine _fsm;
    
    public IdleState(PlayerController player, StateMachine fsm)
    {
        _player = player;
        _fsm = fsm;
    }
    
    public override void Enter()
    {
        Debug.Log("Вошли в Idle");
        // Остановить анимацию движения
    }
    
    public override void Update()
    {
        float input = Input.GetAxis("Horizontal");
        
        if (Mathf.Abs(input) > 0.1f)
        {
            _fsm.ChangeState(_player.WalkState);
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            _fsm.ChangeState(_player.JumpState);
        }
    }
}

// Аналогично реализуются WalkState, JumpState и т.д.
```

---

## 4. 🎯 Применение для разных сущностей
### 🧝 Игрок (Player)
Состояния: `Idle`, `Walk`, `Run`, `Jump`, `Fall`, `Attack`, `Hurt`, `Die`
```csharp
// Пример перехода по условию (в AttackState)
public override void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        _fsm.ChangeState(_player.AttackState);
    }
}
```

### 👾 Враг (Enemy)
Состояния: `Patrol` (патруль), `Chase` (преследование), `Attack`, `Hurt`, `Die`
```csharp
public class ChaseState : IState
{
    public override void Update()
    {
        // Двигаемся к игроку
        Vector3 direction = (_player.transform.position - _enemy.transform.position).normalized;
        _enemy.transform.position += direction * _enemy.speed * Time.deltaTime;
        
        // Если близко → атаковать
        if (Vector3.Distance(_enemy.transform.position, _player.transform.position) < _enemy.attackRange)
        {
            _fsm.ChangeState(_enemy.AttackState);
        }
        
        // Если потеряли игрока из виду → вернуться в патруль
        if (!_enemy.CanSeePlayer())
        {
            _fsm.ChangeState(_enemy.PatrolState);
        }
    }
}
```

### 🖥️ UI (меню, экраны)
Состояния: `MainMenu`, `SettingsMenu`, `PauseMenu`, `GameOverMenu`, `InGameUI`
```csharp
public class UIManager : MonoBehaviour
{
    private StateMachine _uiStateMachine;
    
    private void Start()
    {
        _uiStateMachine.ChangeState(new MainMenuState(this));
    }
    
    public void OnPausePressed()
    {
        _uiStateMachine.ChangeState(new PauseMenuState(this));
    }
}
```

---

## 5. 🚀 Продвинутые техники
### 🔁 Переходы на основе событий (Event-driven Transitions)
Вместо проверок в `Update()` используйте события из других систем:
```csharp
// В HealthComponent
public event Action OnHealthZero;

void TakeDamage()
{
    health--;
    if (health <= 0) OnHealthZero?.Invoke();
}

// В состоянии HurtState
public override void Enter()
{
    _player.Health.OnHealthZero += () => _fsm.ChangeState(_player.DieState);
}
```

### 🧩 Иерархические конечные автоматы (Hierarchical FSM)
Состояния могут содержать вложенные автоматы. Например, `GroundState` (на земле) включает `Idle`, `Walk`, `Run`, а `AirState` — `Jump`, `Fall`, `DoubleJump`.

### 📦 Готовые решения для Unity

| Название | Описание |
| --- | --- |
| Unity Animator (Mecanim) | Встроенный конечный автомат для анимаций (параметры, переходы, слои). |
| State Machine Behaviour | Скрипты, привязанные к состояниям аниматора. |
| GitHub: Runtime State Machine | Популярные реализации (например, `State Machine` от Inspiaaa). |

---

## 6. ✅ Плюсы и минусы паттерна

| Плюсы | Минусы |
| --- | --- |
| ✅ Чистый код (нет спагетти из `if/else`) | ❌ Увеличение количества классов |
| ✅ Легко добавлять новые состояния | ❌ Небольшой оверхед на вызовы |
| ✅ Удобное тестирование каждого состояния отдельно | ❌ Может быть избыточным для простых объектов |
| ✅ Наглядная архитектура | ❌ Сложность с большим количеством переходов |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
