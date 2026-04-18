# 🌀 Корутины в Unity: IEnumerator, WaitForSeconds, отложенные действия

> [!NOTE]
> Корутины (Coroutines) — это мощный механизм Unity, позволяющий выполнять код поэтапно, растягивая его выполнение во времени без блокировки основного потока игры.
> В отличие от `Update()`, который вызывается каждый кадр, корутина может «ждать» заданное время или определённого события, а затем продолжить работу с того места, где остановилась.

---

## 🔍 Что такое IEnumerator?

`IEnumerator` — это специальный тип возвращаемого значения для функций-корутин. Он позволяет функции «замораживать» своё выполнение с помощью ключевого слова `yield return`.

### Как это выглядит:
```csharp
using System.Collections;
using UnityEngine;

public class CoroutineExample : MonoBehaviour
{
    void Start()
    {
        // Запуск корутины
        StartCoroutine(MyCoroutine());
    }

    IEnumerator MyCoroutine()
    {
        Debug.Log("Корутина началась");
        yield return null; // Ждём 1 кадр
        Debug.Log("Прошёл 1 кадр");
    }
}
```

Важно: Корутна всегда возвращает `IEnumerator` и запускается через `StartCoroutine()`.

---

## ⏱️ WaitForSeconds — ожидание по времени

`WaitForSeconds` — самый популярный способ приостановить корутину на указанное количество секунд (реальное время, не зависит от частоты кадров).

### Синтаксис:
```csharp
yield return new WaitForSeconds(2.5f); // Ждём 2.5 секунды
```

### Пример: мигающий объект:
```csharp
IEnumerator Blink()
{
    while (true) // Бесконечное мигание
    {
        GetComponent<Renderer>().enabled = false; // Выключить
        yield return new WaitForSeconds(0.5f);
        GetComponent<Renderer>().enabled = true; // Включить
        yield return new WaitForSeconds(0.5f);
    }
}
```

---

## ⏲️ Другие виды ожиданий

| Команда | Описание |
|-----------------------------------|------------------------------------|
| `yield return null` | Ждать 1 кадр |
| `yield return new WaitForSeconds(1)` | Ждать 1 секунду (реального времени) |
| `yield return new WaitForSecondsRealtime(1)` | Ждать 1 секунду, игнорируя Time.timeScale |
| `yield return new WaitForFixedUpdate()` | Ждать следующего физического обновления (обычно 0.02 сек) |
| `yield return new WaitUntil(() => условие)` | Ждать, пока условие не станет true |
| `yield return new WaitWhile(() => условие)` | Ждать, пока условие true, затем продолжить |
| `yield return StartCoroutine(ДругаяКорутина())` | Ждать завершения другой корутины |

---

## 🎯 Отложенные действия — практические сценарии

### 1. Задержка перед выполнением действия
```csharp
IEnumerator DestroyAfterDelay(GameObject obj, float delay)
{
    yield return new WaitForSeconds(delay);
    Destroy(obj);
}
// Использование: StartCoroutine(DestroyAfterDelay(enemy, 3f));
```

### 2. Постепенное изменение (анимация без аниматора)
```csharp
IEnumerator FadeOut(SpriteRenderer renderer, float duration)
{
    float elapsed = 0;
    Color startColor = renderer.color;
    Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        renderer.color = Color.Lerp(startColor, endColor, t);
        yield return null; // Ждём каждый кадр
    }
}
```

### 3. Спавн волн врагов
```csharp
IEnumerator SpawnWaves()
{
    for (int wave = 1; wave <= 5; wave++)
    {
        Debug.Log($"Волна {wave} начинается!");
        for (int i = 0; i < 10; i++)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f); // Пауза между врагами
        }
        yield return new WaitForSeconds(3f); // Пауза между волнами
    }
}
```

---

## 🛑 Остановка корутины
```csharp
// Запуск
Coroutine myRoutine = StartCoroutine(MyCoroutine());

// Остановка конкретной корутины
StopCoroutine(myRoutine);

// Остановка всех корутин на этом объекте
StopAllCoroutines();
```

---

## ⚠️ Важные замечания
- Корутины работают только на активных (`active = true`) объектах. Если объект отключить, корутина остановится.
- `WaitForSeconds` зависит от `Time.timeScale`. При паузе (`timeScale = 0`) ожидание не закончится. Используйте `WaitForSecondsRealtime` для пауз.
- Не используйте `while(true)` без `yield return` внутри — это заморозит игру.
- Корутины не возвращают значения напрямую. Используйте `Action` или события для передачи результата.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
