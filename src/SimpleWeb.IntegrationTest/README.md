# SimpleWeb 整合測試 (Integration Tests)

此專案包含 SimpleWeb 應用程式的整合測試。整合測試會啟動完整的 HTTP 請求/回應管道來測試應用程式的行為。

This project contains integration tests for the SimpleWeb application. Integration tests start up the full HTTP request/response pipeline to test the application's behavior.

## 測試內容 (Test Coverage)

### HomeControllerIntegrationTests
測試主要的控制器端點 (Tests main controller endpoints):
- Index 頁面 (Index page)
- Privacy 頁面 (Privacy page)
- Upload 頁面 (Upload page)
- Error 頁面 (Error page)
- 身份驗證標頭處理 (Authentication header handling)
- 404 錯誤處理 (404 error handling)

### HealthCheckIntegrationTests
測試健康檢查端點 (Tests health check endpoint):
- 健康檢查回應狀態 (Health check response status)
- JSON 回應格式 (JSON response format)
- 健康檢查結構驗證 (Health check structure validation)

## 執行測試 (Running Tests)

```bash
# 執行所有整合測試 (Run all integration tests)
dotnet test SimpleWeb.IntegrationTest/SimpleWeb.IntegrationTest.csproj

# 執行特定測試類別 (Run specific test class)
dotnet test --filter "FullyQualifiedName~HomeControllerIntegrationTests"

# 執行特定測試方法 (Run specific test method)
dotnet test --filter "FullyQualifiedName~Index_ReturnsSuccessAndCorrectContentType"
```

## 技術細節 (Technical Details)

- 使用 Microsoft.AspNetCore.Mvc.Testing 建立測試伺服器 (Uses Microsoft.AspNetCore.Mvc.Testing to create test server)
- 使用 FluentAssertions 進行斷言 (Uses FluentAssertions for assertions)
- 使用 AngleSharp 解析 HTML 回應 (Uses AngleSharp to parse HTML responses)
- CustomWebApplicationFactory 提供測試配置 (CustomWebApplicationFactory provides test configuration)

## 依賴套件 (Dependencies)

- Microsoft.AspNetCore.Mvc.Testing 6.0.11
- FluentAssertions 6.8.0
- AngleSharp 0.17.1
- MSTest.TestFramework 2.2.10
