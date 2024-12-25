using FragranceRecommendation.Services.FragranceService;
using FragranceRecommendation.Services.NoteService;
using FragranceRecommendation.Services.PerfumerService;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var allowFrontendOrigin = "_allowFrontendOrigin";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowFrontendOrigin, policy =>
    {
        policy.WithOrigins("http://localhost:8081")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true

            // ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFragranceService, FragranceService>();
builder.Services.AddScoped<IPerfumerService, PerfumerService>();
builder.Services.AddScoped<INoteService, NoteService>();
// Manufacturer

//builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDriver>(DbSettings.GetDbDriver());

var app = builder.Build();
app.UseCors(allowFrontendOrigin);

app.UseAuthentication();


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
app.UseAuthorization();
app.MapControllers();

app.Run();
