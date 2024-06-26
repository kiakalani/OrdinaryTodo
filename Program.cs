using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext> (options => {
    options.UseSqlite(builder.Configuration.GetConnectionString("TodoDB"));
});

var app = builder.Build();
AppDbContext db = app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
new TodoAdditions(app, db);
app.Run();
