using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Dto.CartDto;

namespace tlcn_dotnet.IServices
{
    public interface ICartService
    {
        public Task<DataResponse> PayCurrentCart(string authorization, CartPaymentDto cartPaymentDto);
        public Task<DataResponse> ProcessCart(string authorization, long id, ProcessCartDto processCartDto);
    }
}
