using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.FileProviders;
using RemoteJob;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddWebSockets(options => { });
builder.Services.AddSingleton<WebSocketHandler>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "front")),
    RequestPath = "/front"
});

app.MapControllers();

app.UseRouting();

app.UseAuthorization();

app.UseWebSockets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    
    endpoints.MapHub<ChatHub>("/chathub");
    
    endpoints.Map("/ws", async context =>
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var handler = app.Services.GetService<WebSocketHandler>();
            await handler?.HandleAsync(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    });
});

app.Run();