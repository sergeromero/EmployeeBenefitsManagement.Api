namespace Benefits.Application.Features.Employees.CreateEmployeeWithUser
{
    public sealed class UserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; }= string.Empty;
        public string Password {  get; set; }  = string.Empty;
    }
}