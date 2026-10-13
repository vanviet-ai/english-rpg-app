# English RPG Flutter

## Khởi tạo project

Chạy từ thư mục gốc `english-rpg-app`:

```powershell
flutter create --platforms web --project-name english_rpg_app flutter_app
cd flutter_app
flutter pub add http
```

## Chạy ứng dụng

Khởi động backend tại `http://localhost:5137`, sau đó chạy Flutter Web:

```powershell
cd D:\DEV\english-rpg-app\flutter_app
flutter run -d chrome --web-hostname localhost
```

Ứng dụng gửi yêu cầu đăng nhập đến
`http://localhost:5137/api/players/login` với JSON `{ "name": "..." }`.
Backend cần cho phép CORS từ origin localhost của Flutter Web.

Từ Trang chủ, chọn **Vào bản đồ ải** để xem trạng thái các ải. Các ải thường
mở khóa có thể chơi câu hỏi; màn chơi dùng các API `stages`, `attempts/answer`,
`attempts/hint/next` và `attempts/finish` tại cùng địa chỉ backend. Boss mở khóa
có màn hình luật chơi, đồng hồ đồng bộ theo thời hạn server, màn chơi không hiển
thị đáp án và màn kết quả gọi API summary để ôn tập.

## Cấu trúc mã nguồn

- `lib/models`: các model dữ liệu.
- `lib/services`: gọi API.
- `lib/screens`: màn hình đăng nhập, chọn trường, bản đồ ải, chơi ải thường/Boss
  và các màn tổng kết.
- `lib/widgets`: widget giao diện dùng lại.
