using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Principal;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Services;

namespace tlcn_dotnet.ServicesImpl
{
    public class ProductImageServiceImpl: ProductImageService
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public ProductImageServiceImpl(MyDbContext dbContext, IMapper mapper, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            var cloudinarySection = configuration.GetSection("Cloudinary");
            string cloudName = cloudinarySection.GetSection("CloudName").Value;
            string apiKey = cloudinarySection.GetSection("ApiKey").Value;
            string apiSecret = cloudinarySection.GetSection("ApiSecret").Value;
            CloudinaryDotNet.Account cloudinaryAccount = new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(cloudinaryAccount);
            _cloudinary.Api.Secure = true;
        }

        public async Task<IEnumerable<SimpleProductImageDto>> AddProductImages(IFormFileCollection images, Product product)
        {
            var addProductImageTasks = new List<Task<ProductImage>>();
            foreach (var image in images)
            {
                addProductImageTasks.Add(AddSingleProductImage(image, product));
            }
            /*while (addProductImageTasks.Count > 0)
            {
                var finishedTask = await Task.WhenAny<SimpleProductImageDto>(addProductImageTasks);
                productImageCollection.Add(finishedTask.Result);
                addProductImageTasks.Remove(finishedTask);
            }*/
            IEnumerable<ProductImage> productImageCollection = Task.WhenAll<ProductImage>(addProductImageTasks).Result;
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<IEnumerable<SimpleProductImageDto>>(productImageCollection);
        }

        public async Task<IEnumerable<SimpleProductImageDto>> GetImageByProduct(long? productId)
        {
            return await _dbContext.ProductImage
                .Where(image => image.Product.Id == productId)
                .Select(image => _mapper.Map<SimpleProductImageDto>(image)).ToListAsync();
        }

        private async Task<ProductImage> AddSingleProductImage(IFormFile image, Product product)
        {
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                ImageUploadParams uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), ms),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };
                ImageUploadResult result = await _cloudinary.UploadAsync(uploadParams);
                ProductImage productImage = new ProductImage()
                {
                    Url = result.Url.ToString(),
                    Product = product
                };
                return (await _dbContext.ProductImage.AddAsync(productImage)).Entity;
            }
        }
    }
}
