# 📘 Assignment: UI Inventory Panel Animation with DOTween
You are developing an RPG. You need to implement an inventory panel animation that appears and disappears from the screen. The panel should:
- Appear from the bottom of the screen with a bounce effect when pressing "I" (Inventory)
- Disappear with fading and shrinking on second press
- Have a pulsation animation for the selected item icon
- Show a floating tooltip text when hovering over an item

---

## 🧱 Basic Requirements (Mandatory)
### 1. Inventory Panel
- Create a UI panel `InventoryPanel` (Image) with a background
- Initially it is below the screen (e.g., y = -500)
- When pressing `I`, the panel should fly to position y = 200
- Animation Type: `Ease.OutBounce`
- Duration: 0.6 seconds

```csharp
using DG.Tweening;

void ShowInventory() {
    inventoryPanel.transform.DOMoveY(200, 0.6f)
        .SetEase(Ease.OutBounce);
}
```

### 2. Hiding the Panel
- On second `I` press, the panel should:
  - Shrink X scale to 0 (in 0.2 sec)
  - Simultaneously fade to 0 opacity (`CanvasGroup`)
  - Teleport down and reset scale/opacity after completion
 
- Hide Duration: 0.2 seconds

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

### 3. Icon Pulsation
- The inventory has 3 item icons (`Image`)
- On icon click, it should:
  - Scale up to 1.3x in 0.1 sec
  - Return to 1.0x in 0.1 sec
  - Repeat pulsation 2 times (total 3 back-and-forth cycles)
 
- Loop Type: `Yoyo`

```csharp
void PulseIcon(Transform icon) {
    icon.DOScale(1.3f, 0.1f)
        .SetLoops(6, LoopType.Yoyo);  // 3 back-forth = 6 steps
}
```

### 4. Floating Tooltip
- On mouse hover over an item (OnPointerEnter)
- Create a text tooltip above the icon
- Text animation:
  - Appear in 0.2 sec with scale from 0 to 1 (`Ease.OutBack`)
  - Disappear on mouse exit (`Ease.InBack` + fade)
 
---

## 🎨 Bonus Tasks (Self-Development)
1. Item Switch Animation
   - On click, the item moves to the "equipment" slot
   - Use `DOMove()` with `Ease.InOutCubic`
  
2. Panel Shake on Empty Inventory Open
   - If inventory is empty, the panel should shake slightly
   - `transform.DOShakePosition(0.3f, 10f, 20, 90)`
  
3. Custom AnimationCurve
   - Create an `AnimationCurve` in the Inspector for non-standard easing
   - Apply it to the panel fly-out animation instead of `OutBounce`
  
4. Sequential Item Animation
   - When opening inventory, items appear not all at once, but one after another (0.05 sec delay between each)
  
---

## 🧪 Project Structure
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

## 🔧 Starter Template (InventoryUI.cs)
```csharp
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Icons")]
    [SerializeField] private Transform[] itemIcons;
    
    [Header("Animation Settings")]
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
        // TODO: Implement appearance
        isOpen = true;
    }
    
    void HideInventory()
    {
        // TODO: Implement hiding
        isOpen = false;
    }
    
    public void OnItemClick(int index)
    {
        // TODO: Icon pulsation
    }
}
```

---

### ⭐ If this project was useful, put a star on GitHub!
