using FragranceRecommendation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDriver>(DbSettings.GetDbDriver());
var AllowFrontendOrgin = "_allowFrontendOrigin";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowFrontendOrgin, policy =>
    {
        policy.WithOrigins("http://localhost:8081")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



var app = builder.Build();
app.UseCors(AllowFrontendOrgin);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //nece nesto
    //app.useHsts();
}

app.UseHttpsRedirection();

//app.MapConrollerRoute(name: "name", pattern "{controller}/action=Index/id
app.UseRouting();
app.MapControllers();

app.Run();
