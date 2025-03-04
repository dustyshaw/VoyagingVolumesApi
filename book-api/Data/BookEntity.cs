using Microsoft.Azure.Cosmos.Table;

namespace book_api.Data;

public class BookEntity : TableEntity
{
    public BookEntity()
    {
        
    }
    public BookEntity(string title, string author, string condition, double price)
    {
        PartitionKey = title;
        RowKey = author;
    }
    public string Condition { get; set; }
    public double Price { get; set; }
}
