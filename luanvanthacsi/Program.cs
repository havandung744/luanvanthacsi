using AntDesign;
using luanvanthacsi.Areas.Identity;
using luanvanthacsi.Data;
using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Core.Composing.CompositionExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddAntDesign();
builder.Services.AddSingleton<IScientistService, ScientistService>();
builder.Services.AddSingleton<IStudentService, StudentService>();
builder.Services.AddSingleton<TableLocale>(c =>
{
    var locale = new TableLocale()
    {
        CancelSort = "Nhấn để hủy sắp xếp",
        Collapse = "Thu gọn nội dung dòng",
        Expand = "Mở rộng nội dung dòng",
        FilterConfirm = "Lọc",
        FilterEmptyText = "",
        FilterReset = "Xóa lọc",
        FilterTitle = "Lọc dữ liệu",
        SelectAll = "Chọn trang hiện tại",
        SelectInvert = "Đảo ngược trang hiện tại",
        SelectionAll = "Chọn tất cả dữ liệu",
        SortTitle = "Sắp xếp",
        TriggerAsc = "Nhấn để sắp xếp tăng dần (a-z)",
        TriggerDesc = "Nhấn để sắp xếp giảm dần (z-a)",
        FilterOptions = new FilterOptions
        {
            And = "Và",
            Contains = "Chứa",
            EndsWith = "Kết thúc bằng",
            Equals = "Bằng",
            False = "Sai",
            GreaterThan = "Lớn hơn",
            GreaterThanOrEquals = "Lớn hơn hoặc bằng",
            IsNotNull = "Khác rỗng",
            IsNull = "Là rỗng",
            LessThan = "Nhỏ hơn",
            LessThanOrEquals = "Nhỏ hơn hoặc bằng",
            NotContains = "Không chứa",
            NotEquals = "Không bằng",
            Or = "Hoặc",
            StartsWith = "Bắt đầu bằng",
            TheSameDateWith = "Bằng với ngày",
            True = "Đúng"
        }
    };
    return locale;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
