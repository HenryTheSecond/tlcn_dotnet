using Microsoft.EntityFrameworkCore;
using Quartz;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Jobs
{
    public class CheckExpireInventory : IJob
    {
        private readonly MyDbContext _dbContext;
        public CheckExpireInventory(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("===========================================Cron Job========================================");
            var inventories = _dbContext.Inventory
                .Where(i => i.ExpireDate != null && i.ExpireDate.Value.Date <= DateTime.Now.Date && i.IsChecked == false)
                .Include(i => i.Product).ToList();
            inventories.ForEach(i => i.IsChecked = true);
            var inventoriesGroupByProduct = inventories
                .GroupBy(i => i.Product)
                .Select(group => new { Product = group.Key, Quantity = group.Sum(i => i.Quantity) });
            
            foreach (var group in inventoriesGroupByProduct)
            {
                double sales = group.Product.SalesUntilCheckExpire - group.Quantity;
                if (sales >= 0)
                    group.Product.SalesUntilCheckExpire = sales;
                else
                {
                    group.Product.SalesUntilCheckExpire = 0;
                    group.Product.Quantity += sales;

                    var expireNotification = new InventoryNotification
                    {
                        Content = $"{-sales} sản phẩm {group.Product.Name} đã hết hạn",
                        CreatedDate = DateTime.Now.Date,
                        IsRead = false,
                        Type = Constant.InventoryNotificationType.Expire
                    };
                    _dbContext.InventoryNotification.Add(expireNotification);
                }
            }

            DateTime now = DateTime.Now.Date;
            var almostExpireInventory = _dbContext.Inventory
                .Include(i => i.Product)
                .Where(i => i.ExpireDate.Value.Date > now.AddDays(3))
                .ToList();
            var groupAlmostExpireInventory = almostExpireInventory
                .GroupBy(i => i.Product)
                .Select(group => new { Product = group.Key, Quantity = group.Sum(i => i.Quantity) });
            foreach (var grp in groupAlmostExpireInventory)
            {
                if (grp.Product.Quantity - grp.Quantity > 0)
                    _dbContext.InventoryNotification.Add(new InventoryNotification
                    {
                        Content = $"{grp.Product.Quantity - grp.Quantity} sản phẩm {grp.Product.Name} gần hết hạn",
                        CreatedDate= DateTime.Now.Date,
                        IsRead = false,
                        Type = Constant.InventoryNotificationType.AlmostExpire
                    });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
