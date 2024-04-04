using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.Json;
[PrimaryKey(nameof(name), nameof(for_category))]
public class TodoItem
{
    public enum Status {
        STARTED, COMPLETED, IN_PROGRESS, OVERDUE, TODO
    }

    public enum Importance
    {
        LOW, MEDIUM, HIGH
    }
    public string for_category {get; set;}
    public string name {get; set;}
    public UInt64 due_date {get; set;}
    public Status status {get; set;}
    public Importance importance {get; set;} 

    public TodoItem(string name, string for_category, UInt64 due_date, Status status, Importance importance)
    {
        this.name = name;
        this.for_category = for_category;
        this.due_date = due_date;
        this.status = status;
        this.importance = importance;
    }
}
[PrimaryKey(nameof(name))]
public class TodoCategory 
{
    public string name {get; set;}

    public TodoCategory(string name)
    {
        this.name = name;
    }
}
[PrimaryKey(nameof(for_item), nameof(reminder))]
public class TodoReminder
{

    public string for_item {get; set;}

    public UInt64 reminder {get; set;}

    public TodoReminder(string for_item, UInt64 reminder) {
        this.for_item = for_item;
        this.reminder = reminder;
    }
}

public class AppDbContext : DbContext
{
    public AppDbContext(){}
    public AppDbContext(DbContextOptions<AppDbContext> c): base(c)
    {

    }

    public DbSet<TodoItem> items {get; set;}
    public DbSet<TodoCategory> categories {get; set;}

    public DbSet<TodoReminder> reminders {get; set;}
}