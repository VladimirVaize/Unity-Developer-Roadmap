# 🎯 Задача: «Создание высокопроизводительной системы частиц с DOTS»
Вы разрабатываете игру-симулятор космического пространства с тысячами астероидов. 
Вам нужно создать систему частиц (астероидов) с использованием DOTS (ECS), 
которая может обрабатывать 100 000+ объектов с плавной производительностью.

## 📝 Что нужно реализовать:
### Часть 1: Компоненты (Data)
1. Создайте компоненты:

| Компонент | Поля | Назначение |
| --- | --- | --- |
| `AsteroidComponent` | `float Size`, `float RotationSpeed`, `float3 Velocity` | Данные астероида |
| `HealthComponent` | `int CurrentHealth`, `int MaxHealth` | Здоровье астероида |
| `DestroyComponent` | `float Timer` | Таймер до уничтожения | 
| `AsteroidTag` | (пустой) | Тег для идентификации |

2. Создайте буферный компонент `DamageHistoryBuffer`:
   - Хранит последние 10 полученных уронов
   - Поля: `int Damage`, `float Timestamp`
  
### Часть 2: Системы (Logic)
3. Создайте систему `AsteroidMovementSystem`:
   - Обновляет позицию на основе скорости
   - Вращает астероид (меняет ротацию)
   - Использует `BurstCompile` и `ScheduleParallel`
  
4. Создайте систему `AsteroidHealthSystem`:
   - Проверяет здоровье (если <= 0 → добавляет `DestroyComponent`)
   - Обрабатывает получение урона
   - Добавляет запись в `DamageHistoryBuffer`
  
5. Создайте систему `AsteroidDestroySystem`:
   - Увеличивает таймер `DestroyComponent`
   - При достижении таймера → уничтожает сущность
   - Спавнит 3-5 "осколков" (маленьких астероидов)
  
6. Создайте систему `AsteroidSpawnSystem`:
   - Спавнит новые астероиды с заданной скоростью
   - Использует `EntityCommandBuffer`
   - Распределяет по случайным позициям вокруг центра
  
### Часть 3: Job System
7. Создайте Job для параллельного обновления:
   - `IAsteroidUpdateJob` — IJobParallelFor
   - Обновляет 100 000+ астероидов одновременно
   - Использует NativeArrays для данных
  
8. Создайте Job для проверки столкновений:
   - `CollisionDetectionJob` — IJobParallelFor
   - Проверяет расстояние между астероидами
   - При столкновении наносит урон обоим
  
### Часть 4: Burst Compiler
9. Примените `[BurstCompile]` ко всем Job'ам
10. Используйте `[BurstCompile(CompileSynchronously = true)]` для отладки
11. Убедитесь, что Burst работает (проверьте через Profiler)

### Часть 5: Конвертация GameObject → Entity
12. Создайте Authoring компонент:
```csharp
public class AsteroidAuthoring : MonoBehaviour
{
    public float size = 1f;
    public float rotationSpeed = 1f;
    public int health = 10;
    public GameObject prefab;  // Префаб для спавна
    
    class AsteroidBaker : Baker<AsteroidAuthoring>
    {
        public override void Bake(AsteroidAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            // Добавить компоненты
        }
    }
}
```

13. Префаб астероида должен содержать:
    - Mesh (сфера/астероид)
    - `AsteroidAuthoring` скрипт
    - Unity Physics (опционально)
   
### Часть 6: Тестирование производительности
14. Создайте сценарий для спавна:
    - 10,000 астероидов
    - 50,000 астероидов
    - 100,000 астероидов
   
15. Измерьте FPS для каждого сценария через Profiler
16. Сравните производительность:
    - GameObject-версия (для контроля)
    - DOTS-версия без Burst
    - DOTS-версия с Burst
   
---

## 🧰 Требования к реализации:
- Используйте минимум 3 системы (Movement, Health, Destroy/Spawn)
- Используйте минимум 2 Job'а (Update, Collision)
- Все системы должны использовать `BurstCompile`
- Используйте `EntityCommandBuffer` для спавна/уничтожения
- Все компоненты — struct, не class
- Используйте `IEnableableComponent` для отключения уничтоженных астероидов

---

## 🔍 Проверка работоспособности:
1. Проверьте, что астероиды движутся и вращаются
2. Проверьте, что при получении урона здоровье уменьшается
3. Проверьте, что при здоровье <= 0 астероид уничтожается (или взрывается)
4. Проверьте, что новые астероиды спавнятся автоматически
5. Проверьте, что в Profiler DOTS-версия показывает > 100 FPS для 100K астероидов
6. Проверьте, что система работает в сборке (IL2CPP + Burst)

---

## 💡 Ожидаемый результат:
```text
=== Тест производительности ===
GameObject (10K): 30-40 FPS
GameObject (50K): 5-10 FPS
GameObject (100K): 1-2 FPS

DOTS без Burst (10K): 60 FPS
DOTS без Burst (50K): 30-40 FPS
DOTS без Burst (100K): 10-15 FPS

DOTS + Burst (10K): 120+ FPS
DOTS + Burst (50K): 90-100 FPS
DOTS + Burst (100K): 60-80 FPS

Результат: DOTS + Burst в 60-80 раз быстрее GameObject подхода! 🚀
```

---

## 🏆 Дополнительное задание (опционально):
Реализуйте Visual Effect Graph с DOTS:
- Используйте `VFXGraph` для рендеринга астероидов
- Вместо Mesh Renderer используйте VFX Particle System
- Достигните производительности 1 000 000+ астероидов

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
