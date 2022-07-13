using Common;

namespace Inventory.Service.Entities
{
    public class InventoryItem : IEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { set; get; }

        public Guid CatalogItemId { set; get; }

        public int Quantity { set; get; }

        public DateTimeOffset AcquiredDate { set; get; }
    }
}