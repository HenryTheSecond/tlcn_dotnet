using System.Runtime.Serialization;

namespace tlcn_dotnet.Controllers
{
    [Serializable]
    internal class GeneralExceptio : Exception
    {
        private string iNVALID_ID;
        private int bAD_REQUEST_CODE;

        public GeneralExceptio()
        {
        }

        public GeneralExceptio(string? message) : base(message)
        {
        }

        public GeneralExceptio(string iNVALID_ID, int bAD_REQUEST_CODE)
        {
            this.iNVALID_ID = iNVALID_ID;
            this.bAD_REQUEST_CODE = bAD_REQUEST_CODE;
        }

        public GeneralExceptio(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected GeneralExceptio(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}