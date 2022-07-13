namespace Catalog.Contacts
{
    public record CatalogItemCreated(Guid ItemId, string Name, string Description);

    public record CatalogItemUpdate(Guid ItemId, string Name, string Description);

    public record CatalogItemDelete(Guid ItemId);
}