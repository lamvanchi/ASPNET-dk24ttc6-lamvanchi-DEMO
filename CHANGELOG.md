# Apple Shop Web – Lịch sử thay đổi

## Phiên bản 1.0.0 – 24/04/2026

### Tính năng chính

**Quản lý sản phẩm**
- Hiển thị sản phẩm với hình ảnh, giá và thông tin cơ bản  
- Lọc theo danh mục, giá, dung lượng và màu sắc  
- Tìm kiếm nhanh bằng AJAX  
- Trang chi tiết cho phép chọn cấu hình  

**Giỏ hàng**
- Thêm, xóa và cập nhật số lượng sản phẩm  
- Lưu tạm bằng Session  
- Hiển thị số lượng trên thanh menu  
- Tự động tính tổng tiền  

**Đặt hàng**
- Quy trình đặt hàng hoàn chỉnh  
- Hỗ trợ nhiều phương thức thanh toán (COD, chuyển khoản, thẻ)  
- Lưu lịch sử đơn hàng  
- Theo dõi trạng thái đơn hàng  

**Quản trị hệ thống**
- Dashboard hiển thị thống kê cơ bản  
- Quản lý sản phẩm và tồn kho  
- Cập nhật trạng thái đơn hàng  
- Quản lý tài khoản người dùng  

**Tài khoản người dùng**
- Đăng ký và đăng nhập bằng ASP.NET Identity  
- Phân quyền Admin và Guest
- Quản lý session và cookie  

---

### Công nghệ sử dụng

- Backend: ASP.NET Core MVC (.NET 9)  
- Frontend: Bootstrap 5, jQuery, Font Awesome  
- Database: SQL
- Authentication: ASP.NET Identity  
- Session: dùng cho giỏ hàng  

---

### Dữ liệu mẫu

Hệ thống được cung cấp sẵn một số sản phẩm để phục vụ demo:

- Apple Watch Series 11  
- AirPods Pro 3  
- Studio Display Standard Glass  
- iPhone 17 Pro Max  

---

### Giao diện

- Responsive trên mobile, tablet và desktop  
- Thiết kế theo phong cách Apple, đơn giản và dễ sử dụng  
- Hỗ trợ giao diện sáng/tối  
- Có hiệu ứng loading khi gọi AJAX  

---

### Bảo mật

- Mật khẩu được mã hóa bằng ASP.NET Identity  
- Bảo vệ CSRF  
- Hạn chế SQL Injection thông qua Entity Framework  
- Razor tự động encode để giảm thiểu XSS  

---

### Điểm nổi bật

- Giỏ hàng cập nhật số lượng theo thời gian thực  
- Tìm kiếm sản phẩm không cần reload trang  
- Dashboard hiển thị dữ liệu tổng quan  
- Theo dõi trạng thái đơn hàng  

---

### Sửa lỗi

- Sửa lỗi không tạo được tài khoản admin  
- Sửa lỗi thiếu trang Orders  
- Sửa lỗi migration database  
- Sửa lỗi session giỏ hàng bị mất  

---

### Tài liệu

- README.md: hướng dẫn cài đặt  
- TECHNICAL_GUIDE.md: tài liệu kỹ thuật  
- USER_MANUAL.md: hướng dẫn sử dụng  
- CHANGELOG.md: lịch sử thay đổi  

---

## Kế hoạch phát triển

### Phiên bản 1.1.0
- Thêm, sửa, xóa sản phẩm từ Admin  
- Upload hình ảnh sản phẩm  
- Hệ thống đánh giá và bình luận  
- Wishlist (danh sách yêu thích)  
- So sánh sản phẩm  

Cải tiến:
- Phân trang danh sách sản phẩm  
- Gửi email thông báo đơn hàng  
- Xuất báo cáo Excel/PDF  
- Tối ưu hiệu năng  

---

### Phiên bản 1.2.0
- Hệ thống khuyến mãi và mã giảm giá  
- Tích hợp thanh toán online (VNPay, MoMo)  
- Thông báo real-time  
- AI Chatbot 

Mobile:
- Phát triển ứng dụng bằng React Native  
- Xây dựng API cho mobile  
- Push notification  

---

## Ghi chú

Yêu cầu hệ thống:
- .NET 9 trở lên  
- SQL
- Trình duyệt hiện đại  

Tương thích:
- Windows, macOS, Linux  

Hiệu năng:
- Thời gian tải trang chủ < 2 giây  
- Tối ưu truy vấn database  
- Sử dụng tài nguyên thấp  
- Hỗ trợ khoảng 100 người dùng đồng thời  

---

Phiên bản hiện tại: 1.0.0  
Ngày phát hành: 24/04/2026  
Tác giả: TS. Đoàn Phước Miền  
