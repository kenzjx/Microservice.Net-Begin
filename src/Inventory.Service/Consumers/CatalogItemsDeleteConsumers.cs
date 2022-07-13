using Catalog.Contacts;
using Common;
using Inventory.Service.Entities;
using MassTransit;

namespace Inventory.Service.Consumers
{
    public class CatalogItemDeleteConsumers : IConsumer<CatalogItemDelete>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemDeleteConsumers(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemDelete> context)
        {
            var message = context.Message;

            var item = repository.GetAsync(message.ItemId);

            if (item == null)
            {
                return;
            }

            await repository.RemoveAsync(message.ItemId);
            
        }
    }
}