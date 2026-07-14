# DỰ ÁN QUẢN LÝ ĐẶT VÉ XEM PHIM - ASP.NET WEB FORMS

Hệ thống quản lý và đặt vé xem phim trực tuyến được xây dựng bằng công nghệ ASP.NET Web Forms (C#) kết hợp với Cơ sở dữ liệu LocalDB (SQL Server) và thiết kế giao diện Cinematic Dark Mode hiện đại.

---

## 🛠️ Hướng dẫn Mở và Chạy dự án (Cho người mới tải về)

Để chạy dự án này trên máy tính khác một cách suôn sẻ, bạn chỉ cần thực hiện các bước sau:

### Bước 1: Mở dự án trong Visual Studio
1. Khởi động **Visual Studio** (Khuyên dùng bản VS 2017 trở lên).
2. Chọn **File** -> **Open** -> **Project/Solution...**
3. Tìm đến thư mục dự án vừa tải về và chọn file **`QuanLyRapPhim.csproj`** để mở dự án.

### Bước 2: Tự động khôi phục thư viện (NuGet Restore)
* Khi mở dự án lần đầu, Visual Studio sẽ tự động tải các gói thư viện cần thiết (như trình biên dịch C# `.DotNetCompilerPlatform`).
* Nếu dự án báo lỗi thư viện, bạn click chuột phải vào **Solution** trong bảng *Solution Explorer* -> chọn **Restore NuGet Packages**.

### Bước 3: Thiết lập trang khởi chạy
1. Trong bảng *Solution Explorer*, click chuột phải vào file **`Phim.aspx`**.
2. Chọn **Set As Start Page** (Thiết lập làm trang bắt đầu).

### Bước 4: Chạy dự án
* Nhấn nút **F5** hoặc bấm vào biểu tượng **IIS Express (Play)** trên thanh công cụ để khởi chạy dự án trên trình duyệt web.

---

## 📂 Cơ sở dữ liệu và Cấu hình tự động

* **Tự động cấu hình CSDL:** File CSDL đã được đặt sẵn trong thư mục **`App_Data/Database1.mdf`**.
* **Đường dẫn kết nối linh hoạt:** Lớp kết nối [`KetNoi.cs`](/QuanLyRapPhim/KetNoi.cs) sử dụng lệnh `Server.MapPath` động để tự động nhận diện CSDL trên mọi máy tính khác nhau mà không cần cấu hình lại chuỗi kết nối:
  ```csharp
  string dbPath = HttpContext.Current.Server.MapPath("~/App_Data/Database1.mdf");
  ```

---

## 📌 Các chức năng chính của dự án

1. **Trang chủ (`Phim.aspx`):** Hiển thị danh sách các phim đang chiếu, tự động phân loại phim theo danh mục thể loại ở cột bên trái.
2. **Chi tiết phim & Suất chiếu (`ChiTietPhim.aspx`):** Hiển thị chi tiết nội dung phim và liệt kê danh sách các suất chiếu tương ứng của phim đó.
3. **Sơ đồ chọn ghế (`ChonGhe.aspx`):** Sơ đồ ghế ngồi động của phòng chiếu, phân biệt ghế Thường/VIP, tự động khóa các ghế đã được mua trước đó.
4. **Thanh toán đặt vé (`ThanhToan.aspx`):** Điền thông tin đặt vé, lưu hóa đơn đặt vé sử dụng SQL Transaction an toàn.
5. **Đăng nhập & Đăng ký (`DangNhap.aspx`, `DangKy.aspx`):** Đăng ký tài khoản (có kiểm tra dữ liệu bằng Validation Controls) và đăng nhập hệ thống.
6. **Lịch sử đặt vé (`LichSuDatVe.aspx`):** Hiển thị lịch sử các vé đã mua của tài khoản đang đăng nhập.
7. **Trang Admin (`AdminPhim.aspx`):** Trang dành riêng cho quản trị viên thực hiện quản lý danh sách phim (Thêm, Sửa, Xóa phim).
