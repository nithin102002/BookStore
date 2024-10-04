namespace BookStore.Models
{
    public class BookData
    {
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public string Author {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
