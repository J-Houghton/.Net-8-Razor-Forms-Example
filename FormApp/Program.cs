using Microsoft.AspNetCore.CookiePolicy; 
using Microsoft.AspNetCore.Mvc; 
using System.Threading.RateLimiting;
//need: logs, db set up, secure TempData?
var builder = WebApplication.CreateBuilder(args);

//---------------------------------------
// SERVICE CONFIGURATION
//---------------------------------------

// Razor Pages configuration
// While Razor Pages does include anti-forgery token validation by default,
// this explicit configuration provides a valid anti-forgery token check for manually forms or JS submit requests. 
builder.Services.AddRazorPages(options =>
{
    options.Conventions.ConfigureFilter(new AutoValidateAntiforgeryTokenAttribute()); 
}); 

// Security configurations
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443;
});

// Rate limiting configuration
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100, // 100 requests
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Cookie configurations
builder.Services.Configure<CookieTempDataProviderOptions>(options => {
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.MaxAge = TimeSpan.FromHours(2);
});
builder.Services.Configure<CookiePolicyOptions>(options => {
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.SameAsRequest;
});

// Add UserInputValidator as a service, if more complex validation and santization are required
//builder.Services.AddScoped<IUserInputValidator, UserInputValidator>();

//---------------------------------------
// APPLICATION CONFIGURATION
//---------------------------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


// Error handling
// any route not matched by endpoints will go to the Error page
app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}"); 

// Add security headers middleware
//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
//    context.Response.Headers.Add("X-Frame-Options", "DENY");
//    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
//    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

//    await next();
//});

// Middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
