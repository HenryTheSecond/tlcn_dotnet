using tlcn_dotnet.Dto.CartDetailDto;

namespace tlcn_dotnet.IServices
{
    public interface ICartDetailService
    {
        public Task<DataResponse> AddCartDetail(string authorization, AddCartDetailRequest addCartDetailDto);
        public Task<DataResponse> DeleteCartDetail(string authorization, long id);
        public Task<DataResponse> UpdateCartDetailQuantity(string authorization, UpdateCartDetailQuantityDto updateCartDetailQuantityDto);
        public Task<DataResponse> GetCurrentCart(string authorization);
    }
}
