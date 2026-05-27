# 🧪 Практическое задание: URP + Render Feature «Подсветка при наведении»

Цель: Настроить URP в проекте, создать простую сцену с несколькими кубами и реализовать эффект подсветки объекта при наведении мыши без написания шейдера — только через Render Feature и стандартные материалы.

---

## 📌 Задача (пошагово)
### 🔧 Часть 1: Настройка URP
1. Создайте новый проект Unity (3D шаблон) или используйте существующий.
2. Создайте URP Asset и назначьте его в Project Settings (как описано в теории).
3. Обновите материалы сцены (если нужно).

### 🧱 Часть 2: Создание сцены
4. Создайте 3–5 кубов (`GameObject → 3D Object → Cube`) на разных позициях.
5. Назначьте каждому кубу разный стандартный цветной материал (например, красный, синий, зелёный) — используйте URP Lit Shader.
6. Добавьте `Camera` (уже есть) и `Directional Light`.

### ✨ Часть 3: Создание Render Feature «Highlight On Hover»
Задача: При наведении курсора мыши на куб – куб должен подсвечиваться ярко-белым контуром или заливкой без смены основного материала.

🛠️ Реализация через Render Objects Feature:
1. Выберите Forward Renderer Asset (обычно он находится внутри URP Asset).
2. Нажмите `Add Render Feature` → выберите `Render Objects`.
3. Назовите её `Highlight Feature`.
4. Настройте:
   - `Event` = `AfterRenderingOpaques` (после обычных непрозрачных объектов).
   - `Filters` → `Layer Mask` = создайте новый слой `Highlightable` и назначьте его всем кубам.
   - `Overrides` → `Material` = создайте новый материал (URP Lit), цвет белый, `Surface Type` = `Opaque`, `Emission` = белый (или просто яркий цвет).
   - `Render Queue` = `Geometry+1` (чтобы рисовалось поверх).
  
5. Пока оставьте фичу выключенной (галочка слева от названия).

### 🖱️ Часть 4: Скриптинг (включение/выключение фичи)
6. Создайте C# скрипт `HighlightOnHover.cs` и повесьте его на кубы.
```csharp
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HighlightOnHover : MonoBehaviour
{
    private UniversalAdditionalCameraData cameraData;
    private ScriptableRendererFeature highlightFeature;

    void Start()
    {
        cameraData = Camera.main.GetUniversalAdditionalCameraData();
        // Находим нашу Render Feature по имени (замените "Highlight Feature" на ваше название)
        var renderer = cameraData.scriptableRenderer as UniversalRenderer;
        if (renderer != null)
        {
            highlightFeature = renderer.rendererFeatures.Find(f => f.name == "Highlight Feature");
        }
    }

    void OnMouseEnter()
    {
        if (highlightFeature != null) highlightFeature.SetActive(true);
    }

    void OnMouseExit()
    {
        if (highlightFeature != null) highlightFeature.SetActive(false);
    }
}
```

> 💡 Примечание: Доступ к Render Feature через код требует небольшой доработки (можно использовать публичное статическое поле).
> Для простоты можно вместо включения/выключения фичи просто менять материал объекта временно – но по условию мы учимся работать именно с Render Features.

#### Альтернативный простой способ (без поиска фичи):
В `OnMouseEnter()` создать временный объект-дублёр с белым материалом, а в `OnMouseExit()` удалить. Но это костыль. Правильный способ – включение фичи.

---

## 🎯 Ожидаемый результат:
- При наведении мыши на любой куб весь куб подсвечивается ярким цветом (белым/жёлтым).
- При уводе мыши – подсветка исчезает.
- Основные материалы кубов не меняются.
- Эффект работает без потери производительности.

---

## ⭐ Дополнительное задание (повышенной сложности):
- Сделайте так, чтобы подсвечивался только край объекта (outline эффект) через Render Feature с материалом `Unlit/Texture` и наложением через `Stencil`.
- Добавьте вторую Render Feature – размытие в движении (`Motion Blur`) для всей камеры, но только когда игрок зажимает клавишу Shift.

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
