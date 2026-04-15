# Apple Shop - Project

## Mục lục :
1. [Giới thiệu project](#giới-thiệu-project)
2. [Cấu trúc-project](#cấu-trúc-project)
3. [Tài khoản đăng nhập](#tài-khoản-đăng-nhập)
4. [Chức năng từng trang](#chức-năng-từng-trang)
5. [Cơ sở dữ liệu](#cơ-sở-dữ-liệu)
6. [Hướng dẫn cài đặt](#hướng-dẫn-cài-đặt)
7. [API Endpoints](#api-endpoints)
8. [Troubleshooting](#troubleshooting)
## Giới thiệu project

**Apple Shop** là website thương mại điện tử chuyên bán các sản phẩm Apple được phát triển bằng ASP.NET Core 9.0.

### Công nghệ sử dụng:
- **Backend:** ASP.NET Core 9.0 MVC
- **Database:** SQL
- **Authentication:** ASP.NET Identity
- **Frontend:** Bootstrap 5, jQuery
- **Icons:** Font Awesome

####  Cấu trúc project

```
AppleStoreWeb/
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
##### Tài khoản đăng nhập

###### Tài khoản Admin:
- **Email:** `admin@applestore.com`
- **Mật khẩu:** `AdminShop@123`
- **Quyền:** Administrator

######## Tài khoản khách hàng mẫu:
- **Email:** `customer@example.com`
- **Mật khẩu:** `Guest@123456`
- **Quyền:** Mua sắm, xem đơn hàng

######### Chức năng từng trang :
1. Trang chủ (`/`)
- Hiển thị 8 sản phẩm nổi bật
- Navigation menu
- Banner giới thiệu
