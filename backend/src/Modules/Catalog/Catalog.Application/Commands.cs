using JewelryStore.BuildingBlocks.Application;
using JewelryStore.BuildingBlocks.Domain;
using JewelryStore.Modules.Catalog.Domain;

namespace JewelryStore.Modules.Catalog.Application;

public sealed record CreateCategoryCommand(string Name) : ICommand<Guid>;

public sealed class CreateCategoryHandler(
    ICategoryRepository categories,
    ICatalogUnitOfWork unitOfWork) : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = Category.Create(command.Name, DateTimeOffset.UtcNow);
        if (await categories.SlugExists(category.Slug, null, cancellationToken))
        {
            throw new DomainException("Ya existe una categoría con ese nombre.");
        }

        await categories.Add(category, cancellationToken);
        await unitOfWork.SaveChanges(cancellationToken);
        return category.Id;
    }
}

public sealed record CreateProductCommand(
    string Name,
    string Description,
    Guid CategoryId,
    decimal Price,
    int Stock,
    string? ImageUrl) : ICommand<Guid>;

public sealed class CreateProductHandler(
    IProductRepository products,
    ICategoryRepository categories,
    ICatalogUnitOfWork unitOfWork) : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (await categories.Get(command.CategoryId, cancellationToken) is null)
        {
            throw new KeyNotFoundException("La categoría seleccionada no existe.");
        }

        var product = Product.Create(
            command.Name,
            command.Description,
            command.CategoryId,
            command.Price,
            command.Stock,
            command.ImageUrl,
            DateTimeOffset.UtcNow);

        await products.Add(product, cancellationToken);
        await unitOfWork.SaveChanges(cancellationToken);
        return product.Id;
    }
}

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId,
    decimal Price,
    int Stock,
    string? ImageUrl,
    bool IsActive) : ICommand<bool>;

public sealed class UpdateProductHandler(
    IProductRepository products,
    ICategoryRepository categories,
    ICatalogUnitOfWork unitOfWork) : ICommandHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await products.Get(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException("El producto no existe.");

        if (await categories.Get(command.CategoryId, cancellationToken) is null)
        {
            throw new KeyNotFoundException("La categoría seleccionada no existe.");
        }

        product.Update(
            command.Name,
            command.Description,
            command.CategoryId,
            command.Price,
            command.Stock,
            command.ImageUrl,
            command.IsActive,
            DateTimeOffset.UtcNow);

        await unitOfWork.SaveChanges(cancellationToken);
        return true;
    }
}

public sealed record ArchiveProductCommand(Guid Id) : ICommand<bool>;

public sealed class ArchiveProductHandler(
    IProductRepository products,
    ICatalogUnitOfWork unitOfWork) : ICommandHandler<ArchiveProductCommand, bool>
{
    public async Task<bool> Handle(ArchiveProductCommand command, CancellationToken cancellationToken)
    {
        var product = await products.Get(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException("El producto no existe.");

        product.Archive(DateTimeOffset.UtcNow);
        await unitOfWork.SaveChanges(cancellationToken);
        return true;
    }
}

