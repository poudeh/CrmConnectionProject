
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DTOs.WarehouseLocation;
using WebApplication1.Services;

namespace WebApplication1.Pages.WarehouseLocation
{
    public class DetailsModel : PageModel
    {
        private readonly IWarehouseLocationservice _service;

        public DetailsModel(IWarehouseLocationservice service)
        {
            _service = service;
        }

        public WarehouseLocationViewModel? Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Item = await _service.GetByIdAsync(id);

            if (Item == null)
                return NotFound();

            return Page();
        }
    }
}
