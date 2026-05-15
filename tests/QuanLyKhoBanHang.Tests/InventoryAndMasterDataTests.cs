using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.DTO.Inventory;
using System;
using System.Collections.Generic;

namespace QuanLyKhoBanHang.Tests;

[TestClass]
public sealed class InventoryAndMasterDataTests
{
    [TestMethod]
    public void CreateProduct_MissingCodeOrName_Fails()
    {
        var service = new ProductService();
        var product = new ProductDto { Code = "", Name = "Test" };
        var result = service.CreateProduct(product);
        Assert.IsFalse(result.Success);

        product.Code = "SP01"; product.Name = "";
        result = service.CreateProduct(product);
        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public void CreateProduct_NegativePriceOrStock_Fails()
    {
        var service = new ProductService();
        var product = new ProductDto { Code = "SP_TEST1", Name = "Test", CategoryId = 1, CostPrice = -100, SellingPrice = 100, QuantityOnHand = 10 };
        var result = service.CreateProduct(product);
        Assert.IsFalse(result.Success);

        product.CostPrice = 100; product.QuantityOnHand = -5;
        result = service.CreateProduct(product);
        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public void CreateReceipt_EmptyLines_Fails()
    {
        var service = new PurchaseService();
        var receipt = new PurchaseReceiptDto 
        { 
            ReceiptCode = "PN_TEST1", 
            SupplierId = 1, 
            CreatedByUserId = 1,
            Lines = new List<PurchaseReceiptLineDto>() 
        };
        var result = service.CreateReceipt(receipt);
        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    [Ignore("Requires active SQL Server connection to run this integration test")]
    public void CreateReceipt_Valid_IncreasesStockAndLogsTransaction()
    {
        var purchaseService = new PurchaseService();
        var productService = new ProductService();
        var inventoryService = new InventoryService();
        var categoryService = new CategoryService();
        var supplierService = new SupplierService();

        string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

        var catResult = categoryService.CreateCategory(new CategoryDto { Code = "C_" + uniqueSuffix, Name = "Test" });
        int categoryId = catResult.Data;
        Assert.IsTrue(categoryId > 0, "Category creation failed: " + catResult.Message);

        var supResult = supplierService.CreateSupplier(new SupplierDto { Code = "S_" + uniqueSuffix, Name = "Test", Phone = "0123", Email = "test@test.com", Address = "test" });
        int supplierId = supResult.Data;
        Assert.IsTrue(supplierId > 0, "Supplier creation failed: " + supResult.Message);

        var prodResult = productService.CreateProduct(new ProductDto { Code = "P_" + uniqueSuffix, Name = "Test", CategoryId = categoryId, CostPrice = 10000, SellingPrice = 12000, QuantityOnHand = 10 });
        int productId = prodResult.Data;
        Assert.IsTrue(productId > 0, "Product creation failed: " + prodResult.Message);

        int initialStock = 10;
        string receiptCode = "PN_" + uniqueSuffix;

        var receipt = new PurchaseReceiptDto 
        { 
            ReceiptCode = receiptCode, 
            SupplierId = supplierId, 
            CreatedByUserId = 1, // Assume Admin user id = 1
            ReceiptDate = DateTime.Now,
            Lines = new List<PurchaseReceiptLineDto>
            {
                new PurchaseReceiptLineDto { ProductId = productId, Quantity = 5, UnitCost = 10000 }
            }
        };

        var result = purchaseService.CreateReceipt(receipt);
        Assert.IsTrue(result.Success, result.Message);

        // Verify tồn kho tăng
        var productAfter = productService.GetProductById(productId).Data;
        Assert.AreEqual(initialStock + 5, productAfter!.QuantityOnHand);

        // Verify ghi log giao dịch
        var transactions = inventoryService.GetStockTransactionsByProduct(productId, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)).Data;
        Assert.IsNotNull(transactions);
        var trans = transactions.Find(t => t.ReferenceCode == receiptCode);
        Assert.IsNotNull(trans);
        Assert.AreEqual(5, trans.QuantityChange);
    }

    [TestMethod]
    [Ignore("Requires active SQL Server connection to run this integration test")]
    public void CreateStocktake_WithDifference_UpdatesStockAndLogsTransaction()
    {
        var stocktakeService = new StocktakeService();
        var productService = new ProductService();
        var inventoryService = new InventoryService();
        var categoryService = new CategoryService();

        string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

        var catResult = categoryService.CreateCategory(new CategoryDto { Code = "C_KK_" + uniqueSuffix, Name = "Test" });
        int categoryId = catResult.Data;
        Assert.IsTrue(categoryId > 0, "Category creation failed: " + catResult.Message);

        var prodResult = productService.CreateProduct(new ProductDto { Code = "P_KK_" + uniqueSuffix, Name = "Test", CategoryId = categoryId, CostPrice = 10000, SellingPrice = 12000, QuantityOnHand = 10 });
        int productId = prodResult.Data;
        Assert.IsTrue(productId > 0, "Product creation failed: " + prodResult.Message);
        
        int initialStock = 10;
        int actualQuantity = initialStock + 2; // Chênh lệch +2
        string stocktakeCode = "KK_" + uniqueSuffix;

        var stocktake = new StocktakeDto
        {
            StocktakeCode = stocktakeCode,
            CreatedByUserId = 1, // Assume Admin user id = 1
            StocktakeDate = DateTime.Now,
            Lines = new List<StocktakeLineDto>
            {
                new StocktakeLineDto { ProductId = productId, ActualQuantity = actualQuantity }
            }
        };

        var result = stocktakeService.CreateStocktake(stocktake);
        Assert.IsTrue(result.Success, result.Message);

        // Verify tồn kho đã cập nhật
        var productAfter = productService.GetProductById(productId).Data;
        Assert.AreEqual(actualQuantity, productAfter!.QuantityOnHand);

        // Verify ghi log giao dịch
        var transactions = inventoryService.GetStockTransactionsByProduct(productId, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)).Data;
        Assert.IsNotNull(transactions);
        var trans = transactions.Find(t => t.ReferenceCode == stocktakeCode);
        Assert.IsNotNull(trans);
        Assert.AreEqual(2, trans.QuantityChange);
        Assert.AreEqual(actualQuantity, trans.QuantityAfter);
    }
}
