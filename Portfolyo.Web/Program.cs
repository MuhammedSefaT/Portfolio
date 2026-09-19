using Portfolyo.Web.Application.Extensions;
using Portfolyo.Web.Extensions;
using Portfolyo.Web.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// JSON dosya deposu, repository'ler ve UnitOfWork
builder.Services.AddInfrastructure(builder.Environment, builder.Configuration);

// AutoMapper, FluentValidation ve generic servis
builder.Services.AddApplication();

// Çerez tabanlı kimlik doğrulama ve yetki politikaları
builder.Services.AddCookieAuthentication(builder.Environment);

var app = builder.Build();

// Admin rolü, yetkiler ve yönetici kullanıcısı (ilk çalıştırmada oluşur)
await app.UseDataSeedAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
