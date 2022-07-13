using Common;

namespace Catalog.Service.Entities
{

    public class Item : IEntity
    {
        public Guid Id { set; get; }

        public string Name { set; get; }

        public string Description { get; set; }

        public decimal Price { set; get; }

        public DateTimeOffset CreatedDate { set; get; }
    }
}