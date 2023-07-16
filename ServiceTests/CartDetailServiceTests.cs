using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Mapper;
using tlcn_dotnet.Services;

namespace ServiceTests
{
    public class CartDetailServiceTests
    {
        private readonly Mock<ICartDetailRepository> _cartDetailRepository = new Mock<ICartDetailRepository>();
        private readonly Mock<IProductRepository> _productRepository = new Mock<IProductRepository>();
        private readonly IMapper _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<ManageMapper>()));
        private readonly Mock<IProductPromotionRepository> _productPromotionRepository = new Mock<IProductPromotionRepository>();
        private readonly ICartDetailService _cartDetailService;
        public CartDetailServiceTests()
        {
            _cartDetailService = new CartDetailService(_cartDetailRepository.Object, _productRepository.Object,
                _mapper, _productPromotionRepository.Object);
        }

        private static readonly string TOKEN = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI0NyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6InVzZXJAZXhhbXBsZS5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJST0xFX0VNUExPWUVFIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIwOTY0MTQ3NzU3Iiwic3RhdHVzIjoiQUNUSVZFIiwiY2l0eUlkIjoiMDEiLCJkaXN0cmljdElkIjoiMDAyIiwid2FyZElkIjoiMDAwNDAiLCJkZXRhaWxMb2NhdGlvbiI6ImFhYWEiLCJ2ZXJpZnlUb2tlbiI6ImEzOGIyNDNjLWM1MGYtNGU1Mi1iNjcxLTVlMDdiZmRhMTRmNSIsImZpcnN0TmFtZSI6IlRo4buNIiwibGFzdE5hbWUiOiJUcuG7i25oIiwicGhvdG9VcmwiOiJodHRwOi8vcmVzLmNsb3VkaW5hcnkuY29tL2RpaGc3MmV6OC9pbWFnZS91cGxvYWQvdjE2NzI4MDE0MTMvdXNlcl9waG90b180Ny5qcGciLCJleHAiOjE2ODk0MjA4MzUsImlzcyI6IkZydWl0U2hvcCJ9.IAns9CNwXLWz_WrhqGLZPR9BILmiXG9ZEMC0Aw2MKymgqaIB-Sm0d99U27WnYH6Jl7gw17vNWAE-xhyulSSAWw"; 

        [Fact]
        public void AddCartDetail_ProductNotFound_ShouldThrowException()
        {
            _productRepository.Setup(x => x.GetById(It.IsAny<long>()))
                .Returns<Product>(null);

            try
            {
                _cartDetailService.AddCartDetail(TOKEN, It.IsAny<AddCartDetailRequest>());
            }
            catch(GeneralException ex)
            {
                Assert.Equal("PRODUCT NOT FOUND", ex.Message);
            }
        }

        [Fact]
        public void AddCartDetail_ProductIsDeleted_ShouldThrowException()
        {
            _productRepository.Setup(x => x.GetById(It.IsAny<long>()))
                .ReturnsAsync(new Product { IsDeleted = true});

            try
            {
                _cartDetailService.AddCartDetail(TOKEN, It.IsAny<AddCartDetailRequest>());
            }
            catch (GeneralException ex)
            {
                Assert.Equal("PRODUCT NOT FOUND", ex.Message);
            }
        }

        [Fact]
        public void AddCartDetail_ProductIsUnSold_ShouldThrowException()
        {
            _productRepository.Setup(x => x.GetById(It.IsAny<long>()))
                .ReturnsAsync(new Product { Status = tlcn_dotnet.Constant.ProductStatus.UNSOLD });

            try
            {
                _cartDetailService.AddCartDetail(TOKEN, It.IsAny<AddCartDetailRequest>());
            }
            catch (GeneralException ex)
            {
                Assert.Equal("PRODUCT IS UNSOLD", ex.Message);
            }
        }

        [Fact]
        public void AddCartDetail_QuantityIsNotEnough_ShouldThrowException()
        {
            _productRepository.Setup(x => x.GetById(It.IsAny<long>()))
                .ReturnsAsync(new Product { Quantity = 1 });

            try
            {
                _cartDetailService.AddCartDetail(TOKEN, new AddCartDetailRequest { Quantity = 2});
            }
            catch (GeneralException ex)
            {
                Assert.Equal("QUANTITY IS NOT ENOUGH", ex.Message);
            }
        }

        [Fact]
        public async void AddCartDetail_CheckCurrentCartHavingProductReturn0_ShouldAdd()
        {
            _productRepository.Setup(x => x.GetById(It.IsAny<long>()))
               .ReturnsAsync(new Product
               {
                   IsDeleted = false,
                   Status = tlcn_dotnet.Constant.ProductStatus.SELLING,
                   Unit = tlcn_dotnet.Constant.ProductUnit.UNIT,
                   MinPurchase = 1,
                   Quantity = 100
               });
            _cartDetailRepository.Setup(x => x.CheckCurrentCartHavingProduct(It.IsAny<long>(),
                It.IsAny<long>(), It.IsAny<long?>()))
                .ReturnsAsync(0);

            _cartDetailRepository.Setup(x => x.AddCartDetail(It.IsAny<CartDetail>()))
                .ReturnsAsync(new CartDetail
                {
                    Id = 1,
                    ProductId = 1
                });
            _productPromotionRepository.Setup(x => x.GetPromotionByProductId(It.IsAny<long>()))
                .ReturnsAsync(new ProductPromotion());

            var result = (await _cartDetailService.AddCartDetail(TOKEN, new AddCartDetailRequest
            {
                Quantity = 1,
            })).Data as CartDetailResponse;

            Assert.Equal(1, result?.Id);
        }

        [Fact]
        public void DeleteCartDetail_NoAffectedRow_ShouldThrowException()
        {
            _cartDetailRepository.Setup(x => x.DeleteCartDetailByIdAndAccountId(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(0);

            Assert.ThrowsAsync<GeneralException>(() => _cartDetailService.DeleteCartDetail(TOKEN, It.IsAny<long>()));;
        }

        [Fact]
        public async void DeleteCartDetail_ShouldReturnTrue()
        {
            _cartDetailRepository.Setup(x => x.DeleteCartDetailByIdAndAccountId(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(1);

            var result = (bool)(await _cartDetailService.DeleteCartDetail(TOKEN, It.IsAny<long>())).Data;

            Assert.True(result);
        }

        [Fact]
        public async void UpdateCartDetailQuantity_CartDetailNotFound_ShouldReturnNotFoundMessage()
        {
            _cartDetailRepository.Setup(x => x.FindByIdAndAccountId(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync((CartDetail)null);

            try
            {
                await _cartDetailService.UpdateCartDetailQuantity(TOKEN, new UpdateCartDetailQuantityDto { Id = 1 });
            }
            catch(GeneralException ex)
            {
                Assert.Equal("CART DETAIL NOT FOUND", ex.Message);
            }
        }
    }
}
