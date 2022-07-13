using Common;
using Inventory.Service.Clients;
using Inventory.Service.Dtos;
using Inventory.Service.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;


        public ItemsController(IRepository<InventoryItem> itemsRepository, IRepository<CatalogItem> catalogItemsRepository = null)
        {
            this.itemsRepository = itemsRepository;

            this.catalogItemsRepository = catalogItemsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }


            var inventoryItemEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
            var catalogItemEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));


            var inventoryItemDto = inventoryItemEntities.Select(item =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == item.CatalogItemId);
                return item.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDto);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(GrantItemDto granItemsDto)
        {
            var inventoryItem = await itemsRepository.GetAsync(item => item.UserId == granItemsDto.UserId && item.CatalogItemId == granItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = granItemsDto.CatalogItemId,
                    UserId = granItemsDto.UserId,
                    Quantity = granItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += granItemsDto.Quantity;
                await itemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}