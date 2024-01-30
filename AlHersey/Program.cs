using System.Text.Unicode;
using AlHersey.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();



//biz ekledik--oturum süresi.
builder.Services.AddSession(option =>
{
    option.IdleTimeout=TimeSpan.FromMinutes(1);
});

//biz ekledik.türkçe karakter sorunu için
builder.Services.AddWebEncoders(o => {
	o.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(UnicodeRanges.All);
});

//biz ekledik.layout ta kullanýcýnýn session(Email) gösterebilmek için 
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICategoryRepository, Cls_Category>();
builder.Services.AddScoped<IMessageRepository, Cls_Message>();
builder.Services.AddScoped<IOrderRepository, Cls_Order>();

builder.Services.AddScoped<IProductRepository, Cls_Product>();

builder.Services.AddScoped<ISettingRepository, Cls_Setting>();
builder.Services.AddScoped<IStatusRepository, Cls_Status>();
builder.Services.AddScoped<ISupplierRepository, Cls_Supplier>();
builder.Services.AddScoped<IUserRepository, Cls_User>();
builder.Services.AddScoped<MainPageModel>();
builder.Services.AddScoped<AlHerseyContext>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();// biz ekledik.

app.UseRouting();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
