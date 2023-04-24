namespace UserApi.Entities;

public class CreateDto
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime Birthday { get; set; }
    public bool Admin { get; set; }
}