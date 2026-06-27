# 🎯 Задача: «Создание панели персонажа с использованием UI Toolkit»
Вы разрабатываете RPG игру и вам нужно создать панель персонажа (Character Panel), используя UI Toolkit. Панель должна отображать:
1. Аватар (изображение)
2. Имя персонажа (текстовое поле)
3. Уровень (число)
4. Полоски здоровья и маны (прогресс-бары)
5. Кнопку "Прокачка" (увеличивает уровень)

## 📝 Что нужно реализовать:
### Часть 1: Создание UXML (структура)
Создайте файл `CharacterPanel.uxml` со следующей структурой:
```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <!-- Подключение стилей -->
    <Style src="CharacterPanel.uss" />
    
    <ui:VisualElement name="panel">
        <!-- Шапка: аватар + имя + уровень -->
        <ui:VisualElement name="header">
            <ui:VisualElement name="avatar" />
            <ui:VisualElement name="info">
                <ui:Label name="character-name" text="Герой" />
                <ui:Label name="level-label" text="Уровень: 1" />
            </ui:VisualElement>
        </ui:VisualElement>
        
        <!-- Полоски здоровья и маны -->
        <ui:VisualElement name="stats">
            <ui:VisualElement name="health-bar" class="progress-bar">
                <ui:VisualElement name="health-fill" class="progress-fill health-fill" />
                <ui:Label name="health-text" text="100/100" />
            </ui:VisualElement>
            
            <ui:VisualElement name="mana-bar" class="progress-bar">
                <ui:VisualElement name="mana-fill" class="progress-fill mana-fill" />
                <ui:Label name="mana-text" text="50/50" />
            </ui:VisualElement>
        </ui:VisualElement>
        
        <!-- Кнопка действия -->
        <ui:Button name="level-up-btn" text="Повысить уровень" class="primary-button" />
    </ui:VisualElement>
</ui:UXML>
```

### Часть 2: Создание USS (стили)
Создайте файл `CharacterPanel.uss` со следующими стилями:
- Панель: тёмный фон, скруглённые углы, отступы
- Аватар: квадрат 64×64, серая заливка, скруглённые углы
- Полоски здоровья и маны: ширина 100%, высота 20px, фон тёмный
- Заполнение полоски здоровья: красный цвет (`#e74c3c`)
- Заполнение полоски маны: синий цвет (`#3498db`)
- Кнопка: зелёная, с эффектом наведения
- Используйте Flexbox для расположения элементов

### Часть 3: Создание скрипта управления
Напишите скрипт `CharacterPanelController.cs`:
1. Получает ссылки на все элементы UI через `root.Q<>()` и `root.Query<>()`
2. Реализует метод `UpdateUI()` для обновления отображения
3. Обрабатывает нажатие кнопки "Повысить уровень"
4. При нажатии:
   - Увеличивает уровень на 1
   - Увеличивает максимальное здоровье на 10
   - Увеличивает максимальную ману на 5
   - Восстанавливает здоровье и ману до максимума
   - Обновляет UI
  
### Часть 4: Модель данных
Создайте класс `CharacterData`:
```csharp
[System.Serializable]
public class CharacterData
{
    public string characterName = "Герой";
    public int level = 1;
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int maxMana = 50;
    public int currentMana = 50;
}
```

### Часть 5: Интеграция в сцену
1. Добавьте UI Document на GameObject в сцене
2. Назначьте созданный `CharacterPanel.uxml` как Source Asset
3. Назначьте скрипт `CharacterPanelController` на тот же GameObject

---

## 🧰 Требования к реализации:
- Используйте все три компонента UI Toolkit: UXML, USS, C#
- Примените Flexbox для расположения элементов
- Используйте псевдоклассы (`:hover`, `:active`) для кнопки
- Добавьте обработку событий через `RegisterCallback<>`
- UI должен быть адаптивным (менять размер при изменении окна)

---

## 🔍 Проверка работоспособности:
1. Запустите сцену — панель должна отображаться корректно
2. Нажмите кнопку "Повысить уровень" — уровень должен увеличиться
3. Проверьте, что полоски здоровья и маны обновляются
4. Проверьте, что кнопка меняет цвет при наведении
5. Откройте UI Debugger и исследуйте визуальное дерево

---

## 💡 Ожидаемый результат:
```text
=== Панель персонажа ===
Имя: Герой
Уровень: 1 → 2 → 3 ...
Здоровье: 100/100 → 110/110 → 120/120 ...
Мана: 50/50 → 55/55 → 60/60 ...

=== При нажатии "Повысить уровень" ===
- Уровень увеличивается
- Полоски заполняются до максимума
- Кнопка даёт визуальный отклик
```

---

## 🏆 Дополнительное задание (опционально):
1. Data Binding: свяжите модель данных с UI автоматически
2. Анимации: добавьте плавное заполнение полосок здоровья через USS Transitions
3. Темы: создайте светлую и тёмную тему, переключаемые через кнопку

```css
/* Пример CSS-перехода для плавного заполнения */
.health-fill {
    transition: width 0.3s ease;
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
