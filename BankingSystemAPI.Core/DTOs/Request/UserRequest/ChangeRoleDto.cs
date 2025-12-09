using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.DTOs.Request.UserRequest;

public class ChangeRoleDto
{
    public UserRole NewRole { get; set; }
}