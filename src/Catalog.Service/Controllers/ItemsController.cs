using Catalog.Contacts;
using Catalog.Service.Dtos;
using Catalog.Service.Entities;
using Catalog.Service.Service;
using Common;
using MassTransit;
// using Catalog.Service.Service;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {

        private readonly IPublishEndpoint publishEndpoint;
        private static int requestCounter = 0;
        private readonly IRepository<Item> itemsRepository;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> Get()
        {


            var items = (await itemsRepository.GetAllAsync()).Select(item => item.AsDto());
            Console.WriteLine($"Request {requestCounter} : 200 (Ok)...");
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return item.AsDto();

        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreatItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await itemsRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existItem = await itemsRepository.GetAsync(id);
            if (existItem == null)
            {
                return NotFound();
            }

            existItem.Name = updateItemDto.Name;
            existItem.Description = updateItemDto.Description;
            existItem.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existItem);

            await publishEndpoint.Publish(new CatalogItemUpdate(existItem.Id, existItem.Name, existItem.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            await itemsRepository.RemoveAsync(item.Id);

            await publishEndpoint.Publish(new CatalogItemDelete(id));


            return NoContent();
        }
    }
}