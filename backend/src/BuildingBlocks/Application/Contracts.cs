namespace JewelryStore.BuildingBlocks.Application;

public interface IProductExistence
{
    Task<bool> Exists(Guid productId, CancellationToken cancellationToken);
}

public interface IImageStorage
{
    Task<string> UploadProductImage(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);
}

