# 📑 TÀI LIỆU KẾT NỐI API (MICROSERVICES INTEGRATION GUIDE)
### Dự án: Payment & Report Service (Nhóm 9)
*   **Base URL (Online):** `https://btl-payment-api.onrender.com`
*   **Base URL (Local):** `http://localhost:5244`

Tài liệu này dùng để chia sẻ cho **Nhóm 7 (Course & Schedule Service)** và **Nhóm 8 (Student & Attendance Service)** để tích hợp các dịch vụ với nhau.

---

## 1. Dành Cho Nhóm 7 & Nhóm 8: Gọi Sang Nhóm 9 (Tạo & Kiểm Tra Hóa Đơn)

### 📌 API 1: Tạo Hóa Đơn Mới (Đăng ký học $\rightarrow$ Phát sinh hóa đơn)
Khi học viên chọn đăng ký một khóa học ở hệ thống Nhóm 7 hoặc Nhóm 8, nhóm của bạn sẽ gọi API này của Nhóm 9 để tạo yêu cầu đóng tiền.

*   **HTTP Method:** `POST`
*   **Endpoint:** `/api/v1/invoices`
*   **Header:** `Content-Type: application/json`
*   **Body Request (JSON):**
    ```json
    {
      "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", 
      "courseId": "CRS-001",
      "courseName": "Lớp học IELTS 7.0+",
      "amount": 5000000
    }
    ```
    *Trong đó:* `userId` là mã GUID của Học viên (bên Nhóm 8 cấp), `amount` là số tiền học phí.
*   **Response (JSON - HTTP 200 OK):**
    ```json
    {
      "success": true,
      "statusCode": 200,
      "message": "Tạo hóa đơn thành công",
      "data": {
        "invoiceId": "d5b8813f-c689-49ea-8002-47ef89522a6a",
        "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "studentName": "Học viên liên kết",
        "courseName": "Lớp học IELTS 7.0+",
        "courseId": "CRS-001",
        "amount": 5000000.0,
        "status": "Pending",
        "createdAt": "2026-05-26T04:26:42.27Z",
        "paidAt": null
      }
    }
    ```
    *Hãy lưu lại mã `invoiceId` nhận về để truy vấn trạng thái sau này.*

---

### 📌 API 2: Lấy Chi Tiết & Kiểm Tra Trạng Thái Hóa Đơn
Nhóm 7 và Nhóm 8 có thể kiểm tra xem học viên đã đóng tiền hay chưa bất cứ lúc nào bằng API này.

*   **HTTP Method:** `GET`
*   **Endpoint:** `/api/v1/invoices/{invoiceId}`
    *(Ví dụ: `/api/v1/invoices/d5b8813f-c689-49ea-8002-47ef89522a6a`)*
*   **Response (JSON - HTTP 200 OK):**
    ```json
    {
      "success": true,
      "statusCode": 200,
      "data": {
        "invoiceId": "d5b8813f-c689-49ea-8002-47ef89522a6a",
        "status": "Paid", 
        "paidAt": "2026-05-26T04:30:15.00Z"
      }
    }
    ```
    *Các trạng thái trả về của trường `status`:*
    *   `Pending`: Chờ thanh toán.
    *   `Paid`: Đã thanh toán thành công.
    *   `Overdue`: Hóa đơn đã quá hạn đóng tiền.

---

## 2. Nhóm 9 Gọi Sang Nhóm 7 & 8 (Gửi Thông Báo Thanh Toán Thành Công)

Sau khi học viên thực hiện thanh toán hóa đơn thành công ở trang web của Nhóm 9, Nhóm 9 sẽ chủ động gửi thông báo (Webhook) tới API của nhóm bạn để xác nhận.

*   **HTTP Method:** `POST`
*   **Webhook URL:** *(Nhóm 7 / Nhóm 8 cần cung cấp đường dẫn này, ví dụ: `https://nhom7-course-api.onrender.com/api/v1/enrollments/confirm-payment`)*
*   **Body Webhook gửi đi (JSON):**
    ```json
    {
      "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "courseId": "CRS-001",
      "invoiceId": "d5b8813f-c689-49ea-8002-47ef89522a6a",
      "status": "success",
      "amount": 5000000,
      "paidAt": "2026-05-26T04:30:15.00Z"
    }
    ```
    *Khi nhận được request này, nhóm của bạn hãy thực hiện chuyển trạng thái của học viên thành "Đã vào lớp" (Active).*
