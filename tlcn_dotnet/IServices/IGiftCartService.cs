using tlcn_dotnet.Dto.GiftCartDto;

namespace tlcn_dotnet.IServices
{
    public interface IGiftCartService
    {
        public Task<DataResponse> CreateGiftCart(string authorization, GiftCartCreateRequest request);
        public Task<DataResponse> GetAllActiveGiftCart(string authorization, string keyword);
        public Task<DataResponse> DeleteGiftCart(string authorization, long id);
        public Task<DataResponse> ChangeGiftCartName(string authorization, long id, GiftCartCreateRequest request);
    }
}
