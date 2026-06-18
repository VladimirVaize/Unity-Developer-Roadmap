# 🔄 Development Cycle (DevOps) in Unity: Cloud Build, Build Automation
DevOps (Development + Operations) is a culture and practices that unite development and operations to accelerate product delivery. 
In the Unity context, this means automating build, testing, and deployment processes. 
The key tool here is Unity Build Automation (formerly known as Cloud Build).

---

## 1. What is CI/CD and Why in Game Development?
CI/CD (Continuous Integration / Continuous Delivery) is a methodology that enables:
- Frequent code integration — developers regularly merge changes into a common branch.
- Automatic building and testing — every change triggers an automated build.
- Fast product delivery — builds are ready for testing or release.

### 🎮 Benefits for Game Development:
| Benefit | Description |
| --- | --- |
| Faster iteration | Cloud builds don't block development machines |
| Cross-platform | Simultaneous builds for Android, iOS, Windows, etc. |
| Consistency | All developers work with the latest version |
| Early bug detection | Automated builds catch issues early |

---

## 2. Unity Build Automation (Cloud Build)
Unity Build Automation is a cloud service that automatically builds Unity projects when changes are made to the version control system.
### 🧩 How It Works:
```text
┌─────────────────────────────────────────────────────────────┐
│                    Build Automation Process                 │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  1. SETUP                                                  │
│     ↓                                                       │
│     Connect to version control system (Git, Perforce,      │
│     SVN, Unity VCS) [citation:5]                          │
│                                                             │
│  2. TRIGGER                                                │
│     ↓                                                       │
│     Changes to code (push, commit, pull request)           │
│                                                             │
│  3. BUILD                                                  │
│     ↓                                                       │
│     Automatic cloud build for all platforms [citation:2][citation:4]│
│                                                             │
│  4. DEPLOY                                                 │
│     ↓                                                       │
│     Team notification (email, Slack, Discord) [citation:2][citation:4]│
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### 🛠️ Supported Version Control Systems:
| System | Support |
| --- | --- |
| Git | GitHub, GitLab, Bitbucket, private servers |
| Perforce | Full support |
| SVN | Full support |
| Unity VCS | Full support |

---

## 3. Setting Up Build Automation
### 📍 Accessing Build Automation:
1. Go to Unity Developer Dashboard
2. Select DevOps → Build Automation
3. Configure repository connection

### 🔐 Configuring Git Repository Access:
### Option 1: OAuth (for GitHub, GitLab, Bitbucket)
```text
# OAuth authorization — the simplest method
1. Select provider (GitHub/Bitbucket/GitLab)
2. Authorize access via OAuth
3. Choose repository
```

### Option 2: SSH Key (for private repositories)
```text
# Build Automation generates a public SSH key
# Add it to Deploy Keys in your repository

# Example for GitHub:
# Settings → Deploy keys → Add deploy key
# Paste public key from Build Automation dashboard
```

### Option 3: Personal Access Token (PAT)
```text
# For GitHub, GitLab, Bitbucket
1. Generate PAT in account settings
2. Paste into Build Automation dashboard
3. For GitHub: requires repo (private) or public_repo (public)
4. For auto-builds: needs write:repo_hook [citation:5]
```

### 📁 Target Platform Setup:
```yaml
# Target parameters
Target Label: "Android-Release"      # Configuration name
Unity Version: "2022.3.15f1"        # Unity version
Platform: "Android"                  # Target platform
Branch: "main"                       # Branch for builds
Project Subfolder: "UnityProject/"   # Folder with Assets & ProjectSettings
Auto-build: "On"                     # Auto-build on changes
```

---

## 4. Build Automation via API
Build Automation provides an API for integration with your tools.
### 📝 Example: Python Script for Build Management
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
        """Start a build for a target platform"""
        url = f"{self.base_url}/orgs/{self.org_id}/projects/{self.project_id}/buildtargets/{target}/builds"
        
        headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json"
        }
        
        payload = {
            "branch": branch,
            "commit": commit_sha,
            "clean": False,  # Use cache
            "buildParameters": {
                "BUILD_ENVIRONMENT": "production"
            }
        }
        
        response = requests.post(url, headers=headers, json=payload)
        return response.json()
    
    def poll_build_status(self, build_id):
        """Wait for build completion"""
        url = f"{self.base_url}/orgs/{self.org_id}/projects/{self.project_id}/buildtargets/builds/{build_id}"
        
        headers = {"Authorization": f"Bearer {self.api_key}"}
        
        while True:
            response = requests.get(url, headers=headers)
            data = response.json()
            
            status = data.get("buildStatus")
            print(f"Build status: {status}")
            
            if status in ["success", "failed", "canceled"]:
                return data
            
            time.sleep(10)  # Check every 10 seconds
    
    def download_artifact(self, build_id, artifact_name, output_path):
        """Download build artifact"""
        url = f"{self.base_url}/orgs/{self.org_id}/projects/{self.project_id}/buildtargets/builds/{build_id}/artifacts/{artifact_name}"
        
        headers = {"Authorization": f"Bearer {self.api_key}"}
        
        response = requests.get(url, headers=headers, stream=True)
        with open(output_path, 'wb') as f:
            for chunk in response.iter_content(chunk_size=8192):
                f.write(chunk)
        
        print(f"Artifact saved: {output_path}")

# Usage
builder = UnityCloudBuild(
    api_key=os.environ["UNITY_API_KEY"],
    org_id=os.environ["UNITY_ORG_ID"],
    project_id=os.environ["UNITY_PROJECT_ID"]
)

# Start build
build = builder.start_build("Android-Release", "main", "abc123")
build_id = build["buildId"]

# Wait for completion
result = builder.poll_build_status(build_id)

# Download APK
if result["buildStatus"] == "success":
    builder.download_artifact(build_id, "app.apk", "Builds/MyGame.apk")
```

> [!Important]
> In real projects, a ready-made Python handler is often used to interact with the API, called from the CI/CD pipeline.

---

## 5. Integration with CI/CD Pipelines
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

### 🐳 Docker Integration
```dockerfile
# Dockerfile for CI/CD
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

## 6. Cache and Configuration Management
### 🗂️ Caching in Build Automation
Build Automation uses cache to speed up subsequent builds. The cache is stored in Library and Temp folders.

```yaml
# Cache parameters
USE_CACHE: true          # Use cache
CACHE_LEVEL: "full"      # full / incremental / clean

# If cache is "poisoned" (build failing), you can flush it:
# 1. Run a build manually from the Unity Cloud Build UI
# 2. Create a new template configuration without cache
# 3. Update configuration [citation:1]
```

### 🎯 Configuration Templates
Each platform requires a separate build configuration:
```text
Configuration: @T_Android-Release (template)
  Cloned on each trigger:
    → Android-Release-main-abc123
    → Android-Release-main-def456

Configuration: @T_iOS-Release (template)
  → iOS-Release-main-abc123

# Templates allow reusing settings
# and isolating builds from each other [citation:1]
```

---

## 7. Notifications and Monitoring
Build Automation supports notifications:

| Channel | Support |
| --- | --- |
| Email | ✅ |
| Slack | ✅ |
| Discord | ✅ |
| Webhooks | ✅ (via API) |

### 📧 Notification Setup Example:
```yaml
# In Build Automation dashboard → Project Settings
Notifications:
  - Email: "team@studio.com"
    Events: ["build_success", "build_failed"]
  - Slack: "#builds-channel"
    Events: ["build_success", "build_failed"]
  - Webhook: "https://my-monitor.com/webhook"
    Events: ["build_started", "build_success", "build_failed"]
```

---

## 8. Best Practices
### ✅ Recommendations:
1. Use separate configurations for each platform
2. Enable auto-builds for main branches (main, develop)
3. Store API keys in CI/CD secrets
4. Regularly clean old builds to avoid filling storage
5. Use cache to speed up builds, but periodically do "clean" builds
6. Configure notifications for the entire team
7. For private repositories, use SSH keys or PAT

### ❌ Common Mistakes:
```yaml
# ❌ ERROR: Storing API key in code
UNITY_API_KEY: "sk_1234567890"  # Not safe!

# ✅ CORRECT: Using CI/CD secrets
UNITY_API_KEY: ${{ secrets.UNITY_API_KEY }}

# ❌ ERROR: Building from wrong branch
branch: "feature/broken-code"  # Build will fail

# ✅ CORRECT: Build only from stable branches
branch: "main"  # or "develop", "release/*"

# ❌ ERROR: Ignoring cache (too slow builds)
clean: true  # Full rebuild every time

# ✅ CORRECT: Use cache for speed
clean: false  # Incremental build
```

---

### ⭐ If this project was useful, put a star on GitHub!
