using tlcn_dotnet.Constant;

namespace tlcn_dotnet
{
    public class DataResponse
    {
        public string Message { get; set; }
        public Object? Data { get; set; }
        public int Status { get; set; } = Constant.ApplicationConstant.SUCCESSFUL_CODE;
        public string DetailMessage { get; set; }

        public DataResponse(Object data) 
        {
            this.Data = data;
            this.Message = ApplicationConstant.SUCCESSFUL;
            this.Status = ApplicationConstant.SUCCESSFUL_CODE;
        }

        public DataResponse() { }
    }
}
