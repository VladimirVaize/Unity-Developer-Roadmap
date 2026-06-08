# 🧪 Unit Testing in Unity: Test Framework, Edit Mode vs Play Mode Tests, NUnit

Unit testing is the process of verifying individual code components (methods, classes) for correctness. 
In Unity, this is implemented through the Unity Test Framework package, which is built on the NUnit library. 
It allows you to automate code verification both in the Editor and on target platforms.

---

## 1. NUnit — The Foundation of Testing in Unity
NUnit is a popular unit testing framework in the .NET ecosystem. 
Unity uses a modified version of NUnit with special extensions for game development.

### 🔧 Key NUnit Attributes:

| Attribute | Purpose | Example |
| --- | --- | --- |
| `[Test]` | Marks a method as a unit test | `public void Add_TwoNumbers_ReturnsSum()` |
| `[SetUp]` | Runs before each test (data preparation) | `public void Init() { _calculator = new Calculator(); }` |
| `[TearDown]` | Runs after each test (cleanup) | `public void Cleanup() { _calculator = null; }` |
| `[TestCase(a, b, expected)]` | Passes parameters to a test | `[TestCase(2, 3, 5)]` |
| `[UnityTest]` | Extended test with coroutine support | `public IEnumerator TestWithDelay()` |

### 📝 Example of a Standard NUnit Test:
```csharp
using NUnit.Framework;

public class CalculatorTests
{
    private Calculator _calculator;

    [SetUp]
    public void SetUp()
    {
        _calculator = new Calculator();
        Debug.Log("Setup before test");
    }

    [Test]
    public void Add_PositiveNumbers_ReturnsSum()
    {
        // Arrange
        int a = 5;
        int b = 3;
        int expected = 8;

        // Act
        int result = _calculator.Add(a, b);

        // Assert
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
        Debug.Log("Cleanup after test");
    }
}
```

> [!Important]
> Methods with `[SetUp]` and `[TearDown]` attributes run before and after each test in the class. This helps avoid code duplication and ensures test isolation.

---

## 2. Edit Mode vs Play Mode Tests
The Unity Test Framework separates tests into two types based on their purpose and execution environment.

### 🖥️ Edit Mode Tests

| Characteristic | Description |
| --- | --- |
| Execution | Only inside Unity Editor |
| What they can test | Code from `UnityEditor` and `UnityEngine` namespaces |
| How they run | In `EditorApplication.update` loop |
| Coroutine support | No (regular coroutines don't work) |
| Assembly Definition | Must have `"includePlatforms": ["Editor"]` |

#### Example Edit Mode Test:
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
        _testObject = new GameObject("TestObject");
    }

    [Test]
    public void Editor_CreateGameObject_ObjectExists()
    {
        Assert.IsNotNull(_testObject);
        Assert.AreEqual("TestObject", _testObject.name);
    }

    [UnityTest]
    public IEnumerator Editor_WaitForAssetImport_AssetExists()
    {
        string path = "Assets/TestTexture.png";
        AssetDatabase.ImportAsset(path);
        
        yield return null;
        
        var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        Assert.IsNotNull(asset);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_testObject);
    }
}
```

### 🎮 Play Mode Tests

| Characteristic | Description |
| --- | --- |
| Execution | In Editor (Play button) or built Player |
| What they can test | Runtime code (only `UnityEngine`) |
| How they run | As coroutines attached to `MonoBehaviour` |
| Coroutine support | Yes (via `[UnityTest]` + `yield return`) |
| Assembly Definition | No platform restrictions |

#### Example Play Mode Test with Physics:
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

        // Act
        yield return new WaitForFixedUpdate();

        // Assert
        Assert.Less(_ball.transform.position.y, startY);
    }

    [UnityTest]
    public IEnumerator Movement_ObjectMovesOverTime_PositionChanges()
    {
        _ball = new GameObject("MovingBall");
        var mover = _ball.AddComponent<TestMover>();
        mover.speed = 5f;
        Vector3 startPos = _ball.transform.position;

        yield return new WaitForSeconds(1f);

        Assert.AreNotEqual(startPos, _ball.transform.position);
    }

    [TearDown]
    public void Teardown()
    {
        if (_ball != null)
            Object.Destroy(_ball);
    }
}
```

---

## 3. The `[UnityTest]` Attribute — Unity's Key Extension
This attribute is Unity's main addition to standard NUnit. It allows writing tests that execute over multiple frames.

### 📌 `[UnityTest]` Features:
| Mode | How It Works | What You Can Return |
| --- | --- | --- |
| Edit Mode | Runs in `EditorApplication.update` loop | `yield return null` (skip frame) |
| Play Mode | Runs as a `MonoBehaviour` coroutine | `yield return new WaitForSeconds()`, `WaitForFixedUpdate`, `null` |

### Examples:
```csharp
// Edit Mode: waiting for a background task to complete
[UnityTest]
public IEnumerator Editor_BackgroundTask_CompletesSuccessfully()
{
    var task = StartLongRunningEditorTask();
    
    while (!task.isDone)
    {
        yield return null;
    }
    
    Assert.IsTrue(task.isSuccess);
}

// Play Mode: waiting for physics update
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

## 4. Async Tests (async/await)
Starting with certain versions of Test Framework, async tests based on `Task` are supported. 
This is convenient for testing async operations without coroutines.

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
> `Assert.ThrowsAsync` can block the main thread and freeze the Editor. Use `try/catch` with a boolean flag instead.

---

## 5. Setting Up Test Assemblies (Assembly Definitions)
To organize tests, you need to create separate `.asmdef` files.
### 📁 Project Structure:
```text
Assets/
├── Scripts/              # Main code
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

### 🔧 Example PlayModeTests.asmdef:
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
> Test assemblies cannot reference `Assembly-CSharp.dll`. Move the code you want to test into a separate custom assembly.

---

## 6. Assert Methods
NUnit provides many assertion methods:

| Method | Purpose |
| --- | --- |
| `Assert.AreEqual(expected, actual)` | Checks equality | 
| `Assert.AreNotEqual(expected, actual)` | Checks inequality |
| `Assert.IsTrue(condition)` | Checks for true | 
| `Assert.IsFalse(condition)` | Checks for false | 
| `Assert.IsNull(object)` | Checks for null |
| `Assert.IsNotNull(object)` | Checks for not null |
| `Assert.That(actual, Is.EqualTo(expected))` | Constraint-based syntax |
| `Assert.Throws<T>(delegate)` | Checks that a method throws an exception |

---

## 7. Best Practices for Writing Tests
1. Use AAA pattern: Arrange → Act → Assert
2. One test — one assertion — makes errors easier to locate
3. Use `[SetUp]` and `[TearDown]` for test isolation
4. Don't test third-party libraries — only your own code
5. For physics and animation, use `[UnityTest]` with `WaitForFixedUpdate` or `WaitForSeconds`
6. Keep tests independent — execution order shouldn't affect results

---

### ⭐ If this project was useful, put a star on GitHub!
