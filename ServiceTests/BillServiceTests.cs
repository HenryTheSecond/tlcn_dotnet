using AutoMapper;
using Moq;
using tlcn_dotnet;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Mapper;
using tlcn_dotnet.Services;

namespace ServiceTests
{
    public class BillServiceTests
    {
        private readonly Mock<IBillRepository> _billRepository = new Mock<IBillRepository>();
        private readonly Mock<IBillDetailRepository> _billDetailRepository = new Mock<IBillDetailRepository>();
        private readonly IMapper _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<ManageMapper>()));
        private readonly Mock<IPaymentService> _paymentService = new Mock<IPaymentService>();
        private readonly Mock<IProductPromotionRepository> _productPromotionRepository = new Mock<IProductPromotionRepository>();
        private readonly IBillService _billService;
        public BillServiceTests()
        {
            _billService = new BillService(_billRepository.Object, _billDetailRepository.Object,
                _mapper, _paymentService.Object, _productPromotionRepository.Object);
        }

        [Fact]
        public void BillPaying_BillNotFound_ShouldThrowException()
        {
            _billRepository.Setup(x => x.UpdatePurchaseDate(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns<Bill>(null);

            Assert.ThrowsAsync<GeneralException>(() => _billService.BillPaying(0));
        }

        [Fact]
        public async void BillPaying_FoundBill_ShouldReturnData()
        {
            //Arrange
            Bill bill = new Bill
            {
                Id = 0,
                OrderCode = "abc",
                PaymentMethod = tlcn_dotnet.Constant.PaymentMethod.MOMO,
                Total = 100
            };
            _billRepository.Setup(x => x.UpdatePurchaseDate(It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(bill);

            //Act
            var result = (await _billService.BillPaying(It.IsAny<long>())).Data as SimpleBillDto;

            //Assert
            Assert.Equal(bill.Total, result?.Total);
        }
    }
}