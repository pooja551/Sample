
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger middleware (shows /swagger UI in Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok("Todo API is running"));

// In-memory data
List<TodoItem> todos = new()
{
    new(1, "Learn minimal APIs", false),
    new(2, "Build in GitHub Codespaces", true)
};

// CRUD

app.MapGet("/api/todos", () => Results.Ok(todos));

app.MapGet("/api/todos/{id:int}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});

app.MapPost("/api/todos", (TodoItem input) =>
{
    var nextId = todos.Count == 0 ? 1 : todos.Max(t => t.Id) + 1;
    var todo = new TodoItem(nextId, input.Title, input.IsDone);
    todos.Add(todo);
    return Results.Created($"/api/todos/{nextId}", todo);
});

app.MapPut("/api/todos/{id:int}", (int id, TodoItem update) =>
{
    var index = todos.FindIndex(t => t.Id == id);
    if (index == -1) return Results.NotFound();
    todos[index] = update with { Id = id };
    return Results.NoContent();
});

app.MapDelete("/api/todos/{id:int}", (int id) =>
{
    var removed = todos.RemoveAll(t => t.Id == id) > 0;
    return removed ? Results.NoContent() : Results.NotFound();
});

app.Run();

public record TodoItem(int Id, string Title, bool IsDone);
