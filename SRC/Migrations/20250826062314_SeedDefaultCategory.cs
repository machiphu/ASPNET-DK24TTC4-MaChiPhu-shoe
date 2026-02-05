using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaChiPhuShoe.Migrations
{
    public partial class SeedDefaultCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Tạo bảng Categories
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            // 2) Thêm cột CategoryId vào Products NẾU CHƯA CÓ (tránh lỗi "Invalid column name")
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products', 'CategoryId') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD CategoryId INT NULL;
END
");

            // 3) Seed 1 category mặc định
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Name" },
                values: new object[] { "Không phân loại" });

            // 4) Gán mọi Products mồ côi về category mặc định
            migrationBuilder.Sql(@"
DECLARE @DefaultCatId INT = (SELECT TOP 1 Id FROM dbo.Categories WHERE Name = N'Không phân loại');

UPDATE P
   SET P.CategoryId = @DefaultCatId
FROM dbo.Products AS P
LEFT JOIN dbo.Categories AS C ON P.CategoryId = C.Id
WHERE P.CategoryId IS NULL OR P.CategoryId = 0 OR C.Id IS NULL;
");

            // 5) Ép NOT NULL (nếu anh muốn bắt buộc sản phẩm phải có danh mục)
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Products') AND name = 'CategoryId')
BEGIN
    ALTER TABLE dbo.Products ALTER COLUMN CategoryId INT NOT NULL;
END
");

            // 6) Tạo index + FK
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_CategoryId' AND object_id = OBJECT_ID('dbo.Products'))
    CREATE INDEX IX_Products_CategoryId ON dbo.Products(CategoryId);
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Products_Categories_CategoryId')
BEGIN
    ALTER TABLE dbo.Products WITH CHECK
    ADD CONSTRAINT FK_Products_Categories_CategoryId
        FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id)
        ON DELETE NO ACTION;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Products_Categories_CategoryId')
    ALTER TABLE dbo.Products DROP CONSTRAINT FK_Products_Categories_CategoryId;
");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_CategoryId' AND object_id = OBJECT_ID('dbo.Products'))
    DROP INDEX IX_Products_CategoryId ON dbo.Products;
");

            // Nếu muốn giữ cột CategoryId thì comment khối dưới
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products', 'CategoryId') IS NOT NULL
    ALTER TABLE dbo.Products DROP COLUMN CategoryId;
");

            migrationBuilder.DropTable(name: "Categories");
        }
    }
}
