# 🔦 Raycasting в Unity: Луч из камеры/точки, LayerMask, RaycastHit

> [!NOTE]
> Raycasting — это техника, при которой из точки в заданном направлении испускается воображаемый луч. Unity определяет, какие объекты пересекает этот луч, и возвращает информацию о них. 
> Это незаменимый инструмент для: прицеливания, стрельбы, кликов мышкой по объектам, определения дистанции до препятствий, AI (видит ли враг игрока) и многого другого.

---

## 🧠 Основные компоненты Raycasting

### 1. Ray (Луч)
`Ray` — структура, содержащая две части:
- `origin` (начало луча) — точка в пространстве, откуда луч выходит.
- `direction` (направление) — вектор, куда луч летит.

#### Создание луча:
```csharp
Ray ray = new Ray(originPoint, directionVector);
```

### 2. RaycastHit (Информация о попадании)

Если луч попадает в объект, Unity заполняет структуру RaycastHit. Она содержит:
- `point` — точка столкновения в мировых координатах.
- `normal` — нормаль поверхности, в которую попал луч.
- `collider` — коллайдер объекта, с которым произошло столкновение.
- `distance` — расстояние от начала луча до точки попадания.
- `transform` — трансформ объекта.

#### Пример использования:
```csharp
RaycastHit hit;
if (Physics.Raycast(ray, out hit, maxDistance))
{
    Debug.Log("Попали в: " + hit.collider.name);
    Debug.Log("На расстоянии: " + hit.distance);
}
```

### 3. LayerMask (Слой)
`LayerMask` позволяет ограничить луч — он будет взаимодействовать только с объектами, принадлежащими указанным слоям. 
Это критически важно для производительности и логики (например, луч стрельбы не должен попадать в самого стреляющего).

#### Как использовать LayerMask:
1. Назначьте объектам слой (Layer) в инспекторе (например, `Enemy`, `Player`, `Wall`).
2. В скрипте создайте маску:
```csharp
public LayerMask enemyLayer; // Назначьте слой Enemy в инспекторе
```
3. Передайте маску в метод Raycast:
```csharp
if (Physics.Raycast(ray, out hit, 100f, enemyLayer))
{
    // Попали только во врагов
}
```

Битовые операции с LayerMask:
```csharp
// Игнорировать слой "Player"
int layerToIgnore = LayerMask.NameToLayer("Player");
LayerMask mask = ~(1 << layerToIgnore); // Инвертируем бит

// Проверка только на слоях "Enemy" и "Wall"
LayerMask mask = LayerMask.GetMask("Enemy", "Wall");
```

---

## 🎯 Типы лучей (Raycast)

### 1. Из камеры в точку экрана (клик мыши)

Самый частый сценарий: определение объекта, на который нажал игрок.
```csharp
void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log("Вы кликнули по: " + hit.collider.name);
        }
    }
}
```
Как это работает: `ScreenPointToRay` преобразует 2D координаты мыши в луч, идущий из камеры через эту точку экрана в 3D мир.

### 2. Из произвольной точки в направлении
Например, луч из глаза врага к игроку (проверка видимости).
```csharp
Vector3 enemyEyes = enemy.transform.position + Vector3.up * 1.5f;
Vector3 directionToPlayer = (player.transform.position - enemyEyes).normalized;
Ray ray = new Ray(enemyEyes, directionToPlayer);
RaycastHit hit;
float maxDistance = Vector3.Distance(enemyEyes, player.transform.position);

if (Physics.Raycast(ray, out hit, maxDistance))
{
    if (hit.collider.CompareTag("Player"))
        Debug.Log("Враг видит игрока!");
    else
        Debug.Log("Враг не видит игрока (закрыт стеной)");
}
```

### 3. Physics.Raycast с несколькими попаданиями (RaycastAll)
Если нужно получить все объекты на пути луча (например, для пронзающей пули).
```csharp
RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
foreach (RaycastHit hit in hits)
{
    Debug.Log("Задет: " + hit.collider.name);
}
```

### 4. Physics.Raycast с отладкой (Debug.DrawRay)
Очень полезно для визуализации лучей в Scene View.
```csharp
Debug.DrawRay(origin, direction * distance, Color.red, 1f); // красный луч на 1 секунду
```

---

## ⚙️ Перегрузки Physics.Raycast (ключевые параметры)
```csharp
// Самый полный вариант:
Physics.Raycast(Ray ray, out RaycastHit hit, float maxDistance, int layerMask, QueryTriggerInteraction triggerInteraction);

// maxDistance = 0 означает "бесконечность"
// layerMask = ~0 означает "все слои"
// triggerInteraction: Collider, Ignore, Respect
```

---

## 🧪 Пример: Система прицеливания и стрельбы
```csharp
public class Gun : MonoBehaviour
{
    public Camera playerCamera;
    public LayerMask shootableLayers; // Только враги и разрушаемые объекты
    public float range = 100f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, shootableLayers))
        {
            Debug.Log($"Попадание в {hit.collider.name} на дистанции {hit.distance}");
            
            // Наносим урон
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(10);
            
            // Визуализируем попадание
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 0.5f);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 0.5f);
        }
    }
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
