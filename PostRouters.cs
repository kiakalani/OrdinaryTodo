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
            {"item", getItems()},
            {"item/add", addItem()},
            {"item/delete", deleteItem()}
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
    /// <summary>
    /// Method for adding an item to the 
    /// </summary>
    /// <returns></returns>
    private RouteMethod addItem()
    {
        return new RouteMethod("POST", (JsonElement e) => {
            Dictionary<string, string>? items = e.Deserialize<Dictionary<string, string>>();
            if (items != null)
            {
                string? for_category = items.GetValueOrDefault("for_category");
                string? name = items.GetValueOrDefault("name");
                string? due_date = items.GetValueOrDefault("due_date");
                string? status = items.GetValueOrDefault("status");
                string? importance = items.GetValueOrDefault("importance");
                if (for_category == null || name == null || due_date == null || status == null || importance == null)
                {
                    return Results.StatusCode(400);
                }
                UInt64 due = 0;
                int st = -1;
                int imp = -1;
                try {
                    due = Convert.ToUInt64(due_date!);
                    st = Convert.ToInt32(status!);
                    imp = Convert.ToInt32(importance!);
                    if (st < 0 || st > 4 || imp < 0 || imp > 2) {
                        return Results.StatusCode(400);
                    }
                } catch(FormatException)
                {
                    return Results.StatusCode(400);
                }
                if (
                    db.categories.Find(for_category) == null ||
                    db.items.Where(item => item.name.Equals(name) && item.for_category.Equals(for_category)).Count() != 0
                )
                {
                    return Results.StatusCode(400);
                }
                db.items.Add(new TodoItem(name, for_category, due, (TodoItem.Status)st, (TodoItem.Importance)imp));
                db.SaveChanges();
                return Results.StatusCode(200);

            }
            return Results.StatusCode(400);
        });
    }
    private RouteMethod deleteItem()
    {
        return new RouteMethod("POST", (JsonElement element) => {
            Dictionary<string, string>? its = element.Deserialize<Dictionary<string, string>>();
            if (its != null)
            {
                string? name = its.GetValueOrDefault("name");
                string? category = its.GetValueOrDefault("category");
                if (name != null && category != null)
                {
                    bool deleted = false;
                    foreach(TodoItem i in db.items.Where(e=>e.name.Equals(name) && e.for_category.Equals(category)))
                    {
                        db.reminders.Where(r=>r.for_item.Equals(i.name) && r.for_category.Equals(i.status)).Delete();
                        db.items.Remove(i);
                        deleted = true;
                    }
                    return Results.StatusCode(deleted ? 200 : 400);
                }
            }

            return Results.StatusCode(400);
        });
    }
}