# 💳 PaymentService - Hệ Thống Quản Lý Thanh Toán & Hóa Đơn Khóa Học

## 👥 Nhóm 9 - Bài tập lớn Fullstack Vue.js & ASP.NET Core
Dự án được xây dựng phục vụ cho việc quản lý hóa đơn, học phí và các khóa học của học viên tại trung tâm đào tạo.

###  član viên nhóm:
1. **Nguyễn Đình Minh Hiếu**
2. **Nguyễn Công Hiệp**
3. **Bùi Thế Đạt**

---

## 🛠️ Công Nghệ Sử Dụng (Technology Stack)

### 1. Backend: C# ASP.NET Core Web API
*   **Framework:** .NET 8.0
*   **Database Provider:** Microsoft SQL Server (MS SQL)
*   **ORM:** Entity Framework Core (EF Core) 8.0
*   **Authentication:** JWT Bearer Token (Xác thực người dùng)
*   **Documentation:** Swagger / OpenAPI (Kiểm thử API)

### 2. Frontend: Vue.js 3
*   **Build Tool:** Vite (TypeScript)
*   **State Management:** Pinia (Quản lý trạng thái đăng nhập, giỏ hàng)
*   **HTTP Client:** Axios (Gọi API lên Backend)
*   **Router:** Vue Router (Điều hướng trang)
*   **Styling:** Vanilla CSS (Glassmorphism UI hiện đại)

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy Dự Án (Local Development)

### 1. Cấu hình Database (SQL Server)
*   Dự án sử dụng **Windows Authentication** để kết nối SQL Server local.
*   Cấu hình Connection String nằm tại file: `src/PaymentService.API/appsettings.json`
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=DESKTOP-NQDPTKL\\SQLSEVER;Database=PaymentServiceDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
    }
    ```
*   *Lưu ý:* Khi khởi chạy Backend lần đầu tiên, hệ thống sẽ tự động khởi tạo cơ sở dữ liệu `PaymentServiceDb`, tạo các bảng và nạp dữ liệu mẫu (Seed Data) tự động mà không cần chạy migration thủ công.

### 2. Chạy Backend (C# API)
1.  Di chuyển vào thư mục backend:
    ```bash
    cd src/PaymentService.API
    ```
2.  Chạy ứng dụng:
    ```bash
    dotnet run
    ```
3.  Truy cập Swagger để kiểm thử API tại: [http://localhost:5244/swagger](http://localhost:5244/swagger)

### 3. Chạy Frontend (Vue.js)
1.  Di chuyển vào thư mục frontend:
    ```bash
    cd src/PaymentService.Web
    ```
2.  Cài đặt các gói phụ thuộc (Dependencies):
    ```bash
    npm install
    ```
3.  Chạy ứng dụng chế độ phát triển:
    ```bash
    npm run dev
    ```
4.  Mở trình duyệt truy cập: [http://localhost:5175/](http://localhost:5175/)

---

## 🔑 Tài Khoản Thử Nghiệm (Demo Accounts)

Hệ thống đã nạp sẵn 3 tài khoản mẫu để phục vụ kiểm thử các vai trò khác nhau:

| Vai trò | Email đăng nhập | Mật khẩu | Chức năng kiểm thử |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@trungtam.com` | `Admin@123` | Xem tất cả hóa đơn của trung tâm, xem báo cáo doanh thu doanh số |
| **Student (Học viên 1)** | `hieu@student.com` | `Hieu@123` | Xem danh sách hóa đơn học phí của cá nhân, thực hiện thanh toán hóa đơn |
| **Student (Học viên 2)** | `hiep@student.com` | `Hiep@123` | Xem danh sách hóa đơn học phí cá nhân |

---

## 📂 Cấu Trúc Thư Mục Dự Án

```txt
BTL/
├── PaymentService.sln                # File Solution của Visual Studio
└── src/
    ├── PaymentService.API/           # Dự án Backend ASP.NET Core API
    │   ├── Controllers/             # Các API Endpoint (Auth, Invoices, Reports)
    │   ├── Data/                    # DbContext kết nối SQL Server & Seed Data
    │   ├── Entities/                # Các bảng dữ liệu (User, Invoice)
    │   └── Program.cs               # Cấu hình dịch vụ & khởi chạy API
    └── PaymentService.Web/           # Dự án Frontend Vue 3 + TypeScript
        ├── src/
        │   ├── services/            # Axios API Services gọi lên Backend
        │   ├── stores/              # Pinia Stores (Quản lý Auth State)
        │   └── views/               # Các trang giao diện (Login, Invoices, Detail)
        └── vite.config.ts           # Cấu hình Vite & API Proxy (cổng 5244)
```
