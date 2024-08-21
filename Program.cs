using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = null; // Or remove this entirely
})
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    })

.AddTwitter(twitterOptions =>
 {
     twitterOptions.ConsumerKey = "5UrLYPb9kNqLOouf68eUReblp";
     twitterOptions.ConsumerSecret = "9TwCU7okiwZnOebK4Lscuhc8utRZXgfqcPYp6ahqihdiDRKRbd";
     twitterOptions.CallbackPath = "/signin-twitter";
     twitterOptions.AccessDeniedPath = "/signin-facebook";
 })
.AddFacebook(options =>
{
    options.AppId = "1684281592350201";
    options.AppSecret = "51ad0b358a4a8acd098a13a2c3aa035e";
});

builder.Services.AddDistributedMemoryCache();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors(); // Use CORS before authentication

app.UseSession(); // Add this line to enable session support


app.UseAuthentication(); // Add this line to enable authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
