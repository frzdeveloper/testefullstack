using MediatR;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Commands;

public class CreateUserCommand : IRequest<UserDto>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
