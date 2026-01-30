# Azure VM Image Builder 權限設定

## 問題
服務主體無法建立角色分配（Role Assignment），因為它只有 `Contributor` 角色。

## 解決方案

### 選項 1：手動設定 Identity 權限（推薦）

在 Azure Portal 或 CLI 手動為 `simpleweb-aib-identity` 設定權限：

```bash
# 取得 subscription ID
SUBSCRIPTION_ID=$(az account show --query id --output tsv)

# 取得 identity 的 principal ID
IDENTITY_PRINCIPAL_ID=$(az identity show \
  --resource-group Demo \
  --name simpleweb-aib-identity \
  --query principalId --output tsv)

# 為 identity 指派 Contributor 角色
az role assignment create \
  --assignee $IDENTITY_PRINCIPAL_ID \
  --role "Contributor" \
  --scope "/subscriptions/$SUBSCRIPTION_ID/resourceGroups/Demo"
```

執行後，可以在工作流程中移除 "Assign permissions to User Assigned Identity" 步驟。

### 選項 2：提升服務主體權限

如果您希望自動化角色分配，需要提升服務主體的權限：

```bash
# 方法 A: 在 Resource Group 層級給予 Owner 角色
az role assignment create \
  --assignee <SERVICE_PRINCIPAL_APP_ID> \
  --role "Owner" \
  --scope "/subscriptions/<SUBSCRIPTION_ID>/resourceGroups/Demo"

# 方法 B: 在 Resource Group 層級給予 User Access Administrator 角色（加上原有的 Contributor）
az role assignment create \
  --assignee <SERVICE_PRINCIPAL_APP_ID> \
  --role "User Access Administrator" \
  --scope "/subscriptions/<SUBSCRIPTION_ID>/resourceGroups/Demo"
```

### 選項 3：移除自動權限設定（最簡單）

直接移除工作流程中的 "Assign permissions to User Assigned Identity" 步驟，並手動一次性設定好所有權限。

## 建議

**使用選項 1** - 一次性手動設定，之後工作流程就不需要嘗試建立角色分配了。
