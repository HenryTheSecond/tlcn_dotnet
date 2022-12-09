using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Dto.CartDto;

namespace tlcn_dotnet.IServices
{
    public interface ICartService
    {
        public Task<DataResponse> PayCurrentCart(string authorization, CartPaymentDto cartPaymentDto);
        public Task<DataResponse> ProcessCart(string authorization, long id, ProcessCartDto processCartDto);
        public Task<DataResponse> GetCartHistory(string authorization, string? strStatus, string? strPaymentMethod,
            string? strFromDate, string? strToDate, string? strFromTotal, 
            string? strToTotal, string? sortBy, string? order, string? strPage = "1", string? strPageSize = "5");
        public Task<DataResponse> CancelCart(string authorization, long id);
        public Task<DataResponse> ManageCart(RequestFilterProcessCartDto requestFilterProcessCartDto);
    }
}
