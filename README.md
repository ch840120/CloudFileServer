# 👨‍💻 CloudFileServer (雲端檔案管理系統)
## 📝 使用者需求目標 - User Requirements
```
### 核心需求如下：
多樣化檔案支援：系統需能處理多種檔案格式，包含 Word 文件 (.docx)、圖片 (.png) 及純文字檔 (.txt) 。

檔案屬性管理：
基本屬性：所有檔案與目錄皆具備名稱、大小 (KB) 及建立時間 。
- Word 檔：額外記錄「頁數」 。
- 圖片檔：額外記錄「解析度（寬、高）」 。
- 純文字檔：額外記錄「編碼方式」（如：UTF-8、ASCII） 。

階層式目錄組織：
- 支援無限層級的子目錄嵌套（如 Windows 資料夾結構） 。
- 限制規則：所有檔案必須存放於目錄之下，不可獨立存在 。

功能需求 (Functional Requirements)
A. 核心結構功能
- 目錄結構呈現 (Display)：視覺化印出完整的樹狀目錄結構與檔案詳細資訊 。
- 總容量計算 (Size Calculation)：具備遞迴計算邏輯，可動態加總任一目錄及其下所有子項目的總大小 。
- 副檔名搜尋 (Search)：支援透過副檔名關鍵字搜尋，列出結構中所有符合條件的檔案路徑 。

B. 輸出與監控
- XML 結構匯出 (Serialization)：支援將目前的目錄階層與內容屬性轉換為指定的 XML 格式 。
- 執行歷程紀錄 (Traverse Log)：在執行運算或搜尋時，即時印出訪問節點的順序，以呈現系統的遍歷邏輯 。

C. 進階管理功能 (Bonus)
- 排序功能 (Sorting)：支援按名稱、大小、副檔名進行升冪或降冪排序 。
- 編輯作業 (Editing)：實作檔案與目錄節點的刪除、複製與貼上功能 。
- 標籤系統 (Tagging)：支援為項目貼上多重分類標籤（如：Urgent 紅色、Work 藍色、Personal 綠色） 。
- 狀態復原機制 (Undo / Redo)：實作操作動作的復原與重做功能，確保編輯彈性 。

📁 預期目錄結構範例
根目錄 (Root)
├── 專案文件 (Project_Docs) [目錄]
│   ├── 需求規格書.docx [Word 檔案] (頁數: 15, 大小: 500KB)
│   └── 系統架構圖.png [圖片] (解析度: 1920x1080, 大小: 2MB)
├── 個人筆記 (Personal_Notes) [目錄]
│   ├── 待辦清單.txt [純文字檔] (編碼: UTF-8, 大小: 1KB)
│   └── 2025備份 (Archive_2025) [子目錄]
│       └── 舊會議記錄.docx [Word 檔案] (頁數: 5, 大小: 200KB)
└── README.txt [純文字檔] (編碼: ASCII, 大小: 500B)
```
## 🎬 專案demo
### 📁 目錄結構呈現
<img width="1919" height="1080" alt="目錄結構呈現" src="https://github.com/user-attachments/assets/30946b3b-e567-4eba-9f7e-aa0694471977" />

### 📊 總容量計算
<img width="1920" height="1079" alt="計算總容量" src="https://github.com/user-attachments/assets/fc292a21-ed98-482a-af1a-bd2c09a580f0" />

### 🔍 副檔名搜尋 
<img width="1920" height="1080" alt="副檔名搜尋功能" src="https://github.com/user-attachments/assets/255c4b25-78f4-4b04-b2cd-b96267d6cead" />

### 📜 XML 結構匯出
<img width="1920" height="1080" alt="XML 結構輸出1" src="https://github.com/user-attachments/assets/a44fab72-27ae-457b-923a-b03fe91038ba" />
<img width="1921" height="1078" alt="XML 結構輸出2" src="https://github.com/user-attachments/assets/f44e5ed4-fbc3-4e3c-aa5e-6f893fa0e7de" />

### ⚖️ 排序功能
<img width="1920" height="1079" alt="排序功能1" src="https://github.com/user-attachments/assets/ebe70288-b736-42d5-90e0-c2eb08033d8d" />
<img width="1920" height="1080" alt="排序功能2" src="https://github.com/user-attachments/assets/2fc36ff3-169a-4986-a1d4-368a4b913c33" />

### ✂️ 編輯作業
<img width="1920" height="1080" alt="刪除1" src="https://github.com/user-attachments/assets/e50f4d86-3f23-405b-b602-51b8a64bd386" />
<img width="1920" height="1080" alt="刪除2" src="https://github.com/user-attachments/assets/bc0fb375-390d-4629-b8a0-6f9e0f4ee2e3" />
<img width="1920" height="1080" alt="複製貼上1" src="https://github.com/user-attachments/assets/42fd3140-3e7b-4a70-9387-0d2d4ea7d2e5" />
<img width="1920" height="1080" alt="複製貼上2" src="https://github.com/user-attachments/assets/8a60cdb2-a9e2-4bc4-959a-0638bbc680f1" />

### 🏷️ 標籤系統 
<img width="1920" height="1080" alt="新增標籤" src="https://github.com/user-attachments/assets/af3af9d1-bf22-4d48-b8ea-ee53f5e8a3b8" />
<img width="1920" height="1080" alt="移除標籤1" src="https://github.com/user-attachments/assets/33180754-8646-480f-aef3-14b3da952090" />
<img width="1920" height="1080" alt="移除標籤2" src="https://github.com/user-attachments/assets/7f51fa34-b8bb-42f6-a9a7-1f6caa2a7e67" />

### 🔄 狀態復原機制
<img width="1920" height="1080" alt="重做1" src="https://github.com/user-attachments/assets/c6b0283a-191a-4030-a4d6-905418a1f534" />
<img width="1915" height="1080" alt="重做2" src="https://github.com/user-attachments/assets/ed774a32-74ee-4f45-b8ac-5132dc5476b3" />
<img width="1920" height="1080" alt="復原1" src="https://github.com/user-attachments/assets/06309409-4081-4068-b410-50767e1382ee" />
<img width="1913" height="1080" alt="復原2" src="https://github.com/user-attachments/assets/44f6c970-2484-408c-bce8-94c57c6736c4" />

## 📐 UML 類別圖 - Domain Model
<img width="6771" height="5991" alt="CloudFileServer-UML" src="https://github.com/user-attachments/assets/a76da387-dc4c-4410-a8b4-b9e260c7e7ed" />

## 🛢️ Schema 設計 - ER Model 圖
<img width="1898" height="856" alt="CloudFileServerERModel" src="https://github.com/user-attachments/assets/c6490ef6-757f-4e1c-ae86-f7d341814f54" />

## 🚀 執行專案
### 📝 備註
#### 1. 採用本地專案MockFileStorage模擬真實檔案機的路徑
#### 2. 模擬資料沒有區分User，真實情況需要根據UserId產出對應顯示

### 步驟1: 🌐 環境準備 - MSSQL + UI建置(已準備可跳過)
```yml
version: '3.9'

services:
  mssql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: mssql_server
    restart: unless-stopped
    environment:
      ACCEPT_EULA: "Y"
      SA_PASSWORD: "Test@123456" 
      MSSQL_PID: "Developer"
    ports:
      - "1433:1433"
    volumes:
      - mssql_data:/var/opt/mssql
    networks:
      - db_network
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P 'Test@123456' -Q 'SELECT 1' -No || exit 1"]
      interval: 15s
      timeout: 10s
      retries: 5
      start_period: 30s

  adminer:
    image: adminer:latest
    container_name: adminer_ui
    restart: unless-stopped
    ports:
      - "8088:8080"
    environment:
      ADMINER_DEFAULT_SERVER: mssql
      ADMINER_DESIGN: "pepa-linha-dark"   
    depends_on:
      mssql:
        condition: service_healthy
    networks:
      - db_network

volumes:
  mssql_data:
    driver: local

networks:
  db_network:
    driver: bridge
```

```bash
docker compose up -d
```

### 步驟2: 🏗️ ER Model Code First建庫(已準備可跳過)
```bash
cd {專案路徑}\CloudFileServer.Persistent

dotnet ef migrations add InitialCreate `
  --startup-project ../CloudFileServer/CloudFileServer.csproj

dotnet ef database update `
  --startup-project ../CloudFileServer/CloudFileServer.csproj
```

### 步驟3: 🧪 初始化假資料，SQL INSERT語法(對應專案MockFileStorage/底下所有檔案)
```sql
USE [CloudFileServer];
  -- =============================================
  -- 清除舊資料
  -- =============================================
  DELETE FROM NodeTags;
  DELETE FROM NodeWordMeta;
  DELETE FROM NodeTextMeta;
  DELETE FROM NodeImageMeta;
  DELETE FROM Nodes;
  DELETE FROM Tags;
  DELETE FROM NodeTypes;

  -- =============================================
  -- 重置索引
  -- =============================================
  DBCC CHECKIDENT ('Tags', RESEED, 0);
  DBCC CHECKIDENT ('NodeTypes', RESEED, 0);
  DBCC CHECKIDENT ('Nodes', RESEED, 0);
  -- =============================================

  -- =============================================
  -- NodeTypes
  -- =============================================
  INSERT INTO NodeTypes (Code, IsLeaf, CreatedAt) VALUES (0, 0, GETUTCDATE());
  INSERT INTO NodeTypes (Code, IsLeaf, CreatedAt) VALUES (1, 1, GETUTCDATE());
  INSERT INTO NodeTypes (Code, IsLeaf, CreatedAt) VALUES (2, 1, GETUTCDATE());
  INSERT INTO NodeTypes (Code, IsLeaf, CreatedAt) VALUES (3, 1, GETUTCDATE());

  -- =============================================
  -- Tags
  -- =============================================
  INSERT INTO Tags (Name, Color) VALUES ('Urgent',   '#F44336');
  INSERT INTO Tags (Name, Color) VALUES ('Work',     '#2196F3');
  INSERT INTO Tags (Name, Color) VALUES ('Personal', '#4CAF50');
  INSERT INTO Tags (Name, Color) VALUES ('Archive',  '#9E9E9E');
  INSERT INTO Tags (Name, Color) VALUES ('Draft',    '#FF9800');

  -- =============================================
  -- Nodes
  -- 注意：目錄節點 StoragePath IS NULL，用此區分同名的檔案節點
  -- =============================================

  -- Level 1
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), NULL, 'Root', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- Level 2
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Root' AND StoragePath IS NULL), 'Documents', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Root' AND StoragePath IS NULL), 'Images', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Root' AND StoragePath IS NULL), 'Personal', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Root' AND StoragePath IS NULL), 'Archive', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- Level 3
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Documents' AND StoragePath IS NULL), 'Projects', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Documents' AND StoragePath IS NULL), 'Reports', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Documents' AND StoragePath IS NULL), 'Templates', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Images' AND StoragePath IS NULL), 'Wallpapers', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Images' AND StoragePath IS NULL), 'Profile', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Personal' AND StoragePath IS NULL), 'Diary', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Personal' AND StoragePath IS NULL), 'Notes', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Archive' AND StoragePath IS NULL), '2023', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Archive' AND StoragePath IS NULL), '2024', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- Level 4: Project sub-directories
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Projects' AND StoragePath IS NULL), 'Alpha', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=0), (SELECT Id FROM Nodes WHERE Name='Projects' AND StoragePath IS NULL), 'Beta', NULL, NULL, 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- Level 5: Files in Alpha
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=1), (SELECT Id FROM Nodes WHERE Name='Alpha' AND StoragePath IS NULL), 'spec', 153600, 'Documents/Projects/Alpha/spec.docx', 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=2), (SELECT Id FROM Nodes WHERE Name='Alpha' AND StoragePath IS NULL), 'wireframe', 204800, 'Documents/Projects/Alpha/wireframe.jpg', 0, NULL, GETUTCDATE(), GETUTCDATE()); 

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=3), (SELECT Id FROM Nodes WHERE Name='Alpha' AND StoragePath IS NULL), 'notes', 2048, 'Documents/Projects/Alpha/notes.txt', 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- Files in Beta
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=1), (SELECT Id FROM Nodes WHERE Name='Beta' AND StoragePath IS NULL), 'proposal', 122880, 'Documents/Projects/Beta/proposal.docx', 0, NULL, GETUTCDATE(), GETUTCDATE());    

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=2), (SELECT Id FROM Nodes WHERE Name='Beta' AND StoragePath IS NULL), 'mockup', 307200, 'Documents/Projects/Beta/mockup.jpg', 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- Files in Reports
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=1), (SELECT Id FROM Nodes WHERE Name='Reports' AND StoragePath IS NULL), 'annual-report', 512000, 'Documents/Reports/annual-report.docx', 0, NULL, GETUTCDATE(),
  GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=1), (SELECT Id FROM Nodes WHERE Name='Reports' AND StoragePath IS NULL), 'q1-summary', 102400, 'Documents/Reports/q1-summary.docx', 0, NULL, GETUTCDATE(), GETUTCDATE());   

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=3), (SELECT Id FROM Nodes WHERE Name='Reports' AND StoragePath IS NULL), 'meeting-notes', 4096, 'Documents/Reports/meeting-notes.txt', 0, NULL, GETUTCDATE(), GETUTCDATE());
  -- Files in Templates
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=1), (SELECT Id FROM Nodes WHERE Name='Templates' AND StoragePath IS NULL), 'contract', 81920, 'Documents/Templates/contract.docx', 0, NULL, GETUTCDATE(), GETUTCDATE());    

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=1), (SELECT Id FROM Nodes WHERE Name='Templates' AND StoragePath IS NULL), 'invoice', 40960, 'Documents/Templates/invoice.docx', 0, NULL, GETUTCDATE(), GETUTCDATE());      

  -- Files in Wallpapers
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=2), (SELECT Id FROM Nodes WHERE Name='Wallpapers' AND StoragePath IS NULL), 'sunset', 524288, 'Images/Wallpapers/sunset.jpg', 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=2), (SELECT Id FROM Nodes WHERE Name='Wallpapers' AND StoragePath IS NULL), 'mountain', 614400, 'Images/Wallpapers/mountain.jpg', 0, NULL, GETUTCDATE(), GETUTCDATE());     

  -- Files in Profile
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=2), (SELECT Id FROM Nodes WHERE Name='Profile' AND StoragePath IS NULL), 'avatar', 204800, 'Images/Profile/avatar.jpg', 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=2), (SELECT Id FROM Nodes WHERE Name='Profile' AND StoragePath IS NULL), 'cover', 409600, 'Images/Profile/cover.jpg', 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- Files in Diary
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=1), (SELECT Id FROM Nodes WHERE Name='Diary' AND StoragePath IS NULL), '2024', 65536, 'Personal/Diary/2024.docx', 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=1), (SELECT Id FROM Nodes WHERE Name='Diary' AND StoragePath IS NULL), '2025', 49152, 'Personal/Diary/2025.docx', 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- Files in Notes
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=3), (SELECT Id FROM Nodes WHERE Name='Notes' AND StoragePath IS NULL), 'todo', 1024, 'Personal/Notes/todo.txt', 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=3), (SELECT Id FROM Nodes WHERE Name='Notes' AND StoragePath IS NULL), 'ideas', 2048, 'Personal/Notes/ideas.txt', 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- Files in 2023
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=1), (SELECT Id FROM Nodes WHERE Name='2023' AND StoragePath IS NULL), 'old-report', 204800, 'Archive/2023/old-report.docx', 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=3), (SELECT Id FROM Nodes WHERE Name='2023' AND StoragePath IS NULL), 'backup-notes', 3072, 'Archive/2023/backup-notes.txt', 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- Files in 2024 (Archive)
  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=1), (SELECT Id FROM Nodes WHERE Name='2024' AND StoragePath IS NULL), 'year-review', 307200, 'Archive/2024/year-review.docx', 0, NULL, GETUTCDATE(), GETUTCDATE());

  INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)
  VALUES ((SELECT Id FROM NodeTypes WHERE Code=2), (SELECT Id FROM Nodes WHERE Name='2024' AND StoragePath IS NULL), 'summary', 153600, 'Archive/2024/summary.jpg', 0, NULL, GETUTCDATE(), GETUTCDATE());

  -- =============================================
  -- Meta tables（用 StoragePath 唯一識別每個檔案）
  -- =============================================
  INSERT INTO NodeWordMeta (NodeId, PageCount) VALUES
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Projects/Alpha/spec.docx'), 8),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Projects/Beta/proposal.docx'), 5),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Reports/annual-report.docx'), 24),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Reports/q1-summary.docx'), 6),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Templates/contract.docx'), 12),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Templates/invoice.docx'), 3),
  ((SELECT Id FROM Nodes WHERE StoragePath='Personal/Diary/2024.docx'), 120),
  ((SELECT Id FROM Nodes WHERE StoragePath='Personal/Diary/2025.docx'), 85),
  ((SELECT Id FROM Nodes WHERE StoragePath='Archive/2023/old-report.docx'), 15),
  ((SELECT Id FROM Nodes WHERE StoragePath='Archive/2024/year-review.docx'), 10);

  INSERT INTO NodeTextMeta (NodeId, Encoding) VALUES
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Projects/Alpha/notes.txt'), 'UTF-8'),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Reports/meeting-notes.txt'), 'UTF-8'),
  ((SELECT Id FROM Nodes WHERE StoragePath='Personal/Notes/todo.txt'), 'UTF-8'),
  ((SELECT Id FROM Nodes WHERE StoragePath='Personal/Notes/ideas.txt'), 'UTF-8'),
  ((SELECT Id FROM Nodes WHERE StoragePath='Archive/2023/backup-notes.txt'), 'UTF-8');

  INSERT INTO NodeImageMeta (NodeId, WidthPx, HeightPx) VALUES
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Projects/Alpha/wireframe.jpg'), 1920, 1080),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Projects/Beta/mockup.jpg'), 1280, 720),
  ((SELECT Id FROM Nodes WHERE StoragePath='Images/Wallpapers/sunset.jpg'), 3840, 2160),
  ((SELECT Id FROM Nodes WHERE StoragePath='Images/Wallpapers/mountain.jpg'), 2560, 1440),
  ((SELECT Id FROM Nodes WHERE StoragePath='Images/Profile/avatar.jpg'), 400, 400),
  ((SELECT Id FROM Nodes WHERE StoragePath='Images/Profile/cover.jpg'), 1500, 500),
  ((SELECT Id FROM Nodes WHERE StoragePath='Archive/2024/summary.jpg'), 1200, 800);

  -- =============================================
  -- NodeTags
  -- =============================================
  INSERT INTO NodeTags (NodeId, TagId) VALUES
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Projects/Alpha/spec.docx'),    (SELECT Id FROM Tags WHERE Name='Work')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Projects/Alpha/spec.docx'),    (SELECT Id FROM Tags WHERE Name='Draft')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Projects/Alpha/notes.txt'),    (SELECT Id FROM Tags WHERE Name='Work')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Projects/Beta/proposal.docx'), (SELECT Id FROM Tags WHERE Name='Work')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Projects/Beta/proposal.docx'), (SELECT Id FROM Tags WHERE Name='Draft')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Reports/annual-report.docx'),  (SELECT Id FROM Tags WHERE Name='Urgent')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Reports/annual-report.docx'),  (SELECT Id FROM Tags WHERE Name='Work')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Reports/q1-summary.docx'),     (SELECT Id FROM Tags WHERE Name='Work')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Reports/meeting-notes.txt'),   (SELECT Id FROM Tags WHERE Name='Work')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Documents/Templates/contract.docx'),     (SELECT Id FROM Tags WHERE Name='Work')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Personal/Diary/2024.docx'),              (SELECT Id FROM Tags WHERE Name='Personal')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Personal/Diary/2025.docx'),              (SELECT Id FROM Tags WHERE Name='Personal')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Personal/Notes/todo.txt'),               (SELECT Id FROM Tags WHERE Name='Personal')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Archive/2023/old-report.docx'),          (SELECT Id FROM Tags WHERE Name='Archive')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Archive/2023/backup-notes.txt'),         (SELECT Id FROM Tags WHERE Name='Archive')),
  ((SELECT Id FROM Nodes WHERE StoragePath='Archive/2024/year-review.docx'),         (SELECT Id FROM Tags WHERE Name='Archive'));
```

### 步驟4: ▶️ 啟動專案
```
cd {專案資料夾}
dotnet run
瀏覽器:http://localhost:5106/
```
