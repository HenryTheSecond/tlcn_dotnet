namespace tlcn_dotnet.CustomException
{
    public class GeneralException: ApplicationException
    {
        public int? Status { get; set; }
        public string DetailMessage { get; set; }
        public Object Data { get; set; }

        public GeneralException(string message, int status, String detailMessage, Object data): base(message)
        {
            this.Status = status;
            this.DetailMessage = detailMessage;
            this.Data = data;
        }

        public GeneralException(String message, int status): base(message)
        {
            this.Status = status;
        }

        public GeneralException(String message): base(message)
        {}
    }
}
