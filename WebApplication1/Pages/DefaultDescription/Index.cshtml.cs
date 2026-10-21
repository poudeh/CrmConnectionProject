using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DTOs.DefaultDescription;
using WebApplication1.Services;
namespace WebApplication1.Pages.DefaultDescription;

public class IndexModel : PageModel
{
    private readonly IDefaultDescriptionService _defaultDescriptionService;
    public List<DefaultDescriptionViewModel> Items { get; set; } = new();

    public IndexModel(IDefaultDescriptionService defaultDescriptionService)
    {
        _defaultDescriptionService = defaultDescriptionService;
    }
    public async Task OnGetAsync()
    {
        Items = await _defaultDescriptionService.GetAllAsync();
    }
    public async Task<IActionResult> OnGetDeleteAsync(string code)
    {
        await _defaultDescriptionService.DeleteAsync(code);
        return RedirectToPage("/DefaultDescription/Index");
    }
}

