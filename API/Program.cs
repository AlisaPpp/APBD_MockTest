using Application;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Personaldatabase");

builder.Services.AddTransient<ICurrencyService, CurrencyService>(_ => new CurrencyService(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/currency", async (CurrencyRequest request, ICurrencyService currencyService) =>
{
    try
    {
        await currencyService.AddCurrency(request);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/api/search", async (string type, string name, ICurrencyService currencyService) =>
{
    var request = new SearchRequest {Type = type, Name = name};
    var result = await currencyService.SearchByType(request);
    
    if (result == null)
        return Results.NotFound();
    
    return Results.Ok(result);
});





app.Run();


