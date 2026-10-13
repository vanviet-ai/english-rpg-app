# SPEC.md: App luyện thi tiếng Anh gamified (đồ án Lập trình di động)

# 1. Tổng quan

- **Sản phẩm:** App Android luyện thi tiếng Anh, chơi như RPG nhẹ.
- **Người dùng chính:** Học sinh lớp 12 (sinh viên nằm ngoài phạm vi).
- **Ý tưởng cốt lõi:** Chọn trường mục tiêu → app đổi thành mức EXP cần đạt → vượt "ải" để nhận EXP → đủ EXP thì "mở khóa" trường.
- **Nội dung:** Câu hỏi trắc nghiệm lấy từ đề thi THPT các năm trước, soạn sẵn trong database.
- **AI Mentor:** Gợi ý (Hint 1, Hint 2) soạn sẵn, không gọi AI thật khi chạy.
- **Phạm vi:** Đồ án môn học, demo được trong khoảng 5 tuần, nhóm 2 người.
- **Ngoài phạm vi (đã cắt):** bảng xếp hạng, phần Ôn luyện (chủ đề ngữ pháp), đăng ký email/mật khẩu, đăng nhập Google, nhóm người dùng sinh viên, AI sinh câu hỏi/hint theo thời gian thực.

**Quy tắc game đã chốt**

| Mục | Giá trị |
|---|---|
| Ải thường | 3 ải, mỗi ải 10 câu |
| Qua ải | Đúng ≥ 8/10 thì được tính là qua ải (mở ải kế). Không đạt (< 8 đúng) **vẫn nhận EXP theo công thức** |
| EXP mỗi câu đúng | Ải 1 = 200, ải 2 = 300, ải 3 = 400 |
| Sai 1 câu | 0 EXP (không trừ) |
| Hint | Hint 1 free (tự hiện khi trả lời sai, không trừ EXP). Hint 2 (bấm "Xem tiếp") **-60 EXP**, trừ vào tổng EXP của ải. EXP của một ải **chặn tối thiểu ở 0** |
| Trả lời | Sai lần đầu là sai luôn (hint chỉ để học) |
| Boss | 15 câu **random mỗi lần chơi** (lấy ngẫu nhiên từ ngân hàng khoảng 60 câu hỏi Boss), 30 phút tính tổng cả ải, thắng khi đúng ≥ 13 (hết giờ hoặc đúng < 13 là thua). **Thắng lần đầu nhận 2000 EXP**; chơi lại không tính điểm (chỉ ôn tập). Không có EXP theo từng câu, không có Mentor/hint. **Không hiện đúng/sai từng câu khi đang chơi; đáp án công bố ở màn kết quả** |
| Mở ải | **Tuần tự**: qua ải N mới mở ải N+1 |
| Mở Boss | Hoàn thành 3 ải thường (không xét EXP) |
| Chơi lại | Ải thường: có EXP. Boss: không tính điểm (chỉ ôn tập) |
| Thoát giữa ải | Chỉ áp dụng ải thường: giữ tiến độ, 15 phút tính từ lần tương tác cuối (trả lời hoặc xem hint). Quá hạn thì mất. Boss: thoát giữa chừng thì chơi lại từ đầu |
| Kết thúc ải (finish) | Ải thường: phải trả lời đủ 10 câu mới kết thúc được. Boss: kết thúc lúc nào cũng được (câu chưa trả lời tính là sai) |
| Mất mạng | Hiện thông báo giữa màn hình |

**Danh sách trường và EXP mục tiêu**

| Trường | EXP |
|---|---|
| ĐH Quốc gia Hà Nội (VNU) | 10000 |
| ĐH Quốc gia TP.HCM (VNU-HCM) | 10000 |
| ĐH Bách khoa Hà Nội (HUST) | 9000 |
| ĐH Bách khoa, ĐHQG TP.HCM (HCMUT) | 8000 |
| ĐH Y Hà Nội (HMU) | 7500 |
| ĐH Y Dược TP.HCM (UMP) | 7000 |
| ĐH Ngoại thương (FTU) | 5000 |
| ĐH Kinh tế TP.HCM | 4500 |

# 2. Tính năng

**Module A: Người chơi**
- [Must] Nhập tên để vào app, tiến độ lưu theo tên.
- [Must] Chọn trường mục tiêu từ danh sách 8 trường; **mục tiêu là cố định**, không đổi sau khi chọn.
- [Must] Hiển thị tổng EXP và tiến độ tới EXP mục tiêu.
- [Must] Khi đủ EXP, trường mục tiêu chuyển sang trạng thái "đã mở khóa".

**Module B: Ải thường**
- [Must] 3 ải, mỗi ải 10 câu trắc nghiệm, độ khó tăng dần theo ải.
- [Must] Qua ải khi đúng ≥ 8/10 (mở ải kế).
- [Must] Không đạt (< 8 đúng) vẫn nhận EXP theo công thức (đúng +EXP theo ải, Hint 2 -60, chặn tối thiểu 0); chỉ không được mở ải kế.
- [Must] Phải trả lời đủ 10 câu mới kết thúc được ải.
- [Must] EXP mỗi câu đúng theo ải: ải 1 = 200, ải 2 = 300, ải 3 = 400.
- [Must] Mỗi câu chỉ trả lời một lần; sai là sai luôn.
- [Must] Mở ải tuần tự: qua ải N mới mở ải N+1.
- [Must] Chơi lại ải đã qua vẫn nhận EXP.
- [Should] Thoát giữa chừng: giữ tiến độ, 15 phút tính từ lần tương tác cuối (trả lời hoặc xem hint), quá hạn thì mất.

**Module C: Hint và tổng kết**
- [Must] Hint 1 (gợi ý hướng suy nghĩ) và Hint 2 (gần như lộ đáp án), soạn sẵn.
- [Must] Hint 1 free: tự hiện khi trả lời sai ở ải thường, không trừ EXP.
- [Must] Hint 2: chỉ hiện khi người chơi bấm "Xem tiếp", trừ 60 EXP vào tổng EXP của ải.
- [Must] EXP của một ải không xuống dưới 0.
- [Must] Màn tổng kết cuối ải: số đúng/sai, EXP nhận được (kể cả khi chưa đạt), danh sách câu sai kèm hint để xem lại.

**Module D: Boss**
- [Must] Mở sau khi hoàn thành 3 ải thường.
- [Must] Mỗi lần chơi, server chọn ngẫu nhiên 15 câu từ ngân hàng câu hỏi Boss (khoảng 60 câu).
- [Must] Đồng hồ đếm ngược 30 phút cho cả ải, không có Mentor/hint.
- [Must] Không hiện đúng/sai từng câu khi đang chơi; đáp án đúng chỉ công bố ở màn kết quả sau khi kết thúc.
- [Must] Kết thúc (finish) lúc nào cũng được; câu chưa trả lời tính là sai.
- [Must] Thoát giữa chừng thì chơi lại từ đầu (không giữ tiến độ).
- [Must] Thắng khi đúng ≥ 13/15; thắng lần đầu nhận thưởng cố định 2000 EXP. Không có EXP theo từng câu.
- [Must] Hết giờ hoặc đúng < 13 là thua; thua có thể chơi lại.
- [Must] Chơi lại Boss không tính điểm (chỉ ôn tập).

**Module E: EXP và trường mục tiêu**
- [Must] Server tính EXP theo công thức; client chỉ hiển thị.
- [Must] Bảng quy đổi trường → EXP lưu trong database (8 trường).

**Module F: Hệ thống**
- [Should] Thông báo giữa màn hình khi mất mạng/không gọi được server.

**Nice (chỉ chỉnh bóng, không phải tính năng mới)**
- [Nice] Hiệu ứng nhẹ khi qua ải, thắng Boss, mở khóa trường (bạn phụ trách UI làm nếu còn thời gian).

# 3. Kiến trúc kỹ thuật

- **Frontend:** Flutter + Dart, build Android; test nhanh trên Flutter web (máy yếu).
- **Backend:** ASP.NET Core Web API (REST, JSON).
- **Database:** SQL Server, truy cập qua Entity Framework Core.
- **Luồng:** Flutter → HTTP → Web API → SQL Server.
- **Nguyên tắc:**
  - Server chấm đúng/sai và tính EXP; client không nhận đáp án đúng trước khi trả lời.
  - Server giữ giờ bắt đầu và giờ hết hạn của lượt chơi (Boss 30 phút; ải thường 15 phút giữ tiến độ), không tin giờ của client.
  - Hint chỉ được gửi kèm kết quả khi trả lời sai ở ải thường (Boss không bao giờ gửi hint), để server trừ EXP đúng.
  - Boss: server không trả đúng/sai từng câu khi đang chơi; đáp án chỉ trả về sau khi `finish`.
  - Dữ liệu câu hỏi, hint, trường nạp bằng script seed.
- **Cấu trúc code (để chia việc 2 người):**
  - Flutter: tách riêng thư mục giao diện (screens/widgets) khỏi logic và gọi API (services/models). Bạn UI chỉ sửa phần giao diện.
  - Backend: Controllers / Services / Models / Data.
- **CORS:** bật cho origin của Flutter web khi test.
- **Chạy lúc chấm:** backend chạy localhost trên máy của bạn (đã chốt).

# 4. Database schema (dạng text)

```
Universities
  Id (PK), Name, ShortName, ExpRequired

Players
  Id (PK), Name (unique), TargetUniversityId (FK -> Universities, nullable),
  TotalExp, CreatedAt

Stages
  Id (PK), OrderNo, Name, Type ('Normal' | 'Boss'),
  QuestionCount, PassCorrect, TimeLimitSeconds (null với ải thường),
  ExpPerCorrect (200 / 300 / 400 với ải thường, 0 với Boss),
  BonusExp (0 với ải thường, 2000 với Boss)

Questions
  Id (PK), StageId (FK -> Stages), OrderNo, Content,
  OptionA, OptionB, OptionC, OptionD, CorrectOption ('A'..'D'),
  Hint1, Hint2, Explanation (nullable; Boss có thể để trống hint)

PlayerStageProgress
  PlayerId (FK), StageId (FK)  -- PK ghép
  IsCompleted, FirstClearedAt, BestCorrectCount, BossRewardClaimed

StageAttempts
  Id (PK), PlayerId (FK), StageId (FK),
  StartedAt, ExpiresAt, FinishedAt (nullable),
  Status ('InProgress' | 'Passed' | 'Failed' | 'Expired'),
  CorrectCount, ExpEarned
  -- Ải thường: ExpiresAt được server cập nhật lại mỗi khi user tương tác
  --   (trả lời câu hoặc gọi hint), không set cố định 1 lần.
  -- Boss: ExpiresAt cố định = StartedAt + 30 phút, không gia hạn.

AttemptQuestions
  AttemptId (FK), QuestionId (FK), OrderNo  -- PK ghép; các câu của một lượt chơi

AttemptAnswers
  Id (PK), AttemptId (FK), QuestionId (FK),
  SelectedOption (nullable), IsCorrect,
  ShownHint1, UsedNextHint, ExpDelta
```

**Ghi chú:**
- `ExpRequired` của trường là con số đặt tay trong bảng Universities.
- Câu hỏi Boss nằm trong bảng `Questions` với `StageId` = ải Boss, đóng vai trò ngân hàng câu hỏi. Mỗi lần `start` Boss, server chọn ngẫu nhiên 15 câu và ghi vào `AttemptQuestions`. Ải thường: ghi cả 10 câu theo thứ tự cố định.
- Server chỉ nhận câu trả lời cho câu có trong `AttemptQuestions` của lượt chơi đó.
- `ExpDelta` mỗi câu: +`ExpPerCorrect` của ải nếu đúng (ải thường), -60 nếu bấm "Xem tiếp".
- `Status` khi `finish` ải thường: `Passed` nếu đúng ≥ 8, ngược lại `Failed`; **cả hai đều được cộng EXP**.
- `ExpEarned` của ải thường = tổng `ExpDelta`, chặn tối thiểu 0 (tính cả khi `Failed`). Lượt `Expired` hoặc bị bỏ do `restart` không cộng EXP. Boss: `ExpEarned` = `BonusExp` (2000) nếu thắng lần ĐẦU TIÊN (`BossRewardClaimed` = false), ngược lại 0.
- Lượt chơi ải thường dở quá 15 phút kể từ lần tương tác cuối (`ExpiresAt` đã qua) bị coi là `Expired`. Boss: lượt dở không được giữ; mỗi lần vào Boss tạo lượt mới từ đầu (lượt cũ coi là `Expired`).

# 5. API endpoints

**Người chơi**
- `POST /api/players/login`: body `{ name }`; tạo mới nếu chưa có, trả thông tin người chơi.
- `GET /api/players/{id}`: thông tin, EXP, trường mục tiêu, trạng thái mở khóa.
- `PUT /api/players/{id}/target`: body `{ universityId }`.
- `GET /api/universities`: danh sách trường và EXP mục tiêu.

**Ải và lượt chơi**
- `GET /api/stages?playerId=`: danh sách ải kèm trạng thái (khóa / mở / đã qua) và `hasInProgressAttempt` (chỉ có ở ải thường: true nếu người chơi đang có lượt dở còn hạn; client dùng để hiện hộp thoại "tiếp tục / chơi lại").
- `POST /api/stages/{stageId}/start`: body `{ playerId, restart }` (`restart` mặc định false). Ải thường: nếu có lượt dở còn hạn và `restart` = false thì trả lượt dở; `restart` = true thì lượt cũ chuyển `Expired` (không cộng EXP) và tạo lượt mới. Boss: `restart` không có tác dụng, luôn tạo lượt mới từ đầu và chọn ngẫu nhiên 15 câu từ ngân hàng Boss. Từ chối nếu ải chưa mở. Trả danh sách câu hỏi của lượt đó (không có đáp án đúng, không có hint) và `expiresAt`.
- `GET /api/attempts/{attemptId}`: lấy lại trạng thái để chơi tiếp sau khi thoát (chỉ ải thường).
- `POST /api/attempts/{attemptId}/answer`: body `{ questionId, selectedOption }`; ải thường: trả `isCorrect`, nếu sai thì trả kèm Hint 1 (free, không trừ EXP); Boss: chỉ xác nhận đã ghi nhận câu trả lời, không trả `isCorrect`, không trả hint.
- `POST /api/attempts/{attemptId}/hint/next`: body `{ questionId }`; chỉ gọi khi người chơi bấm "Xem tiếp"; trả Hint 2 và trừ 60 EXP vào tổng EXP của ải; từ chối nếu là Boss hoặc câu không sai.
- `POST /api/attempts/{attemptId}/finish`: kết thúc, trả kết quả (qua/thua, số đúng, EXP nhận, tổng EXP mới). Ải thường: từ chối nếu chưa trả lời đủ 10 câu. Boss: gọi lúc nào cũng được, câu chưa trả lời tính là sai.
- `GET /api/attempts/{attemptId}/summary`: chỉ gọi được sau `finish`. Ải thường: các câu sai + hint + giải thích. Boss: đáp án đúng của cả 15 câu, câu người chơi đã chọn và giải thích (nếu có).

# 6. UI/UX Direction

**Phong cách:** RPG nhẹ, tươi, dễ đọc; không rối mắt vì học sinh dùng để học.

**Palette (đề xuất)**
- Nền chính: `#1B1F3B` (xanh navy đậm)
- Nhấn chính: `#6C4AB6` (tím)
- EXP / phần thưởng: `#F5B83D` (vàng)
- Đúng / qua ải: `#3FB27F` (xanh lá)
- Sai / hết giờ / Boss: `#E5484D` (đỏ)
- Chữ trên nền tối: `#F4F1FF`; thẻ/khung: `#2A2F57`

**Typography (đề xuất)**
- Font: **Be Vietnam Pro** (hỗ trợ tiếng Việt tốt), dùng gói `google_fonts`.
- Tiêu đề: đậm 700; câu hỏi: 600, cỡ lớn; nội dung/hint: 400.

**Danh sách màn hình**
1. Nhập tên (vào app)
2. Chọn trường mục tiêu
3. Trang chủ: EXP hiện tại, thanh tiến độ tới EXP mục tiêu, trạng thái mở khóa trường
4. Bản đồ ải: 3 ải thường + Boss (khóa/mở/đã qua)
5. Màn chơi câu hỏi (ải thường): câu hỏi, 4 đáp án
6. Khung AI Mentor: tự hiện Hint 1 khi trả lời sai, nút "Xem tiếp" để ra Hint 2 (-60 EXP)
7. Tổng kết cuối ải: đúng/sai, EXP nhận (kể cả khi chưa đạt), xem lại câu sai + hint
8. Giới thiệu Boss (luật chơi, 30 phút)
9. Màn chơi Boss: đồng hồ đếm ngược, không có Mentor, không hiện đúng/sai từng câu
10. Kết quả Boss (thắng/thua) kèm đáp án đúng của từng câu
11. Hộp thoại: tiếp tục ải dở / chơi lại (hiện khi `hasInProgressAttempt` = true; chọn "chơi lại" thì gọi `start` với `restart` = true)
12. Hộp thoại: mất mạng (hiện giữa màn hình)
13. Hộp thoại: mở khóa trường mục tiêu

**Phân việc UI:** bạn làm đủ luồng chạy được với giao diện tối thiểu; bạn kia tối ưu giao diện trên GitHub. Hai bạn nên vẽ phác 13 màn hình này (giấy hoặc Figma) ở tuần 1.

# 7. Roadmap (khoảng 5 tuần)

**Milestone 1 (tuần 1): Nền móng**
- Chốt luật game, vẽ phác màn hình cùng bạn kia.
- Tạo database, seed 8 trường + 3 ải + Boss + một ít câu hỏi mẫu.
- Dựng API khung, nhập tên, chạy thông Flutter ↔ API (xử lý CORS).
- Chọn trường mục tiêu (màn chọn trường + lưu mục tiêu): làm sớm.

**Milestone 2 (tuần 2): Lõi ải thường**
- Start ải, trả lời, chấm đúng/sai, tính EXP theo ải (200 / 300 / 400), hoàn thành ải, mở ải kế.
- Hint 1 free tự hiện khi sai, nút "Xem tiếp" (trừ 60 EXP), EXP ải chặn ở 0.
- Build thử bản Android thật (không chờ tới cuối).

**Milestone 3 (tuần 3): Boss và tổng kết, bàn giao UI**
- Màn tổng kết cuối ải.
- Boss: đồng hồ, thắng/thua, màn kết quả công bố đáp án, thưởng 2000 EXP một lần (chơi lại không tính điểm).
- Ải thường: thoát giữa ải giữ tiến độ 15 phút; thông báo mất mạng.
- **Đẩy code lên GitHub, bạn kia bắt đầu tối ưu UI.**

**Milestone 4 (tuần 4): Trường mục tiêu và nội dung**
- Thanh tiến độ, mở khóa trường.
- Hoàn tất nhập câu hỏi: 30 câu ải thường (kèm Hint 1, Hint 2) + khoảng 60 câu cho ngân hàng Boss (không hint); tổng khoảng 90 câu, nhờ người giỏi tiếng Anh rà đáp án.

**Milestone 5 (tuần 5): Hoàn thiện**
- Test toàn bộ luồng, sửa lỗi, ghép UI, chuẩn bị demo và báo cáo.
- Chừa buffer; không thêm tính năng mới.

**Quy tắc:** không thêm tính năng ngoài spec; thời gian dư dùng để test, sửa lỗi và chỉnh UI.

# 8. Rủi ro và câu hỏi mở

**Rủi ro**
- **Số lượng nội dung:** khoảng 90 câu: 30 câu ải thường (đề, đáp án, 2 hint) + khoảng 60 câu Boss (đề, đáp án; Boss không có hint). Đây là phần tốn công nhất của đồ án, nên bắt đầu soạn sớm và nhập theo đợt, không dồn vào tuần 4; AI soạn nháp có thể sai đáp án, cần người rà.
- **EXP và mục tiêu:** tối đa một lượt hoàn hảo (10 câu đúng mỗi ải, không xem Hint 2) là 10 × 200 + 10 × 300 + 10 × 400 = 9000, cộng thưởng Boss 2000 = 11000 EXP.
- **Web vs Android:** code chạy trên Flutter web chưa chắc chạy trên Android; build sớm ở tuần 2.
- **Phối hợp nhóm:** UI bị dồn cuối nếu bàn giao trễ.
- **Vibe code:** bạn không đọc được code, nên kiểm tra bằng chạy thử sau mỗi tính năng và commit nhỏ, thường xuyên.
- **Bản quyền đề thi:** chỉ dùng cho đồ án học thuật; nếu phát triển thật thì phải kiểm tra lại.

**Đã chốt:** 3 ải thường (EXP mỗi câu đúng: 200 / 300 / 400); Hint 1 free tự hiện khi sai, "Xem tiếp" ra Hint 2 và trừ 60 EXP vào tổng EXP của ải; EXP ải chặn ở 0; Boss mở khi hoàn thành 3 ải thường (không xét EXP), 15 câu random mỗi lần chơi từ ngân hàng khoảng 60 câu, thắng lần đầu thưởng 2000 EXP, chơi lại không tính điểm (chỉ ôn tập), thoát giữa Boss thì chơi lại từ đầu, thua Boss không bị khóa, Boss không hiện đúng/sai từng câu (đáp án công bố ở màn kết quả), finish Boss gọi lúc nào cũng được; ải thường không đạt (< 8 đúng) vẫn nhận EXP theo công thức, finish ải thường yêu cầu đủ 10 câu; `GET /api/stages` có `hasInProgressAttempt`, `POST start` có `restart`; mở ải tuần tự; khi tổng EXP đạt mức của trường đã chọn thì chỉ hiện trạng thái "đã mở khóa" kèm hộp thoại chúc mừng; mục tiêu trường cố định; bỏ phần Ôn luyện; backend chạy localhost.

**Câu hỏi mở**
- Hiện chưa còn câu hỏi mở. Nếu phát sinh khi code (ví dụ cách tính giờ Boss khi mất mạng), ghi thêm vào đây.