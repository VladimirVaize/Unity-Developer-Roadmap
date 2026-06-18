# 🔄 Цикл разработки (DevOps) в Unity: Cloud Build, автоматизация сборок

DevOps (Development + Operations) — это культура и практики, объединяющие разработку и эксплуатацию для ускорения выпуска продуктов. 
В контексте Unity это означает автоматизацию процессов сборки, тестирования и развертывания игр. 
Ключевой инструмент здесь — Unity Build Automation (ранее известный как Cloud Build).

---

## 1. Что такое CI/CD и зачем это в геймдеве?
CI/CD (Continuous Integration / Continuous Delivery) — это методология, которая позволяет:
- Часто интегрировать код — разработчики регулярно сливают изменения в общую ветку.
- Автоматически собирать и тестировать — при каждом изменении запускается автоматическая сборка.
- Быстро доставлять продукт — готовые сборки доступны для тестирования или релиза.

### 🎮 Преимущества для разработки игр:
| Преимущество | Описание |
| --- | --- |
| Ускорение итераций | Сборка в облаке не блокирует рабочие машины |
| Межплатформенность | Одновременная сборка для Android, iOS, Windows и других платформ |
| Консистентность | Все разработчики работают с актуальной версией игры |
| Раннее обнаружение ошибок | Автоматические сборки выявляют проблемы на ранних этапах |

---

## 2. Unity Build Automation (Cloud Build)
Unity Build Automation — это облачный сервис, который автоматически собирает проекты Unity при изменениях в системе контроля версий.

### 🧩 Как это работает:
```text
┌─────────────────────────────────────────────────────────────┐
│                    Процесс Build Automation                 │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  1. НАСТРОЙКА (Setup)                                      │
│     ↓                                                       │
│     Подключение к системе контроля версий (Git, Perforce,  │
│     SVN, Unity VCS) [citation:5]                          │
│                                                             │
│  2. ТРИГГЕР (Trigger)                                      │
│     ↓                                                       │
│     Изменения в коде (push, commit, pull request)          │
│                                                             │
│  3. СБОРКА (Build)                                         │
│     ↓                                                       │
│     Автоматическая сборка в облаке для всех платформ [citation:2][citation:4]│
│                                                             │
│  4. РАЗВЕРТЫВАНИЕ (Deploy)                                 │
│     ↓                                                       │
│     Уведомление команды (email, Slack, Discord) [citation:2][citation:4]│
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### 🛠️ Поддерживаемые системы контроля версий:

| Система | Поддержка |
| --- | --- |
| Git | GitHub, GitLab, Bitbucket, приватные серверы |
| Perforce | Полная поддержка |
| SVN | Полная поддержка |
| Unity VCS | Полная поддержка |

---

## 3. Настройка Build Automation
### 📍 Доступ к Build Automation:
1. Перейдите в Unity Developer Dashboard (панель разработчика)
2. Выберите DevOps → Build Automation
3. Настройте подключение к репозиторию

### 🔐 Настройка доступа к Git-репозиторию:
### Вариант 1: OAuth (для GitHub, GitLab, Bitbucket)
```text
# Авторизация через OAuth — самый простой способ
1. Выберите провайдера (GitHub/Bitbucket/GitLab)
2. Авторизуйте доступ через OAuth
3. Выберите репозиторий
```

### Вариант 2: SSH-ключ (для приватных репозиториев)
```text
# Build Automation генерирует публичный SSH-ключ
# Его нужно добавить в Deploy Keys репозитория

# Пример добавления на GitHub:
# Settings → Deploy keys → Add deploy key
# Вставить публичный ключ из панели Build Automation
```

### Вариант 3: Personal Access Token (PAT)
```text
# Для GitHub, GitLab, Bitbucket
1. Сгенерировать PAT в настройках аккаунта
2. Вставить в панель Build Automation
3. Для GitHub требуется repo (для приватных) или public_repo (для публичных)
4. Для автоматических сборок нужен write:repo_hook [citation:5]
```

📁 Настройка целевой платформы (Target):
```yaml
# Параметры Target
Target Label: "Android-Release"      # Имя конфигурации
Unity Version: "2022.3.15f1"        # Версия Unity
Platform: "Android"                  # Целевая платформа
Branch: "main"                       # Ветка для сборки
Project Subfolder: "UnityProject/"   # Папка с Assets и ProjectSettings
Auto-build: "On"                     # Автоматическая сборка при изменениях
```

---

## 4. Автоматизация сборок через API
Build Automation предоставляет API для интеграции с вашими инструментами.

### 📝 Пример: Python-скрипт для управления сборками
```python
# scripts/cloudbuild/build.py
import os
import time
import requests

class UnityCloudBuild:
    def __init__(self, api_key, org_id, project_id):
        self.api_key = api_key
        self.org_id = org_id
        self.project_id = project_id
        self.base_url = "https://build-api.cloud.unity3d.com/api/v1"
    
    def start_build(self, target, branch, commit_sha):
        """Запуск сборки для целевой платформы"""
        url = f"{self.base_url}/orgs/{self.org_id}/projects/{self.project_id}/buildtargets/{target}/builds"
        
        headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json"
        }
        
        payload = {
            "branch": branch,
            "commit": commit_sha,
            "clean": False,  # Использовать кэш
            "buildParameters": {
                "BUILD_ENVIRONMENT": "production"
            }
        }
        
        response = requests.post(url, headers=headers, json=payload)
        return response.json()
    
    def poll_build_status(self, build_id):
        """Ожидание завершения сборки"""
        url = f"{self.base_url}/orgs/{self.org_id}/projects/{self.project_id}/buildtargets/builds/{build_id}"
        
        headers = {"Authorization": f"Bearer {self.api_key}"}
        
        while True:
            response = requests.get(url, headers=headers)
            data = response.json()
            
            status = data.get("buildStatus")
            print(f"Статус сборки: {status}")
            
            if status in ["success", "failed", "canceled"]:
                return data
            
            time.sleep(10)  # Проверка каждые 10 секунд
    
    def download_artifact(self, build_id, artifact_name, output_path):
        """Скачивание артефакта сборки"""
        url = f"{self.base_url}/orgs/{self.org_id}/projects/{self.project_id}/buildtargets/builds/{build_id}/artifacts/{artifact_name}"
        
        headers = {"Authorization": f"Bearer {self.api_key}"}
        
        response = requests.get(url, headers=headers, stream=True)
        with open(output_path, 'wb') as f:
            for chunk in response.iter_content(chunk_size=8192):
                f.write(chunk)
        
        print(f"Артефакт сохранён: {output_path}")

# Использование
builder = UnityCloudBuild(
    api_key=os.environ["UNITY_API_KEY"],
    org_id=os.environ["UNITY_ORG_ID"],
    project_id=os.environ["UNITY_PROJECT_ID"]
)

# Запуск сборки
build = builder.start_build("Android-Release", "main", "abc123")
build_id = build["buildId"]

# Ожидание завершения
result = builder.poll_build_status(build_id)

# Скачивание APK
if result["buildStatus"] == "success":
    builder.download_artifact(build_id, "app.apk", "Builds/MyGame.apk")
```

> [!Important]
> В реальных проектах для взаимодействия с API часто используется готовый Python-обработчик, который вызывается из CI/CD-пайплайна.

---

## 5. Интеграция с CI/CD-пайплайнами
### 🔧 GitHub Actions
```yaml
# .github/workflows/build-unity.yml
name: Unity Cloud Build

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]
  workflow_dispatch:

env:
  UNITY_API_KEY: ${{ secrets.UNITY_API_KEY }}
  UNITY_ORG_ID: ${{ secrets.UNITY_ORG_ID }}
  UNITY_PROJECT_ID: ${{ secrets.UNITY_PROJECT_ID }}

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v3

      - name: Setup Python
        uses: actions/setup-python@v4
        with:
          python-version: '3.10'

      - name: Install dependencies
        run: |
          pip install requests
          pip install -r scripts/cloudbuild/requirements.txt

      - name: Trigger Unity Cloud Build
        run: |
          python scripts/cloudbuild/build.py \
            --target "Android-Release" \
            --branch ${{ github.ref_name }} \
            --commit ${{ github.sha }}

      - name: Download artifacts
        if: success()
        uses: actions/download-artifact@v3
        with:
          name: unity-build
          path: Builds/
```

### 🐳 Интеграция через Docker
```dockerfile
# Dockerfile для CI/CD
FROM python:3.10-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY scripts/cloudbuild/ /app/scripts/cloudbuild/

ENTRYPOINT ["python", "/app/scripts/cloudbuild/build.py"]
```

### 📦 Jenkins Pipeline
```groovy
// Jenkinsfile
pipeline {
    agent any
    
    environment {
        UNITY_API_KEY = credentials('unity-api-key')
        UNITY_ORG_ID = credentials('unity-org-id')
        UNITY_PROJECT_ID = credentials('unity-project-id')
    }
    
    stages {
        stage('Trigger Cloud Build') {
            steps {
                sh '''
                    python scripts/cloudbuild/build.py \
                        --target "Android-Release" \
                        --branch ${GIT_BRANCH} \
                        --commit ${GIT_COMMIT}
                '''
            }
        }
        
        stage('Download and Archive') {
            steps {
                sh '''
                    mkdir -p Builds/
                    python scripts/cloudbuild/download.py \
                        --build-id ${BUILD_ID} \
                        --output Builds/
                '''
                archiveArtifacts artifacts: 'Builds/*.apk'
            }
        }
    }
}
```

---

## 6. Управление кэшем и конфигурациями
### 🗂️ Кэширование в Build Automation
Build Automation использует кэш для ускорения последующих сборок. Кэш хранится в Library и Temp папках.

```yaml
# Параметры кэша
USE_CACHE: true          # Использовать кэш
CACHE_LEVEL: "full"      # full / incremental / clean

# Если кэш "отравлен" (build failing), его можно сбросить:
# 1. Запустить сборку вручную на панели Unity Cloud Build UI
# 2. Создать новый шаблон конфигурации без кэша
# 3. Обновить конфигурацию [citation:1]
```

### 🎯 Шаблоны конфигураций
Каждая платформа требует отдельной конфигурации сборки:
```text
Конфигурация: @T_Android-Release (шаблон)
  Клонируется при каждом триггере:
    → Android-Release-main-abc123
    → Android-Release-main-def456

Конфигурация: @T_iOS-Release (шаблон)
  → iOS-Release-main-abc123

# Шаблоны позволяют переиспользовать настройки
# и изолировать сборки друг от друга [citation:1]
```

---

## 7. Уведомления и мониторинг
Build Automation поддерживает отправку уведомлений:
| Канал | Поддержка |
| --- | --- |
| Email | ✅ |
| Slack | ✅ |
| Discord | ✅ |
| Webhooks | ✅ (через API) |

### 📧 Пример настройки уведомлений:
```yaml
# В панели Build Automation → Project Settings
Notifications:
  - Email: "team@studio.com"
    Events: ["build_success", "build_failed"]
  - Slack: "#builds-channel"
    Events: ["build_success", "build_failed"]
  - Webhook: "https://my-monitor.com/webhook"
    Events: ["build_started", "build_success", "build_failed"]
```

---

## 8. Лучшие практики
### ✅ Рекомендации:
1. Используйте отдельные конфигурации для каждой платформы
2. Включайте автоматические сборки для основных веток (main, develop)
3. Храните API-ключи в секретах CI/CD
4. Регулярно очищайте старые сборки чтобы не переполнять хранилище
5. Используйте кэш для ускорения сборок, но периодически выполняйте "чистые" сборки
6. Настраивайте уведомления для всей команды
7. Для приватных репозиториев используйте SSH-ключи или PAT

### ❌ Частые ошибки:
```yaml
# ❌ ОШИБКА: Хранение API-ключа в коде
UNITY_API_KEY: "sk_1234567890"  # Небезопасно!

# ✅ ПРАВИЛЬНО: Использование секретов CI/CD
UNITY_API_KEY: ${{ secrets.UNITY_API_KEY }}

# ❌ ОШИБКА: Сборка из неправильной ветки
branch: "feature/broken-code"  # Сборка сломается

# ✅ ПРАВИЛЬНО: Сборка только из стабильных веток
branch: "main"  # или "develop", "release/*"

# ❌ ОШИБКА: Игнорирование кэша (слишком долгие сборки)
clean: true  # Каждый раз полная пересборка

# ✅ ПРАВИЛЬНО: Использовать кэш для ускорения
clean: false  # Инкрементальная сборка
```

---

### ⭐ Если этот проект был полезен, поставьте звезду на GitHub!
