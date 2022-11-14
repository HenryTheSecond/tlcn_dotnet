using System.Data.SqlClient;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Utils
{
    public class ExceptionUtil
    {
        public static DataResponse SqlExceptionHandle(SqlException exception)
        {
            if (exception.Message.Contains("FOREIGN KEY"))
            {
                int tableNameStartIndex = exception.Message.LastIndexOf("table \"dbo.") + 11;
                int tableNameEndIndex = exception.Message.LastIndexOf('"');
                string tableName = exception.Message.Substring(tableNameStartIndex, tableNameEndIndex - tableNameStartIndex);

                int columnNameStartIndex = exception.Message.LastIndexOf("column '") + 8;
                int columnNameEndIndex = exception.Message.LastIndexOf("'");
                string columnName = exception.Message.Substring(columnNameStartIndex, columnNameEndIndex - columnNameStartIndex);

                return new DataResponse()
                {
                    Message = ApplicationConstant.SQL_ERROR,
                    Status = ApplicationConstant.SQL_ERROR_CODE,
                    DetailMessage = $"{tableName} {columnName} NOT FOUND"
                };
            }
            return new DataResponse 
            {
                Message = ApplicationConstant.SQL_ERROR,
                Status = ApplicationConstant.SQL_ERROR_CODE,
                DetailMessage = exception.Message
            };
        }
    }
}
