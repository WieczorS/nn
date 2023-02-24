namespace solutions;

public class News
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set;  }
}