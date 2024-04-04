using Microsoft.AspNetCore.SignalR;

public abstract class RestRouter
{

    private string baseUrl;
    protected AppDbContext db {get;}
    protected class RouteMethod{
        private readonly string method;
        public string Method {
            get {return method;}
        }
        private readonly Delegate del;
        public Delegate Del {
            get {return this.del;}
        }
        public RouteMethod(string method, Delegate del) {
            this.method = method;
            this.del = del;
        }
        
    }
    public RestRouter(string baseName, WebApplication app, AppDbContext db)
    {
        this.baseUrl = baseName;
        this.db = db;
        Dictionary<string, RouteMethod> pages = setupRoutes();
        foreach(string page in pages.Keys) {
            string url = baseUrl + '/' + page;
            RouteMethod m = pages[page];
            if (m.Method.Equals("GET")) {
                app.MapGet(url, m.Del);
            }
            else if (m.Method.Equals("POST"))
            {
                app.MapPost(url, m.Del);
            }
        }
    }
    abstract protected Dictionary<string, RouteMethod> setupRoutes();

}