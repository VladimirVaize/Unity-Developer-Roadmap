# 🎮 UI Скрипты в Unity: Button, Slider, Toggle, ScrollRect

> [!Note]
> Этот материал посвящён работе с основными UI-элементами через скрипты в Unity.
> Вы узнаете, как обрабатывать нажатия кнопок, считывать значения ползунков, отслеживать состояние переключателей и управлять прокруткой.
> Эти навыки необходимы для создания меню, настроек, инвентарей и любых интерфейсов.

---

## 🖱️ Button.onClick

### Для чего нужно:
Кнопка (`Button`) – основной элемент взаимодействия. Событие `.onClick` позволяет выполнить ваш код в момент, когда пользователь нажимает и отпускает кнопку.

### Как использовать в коде:
1. Получите ссылку на компонент Button.
2. Добавьте слушатель (listener) через `onClick.AddListener(НазваниеМетода)`.
3. Напишите метод, который будет вызываться.
```csharp
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        Debug.Log("Игра начата!");
        // Загрузить сцену, запустить игру и т.д.
    }
}
```

### 🔧 Как настроить без кода (для простых случаев):
В инспекторе кнопки есть список `OnClick()`. Перетащите туда объект, выберите его метод (например, `SceneManager.LoadScene`). Это полезно для быстрых прототипов.

### Пример:
Кнопка `OptionsButton` открывает панель настроек. По нажатию – панель становится активной (`SetActive(true)`).

---

## 📊 Slider

### Для чего нужно:
Слайдер (`Slider`) позволяет пользователю выбирать значение из диапазона (например, громкость звука, чувствительность мыши, размер шрифта).

### Как использовать в коде:
1. Получите ссылку на `Slider`.
2. Читайте его свойство `.value`.
3. Реагируйте на изменения через `onValueChanged.AddListener`.
```csharp
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioSource backgroundMusic;

    void Start()
    {
        // Установить начальное значение
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
        // Добавить слушатель изменений
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        backgroundMusic.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }
}
```

### 🔧 Свойства Slider в инспекторе:
- `Min Value` / `Max Value` – минимальное и максимальное значение.
- `Whole Numbers` – если включено, значение будет целым (например, 1, 2, 3).
- `Value` – текущее значение (можно задать вручную).

### Пример:
Слайдер громкости от 0 до 1. При движении ползунка громкость музыки меняется в реальном времени, а значение сохраняется в `PlayerPrefs`.

---

## ✅ Toggle

### Для чего нужно:
Переключатель (`Toggle`) – это флажок, который может быть включён (`true`) или выключен (`false`). 
Используется для настроек типа "вкл/выкл": полноэкранный режим, звуки интерфейса, инверсия камеры.

### Как использовать в коде:
1. Получите ссылку на `Toggle`.
2. Читайте свойство `.isOn`.
3. Подпишитесь на событие `onValueChanged`.
```csharp
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Toggle fullscreenToggle;

    void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
    }

    void OnFullscreenToggled(bool isOn)
    {
        Screen.fullScreen = isOn;
        Debug.Log("Полноэкранный режим: " + isOn);
    }
}
```

### 🔧 Дополнительно:
`Toggle` может быть частью группы (`Toggle Group`), где автоматически выключаются другие переключатели. 
Это удобно для выбора одного из нескольких вариантов (например, сложность: Лёгкая / Средняя / Сложная).

### Пример:
Чекбокс "Отключить музыку" – при его активации громкость музыки становится 0, при деактивации возвращается к сохранённому значению.

---

## 📜 ScrollRect

### Для чего нужно:
Прокручиваемая область (`ScrollRect`) используется для списков, инвентарей, длинных текстов или любого контента, который не помещается на экране.

### Как использовать в коде:
Хотя `ScrollRect` часто настраивается через инспектор, скрипты позволяют управлять прокруткой программно:
```csharp
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Button scrollToTopButton;

    void Start()
    {
        scrollToTopButton.onClick.AddListener(ScrollToTop);
    }

    void ScrollToTop()
    {
        scrollRect.verticalNormalizedPosition = 1f; // 1 = верх, 0 = низ
    }
    
    public float GetScrollPosition()
    {
        return scrollRect.verticalNormalizedPosition;
    }
}
```

### 🔧 Настройка ScrollRect в инспекторе (важно!):
1. `Content` – объект с `RectTransform`, внутри которого находятся элементы списка. Его размер определяет область прокрутки.
2. `Viewport` – "окно", через которое видно контент.
3. `Horizontal` / `Vertical` – разрешить горизонтальную/вертикальную прокрутку.
4. `Scroll Sensitivity` – чувствительность колесика мыши.

### Пример:
Инвентарь из 20 предметов. Вы динамически создаёте 20 кнопок как дочерние объекты `Content`, а `ScrollRect` автоматически добавляет полосу прокрутки – игрок может пролистывать весь инвентарь.

---

## 🔗 Связка элементов в реальном проекте
### Часто UI-элементы работают вместе:
- **Slider** меняет значение текста `Text` (отображать текущую громкость в процентах).
- **Toggle** включает/выключает игровой объект (панель с дополнительными настройками).
- **Button** сохраняет все настройки или закрывает меню.
- **ScrollRect** содержит список уровней, а при клике на кнопку в списке загружается уровень.

### Пример кода для связки:
```csharp
public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle musicToggle;
    public Text volumePercentText;
    public Button saveButton;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(v => volumePercentText.text = Mathf.Round(v * 100) + "%");
        musicToggle.onValueChanged.AddListener(OnMusicToggled);
        saveButton.onClick.AddListener(SaveSettings);
    }

    void OnMusicToggled(bool isOn) { /* Выключить фон. музыку */ }
    void SaveSettings() { /* Сохранить все в PlayerPrefs */ }
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
