namespace Catalog.Service.Dtos
{
    public record ItemDto(Guid Id, string Name,
    string Description, decimal Price, DateTimeOffset CreatedDate);

    public record CreatItemDto(string Name, string Description, decimal Price);

    public record UpdateItemDto(string Name, string Description, decimal Price);

}