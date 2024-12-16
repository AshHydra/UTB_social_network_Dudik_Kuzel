using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utb_sc_Domain.Entities.Interfaces
{
    public interface IUser<TKey> : IEntity<TKey>
    {
        string? UserName { get; set; }
        string? Email { get; set; }
        string? FirstName { get; set; }
        string? LastName { get; set; }
        string? PasswordHash { get; set; }
    }
}
