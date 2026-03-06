# CloudFileServer — AI 開發規範 (claude.md)

> 本文件為 AI 輔助開發的強制規範，所有程式碼生成、重構、補全均須遵守。

---

## 零、專案描述

**CloudFileServer** 是以 ASP.NET Core 8.0 實作的雲端檔案管理 Web API 服務，搭配 TypeScript 靜態前端頁面。

**目標與背景：**
本專案採用分層架構（Domain / Persistent / API / Tests），各層職責單一、相依方向由外向內，確保程式碼可維護、可測試且易於擴充。領域模型與資料存取邏輯透過介面解耦，任一層的實作可獨立替換而不影響其餘層。

---

## 一、專案結構總覽

```
CloudFileServer/                         # 方案根資料夾 (Solution)
├── CloudFileServer.Domain/              # 領域層：介面 & 模型
├── CloudFileServer.Persistent/          # 持久層：EF Core SQL 實作
├── CloudFileServer/                     # 主專案：Web API + 靜態頁面
│   ├── Applibs/
│   │   └── ConfigHelper.cs             # 強型別設定讀取輔助類別
│   ├── appsettings.json                 # 基礎設定（所有環境）
│   ├── appsettings.Development.json     # 開發環境覆蓋設定（不提交 Git）
│   └── wwwroot/                         # 靜態前端資源
└── CloudFileServer.Tests/              # 單元測試：Moq
```

---

## 二、技術棧規範

| 層級 | 技術 | 版本 |
|------|------|------|
| 後端框架 | ASP.NET Core Web API | .NET 8.0 |
| ORM | Entity Framework Core | 8.x |
| 前端腳本 | TypeScript | 5.x |
| 測試框架 | xUnit + Moq | 最新穩定版 |
| DI 容器 | ASP.NET Core 內建 DI | — |

---

## 三、各層職責與規範

### 3.1 CloudFileServer.Domain（領域層）

**職責：** 定義系統的核心概念，是整個方案唯一不依賴任何具體實作的專案，所有其他層都向它依賴。

**內容規範：**
- 只放介面（Interface）、領域實體（Entity）、DTO、Enum 與 Value Object。
- 介面定義資料存取的契約，由持久層負責實作，Domain 本身不實作任何介面。
- 領域模型只描述業務屬性與基本驗證，不包含資料庫映射邏輯。
- 命名空間依用途分為 `Domain.Interfaces` 與 `Domain.Models` 兩個子命名空間。

**EF Core Code First 規範：**
- Domain 層需引用 `Microsoft.EntityFrameworkCore` 套件，以支援 Code First 實體定義。
- 領域實體類別使用 EF Core 的 DataAnnotation（如 `[Key]`、`[Required]`、`[MaxLength]`）標註資料庫映射規則，讓實體本身即為資料庫結構的單一來源。
- 實體導覽屬性（Navigation Property）與外鍵屬性可直接定義在模型類別中，反映資料表之間的關聯關係。
- Fluent API 的進階設定（如複合主鍵、索引、資料表名稱）留給持久層的 `IEntityTypeConfiguration<T>` 處理，避免實體類別過度膨脹。
- Migration 的產生與執行由持久層的 `AppDbContext` 負責，Domain 層只定義實體結構，不涉及 Migration 操作。

**禁止事項：**
- 禁止在 Domain 層引用 EF Core 以外的資料庫相關套件（如 SqlClient、Dapper）。
- 禁止包含任何商業邏輯的具體實作。
- 禁止引用其他三個專案。

---

### 3.2 CloudFileServer.Persistent（持久層）

**職責：** 實作 Domain 層定義的所有 Repository 介面，負責與資料庫溝通的全部細節，上層不需要知道資料如何儲存。

**內容規範：**
- 每個 Domain 介面都必須有對應的具體實作類別放在此專案中。
- 使用 Entity Framework Core 的 `DbContext` 作為資料庫進入點，並以 Fluent API（`IEntityTypeConfiguration<T>`）設定資料表結構，禁止使用 DataAnnotation 做資料庫映射。
- Migration 檔案統一放在專案內的 `Migrations/` 子資料夾，由 EF Core CLI 管理，不手動修改。
- 提供 `IServiceCollection` 的擴充方法（Extension Method），讓主專案可以一行完成所有持久層的 DI 注冊，保持 `Program.cs` 整潔。
- 查詢時預設使用 `AsNoTracking()`，只有需要更新的情境才追蹤實體。

**禁止事項：**
- 禁止將 `IQueryable<T>` 跨層暴露給上層使用。
- 禁止在 Repository 中硬寫 SQL 字串，一律使用 LINQ。
- 禁止 Repository 直接依賴主專案或測試專案。

---

### 3.3 CloudFileServer（主專案 — Web API + 靜態頁面）

**職責：** 系統對外的進入點，負責 HTTP 路由、請求驗證、回應格式化，以及服務 TypeScript 編譯後的靜態前端頁面。

**後端規範：**
- `Program.cs` 只做應用程式組態與 DI 注冊，不包含任何商業邏輯。持久層與各服務的注冊統一透過各自的 Extension Method 完成。
- Controller 只負責路由分派與 HTTP 語意轉換（狀態碼、回應格式），不直接呼叫 Repository，必須透過 Service 或直接透過 Domain 介面。
- API 回應統一使用 `ActionResult<T>` 或 Minimal API 的 `Results<T>`，錯誤回應遵循 RFC 7807 的 `ProblemDetails` 格式。
- 靜態前端資源放在 `wwwroot/` 目錄下，TypeScript 編譯後的 JavaScript 輸出至 `wwwroot/js/`。
- 設定值（連線字串、金鑰等）統一從 `appsettings.json` 或環境變數讀取，禁止硬寫在程式碼中。

**appsettings 設定檔規範：**
- 專案根目錄維護兩支設定檔：`appsettings.json` 作為所有環境的基礎預設值；`appsettings.Development.json` 覆蓋開發環境專用設定（如本機連線字串、詳細 Log 等級）。
- `appsettings.Development.json` 不得提交至版本控制（加入 `.gitignore`），敏感資訊改以 .NET User Secrets 或環境變數管理。
- 設定檔結構以功能區段（Section）組織，例如 `ConnectionStrings`、`AppSettings`、`Jwt`，禁止將所有設定攤平放在根層級。
- ASP.NET Core 會依環境變數 `ASPNETCORE_ENVIRONMENT` 自動合併對應的 `appsettings.{Environment}.json`，`Program.cs` 無需手動載入。

**ConfigHelper 規範（`Applibs/ConfigHelper.cs`）：**
- 路徑固定為 `CloudFileServer/Applibs/ConfigHelper.cs`，命名空間為 `CloudFileServer.Applibs`。
- `ConfigHelper` 是靜態輔助類別，負責將 `IConfiguration` 中的各個 Section 強型別化後對外提供，讓其餘程式碼不直接操作原始 `IConfiguration`。
- 每個設定區段對應一個靜態唯讀屬性，型別為對應的 POCO 設定類別（Options Pattern），例如 `AppSettings`、`ConnectionStrings`。
- `ConfigHelper` 在應用程式啟動時由 `Program.cs` 呼叫靜態初始化方法，傳入 `IConfiguration` 實例完成綁定，之後全程唯讀，不允許執行期修改。
- 外部程式碼（Controller、Service）需要設定值時，一律透過 `ConfigHelper.AppSettings.SomeKey` 的方式取用，不直接注入 `IConfiguration`。
- 若設定值為必填但缺少，`ConfigHelper` 初始化時應拋出明確的例外訊息，提早暴露設定錯誤，禁止以 `null` 靜默帶過。

**前端 TypeScript 規範：**
- 所有前端腳本一律使用 TypeScript，禁止直接撰寫 `.js` 檔案。
- `tsconfig.json` 必須開啟 `strict: true`，不允許 `any` 型別（必要時以 `unknown` 替代）。
- API 呼叫統一封裝在 `src/api/` 目錄下的模組，禁止在 UI 元件或頁面腳本中直接使用 `fetch`。
- 所有非同步操作必須做錯誤處理，不可讓未捕捉的 Promise 靜默失敗。

---

### 3.4 CloudFileServer.Tests（單元測試層）

**職責：** 對 Controller、Service 等元件進行隔離的單元測試，使用 Moq 模擬所有外部相依，確保測試快速且不依賴資料庫或網路。

**規範：**
- 每個被測類別對應一個獨立的 `*Tests.cs` 測試檔案，放在對應的子資料夾中反映主專案結構。
- 測試方法命名採 `方法名稱_情境描述_預期結果` 格式（英文），讓測試名稱本身即為文件。
- 每個測試方法只驗證單一行為，一個測試只有一個邏輯 Assert，保持測試意圖清晰。
- 使用 `Mock<TInterface>` 隔離所有相依介面，不使用真實的 Repository 或 DbContext。
- 驗證互動行為時使用 Moq 的 `Verify`，驗證回傳值時使用 xUnit 的 `Assert`，兩者用途不混用。
- 測試類別的建構函式負責初始化 Mock 物件與被測系統（SUT），保持每個測試方法的 Arrange 區段簡潔。

**禁止事項：**
- 禁止測試中使用真實資料庫連線、真實 HTTP 請求或 `Thread.Sleep`。
- 禁止測試間共用可變的靜態狀態。
- 禁止在測試中 `new` 具體的 Repository 或 DbContext 實作。

---

## 四、命名規範

| 項目 | 規則 |
|------|------|
| 介面 | `I` 前綴 + PascalCase，例如 `IFileRepository` |
| 實作類別 | PascalCase，不加前綴，例如 `FileRepository` |
| 非同步方法 | 結尾一律加 `Async`，例如 `GetByIdAsync` |
| DTO / Request / Response | 結尾加 `Dto`、`Request` 或 `Response` |
| 測試類別 | 被測類別名稱加 `Tests` 後綴 |
| TypeScript 檔案 | camelCase，例如 `filesApi.ts` |
| TypeScript 型別/介面 | PascalCase，例如 `FileEntry` |

---

## 五、錯誤處理規範

- 後端統一透過 `ProblemDetails` 回應 API 錯誤，不在 Controller 中各自組裝錯誤訊息。
- Repository 層不吞例外，讓上層自行決定錯誤處理策略。
- 禁止使用空白的 `catch` 區塊靜默吞掉例外。
- TypeScript 所有 API 呼叫皆需 `try/catch`，錯誤訊息須顯示給使用者或記錄至 console，不可靜默失敗。

---

## 六、AI 生成程式碼禁止事項

1. 禁止在 Domain 層引用 EF Core 以外的資料庫相關套件（如 SqlClient、Dapper）。
2. 禁止在 Controller 直接 `new` DbContext 或 Repository。
3. 禁止測試中使用真實資料庫或真實 HTTP 呼叫。
4. 禁止使用單一字母變數名稱（迴圈計數器 `i`、`j` 除外）。
5. 禁止裸露的空白 `catch` 區塊吞錯。
6. 禁止在 TypeScript 使用 `any` 型別。
7. 禁止跨層暴露 `IQueryable<T>`。
8. 禁止將設定值（連線字串、密碼等）硬寫在程式碼中。

---

## 七、Git Commit 規範

格式為 `<type>(<scope>): <subject>`，type 使用 `feat`、`fix`、`refactor`、`test`、`docs`、`chore` 之一，scope 對應 `domain`、`persistent`、`api`、`frontend`、`tests` 其中一個層級，subject 以祈使語氣簡述變更內容。

---

*本規範應隨專案演進持續更新，修改後同步通知所有協作成員。*