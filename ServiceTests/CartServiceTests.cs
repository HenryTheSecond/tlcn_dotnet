using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet;
using Moq;
using tlcn_dotnet.Mapper;
using tlcn_dotnet.Services;
using tlcn_dotnet.Entity;
using tlcn_dotnet.CustomException;

namespace ServiceTests
{
    public class CartServiceTests
    {
        private static readonly string TOKEN = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI0NyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6InVzZXJAZXhhbXBsZS5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJST0xFX0VNUExPWUVFIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIwOTY0MTQ3NzU3Iiwic3RhdHVzIjoiQUNUSVZFIiwiY2l0eUlkIjoiMDEiLCJkaXN0cmljdElkIjoiMDAyIiwid2FyZElkIjoiMDAwNDAiLCJkZXRhaWxMb2NhdGlvbiI6ImFhYWEiLCJ2ZXJpZnlUb2tlbiI6ImEzOGIyNDNjLWM1MGYtNGU1Mi1iNjcxLTVlMDdiZmRhMTRmNSIsImZpcnN0TmFtZSI6IlRo4buNIiwibGFzdE5hbWUiOiJUcuG7i25oIiwicGhvdG9VcmwiOiJodHRwOi8vcmVzLmNsb3VkaW5hcnkuY29tL2RpaGc3MmV6OC9pbWFnZS91cGxvYWQvdjE2NzI4MDE0MTMvdXNlcl9waG90b180Ny5qcGciLCJleHAiOjE2ODk0MjA4MzUsImlzcyI6IkZydWl0U2hvcCJ9.IAns9CNwXLWz_WrhqGLZPR9BILmiXG9ZEMC0Aw2MKymgqaIB-Sm0d99U27WnYH6Jl7gw17vNWAE-xhyulSSAWw";

        private readonly Mock<ICartDetailRepository> _cartDetailRepository = new();
        private readonly Mock<IBillService> _billService= new();
        private readonly Mock<ICartRepository> _cartRepository = new();
        private readonly IMapper _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<ManageMapper>()));
        private readonly Mock<IDeliveryService> _deliveryService = new();
        private readonly Mock<IBillRepository> _billRepository = new();
        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly Mock<ICartNotificationService> _cartNotificationService = new();
        private readonly MyDbContext _dbContext;
        private readonly Mock<IGiftCartRepository> _giftCartRepository = new();
        private readonly Mock<ISmsService> _smsService = new();
        private readonly ICartService _cartService;
        public CartServiceTests()
        {
            _cartService = new CartService(_dbContext, _cartDetailRepository.Object,
                _billService.Object, _cartRepository.Object, _mapper, _deliveryService.Object, _billRepository.Object,
                _productRepository.Object, _cartNotificationService.Object, _giftCartRepository.Object, _smsService.Object);
        }

        [Fact]
        public async void CancelCart_CartNotFound_ShouldThrowException()
        {
            _cartRepository.Setup(x => x.GetById(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult<Cart>(null));

            Assert.ThrowsAsync<GeneralException>(() => _cartService.CancelCart(TOKEN, It.IsAny<long>()));
        }

        [Fact]
        public async void CancelCart_CartDeliveried_ShouldThrowException()
        {
            _cartRepository.Setup(x => x.GetById(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult<Cart>(new Cart { Status = tlcn_dotnet.Constant.CartStatus.DELIVERIED}));

            Assert.ThrowsAsync<GeneralException>(() => _cartService.CancelCart(TOKEN, It.IsAny<long>()));
        }

        [Fact]
        public async void CancelCart_CartCanceld_ShouldThrowException()
        {
            _cartRepository.Setup(x => x.GetById(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult<Cart>(new Cart { Status = tlcn_dotnet.Constant.CartStatus.CANCELLED }));

            Assert.ThrowsAsync<GeneralException>(() => _cartService.CancelCart(TOKEN, It.IsAny<long>()));
        }

        [Fact]
        public async void DeleteCurrentCart_AfftedRowReturn0_ShouldThrowException()
        {
            _cartRepository.Setup(x => x.DeleteCurrentCart(It.IsAny<long>()))
                .Returns(Task.FromResult(0));

            Assert.ThrowsAsync<GeneralException>(() => _cartService.DeleteCurrentCart(TOKEN));
        }
    }
}
