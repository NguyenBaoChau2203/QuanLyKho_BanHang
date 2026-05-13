## 1. Goal

Phase 0 establishes the shared contract surface for the team so UI, inventory backend, and sales/report backend can move independently. The design intentionally stops at contract definition and mock behavior; it does not implement screens or backend logic.

## 2. Contract decisions

### 2.1 Database contract

Use SQL Server LocalDB and keep the existing 3-layer architecture:

- WinForms → BLL → DAL → DTO
- WinForms must never call DAL directly
- SQL must never appear in WinForms
- DAL implementation will use ADO.NET with parameterized queries when it is built later

Finalize the following schema rules in `database/schema.sql`:

- Keep these core tables: `Roles`, `Users`, `Categories`, `Products`, `Suppliers`, `Customers`, `PurchaseReceipts`, `PurchaseReceiptDetails`, `SalesInvoices`, `SalesInvoiceDetails`, `StockTransactions`, `Stocktakes`, `StocktakeDetails`, `AuditLogs`.
- Preserve single-column integer identity primary keys for all tables.
- Preserve unique business codes on master data and documents:
  - `Users.Username`
  - `Categories.Code`
  - `Products.Code`
  - `Suppliers.Code`
  - `Customers.Code`
  - `PurchaseReceipts.ReceiptCode`
  - `SalesInvoices.InvoiceCode`
  - `Stocktakes.StocktakeCode`
- Keep money fields as `DECIMAL(18,2)`.
- Keep timestamps as `DATETIME2`.
- Keep soft-delete style flags with `IsActive` on master data.
- Keep stock and sales transaction history in `StockTransactions`.
- Keep referential integrity with foreign keys between document headers, details, and master data.
- Keep validation constraints for non-negative price/stock fields and positive detail quantities.
- Add any missing CHECK/INDEX/NOT NULL rules only if they do not break the current public contract.

Finalize the following seed rules in `database/seed.sql`:

- Seed at least one admin/login account for Châu and demo accounts for Dũ and Hùng.
- Seed enough categories, suppliers, customers, products, and stock history to demonstrate inventory and sales flows.
- Seed data must support login and a visible dashboard without requiring manual inserts.
- Seed data must not depend on backend implementation details.

### 2.2 DTO contract

DTOs are the only shared data contract between WinForms and BLL.

Finalize these DTO groups in `src/QuanLyKhoBanHang.DTO/`:

- `MasterData`: `CategoryDto`, `ProductDto`, `SupplierDto`
- `Inventory`: `PurchaseReceiptDto`, `PurchaseReceiptLineDto`, `StockTransactionDto`, `StocktakeDto`, `StocktakeLineDto`
- `Sales`: `CustomerDto`, `SalesInvoiceDto`, `SalesInvoiceLineDto`
- `Reports`: `RevenueSummaryDto`, `ProductSalesSummaryDto`, `CustomerPurchaseSummaryDto`, `DashboardSummaryDto`
- `Auth`: `UserDto`
- `Assistant`: `AssistantResponseDto`
- `Common`: `UserRole`, `StockTransactionType`

Contract rules:

- Existing property names should remain stable.
- Additive changes are allowed if they do not break current consumers.
- DTOs may include navigation/display fields needed by UI, such as `CategoryName` or summary totals, but must remain data-only.
- DTOs must not contain DAL, UI, or service logic.

### 2.3 BLL service contract

Public service methods must expose stable names, parameters, and return types using `ServiceResult<T>`.

Finalize these public signatures:

#### Master data and inventory

- `CategoryService.GetAllCategories()` → `ServiceResult<List<CategoryDto>>`
- `CategoryService.CreateCategory(CategoryDto category)` → `ServiceResult<int>`
- `CategoryService.UpdateCategory(CategoryDto category)` → `ServiceResult<bool>`
- `CategoryService.DeactivateCategory(int id)` → `ServiceResult<bool>`

- `ProductService.GetAllProducts()` → `ServiceResult<List<ProductDto>>`
- `ProductService.SearchProducts(string keyword)` → `ServiceResult<List<ProductDto>>`
- `ProductService.GetProductById(int id)` → `ServiceResult<ProductDto>`
- `ProductService.CreateProduct(ProductDto product)` → `ServiceResult<int>`
- `ProductService.UpdateProduct(ProductDto product)` → `ServiceResult<bool>`
- `ProductService.DeactivateProduct(int id)` → `ServiceResult<bool>`

- `SupplierService.GetAllSuppliers()` → `ServiceResult<List<SupplierDto>>`
- `SupplierService.SearchSuppliers(string keyword)` → `ServiceResult<List<SupplierDto>>`
- `SupplierService.CreateSupplier(SupplierDto supplier)` → `ServiceResult<int>`
- `SupplierService.UpdateSupplier(SupplierDto supplier)` → `ServiceResult<bool>`
- `SupplierService.DeactivateSupplier(int id)` → `ServiceResult<bool>`

- `PurchaseService.CreateReceipt(PurchaseReceiptDto receipt)` → `ServiceResult<int>`
- `PurchaseService.GetReceipts(DateTime fromDate, DateTime toDate)` → `ServiceResult<List<PurchaseReceiptDto>>`
- `PurchaseService.GetReceiptById(int id)` → `ServiceResult<PurchaseReceiptDto>`

- `InventoryService.GetCurrentStock()` → `ServiceResult<List<ProductDto>>`
- `InventoryService.GetLowStockProducts()` → `ServiceResult<List<ProductDto>>`
- `InventoryService.GetStockTransactions(DateTime fromDate, DateTime toDate)` → `ServiceResult<List<StockTransactionDto>>`

- `StocktakeService.CreateStocktake(StocktakeDto stocktake)` → `ServiceResult<int>`
- `StocktakeService.GetStocktakes(DateTime fromDate, DateTime toDate)` → `ServiceResult<List<StocktakeDto>>`
- `StocktakeService.GetStocktakeById(int id)` → `ServiceResult<StocktakeDto>`

#### Sales, reports, assistant

- `CustomerService.GetAllCustomers()` → `ServiceResult<List<CustomerDto>>`
- `CustomerService.SearchCustomers(string keyword)` → `ServiceResult<List<CustomerDto>>`
- `CustomerService.GetCustomerById(int id)` → `ServiceResult<CustomerDto>`
- `CustomerService.CreateCustomer(CustomerDto customer)` → `ServiceResult<int>`
- `CustomerService.UpdateCustomer(CustomerDto customer)` → `ServiceResult<bool>`
- `CustomerService.DeactivateCustomer(int id)` → `ServiceResult<bool>`

- `SalesService.CreateInvoice(SalesInvoiceDto invoice)` → `ServiceResult<int>`
- `SalesService.GetInvoices(DateTime fromDate, DateTime toDate)` → `ServiceResult<List<SalesInvoiceDto>>`
- `SalesService.GetInvoiceById(int id)` → `ServiceResult<SalesInvoiceDto>`
- Invoice printing is explicitly out of scope for Phase 0 and no `PrintInvoice` service method is added or finalized in this change

- `ReportService.GetRevenue(DateTime fromDate, DateTime toDate)` → `ServiceResult<List<RevenueSummaryDto>>`
- `ReportService.GetTopSellingProducts(DateTime fromDate, DateTime toDate, int top = 5)` → `ServiceResult<List<ProductSalesSummaryDto>>`
- `ReportService.GetTopCustomers(DateTime fromDate, DateTime toDate, int top = 5)` → `ServiceResult<List<CustomerPurchaseSummaryDto>>`

- `AssistantService` should support rule-based responses for revenue and top-selling queries, returning `ServiceResult<AssistantResponseDto>`.

### 2.4 Mock/stub behavior

To keep UI work unblocked, Phase 0 defines stub behavior for all public BLL services:

- Read-only methods return seeded demo data or empty but valid collections when backend data access is not yet wired.
- Create/update/deactivate methods validate obvious input rules and return deterministic success/failure messages even before DAL exists.
- Stub responses must be shaped exactly like the final service contract so UI code does not need to change later.
- Stub services should be easy to replace with real implementations without changing form code.

Recommended implementation pattern for later coding:

- Keep the service public signature fixed.
- Put mock data behind a simple in-memory provider or a temporary branch inside service methods.
- Use the same DTO shapes and the same `ServiceResult<T>` envelope for both mock and real paths.

### 2.5 Ownership and change rules

- Châu owns the database scripts, OpenSpec change, and final contract arbitration.
- Dũ owns inventory-related DAL/BLL implementation.
- Hùng owns sales/report-related DAL/BLL implementation.
- Any change to schema, DTO property names, or public service signatures is a contract change and requires Châu review before merge.
- Implementation-only changes inside DAL or private service logic do not require UI changes if the contract stays the same.

## 3. Phase boundary

This phase does not include WinForms screen implementation or full backend implementation.

It only locks the shared contract so the next work can proceed in parallel:

- Châu can build UI against stable contracts.
- Dũ can build inventory backend independently.
- Hùng can build sales/report backend independently.

## 4. Risks and mitigations

- **Risk**: DTO property churn breaks UI builds.
  - **Mitigation**: freeze property names now and prefer additive changes later.
- **Risk**: service signature drift between branches.
  - **Mitigation**: treat signatures as contract and require review for any change.
- **Risk**: seed data is too thin for UI demos.
  - **Mitigation**: seed all required master data and a few stock transactions.
- **Risk**: stub behavior diverges from final behavior.
  - **Mitigation**: keep stubs in the same result envelope and DTO shapes as final services.
