using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DTOs.DefaultDescription;
using WebApplication1.Services;

namespace WebApplication1.Pages.DefaultDescription
{
    public class CreateModel : PageModel
    {
        private readonly IDefaultDescriptionService _service;

        public CreateModel(IDefaultDescriptionService service)
        {
            _service = service;
        }

        [BindProperty]
        public CreateDefaultDescriptionDto Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _service.CreateAsync(Input);

            return RedirectToPage("/DefaultDescription/Index");
        }
    }
}
