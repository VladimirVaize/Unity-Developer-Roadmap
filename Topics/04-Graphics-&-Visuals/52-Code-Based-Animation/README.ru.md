# 🎯 Программная анимация в Unity: DoTween, LeanTween, интерполяции и Easing Functions

В Unity существует два основных подхода к анимации: через Animation Window (создание клипов в редакторе) и программно (code-based animation). 
Второй подход даёт гибкость, динамическое управление и позволяет создавать анимации "на лету" без создания сотен анимационных клипов.

### 📌 Когда использовать программную анимацию?
- Всплывающие окна, подсказки, уведомления
- Эффекты получения предметов, очки опыта
- Анимации UI (кнопки, панели, полосы здоровья)
- Процедурные покачивания объектов (деревья, враги в idle-режиме)
- Анимации, зависящие от игровых параметров (чем сильнее удар — тем сильнее отскок)

---

## 📚 1. Что такое Tween-библиотеки?
Tween (сокращение от in-between, "промежуточный") — это метод анимации, при котором вы задаёте начальное и конечное значения, а библиотека автоматически вычисляет все промежуточные состояния.

Вместо того чтобы вручную писать в Update():
```csharp
float t = 0;
void Update() {
    t += Time.deltaTime / duration;
    transform.position = Vector3.Lerp(startPos, endPos, t);
}
```

Вы пишете одну строку:
```csharp
transform.DOMove(endPos, duration);  // DOTween
LeanTween.move(gameObject, endPos, duration);  // LeanTween
```

---

## 🚀 2. Популярные Tween-библиотеки
### 🟣 DOTween (Demigiant) — рекомендуемый выбор
Установка: Window → Package Manager → [+ ] → "Add package from git URL" → `https://github.com/Demigiant/dotween` 

| Характеристика | Описание |
| --- | --- |
| Функциональность | Очень богатая (почти всё, что можно анимировать) |
| UI-совместимость | Отличная (Text, CanvasGroup, Image) |
| Производительность | Высокая, низкое выделение памяти |
| Сообщество | Огромное, множество примеров |

Базовый синтаксис:
```csharp
using DG.Tweening;

// Простое перемещение
transform.DOMove(new Vector3(5, 0, 0), 1f);

// С дополнительными параметрами
transform.DOMove(new Vector3(5, 0, 0), 1f)
    .SetEase(Ease.OutBounce)  // тип плавности
    .SetLoops(2, LoopType.Yoyo)  // петля с возвратом
    .SetDelay(0.5f);  // задержка перед стартом
```

### 🟢 LeanTween — лёгкая альтернатива
Установка: Unity Asset Store или GitHub (`https://github.com/dentedpixel/LeanTween`)

| Характеристика | Описание |
| --- | --- |
| Вес | Очень лёгкая, минимальный overhead |
| Скорость | Быстрая инициализация анимаций |
| Синтаксис | Цепочки вызовов, похож на DOTween |

Базовый синтаксис:
```csharp
// Простое перемещение
LeanTween.move(gameObject, new Vector3(5, 0, 0), 1f);

// С дополнительными параметрами
LeanTween.move(gameObject, new Vector3(5, 0, 0), 1f)
    .setEase(LeanTweenType.easeOutBounce)
    .setLoopPingPong(2)  // 2 цикла туда-обратно
    .setDelay(0.5f);
```

### 📊 Сравнение библиотек (из бенчмарков)

| Библиотека | GC Allocation (создание анимации) | Скорость запуска 50k анимаций |
| --- | --- | --- |
| PrimeTween | 0 B | ~6.4 ms |
| LeanTween | ~292 B | ~19 ms |
| DOTween | ~732 B | ~33 ms |
| UnityTweens | ~887 B | ~33 ms |

*Данные из публичных бенчмарков*

### 💡 Как выбрать?
- DOTween — для большинства проектов (лучший баланс функциональности и производительности)
- LeanTween — для мобильных игр, где важен малый размер сборки
- PrimeTween — для проектов, критичных к GC (ноль аллокаций)

---

## 🧮 3. Интерполяции (Interpolations)
Интерполяция — это математический метод вычисления промежуточных значений между двумя точками.

### Встроенные методы Unity
Unity предоставляет несколько базовых методов интерполяции:

| Метод | Описание | Формула |
| --- | --- | --- |
| `Mathf.Lerp` | Линейная интерполяция | Равномерное движение от A к B |
| `Mathf.LerpUnclamped` | Линейная без ограничений | Может выходить за пределы [0,1] |
| `Mathf.SmoothStep` | Плавный старт и финиш | Ускорение в начале, замедление в конце |
| `Vector3.Lerp` | Векторная интерполяция | Для позиций, поворотов, масштабов |

```csharp
// Пример SmoothStep — плавное ускорение и замедление
float t = (Time.time - startTime) / duration;
float smoothValue = Mathf.SmoothStep(0, 10, t);
transform.position.x = smoothValue;  // Движение с плавным стартом и финишем
```

### Рукописная интерполяция (без библиотек)
Если вы не хотите использовать сторонние библиотеки, можно реализовать простую анимацию вручную:

```csharp
public class SimpleTween : MonoBehaviour {
    public Vector3 startPos;
    public Vector3 endPos;
    public float duration = 1f;
    private float timer;
    private bool isAnimating;

    public void StartAnimation() {
        startPos = transform.position;
        timer = 0;
        isAnimating = true;
    }

    void Update() {
        if (!isAnimating) return;
        
        timer += Time.deltaTime / duration;
        float t = Mathf.Clamp01(timer);
        
        // Применяем easing вручную
        float easedT = EaseOutCubic(t);
        
        transform.position = Vector3.Lerp(startPos, endPos, easedT);
        
        if (t >= 1f) isAnimating = false;
    }
    
    private float EaseOutCubic(float x) {
        return 1 - Mathf.Pow(1 - x, 3);
    }
}
```

---

## 📈 4. Easing Functions (Функции плавности)
Easing functions определяют характер движения: будет ли объект резко стартовать и плавно останавливаться, отскакивать, пружинить и т.д.

### Популярные типы Easing

| Тип | Визуальное описание | Пример использования |
| --- | --- | --- |
| Linear | Постоянная скорость | Движение конвейера, скроллинг |
| Ease.In | Медленный старт → ускорение | Падающий объект (гравитация) |
| Ease.Out | Быстрый старт → замедление | Тормозящая машина |
| Ease.InOut | Медленно → быстро → медленно | Мяч, брошенный вверх и падающий |
| Ease.OutBounce | С затухающими отскоками | Мяч, упавший на пол |
| Ease.OutElastic | С перелётом и возвратом | "Пружинящая" кнопка |
| Ease.OutBack | Перелёт за цель и возврат | Выдвигающееся меню |

### Примеры в DOTween

```csharp
// Ease как параметр в цепочке
transform.DOMoveX(5, 1f).SetEase(Ease.OutBounce);

// Предустановленные кривые
transform.DOScale(1.5f, 0.5f).SetEase(Ease.InElastic);

// Пользовательская кривая
AnimationCurve customCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
transform.DOMoveY(10, 2f).SetEase(customCurve);
```

### Примеры в LeanTween

```csharp
LeanTween.move(gameObject, targetPos, 1f)
    .setEase(LeanTweenType.easeOutBounce);

LeanTween.scale(gameObject, Vector3.one * 1.5f, 0.5f)
    .setEase(LeanTweenType.easeInElastic);
```

### Математические формулы популярных Easing

```csharp
// Функции для ручной реализации

// Квадратичная (ускорение)
float EaseInQuad(float x) => x * x;

// Квадратичная (замедление)
float EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);

// Кубическая
float EaseInCubic(float x) => x * x * x;
float EaseOutCubic(float x) => 1 - Mathf.Pow(1 - x, 3);

// Отскок (упрощённая версия)
float EaseOutBounce(float x) {
    if (x < 1 / 2.75) return 7.5625f * x * x;
    if (x < 2 / 2.75) return 7.5625f * (x - 1.5f / 2.75f) * (x - 1.5f / 2.75f) + 0.75f;
    if (x < 2.5 / 2.75) return 7.5625f * (x - 2.25f / 2.75f) * (x - 2.25f / 2.75f) + 0.9375f;
    return 7.5625f * (x - 2.625f / 2.75f) * (x - 2.625f / 2.75f) + 0.984375f;
}
```

---

## 🔄 5. Типы зацикливания (Loop Types)

| LoopType | Описание | DOTween | LeanTween |
| --- | --- | --- | --- |
| Restart | Перезапуск с начала | `LoopType.Restart` | `setLoopCount()` (по умолчанию) |
| Yoyo | Туда-обратно | `LoopType.Yoyo` | `setLoopPingPong()` |
| Incremental | Накапливающийся сдвиг | `LoopType.Incremental` | Нет аналога |

```csharp
// DOTween: пульсация размера
transform.DOScale(1.2f, 0.3f)
    .SetLoops(-1, LoopType.Yoyo);  // Бесконечное пульсирование

// LeanTween: пульсация размера
LeanTween.scale(gameObject, Vector3.one * 1.2f, 0.3f)
    .setLoopPingPong();  // Бесконечное пульсирование
```

---

## 🎮 6. Практические примеры для игр
### Мигание при получении урона 

```csharp
using DG.Tweening;

public class DamageFlash : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    
    public void Flash() {
        // Цвет → прозрачный → обратно, повторить 3 раза
        spriteRenderer.DOColor(Color.clear, 0.1f)
            .SetLoops(6, LoopType.Yoyo);
    }
}
```

### Тряска камеры при ударе

```csharp
// DOTween
Camera.main.transform.DOShakePosition(0.5f, 0.3f, 10, 90);

// LeanTween
LeanTween.moveX(Camera.main.gameObject, 
    Camera.main.transform.position.x + 0.2f, 0.05f)
    .setLoopPingPong(5);
```

### Всплывающий текст урона

```csharp
public void ShowDamageText(int damage, Vector3 worldPosition) {
    GameObject textObj = Instantiate(damagePrefab, worldPosition, Quaternion.identity);
    TextMeshPro text = textObj.GetComponent<TextMeshPro>();
    text.text = damage.ToString();
    
    // Анимация: подлетаем вверх и исчезаем
    Sequence sequence = DOTween.Sequence();
    sequence.Append(textObj.transform.DOMoveY(worldPosition.y + 1.5f, 0.8f));
    sequence.Join(textObj.transform.DOScale(1.5f, 0.3f));
    sequence.Append(textObj.transform.DOScale(0f, 0.2f));
    sequence.OnComplete(() => Destroy(textObj));
}
```

### Покачивание объекта в idle-режиме

```csharp
// Бесконечное плавное покачивание
transform.DORotate(new Vector3(0, 0, 15f), 1f)
    .SetLoops(-1, LoopType.Yoyo)
    .SetEase(Ease.InOutSine);
```

### Полоса здоровья с плавным изменением

```csharp
public void UpdateHealthBar(float currentHealth, float maxHealth) {
    float targetFill = currentHealth / maxHealth;
    slider.DOValue(targetFill, 0.3f)
        .SetEase(Ease.OutCubic);
}
```

---

## ⚡ 7. Советы по оптимизации

| Совет | Объяснение |
| --- | --- |
| Используйте SetAutoKill | DOTween уничтожает завершённые анимации автоматически. Отключайте для анимаций, которые могут понадобиться снова (`SetAutoKill(false)`) |
| Не создавайте тысячи коротких анимаций | Лучше использовать один таймер с ручным обновлением, чем 1000 отдельных Tween на каждый кадр |
| Используйте .Complete() перед повторным запуском | `tween.Complete(); tween.Restart();` — гарантирует чистое состояние |
| LeanTween.describe() | Отладка активных анимаций (LeanTween) |
| DOTween.KillAll() | При смене сцены — очистка всех анимаций во избежание ошибок |
| Заморозка анимаций | `DOTween.TogglePauseAll()` / `LeanTween.pauseAll()` |

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
