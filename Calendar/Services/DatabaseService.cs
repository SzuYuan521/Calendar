using Microsoft.Data.SqlClient; // 使用 Microsoft.Data.SqlClient 進行 SQL Server 的資料庫連接
using Dapper; // 引入 Dapper 來進行輕量級的 ORM(物件關聯映射)
using Calendar.Models; // 引入 Calendar 的資料模型

namespace Calendar.Services
{
    // 資料庫服務類別, 用於操作資料庫
    public class DatabaseService
    {
        private readonly string _connectionString;

        // 建構函數: 從配置檔案中取得連接字串
        public DatabaseService(IConfiguration configuration)
        {
            // 從配置檔中取得名為 "DefaultConnection" 的連接字串
            // 如果連接字串為空, 則拋出例外
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        // 初始化資料庫: 建立資料表 Events, 若該表不存在
        public void InitializeDatabase()
        {
            // 使用連接字串建立與 SQL Server 的連接
            using var connection = new SqlConnection(_connectionString);

            // 執行 SQL 指令, 檢查資料表 Events 是否存在, 若不存在則建立
            connection.Execute(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Events' and xtype='U')
                CREATE TABLE Events (
                    Id INT PRIMARY KEY IDENTITY(1,1), -- 自動遞增的主鍵
                    Title NVARCHAR(255) NOT NULL, -- 事件標題, 長度 255 的字元
                    DateTime DATETIME2 NOT NULL, -- 事件日期時間, DATETIME2 精確度更高
                    HasReminder BIT NOT NULL, -- 是否有提醒, 0 或 1(布林值)
                    ReminderMinutes INT NOT NULL -- 提醒的分鐘數
                )");
        }

        // 取得所有事件: 從 Events 表中查詢所有的事件, 並按日期排序
        public IEnumerable<Calendars> GetAllEvents()
        {
            // 建立與資料庫的連接
            using var connection = new SqlConnection(_connectionString);

            // 執行查詢, 取得所有的事件並按 DateTime 排序
            return connection.Query<Calendars>("SELECT * FROM Events ORDER BY DateTime");
        }

        // 取得單一事件: 根據事件的 Id 查詢特定事件
        public Calendars? GetEvent(int id)
        {
            // 建立與資料庫的連接
            using var connection = new SqlConnection(_connectionString);

            // 使用 SQL 查詢來取得特定 Id 的事件
            return connection.QueryFirstOrDefault<Calendars>(
                "SELECT * FROM Events WHERE Id = @Id", new { Id = id }); // 使用匿名物件來傳遞參數
        }

        // 新增事件: 將新的事件插入到 Events 表
        public void AddEvent(Calendars evt)
        {
            // 建立與資料庫的連接
            using var connection = new SqlConnection(_connectionString);

            // 執行插入 SQL, 將事件的資料插入 Events 表
            connection.Execute(@"
                INSERT INTO Events (Title, DateTime, HasReminder, ReminderMinutes)
                VALUES (@Title, @DateTime, @HasReminder, @ReminderMinutes)", evt); // 直接將事件物件作為參數
        }

        // 更新事件: 更新指定的事件
        public void UpdateEvent(Calendars evt)
        {
            // 建立與資料庫的連接
            using var connection = new SqlConnection(_connectionString);

            // 執行 SQL 更新語句, 更新事件的資訊
            connection.Execute(@"
                UPDATE Events 
                SET Title = @Title, DateTime = @DateTime, 
                    HasReminder = @HasReminder, ReminderMinutes = @ReminderMinutes
                WHERE Id = @Id", evt); // 依據 Id 更新事件資料
        }

        // 刪除事件: 根據 Id 刪除特定事件
        public void DeleteEvent(int id)
        {
            // 建立與資料庫的連接
            using var connection = new SqlConnection(_connectionString);

            // 執行 SQL 刪除語句, 刪除指定 Id 的事件
            connection.Execute("DELETE FROM Events WHERE Id = @Id", new { Id = id }); // 使用匿名物件作為參數
        }
    }
}
