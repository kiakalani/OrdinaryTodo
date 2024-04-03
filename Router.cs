using Microsoft.AspNetCore.SignalR;

abstract class RestRouter
{

    private string baseUrl;
    protected class RouteMethod{
        private string method;
        public string Method {
            get {return method;}
        }
        private Delegate del;
        public Delegate Del {
            get {return this.del;}
        }
        public RouteMethod(string method, Delegate del) {
            this.method = method;
            this.del = del;
        }
        
    }
    public RestRouter(string baseName, WebApplication app)
    {
        this.baseUrl = baseName;
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