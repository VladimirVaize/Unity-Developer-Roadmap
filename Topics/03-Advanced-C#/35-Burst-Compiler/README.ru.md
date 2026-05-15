# ⚡ Burst Compiler: Высокопроизводительный компилятор для Job System

Burst Compiler — это оптимизирующий AOT (Ahead-Of-Time) компилятор для Unity, который превращает управляемый код (C#), 
написанный в определённых ограничениях (внутри `jobs`), в высокоэффективный, нативный (native) код. 
В сочетании с Job System он позволяет использовать всю мощь современных многоядерных процессоров без сложностей ручного управления потоками.

---

## 🎯 Зачем нужен Burst?
- 🚀 Фантастический прирост производительности — ускорение в 5-100 раз по сравнению с обычным C# для вычислительно тяжёлых задач.
- 🔁 Использование SIMD (Single Instruction, Multiple Data) автоматически — компилятор векторизует циклы.
- 🔒 Безопасность — Burst работает только внутри `unsafe` или специально помеченного кода (`[BurstCompile]`), предотвращая ошибки управления памятью.
- 🧠 Понятный ассемблерный код — для профилирования и экстремальной оптимизации.
- 🎮 Идеален для игр — процедурные генерации, физика частиц, AI (большое количество юнитов), обработка сетки, симуляции.

---

## 🧠 Как это работает вместе с Job System?

1. Вы пишете struct, реализующую один из интерфейсов `IJob`, `IJobParallelFor`, `IJobChunk` и т.д.
2. Помечаете метод `Execute()` атрибутом `[BurstCompile]`.
3. Планируете джобу через `Schedule()`.
4. Burst компилирует её перед первым выполнением в максимально быстрый нативный код.
5. Job System распределяет выполнение по рабочим потокам.

---

## 📦 Что нужно сделать, чтобы использовать Burst?
1. Установить пакеты:
   - `com.unity.jobs`
   - `com.unity.burst`
   - `com.unity.collections` (для типов `NativeArray<T>`)
  
2. Импортировать пространства имён:
   ```csharp
   using Unity.Burst;
   using Unity.Jobs;
   using Unity.Collections;
   ```
   
3. Написать джобу, используя только допустимые типы:
   - `NativeArray<T>`, `NativeList<T>` (но не обычные `List<T>`)
   - `float`, `int`, `bool`, `Vector3`, `Mathf` (но не `Math` из .NET)
   - Никаких ссылочных типов (`class`, `string`), виртуальных методов или исключений.
  
4. Убедиться, что включена опция Burst в настройках проекта (Editor → Project Settings → Burst AOT Settings).

---

## ✅ Пример простой джобы с Burst
```csharp
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
public struct VelocityJob : IJobParallelFor
{
    public NativeArray<Vector3> positions;
    public NativeArray<Vector3> velocities;
    public float deltaTime;

    public void Execute(int i)
    {
        positions[i] += velocities[i] * deltaTime;
    }
}
```
### Использование:
```csharp
NativeArray<Vector3> positions = new NativeArray<Vector3>(1000, Allocator.TempJob);
NativeArray<Vector3> velocities = new NativeArray<Vector3>(1000, Allocator.TempJob);
// ... заполнить данные
VelocityJob job = new VelocityJob
{
    positions = positions,
    velocities = velocities,
    deltaTime = Time.deltaTime
};
JobHandle handle = job.Schedule(positions.Length, 64); // 64 = batch count
handle.Complete();
positions.Dispose();
velocities.Dispose();
```

---

## ⚠️ Ограничения и подводные камни
- ❌ Нельзя использовать `Debug.Log` внутри джобы с Burst.
- ❌ Нельзя обращаться к `GameObject` или компонентам MonoBehaviour.
- ❌ Нужно использовать `Unity.Mathematics` вместо стандартных математических функций для максимальной производительности.
- ✅ Можно использовать `float3`, `float4x4`, `math.sqrt()` и т.д.
- ✅ Burst не работает с виртуальными методами и интерфейсами внутри джобы.

---

## 🧪 Когда Burst НЕ нужен?
- Мало данных (менее 100-500 итераций) — накладные расходы на компиляцию и планирование не окупятся.
- Код выполняется очень редко (раз в несколько секунд).
- Вы активно используете рефлексию, исключения или сложную объектную модель.

---

## 💡 Вывод
Burst Compiler + Job System — это «серебряная пуля» для вычислительно насыщенных задач в Unity. 
Они превращают C# код в код, близкий к производительности C++, и позволяют эффективно загружать все ядра CPU без головной боли с потоками.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
