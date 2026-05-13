# Phase 0 - Contract Foundation

## Mục tiêu

Phase 0 dùng để chốt nền tảng chung trước khi ba thành viên code song song. Sau phase này, Châu có thể làm UI bằng mock/stub, Dũ có thể làm backend kho, Hùng có thể làm backend bán hàng/báo cáo mà không phải chờ nhau implement xong.

## Vì sao cần phase này

Nếu cả nhóm code ngay mà chưa chốt database, DTO và service method, project rất dễ bị vỡ khi merge:

- UI gọi method chưa thống nhất.
- DAL query theo cột chưa tồn tại hoặc đổi tên.
- Dũ và Hùng cùng sửa `Products.QuantityOnHand` hoặc `StockTransactions` theo hai cách khác nhau.
- Một người đổi DTO làm form của người khác lỗi build.

Phase 0 giải quyết chuyện đó bằng cách chốt contract trước, implementation sau.

## Châu phụ trách chốt

- `database/schema.sql`
- `database/seed.sql`
- DTO chính trong `QuanLyKhoBanHang.DTO`
- Public method signature trong các service thuộc `QuanLyKhoBanHang.BLL`
- Quy định ownership file giữa ba người
- Mock/stub behavior để UI có thể chạy trước khi backend thật hoàn thành

## Contract cần ổn định

### Database

- Tên bảng, tên cột, khóa chính, khóa ngoại và constraint phải rõ ràng.
- Dữ liệu seed phải đủ để demo UI và test nghiệp vụ cơ bản.
- Nếu thêm bảng/cột sau Phase 0, người đề xuất phải ghi rõ trong PR.

### DTO

- DTO dùng làm contract giữa UI và BLL.
- Không đổi tên property tùy tiện sau khi UI đã sử dụng.
- Nếu cần thêm property thì ưu tiên thêm không phá vỡ code cũ.
- Nếu bắt buộc đổi tên/xóa property thì phải báo trong PR và ghi rõ form/service bị ảnh hưởng.

### BLL Service

- Public method name, parameter và return type phải ổn định.
- Service public ưu tiên trả về `ServiceResult<T>`.
- UI chỉ phụ thuộc vào service contract, không phụ thuộc implementation bên trong.
- Khi backend chưa xong, service có thể trả mock/stub data hợp lệ để UI làm tiếp.

## Phase 0 contract chốt nhanh

### File ownership

- Châu sở hữu `database/schema.sql`, `database/seed.sql`, `docs/`, `openspec/` và phần chốt contract chung.
- Dũ sở hữu backend kho, nhập kho, tồn kho, kiểm kê ở DAL/BLL.
- Hùng sở hữu backend khách hàng, bán hàng, báo cáo và assistant rule liên quan doanh thu.

### Rule đổi contract

- Bất kỳ thay đổi nào về schema, DTO property, hoặc public service signature đều là thay đổi contract.
- Thay đổi contract phải được Châu review trước khi merge.
- Không thêm `PrintInvoice` trong Phase 0.

### Rule stub/mock

- Service public có thể trả dữ liệu seed, empty list hợp lệ, hoặc fail validation có chủ đích.
- Không tạo hidden business logic khiến Dũ/Hùng phải gỡ bỏ lại ở phase sau.
- WinForms chỉ đi qua BLL, không gọi DAL trực tiếp.

## Làm song song sau Phase 0

### Châu

- Làm WinForms UI dựa trên DTO/service contract đã chốt.
- Tạm dùng mock/stub data khi service thật chưa hoàn thành.
- Không cần chờ Dũ/Hùng implement xong backend mới làm UI.
- Tích hợp service thật ở phase cuối.

### Dũ

- Làm DAL/BLL cho sản phẩm, loại hàng, nhà cung cấp, nhập kho, tồn kho, kiểm kê.
- Code theo schema/DTO/service contract đã chốt.
- Không cần chờ UI của Châu hoàn thiện.
- Không tự ý đổi `schema.sql`, `seed.sql`, DTO hoặc public method mà không ghi rõ trong PR.

### Hùng

- Làm DAL/BLL cho khách hàng, bán hàng, hóa đơn, báo cáo, assistant commands liên quan doanh thu.
- Code theo schema/DTO/service contract đã chốt.
- Không cần chờ UI của Châu hoàn thiện.
- Không tự ý đổi `schema.sql`, `seed.sql`, DTO hoặc public method mà không ghi rõ trong PR.

## File ownership

### Châu sở hữu chính

- `database/schema.sql`
- `database/seed.sql`
- `src/QuanLyKhoBanHang.WinForms/`
- `docs/`
- `openspec/`
- Contract DTO/service signature khi cần chốt kiến trúc chung

### Dũ sở hữu chính

- DAL/BLL liên quan kho, nhập kho, tồn kho, kiểm kê
- Tests cho nghiệp vụ kho
- Có thể đề xuất DTO/database thay đổi, nhưng phải ghi rõ trong PR

### Hùng sở hữu chính

- DAL/BLL liên quan khách hàng, bán hàng, hóa đơn, báo cáo
- Tests cho nghiệp vụ bán hàng/báo cáo
- Có thể đề xuất DTO/database thay đổi, nhưng phải ghi rõ trong PR

## Quy định merge

- Không push trực tiếp vào `main`.
- Mỗi người làm trên branch riêng.
- PR phải ghi rõ:
  - Service nào đã xong.
  - DTO nào thay đổi.
  - Database script có thay đổi không.
  - Cách test nhanh.
- Nếu PR đổi contract, Châu review trước khi merge.
- Nếu chỉ đổi implementation bên trong service mà không đổi contract, các thành viên khác không cần sửa code của mình.

## Checklist Phase 0

- [ ] `schema.sql` chạy được trên SQL Server LocalDB.
- [ ] `seed.sql` tạo đủ dữ liệu demo.
- [ ] DTO chính đã đủ cho UI và backend MVP.
- [ ] Public service methods đã có signature rõ ràng.
- [ ] Mock/stub service không làm UI bị block.
- [ ] Dũ và Hùng biết rõ file nào mình được sửa.
- [ ] Cả nhóm có thể bắt đầu làm song song sau khi kéo `main` mới nhất.
