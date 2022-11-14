using Dapper;
using DapperQueryBuilder;
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
    public class InventoryRepositoryImpl: InventoryRepository
    {
        private readonly DapperContext _dapperContext;

        private static readonly FormattableString SELECT_INVENTORY_QUERY = $@"select i.Id, i.Quantity, i.ImportPrice, i.DeliveryDate, i.ExpireDate, i.Description, i.Unit,
	                                    p.Id, p.Description, p.MinPurchase, p.Name, p.Price, p.Quantity, p.Unit,
	                                    s.Id, s.Name, s.CountryCode, s.CityCode, s.DetailLocation,
	                                    c.Id, c.Name,
	                                    img.Id, img.FileName, img.Url
                                from (((inventory i left outer join product p on i.productid = p.id)
	                                    left outer join Supplier s on i.SupplierId = s.Id)
	                                    left outer join Category c on p.CategoryId = c.Id)
	                                    left outer join ProductImage img on p.Id = img.ProductId ";

        public InventoryRepositoryImpl(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<Inventory> AddInventory(AddInventoryDto addInventoryDto)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"Insert into Inventory (ProductId, Quantity, 
                                                    ImportPrice, DeliveryDate, ExpireDate, 
                                                    Description, SupplierId, Unit)
                                OUTPUT inserted.Id
                                values (@ProductId, @Quantity, @ImportPrice, 
                                        @DeliveryDate, @ExpireDate, @Description, 
                                        @SupplierId, @Unit)";

                long id = await connection.ExecuteScalarAsync<long>(query, addInventoryDto);
                return await GetInventoryById(id);
            }
        }

        public async Task<Inventory> GetInventoryById(long id)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"select i.Id, i.Quantity, i.ImportPrice, i.DeliveryDate, i.ExpireDate, i.Description, i.Unit,
	                                    p.Id, p.Description, p.MinPurchase, p.Name, p.Price, p.Quantity, p.Unit,
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

        public async Task<IEnumerable<Inventory>> SearchInventory(string? keyword, long? productId,
            double? minQuantity, double? maxQuantity, decimal? minImportPrice, decimal? maxImportPrice, 
            DateTime? fromDeliveryDate, DateTime? toDeliveryDate, DateTime? fromExpireDate, 
            DateTime? toExpireDate, long? supplierId, string? unit, InventoryOrderBy inventoryOrderBy, SortOrder sortOrder, int page = 1)
        {
            string query = SELECT_INVENTORY_QUERY + " WHERE 1=1 ";
            var parameters = new DynamicParameters();
            using (var connection = _dapperContext.CreateConnection())
            {

                if (keyword != null)
                {
                    query += " AND i.Description LIKE '%@keyword%' ";
                    parameters.Add("keyword", keyword);

                }
                if (productId != null)
                {
                    query += " AND p.Id = @productId ";
                    parameters.Add("productId", productId);
                }
                if (minQuantity != null)
                {
                    query += " AND i.Quantity >= @minQuantity ";
                    parameters.Add("minQuantity", minQuantity);
                }
                if (maxQuantity != null)
                {
                    query += " AND i.Quantity <= @maxQuantity ";
                    parameters.Add("maxQuantity", maxQuantity);
                }
                if (minImportPrice != null)
                {
                    query += " AND i.ImportPrice >= @minImportPrice ";
                    parameters.Add("minImportPrice", minImportPrice);
                }
                if (maxImportPrice != null)
                {
                    query += " AND i.ImportPrice <= @maxImportPrice ";
                    parameters.Add("maxImportPrice", maxImportPrice);
                }
                if (fromDeliveryDate != null)
                {
                    query += " AND i.DeliveryDate >= @fromDeliveryDate ";
                    parameters.Add("fromDeliveryDate", fromDeliveryDate);
                }
                if (toDeliveryDate != null)
                {
                    query += " AND i.DeliveryDate <= @toDeliveryDate ";
                    parameters.Add("toDeliveryDate", toDeliveryDate);
                }
                if (fromExpireDate != null)
                {
                    query += " AND i.ExpireDate >= @fromExpireDate ";
                    parameters.Add("fromExpireDate", fromExpireDate);
                }
                if (toExpireDate != null)
                {
                    query += " AND i.ExpireDate <= @toExpireDate ";
                    parameters.Add("toExpireDate", toExpireDate);
                }
                if (supplierId != null)
                {
                    query += " AND s.Id = @supplierId ";
                    parameters.Add("supplierId", supplierId);
                }
                if (unit != null)
                {
                    query += " AND i.Unit LIKE @unit ";
                    parameters.Add("unit", unit);
                }
                query += " ORDER BY " + ParseToOrderBy(inventoryOrderBy) +" " + sortOrder.ToString() + " OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
                parameters.Add("skip", (page - 1) * 2);
                parameters.Add("take", 2);
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
                inventories = inventories.DistinctBy(inventory => inventory.Id);
                return inventories;
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
    }
}
