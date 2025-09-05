using MediatR;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Queries;

public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
{
}
