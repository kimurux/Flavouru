using Flavouru.Application.Interfaces;
using Flavouru.Infrastructure.Data;
using Flavouru.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Flavouru.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Добавление контроллеров
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Корсы
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
// Регистрация DbContext
builder.Services.AddDbContext<FlavouruDbContext>(options =>
    options.UseSqlite("Data Source=base.db"));

// Регистрация сервисов
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// Добавление Автомапера
builder.Services.AddAutoMapper(typeof(RecipeProfile).Assembly);

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FlavouruDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка применения миграций :(");
    }
}
// Свагер
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Urls.Clear();
app.Urls.Add("http://localhost:5000");

app.Run();

