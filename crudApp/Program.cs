using Microsoft.EntityFrameworkCore;
using crudApp.Persistence.Contexts;
using crudApp.Services.ProductService;
using crudApp.Services.AutomationService;



// create builder
var builder = WebApplication.CreateBuilder(args); 

// default services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// add database service with configuration
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// add service
builder.Services.AddTransient<IAutomationService, AutomationService>();
builder.Services.AddTransient<IProductService, ProductService>();


// build app
var app = builder.Build(); 


// middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(); 
app.UseHttpsRedirection(); 
app.UseAuthorization(); 
app.MapControllers();

// run app
app.Run(); 


