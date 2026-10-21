using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DTOs.DefaultDescription;
using WebApplication1.Services;

namespace WebApplication1.Pages.DefaultDescription
{
    public class EditModel : PageModel
    {
        private readonly IDefaultDescriptionService _service;

        public EditModel(IDefaultDescriptionService service)
        {
            _service = service;
        }

        [BindProperty]
        public UpdateDefaultDescriptionDto Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _service.GetByIdAsync(id);

            if (item == null)
                return NotFound();

            Input = new UpdateDefaultDescriptionDto
            {
                Id = item.Id,
                Owner = item.Owner,
                Name = item.Name,
                Code = item.Code
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var updated = await _service.UpdateAsync(Input);

            if (updated == null)
                return NotFound();

            return RedirectToPage("/DefaultDescription/Index");
        }
    }
}
