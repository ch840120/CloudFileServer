# CloudFileServer
📁 雲端檔案管理系統

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

### 🏗️ ER Model Code Frist建庫
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
-- 0. 清空所有資料表（依照外鍵順序，子表先刪）
DELETE FROM NodeTags;
DELETE FROM NodeWordMeta;
DELETE FROM NodeTextMeta;
DELETE FROM NodeImageMeta;
DELETE FROM Nodes;
DELETE FROM NodeTypes;
DELETE FROM Tags;

-- 重置 Identity 從 1 開始
DBCC CHECKIDENT ('NodeTypes', RESEED, 0);
DBCC CHECKIDENT ('Tags',      RESEED, 0);
DBCC CHECKIDENT ('Nodes',     RESEED, 0);

-- 1. NodeTypes
INSERT INTO NodeTypes (Code, IsLeaf, CreatedAt) VALUES
(0, 0, GETUTCDATE()),
(1, 1, GETUTCDATE()),
(2, 1, GETUTCDATE()),
(3, 1, GETUTCDATE());

-- 2. Tags
INSERT INTO Tags (Name, Color) VALUES
('Urgent',   '#F44336'),
('Work',     '#2196F3'),
('Personal', '#4CAF50');

-- 3. Nodes
INSERT INTO Nodes (NodeTypeId, ParentId, Name, SizeBytes, StoragePath, IsDeleted, DeletedAt, CreatedAt, UpdatedAt) VALUES
(1, NULL, 'Root',          NULL,   NULL,                      0, NULL, GETUTCDATE(), GETUTCDATE()),
(1,    1, 'Documents',     NULL,   NULL,                      0, NULL, GETUTCDATE(), GETUTCDATE()),
(1,    1, 'Images',        NULL,   NULL,                      0, NULL, GETUTCDATE(), GETUTCDATE()),
(1,    1, 'Personal',      NULL,   NULL,                      0, NULL, GETUTCDATE(), GETUTCDATE()),
(2,    2, 'Annual Report', 153600, 'docs/annual-report.docx',  0, NULL, GETUTCDATE(), GETUTCDATE()),
(4,    2, 'Meeting Notes', 2048,   'docs/meeting-notes.txt',   0, NULL, GETUTCDATE(), GETUTCDATE()),
(3,    3, 'Sunset',        512000, 'images/sunset.jpg',        0, NULL, GETUTCDATE(), GETUTCDATE()),
(3,    3, 'Profile',       204800, 'images/profile.jpg',       0, NULL, GETUTCDATE(), GETUTCDATE()),
(2,    4, 'Diary',         81920,  'personal/diary.docx',      0, NULL, GETUTCDATE(), GETUTCDATE());

-- 4. Meta tables
INSERT INTO NodeWordMeta  (NodeId, PageCount)         VALUES (5, 12), (9, 5);
INSERT INTO NodeTextMeta  (NodeId, Encoding)          VALUES (6, 'UTF-8');
INSERT INTO NodeImageMeta (NodeId, WidthPx, HeightPx) VALUES (7, 1920, 1080), (8, 400, 400);

-- 5. NodeTags
INSERT INTO NodeTags (NodeId, TagId) VALUES
(5, 1), (5, 2),
(6, 2),
(8, 3),
(9, 3);
```