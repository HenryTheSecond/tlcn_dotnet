using tlcn_dotnet.Dto.CartDetailDto;

namespace tlcn_dotnet.IServices
{
    public interface ICartService
    {
        public Task<DataResponse> PayCurrentCart(string authorization, CartPaymentDto cartPaymentDto);
    }
}
