using book_api;
using book_api.Services;

namespace book_tests
{
    public class CRUD_Tests
    {
        const string FilePath = "C:\\Users\\shust\\code\\advanced-front-end\\book-api\\book-tests\\test.json";

        [Fact]
        public async void ReadAllBooks()
        {
            BookService bs = new BookService(FilePath);

            var books = await bs.GetAllBooks();

            Assert.Equal(3, books.Count);
        }

        [Fact]
        public async void AddBook()
        {
            BookService bs = new BookService(FilePath);

            var books = await bs.GetAllBooks();

            Book newBook = new Book() { 
                title = "Norweigen Wood", 
                author = "Haruki Marukami", 
                image_url= "https://m.media-amazon.com/images/I/81b+vC579WL._AC_UF1000,1000_QL80_.jpg",
                isbn="123",
                price=2
            };

            await bs.AddBook(newBook);
            await bs.GetAllBooks();
        }
    }
}