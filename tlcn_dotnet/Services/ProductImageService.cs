using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Principal;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.Services;
using static System.Net.Mime.MediaTypeNames;

namespace tlcn_dotnet.ServicesImpl
{
    public class ProductImageService: IProductImageService
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly IProductImageRepository _productImageRepository;

        public ProductImageService(MyDbContext dbContext, IMapper mapper, IConfiguration configuration, IProductImageRepository productImageRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productImageRepository = productImageRepository;
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

        public async Task<IEnumerable<SimpleProductImageDto>> EditProductImage(Product product, IList<ProductImageEditStatus> editStatus, IFormFileCollection files)
        {
            var productImages = await _productImageRepository.GetProductImageByProductId(product.Id);
            int indexFiles = 0;

            IList<Task<ProductImage>> listEditTasks = new List<Task<ProductImage>>();
            IList<Task<ProductImage>> listAddTasks = new List<Task<ProductImage>>();
            for (int i = 0; i < productImages.Count; i++)
            {
                switch (editStatus[i])
                {
                    case ProductImageEditStatus.DELETE:
                        DeleteSingleProductImage(productImages[i]);
                        _dbContext.ProductImage.Remove(productImages[i]);
                        productImages.RemoveAt(i);
                        editStatus.RemoveAt(i);
                        i--;
                        break;
                    case ProductImageEditStatus.EDIT:
                        try
                        {
                            listEditTasks.Add(EditSingleProductImage(productImages[i], files[indexFiles]));
                            indexFiles++;
                        }
                        catch (ArgumentOutOfRangeException e) //Throw if image file index is out of range
                        {
                            continue;
                        }
                        break;
                    case ProductImageEditStatus.NONE:
                        continue;
                }
            }
            //Add all the remaining images
            for (; indexFiles < files.Count; indexFiles++)
            {
                listAddTasks.Add(AddSingleProductImage(files[indexFiles], product));
            }
            await Task.WhenAll(listEditTasks);
            productImages.AddRange(Task.WhenAll<ProductImage>(listAddTasks).Result);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<IEnumerable<SimpleProductImageDto>>(productImages);
        }

        private async Task DeleteSingleProductImage(ProductImage imageDb)
        {
            var deletionParams = new DeletionParams(imageDb.FileName);
            _cloudinary.DestroyAsync(deletionParams);
        }
        private async Task<ProductImage> EditSingleProductImage(ProductImage imageDb, IFormFile imageUpload)
        {
            using (var ms = new MemoryStream())
            {
                await imageUpload.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                ImageUploadParams uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageDb.FileName, ms),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };
                ImageUploadResult result = await _cloudinary.UploadAsync(uploadParams);
                imageDb.Url = result.Url.ToString();
                return imageDb;
            }
        }

        public async Task<IEnumerable<SimpleProductImageDto>> GetImageByProduct(long? productId)
        {
            return _mapper.Map<IEnumerable<SimpleProductImageDto>>(
                await _productImageRepository.GetProductImageByProductId(productId));
        }

        private async Task<ProductImage> AddSingleProductImage(IFormFile image, Product product)
        {
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                string fileName = Guid.NewGuid().ToString();
                ImageUploadParams uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, ms),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };
                ImageUploadResult result = await _cloudinary.UploadAsync(uploadParams);
                ProductImage productImage = new ProductImage()
                {
                    Url = result.Url.ToString(),
                    Product = product,
                    FileName = fileName
                };
                return (await _dbContext.ProductImage.AddAsync(productImage)).Entity;
            }
        }

        public async Task DeleteAllImageOfProduct(long? id)
        {
            var images = await _productImageRepository.GetProductImageByProductId(id);
            images.ForEach(image =>
            {
                _dbContext.Remove(image);
                DeleteSingleProductImage(image);
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
