# Azure Image Builder for SimpleWeb

這個專案使用 Azure Image Builder (AIB) 將 SimpleWeb 應用程式打包成可重複使用的 VM Image。

## 架構概述

當程式碼 push 到 `main` 分支時，GitHub Actions 會：
1. 建置 SimpleWeb .NET 10 應用程式
2. 使用 Azure Image Builder 建立包含以下元件的 VM Image：
   - Windows Server 2022 Datacenter
   - IIS (Internet Information Services)
   - .NET 10 Hosting Bundle
   - SimpleWeb 應用程式（已配置並準備執行）
3. 將完成的 Image 發佈到 Azure Shared Image Gallery (SIG)

## 前置需求

### 1. Azure 資源

在執行 workflow 之前，需要建立以下 Azure 資源：

```bash
# 設定變數
RESOURCE_GROUP="rg-simpleweb-imagebuilder"
LOCATION="eastus"

# 建立 Resource Group
az group create --name $RESOURCE_GROUP --location $LOCATION
```

### 2. GitHub Secrets

在 GitHub Repository 設定中，需要配置以下 Secret：

#### AZURE_CREDENTIALS

建立 Service Principal 並賦予必要權限：

```bash
# 建立 Service Principal
az ad sp create-for-rbac \
  --name "sp-simpleweb-github" \
  --role "Contributor" \
  --scopes "/subscriptions/<YOUR_SUBSCRIPTION_ID>/resourceGroups/$RESOURCE_GROUP" \
  --sdk-auth
```

將輸出的 JSON 設定為 GitHub Secret `AZURE_CREDENTIALS`：

```json
{
  "clientId": "<CLIENT_ID>",
  "clientSecret": "<CLIENT_SECRET>",
  "subscriptionId": "<SUBSCRIPTION_ID>",
  "tenantId": "<TENANT_ID>",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

## 檔案說明

### image-builder/aib-template.json

Azure Image Builder 的模板檔案，定義了：
- **來源映像**: Windows Server 2022 Datacenter
- **自訂步驟**:
  1. 安裝 IIS 和管理工具
  2. 下載並安裝 .NET 10 Hosting Bundle
  3. 建立應用程式目錄
  4. 複製 SimpleWeb 應用程式
  5. 配置 IIS 網站和應用程式集區
  6. 重新啟動並驗證安裝
- **發佈目標**: Azure Shared Image Gallery

### .github/workflows/vm-image-builder.yml

GitHub Actions workflow，包含兩個主要 job：

1. **build-and-publish-app**: 建置 SimpleWeb 並打包
2. **build-vm-image**: 使用 AIB 建立 VM Image

## 工作流程

### 觸發方式

#### 1. 使用 Git Tag 自動觸發（推薦）

當您建立並推送版本標籤時，會自動觸發 Image 建置：

```bash
# 建立版本標籤
git tag v1.0.0
git push origin v1.0.0

# 或使用語義化版本
git tag 1.2.3
git push origin 1.2.3
```

支援的標籤格式：
- `v*.*.*` (例如: v1.0.0, v2.1.3)
- `*.*.*` (例如: 1.0.0, 2.1.3)

#### 2. 手動觸發

```bash
# 在 GitHub Actions 頁面點擊 "Run workflow"
# 或使用 GitHub CLI
gh workflow run vm-image-builder.yml
```

## 重要注意事項

### .NET 10 Hosting Bundle

⚠️ 目前 .NET 10 尚未正式發佈，AIB template 中的下載 URL 需要在 .NET 10 正式發佈後更新。

更新位置：
- 檔案: [image-builder/aib-template.json](image-builder/aib-template.json)
- 區段: "Install .NET 10 Hosting Bundle"
- 變數: `$downloadUrl`

請從官方網站取得正確的下載連結：https://dotnet.microsoft.com/download/dotnet/10.0

### 建置時間

Image 建置通常需要 30-60 分鐘，請耐心等待。

### 成本考量

Azure Image Builder 會產生以下費用：
- 建置 VM 的運算成本
- Shared Image Gallery 儲存成本
- 網路傳輸成本

## 環境變數配置

可在 [vm-image-builder.yml](.github/workflows/vm-image-builder.yml#L11-L18) 修改：

```yaml
env:
  AZURE_RESOURCE_GROUP: 'rg-simpleweb-imagebuilder'  # Resource Group 名稱
  LOCATION: 'eastus'                                  # Azure 區域
  IMAGE_TEMPLATE_NAME: 'simpleweb-vm-template'        # AIB 模板名稱
  SIG_NAME: 'sig_simpleweb'                          # Shared Image Gallery 名稱
  SIG_IMAGE_DEFINITION: 'SimpleWebVM'                 # Image Definition 名稱
```

## 使用建立的 Image

建置完成後，可以使用 Shared Image Gallery 中的 Image 建立 VM：

```bash
# 列出可用的 Image 版本
az sig image-version list \
  --resource-group rg-simpleweb-imagebuilder \
  --gallery-name sig_simpleweb \
  --gallery-image-definition SimpleWebVM

# 使用 Image 建立 VM
az vm create \
  --resource-group <YOUR_VM_RESOURCE_GROUP> \
  --name simpleweb-vm-001 \
  --image "/subscriptions/<SUBSCRIPTION_ID>/resourceGroups/rg-simpleweb-imagebuilder/providers/Microsoft.Compute/galleries/sig_simpleweb/images/SimpleWebVM/versions/<VERSION>" \
  --admin-username azureuser \
  --generate-ssh-keys
```

## 疑難排解

### 檢查 Image Builder 狀態

```bash
az image builder show \
  --resource-group rg-simpleweb-imagebuilder \
  --name simpleweb-vm-template \
  --query lastRunStatus
```

### 檢視建置日誌

在 Azure Portal 中：
1. 前往 Resource Group `rg-simpleweb-imagebuilder`
2. 找到 Image Template `simpleweb-vm-template`
3. 查看 "Last run" 狀態和日誌

## 相關資源

- [Azure Image Builder 文件](https://learn.microsoft.com/azure/virtual-machines/image-builder-overview)
- [Shared Image Gallery 文件](https://learn.microsoft.com/azure/virtual-machines/shared-image-galleries)
- [.NET 下載頁面](https://dotnet.microsoft.com/download)
