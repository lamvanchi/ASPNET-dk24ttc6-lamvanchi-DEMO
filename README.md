# Apple Shop - Project
## 📋 Mục lục
1. [Giới thiệu dự án](#giới-thiệu-dự-án)
2. [Cấu trúc dự án](#cấu-trúc-dự-án)
3. [Tài khoản đăng nhập](#tài-khoản-đăng-nhập)
4. [Chức năng từng trang](#chức-năng-từng-trang)
5. [Cơ sở dữ liệu](#cơ-sở-dữ-liệu)
6. [Hướng dẫn cài đặt](#hướng-dẫn-cài-đặt)
7. [API Endpoints](#api-endpoints)
8. [Troubleshooting](#troubleshooting)

##  Giới thiệu dự án

**Apple Shop** là website thương mại điện tử chuyên bán các sản phẩm Apple được phát triển bằng ASP.NET Core 9.0.

### Công nghệ sử dụng:
- **Backend:** ASP.NET Core 9.0 MVC
- **Database:** SQL
- **Authentication:** ASP.NET Identity
- **Frontend:** Bootstrap 5, jQuery
- **Icons:** Font Awesome

##  Cấu trúc dự án

```
AppleShopWeb/
├── Controllers/           # Các controller xử lý logic
│   ├── HomeController.cs     # Trang chủ
│   ├── ProductsController.cs # Quản lý sản phẩm
│   ├── AccountController.cs  # Đăng nhập/đăng ký
│   ├── CartController.cs     # Giỏ hàng
│   ├── OrdersController.cs   # Đơn hàng
│   ├── AdminController.cs    # Quản trị
│   └── DebugController.cs    # Debug (có thể xóa)
├── Models/                # Các model dữ liệu
│   └── ErrorViewModel.cs     # Chứa tất cả models
├── Views/                 # Giao diện người dùng
│   ├── Home/                 # Trang chủ
│   ├── Products/             # Sản phẩm
│   ├── Account/              # Tài khoản
│   ├── Cart/                 # Giỏ hàng
│   ├── Orders/               # Đơn hàng
│   ├── Admin/                # Quản trị
│   └── Shared/               # Layout chung
├── Data/                  # Cấu hình database
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── ViewModels/            # View models
├── wwwroot/               # Static files
└── Migrations/            # Database migrations
```

##  Tài khoản đăng nhập

### Tài khoản Admin:
- **Email:** `admin@appleshop.com`
- **Mật khẩu:** `Shopapple@2026`
- **Quyền:** Quản trị toàn bộ hệ thống

### Tài khoản khách hàng mẫu:
- **Email:** `guest@gmail.com`
- **Mật khẩu:** `guest@123456`
- **Quyền:** Mua sắm, xem đơn hàng

##  Chức năng từng trang

### 1. Trang chủ (`/`)
- Hiển thị 4 sản phẩm nổi bật
- Navigation menu
- Banner giới thiệu

### 2. Danh sách sản phẩm (`/Products`)
- Hiển thị tất cả sản phẩm Apple
- **Bộ lọc nâng cao:**
  - Theo danh mục (iPhone, iPad, Mac)
  - Theo thương hiệu
  - Theo khoảng giá
  - Theo dung lượng lưu trữ
  - Theo màu sắc
- Tìm kiếm AJAX

### 3. Chi tiết sản phẩm (`/Products/Details/{id}`)
- Thông tin chi tiết sản phẩm
- Hình ảnh, thông số kỹ thuật
- Lựa chọn dung lượng và màu sắc
- Thêm vào giỏ hàng

### 4. Giỏ hàng (`/Cart`)
- Hiển thị sản phẩm đã chọn
- Cập nhật số lượng
- Xóa sản phẩm
- Tính tổng tiền tự động

### 5. Đăng nhập/Đăng ký (`/Account`)
- **Login:** `/Account/Login`
- **Register:** `/Account/Register`
- **Logout:** `/Account/Logout`

### 6. Đơn hàng (`/Orders`)
- **Danh sách:** `/Orders` - Lịch sử đơn hàng
- **Chi tiết:** `/Orders/Details/{id}` - Thông tin chi tiết
- **Thanh toán:** `/Orders/Checkout` - Đặt hàng

### 7. Quản trị Admin (`/Admin`)
- **Dashboard:** `/Admin` - Thống kê tổng quan
- **Sản phẩm:** `/Admin/Products` - Quản lý sản phẩm
- **Đơn hàng:** `/Admin/Orders` - Quản lý đơn hàng
- **Khách hàng:** `/Admin/Customers` - Quản lý người dùng

## 🗄️ Cơ sở dữ liệu

### Bảng chính:
1. **AspNetUsers** - Người dùng
2. **Products** - Sản phẩm
3. **Orders** - Đơn hàng
4. **OrderItems** - Chi tiết đơn hàng
5. **AspNetRoles** - Vai trò người dùng

### Dữ liệu mẫu:
- 4 sản phẩm Apple (iPhone 17 Pro Max, Apple Watch Series 11, Studio Display Standard Glass, AirPods Pro 3)
- Tài khoản admin và guest
- Đơn hàng mẫu (được tạo tự động)

## ⚙️ Hướng dẫn cài đặt

### Yêu cầu hệ thống:
- .NET 9.0 SDK
- Visual Studio 2022 hoặc VS Code
- SQLite

### Các bước cài đặt:

1. **Clone dự án:**
```bash
git clone [repository-url]
cd AppleStoreWeb
```

2. **Restore packages:**
```bash
dotnet restore
```

3. **Tạo database:**
```bash
dotnet ef database update
```

4. **Chạy ứng dụng:**
```bash
dotnet run
```

5. **Truy cập:**
- Website: `http://localhost:7130`
- HTTPS: `https://localhost:5296`

## 🔌 API Endpoints

### Products API:
- `GET /Products/Search?term={keyword}` - Tìm kiếm sản phẩm

### Cart API:
- `POST /Cart/AddToCart` - Thêm vào giỏ hàng
- `POST /Cart/UpdateQuantity` - Cập nhật số lượng
- `POST /Cart/RemoveFromCart` - Xóa khỏi giỏ hàng
- `GET /Cart/GetCartCount` - Lấy số lượng giỏ hàng

### Admin API:
- `POST /Admin/UpdateOrderStatus` - Cập nhật trạng thái đơn hàng
- `POST /Admin/ToggleProductStock` - Bật/tắt tồn kho
- `POST /Admin/LockUser` - Khóa người dùng
- `POST /Admin/UnlockUser` - Mở khóa người dùng

## 🛠️ Troubleshooting

### Lỗi thường gặp:

1. **Lỗi 403 - Access Denied:**
   - Kiểm tra đã đăng nhập admin chưa
   - Xóa database và tạo lại: `del AppleStoreWeb.db && dotnet ef database update`

2. **Lỗi database:**
   - Chạy: `dotnet ef database update`
   - Nếu vẫn lỗi: Xóa file `AppleStoreWeb.db` và chạy lại

3. **Lỗi build:**
   - Chạy: `dotnet clean && dotnet build`

4. **Lỗi port đã sử dụng:**
   - Dừng process: `taskkill /f /im dotnet.exe`
   - Hoặc thay đổi port trong `launchSettings.json`

### Debug Commands:
```bash
# Kiểm tra tài khoản admin
GET /Debug/CheckAdmin

# Tạo lại tài khoản admin
POST /Debug/CreateAdmin

# Xem log chi tiết
dotnet run --verbosity detailed
```

##  Ghi chú phát triển

### Thêm sản phẩm mới:
1. Thêm vào `SeedData.cs`
2. Chạy `dotnet ef database update`

### Thêm tính năng mới:
1. Tạo Controller mới
2. Tạo Views tương ứng
3. Cập nhật navigation menu

### Cấu hình email (tùy chọn):
- Cập nhật `appsettings.json`
- Thêm SMTP settings

##  Bảo mật

- Mật khẩu được mã hóa bằng ASP.NET Identity
- Session timeout: 30 phút
- CSRF protection được bật
- SQL Injection protection qua Entity Framework

##  Hỗ trợ

Nếu gặp vấn đề, hãy kiểm tra:
1. Log trong console
2. File `AppleStoreWeb.db` có tồn tại không
3. Tài khoản admin đã được tạo chưa

---
- Sinh viên thực hiện: Lâm Văn Chỉ
- Số điện thoại: 0942 473 373  
- Email: lamchi93@gmail.com  
- Giảng viên hướng dẫn: TS. Đoàn Phước Miền  
- Thời gian thực hiện: Tháng 3 – 5/2026
