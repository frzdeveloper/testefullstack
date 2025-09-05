using FluentAssertions;
using Moq;
using UserManagement.Application.Commands;
using UserManagement.Application.Handlers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Tests.Handlers;

public class CreateUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new CreateUserHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateUserSuccessfully()
    {
        var command = new CreateUserCommand
        {
            Name = "Felipe Rabelo Zavitoski",
            Email = "felipezavitoski1996@gmail.com",
            Password = "123456789"
        };

        var expectedUserId = Guid.NewGuid();
        var expectedCreatedAt = DateTime.UtcNow;

        // Mock: Email não existe
        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email))
            .ReturnsAsync(false);

        // Mock: Adicionar usuário
        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) =>
            {
                var userWithId = new User(user.Name, user.Email, user.PasswordHash);
                
                // Usar reflection para definir propriedades privadas (simulando DB)
                var idProperty = typeof(User).BaseType!.GetProperty("Id")!;
                idProperty.SetValue(userWithId, expectedUserId);
                
                var createdAtProperty = typeof(User).BaseType!.GetProperty("CreatedAt")!;
                createdAtProperty.SetValue(userWithId, expectedCreatedAt);
                
                return userWithId;
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedUserId);
        result.Name.Should().Be(command.Name);
        result.Email.Should().Be(command.Email);
        result.CreatedAt.Should().Be(expectedCreatedAt);

        // Verificar se a senha foi hashada (não deve ser igual à senha original)
        _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => 
            u.Name == command.Name &&
            u.Email == command.Email &&
            u.PasswordHash != command.Password &&
            !string.IsNullOrEmpty(u.PasswordHash)
        )), Times.Once);

        // Verificar se verificou a existência do email
        _userRepositoryMock.Verify(x => x.ExistsByEmailAsync(command.Email), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        var command = new CreateUserCommand
        {
            Name = "Felipe Rabelo Zavitoski",
            Email = "felipezavitoski1996@gmail.com",
            Password = "123456789"
        };

        // Mock: Email já existe
        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email))
            .ReturnsAsync(true);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("O e-mail já existe");

        // Verificar que não tentou adicionar usuário
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        
        // Verificar que verificou a existência do email
        _userRepositoryMock.Verify(x => x.ExistsByEmailAsync(command.Email), Times.Once);
    }

    [Theory]
    [InlineData("", "teste@email.com", "senha123")]
    [InlineData("Felipe", "", "senha123")]
    [InlineData("Felipe", "teste@email.com", "")]
    public async Task Handle_WithInvalidData_ShouldStillProcessButCreateUserWithProvidedData(
        string name, string email, string password)
    {
        var command = new CreateUserCommand
        {
            Name = name,
            Email = email,
            Password = password
        };

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(name);
        result.Email.Should().Be(email);
        
    }

    [Fact]
    public async Task Handle_ShouldHashPasswordUsingBCrypt()
    {
        var command = new CreateUserCommand
        {
            Name = "Teste Usuario",
            Email = "teste@bcrypt.com",
            Password = "minhasenhasecreta"
        };

        User capturedUser = null!;

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user)
            .ReturnsAsync((User user) => user);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser.PasswordHash.Should().NotBe(command.Password);
        capturedUser.PasswordHash.Should().StartWith("$2a$"); 
        
        BCrypt.Net.BCrypt.Verify(command.Password, capturedUser.PasswordHash)
            .Should().BeTrue("o hash da senha deve ser verificável com BCrypt");
    }

    [Fact]
    public async Task Handle_WithRepositoryException_ShouldPropagateException()
    {
        var command = new CreateUserCommand
        {
            Name = "Usuario Teste",
            Email = "erro@repository.com",
            Password = "senha123"
        };

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ThrowsAsync(new InvalidOperationException("Erro no banco de dados"));

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro no banco de dados");
    }
}
