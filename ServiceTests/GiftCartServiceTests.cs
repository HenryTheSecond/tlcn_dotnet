using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.GiftCartDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Mapper;
using tlcn_dotnet.Services;

namespace ServiceTests
{
    public class GiftCartServiceTests
    {
        private static readonly string TOKEN = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI0NyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6InVzZXJAZXhhbXBsZS5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJST0xFX0VNUExPWUVFIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIwOTY0MTQ3NzU3Iiwic3RhdHVzIjoiQUNUSVZFIiwiY2l0eUlkIjoiMDEiLCJkaXN0cmljdElkIjoiMDAyIiwid2FyZElkIjoiMDAwNDAiLCJkZXRhaWxMb2NhdGlvbiI6ImFhYWEiLCJ2ZXJpZnlUb2tlbiI6ImEzOGIyNDNjLWM1MGYtNGU1Mi1iNjcxLTVlMDdiZmRhMTRmNSIsImZpcnN0TmFtZSI6IlRo4buNIiwibGFzdE5hbWUiOiJUcuG7i25oIiwicGhvdG9VcmwiOiJodHRwOi8vcmVzLmNsb3VkaW5hcnkuY29tL2RpaGc3MmV6OC9pbWFnZS91cGxvYWQvdjE2NzI4MDE0MTMvdXNlcl9waG90b180Ny5qcGciLCJleHAiOjE2ODk0MjA4MzUsImlzcyI6IkZydWl0U2hvcCJ9.IAns9CNwXLWz_WrhqGLZPR9BILmiXG9ZEMC0Aw2MKymgqaIB-Sm0d99U27WnYH6Jl7gw17vNWAE-xhyulSSAWw";

        private readonly Mock<IGiftCartRepository> _giftCartRepository = new();
        private readonly IMapper _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<ManageMapper>()));
        private readonly IGiftCartService _giftCartService;
        public GiftCartServiceTests()
        {
            _giftCartService = new GiftCartService(_giftCartRepository.Object, _mapper);
        }

        [Fact]
        public void ChangeGiftCartName_GiftCartNotFound_ShouldThrowException()
        {
            _giftCartRepository.Setup(x => x.FindByIdAndAccountId(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult<GiftCart>(null));

            Assert.ThrowsAsync<GeneralException>(() => _giftCartService.ChangeGiftCartName(TOKEN, 0, null));
        }

        [Fact]
        public void ChangeGiftCartName_GiftCartNotActive_ShouldThrowException()
        {
            _giftCartRepository.Setup(x => x.FindByIdAndAccountId(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult<GiftCart>(new GiftCart { IsActive = false}));

            Assert.ThrowsAsync<GeneralException>(() => _giftCartService.ChangeGiftCartName(TOKEN, 0, null));
        }

        [Fact]
        public async void ChangeGiftCartName_AffectedEquals0_ShouldReturnFalse()
        {
            _giftCartRepository.Setup(x => x.FindByIdAndAccountId(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult<GiftCart>(new GiftCart { IsActive = true }));
            _giftCartRepository.Setup(x => x.UpdateGiftCartName(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(0);

            var result = (await _giftCartService.ChangeGiftCartName(TOKEN, 0, new GiftCartCreateRequest { Name = string.Empty})).Data;

            Assert.False((bool)result);
        }

        [Fact]
        public async void ChangeGiftCartName_AffectedEquals1_ShouldReturnFalse()
        {
            _giftCartRepository.Setup(x => x.FindByIdAndAccountId(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult<GiftCart>(new GiftCart { IsActive = true }));
            _giftCartRepository.Setup(x => x.UpdateGiftCartName(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            var result = (await _giftCartService.ChangeGiftCartName(TOKEN, 0, new GiftCartCreateRequest { Name = string.Empty })).Data;

            Assert.True((bool)result);
        }
    }
}
