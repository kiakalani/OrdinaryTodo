using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TasksPageModel : PageModel
{
  [BindProperty]
  public string NewTask { get; set; }
  
  public List<string> Tasks
    {
      get
      {
        // use TempData to store the tasks list
        var tasksJson = TempData["Tasks"] as string;
        if (tasksJson != null)
        {
          // Deseriealize TempData to read the tasks list
          return JsonConvert.DeserializeObject<List<string>>(tasksJson);
        }
        return new List<string>();
      }
      set
      {
        // Serialize the list to a JSON string ot store it in TempData
        TempData["Tasks"] = JsonConvert.SerializeObject(value);
      }
    }

  public void OnGet()
  {
    TempData.Keep("Tasks");
  }

  public IActionResult OnPostAddTask()
  {
      if (!string.IsNullOrWhiteSpace(NewTask))
      {
        var tasks = Tasks; 
        tasks.Add(NewTask);
        Tasks = tasks;
        NewTask = string.Empty;  
      }
      return RedirectToPage();
  }
}
