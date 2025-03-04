using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.RateLimiting;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace book_api.Services;

public class BookService : IBookService
{
    public string FilePath { get; set; }

    public BookService(string file_path)
    {
        FilePath = file_path;
    }

    public async Task<bool> AddBook(Book book)
    {
        try
        {
            List<Book> books = new List<Book>();

            if (File.Exists(FilePath))
            {
                var json = await System.IO.File.ReadAllTextAsync(FilePath);
                books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }
            books.Add(book);

            var updatedJson = JsonSerializer.Serialize(books);
            await System.IO.File.WriteAllTextAsync(FilePath, updatedJson);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<List<Book>> GetAllBooks()
    {
        try
        {

        if (!File.Exists(FilePath))
        {
            File.Create(FilePath);
        }
        }
        catch (Exception e)
        {
            throw new Exception($"Something went wrong getting all book data {e.Message.ToString()}");
        }

        var json = await System.IO.File.ReadAllTextAsync(FilePath);

        return JsonSerializer.Deserialize<List<Book>>(json) ?? throw new Exception("No data");

    }

    public async Task<Book> GetBook(int id)
    {
        if (!System.IO.File.Exists(FilePath))
        {
            return new Book();
        }
        var json = await System.IO.File.ReadAllTextAsync(FilePath);
        var books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();

        return books.FirstOrDefault(book => book.id == id) ?? new Book();
    }

    public async Task<bool> RemoveBook(int id)
    {
        if (!System.IO.File.Exists(FilePath))
        {
            return false;
        }
        var json = await System.IO.File.ReadAllTextAsync(FilePath);
        var books = JsonSerializer.Deserialize<List<Book>>(json); // Assuming Book is your model class

        // Find the book by ID
        var bookToRemove = books?.FirstOrDefault(b => b.id == id);
        if (bookToRemove == null)
        {
            return false; // Book not found
        }

        // Remove the book
        books?.Remove(bookToRemove);

        // Serialize the updated list back to JSON
        var updatedJson = JsonSerializer.Serialize(books);

        // Write the updated JSON back to the file
        await System.IO.File.WriteAllTextAsync(FilePath, updatedJson);

        return true;
    }

    public async Task<List<Book>> UpdateBook(int id, Book book, IFormFile? imageFile = null)
    {
        List<Book> books = new List<Book>();

        if (File.Exists(FilePath))
        {
            var json = await System.IO.File.ReadAllTextAsync(FilePath);
            books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        var index = books.FindIndex(x => x.id == id);


        if (imageFile != null)
        {
            var uploadedImagePath = await UploadImageAsync(imageFile);
            book.image_url = uploadedImagePath;
        }

        books[index] = book;
        book.id = id;

        var updatedJson = JsonSerializer.Serialize(books);
        await System.IO.File.WriteAllTextAsync(FilePath, updatedJson);

        return JsonSerializer.Deserialize<List<Book>>(updatedJson) ?? throw new Exception("No data");
    }

    private async Task<string> UploadImageAsync(IFormFile file)
    {
        using (var httpClient = new HttpClient())
        {
            using (var content = new MultipartFormDataContent())
            {
                using (var stream = file.OpenReadStream())
                {
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                    content.Add(fileContent, "file", file.FileName);

                    var response = await httpClient.PostAsync("http://api.voyaging-volumes.duckdns.org/upload", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var jsonResult = JsonDocument.Parse(result);

                        return jsonResult.RootElement.GetProperty("filePath").GetString() ?? "default.jpg";
                    }
                    else
                    {
                        throw new Exception("Image upload failed");
                    }
                }
            }
        }
    }

}
