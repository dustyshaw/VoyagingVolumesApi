namespace book_api;

public interface IBook
{
    public int id { get; set; }
    public string title { get; set; }
    public string author { get; set; }
    public string isbn { get; set; }
    public string image_url { get; set; }
    public decimal price { get; set; }
}
