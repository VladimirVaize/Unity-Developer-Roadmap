# 🎯 Практическое задание: Система логирования для подземелья

Вы разрабатываете RPG с процедурно генерируемым подземельем. Ваша задача — реализовать систему логирования, которая поможет отлаживать генерацию уровней и боевую систему.

---

## 📝 Описание задачи
Вам дан класс `DungeonGenerator`, который генерирует комнаты и врагов, и класс `CombatSystem`, который обрабатывает бои. 
В текущем виде они используют сырой `Debug.Log`, который захламляет консоль и будет тормозить релизную сборку.

### Требования:
1. Создайте класс `GameLogger` со следующими методами:
   - `LogGeneration(string message)` — логи для процесса генерации подземелья. Должен работать ТОЛЬКО в редакторе Unity.
   - `LogCombat(string message)` — логи для боевой системы. Должен работать в Development билдах и в редакторе.
   - `LogCriticalError(string message)` — критическая ошибка. Должен работать ВСЕГДА (включая Release билды) и записывать сообщение в файл `critical_errors.txt` в `Application.persistentDataPath`.
2. Модифицируйте код классов, заменив прямые вызовы `Debug.Log` на вызовы вашего `GameLogger`.
3. Настройте перехват всех логов через `Application.logMessageReceived`,
   чтобы все сообщения `LogGeneration` и `LogCombat` тоже дублировались в файл `full_debug.log`, но только в Development билдах (проверьте через `Debug.isDebugBuild`).

---

## 🔧 Исходный код (дано)
```csharp
// DungeonGenerator.cs
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public void GenerateDungeon()
    {
        Debug.Log("Начинаем генерацию подземелья...");
        // ... логика генерации
        Debug.Log("Создана комната #" + Random.Range(1, 20));
        // ... ещё логика
        Debug.Log("Генерация завершена. Всего врагов: " + Random.Range(5, 50));
    }
}

// CombatSystem.cs
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public void Attack( GameObject target, int damage )
    {
        Debug.Log($"Атака по {target.name} с уроном {damage}");
        // ... боевая логика
        if (Random.value < 0.1f)
            Debug.LogError("КРИТИЧЕСКАЯ ОШИБКА: цель не найдена в боевой системе!");
    }
}
```

---

## ✅ Ожидаемый результат
- При запуске в редакторе Unity — в консоли видны все логи (и генерации, и боя, и ошибки).
- В Release билде (без галочки Development Build) — вызовы `LogGeneration` и `LogCombat` не выполняются совсем, но ошибки пишутся в файл.
- В Development билде — в папке `persistentDataPath` появляется `full_debug.log` со ВСЕМИ логами (дублируются из консоли) и отдельно `critical_errors.txt` с критическими ошибками.
- Код не должен содержать директив `#if ... #endif` (используйте `[Conditional]` и проверку `Debug.isDebugBuild`).

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
