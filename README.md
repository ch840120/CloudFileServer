# CloudFileServer
📁 雲端檔案管理系統
## 🛢️ Schema 設計 - ER Model 圖
<img width="1898" height="856" alt="CloudFileServerERModel" src="https://github.com/user-attachments/assets/c6490ef6-757f-4e1c-ae86-f7d341814f54" />


## 🌐 環境準備
### 🐳 MSSQL + UI建置
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

### 🏗️ ER Model Code First建庫
```bash
cd {專案路徑}\CloudFileServer.Persistent

dotnet ef migrations add InitialCreate `
  --startup-project ../CloudFileServer/CloudFileServer.csproj

dotnet ef database update `
  --startup-project ../CloudFileServer/CloudFileServer.csproj
```

### 🧪 初始化假資料，SQL INSERT語法（使用者自行執行）
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
