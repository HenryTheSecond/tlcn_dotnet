using Google.Cloud.Firestore;

namespace tlcn_dotnet.FirebaseModel
{
    [FirestoreData]
    public class CountCartNotificationByUser
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty]
        public long AccountId { get; set; }
        [FirestoreProperty]
        public long Count { get; set; }
    }
}
