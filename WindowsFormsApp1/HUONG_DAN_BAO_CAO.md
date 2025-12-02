# ?? Hý?ng d?n s? d?ng ch?c nãng Báo cáo

## ? Ð? tri?n khai thành công!

Ch?c nãng báo cáo danh sách sách ð? ðý?c tri?n khai ð?y ð? v?i kh? nãng xu?t ra 2 ð?nh d?ng:
- **HTML** - Báo cáo ð?p m?t, có th? xem trên tr?nh duy?t
- **CSV** - D? dàng import vào Excel/Google Sheets

---

## ?? Cách s? d?ng

### Bý?c 1: Ðãng nh?p
- Nh?n vào nút **??** ? góc trên bên ph?i
- Ch?n **"?? Ðãng Nh?p"** ho?c **"?? Ðãng K?"**

### Bý?c 2: Thêm sách vào thý vi?n
- Nh?n nút **"? Thêm sách"**
- Ch?n:
  - **"?? Nh?p file"** - Ch?n t?ng file sách (.epub, .pdf, .txt, .mobi)
  - **"?? Quét thý m?c"** - Quét toàn b? thý m?c có ch?a sách

### Bý?c 3: Xu?t báo cáo
1. Nh?n vào nút **"?? Báo cáo"** trên thanh công c?
2. Ch?n nõi lýu file và ch?n ð?nh d?ng:
   - **HTML** (*.html) - Báo cáo ð?p, có màu s?c
   - **CSV** (*.csv) - D?ng b?ng, d? x? l?
3. Nh?n **"Lýu"**
4. Ch?n **"Có"** ð? m? file báo cáo ngay

---

## ?? N?i dung báo cáo

Báo cáo s? bao g?m các thông tin:

### Thông tin chung
- ?? Tên ngý?i dùng
- ?? Ngày gi? t?o báo cáo
- ?? T?ng s? sách trong thý vi?n

### Chi ti?t t?ng sách
| C?t | Mô t? |
|-----|-------|
| **STT** | S? th? t? |
| **Tên sách** | Tiêu ð? sách |
| **Tác gi?** | Tên tác gi? |
| **Ð?nh d?ng** | EPUB, PDF, TXT, MOBI... |
| **Ti?n ð? ð?c** | Ph?n trãm ð? ð?c (0-100%) |
| **Ngày thêm** | Ngày thêm sách vào thý vi?n |
| **Yêu thích** | ?? n?u là sách yêu thích |

---

## ?? Ð?nh d?ng HTML

### Ýu ði?m
? Giao di?n ð?p m?t v?i màu s?c
? Có thanh progress bar hi?n th? ti?n ð? ð?c
? D? ð?c, d? in
? Có th? m? trên m?i tr?nh duy?t

### Tính nãng n?i b?t
- ?? Thi?t k? hi?n ð?i, responsive
- ?? Progress bar tr?c quan cho ti?n ð? ð?c
- ??? Hover effect trên t?ng d?ng
- ?? Màu xanh ch? ð?o chuyên nghi?p

### Cách xem
- M? file `.html` b?ng tr?nh duy?t (Chrome, Edge, Firefox...)
- Có th? in ra gi?y (Ctrl + P)

---

## ?? Ð?nh d?ng CSV

### Ýu ði?m
? D? m? b?ng Excel/Google Sheets
? Có th? ch?nh s?a, l?c, s?p x?p
? Nh?, d? chia s?
? D? import vào các h? th?ng khác

### Cách m?
1. **Excel**: Click ðúp vào file `.csv`
2. **Google Sheets**: 
   - File ? Import ? Upload file
   - Ch?n "Comma" làm separator
3. **Notepad**: Xem d?ng text thu?n

### Lýu ? CSV
- Encoding: UTF-8 (h? tr? ti?ng Vi?t)
- Separator: D?u ph?y (,)
- Các trý?ng có d?u ph?y s? ðý?c b?c trong `"..."`

---

## ?? Các t?nh hu?ng s? d?ng

### 1. Qu?n l? thý vi?n cá nhân
- Xu?t báo cáo HTML ð? xem t?ng quan
- In ra ð? theo d?i ti?n ð? ð?c

### 2. Chia s? danh sách sách
- Xu?t CSV ð? g?i cho b?n bè
- Import vào Google Sheets ð? chia s? online

### 3. Backup d? li?u
- Xu?t CSV ð?nh k? ð? lýu tr?
- D? dàng khôi ph?c n?u c?n

### 4. Th?ng kê
- M? CSV trong Excel
- T?o bi?u ð?, pivot table
- Phân tích thói quen ð?c sách

---

## ?? X? l? l?i

### Không th? lýu file
**Nguyên nhân**: Không có quy?n ghi vào thý m?c
**Gi?i pháp**: Ch?n thý m?c khác (Desktop, Documents...)

### File HTML không hi?n th? ti?ng Vi?t
**Nguyên nhân**: Tr?nh duy?t không nh?n UTF-8
**Gi?i pháp**: File ð? ðý?c encode UTF-8, th? tr?nh duy?t khác

### CSV b? l?i font trong Excel
**Nguyên nhân**: Excel không t? nh?n UTF-8
**Gi?i pháp**: 
1. M? Excel tr?ng
2. Data ? From Text/CSV
3. Ch?n file, ch?n UTF-8
4. Import

---

## ?? Tính nãng s?p t?i

Các tính nãng ðang phát tri?n:
- [ ] Xu?t PDF v?i giao di?n ð?p
- [ ] Báo cáo có bi?u ð? th?ng kê
- [ ] Xu?t báo cáo Ghi chú & Ðánh d?u
- [ ] Báo cáo M?c tiêu ð?c sách
- [ ] Tùy ch?nh template báo cáo
- [ ] L?c sách trý?c khi xu?t báo cáo

---

## ?? Tips & Tricks

### Tip 1: T?o báo cáo nhanh
- Phím t?t: Nh?n nút "?? Báo cáo" ? Enter ? Enter
- File s? ðý?c lýu v?i tên m?c ð?nh có ngày gi?

### Tip 2: Backup ð?nh k?
- Xu?t CSV m?i tu?n/tháng
- Lýu vào Google Drive/OneDrive
- Ð?t tên file theo ð?nh d?ng: `BaoCao_2025_01_12.csv`

### Tip 3: Chia s? v?i b?n bè
- Xu?t HTML ð? g?i qua email (ð?p hõn)
- Xu?t CSV n?u b?n bè c?n ch?nh s?a

### Tip 4: In báo cáo
- M? file HTML
- Nh?n Ctrl + P (ho?c File ? Print)
- Ch?n máy in ho?c "Save as PDF"

---

## ?? Ví d? tên file báo cáo

```
BaoCaoSach_20250112_143025.html
BaoCaoSach_20250112_143025.csv
```

Ð?nh d?ng: `BaoCaoSach_[Nãm][Tháng][Ngày]_[Gi?][Phút][Giây].[html/csv]`

---

## ? FAQ

**Q: Tôi có th? xu?t báo cáo khi chýa ðãng nh?p không?**
A: Không, b?n ph?i ðãng nh?p m?i có d? li?u sách ð? xu?t báo cáo.

**Q: Báo cáo có tính c? sách trong thùng rác không?**
A: Không, ch? tính các sách ðang ho?t ð?ng (chýa xóa).

**Q: Tôi có th? ch?nh s?a báo cáo HTML không?**
A: Có, m? b?ng text editor (Notepad, VS Code) và ch?nh s?a HTML/CSS.

**Q: CSV có h? tr? ti?ng Vi?t không?**
A: Có, file ðý?c lýu v?i encoding UTF-8.

**Q: Tôi có th? ch?n sách nào xu?t báo cáo không?**
A: Hi?n t?i xu?t toàn b? sách. Tính nãng l?c s? ðý?c thêm sau.

---

## ?? K?t lu?n

Chúc b?n s? d?ng ch?c nãng báo cáo hi?u qu?! 
N?u có góp ? ho?c phát hi?n l?i, vui l?ng báo cáo ð? c?i thi?n.

**Happy Reading! ??**
