namespace Benefits.Domain;

public enum ProvisioningStatus
{
    Started = 0,
    UserCreated = 1,
    RoleAssigned = 2,
    EmployeeCreated = 3,
    Completed = 4,
    Failed = 5,
    UserUpdated = 6,
    PasswordUpdated = 7,
    EmployeeUpdated = 8,
}