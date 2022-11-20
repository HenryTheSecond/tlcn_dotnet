namespace tlcn_dotnet.Constant
{
    public class ApplicationConstant
    {
        // RESPONSE CONSTANT
        public static readonly string SUCCESSFUL = "SUCCESSFUL";
        public static readonly int SUCCESSFUL_CODE = 200;
        public static readonly string FAILED = "FAILED";
        public static readonly int FAILED_CODE = 400;
        public static readonly string UNAUTHORIZED = "UNAUTHORIZED";
        public static readonly int UNAUTHORIZED_CODE = 401;
        public static readonly string FORBIDDEN = "FORBIDDEN";
        public static readonly int FORBIDDEN_CODE = 403;
        public static readonly string BAD_REQUEST = "BAD_REQUEST";
        public static readonly int BAD_REQUEST_CODE = 400;
        public static readonly string BAD_REQUEST_MESSAGE = "BAD REQUEST";
        public static readonly string NOT_FOUND = "NOT FOUND";
        public static readonly int NOT_FOUND_CODE = 404;
        public static readonly string INVALID_ID = "ID IS INVALID";
        public static readonly int SQL_ERROR_CODE = 470;
        public static readonly string SQL_ERROR = "SQL ERROR";


        public static readonly string VALIDATE_ERROR = "VALIDATE ERROR";

        public static readonly string EMAIL_OR_PASSWORD_MISSING = "EMAIL OR PASSWORD MISSING";
        public static readonly string ACCOUNT_INACTIVE = "ACCOUNT HAS NOT BEEN ACTIVATED";
        public static readonly string EMAIL_OR_PASSWORD_INCORRECT = "EMAIL OR PASSWORD IS INCORRECT";
        public static readonly string CONFIRM_TOKEN_EMAIL_SUBJECT = "Verify your email";
        public static readonly string CONFIRM_CHANGE_PASSWORD_SUBJECT = "Verify Your Change Password";
        public static readonly string TOKEN_NOT_FOUND = "TOKEN NOT FOUND";
        public static readonly string EMAIL_HAS_BEEN_CONFIRMED = "EMAIL HAS BEEN CONFIRMED";
        public static readonly string TOKEN_EXPIRED = "TOKEN EXPIRED";
        public static readonly string USER_INACTIVE = "USER IS INACTIVE";

        public static readonly long JWT_ACCESS_TOKEN_EXPIRATION = 600000L; // 10 minutes in milliseconds
        public static readonly string JWT_SECRET = "HenryTheSecond";
        //public static readonly Algorithm JWT_ALGORITHM = Algorithm.HMAC256(JWT_SECRET.getBytes());
        public static readonly string JWT_TOKEN_MISSING = "JWT_TOKEN_MISSING";
        public static readonly string JWT_INCORRECT = "JWT_INCORRECT";

        public static readonly string COUNTRY_AND_CITY_DIRECTORY = "Properties\\country_and_city.json";
        public static readonly string VIETNAM_REGION = "Properties\\vietnam_city_district_ward.json";

        //Localtion constants
        public static readonly string WARD_NOT_FOUND = "WARD NOT FOUND";
        public static readonly string CITY_NOT_FOUND = "CITY NOT FOUND";
        public static readonly string DISTRICT_NOT_FOUND = "DISTRICT NOT FOUND";
    }
}
