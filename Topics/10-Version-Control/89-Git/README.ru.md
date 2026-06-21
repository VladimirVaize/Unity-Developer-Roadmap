# 🔀 Git для Unity: .gitignore, работа со сценами (Smart Merge), конфликты префабов, Git LFS для больших ассетов
Unity и Git — мощная комбинация, но требуют правильной настройки из-за бинарных файлов, 
сложной структуры сцен и префабов. В этом руководстве мы разберём все аспекты эффективной работы с Git в Unity-проектах.

---

## 1. .gitignore для Unity — что НЕ нужно хранить в репозитории
Многие файлы Unity генерируются автоматически или являются кешем. 
Их хранение в Git раздувает репозиторий и создаёт конфликты.

### 📁 Базовая структура Unity-проекта:
```text
MyUnityProject/
├── Assets/               # 🔥 ВСЁ ВАЖНОЕ ЗДЕСЬ
│   ├── Scripts/
│   ├── Scenes/
│   ├── Prefabs/
│   ├── Textures/
│   └── ...
├── Packages/             # 🟡 Манифесты пакетов (хранить)
├── ProjectSettings/      # 🟡 Настройки проекта (хранить)
├── Library/              # ❌ НЕ ХРАНИТЬ (кеш, метаданные)
├── Temp/                 # ❌ НЕ ХРАНИТЬ
├── Logs/                 # ❌ НЕ ХРАНИТЬ
└── UserSettings/         # ❌ НЕ ХРАНИТЬ (локальные настройки)
```

### 📄 Полный .gitignore для Unity:
```gitignore
# ==========================================
# 🔴 Unity генерируемые папки (НЕ ХРАНИТЬ)
# ==========================================
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/
/[Mm]emoryCaptures/

# ==========================================
# 🔴 Unity метафайлы с особыми суффиксами
# ==========================================
*.cs.meta
*.unity.meta
*.prefab.meta
*.asset.meta
*.mat.meta
*.anim.meta
*.controller.meta

# ==========================================
# 🔴 Автоматически генерируемые файлы
# ==========================================
*.pidb.meta
*.pdb.meta
*.mdb.meta
*.exe.meta
*.dll.meta

# ==========================================
# 🔴 Папки плагинов для конкретных платформ
# ==========================================
[Ll]ibrary/Plugins/[Aa]ndroid/
[Ll]ibrary/Plugins/[Ii]OS/

# ==========================================
# 🔴 Файлы Visual Studio / Rider
# ==========================================
*.csproj
*.unityproj
*.sln
*.suo
*.user
*.userprefs
*.pidb
*.swp
*.slnf

# ==========================================
# 🔴 Папка для Git LFS (если используется)
# ==========================================
/lfs/

# ==========================================
# 🔴 Логи и кеш
# ==========================================
*.log
*.cache
*.tmp
*.temp

# ==========================================
# 🔴 Системные файлы
# ==========================================
.DS_Store
Thumbs.db
Desktop.ini

# ==========================================
# 🔴 Rider / VS Code
# ==========================================
.idea/
.vs/
.vscode/
*.code-workspace
```

### 🛠️ Как применить .gitignore:
1. Создайте файл `.gitignore` в корне проекта
2. Вставьте содержимое выше
3. Выполните `git add .gitignore && git commit -m "Add .gitignore"`
4. Если файлы уже в репозитории, удалите их: `git rm -r --cached Library/`

---

## 2. Работа со сценами (Scenes) в Git — Smart Merge
Сцены Unity хранятся в формате YAML (текстовом), но с большим количеством идентификаторов объектов (GUID). 
Это делает их сложными для стандартного слияния.

### 🔍 Проблема со сценами:
```yaml
--- !u!1 &123456789
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 987654321}  # <--- GUID меняется при изменении
  m_Layer: 0
  m_Name: Player
```

Проблема: При изменении сцены GUID объектов и компонентов меняются. 
Стандартный `git merge` не понимает структуру YAML и выдаёт конфликты на каждый изменённый GUID.

### ✅ Решение: Smart Merge (UnityYAMLMerge)
Unity предоставляет инструмент UnityYAMLMerge, который понимает структуру YAML и выполняет "умное" слияние сцен, префабов и ассетов.

### ⚙️ Настройка UnityYAMLMerge:
Для всех проектов (глобально):
```bash
# Настройка в глобальном конфиге Git
git config --global merge.unityyamlmerge.name "Unity Smart Merge (YAML)"
git config --global merge.unityyamlmerge.driver 'C:/Program Files/Unity/Hub/Editor/2022.3.21f1/Editor/Data/Tools/UnityYAMLMerge.exe merge --force -p --fallback base "%P" "%O" "%B"'
```

Для конкретного проекта (локально):
```bash
# Создать файл .gitattributes в корне проекта
echo "*.unity merge=unityyamlmerge" >> .gitattributes
echo "*.prefab merge=unityyamlmerge" >> .gitattributes
echo "*.asset merge=unityyamlmerge" >> .gitattributes
```

### 📄 Полный .gitattributes:
```gitattributes
# ==========================================
# 🟢 Unity Smart Merge для YAML-файлов
# ==========================================
*.unity merge=unityyamlmerge
*.prefab merge=unityyamlmerge
*.asset merge=unityyamlmerge
*.mat merge=unityyamlmerge
*.anim merge=unityyamlmerge
*.controller merge=unityyamlmerge

# ==========================================
# 🟢 Git LFS для больших файлов
# ==========================================
*.psd filter=lfs diff=lfs merge=lfs -text
*.tga filter=lfs diff=lfs merge=lfs -text
*.png filter=lfs diff=lfs merge=lfs -text
*.jpg filter=lfs diff=lfs merge=lfs -text
*.jpeg filter=lfs diff=lfs merge=lfs -text
*.fbx filter=lfs diff=lfs merge=lfs -text
*.obj filter=lfs diff=lfs merge=lfs -text
*.blend filter=lfs diff=lfs merge=lfs -text
*.3ds filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text
*.ogg filter=lfs diff=lfs merge=lfs -text
*.aiff filter=lfs diff=lfs merge=lfs -text
*.avi filter=lfs diff=lfs merge=lfs -text
*.mov filter=lfs diff=lfs merge=lfs -text
*.mp4 filter=lfs diff=lfs merge=lfs -text
*.unitypackage filter=lfs diff=lfs merge=lfs -text
```

### 🎮 Как работает Smart Merge:
```text
Ваша ветка (main)          Ветка коллеги (feature)
     |                           |
  Scene.unity               Scene.unity
     |                           |
     +--------[ MERGE ]---------+
                |
    Smart Merge сравнивает объекты по GUID,
    а не построчно. Конфликтуют только
    действительно изменённые объекты.
```

Результат: Обычно 90% конфликтов разрешаются автоматически, остальные 10% — ручное вмешательство.

### 🛠️ Ручное разрешение конфликтов в сцене:
Если Smart Merge не справился:
1. Откройте файл сцены в текстовом редакторе
2. Найдите секции `<<<<<<< HEAD` и `>>>>>>> feature`
3. Определите, какой вариант оставить (или объединить)
4. Удалите маркеры конфликта
5. Пересоберите сцену в Unity

```yaml
<<<<<<< HEAD
  m_Name: PlayerV2
=======
  m_Name: Player
>>>>>>> feature
```

---

## 3. Конфликты префабов (Prefab Conflicts)
Префабы — это многократно используемые объекты. Их конфликты возникают чаще всего.

### 🔍 Типичные конфликты префабов:
1. Изменение иерархии (добавление/удаление дочерних объектов)
2. Изменение компонентов (добавление/удаление скриптов)
3. Изменение значений полей (public переменные)

### 📝 Пример конфликта:
```yaml
# Оригинальный префаб (base)
Transform:
  m_LocalPosition: {x: 0, y: 0, z: 0}

# Ветка A (изменили позицию)
Transform:
  m_LocalPosition: {x: 10, y: 0, z: 0}

# Ветка B (изменили масштаб)
Transform:
  m_LocalScale: {x: 2, y: 2, z: 2}

# MERGE: Оба изменения конфликтуют!
```

### ✅ Решение: Правильная работа с префабами
1. Используйте Variant Prefabs (Префабы-варианты):
   - Базовый префаб хранит общие данные
   - Варианты наследуют и переопределяют только нужное
  
2. Разбивайте большие префабы на маленькие:
```text
❌ Плохо: Enemy (всё в одном префабе)
✅ Хорошо: Enemy → EnemyModel, EnemyController, EnemyHealth
```

3. Используйте Addressables для больших ассетов
4. Коммуникация в команде:
   - Кто работает над префабом — сообщает остальным
   - Делайте маленькие коммиты с описанием изменений
  
### 🛠️ Разрешение конфликтов префабов:
```bash
# 1. Посмотреть конфликтующие файлы
git status

# 2. Открыть префаб в Unity (он покажет конфликт)
# 3. В Unity открыть окно "Resolve Prefab Conflicts"
# 4. Выбрать вариант (свой, их, или объединить)
# 5. Сохранить и закоммитить
```

---

## 4. Git LFS для больших ассетов
Git LFS (Large File Storage) — расширение Git для работы с большими файлами.

### 📊 Статистика:
| Файл | Размер | Git (без LFS) | Git LFS |
| --- | --- | --- | --- |
| Текстура (PNG) | 10 MB | ⚠️ Раздувает репо | ✅ Хранится отдельно |
| Модель (FBX) | 50 MB | ❌ Проблемы с производительностью	 | ✅ Оптимально |
| Видео (MP4) | 100 MB | ❌ Невозможно хранить | ✅ Отлично работает |
| Звук (WAV) | 20 MB | ❌ Медленный клон | ✅ Быстрый клон |

### 🚀 Установка Git LFS:
```bash
# 1. Установить Git LFS
# Windows: https://git-lfs.github.com/
# macOS: brew install git-lfs
# Linux: sudo apt install git-lfs

# 2. Инициализация в репозитории
git lfs install

# 3. Добавить файлы для отслеживания
git lfs track "*.psd"
git lfs track "*.png"
git lfs track "*.fbx"
git lfs track "*.wav"
git lfs track "*.mp4"

# 4. Проверить настройки
git lfs track

# 5. Добавить .gitattributes в репозиторий
git add .gitattributes
git commit -m "Add Git LFS configuration"

# 6. Клонировать репозиторий с LFS
git clone <repository-url>
git lfs pull  # Скачать LFS-файлы
```

### 📄 Настройка Git LFS в Unity-проекте:
```bash
# Установка LFS для всего проекта
git lfs track "Assets/Textures/**/*.png"
git lfs track "Assets/Models/**/*.fbx"
git lfs track "Assets/Audio/**/*.wav"
git lfs track "Assets/Animations/**/*.anim"
git lfs track "Packages/*.tgz"

# Проверить, какие файлы уже в LFS
git lfs ls-files

# Миграция существующих файлов в LFS
git lfs migrate import --include="*.png,*.fbx,*.wav"
```

### ⚡ Git LFS в команде:
1. Все разработчики должны установить Git LFS
2. При клонировании: `git clone && git lfs pull`
3. При пуше: LFS-файлы загружаются автоматически
4. Квоты: На GitHub/GitLab есть лимиты (2-10 GB)

### 🚫 Что НЕ нужно хранить в Git LFS:
- Метафайлы Unity (`.meta`) — они маленькие
- Исходники скриптов (`.cs`) — они текстовые
- Файлы конфигурации (`.json`, `.xml`) — они текстовые

### 🔍 Пример: Сравнение репозиториев
```text
Без LFS:
- Размер репозитория: 1.5 GB
- Клон: 5 минут
- История: 800 MB

С LFS:
- Размер репозитория: 150 MB (только текстовые файлы)
- Клон: 30 секунд
- LFS-файлы: 1.35 GB (скачиваются по требованию)
```

---

## 5. Полный пример настройки Git-репозитория для Unity
```bash
# 1. Создать репозиторий
git init

# 2. Добавить .gitignore
echo "# Unity .gitignore" > .gitignore
curl -o .gitignore https://raw.githubusercontent.com/github/gitignore/main/Unity.gitignore

# 3. Добавить .gitattributes для Smart Merge
echo "*.unity merge=unityyamlmerge" >> .gitattributes
echo "*.prefab merge=unityyamlmerge" >> .gitattributes
echo "*.asset merge=unityyamlmerge" >> .gitattributes

# 4. Настроить Git LFS
git lfs install
git lfs track "*.psd" "*.png" "*.fbx" "*.wav" "*.mp4"

# 5. Настроить UnityYAMLMerge (глобально)
git config --global merge.unityyamlmerge.name "Unity Smart Merge"
git config --global merge.unityyamlmerge.driver 'C:/Program Files/Unity/Editor/Data/Tools/UnityYAMLMerge.exe merge --force -p --fallback base "%P" "%O" "%B"'

# 6. Первый коммит
git add .
git commit -m "Initial Unity project setup with Git LFS and Smart Merge"

# 7. Добавить удалённый репозиторий
git remote add origin https://github.com/username/unity-game.git
git push -u origin main
```

### 📋 Чек-лист перед коммитом:
- `.gitignore` настроен (нет `Library/`, `Temp/`)
- `.gitattributes` настроен (Smart Merge + LFS)
- Git LFS инициализирован
- Большие файлы отслеживаются через LFS
- UnityYAMLMerge настроен глобально
- Коммит сообщение понятное (`Fix player movement` вместо `Update`)

---

## 6. Лучшие практики и частые ошибки
### ✅ Рекомендации:
1. Коммитьте часто, пушите регулярно — маленькие коммиты проще мержить
2. Используйте Feature Branches — не работайте в main
3. Пишите осмысленные коммиты — коллеги должны понимать изменения
4. Используйте Smart Merge — экономит 90% времени на разрешение конфликтов
5. Храните большие файлы в LFS — иначе репозиторий разрастётся
6. Тестируйте перед пушем — сломанная сборка мешает всей команде

### ❌ Частые ошибки:
```bash
# ❌ ОШИБКА: Забыли .gitignore, Library попал в репозиторий
git add .  # Добавит Library
# ✅ ПРАВИЛЬНО: Добавить .gitignore ДО первого коммита

# ❌ ОШИБКА: Забыли настроить Smart Merge
# Конфликты сцен и префабов становятся адом
# ✅ ПРАВИЛЬНО: Настроить UnityYAMLMerge ДО начала работы

# ❌ ОШИБКА: Загрузка больших файлов без LFS
git add Assets/Textures/*.png  # 500 MB в репо
# ✅ ПРАВИЛЬНО: git lfs track "*.png"

# ❌ ОШИБКА: Коммит с изменением сцены без комментария
git commit -m "update"  # Что изменилось?
# ✅ ПРАВИЛЬНО: "Fix player spawning position in Level1"

# ❌ ОШИБКА: Работа в main напрямую
# ✅ ПРАВИЛЬНО: git checkout -b feature/new-ui
```

---

## 7. Работа с метафайлами (.meta)
Метафайлы — критически важны для Unity. Хранят GUID объектов, настройки импорта и т.д.

### ✅ Правила работы с .meta файлами:
1. Всегда храните .meta в Git (они управляют ссылками)
2. Никогда не редактируйте .meta вручную (только через Unity)
3. При конфликтах .meta — всегда принимайте свою версию (GUID должны совпадать)
```bash
# Стратегия для конфликтов .meta
git checkout --ours Assets/Scripts/Player.cs.meta
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
