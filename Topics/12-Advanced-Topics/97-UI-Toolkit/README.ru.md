# 🎨 UI Toolkit: Новый способ создания UI в Unity (на подобие веб-технологий)
UI Toolkit — это современная система создания пользовательских интерфейсов в Unity, вдохновлённая веб-технологиями. 
Она приходит на смену старому Canvas (uGUI) и предлагает разделение структуры, стилей и логики — подобно HTML, CSS и JavaScript в веб-разработке.

> [!Important]
> Unity рекомендует использовать UI Toolkit для новых проектов, однако uGUI и IMGUI остаются актуальными для некоторых сценариев и поддержки устаревших проектов.

---

## 1. Основные концепции UI Toolkit
В основе UI Toolkit лежит система с сохранением состояния (retained mode) — в отличие от uGUI, 
где UI перерисовывается каждый кадр, здесь визуальное дерево один раз создаётся и хранится в памяти, а изменения применяются автоматически.

### Ключевые компоненты:

| Компонент | Описание | Аналог в вебе |
| --- | --- | --- |
| Visual Tree | Дерево визуальных элементов, определяющее структуру UI | DOM-дерево |
| UXML | Разметка для описания структуры UI | HTML |
| USS | Таблицы стилей для визуального оформления | CSS |
| C# Scripts | Логика и обработка событий | JavaScript |

UI Toolkit работает как в Editor, так и в runtime, позволяя создавать интерфейсы для игр, редакторские расширения и отладочные инструменты.

---

## 2. UXML — Структура UI (как HTML)
UXML (Unity eXtensible Markup Language) — это язык разметки, вдохновлённый HTML и XML. 
Он определяет структуру UI и позволяет создавать переиспользуемые шаблоны.

### 📝 Базовый пример UXML:
```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement name="root-container">
        <ui:Label name="title" text="Добро пожаловать!" />
        <ui:Button name="start-button" text="Начать игру" />
        <ui:TextField name="player-name" label="Имя игрока" />
    </ui:VisualElement>
</ui:UXML>
```

### 🔧 Создание UXML:
1. Вручную: создать файл с расширением `.uxml`
2. Через UI Builder: визуальный редактор, аналогичный конструктору веб-страниц

### 🧩 Наследование и шаблоны:
UXML поддерживает создание шаблонов для переиспользования:
```xml
<!-- Template: ButtonWithIcon.uxml -->
<ui:UXML>
    <ui:Button class="icon-button">
        <ui:Image name="icon" />
        <ui:Label name="button-text" />
    </ui:Button>
</ui:UXML>

<!-- Использование шаблона -->
<ui:Instance template="ButtonWithIcon">
    <ui:Label name="button-text" text="Сохранить" />
</ui:Instance>
```

---

## 3. USS — Стилизация UI (как CSS)
USS (Unity Style Sheets) — это таблицы стилей, которые применяют визуальные стили и правила компоновки к UI. 
Они поддерживают подмножество стандартных свойств CSS и работают на основе модели Flexbox.

### 📝 Базовый пример USS:
```css
/* Стиль для всего корневого контейнера */
#root-container {
    flex-grow: 1;
    background-color: rgb(30, 30, 45);
    padding: 20px;
}

/* Стиль для заголовка */
#title {
    font-size: 32px;
    color: rgb(255, 200, 50);
    -unity-font-style: bold;
    margin-bottom: 10px;
}

/* Класс для кнопок */
.primary-button {
    background-color: rgb(50, 120, 255);
    border-radius: 8px;
    padding: 10px 20px;
    font-size: 18px;
    color: white;
}

/* Псевдоклассы — как в CSS */
.primary-button:hover {
    background-color: rgb(80, 150, 255);
}

.primary-button:active {
    background-color: rgb(30, 90, 200);
}

/* Flexbox — управление раскладкой */
.horizontal-container {
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
}
```

### 🎯 Селекторы в USS:

| Селектор | Пример | Что выбирает |
| --- | --- | --- |
| Элемент | `Button` | Все элементы типа `Button` |
| Класс | `.button-primary` | Элементы с классом `button-primary` |
| ID | `#submit-btn` | Элемент с именем `submit-btn` |
| Псевдокласс | `:hover`, `:active`, `:disabled` | Состояния элемента |

### 📐 Модель Flexbox:
UI Toolkit использует Flexbox для позиционирования элементов — это делает интерфейс адаптивным и гибким, как в веб-дизайне.

```css
.flex-row {
    flex-direction: row;        /* Горизонтальное расположение */
    align-items: center;        /* Выравнивание по вертикали */
    justify-content: center;    /* Выравнивание по горизонтали */
    flex-wrap: wrap;            /* Перенос на новую строку */
}

.flex-column {
    flex-direction: column;     /* Вертикальное расположение */
    flex-grow: 1;               /* Занимает всё доступное место */
}
```

---

## 4. UIDocument — Подключение UI к сцене
UIDocument — это компонент, который подключает UI Toolkit к игровому объекту в сцене. Он является аналогом Canvas из uGUI.

### 🎮 Настройка в сцене:
1. GameObject → UI Toolkit → UI Document
2. В инспекторе назначить Source Asset — ваш `.uxml` файл
3. Опционально настроить Panel Settings (разрешение, масштабирование)

### 📝 Пример подключения:
```csharp
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private void OnEnable()
    {
        // Получаем корневой элемент визуального дерева
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;
        
        // Находим элементы по имени
        Button startButton = root.Q<Button>("start-button");
        Label titleLabel = root.Q<Label>("title");
        
        // Подписываемся на события
        startButton.clicked += OnStartButtonClicked;
    }
    
    private void OnStartButtonClicked()
    {
        Debug.Log("Игра начинается!");
    }
}
```

---

## 5. Обработка событий
События в UI Toolkit работают похоже на DOM-события в браузере — они всплывают по дереву и могут быть перехвачены на разных уровнях.

### 📝 Подписка на события:
```csharp
// В контроллере
private void RegisterEvents()
{
    // Поиск всех кнопок
    root.Query<Button>().ForEach(button => {
        button.RegisterCallback<ClickEvent>(OnButtonClick);
    });
    
    // Подписка на наведение
    VisualElement panel = root.Q<VisualElement>("panel");
    panel.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
    panel.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
}

private void OnButtonClick(ClickEvent evt)
{
    Button clicked = evt.currentTarget as Button;
    Debug.Log($"Нажата кнопка: {clicked.text}");
}
```

### 🌊 Всплытие событий:
```csharp
// Можно перехватывать события на родительском элементе
root.RegisterCallback<ClickEvent>(OnRootClick);

private void OnRootClick(ClickEvent evt)
{
    // evt.target — элемент, на котором произошёл клик
    Debug.Log($"Клик на элементе: {evt.target}");
}
```

---

## 6. Полный пример: Табовое меню
Создадим простое меню с тремя вкладками, используя все три компонента UI Toolkit.

### 📄 `TabbedMenu.uxml` (структура):
```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <Style src="TabbedMenu.uss" />
    
    <ui:VisualElement name="tabs">
        <ui:Label name="LondonTab" text="Лондон" class="tab currently-selected" />
        <ui:Label name="ParisTab" text="Париж" class="tab" />
        <ui:Label name="OttawaTab" text="Оттава" class="tab" />
    </ui:VisualElement>
    
    <ui:VisualElement name="tab-content">
        <ui:Label name="LondonContent" text="Лондон — столица Англии" />
        <ui:Label name="ParisContent" text="Париж — столица Франции" class="hidden" />
        <ui:Label name="OttawaContent" text="Оттава — столица Канады" class="hidden" />
    </ui:VisualElement>
</ui:UXML>
```

### 🎨 `TabbedMenu.uss` (стили):
```css
#tabs {
    flex-direction: row;
    background-color: rgb(229, 223, 223);
    font-size: 14px;
}

.tab {
    flex-grow: 1;
    padding: 10px;
    text-align: center;
}

.currently-selected {
    background-color: rgb(173, 166, 166);
}

#tab-content {
    background-color: rgb(255, 255, 255);
    font-size: 20px;
    padding: 20px;
}

.hidden {
    display: none;
}
```

### 💻 `TabbedMenuController.cs` (логика):
```csharp
using UnityEngine;
using UnityEngine.UIElements;

public class TabbedMenu : MonoBehaviour
{
    private void OnEnable()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;
        
        // Находим все вкладки
        root.Query<Label>(className: "tab").ForEach(tab => {
            tab.RegisterCallback<ClickEvent>(OnTabClick);
        });
    }
    
    private void OnTabClick(ClickEvent evt)
    {
        Label clickedTab = evt.currentTarget as Label;
        
        // Снимаем выделение со всех вкладок
        clickedTab.parent.Query<Label>().ForEach(tab => {
            tab.RemoveFromClassList("currently-selected");
        });
        
        // Выделяем нажатую вкладку
        clickedTab.AddToClassList("currently-selected");
        
        // Показываем нужный контент
        string contentName = clickedTab.name.Replace("Tab", "Content");
        VisualElement content = clickedTab.parent.parent.Q<Label>(contentName);
        
        // Скрываем весь контент, показываем нужный
        clickedTab.parent.parent.Q<VisualElement>("tab-content")
            .Query<Label>().ForEach(label => {
                label.AddToClassList("hidden");
            });
        
        content.RemoveFromClassList("hidden");
    }
}
```

---

## 7. Сравнение uGUI и UI Toolkit

| Аспект | uGUI (Canvas) | UI Toolkit |
| --- | --- | --- |
| Структура | GameObjects + Компоненты | Visual Tree (лёгкие узлы) |
| Разметка | Визуальный конструктор | UXML (текстовая разметка) |
| Стили | Инспектор | USS (CSS-подобный) |
| Производительность | Хорошо для простых UI | Лучше для сложных UI |
| Editor UI | ❌ Не поддерживается | ✅ Поддерживается |
| Масштабирование | Canvas Scaler | Flexbox |
| Отладка | Инспектор | UI Debugger (как в браузере) |
| Data Binding | ❌ Ручное обновление | ✅ Встроенная система |

---

## 8. Инструменты для работы с UI Toolkit
### 🛠️ UI Builder:
Визуальный редактор для создания UXML и USS без написания кода. 
Доступен через Window → UI Toolkit → UI Builder.

### 🐛 UI Debugger:
Инструмент отладки, похожий на "Инспектор элементов" в браузере. Позволяет исследовать визуальное дерево, смотреть применённые стили и проверять состояние элементов.

Доступен через Window → UI Toolkit → Debugger.

### 📚 UI Samples:
Библиотека готовых примеров использования UI Toolkit. 
Доступна через Window → UI Toolkit → Samples.

---

## 9. Лучшие практики
### ✅ Рекомендации:
1. Разделяйте структуру, стили и логику — UXML для разметки, USS для стилей, C# для логики
2. Используйте UI Builder для быстрого прототипирования
3. Изучите Flexbox — это ключ к адаптивному дизайну в UI Toolkit
4. Применяйте CSS-подход — переиспользуйте классы, используйте псевдоклассы (`:hover`, `:active`)
5. Для сложных UI используйте Data Binding для автоматического обновления данных

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Поиск элементов в неправильный момент
// UI ещё не создан, root может быть пустым
void Start()
{
    var button = GetComponent<UIDocument>().rootVisualElement.Q<Button>("btn");
}

// ✅ ПРАВИЛЬНО: В OnEnable или после создания UI
private void OnEnable()
{
    var root = GetComponent<UIDocument>().rootVisualElement;
    var button = root.Q<Button>("btn");
}

// ❌ ОШИБКА: Использование Transform для UI элементов
// Визуальные элементы не являются GameObjects
transform.Find("Button"); // Не работает!

// ✅ ПРАВИЛЬНО: Использовать Query
root.Q<Button>("start-button");
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
