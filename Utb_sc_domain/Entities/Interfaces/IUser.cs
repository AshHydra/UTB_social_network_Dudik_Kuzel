using System.Collections.Generic;

namespace Utb_sc_Domain.Entities.Interfaces
{
    public interface IUser<TKey> : IEntity<TKey>
    {
        string? UserName { get; set; }
        string? Email { get; set; }
        string? FirstName { get; set; }
        string? LastName { get; set; }
        string? PasswordHash { get; set; }
        string? ProfilePicturePath { get; set; } // Path to the user's profile picture

    }
}
