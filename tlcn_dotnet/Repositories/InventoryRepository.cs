using Dapper;
using DapperQueryBuilder;
using Microsoft.OpenApi.Extensions;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.InventoryDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Repositories;
using SortOrder = tlcn_dotnet.Constant.SortOrder;

namespace tlcn_dotnet.RepositoriesImpl
{
    public class InventoryRepository: IInventoryRepository
    {
        private readonly DapperContext _dapperContext;

        private readonly string SELECT = @" select i.Id, i.Quantity, i.ImportPrice, i.DeliveryDate, i.ExpireDate, i.Description, i.Unit,
	                                            p.Id, p.Description, p.MinPurchase, p.Name, p.Price, p.Unit, p.Quantity,
	                                            s.Id, s.Name, s.CountryCode, s.CityCode, s.DetailLocation,
	                                            c.Id, c.Name,
	                                            img.Id, img.FileName, img.Url ";
        private readonly string FROM = @" from (((inventory i left outer join product p on i.productid = p.id)
                                                left outer join Supplier s on i.SupplierId = s.Id)
                                                left outer join Category c on p.CategoryId = c.Id)
                                                outer apply (select top 1 img.Id, img.FileName, img.Url from ProductImage img where img.ProductId = p.Id) as img  ";

        public InventoryRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<Inventory> AddInventory(AddInventoryDto addInventoryDto)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"EXEC sp_InsertInventory
                                @ProductId, @Quantity, @ImportPrice, 
                                @DeliveryDate, @ExpireDate, @Description, 
                                @SupplierId, @Unit";
                DynamicParameters parameters = new DynamicParameters(addInventoryDto);
                parameters.Add("Unit", addInventoryDto.Unit.GetDisplayName());
                long id = await connection.ExecuteScalarAsync<long>(query, parameters);
                return await GetInventoryById(id);
            }
        }

        public async Task<Inventory> GetInventoryById(long id)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"select i.Id, i.Quantity, i.ImportPrice, i.DeliveryDate, i.ExpireDate, i.Description, i.Unit,
	                                    p.Id, p.Description, p.MinPurchase, p.Name, p.Price, p.Unit, p.Quantity,
	                                    s.Id, s.Name, s.CountryCode, s.CityCode, s.DetailLocation,
	                                    c.Id, c.Name,
	                                    img.Id, img.FileName, img.Url
                                from (((inventory i left outer join product p on i.productid = p.id)
	                                    left outer join Supplier s on i.SupplierId = s.Id)
	                                    left outer join Category c on p.CategoryId = c.Id)
	                                    left outer join ProductImage img on p.Id = img.ProductId 
                                where i.Id = @Id";
                IEnumerable<Inventory> inventories = await connection.QueryAsync<Inventory, Product, Supplier, Category, ProductImage, Inventory>(query, 
                    (inventory, product, supplier, category, productImage ) =>
                    {
                        if(productImage != null)
                            product.ProductImages.Add(productImage);
                        product.Category = category;
                        inventory.Supplier = supplier;
                        inventory.Product = product;
                        return inventory;
                    }, splitOn: "Id",
                    param: new { Id = id});
                Inventory inventoryDb = inventories.FirstOrDefault();
                if (inventories == null)
                    throw new GeneralException("INVENTORY NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
                return inventoryDb;
            }
        }

        public async Task<dynamic> SearchInventory(string? keyword, long? productId,
            double? minQuantity, double? maxQuantity, decimal? minImportPrice, decimal? maxImportPrice, 
            DateTime? fromDeliveryDate, DateTime? toDeliveryDate, DateTime? fromExpireDate, 
            DateTime? toExpireDate, long? supplierId, string? unit, InventoryOrderBy inventoryOrderBy, SortOrder sortOrder, int page = 1)
        {
            string query = SELECT + FROM + " WHERE 1=1 ";
            string countQuery = " SELECT COUNT(DISTINCT i.Id) " + FROM + " WHERE 1=1 ";
            string conditions = " ";
            var parameters = new DynamicParameters();
            using (var connection = _dapperContext.CreateConnection())
            {

                if (keyword != null)
                {
                    conditions += " AND i.Description LIKE '%@keyword%' ";
                    parameters.Add("keyword", keyword);

                }
                if (productId != null)
                {
                    conditions += " AND p.Id = @productId ";
                    parameters.Add("productId", productId);
                }
                if (minQuantity != null)
                {
                    conditions += " AND i.Quantity >= @minQuantity ";
                    parameters.Add("minQuantity", minQuantity);
                }
                if (maxQuantity != null)
                {
                    conditions += " AND i.Quantity <= @maxQuantity ";
                    parameters.Add("maxQuantity", maxQuantity);
                }
                if (minImportPrice != null)
                {
                    conditions += " AND i.ImportPrice >= @minImportPrice ";
                    parameters.Add("minImportPrice", minImportPrice);
                }
                if (maxImportPrice != null)
                {
                    conditions += " AND i.ImportPrice <= @maxImportPrice ";
                    parameters.Add("maxImportPrice", maxImportPrice);
                }
                if (fromDeliveryDate != null)
                {
                    conditions += " AND i.DeliveryDate >= @fromDeliveryDate ";
                    parameters.Add("fromDeliveryDate", fromDeliveryDate);
                }
                if (toDeliveryDate != null)
                {
                    conditions += " AND i.DeliveryDate <= @toDeliveryDate ";
                    parameters.Add("toDeliveryDate", toDeliveryDate);
                }
                if (fromExpireDate != null)
                {
                    conditions += " AND i.ExpireDate >= @fromExpireDate ";
                    parameters.Add("fromExpireDate", fromExpireDate);
                }
                if (toExpireDate != null)
                {
                    conditions += " AND i.ExpireDate <= @toExpireDate ";
                    parameters.Add("toExpireDate", toExpireDate);
                }
                if (supplierId != null)
                {
                    conditions += " AND s.Id = @supplierId ";
                    parameters.Add("supplierId", supplierId);
                }
                if (unit != null)
                {
                    conditions += " AND i.Unit LIKE @unit ";
                    parameters.Add("unit", unit);
                }
                query += conditions + " ORDER BY " + ParseToOrderBy(inventoryOrderBy) + " " + sortOrder.ToString() + " OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
                countQuery += conditions;
                parameters.Add("skip", (page - 1) * 2);
                parameters.Add("take", 2);
                Console.WriteLine(query);
                IEnumerable<Inventory> inventories = await connection.QueryAsync<Inventory, Product, Supplier, Category, ProductImage, Inventory>(query,
                   (inventory, product, supplier, category, productImage) =>
                   {
                       if (productImage != null)
                           product.ProductImages.Add(productImage);
                       product.Category = category;
                       inventory.Supplier = supplier;
                       inventory.Product = product;
                       return inventory;
                   }, splitOn: "Id",
                   param: parameters);
                int total = await connection.ExecuteScalarAsync<int>(countQuery, parameters);
                return new { 
                    inventories = inventories,
                    total = total
                };
            }
        }

        private string ParseToOrderBy(InventoryOrderBy inventoryOrderBy) 
        {
            switch (inventoryOrderBy)
            {
                case InventoryOrderBy.DELIVERY_DATE:
                    return "i.DeliveryDate";
                case InventoryOrderBy.EXPIRE_DATE:
                    return "i.ExpireDate";
                case InventoryOrderBy.QUANTITY:
                    return "i.Quantity";
                case InventoryOrderBy.IMPORT_PRICE:
                    return "ImportPrice";
                default:
                    return "i.DeliveryDate";
            }
        }

        public async Task<Inventory> UpdateInventory(long id, EditInventoryDto editInventoryDto)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"EXEC sp_UpdateInventory 
                                    @Id, @ProductId, @Quantity, @ImportPrice, @Unit,
                                    @DeliveryDate, @ExpireDate,
                                    @Description, @SupplierId ";
                DynamicParameters parameters = new DynamicParameters(editInventoryDto);
                parameters.Add("Id", id);
                parameters.Add("Unit", editInventoryDto.Unit.GetDisplayName());
                int affectedRow = await connection.ExecuteAsync(query, parameters);
                if (affectedRow == 0)
                    throw new GeneralException("INVENTORY NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
                return await GetInventoryById(id);
            }
        }
    }
}
