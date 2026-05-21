# 🧪 Практическая задача: Отладка скрипта движения с брейкпоинтами

## 📌 Описание задачи
Вы создали простой скрипт движения для персонажа в Unity. 
Скрипт работает, но персонаж двигается слишком быстро и проходит сквозь стены из-за ошибки в логике. 
Ваша задача — использовать брейкпоинты, Immediate Window и Attach to Unity, чтобы найти и исправить ошибки.

### Исходный код (с ошибками)
```csharp
using UnityEngine;

public class BuggyMovement : MonoBehaviour
{
    public float speed = 50f;       // Слишком большое значение
    public float jumpForce = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Ошибка: rb может быть null, если компонент отсутствует
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(move, 0, 0);

        if (Input.GetButtonDown("Jump"))
        {
            // Ошибка: прыжок применяется каждый кадр (должно быть в FixedUpdate)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
```

### Ожидаемое поведение
- Персонаж двигается с комфортной скоростью (например, 10 единиц в секунду).
- Прыжок срабатывает один раз и не накапливается.
- Персонаж не проходит сквозь стены (коллайдеры работают).

### Текущее (ошибочное) поведение
- Персонаж слишком быстрый.
- Прыжок может сработать несколько раз за одно нажатие.
- Проходит сквозь стены из-за использования `transform.Translate` вместо физики.

---

## 🎯 Цель задачи
1. Настроить IDE (Visual Studio или Rider) для отладки с Unity.
2. Установить брейкпоинты в проблемных местах кода.
3. Использовать Immediate Window для изменения переменных во время паузы.
4. Применить Attach to Unity (отладка без перезапуска игры).
5. Исправить ошибки.

---

## 📝 Пошаговые указания
### Шаг 1. Подготовка
- Создайте новый 2D или 3D проект в Unity.
- Добавьте на сцену куб (Cube) — это будет персонаж.
- Добавьте компонент `Rigidbody` (для физики).
- Создайте стену (другой куб с коллайдером) на пути персонажа.
- Прикрепите скрипт `BuggyMovement` к кубу.

### Шаг 2. Настройка IDE
- Убедитесь, что в `Edit → Preferences → External Tools` выбран ваш редактор (VS или Rider).
- Откройте скрипт из Unity (двойной клик по нему в окне Project).

### Шаг 3. Запуск и наблюдение (без отладки)
- Нажмите Play в Unity.
- Попробуйте двигаться стрелками и прыгать (пробел).
- Зафиксируйте проблемы: скорость слишком высокая, прыжок странный, проход сквозь стену.

### Шаг 4. Отладка с брейкпоинтами
- Остановите игру.
- В коде поставьте брейкпоинт на строке `transform.Translate(...)`.
- Запустите игру снова (Play). Когда персонаж пошевелится, выполнение остановится.
- Наведите курсор на переменную `move` — посмотрите её значение.

### Шаг 5. Immediate Window
- Откройте Immediate Window (`Ctrl + Alt + I` в VS, поищите в Rider).
- Пока игра на паузе (брейкпоинт активен), введите:
  - `speed = 10;`
  - Нажмите Enter.
 
- Нажмите F5 (Continue) — персонаж поедет медленнее прямо во время игры.

### Шаг 6. Attach to Unity (дополнительно)
- Закройте IDE (оставьте Unity открытой).
- Запустите игру в Unity.
- Откройте IDE снова, но не открывайте проект автоматически.
- Выберите `Debug → Attach to Unity Process` (или аналогичное в Rider).
- Теперь поставьте брейкпоинт в прыжке (`rb.AddForce...`).
- Нажмите пробел в игре — брейкпоинт сработает, даже если вы не запускали отладку из IDE.

### Шаг 7. Исправление ошибок
Исправьте код, чтобы он работал правильно:
```csharp
using UnityEngine;

public class FixedMovement : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Rigidbody missing!");
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + Vector3.right * move);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
```
- Перетащите исправленный скрипт на куб (удалите старый).
- Убедитесь, что стена имеет тег `Ground` (создайте тег в Unity).
- Запустите — персонаж теперь двигается через физику, не проходит сквозь стены, прыгает один раз.

---

## ✅ Критерии успеха
- Вы успешно установили брейкпоинт и игра остановилась.
- Вы изменили переменную `speed` через Immediate Window и увидели эффект.
- Вы использовали `Attach to Unity` для отладки без перезапуска.
- Итоговый скрипт работает правильно: скорость ≈10, прыжок одноразовый, стены не проходятся.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
