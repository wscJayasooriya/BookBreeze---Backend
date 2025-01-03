namespace E_Book.Dto
{
    public class AuthResponseDto
    {
        public string? Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public string Fullname { get; set; }
    }
}
