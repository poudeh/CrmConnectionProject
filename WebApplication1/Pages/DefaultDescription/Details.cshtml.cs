
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DTOs.DefaultDescription;
using WebApplication1.Services;

namespace WebApplication1.Pages.DefaultDescription
{
    public class DetailsModel : PageModel
    {
        private readonly IDefaultDescriptionService _service;

        public DetailsModel(IDefaultDescriptionService service)
        {
            _service = service;
        }

        public DefaultDescriptionViewModel? Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Item = await _service.GetByIdAsync(id);

            if (Item == null)
                return NotFound();

            return Page();
        }
    }
}
