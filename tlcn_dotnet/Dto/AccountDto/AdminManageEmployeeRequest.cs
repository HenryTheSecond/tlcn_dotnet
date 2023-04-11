using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;

namespace tlcn_dotnet.Dto.AccountDto
{
    public class AdminManageEmployeeRequest
    {
        private Role? _role;
        public ManageEmployeeSearchBy? SearchBy { get; set; }
        public string? Keyword { get; set; }
        public Role? Role 
        {
            get => _role;
            set
            {
                if (value == Constant.Role.ROLE_ADMIN || value == Constant.Role.ROLE_EMPLOYEE)
                    _role = value;
                else
                    throw new GeneralException("ROLE MUST BE ADMIN OR EMPLOYEE");
            }
        }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
