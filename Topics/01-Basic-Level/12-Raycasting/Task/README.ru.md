# 🎯 Практическое задание: Система «Интерактивный луч»

## 📝 Описание задачи
Вам нужно создать простую, но функциональную систему взаимодействия с объектами через луч (Raycast). 
Игрок управляет камерой от первого лица, наводит прицел (крестик в центре экрана) на интерактивные объекты и взаимодействует с ними клавишей `E`.

---

## ✅ Требования

1. **Луч из центра камеры** — каждое нажатие E испускает луч из центра экрана (как в шутерах).
2. **LayerMask** — луч должен взаимодействовать только с объектами слоя `"Interactable"`. Все остальные объекты (стены, пол, триггеры) игнорируются.
3. **RaycastHit** — при попадании луч должен:
   - Получить компонент `IInteractable` у объекта.
   - Вызвать метод `Interact()`.
   - Вывести в консоль: `"Взаимодействие с {имя объекта} на дистанции {дистанция}"`.
4. **Визуализация** — нарисуйте луч с помощью `Debug.DrawRay`:
   - Зелёный при попадании (длительность 0.5 сек).
   - Красный при промахе (длительность 0.5 сек).
5. **Интерфейс IInteractable** — реализуйте его в двух типах объектов:
   - `Door` (дверь): открывается/закрывается (логика любая: Debug.Log или анимация).
   - `LightSwitch` (выключатель): включает/выключает свет (меняет интенсивность точечного света).
  
---

## 🧱 Стартовый код (шаблон)

```csharp
// IInteractable.cs
public interface IInteractable
{
    void Interact();
}

// Interactor.cs (на камере)
public class Interactor : MonoBehaviour
{
    public float maxDistance = 5f;
    public LayerMask interactableLayer; // Назначьте слой "Interactable" в инспекторе

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CastInteractionRay();
        }
    }

    void CastInteractionRay()
    {
        // 1. Создать луч из центра экрана
        
        // 2. Raycast с LayerMask
        
        // 3. Если попали — получить IInteractable и вызвать Interact()
        
        // 4. Debug.DrawRay (зелёный/красный)
    }
}

// Door.cs
public class Door : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    
    public void Interact()
    {
        isOpen = !isOpen;
        Debug.Log($"Дверь {(isOpen ? "открыта" : "закрыта")}");
        // Дополнительно: повернуть дверь, проиграть звук и т.д.
    }
}

// LightSwitch.cs
public class LightSwitch : MonoBehaviour, IInteractable
{
    public Light targetLight;
    private bool isOn = true;
    
    public void Interact()
    {
        isOn = !isOn;
        targetLight.intensity = isOn ? 1f : 0f;
        Debug.Log($"Свет {(isOn ? "включён" : "выключен")}");
    }
}
```

---

## 🧪 Проверка работоспособности

1. Создайте сцену: пол, стены, камера (FPS-контроллер).
2. Добавьте два куба: один как `Door`, второй как `LightSwitch`.
3. Назначьте им слой `"Interactable"`.
4. Навесьте соответствующие скрипты (`Door`, `LightSwitch`) и настройте ссылки.
5. Запустите сцену, наведитесь на объекты (крестик в центре экрана) и нажмите `E`.
6. Проверьте консоль и визуализацию лучей в Scene View.

---

## 🔥 Дополнительное задание (по желанию)
- Добавьте подсветку объекта, на который наведён прицел (меняйте материал или Outline).
- Реализуйте звук взаимодействия (`AudioSource.PlayOneShot`).
- Сделайте так, чтобы `IInteractable` имел свойство `string InteractionPrompt` (например, `"Нажмите E, чтобы открыть дверь"`) и отображайте его на UI.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
