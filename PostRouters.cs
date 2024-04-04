using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using LinqToDB;

public class TodoAdditions: RestRouter {
    public TodoAdditions(WebApplication w, AppDbContext ctxt): base("/todo", w, ctxt) {
    }

    protected override Dictionary<string, RouteMethod> setupRoutes()
    {
        return new Dictionary<string, RouteMethod> {
            {"category", getCategory()},
            {"category/add", addCategory()},
            {"category/delete", deleteCategory()},
            {"item", getItems()}

        };
    }

    /// <summary>
    /// Adds a new category to the todo app
    /// </summary>
    /// <returns>a RouteMethod instance</returns>
    private RouteMethod addCategory()
    {
        return new RouteMethod("POST", (JsonElement c) => {
            var test = c.Deserialize<Dictionary<string, string>>();
            if (test != null)
            {
                if (!test.ContainsKey("category")) return Results.StatusCode(400);
                string? category = test.GetValueOrDefault("category");
                if (category != null) {
                    if (db.categories.Find(category) == null) {
                        db.categories.Add(new TodoCategory(category));
                        db.SaveChanges();
                        return Results.StatusCode(200); 
                    }
                }
            }
            return Results.StatusCode(400);
        });
    }

    /// <summary>
    /// deletes a category and all of the associated items and their reminders
    /// </summary>
    /// <returns>a RouteMethod instance</returns>
    private RouteMethod deleteCategory()
    {
        return new RouteMethod("POST", (JsonElement c) => {
            var dict = c.Deserialize<Dictionary<string, string>>();
            if (dict != null)
            {
                string? deletable = dict.GetValueOrDefault("delete");
                if (deletable != null) 
                {
                    TodoCategory? cat = db.categories.Find(deletable);
                    if (cat != null)
                    {
                        db.categories.Remove(cat);
                        // removes the items for the category cat
                        foreach (TodoItem i in db.items.Where(item => item.for_category.Equals(deletable)))
                        {
                            // removes the reminders for the item i
                            foreach(TodoReminder r in db.reminders.Where(item2 => item2.for_item.Equals(i.name) && item2.for_category.Equals(deletable)))
                            {
                                db.reminders.Remove(r);
                            }
                            db.items.Remove(i);
                        }
                        db.SaveChanges();
                        return Results.StatusCode(200);
                    }
                }
            }
            return Results.StatusCode(400);
        });
    }

    /// <summary>
    /// Get method for the categories
    /// </summary>
    /// <returns>a list of string containing all of the categories</returns>
    private RouteMethod getCategory()
    {
        return new RouteMethod("POST", () => {
            return JsonSerializer.Serialize(db.categories.Select(element => element.name));
        });
    }

    private RouteMethod getItems()
    {
        return new RouteMethod("POST", (JsonElement e) => {
            IQueryable<TodoItem>? strings = null;
            Dictionary<string, string>? dict = e.Deserialize<Dictionary<string, string>>();
            if (dict != null)
            {
                string? str = dict.GetValueOrDefault("category");
                if (str != null)
                {
                    strings = db.items.Where(el => el.for_category == str);
                }
            }
            if (strings == null) strings = db.items;
            return JsonSerializer.Serialize(strings.Select(element => new Dictionary<string, dynamic>{
                {"name", element.name},
                {"due_date", element.due_date},
                {"for_category", element.for_category},
                {"status", element.status.ToString()},
                {"importance", element.importance.ToString()}
            }));
        });
    }
}