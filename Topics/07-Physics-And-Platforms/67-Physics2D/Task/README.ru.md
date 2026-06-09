# 🎯 Задача: «Сборщик монет с физикой 2D»
Вы разрабатываете 2D платформер. Нужно реализовать сбор монет с использованием физики 2D.

## 📝 Классы для реализации:
1. Класс игрока `PlayerController2D`:
```csharp
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 2f;
    }
    
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
        
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckPoint.position, 0.2f, groundLayer);
    }
}
```

2.  Класс монеты `Coin.cs` (нужно реализовать самостоятельно):
   - Требования:
     - Должен иметь `CircleCollider2D`, настроенный как триггер
     - При сборе увеличивать счётчик (статическая переменная или событие)
     - Иметь анимацию вращения (вращение вокруг оси Z через `Transform.Rotate`)
     - При сборе воспроизводить звук и запускать анимацию исчезновения (например, уменьшение масштаба)

---

## 📋 Конкретные задачи для реализации:
1. Настройка физики:
   - Игрок — `Dynamic` Rigidbody2D с гравитацией 2
   - Пол — `Static` Rigidbody2D с BoxCollider2D
   - Монета — `Kinematic` Rigidbody2D (только для триггера)
  
2. Настройка слоёв (Layers):
   - Создать слои: `Player`, `Ground`, `Coin`
   - Настроить Physics2D Matrix так, чтобы:
     - Игрок сталкивался с землёй
     - Игрок не сталкивался физически с монетами (только триггер)
     - Монеты не сталкивались друг с другом
    
3. Сортировка отрисовки (Sorting Layers):
   - Создать Sorting Layers: `Background`, `Gameplay`, `Foreground`, `UI`
   - Поместить фон на `Background`, игрока и монеты на `Gameplay`, интерфейс на `UI`
  
4. Написать класс `Coin` со следующим функционалом:
   - `OnTriggerEnter2D` — сбор монеты
   - Поле `coinValue` (int) — сколько очков даёт монета
   - Метод `Collect()` — отключает коллайдер, проигрывает анимацию, вызывает событие
   - Использовать `Destroy(gameObject, 0.5f)` для удаления после анимации
  
5. Написать класс `GameManager` (синглтон) для подсчёта очков:
   - Статический метод `AddCoins(int amount)`
   - Отображение счёта в консоли или на UI Text

---

## 🧰 Дополнительные требования:
- Добавить физический материал на пол с трением 0.4 и упругостью 0
- Использовать `Physics2D.Raycast` для проверки пола (вместо `OverlapCircle`) в одном из методов
- Реализовать эффект «магнитной монеты»: если игрок рядом (расстояние < 2), монета притягивается к игроку с помощью `AddForce`

---

## 🔍 Ожидаемый результат:
- Игрок может ходить и прыгать по платформам
- При касании монеты она исчезает, счёт увеличивается
- В консоль выводится сообщение: `"Coin collected! Total: X"`
- Монеты вращаются и имеют анимацию сбора

---

## 💡 Структура сцены (пример):
```text
Scene
├── Ground (Static Rigidbody2D + BoxCollider2D)
├── Player (PlayerController2D + Dynamic Rigidbody2D + BoxCollider2D + Sprite)
└── Coins (Kinematic Rigidbody2D + CircleCollider2D (isTrigger) + Coin.cs + Sprite)
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
