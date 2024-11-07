public class UserDto
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string? Password { get; set; } // Optional field for user creation or update
    public int RoleID { get; set; }
    public string RoleName { get; set; } // Add this property to display the role name
}
