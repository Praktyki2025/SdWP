public class EditUserRequestDTO
{
    public Guid Id { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Role { get; set; }
}