public class CommentDto
{
    public int CommentID { get; set; } // If needed, else you can leave it out for POST requests
    public string Text { get; set; }
    public int UserID { get; set; }
    public int TicketID { get; set; }
    public DateTime CreatedOn { get; set; } // Optional if you want to show when the comment was created
    public string? CommentImage { get; set; } // Change to string to hold Base64 string
    public string? UserName { get; set; } // To store the username of the commenter
}
