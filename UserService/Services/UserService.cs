using UserService.DTOs;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Services;

/// <summary>
/// Forretningslogik for brugerhåndtering.
/// Fungerer som mellemled mellem controlleren og repository-laget —
/// mapper DTOs til domæneobjekter og håndterer forretningsregler.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<User> GetAll()
    {
        return _repository.GetAll();
    }

    public User? GetById(string userId)
    {
        return _repository.GetById(userId);
    }

    public User? GetByAuthId(string authId)
    {
        return _repository.GetByAuthId(authId);
    }

    /// <summary>
    /// Mapper <see cref="CreateUserDto"/> til et <see cref="User"/>-domæneobjekt og persisterer det.
    /// </summary>
    public User Create(CreateUserDto dto)
    {
        var user = new User
        {
            AuthId = dto.AuthId,
            FirstName = dto.FirstName!,
            LastName = dto.LastName!,
            Email = dto.Email!,
            PhoneNumber = dto.PhoneNumber,
            Membership = dto.Membership,
            MembershipStatus = dto.MembershipStatus,
            TimeCreated = DateTime.UtcNow
        };

        _repository.Add(user);

        return user;
    }

    /// <summary>
    /// Mapper <see cref="UpdateUserDto"/> til et nyt User-objekt med det eksisterende ID.
    /// AuthId og TimeCreated bevares ikke — de stammer altid fra oprettelsestidspunktet.
    /// </summary>
    public bool Update(string userId, UpdateUserDto dto)
    {
        var updatedUser = new User
        {
            Id = userId,
            FirstName = dto.FirstName!,
            LastName = dto.LastName!,
            Email = dto.Email!,
            PhoneNumber = dto.PhoneNumber,
            Membership = dto.Membership,
            MembershipStatus = dto.MembershipStatus
        };

        return _repository.Update(userId, updatedUser);
    }

    public bool Delete(string userId)
    {
        return _repository.Delete(userId);
    }

    /// <summary>
    /// Returnerer et let objekt med membership-status og en praktisk <c>isActive</c>-boolean,
    /// så klienter slipper for at parse enum-strenge selv.
    /// </summary>
    public object? GetMembershipStatus(string userId)
    {
        var user = _repository.GetById(userId);
        if (user is null) return null;

        return new
        {
            userId = user.Id,
            membershipStatus = user.MembershipStatus.ToString(),
            isActive = user.MembershipStatus == MembershipStatus.Active
        };
    }

    /// <summary>
    /// Returnerer både enum-navn og numerisk værdi, så klienter kan bruge hvad der passer dem.
    /// </summary>
    public object? GetMembership(string userId)
    {
        var user = _repository.GetById(userId);
        if (user is null) return null;

        return new
        {
            userId = user.Id,
            membership = user.Membership.ToString(),
            // Numerisk værdi gør det nemt at sammenligne uden string-parsing på klientsiden
            membershipType = (int)user.Membership
        };
    }

    public IEnumerable<User> GetByMembership(MembershipType membershipType)
    {
        return _repository.GetAll()
            .Where(user => user.Membership == membershipType);
    }
}