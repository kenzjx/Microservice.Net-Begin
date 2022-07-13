using Common;

namespace Inventory.Service.Entities
{
    public class CatalogItem : IEntity
    {
        public Guid Id { get; set; }

        public string Name { set; get; }

        public string Description { get; set; }


    }
}