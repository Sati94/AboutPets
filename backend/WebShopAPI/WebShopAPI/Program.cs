using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebShopAPI.Service.Authentication;
using WebShopAPI.Model.UserModels;
using System.Numerics;
using System.Text;
using WebShopAPI.Data;
using WebShopAPI.Service.ProductServiceMap;
using WebShopAPI.Service.UserServiceMap;
using Microsoft.Extensions.DependencyInjection;
using WebShopAPI.Service.UserProfileMap;
using WebShopAPI.Service.OrderItemServiceMap;
using WebShopAPI.Service.OrderServiceMap;

var builder = WebApplication.CreateBuilder(args);
AddAuthentication();
AddIdentity();
ConfigureSwagger();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddDbContext<WebShopContext>();

var app = builder.Build();
AddRoles();
AddAdmin();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin();
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();

});


            
if (app.Environment.IsDevelopment())
{
     app.UseSwagger();
     app.UseSwaggerUI();
     app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
        
void AddAuthentication()
{
    builder.Services
       .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters()
           {
               ClockSkew = TimeSpan.Zero,
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = "apiWithAuthBackend",
               ValidAudience = "apiWithAuthBackend",
               IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes("!SomethingSecret!")
               ),
           };
       });
}
void AddIdentity()
{
    builder.Services
    .AddIdentityCore<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<WebShopContext>();

}
void ConfigureSwagger()
{
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
                     {
                   {
                                 new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
});

    });


}
void AddRoles()
{
    using var scope = app.Services.CreateScope(); // RoleManager is a scoped service, therefore we need a scope instance to access it
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var tAdmin = CreateAdminRole(roleManager);
    tAdmin.Wait();

    var tUser = CreateUserRole(roleManager);
    tUser.Wait();
}
async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
{
   var adminRoleExists =  await roleManager.RoleExistsAsync("Admin");
    if (!adminRoleExists) 
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
}

async Task CreateUserRole(RoleManager<IdentityRole> roleManager)
{
     var userRoleExists = await roleManager.RoleExistsAsync("User");
    if (!userRoleExists)
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }
}
void AddAdmin()
{
    var tAdmin = CreateAdminIfNotExists();
    tAdmin.Wait();
}

async Task CreateAdminIfNotExists()
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var adminInDb = await userManager.FindByEmailAsync("admin@admin.com");
    if (adminInDb == null)
    {
        var admin = new IdentityUser { UserName = "admin", Email = "admin@admin.com" };
        var adminCreated = await userManager.CreateAsync(admin, "admin1234");

        if (adminCreated.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
            Console.WriteLine($"Admin created: {admin.Id}, {admin.UserName}, {admin.Email}");
            var customUser = new User
            {
                UserName = admin.UserName,
                Email = admin.Email,
            };
            using var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
            dbContext.Users.Add(customUser);
            await dbContext.SaveChangesAsync();
        }
      
    }


}
public partial class Program { };


