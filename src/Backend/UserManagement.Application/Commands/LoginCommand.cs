using MediatR;
using UserManagement.Application.DTOs;
using UserManagement.Application.DTOs.Login;

namespace UserManagement.Application.Commands;

public class LoginCommand : IRequest<LoginResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
