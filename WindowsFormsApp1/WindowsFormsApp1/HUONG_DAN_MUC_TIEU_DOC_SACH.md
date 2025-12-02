# HÝ?NG D?N S? D?NG CH?C NÃNG M?C TIÊU Ð?C SÁCH

## ?? T?ng quan

H? th?ng m?c tiêu ð?c sách giúp b?n:
- ? Ð?t m?c tiêu ð?c hàng ngày (tùy ch?nh 10-300 phút/ngày)
- ? Ð?t m?c tiêu ð?c hàng tháng (tùy ch?nh 1-30 cu?n/tháng)
- ? Ð?t m?c tiêu ð?c hàng nãm (tùy ch?nh 1-100 cu?n/nãm)
- ? Nh?c nh? ð?c sách t? ð?ng (notifications)
- ? Streak counter (ð?m chu?i ngày ð?c liên t?c)
- ? Th?ng kê chi ti?t v? th?i gian ð?c

---

## ?? CÁC BÝ?C CÀI Ð?T

### Bý?c 1: Ch?y Script SQL t?o b?ng

1. M? SQL Server Management Studio
2. K?t n?i t?i database c?a b?n
3. M? file `WindowsFormsApp1\Database\CreateReadingGoalTables.sql`
4. Th?c thi script ð? t?o các b?ng:
   - `MucTieuDocSach` (Reading Goals)
   - `PhienDocSach` (Reading Sessions)
   - `ChuoiNgayDocSach` (Reading Streaks)
   - `LichSuNhacNho` (Notifications)

### Bý?c 2: Build và ch?y ?ng d?ng

```bash
# Build project
dotnet build

# Ho?c nh?n F5 trong Visual Studio
```

---

## ?? CÁCH S? D?NG

### 1. Xem Th?ng Kê và Ð?t M?c Tiêu

1. Ðãng nh?p vào ?ng d?ng
2. Nh?n vào m?c **"?? M?c tiêu"** trên sidebar bên trái
3. Form th?ng kê s? hi?n th?:
   - ?? **Chu?i ngày ð?c liên t?c** (Current Streak)
   - ?? **Ti?n ð? m?c tiêu hôm nay** (Daily Goal)
   - ?? **S? cu?n ð? ð?c trong tháng** (Monthly Goal)
   - ?? **S? cu?n ð? ð?c trong nãm** (Yearly Goal)
   - ?? **Bi?u ð? 7 ngày g?n nh?t**

### 2. Ð?t M?c Tiêu Ð?c Hàng Ngày (Tùy Ch?nh)

1. Trong form "M?c tiêu", nh?n nút **"? Ð?t m?c tiêu"** trong ph?n **"M?C TIÊU HÔM NAY"**
2. Ch?n s? phút mu?n ð?c m?i ngày:
   - **T?i thi?u**: 10 phút
   - **T?i ða**: 300 phút (5 gi?)
   - **G?i ?**: 30-60 phút
3. Nh?n **"Ð?t m?c tiêu"**

**Ví d?:**
- Ngý?i m?i b?t ð?u: 15-20 phút/ngày
- Ngý?i ð?c trung b?nh: 30-45 phút/ngày
- Ngý?i ð?c nhi?u: 60-120 phút/ngày

### 3. Ð?t M?c Tiêu Ð?c Hàng Tháng (Tùy Ch?nh)

1. Trong form "M?c tiêu", nh?n nút **"? Ð?t m?c tiêu"** trong ph?n **"M?C TIÊU THÁNG NÀY"**
2. Ch?n s? cu?n sách mu?n ð?c m?i tháng:
   - **T?i thi?u**: 1 cu?n
   - **T?i ða**: 30 cu?n
   - **G?i ?**: 2-4 cu?n
3. Nh?n **"Ð?t m?c tiêu"**

**Ví d?:**
- Ngý?i b?n r?n: 1-2 cu?n/tháng
- Ngý?i có th?i gian: 3-5 cu?n/tháng
- Ð?c gi? chuyên nghi?p: 8-10 cu?n/tháng

### 4. Ð?t M?c Tiêu Ð?c Trong Nãm (Tùy Ch?nh)

1. Trong form "M?c tiêu", nh?n nút **"? Ð?t m?c tiêu"** trong ph?n **"M?C TIÊU NÃM NAY"**
2. Ch?n s? cu?n sách mu?n ð?c trong nãm:
   - **T?i thi?u**: 1 cu?n
   - **T?i ða**: 100 cu?n
   - **G?i ?**: 12-24 cu?n (1-2 cu?n/tháng)
3. Nh?n **"Ð?t m?c tiêu"**

**Ví d?:**
- M?c tiêu cõ b?n: 12 cu?n/nãm (1 cu?n/tháng)
- M?c tiêu trung b?nh: 24 cu?n/nãm (2 cu?n/tháng)
- M?c tiêu cao: 52 cu?n/nãm (1 cu?n/tu?n)

### 5. Tracking T? Ð?ng Khi Ð?c Sách

- **T? ð?ng b?t ð?u**: Khi b?n m? m?t cu?n sách, h? th?ng t? ð?ng b?t ð?u ghi nh?n th?i gian ð?c
- **T? ð?ng k?t thúc**: Khi b?n ðóng sách, h? th?ng t? ð?ng:
  - Lýu s? phút ð? ð?c
  - C?p nh?t streak (n?u ð?c liên t?c nhi?u ngày)
  - Ki?m tra và g?i thông báo n?u hoàn thành m?c tiêu

### 6. Nh?n Nh?c Nh? Ð?c Sách

- **T? ð?ng kh?i ð?ng**: Service nh?c nh? t? ð?ng ch?y khi b?n ðãng nh?p
- **Th?i gian nh?c**: Ch? g?i nh?c nh? t? 8h-22h
- **Ði?u ki?n**: Ch? nh?c khi b?n chýa ð?c sách trong ngày
- **T?n su?t**: Ki?m tra m?i 1 gi?

**Các tin nh?n nh?c nh? ng?u nhiên:**
- ?? Ð?ng quên ð?c sách hôm nay! Ch? c?n 30 phút thôi.
- ? Ð? ð?n gi? ð?c sách r?i ð?y! H?y duy tr? streak c?a b?n.
- ?? Streak c?a b?n ðang ch? ð?y! H?y ð?c sách ngay hôm nay.
- ...và nhi?u tin nh?n khác!

### 7. Xem Bi?u Ð? Th?ng Kê

- **Bi?u ð? c?t**: Hi?n th? s? phút ð?c trong 7 ngày g?n nh?t
- **Màu xanh lá**: Ngày ð?t m?c tiêu (>= m?c tiêu ð? ð?t)
- **Màu xám**: Ngày chýa ð?t m?c tiêu
- **Tooltip**: Hi?n th? s? phút ð?c khi hover vào c?t

---

## ?? M?O & G?I ?

### Ð?t M?c Tiêu H?p L?

1. **B?t ð?u nh?**: N?u m?i b?t ð?u, h?y ð?t m?c tiêu th?p (15 phút/ngày, 1 cu?n/tháng)
2. **Tãng d?n**: Sau 1-2 tu?n thành công, tãng m?c tiêu lên
3. **Th?c t?**: D?a vào l?ch tr?nh c?a b?n ð? ð?t m?c tiêu phù h?p
4. **Linh ho?t**: Có th? thay ð?i m?c tiêu b?t c? lúc nào

### Các M?c Tiêu Ph? Bi?n

**M?c tiêu hàng ngày:**
- Ngý?i m?i: 15-20 phút
- Trung b?nh: 30-45 phút
- Nhi?u: 60-90 phút

**M?c tiêu hàng tháng:**
- Cõ b?n: 1-2 cu?n
- Trung b?nh: 3-4 cu?n
- Cao: 5-8 cu?n

**M?c tiêu hàng nãm:**
- Cõ b?n: 12 cu?n (1/tháng)
- Trung b?nh: 24 cu?n (2/tháng)
- Cao: 52 cu?n (1/tu?n)
- R?t cao: 100+ cu?n

### Duy Tr? Streak

1. **Ð?c m?i ngày**: Dù ch? 5-10 phút c?ng ðý?c tính
2. **Ð?t l?ch c? ð?nh**: VD: Ð?c m?i t?i trý?c khi ng?
3. **Ð?c trên di ð?ng**: Khi ði tàu, xe bus
4. **K?t h?p audiobook**: Nghe sách khi t?p th? d?c, n?u ãn

---

## ?? CÁCH TH?C HO?T Ð?NG

### Reading Session Tracking

```
M? sách ? StartReadingSession()
         ?
    [Ð?c sách...]
         ?
Ðóng sách ? EndReadingSession()
         ?
    Lýu vào DB:
    - Th?i gian b?t ð?u
    - Th?i gian k?t thúc
    - S? phút ð?c
    - Ngày ð?c
         ?
    UpdateReadingStreak()
         ?
    CheckAndNotifyGoalAchievement()
```

### Streak Calculation

```
Ngày 1: Ð?c sách ? Streak = 1
Ngày 2: Ð?c sách ? Streak = 2 (liên t?c)
Ngày 3: B? qua   ? Streak = 0 (reset)
Ngày 4: Ð?c sách ? Streak = 1 (b?t ð?u l?i)
```

**Lýu ?:** Ch? c?n ð?c ít nh?t 1 phút trong ngày là ðý?c tính streak!

### Tính Sách Ð? Ð?c

**Ði?u ki?n tính là "ð? ð?c 1 cu?n":**
- Ð?c ít nh?t **10 phút** trong 1 phiên
- Ðý?c tính riêng cho t?ng tháng/nãm
- M?i cu?n ch? tính 1 l?n trong k?

---

## ??? C?U TRÚC DATABASE

### B?ng MucTieuDocSach
```sql
MaMucTieu INT PRIMARY KEY
MaNguoiDung INT (User ID)
LoaiMucTieu NVARCHAR(50) -- "DAILY_MINUTES", "MONTHLY_BOOKS" ho?c "YEARLY_BOOKS"
GiaTriMucTieu INT -- Giá tr? tùy ch?nh
NgayBatDau DATE
DangHoatDong BIT
NgayHoanThanh DATE (nullable)
```

### B?ng PhienDocSach
```sql
MaPhien INT PRIMARY KEY
MaNguoiDung INT
MaSach INT
ThoiGianBatDau DATETIME
ThoiGianKetThuc DATETIME (nullable)
SoPhutDoc INT
NgayDoc DATE
```

### B?ng ChuoiNgayDocSach
```sql
MaNguoiDung INT PRIMARY KEY
SoNgayHienTai INT -- Current streak
SoNgayDaiNhat INT -- Longest streak
NgayDocGanNhat DATE
```

### B?ng LichSuNhacNho
```sql
MaNhacNho INT PRIMARY KEY
MaNguoiDung INT
ThoiGianNhacNho DATETIME
NoiDungNhacNho NVARCHAR(500)
DaXem BIT
```

---

## ?? TÍNH NÃNG N?I B?T

### 1. Tùy Ch?nh Hoàn Toàn
- Không gi?i h?n ? 30 phút hay 12 cu?n
- B?n quy?t ð?nh m?c tiêu c?a m?nh
- Thay ð?i b?t c? lúc nào

### 2. 3 Lo?i M?c Tiêu
- **Hàng ngày**: Ðo b?ng phút
- **Hàng tháng**: Ðo b?ng s? cu?n
- **Hàng nãm**: Ðo b?ng s? cu?n

### 3. T? Ð?ng Hóa
- Không c?n làm g?, t? ð?ng track
- Service ch?y background liên t?c
- Notification t? ð?ng

### 4. Tr?c Quan
- Progress bar r? ràng
- Bi?u ð? 7 ngày
- Màu s?c phân bi?t ð?t/chýa ð?t

---

## ?? TROUBLESHOOTING

### L?i: "Không t?m th?y b?ng MucTieuDocSach"
? B?n chýa ch?y script SQL. H?y ch?y file `CreateReadingGoalTables.sql`

### M?c tiêu không c?p nh?t
? Ð?m b?o b?n ð? nh?n "Ð?t m?c tiêu" sau khi thay ð?i

### Streak không tãng
? B?n ph?i ð?c sách (m? và ðóng) ð? streak ðý?c tính

### Th?ng kê sai
? Ki?m tra timezone c?a database và client

---

**Chúc b?n ð?t ðý?c m?c tiêu ð?c sách!** ???
