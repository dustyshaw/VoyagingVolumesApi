using book_api;
using book_api.Data;
using book_api.Services;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();
var app = builder.Build();


app.UseCors(c =>
c.AllowAnyHeader()
.AllowAnyMethod()
.AllowAnyOrigin());


app.UseSwagger();
app.UseSwaggerUI();



//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "imageuploads")),
//    RequestPath = "/imageuploads"
//});

//string storagePath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "site", "wwwroot", "books.json");
//BookService bookService = new BookService(storagePath);

//string storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "books.json");
//BookService bookService = new BookService(storagePath);

app.MapGet("/all-books", () =>
{
    var storageConnectionString = Environment.GetEnvironmentVariable("StorageSecrets") ?? throw new Exception("Couldn't get Storage Connection String");

    var tableName = "books";
    try
    { 

        Microsoft.Azure.Cosmos.Table.CloudStorageAccount storageAccount;
        storageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(storageConnectionString);

        Microsoft.Azure.Cosmos.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        Microsoft.Azure.Cosmos.Table.CloudTable table = tableClient.GetTableReference(tableName);

        var entities = table.ExecuteQuery(new Microsoft.Azure.Cosmos.Table.TableQuery<BookEntity>()).ToList();

        return Results.Ok(entities); // Return the list of books
    }
    catch (Exception ex)
    {
        throw new Exception($"Something went wrong fetching books from table, {ex.Message}");
    }
    //return "all-books !";
}
//await bookService.GetAllBooks()
);


app.MapGet("/get-book/{id}", (int id) =>
{
    return "get-book";
}
    //await bookService.GetBook(id)
);

app.MapPost("/add-book", async (Book book) =>
//await bookService.AddBook(book)
{
    return "add-books !";
}
);

app.MapPatch("/update-book/{id}", async (int id, Book book) =>
   // await bookService.UpdateBook(id, book)
    {
    return "update-books !";
}
);

app.MapDelete("/delete-book", async (int id) =>
    //await bookService.RemoveBook(id)
    {
    return "delete-books !";
}
);

app.MapPost("/upload", async (IFormFile file) =>
{
    
        return "delete-books !";
    
    //string uploadsFolder = "imageuploads";
    //if (!Directory.Exists(uploadsFolder))
    //{
    //    Directory.CreateDirectory(uploadsFolder);
    //}

    //var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    //var filePath = Path.Combine(uploadsFolder, fileName);

    //using (var stream = new FileStream(filePath, FileMode.Create))
    //{
    //    await file.CopyToAsync(stream);
    //}

    //return Results.Ok(new { FilePath = filePath });

}).DisableAntiforgery();

app.Run();
