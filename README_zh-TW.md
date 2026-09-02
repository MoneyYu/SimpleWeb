# SimpleWeb

[![建置狀態](https://github.com/MoneyYu/SimpleWeb/actions/workflows/01.build.yml/badge.svg)](https://github.com/MoneyYu/SimpleWeb/actions/workflows/01.build.yml)

一個展示現代 DevOps 實踐、雲端部署策略和基礎架構即程式碼 (IaC) 模式的 ASP.NET Core 10.0 示範網頁應用程式。

> 📖 [English Version](README.md)

## 目錄

- [概述](#概述)
- [功能特色](#功能特色)
- [專案結構](#專案結構)
- [環境需求](#環境需求)
- [快速開始](#快速開始)
- [設定配置](#設定配置)
- [執行測試](#執行測試)
- [Docker 支援](#docker-支援)
- [CI/CD 管線](#cicd-管線)
- [Java 版本](#java-版本)
- [基礎架構即程式碼](#基礎架構即程式碼)
- [Kubernetes 部署](#kubernetes-部署)
- [貢獻指南](#貢獻指南)
- [授權條款](#授權條款)

## 概述

SimpleWeb 是一個示範專案，旨在展示以下最佳實踐：

- 建置 ASP.NET Core 網頁應用程式
- 使用 Azure DevOps 和 GitHub Actions 實作 CI/CD 管線
- 使用 Docker 和 Kubernetes (AKS) 進行容器部署
- 使用 Terraform 和 Bicep 進行基礎架構佈建
- 安全性掃描和程式碼品質分析

## 功能特色

- **ASP.NET Core 10.0 MVC** - 採用 MVC 架構的現代網頁框架
- **健康檢查端點** - 內建於 `/health` 的健康監控功能
- **檔案上傳** - 支援本機和 Azure Blob 儲存體
- **Application Insights** - 遙測和監控整合
- **使用者驗證** - Azure AD 驗證標頭支援
- **彈性儲存** - 可配置的儲存提供者（本機/Azure）

## 專案結構

```
SimpleWeb/
├── src/
│   ├── SimpleWeb/                    # 主要網頁應用程式
│   │   ├── Controllers/              # MVC 控制器
│   │   ├── Models/                   # 資料模型
│   │   ├── Views/                    # Razor 視圖
│   │   ├── wwwroot/                  # 靜態檔案
│   │   ├── Dockerfile                # 容器定義
│   │   └── appsettings.json          # 應用程式設定
│   ├── SimpleWeb.UnitTest/           # 單元測試
│   ├── SimpleWeb.IntegrationTest/    # 整合測試
│   └── SimpleWeb.UITest/             # UI 自動化測試
├── src-java/                         # Java 版本（Spring Boot 4.1.1 / Java 21）
│   ├── pom.xml                       # Maven 專案（產出 target/simpleweb.jar）
│   ├── mvnw / mvnw.cmd / .mvn/       # Maven Wrapper
│   ├── Dockerfile                    # 容器定義
│   ├── src/                          # 應用程式與測試原始碼
│   └── README.md                     # Java 版本說明文件
├── ci/                               # Azure DevOps 管線定義
│   ├── 01.build.yml                  # 基本建置管線
│   ├── 01.prwithlimitbranch.yml      # PR 驗證（限制分支）
│   ├── 02.packagescan.yml            # 安全性掃描 (Snyk)
│   ├── 03.sonarcloud.yml             # 程式碼品質分析
│   ├── 04.publish.artifacts.yml      # 成品發布
│   ├── 05.multistagerelease.yml      # 多階段發布
│   ├── 06.dockerseperate.yml         # Docker 分離建置
│   ├── 07.dockerbuildandpush.yml     # Docker 建置並推送
│   ├── 08.aks.yml                    # AKS 部署
│   ├── 09.terraform.build.yml        # Terraform 建置
│   ├── 09.terraform.release.yml      # Terraform 發布
│   └── 10.bicep.yml                  # Bicep 部署
├── bicep/                            # Azure Bicep 範本
│   ├── main.bicep                    # 主要基礎架構範本
│   └── parameters.json               # 參數檔案
├── tf/                               # Terraform 設定
│   └── infra.tf                      # Azure 基礎架構
├── manifests/                        # Kubernetes 資源清單
│   ├── deployment.yml                # K8s 部署
│   └── service.yml                   # K8s 服務 (LoadBalancer)
├── scripts/                          # 工具腳本
│   └── TestifyZeroDowntime.ps1       # 零停機時間測試
└── .github/workflows/                # GitHub Actions
    ├── 01.build.yml                  # 建置和測試工作流程
    ├── 01.prwithlimitbranch.yml      # PR 驗證（限制分支）
    ├── 02.packagescan.yml            # 安全性掃描 (Snyk)
    ├── 03.sonarcloud.yml             # 程式碼品質分析 (SonarCloud)
    ├── 04.publish.artifacts.yml      # 成品發布
    ├── 05.multistagerelease.yml      # 多階段發布至 Azure Web App
    ├── 06.dockerseperate.yml         # Docker 建置並推送至 GHCR
    ├── 07.dockerbuildandpush.yml     # Docker 建置並推送至 ACR
    ├── 08.aks.yml                    # 部署至 Azure Kubernetes Service
    ├── 09.terraform.build.yml        # Terraform 建置工作流程
    ├── 09.terraform.release.yml      # Terraform 發布工作流程
    ├── 10.bicep.yml                  # Bicep 部署工作流程
    ├── 12.java-build.yml             # Java 版本建置與測試
    └── 13.java-vm-deploy.yml         # Java 版本手動 VM 部署
```

## 環境需求

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) 或更新版本
- [Docker](https://www.docker.com/get-started)（用於容器化）
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)（用於 Azure 部署）
- [Terraform](https://www.terraform.io/downloads)（用於 Terraform IaC）
- [kubectl](https://kubernetes.io/docs/tasks/tools/)（用於 Kubernetes 部署）

## 快速開始

### 複製儲存庫

```bash
git clone https://github.com/MoneyYu/SimpleWeb.git
cd SimpleWeb
```

### 還原相依套件

```bash
dotnet restore src/SimpleWeb.sln
```

### 建置應用程式

```bash
dotnet build src/SimpleWeb.sln --configuration Release
```

### 執行應用程式

```bash
cd src/SimpleWeb
dotnet run
```

應用程式將在 `https://localhost:5001` 或 `http://localhost:5000` 上執行。

## 設定配置

### 應用程式設定

應用程式在 `appsettings.json` 中支援以下設定選項：

```json
{
  "Storage": {
    "Type": "Local",           // 選項："Local" 或 "Azure"
    "FileName": "sample.jpg",
    "Azure": {
      "ConnectionString": ""   // Azure 儲存體連接字串
    }
  },
  "APPINSIGHTS_CONNECTIONSTRING": ""  // Application Insights 連接字串
}
```

### 使用者密鑰（開發環境）

在開發環境中安全儲存敏感設定：

```bash
# 初始化使用者密鑰
dotnet user-secrets init

# 設定 Azure 儲存體連接字串
dotnet user-secrets set "Storage:Azure:ConnectionString" "your-connection-string"

# 設定 Application Insights 連接字串
dotnet user-secrets set "APPINSIGHTS_CONNECTIONSTRING" "your-connection-string"
```

更多資訊請參閱 [ASP.NET Core 開發環境中的應用程式密鑰安全儲存](https://docs.microsoft.com/zh-tw/aspnet/core/security/app-secrets)。

### 環境變數

| 變數 | 說明 |
|----------|-------------|
| `Storage__Type` | 儲存提供者類型（`Local` 或 `Azure`） |
| `Storage__Azure__ConnectionString` | Azure Blob 儲存體連接字串 |
| `APPINSIGHTS_CONNECTIONSTRING` | Application Insights 連接字串 |

## 執行測試

### 單元測試

```bash
dotnet test src/SimpleWeb.UnitTest/SimpleWeb.UnitTest.csproj --verbosity normal
```

### 整合測試

```bash
dotnet test src/SimpleWeb.IntegrationTest/SimpleWeb.IntegrationTest.csproj --verbosity normal
```

### 所有測試

```bash
dotnet test src/SimpleWeb.sln --verbosity normal
```

## Docker 支援

### 建置 Docker 映像

```bash
cd src/SimpleWeb
docker build -t simpleweb:latest .
```

### 執行容器

```bash
docker run -d -p 8080:80 --name simpleweb simpleweb:latest
```

在 `http://localhost:8080` 存取應用程式。

### Docker Compose（選用）

```bash
# 建置並執行
docker-compose up -d

# 停止並移除
docker-compose down
```

## CI/CD 管線

此專案包含多個 Azure DevOps 的 CI/CD 管線定義：

| 管線 | 說明 |
|----------|-------------|
| `01.build.yml` | 基本建置和測試管線 |
| `01.prwithlimitbranch.yml` | PR 驗證（需要來自 dev 分支） |
| `02.packagescan.yml` | 使用 Snyk 進行安全性掃描 |
| `03.sonarcloud.yml` | 使用 SonarCloud 進行程式碼品質分析 |
| `04.publish.artifacts.yml` | 建置成品發布 |
| `05.multistagerelease.yml` | 多階段部署至 Azure Web App |
| `06.dockerseperate.yml` | 分離的 Docker 建置階段 |
| `07.dockerbuildandpush.yml` | Docker 映像建置並推送至 ACR |
| `08.aks.yml` | 部署至 Azure Kubernetes Service |
| `09.terraform.*.yml` | 使用 Terraform 進行基礎架構部署 |
| `10.bicep.yml` | 使用 Bicep 進行基礎架構部署 |

### GitHub Actions

專案包含完整的 GitHub Actions 工作流程（`.github/workflows/`），與 Azure DevOps 管線相對應：

| 工作流程 | 說明 |
|----------|-------------|
| `01.build.yml` | 推送至任何分支時建置和測試 |
| `01.prwithlimitbranch.yml` | PR 驗證（需要 PR 從 dev 分支發起至 master） |
| `02.packagescan.yml` | 使用 Snyk 進行安全性掃描 |
| `03.sonarcloud.yml` | 使用 SonarCloud 進行程式碼品質分析 |
| `04.publish.artifacts.yml` | 建置成品發布 |
| `05.multistagerelease.yml` | 多階段部署至 Azure Web App |
| `06.dockerseperate.yml` | Docker 建置並推送至 GitHub Container Registry |
| `07.dockerbuildandpush.yml` | Docker 映像建置並推送至 Azure Container Registry |
| `08.aks.yml` | 建置、推送至 ACR 並部署至 Azure Kubernetes Service |
| `09.terraform.build.yml` | 建置並準備 Terraform 成品 |
| `09.terraform.release.yml` | 使用 Terraform 部署基礎架構並部署應用程式 |
| `10.bicep.yml` | 使用 Bicep 部署 Azure 基礎架構 |

## Java 版本

除了 `src/` 的 ASP.NET Core 應用程式之外，本儲存庫還包含 SimpleWeb 的 **Java 版本**，
位於 [`src-java/`](src-java/)：以 Spring Boot 4.1.1 / Java 21 實作，提供同樣的環境與建置資訊。
兩個版本各自獨立建置與部署，用來示範同一套 DevOps 實踐如何套用在不同技術堆疊上。

```bash
cd src-java

# 建置、測試並打包（產出 target/simpleweb.jar）
./mvnw -B verify

# 在 http://localhost:8080 本機執行
./mvnw spring-boot:run
```

端點：`/`（首頁）、`/api/info`（JSON）、`/actuator/health`、`/actuator/info`。

完整說明（本機執行、測試、Docker、環境變數，以及 `/opt/simpleweb/{test,prod}` 的通用 VM
部署配置）請見 [`src-java/README.md`](src-java/README.md)。

### Java 工作流程

| 工作流程 | 說明 |
|----------|-------------|
| `12.java-build.yml` | `src-java/**` 有變更時建置並測試 Java 版本 |
| `13.java-vm-deploy.yml` | 手動（`workflow_dispatch`）部署 Java 版本至 Linux VM，可選擇 `test` 或 `production` GitHub Environment |

## 基礎架構即程式碼

### Terraform

使用 Terraform 部署 Azure 基礎架構：

```bash
cd tf

# 初始化 Terraform
terraform init

# 預覽變更
terraform plan

# 套用變更
terraform apply
```

這會建立：
- Azure 資源群組
- App Service 方案（Linux、Standard S1）
- Azure App Service（.NET 10.0）

### Bicep

使用 Bicep 部署 Azure 基礎架構：

```bash
# 使用 Azure CLI
az deployment group create \
  --resource-group <資源群組名稱> \
  --template-file bicep/main.bicep \
  --parameters bicep/parameters.json

# 使用 PowerShell
New-AzResourceGroupDeployment \
  -ResourceGroupName <資源群組名稱> \
  -TemplateFile bicep/main.bicep
```

這會建立：
- 虛擬網路和子網路
- 儲存體帳戶
- 公用 IP 位址
- 網路介面
- Windows Server 虛擬機器

## Kubernetes 部署

部署至 Kubernetes（AKS 或任何 K8s 叢集）：

```bash
# 套用部署
kubectl apply -f manifests/deployment.yml

# 套用服務 (LoadBalancer)
kubectl apply -f manifests/service.yml

# 檢查部署狀態
kubectl get deployments
kubectl get pods
kubectl get services
```

### Kubernetes 資源

- **Deployment**：建立執行 SimpleWeb 容器的 Pod
- **Service**：透過 LoadBalancer 在連接埠 80 公開應用程式

## API 端點

| 端點 | 方法 | 說明 |
|----------|--------|-------------|
| `/` | GET | 首頁 |
| `/Home/Privacy` | GET | 隱私權政策頁面 |
| `/Home/Upload` | GET | 檔案上傳頁面 |
| `/Home/Upload` | POST | 上傳檔案 |
| `/health` | GET | 健康檢查端點（JSON） |

## 健康檢查

應用程式在 `/health` 提供健康檢查端點，回傳 JSON 格式：

```json
{
  "status": "Healthy",
  "results": {}
}
```

## 貢獻指南

1. Fork 此儲存庫
2. 建立功能分支（`git checkout -b feature/amazing-feature`）
3. 提交您的變更（`git commit -m 'Add amazing feature'`）
4. 推送至分支（`git push origin feature/amazing-feature`）
5. 開啟 Pull Request

## 授權條款

此專案採用開放原始碼授權，授權條款為 [MIT License](LICENSE)。
