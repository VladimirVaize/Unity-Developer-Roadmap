# 📘 Задание: Анимация UI-панели инвентаря с помощью DOTween
Вы разрабатываете RPG. Нужно реализовать анимацию панели инвентаря, которая появляется и исчезает с экрана. Панель должна:
- Появляться снизу экрана с отскоком (bounce) при нажатии кнопки "I" (Inventory)
- Исчезать с затуханием и сжатием при повторном нажатии
- Иметь анимацию пульсации иконки выбранного предмета
- Показывать всплывающий текст описания при наведении на предмет

---

## 🧱 Базовые требования (обязательная часть)
### 1. Панель инвентаря
- Создайте UI-панель `InventoryPanel` (Image) с фоном
- Изначально она находится за пределами экрана снизу (например, y = -500)
- При нажатии `I` панель должна вылетать на позицию y = 200
- Тип анимации: `Ease.OutBounce`
- Длительность: 0.6 секунды

```csharp
using DG.Tweening;

void ShowInventory() {
    inventoryPanel.transform.DOMoveY(200, 0.6f)
        .SetEase(Ease.OutBounce);
}
```

### 2. Скрытие панели
- При повторном нажатии `I` панель должна:
  - Сжаться по шкале X до 0 (за 0.2 сек)
  - Одновременно уменьшить прозрачность до 0 (`CanvasGroup`)
  - После завершения телепортироваться вниз и вернуть масштаб/прозрачность
 
- Длительность скрытия: 0.2 секунды

```csharp
void HideInventory() {
    Sequence hideSequence = DOTween.Sequence();
    hideSequence.Join(inventoryPanel.transform.DOScaleX(0, 0.2f));
    hideSequence.Join(canvasGroup.DOFade(0, 0.2f));
    hideSequence.OnComplete(() => {
        inventoryPanel.transform.position = offScreenBottom;
        inventoryPanel.transform.localScale = Vector3.one;
        canvasGroup.alpha = 1;
    });
}
```

### 3. Пульсация иконки
- В инвентаре есть 3 иконки предметов (`Image`)
- При клике на иконку она должна:
  - Увеличиться до 1.3x за 0.1 сек
  - Вернуться к 1.0x за 0.1 сек
  - Повторить пульсацию 2 раза (всего 3 цикла "туда-обратно")

- Тип зацикливания: `Yoyo`

```csharp
void PulseIcon(Transform icon) {
    icon.DOScale(1.3f, 0.1f)
        .SetLoops(6, LoopType.Yoyo);  // 3 туда-обратно = 6 шагов
}
```

### 4. Всплывающий текст
- При наведении мыши на предмет (OnPointerEnter)
- Создайте текстовую подсказку над иконкой
- Анимация текста:
  - Появление через 0.2 сек с увеличением от 0 до 1 (`Ease.OutBack`)
  - Исчезновение при уходе мыши (`Ease.InBack` + затухание)
 
---

## 🎨 Дополнительные задания (для саморазвития)
1. Анимация переключения предметов
   - При клике на предмет он перемещается в слот "экипировки"
   - Используйте `DOMove()` с `Ease.InOutCubic`
  
2. Тряска панели при попытке открыть с пустым инвентарём
   - Если инвентарь пуст, панель должна слегка трястись
   - `transform.DOShakePosition(0.3f, 10f, 20, 90)`
  
3. Кастомная анимация через AnimationCurve
   - Создайте в инспекторе `AnimationCurve` для нестандартной плавности
   - Примените её к анимации вылета панели вместо `OutBounce`
  
4. Последовательная анимация предметов
   - При открытии инвентаря предметы появляются не все сразу, а по очереди (с задержкой 0.05 сек между каждым)
  
---

## 🧪 Структура проекта
```text
Assets/
├── Scripts/
│   └── InventoryUI.cs
├── Prefabs/
│   └── ItemTooltip.prefab
└── Scenes/
    └── InventoryDemo.unity
```

---

## 🔧 Шаблон для начала (InventoryUI.cs)
```csharp
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryUI : MonoBehaviour
{
    [Header("Панели")]
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Иконки")]
    [SerializeField] private Transform[] itemIcons;
    
    [Header("Настройки анимации")]
    [SerializeField] private float showDuration = 0.6f;
    [SerializeField] private float hideDuration = 0.2f;
    [SerializeField] private float pulseDuration = 0.1f;
    
    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private bool isOpen = false;
    
    void Start()
    {
        hiddenPosition = new Vector2(inventoryPanel.anchoredPosition.x, -500);
        shownPosition = new Vector2(inventoryPanel.anchoredPosition.x, 200);
        inventoryPanel.anchoredPosition = hiddenPosition;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isOpen) HideInventory();
            else ShowInventory();
        }
    }
    
    void ShowInventory()
    {
        // TODO: Реализовать появление
        isOpen = true;
    }
    
    void HideInventory()
    {
        // TODO: Реализовать скрытие
        isOpen = false;
    }
    
    public void OnItemClick(int index)
    {
        // TODO: Пульсация иконки
    }
}
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
