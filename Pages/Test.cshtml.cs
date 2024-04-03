using Microsoft.AspNetCore.Mvc.RazorPages;

public class TestModel: PageModel
{
    public void OnPost()
    {
        Console.WriteLine("This works?");
    }
};