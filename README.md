# ASP.NET Mvc Tracking

```
 protected void Application_BeginRequest(object sender, EventArgs e)
 {
     //if (User.Identity.IsAuthenticated)
     Context.WriteAccessLog("12345");
 }
 ```
