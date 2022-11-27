namespace PasswordManager.Model
{
    public partial class Card
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
