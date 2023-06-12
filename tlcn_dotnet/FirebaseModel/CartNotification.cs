using Google.Cloud.Firestore;

namespace tlcn_dotnet.FirebaseModel
{
    [FirestoreData]
    public class CartNotification
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Content { get; set; }
        [FirestoreProperty]
        public string Url { get; set; }
        [FirestoreProperty]
        public Timestamp CreatedDate { get; set; }
        [FirestoreProperty]
        public bool IsRead { get; set; } = false;
        [FirestoreProperty]
        public bool IsDeleted { get; set; } = false;
        [FirestoreProperty]
        public long AccountId { get; set; }
        [FirestoreProperty]
        public long CartId { get; set; }
    }
}
