# 🎲 Instantiate и Destroy: Создание и уничтожение объектов во время выполнения

> [!NOTE]
> В статических сценах все объекты расставлены вручную до нажатия кнопки Play.
> Но в настоящих играх объекты постоянно появляются и исчезают: выстрелы, враги, осколки, эффекты взрывов, собираемые монеты.
> В Unity за это отвечают два метода: `Instantiate()` (создание) и `Destroy()` (уничтожение).

---

## 📦 Instantiate — создание объекта
### Для чего нужно:
Создать копию существующего игрового объекта (обычно префаба) прямо во время игры. Например, спавнить пули, врагов, частицы.

### Как использовать:
```csharp
public GameObject bulletPrefab; // перетащите сюда префаб пули в инспекторе
public Transform firePoint;     // откуда вылетает пуля

void Shoot()
{
    Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
}
```

### Варианты вызова `Instantiate`:
```csharp
// 1. Только префаб (появится в центре мира с rotation = Quaternion.identity)
Instantiate(enemyPrefab);

// 2. Префаб + позиция + поворот
Instantiate(enemyPrefab, new Vector3(0, 5, 0), Quaternion.identity);

// 3. Полный вариант: префаб, позиция, поворот, и родительский объект
GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, parentTransform);
```

> 💡 Совет: Результат `Instantiate` обычно сохраняют в переменную, чтобы потом управлять созданным объектом (например, изменить скорость или уничтожить через несколько секунд).

### Пример:
Вы делаете шутер. При каждом клике мыши вызывается `Shoot()`, и в сцене появляется новая пуля (копия префаба `Bullet`). Каждая пуля летит самостоятельно.

---

## 💥 Destroy — уничтожение объекта

### Для чего нужно:
Удалить объект со сцены. Это освобождает память и ресурсы. Уничтожать можно как сам объект (например, пуля при попадании), так и другой объект (враг при столкновении с пулей).

### Как использовать:
```csharp
// Уничтожить объект, к которому прикреплён этот скрипт
Destroy(gameObject);

// Уничтожить другой объект
Destroy(otherGameObject);

// Уничтожить с задержкой в секундах (очень полезно для эффектов)
Destroy(explosionEffect, 2f); // взрыв исчезнет через 2 секунды
```

### Пример:
Пуля попадает во врага. В скрипте пули вы пишете:
```csharp
void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.tag == "Enemy")
    {
        Destroy(collision.gameObject); // уничтожить врага
        Destroy(gameObject);           // уничтожить саму пулю
    }
}
```

---

## 🔁 Типичные паттерны использования

| Ситуация | Что делаем |
|------------------------|--------------------------------------------|
| 🎯 Выстрел | `Instantiate(bulletPrefab)` → пуля летит |
| 💀 Попадание пули во врага | `Destroy(enemy)` + `Destroy(bullet)` |
| 🧨 Враг умер — оставить взрыв | `Instantiate(explosionPrefab, enemy.position, ...)` → затем `Destroy(enemy)` |
| 🪙 Подобрана монета | `Destroy(coin)` и добавить счёт |
| 🚀 Заспавнить волну врагов | Цикл `for` с `Instantiate(enemyPrefab, randomPosition, ...)` |
| ⏱ Убрать эффект через время | `Destroy(particleEffect, 1.5f)` |

---

## ⚠️ Важные замечания
- `Destroy(gameObject)` не происходит мгновенно в том же кадре (обычно в конце кадра). Но для большинства задач это не важно.
- Не пытайтесь вызвать `Destroy` на объекте, который уже уничтожен — будет ошибка. Проверяйте через `if (object != null)`.
- Для полного удаления из памяти сразу есть `DestroyImmediate()`, но он используется редко и может быть опасен.
- Если нужно не уничтожить, а временно спрятать объект — используйте `gameObject.SetActive(false)` (это быстрее, чем Destroy и Instantiate заново).

---

## 🧪 Простой полный пример (спавнер врагов)
```csharp
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        GameObject newEnemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        
        // Уничтожить врага, если он не умрёт раньше, через 5 секунд автоматически
        Destroy(newEnemy, 5f);
    }
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
