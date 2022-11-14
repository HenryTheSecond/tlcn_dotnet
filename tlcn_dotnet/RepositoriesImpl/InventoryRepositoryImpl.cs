using Dapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Repositories;

namespace tlcn_dotnet.RepositoriesImpl
{
    public class InventoryRepositoryImpl: InventoryRepository
    {
        private readonly DapperContext _dapperContext;

        public InventoryRepositoryImpl(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
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
    }
}
