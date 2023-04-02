using Microsoft.EntityFrameworkCore;
using Quartz;

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
            Console.WriteLine("===========================================Cron Job");
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
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
