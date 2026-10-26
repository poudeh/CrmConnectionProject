using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DTOs.WarehouseLocation;
using WebApplication1.Services;

namespace WebApplication1.Pages.WarehouseLocation
{
    public class CreateModel : PageModel
    {
        private readonly IWarehouseLocationservice _service;

        public CreateModel(IWarehouseLocationservice service)
        {
            _service = service;
        }

        [BindProperty]
        public CreateWarehouseLocationDto Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _service.CreateAsync(Input);

            return RedirectToPage("/WarehouseLocation/Index");
        }
    }
}
