using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using System.Net;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;
using tlcn_dotnet.FirebaseModel;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;
using CartNotification = tlcn_dotnet.FirebaseModel.CartNotification;
using CountCartNotificationByUser = tlcn_dotnet.FirebaseModel.CountCartNotificationByUser;

namespace tlcn_dotnet.Services
{
    public class CartNotificationFirebaseService : ICartNotificationService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly MyDbContext _dbContext;
        public CartNotificationFirebaseService(FirestoreDb firestoreDb, MyDbContext dbContext)
        {
            _firestoreDb = firestoreDb;
            _dbContext = dbContext;
        }

        public Task<DataResponse> CountNewNotifications(string authorization)
        {
            throw new NotImplementedException();
        }

        public async Task CreateCartNotification(Cart cart)
        {
            var account = (await _dbContext.CartDetail.Include(cd => cd.Account).Where(cd => cd.CartId == cart.Id).FirstOrDefaultAsync()).Account;
            string content = $"Đơn hàng với mã số {cart.Id} đã được vận chuyển vào thời gian {cart.DeliveryTime.Value.Date}";
            if (cart.Status == CartStatus.CANCELLED)
                content = $"Rất tiếc, đơn hàng với mã số {cart.Id} đã bị từ chối";
            CartNotification notification = new CartNotification
            {
                AccountId = account.Id.Value,
                CreatedDate = Timestamp.GetCurrentTimestamp(),
                CartId = cart.Id.Value,
                Url = $"my-account/orders/detail/{cart.Id}",
                Content = content
            };

            //Add cart notification
            CollectionReference collectionCartNotification = _firestoreDb.Collection("CartNotification");
            await collectionCartNotification.AddAsync(notification);

            //Increase count cart
            CollectionReference collectionCount = _firestoreDb.Collection("CountCartNotificationByUser");
            var query = collectionCount.WhereEqualTo("AccountId", account.Id.Value);
            var snapshot = await query.GetSnapshotAsync();
            if (snapshot.Count == 0)
            {
                await collectionCount.AddAsync(new CountCartNotificationByUser
                {
                    AccountId = account.Id.Value,
                    Count = 1
                });
            }
            else
            {
                var doc = snapshot[0];
                await collectionCount.Document(doc.Id).UpdateAsync("Count", FieldValue.Increment(1));
            }
        }

        public Task<DataResponse> GetCartNotification(string authorization, int offset, int size)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse> ReadNotification(string authorization, long notificationId)
        {
            throw new NotImplementedException();
        }

        public Task<DataResponse> ResetNewNotification(string authorization)
        {
            throw new NotImplementedException();
        }
    }
}
