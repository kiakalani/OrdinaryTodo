using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Newtonsoft.Json;

public class HomeViewModel
{
    public List<string> Tasks { get; set; } = new List<string>();
    public List<string> Lists { get; set; } = new List<string>();
    public string? NewTask { get; set; }
    public string? NewList { get; set; }
}

public class HomePageModel : PageModel
{
    [BindProperty]
    public HomeViewModel ViewModel { get; set; } = new HomeViewModel();

    public void OnGet()
    {
        // Initialize ViewModel.Tasks and ViewModel.Lists from TempData
        ViewModel.Tasks = GetListFromTempData("Tasks");
        ViewModel.Lists = GetListFromTempData("Lists");
        TempData.Keep("Tasks");
        TempData.Keep("Lists");
    }

    public IActionResult OnPostAddTask()
    {
        if (!string.IsNullOrWhiteSpace(ViewModel.NewTask))
        {
            ViewModel.Tasks.Add(ViewModel.NewTask);
            // Save updated list to TempData
            TempData["Tasks"] = JsonConvert.SerializeObject(ViewModel.Tasks);
            ViewModel.NewTask = string.Empty;
        }
        return RedirectToPage();
    }

    public IActionResult OnPostAddList()
    {
        if (!string.IsNullOrWhiteSpace(ViewModel.NewList))
        {
            ViewModel.Lists.Add(ViewModel.NewList);
            // Save updated list to TempData
            TempData["Lists"] = JsonConvert.SerializeObject(ViewModel.Lists);
            ViewModel.NewList = string.Empty;
        }
        return RedirectToPage();
    }

    private List<string> GetListFromTempData(string key)
    {
        var json = TempData[key] as string;
        TempData.Keep(key);
        if (!string.IsNullOrEmpty(json))
        {
          return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
        }
        return new List<string>();
    }
}
