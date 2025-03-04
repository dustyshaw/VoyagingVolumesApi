
using book_api;
using book_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();
var app = builder.Build();


app.UseCors(c =>
c.AllowAnyHeader()
.AllowAnyMethod()
.AllowAnyOrigin());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "imageuploads")),
    RequestPath = "/imageuploads"
});

BookService bookService = new BookService("books.json");

app.MapGet("/all-books", async () =>
    await bookService.GetAllBooks()
);

app.MapGet("/get-book/{id}", async (int id) =>
    await bookService.GetBook(id)
);

app.MapPost("/add-book", async (Book book) =>
    await bookService.AddBook(book)
);

app.MapPatch("/update-book/{id}", async (int id, Book book) =>
    await bookService.UpdateBook(id, book)
);

app.MapDelete("/delete-book", async (int id) =>
    await bookService.RemoveBook(id)
);

app.MapPost("/upload", async (IFormFile file) =>
{
    string uploadsFolder = "imageuploads";
    if (!Directory.Exists(uploadsFolder))
    {
        Directory.CreateDirectory(uploadsFolder);
    }

    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    var filePath = Path.Combine(uploadsFolder, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    return Results.Ok(new { FilePath = filePath });

}).DisableAntiforgery();

app.Run();
