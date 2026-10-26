using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.DTOs.WarehouseLocation;
using WebApplication1.Infrastructure.Apis;
using WebApplication1.Services;

namespace WebApplication1.Pages.WarehouseLocation;
public class DirectCrmEditModel : PageModel
{
    [BindProperty]
    public UpdateWarehouseLocationDto UpdateWarehouseLocation { get; set; } = new();
    private readonly ICrmApiWarehouseLocation _apiWarehouseLocation;
    private readonly IWarehouseLocationservice _warehouseLocationService;

    public DirectCrmEditModel(ICrmApiWarehouseLocation apiWarehouseLocation, IWarehouseLocationservice warehouseLocationService)
    {
        _apiWarehouseLocation = apiWarehouseLocation;
        _warehouseLocationService = warehouseLocationService;
    }

    public async Task OnGet(string code)
    {
        UpdateWarehouseLocation = await _apiWarehouseLocation.GetWarehouseLocationByCode(code);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();
        await _warehouseLocationService.UpdateAsync(UpdateWarehouseLocation);
        return RedirectToPage("/WarehouseLocation/Index");

    }
}

