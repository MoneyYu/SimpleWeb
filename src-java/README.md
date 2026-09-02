# SimpleWeb Java Edition

SimpleWeb 的 **Java 版本**：以 Spring Boot 4.1.1 / Java 21 實作，與既有的 C# (ASP.NET Core) 版本
並存於同一個儲存庫。兩個版本各自獨立建置、各自獨立部署，用來示範同一套 DevOps 流程
（CI、成品發佈、環境保護、VM 部署）如何套用在不同技術堆疊上。

> 📁 C# 版本位於 [`src/`](../src/)；本文件只描述 `src-java/` 這個 Maven 專案。

## 目錄

- [專案結構](#專案結構)
- [環境需求](#環境需求)
- [本機執行](#本機執行)
- [執行測試](#執行測試)
- [端點](#端點)
- [環境變數](#環境變數)
- [Docker](#docker)
- [部署到 VM](#部署到-vm)
- [GitHub Actions 工作流程](#github-actions-工作流程)

## 專案結構

```
src-java/
├── pom.xml                                   # Maven 專案定義（finalName 固定為 simpleweb）
├── mvnw / mvnw.cmd / .mvn/                   # Maven Wrapper，不需要事先安裝 Maven
├── Dockerfile                                # 多階段建置：maven 建置 + temurin JRE 執行
└── src/
    ├── main/
    │   ├── java/money/gh200/simpleweb/
    │   │   ├── SimpleWebApplication.java     # 進入點
    │   │   ├── model/AppInfo.java            # 首頁與 /api/info 共用的資料快照
    │   │   ├── service/InfoService.java      # 組出環境、版本、建置資訊
    │   │   └── web/                          # HomeController（頁面）、InfoApiController（API）
    │   └── resources/
    │       ├── application.yml               # 設定（全部可用環境變數覆寫）
    │       ├── templates/index.html          # Thymeleaf 首頁
    │       └── static/css/site.css           # 樣式
    └── test/java/money/gh200/simpleweb/
        ├── SimpleWebApplicationIT.java       # 整合測試（Failsafe，*IT 結尾）
        ├── service/InfoServiceTest.java      # 單元測試（Surefire）
        └── web/HomeControllerTest.java       # MVC 切片測試
```

建置產出物固定為 **`target/simpleweb.jar`**（由 `pom.xml` 的 `<finalName>` 決定）。
部署腳本與工作流程都直接引用這個路徑，改名會讓部署失敗。

## 環境需求

- [JDK 21](https://adoptium.net/temurin/releases/?version=21)（Temurin 或其他發行版皆可）
- [Docker](https://www.docker.com/get-started)（選用，僅容器化時需要）
- 不需要另外安裝 Maven，專案已內含 Maven Wrapper

## 本機執行

```bash
cd src-java

# Linux / macOS
./mvnw spring-boot:run

# Windows
.\mvnw.cmd spring-boot:run
```

預設在 <http://localhost:8080> 啟動。也可以先打包再直接執行 jar：

```bash
./mvnw -B clean package
java -jar target/simpleweb.jar
```

## 執行測試

```bash
# 單元測試（Surefire）
./mvnw -B test

# 單元測試 + 整合測試 + 打包（Failsafe，CI 使用的指令）
./mvnw -B verify
```

`verify` 會執行 `*Test` 與 `*IT` 兩類測試，並產生 `target/simpleweb.jar`。

## 端點

| 端點 | 方法 | 說明 |
|------|------|------|
| `/` | GET | 首頁，依環境顯示不同顏色的橫幅 |
| `/api/info` | GET | 應用程式與建置資訊（JSON） |
| `/actuator/health` | GET | 健康檢查 |
| `/actuator/info` | GET | Actuator 資訊端點 |

`/api/info` 回應範例：

```json
{
  "application": "SimpleWeb",
  "version": "1.0.0",
  "environment": "test",
  "buildSha": "a1b2c3d",
  "buildTime": "2026-09-03T00:00:00Z",
  "hostname": "vm-simpleweb",
  "javaVersion": "21.0.11",
  "serverTime": "2026-09-03 08:00:00 CST"
}
```

## 環境變數

| 變數 | 預設值 | 說明 |
|------|--------|------|
| `SERVER_PORT` | `8080` | HTTP 監聽埠 |
| `APP_ENVIRONMENT` | `local` | 只接受 `local`、`test`、`production`；其他值一律退回 `local`，避免誤顯示正式環境橫幅 |
| `APP_BUILD_SHA` | `dev` | 建置來源 commit，由部署流程寫入 |
| `APP_BUILD_TIME` | `unknown` | 建置/部署時間（UTC），由部署流程寫入 |

## Docker

```bash
cd src-java

docker build -t simpleweb-java:latest .

docker run -d -p 8080:8080 \
  -e APP_ENVIRONMENT=local \
  --name simpleweb-java simpleweb-java:latest
```

`Dockerfile` 為多階段建置：第一階段以 `maven:3.9-eclipse-temurin-21` 打包，
第二階段只帶 `eclipse-temurin:21-jre` 與 jar，減少映像體積。

## 部署到 VM

部署採用「一台 Linux VM、兩個 systemd 服務」的通用配置，測試與正式環境彼此隔離：

| 環境 | 安裝目錄 | systemd unit | 預設埠 |
|------|----------|--------------|--------|
| 測試 | `/opt/simpleweb/test` | `simpleweb-test` | 8080 |
| 正式 | `/opt/simpleweb/prod` | `simpleweb-prod` | 8081 |

VM 端需事先準備（由管理者一次性完成，不在工作流程內建立）：

1. 建立不可登入的服務帳號 `simpleweb`，以及上述兩個目錄（owner 為 `simpleweb`）。
2. 安裝 JRE 21。
3. 為每個環境建立 systemd unit，指向該環境的 jar，並讀取同目錄的 `app.env`：

   ```ini
   [Unit]
   Description=SimpleWeb (test)
   After=network.target

   [Service]
   User=simpleweb
   EnvironmentFile=/opt/simpleweb/test/app.env
   Environment=SERVER_PORT=8080
   ExecStart=/usr/bin/java -jar /opt/simpleweb/test/simpleweb.jar
   Restart=on-failure

   [Install]
   WantedBy=multi-user.target
   ```

部署流程（`13.java-vm-deploy.yml`）會把 jar 安裝到對應目錄、寫入 `app.env`
（`APP_ENVIRONMENT` / `APP_BUILD_SHA` / `APP_BUILD_TIME`）、重啟服務，最後輪詢 `/api/info`
並比對 `buildSha` 是否等於這次部署的 commit —— 只看到 HTTP 200 不算部署成功，
因為舊版程式還活著時也會回 200。

## GitHub Actions 工作流程

| 工作流程 | 觸發方式 | 說明 |
|----------|----------|------|
| [`12.java-build.yml`](../.github/workflows/12.java-build.yml) | 推送且 `src-java/**` 或此工作流程檔有變更、手動 | `./mvnw -B verify`，上傳 `simpleweb.jar` 成品 |
| [`13.java-vm-deploy.yml`](../.github/workflows/13.java-vm-deploy.yml) | **僅手動**（選擇 `test` 或 `production`） | 建置後發佈滾動式 Release asset，透過 OIDC 登入 Azure，以 `az vm run-command` 部署，並以 `buildSha` 驗證；同時間只允許一個部署（concurrency） |

`13.java-vm-deploy.yml` 需要下列設定（可設在 Repository 或 Environment 層級）：

| 類型 | 名稱 | 說明 |
|------|------|------|
| Variable | `AZURE_RESOURCE_GROUP` | VM 所在的資源群組 |
| Variable | `AZURE_VM_NAME` | 目標 VM 名稱 |
| Variable | `VM_PUBLIC_IP` | 對外 IP，供 smoke test 與環境連結使用 |
| Secret | `AZURE_CLIENT_ID` | 用於 OIDC 的 App Registration client ID |
| Secret | `AZURE_TENANT_ID` | Entra ID 租用戶 ID |
| Secret | `AZURE_SUBSCRIPTION_ID` | 訂閱 ID |

> 🔒 正式環境的保護（required reviewers、wait timer、可部署分支）請在
> GitHub Environment `production` 設定，工作流程本身不寫死任何審核邏輯。
