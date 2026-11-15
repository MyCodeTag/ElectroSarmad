using Sarmad.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

Center.Configuration = builder.Configuration;


builder.Services
    .AddResponseCaching()
    .AddRazorPages()
    .AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.UseStaticFiles();
app.UseStatusCodePagesWithRedirects("/Other/{0}");

app.Run();



