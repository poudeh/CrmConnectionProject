using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs.WarehouseLocation;
using WebApplication1.Services;

namespace WebApplication1.Pages.WarehouseLocation;

public class IndexModel : PageModel
{
    private readonly IWarehouseLocationservice _warehouseLocationservice;
    public List<WarehouseLocationViewModel> Items { get; set; } = new();

    public IndexModel(IWarehouseLocationservice warehouseLocationservice)
    {
        _warehouseLocationservice = warehouseLocationservice;
    }
    public async Task OnGetAsync()
    {
        Items = await _warehouseLocationservice.GetAllAsync(SavedType.CRM);
    }
    public async Task<IActionResult> OnGetDeleteAsync(string code)
    {
        await _warehouseLocationservice.DeleteAsync(code);
        return RedirectToPage("/WarehouseLocation/Index");
    }
}

