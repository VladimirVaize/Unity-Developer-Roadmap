# 🧪 Юнит-тестирование в Unity: Test Framework, Edit Mode vs Play Mode тесты, NUnit

Юнит-тестирование — это процесс проверки отдельных компонентов кода (методов, классов) на корректность их работы. 
В Unity этот процесс реализован через пакет Unity Test Framework, который базируется на библиотеке NUnit. 
Он позволяет автоматизировать проверку кода как в редакторе, так и на целевых платформах.

---

## 1. NUnit — основа тестирования в Unity
NUnit — это популярный фреймворк для юнит-тестирования в .NET-экосистеме. 
Unity использует модифицированную версию NUnit со специальными расширениями для работы в игровой среде.

### 🔧 Основные атрибуты NUnit:

| Атрибут | Назначение | Пример |
| --- | --- | --- |
| `[Test]` | Обозначает метод как юнит-тест | `public void Add_TwoNumbers_ReturnsSum()` |
| `[SetUp]` | Выполняется перед каждым тестом (подготовка данных) | `public void Init() { _calculator = new Calculator(); }` |
| `[TearDown]` | Выполняется после каждого теста (очистка) | `public void Cleanup() { _calculator = null; }` |
| `[TestCase(a, b, expected)]` | Позволяет передавать параметры в тест | `[TestCase(2, 3, 5)]` |
| `[UnityTest]` | Расширенный тест с поддержкой корутин (см. ниже) | `public IEnumerator TestWithDelay()` |

### 📝 Пример обычного NUnit-теста:
```csharp
using NUnit.Framework;

public class CalculatorTests
{
    private Calculator _calculator;

    [SetUp]
    public void SetUp()
    {
        _calculator = new Calculator();
        Debug.Log("Подготовка перед тестом");
    }

    [Test]
    public void Add_PositiveNumbers_ReturnsSum()
    {
        // Arrange (подготовка)
        int a = 5;
        int b = 3;
        int expected = 8;

        // Act (действие)
        int result = _calculator.Add(a, b);

        // Assert (проверка)
        Assert.AreEqual(expected, result);
    }

    [Test]
    [TestCase(10, 4, 6)]
    [TestCase(0, 0, 0)]
    [TestCase(-5, -3, -2)]
    public void Subtract_VariousInputs_ReturnsCorrectResult(int a, int b, int expected)
    {
        int result = _calculator.Subtract(a, b);
        Assert.AreEqual(expected, result);
    }

    [TearDown]
    public void TearDown()
    {
        _calculator = null;
        Debug.Log("Очистка после теста");
    }
}
```

> [!Important]
> Методы с атрибутом `[SetUp]` и `[TearDown]` выполняются перед и после каждого теста в классе.
> Это позволяет избежать дублирования кода и обеспечивает изоляцию тестов друг от друга.

---

## 2. Edit Mode vs Play Mode тесты
Unity Test Framework разделяет тесты на два типа в зависимости от их назначения и окружения выполнения.

### 🖥️ Edit Mode тесты (Режим редактора)

| Характеристика | Описание |
| --- | --- |
| Где выполняются | Только внутри Unity Editor |
| Что могут тестировать | Код из пространств имен `UnityEditor` и `UnityEngine` |
| Как выполняются | В цикле `EditorApplication.update` |
| Поддержка корутин | Нет (обычные корутины не работают) |
| Assembly Definition | Должен иметь `"includePlatforms": ["Editor"]` |

#### Пример Edit Mode теста:
```csharp
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

public class EditorTestsExample
{
    private GameObject _testObject;

    [SetUp]
    public void Setup()
    {
        // Создаём тестовый объект в редакторе
        _testObject = new GameObject("TestObject");
    }

    [Test]
    public void Editor_CreateGameObject_ObjectExists()
    {
        // Проверяем, что объект создан
        Assert.IsNotNull(_testObject);
        Assert.AreEqual("TestObject", _testObject.name);
    }

    [UnityTest]
    public IEnumerator Editor_WaitForAssetImport_AssetExists()
    {
        // Импортируем тестовый ассет
        string path = "Assets/TestTexture.png";
        AssetDatabase.ImportAsset(path);
        
        // Пропускаем кадр, чтобы импорт завершился
        yield return null;
        
        // Проверяем, что ассет существует
        var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        Assert.IsNotNull(asset);
    }

    [TearDown]
    public void Teardown()
    {
        // Очищаем тестовый объект
        Object.DestroyImmediate(_testObject);
    }
}
```

### 🎮 Play Mode тесты (Игровой режим)

| Характеристика | Описание |
| --- | --- |
| Где выполняются | В редакторе (при нажатии Play) или в собранном Player |
| Что могут тестировать | Runtime-код (только `UnityEngine`) |
| Как выполняются | Как корутины, прикреплённые к `MonoBehaviour` |
| Поддержка корутин | Да (через `[UnityTest]` + `yield return`) |
| Assembly Definition | Без ограничений по платформам |

#### Пример Play Mode теста с физикой:
```csharp
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayModeTestsExample
{
    private GameObject _ball;
    private Rigidbody _rb;

    [UnityTest]
    public IEnumerator Physics_GravityAffectsObject_ObjectFalls()
    {
        // Arrange
        _ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _rb = _ball.AddComponent<Rigidbody>();
        _rb.useGravity = true;
        float startY = _ball.transform.position.y;

        // Act - ждём один фиксированный шаг физики
        yield return new WaitForFixedUpdate();

        // Assert
        Assert.Less(_ball.transform.position.y, startY);
    }

    [UnityTest]
    public IEnumerator Movement_ObjectMovesOverTime_PositionChanges()
    {
        // Arrange
        _ball = new GameObject("MovingBall");
        var mover = _ball.AddComponent<TestMover>();
        mover.speed = 5f;
        Vector3 startPos = _ball.transform.position;

        // Act - ждём 1 секунду
        yield return new WaitForSeconds(1f);

        // Assert
        Assert.AreNotEqual(startPos, _ball.transform.position);
    }

    [TearDown]
    public void Teardown()
    {
        // Очистка после каждого теста
        if (_ball != null)
            Object.Destroy(_ball);
    }
}
```

---

## 3. Атрибут `[UnityTest]` — ключевое расширение Unity
Этот атрибут — главное дополнение Unity к стандартному NUnit. 
Он позволяет писать тесты, которые выполняются в несколько кадров.

### 📌 Особенности `[UnityTest]`:

| Режим | Как работает | Что можно возвращать |
| --- | --- | --- |
| Edit Mode | Выполняется в цикле `EditorApplication.update` | `yield return null` (пропуск кадра) |
| Play Mode | Выполняется как корутина `MonoBehaviour` | `yield return new WaitForSeconds()`, `WaitForFixedUpdate`, `null` |

### Примеры:
```csharp
// Edit Mode: ожидание завершения фоновой задачи
[UnityTest]
public IEnumerator Editor_BackgroundTask_CompletesSuccessfully()
{
    var task = StartLongRunningEditorTask();
    
    while (!task.isDone)
    {
        yield return null; // Пропускаем кадр
    }
    
    Assert.IsTrue(task.isSuccess);
}

// Play Mode: ожидание физического обновления
[UnityTest]
public IEnumerator PlayMode_PhysicsUpdate_ChangesVelocity()
{
    var rb = new GameObject().AddComponent<Rigidbody>();
    rb.velocity = Vector3.up * 10f;
    
    yield return new WaitForFixedUpdate();
    
    Assert.AreNotEqual(Vector3.up * 10f, rb.velocity);
}
```

---

## 4. Асинхронные тесты (async/await)
Начиная с определённых версий Test Framework, поддерживаются асинхронные тесты на основе `Task`. 
Это удобно для тестирования асинхронных операций без корутин.

```csharp
using System.Threading.Tasks;
using NUnit.Framework;

public class AsyncTestsExample
{
    [Test]
    public async Task Async_DataLoading_ReturnsExpectedData()
    {
        // Arrange
        var loader = new DataLoader();
        
        // Act
        var result = await loader.LoadDataAsync("test.json");
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("expected content", result.Content);
    }
}
```

> [!Important]
> `Assert.ThrowsAsync` может блокировать главный поток и вызывать зависание редактора. Вместо этого используйте `try/catch` с проверкой.

---

## 5. Настройка тестовых сборок (Assembly Definitions)
Для организации тестов необходимо создать отдельные `.asmdef` файлы.

### 📁 Структура проекта:
```text
Assets/
├── Scripts/              # Основной код
│   ├── MyMath.cs
│   └── MyMath.asmdef
├── Tests/
│   ├── EditMode/
│   │   ├── EditModeTests.asmdef
│   │   └── MyMathEditorTests.cs
│   └── PlayMode/
│       ├── PlayModeTests.asmdef
│       └── MyMathPlayModeTests.cs
```

### 🔧 Пример PlayModeTests.asmdef:
```json
{
    "name": "PlayModeTests",
    "references": [
        "MyMath"
    ],
    "includePlatforms": [],
    "optionalUnityReferences": [
        "TestAssemblies"
    ]
}
```

> [!Important]
> Тестовая сборка не может ссылаться на `Assembly-CSharp.dll`.
> Нужно переместить тестируемый код в отдельную кастомную сборку.

---

## 6. Методы Assert (проверки)
NUnit предоставляет множество методов для проверок:

| Метод | Назначение |
| --- | --- |
| `Assert.AreEqual(expected, actual)` | Проверка равенства |
| `Assert.AreNotEqual(expected, actual)` | Проверка неравенства |
| `Assert.IsTrue(condition)` | Проверка истинности |
| `Assert.IsFalse(condition)` | Проверка ложности |
| `Assert.IsNull(object)` | Проверка на null |
| `Assert.IsNotNull(object)` | Проверка, что не null |
| `Assert.That(actual, Is.EqualTo(expected))` | Синтаксис с ограничениями |
| `Assert.Throws<T>(delegate)` | Проверка, что метод выбрасывает исключение |

---

## 7. Рекомендации по написанию тестов
1. Используйте AAA-паттерн: Arrange (подготовка) → Act (действие) → Assert (проверка)
2. Один тест — одна проверка — так проще локализовать ошибку
3. Используйте `[SetUp]` и `[TearDown]` для изоляции тестов
4. Не тестируйте сторонние библиотеки — только свой код
5. Для физики и анимации используйте `[UnityTest]` с `WaitForFixedUpdate` или `WaitForSeconds`
6. Держите тесты независимыми — порядок выполнения не должен влиять на результат

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
