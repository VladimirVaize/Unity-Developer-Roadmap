# 📦 Asset Store и пакеты: Package Manager, импорт ассетов, обновление зависимостей, создание собственных пакетов
Unity Package Manager (UPM) — это система управления пакетами, которая произвела революцию в работе с ассетами и зависимостями в Unity. 
Вместо старого подхода "скачай и импортируй в Assets", UPM предлагает централизованное управление версиями, автоматическое обновление зависимостей и чистую структуру проекта.

---

## 1. Unity Package Manager (UPM) — основные понятия
Package Manager — это инструмент для добавления, обновления и удаления пакетов в Unity-проекте. 
Он заменяет устаревший Asset Store встроенным решением с поддержкой версионирования.

### 📁 Структура пакета UPM:
```text
MyPackage/
├── package.json           # 📄 Манифест пакета (обязателен)
├── Runtime/               # 🔧 Код, выполняемый в рантайме
│   ├── Scripts/
│   └── Prefabs/
├── Editor/                # 🛠️ Код для редактора (не попадает в сборку)
│   └── EditorScripts/
├── Documentation~/        # 📚 Документация (опционально)
├── Tests/                 # 🧪 Тесты (опционально)
│   ├── Editor/
│   └── Runtime/
├── Samples~/              # 📂 Примеры использования (опционально)
└── Resources/             # 📀 Ресурсы
```

### 📄 package.json — манифест пакета:
```json
{
    "name": "com.mycompany.mypackage",
    "version": "1.0.0",
    "displayName": "My Custom Package",
    "description": "A package for handling game data serialization",
    "unity": "2021.3",
    "unityRelease": "0f1",
    "dependencies": {
        "com.unity.textmeshpro": "3.0.6",
        "com.unity.ugui": "1.0.0"
    },
    "keywords": [
        "data",
        "serialization",
        "json"
    ],
    "author": {
        "name": "MyCompany",
        "email": "support@mycompany.com",
        "url": "https://www.mycompany.com"
    },
    "samples": [
        {
            "displayName": "Basic Example",
            "description": "Basic usage example",
            "path": "Samples~/BasicExample"
        }
    ]
}
```

---

## 2. Работа с Package Manager
### 🖥️ Интерфейс Package Manager:
Доступ: `Window → Package Manager`

| Вкладка | Описание |
| --- | --- |
| Packages: In Project | Пакеты, уже добавленные в проект |
| Packages: My Assets | Ваши покупки в Asset Store |
| Packages: Built-in | Встроенные пакеты Unity |
| Packages: Unity Registry | Официальные пакеты от Unity |
| Packages: My Registries | Пользовательские реестры |
| Packages: Asset Store | Пакеты из Asset Store |

### 📦 Добавление пакета через интерфейс:
1. Открыть `Window → Package Manager`
2. Выбрать источник (Unity Registry / My Assets)
3. Найти нужный пакет
4. Нажать Install

### 📦 Добавление пакета через Git URL:
Способ 1: Через интерфейс
1. Package Manager → "➕" → "Add package from git URL"
2. Ввести URL, например: `https://github.com/Unity-Technologies/InputSystem.git`

Способ 2: Через manifest.json
```json
// Project/Packages/manifest.json
{
    "dependencies": {
        "com.unity.inputsystem": "https://github.com/Unity-Technologies/InputSystem.git",
        "com.mycompany.custom": "https://github.com/mycompany/custom-package.git#v1.2.0"
    }
}
```

### 📦 Добавление пакета локально (для разработки):
```json
// Project/Packages/manifest.json
{
    "dependencies": {
        "com.mycompany.localpackage": "file:../MyLocalPackage"
    }
}
```

### 📦 Добавление пакета из Asset Store:
1. Купить ассет в Asset Store (через браузер или в окне Unity)
2. В Unity: `Window → Asset Store` → найти ассет → Download
3. После загрузки: Import

> [!Important]
> Ассеты из Asset Store импортируются в папку `Assets/` и НЕ являются UPM-пакетами.
> Это старая система, но она всё ещё используется для большинства коммерческих ассетов.

---

## 3. Импорт ассетов
### 🏪 Импорт из Asset Store:
1. Открыть `Window → Asset Store`
2. Найти ассет
3. Скачать (Download)
4. Импортировать (Import)
5. Выбрать, какие файлы импортировать

```csharp
// Пример скрипта для импорта ассета через код (используется в CI/CD)
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

public class AssetImporter
{
    [MenuItem("Tools/Import Asset")]
    public static void ImportAsset()
    {
        // Добавление пакета через UPM (если это UPM-пакет)
        // Для Asset Store ассетов используется другой подход
        AddRequest request = Client.Add("com.unity.textmeshpro");
        // Ждём завершения...
    }
}
#endif
```

### 📂 Организация импортированных ассетов:
```text
Assets/
├── _MyProject/                # 📁 Основные ассеты проекта
│   ├── Scripts/
│   ├── Scenes/
│   └── Prefabs/
├── Plugins/                   # 🔌 Плагины (для всех платформ)
├── TextMesh Pro/              # 📝 Ассет из Asset Store
├── DOTween/                   # 🌀 Ассет из Asset Store
└── Packages/                  # 📦 Симлинки на UPM-пакеты (автоматически)
```

> [!Tip]
> Создайте папку `_Project` для своих файлов, чтобы отделить их от импортированных ассетов.
> Это упрощает поиск и резервное копирование.

### ⚠️ Проблемы при импорте:
1. Конфликты версий — два пакета требуют разные версии одной зависимости
2. Дублирование DLL — в разных пакетах могут быть одинаковые библиотеки
3. Сборка (Assembly Definition) — импортированные ассеты могут не работать с вашими asmdef

---

## 4. Обновление зависимостей
### 🔄 Обновление пакета в UPM:
1. Открыть `Package Manager`
2. Выбрать пакет из списка In Project
3. Нажать Update (если доступно)

### 📝 Обновление через manifest.json:
```json
// Было:
"com.unity.textmeshpro": "3.0.6"

// Стало:
"com.unity.textmeshpro": "3.0.8"  // Обновили версию
```

### 🧩 Разрешение конфликтов зависимостей:
```json
// Project/Packages/manifest.json
{
    "dependencies": {
        // Прямая зависимость
        "com.mycompany.mypackage": "1.0.0",
        
        // Переопределение версии для разрешения конфликта
        "com.unity.textmeshpro": "3.0.8"
    }
}
```

### 🛠️ Команды для управления зависимостями (CLI):
```bash
# Установка пакета через командную строку
unity-editor -projectPath ./MyProject -executeMethod PackageManagerHelper.InstallPackage

# Сброс кеша пакетов
rm -rf Library/PackageCache
# Переоткрыть проект — пакеты перезагрузятся
```

---

## 5. Создание собственных пакетов
### 🚀 Шаг 1: Создание структуры пакета
```bash
# Создаём папку для пакета
mkdir MyPackage
cd MyPackage

# Создаём структуру
mkdir Runtime
mkdir Editor
mkdir Documentation~
mkdir Samples~
mkdir Tests

# Создаём манифест
touch package.json
```

### 📄 Шаг 2: Заполнение package.json
```json
{
    "name": "com.mycompany.utility",
    "version": "1.0.0",
    "displayName": "My Utility Package",
    "description": "A collection of reusable Unity utilities",
    "unity": "2021.3",
    "dependencies": {
        "com.unity.textmeshpro": "3.0.6"
    },
    "keywords": ["utility", "tools", "helpers"],
    "author": {
        "name": "MyCompany",
        "email": "dev@mycompany.com",
        "url": "https://mycompany.com"
    }
}
```

### 🧩 Шаг 3: Добавление кода
Runtime/Scripts/MyHelper.cs:
```csharp
using UnityEngine;

namespace MyCompany.Utility
{
    public static class MyHelper
    {
        public static void LogMessage(string message)
        {
            Debug.Log($"[MyPackage] {message}");
        }
        
        public static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null)
                component = obj.AddComponent<T>();
            return component;
        }
    }
}
```

Editor/MyPackageMenu.cs:
```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyCompany.Utility.Editor
{
    public static class MyPackageMenu
    {
        [MenuItem("Tools/My Package/Hello World")]
        public static void HelloWorld()
        {
            MyHelper.LogMessage("Hello from My Package!");
        }
    }
}
#endif
```

### 🧪 Шаг 4: Добавление примеров (Samples)
Создайте папку `Samples~/BasicExample/` с содержимым:
- `BasicExample.unity`
- `ExampleScript.cs`

В `package.json` добавьте:
```json
"samples": [
    {
        "displayName": "Basic Example",
        "description": "Shows how to use the utility package",
        "path": "Samples~/BasicExample"
    }
]
```

После импорта пакета в проект, в Package Manager появится кнопка Import для примера.

### 📦 Шаг 5: Добавление тестов
Tests/Runtime/MyHelperTests.cs:
```csharp
using NUnit.Framework;
using MyCompany.Utility;

public class MyHelperTests
{
    [Test]
    public void LogMessage_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => MyHelper.LogMessage("Test"));
    }
}
```

### 🏷️ Шаг 6: Публикация пакета
Вариант 1: Локальный пакет
```json
// В manifest.json проекта
"dependencies": {
    "com.mycompany.utility": "file:../MyPackage"
}
```

Вариант 2: Git-репозиторий
```bash
git init
git add .
git commit -m "Initial package commit"
git tag v1.0.0
git remote add origin https://github.com/mycompany/MyPackage.git
git push -u origin main --tags
```

```json
// В manifest.json
"dependencies": {
    "com.mycompany.utility": "https://github.com/mycompany/MyPackage.git#v1.0.0"
}
```

Вариант 3: Публичный реестр (npm / OpenUPM)
```bash
# Публикация на OpenUPM
npx openupm publish
```

---

## 6. Создание Assembly Definition для пакета
Чтобы пакет был изолирован, добавьте `Assembly Definition` файлы:

Runtime/MyPackage.Runtime.asmdef:
```json
{
    "name": "MyCompany.Utility.Runtime",
    "rootNamespace": "MyCompany.Utility",
    "references": [],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": []
}
```

Editor/MyPackage.Editor.asmdef:
```json
{
    "name": "MyCompany.Utility.Editor",
    "rootNamespace": "MyCompany.Utility.Editor",
    "references": [
        "MyCompany.Utility.Runtime"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": []
}
```

---

## 7. Обновление и поддержка пакетов
### 📋 Семантическое версионирование (SemVer):

| Версия | Что означает |
| --- | --- |
| 1.0.0 | Первая стабильная версия |
| 1.0.1 | Исправление багов (не ломает API) |
| 1.1.0 | Новая функциональность (не ломает API) |
| 2.0.0 | Ломающие изменения (API изменён) |

### 🔄 Обновление пакета:
1. Изменить версию в `package.json`
2. Создать Git-тег: `git tag v1.1.0`
3. Запушить тег: `git push --tags`
4. В проекте: `Package Manager → Update`

### 🧹 Удаление устаревших пакетов:
```json
// Удалить строку из manifest.json
// Или через Package Manager: Remove
```

---

## 8. Лучшие практики
### ✅ Рекомендации:
1. Используйте SemVer для версионирования пакетов
2. Документируйте изменения в `CHANGELOG.md`
3. Используйте Assembly Definitions для изоляции кода
4. Добавляйте примеры в `Samples~` для демонстрации использования
5. Тестируйте пакет перед публикацией (в отдельном проекте)
6. Храните исходники в Git с тегами версий
7. Для коммерческих пакетов используйте Asset Store Tool

### ❌ Частые ошибки:
```csharp
// ❌ ОШИБКА: Жёсткая привязка к версии Unity
"unity": "2022.3"  // Слишком конкретно

// ✅ ПРАВИЛЬНО:
"unity": "2021.3"  // Минимальная версия

// ❌ ОШИБКА: Пакет использует код из папки Assets/
// Нельзя ссылаться на файлы вне пакета

// ❌ ОШИБКА: Отсутствие asmdef
// Пакет добавляет все скрипты в Assembly-CSharp

// ❌ ОШИБКА: Забыли добавить Samples~ в .gitignore?
// Sample-папка с ~ игнорируется Unity при импорте
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
