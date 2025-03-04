namespace book_api.Services;

public interface IBookService
{
    public Task<List<Book>> GetAllBooks();
    public Task<bool> AddBook(Book book);
    public Task<List<Book>> UpdateBook(int id, Book book, IFormFile? imageFile = null);
    public Task<bool> RemoveBook(int id);
    public Task<Book> GetBook(int id);
}
