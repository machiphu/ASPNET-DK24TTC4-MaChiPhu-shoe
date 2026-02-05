# XÂY DỰNG WEBSITE BÁN GIÀY DÉP MACHIPHU-SHOE

## 📌 Thông tin cá nhân

- **Họ và tên**: MÃ CHÍ PHÚ  

- **Mã lớp**: DK24TTC4  

- **MSSV**: 170124381

- **Email**: machiphu@gmail.com

- **Số điện thoại**: 0352129582 

## Cấu trúc thư mục

- Progress-report: báo cáo tiến độ

- SRC: chứa toàn bộ soucre code, database của website: Areas, Bin, Obj, Components, Controllers, Data, Migrations, Models, Properties, Utilities, Views, wwwroot, appsettings.json, MaChiPhuShoe.csproj, Program.cs, MaChiPhuShoe.bak

  + Trong đó  Bin, Obj :thư mục build chạy thử từ lúc tạo project

- Thesis: báo cáo đồ án

- ReadMe: Thông tin đồ án, hướng dẫn cài đặt chạy đồ án

## Cách cài đặt

   1. Yêu cầu hệ thống

    -  .NET SDK 8.0** (khớp phiên bản project)

    -  SQL Server: (LocalDB / SQL Express / SQL Server 2019 trở lên)

    -  SSMS(SQL Server Management Studio tùy chọn): để quản trị cơ sở dữ liệu

    -  Visual Studio 2022 với workloads:

       + ASP.NET and web development

       +.NET desktop development

   2. Lấy mã nguồn

    - Git clone website MaChiPhu-Shoe từ : https://github.com/machiphu/ASPNET-DK24TTC4-MaChiPhu-shoe.git về máy.

    - Mở Visual Studio: Open → ASPNET-DK24TTC4-MACHIPHU-SHOE/SRC/MaChiPhuShoe.csproj

    - Hoặc Terminal: cd ASPNET-DK24TTC4-MACHIPHU-SHOE/SRC

   3. Cấu hình chuỗi kết nối (Connection String)

   - Mở SRC/appsettings.json (và *nếu có* `appsettings.Development.json`) → chỉnh `ConnectionStrings:DefaultConnection` phù hợp môi trường.

   - LocalDB (mặc định khi cài VS):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MaChiPhuShoe;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

**SQL Express:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=MaChiPhuShoe;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**SQL Server (user/password):**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SERVERNAME;Database=MaChiPhuShoe;User Id=sa;Password=your_password;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

---

   4. Khởi tạo CSDL (chọn **một** trong hai cách)

	- Cách A — **EF Core Migration** *(khuyến nghị)*

	**Visual Studio → Tools → NuGet Package Manager → Package Manager Console (PMC)**, chọn đúng project `SRC` làm *Default Project*, rồi:
 
      ```powershell
        # Nếu dự án CHƯA có thư mục Migrations

         Add-Migration InitialCreate -Context ApplicationDbContext

        # Tạo DB theo Migration hiện có

         Update-Database -Context ApplicationDbContext
       ```

     > Nếu *đã có* thư mục **Migrations/** trong source, có thể bỏ `Add-Migration` và chạy thẳng `Update-Database`.

	- Cách B — **Restore từ file .bak**

     a. Mở **SSMS** → kết nối tới `((localdb)\MSSQLLocalDB)` hoặc instance của bạn.

     b. **Databases → Right click → Restore Database…**

     c. Chọn **Device** → trỏ tới file **`MaChiPhuShoe.bak`** trong repo → **Restore**.

     d. Chỉnh lại `Database` name trong connection string trùng với DB vừa restore.

   5. Chạy ứng dụng

    - Cách 1 — **Visual Studio / IIS Express , Http, Https**

      a. **Build** (Ctrl+Shift+B)

      b. Chọn http , https ,  **IIS Express** → **Run**

      c. Trình duyệt tự mở `http://localhost:xxxxx` (ví dụ `http://localhost:5196` , http://localhost:7288 , http://localhost:44378)
      
      d. User quản trị : machiphu@gmail.com , mật khẩu : 123456

    - Cách 2 — **Kestrel** (không cần VS)

     ```bash
        cd SRC

       # Phục hồi & build
          dotnet restore
          dotnet build

       # Chạy
          dotnet run
      ```

     > Terminal sẽ in URL (thường `http://localhost:5xxx`).

      ---

   6. Script tự động hoá (Windows PowerShell)

     Tạo file **`scripts/setup.ps1`** với nội dung sau:

     ```powershell
       param(
         [string]$ProjectPath = "./SRC",
         [string]$SqlServer = "(localdb)\MSSQLLocalDB",
         [string]$Database = "MaChiPhuShoe",
         [string]$User = "",
         [string]$Password = "",
         [switch]$UseWindowsAuth = $true,
         [switch]$Run = $false
            )
function Update-ConnStr($filePath) {
  if (-not (Test-Path $filePath)) { return }
  $json = Get-Content $filePath -Raw | ConvertFrom-Json
  if (-not $json.ConnectionStrings) { $json | Add-Member -MemberType NoteProperty -Name ConnectionStrings -Value (@{}) }

  if ($UseWindowsAuth) {
    $conn = "Server=$SqlServer;Database=$Database;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  } else {
    $conn = "Server=$SqlServer;Database=$Database;User Id=$User;Password=$Password;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }

  $json.ConnectionStrings.DefaultConnection = $conn
  $json | ConvertTo-Json -Depth 100 | Set-Content -Path $filePath -Encoding UTF8
  Write-Host "Updated connection string in $filePath"
}

Push-Location $ProjectPath

# 1) Kiểm tra .NET SDK
try { dotnet --info | Out-Null } catch { Write-Error ".NET SDK not found."; exit 1 }

# 2) Đảm bảo có dotnet-ef
$ef = (dotnet tool list -g | Select-String -Pattern 'dotnet-ef')
if (-not $ef) { dotnet tool install -g dotnet-ef }

# 3) Restore & Build
if ((dotnet restore).ExitCode -ne 0) { Write-Error "dotnet restore failed"; exit 1 }
if ((dotnet build -c Debug).ExitCode -ne 0) { Write-Error "dotnet build failed"; exit 1 }

# 4) Cập nhật appsettings
Update-ConnStr -filePath "./appsettings.json"
if (Test-Path "./appsettings.Development.json") { Update-ConnStr -filePath "./appsettings.Development.json" }

# 5) Tạo/Cập nhật DB theo Migration
$exit = (dotnet ef database update --context ApplicationDbContext).ExitCode
if ($exit -ne 0) { Write-Warning "EF update failed. Kiểm tra connection string & công cụ." }

# 6) Chạy ứng dụng (tuỳ chọn)
if ($Run) { dotnet run }

Pop-Location
```

**Cách dùng**

```powershell
# Cửa sổ PowerShell mở tại thư mục gốc repo
./scripts/setup.ps1 -ProjectPath ./SRC -SqlServer "(localdb)\MSSQLLocalDB" -Database MaChiPhuShoe -UseWindowsAuth -Run

# Hoặc dùng tài khoản SQL
./scripts/setup.ps1 -ProjectPath ./SRC -SqlServer ".\SQLEXPRESS" -Database MaChiPhuShoe -User sa -Password "your_password" -Run
```

---

## 7. Khắc phục sự cố (FAQ)

* **Đã cài SSMS nhưng không kết nối được?** → Cần **cài SQL Server** (SSMS chỉ là công cụ quản trị).
* **`dotnet ef` không chạy?** → Cài công cụ: `dotnet tool install -g dotnet-ef`.
* **`Update-Database` lỗi chứng chỉ** → Thêm `TrustServerCertificate=True` vào chuỗi kết nối.
* **Không thấy DB sau khi Restore .bak** → Kiểm tra *Database name* khớp với connection string.

---

