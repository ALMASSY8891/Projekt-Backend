using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt_Backend.Models
{
    [Table("authority_session")]
    public partial class AuthoritySession
    {
        // elsődleges kulcs
        public int Id { get; set; }

        // másodlagos kulcs a Client entitáshoz
        public int ClientId { get; set; }

        // A token
        public string Token { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        //engedélyezett vagy visszavont
        public bool IsRevoked { get; set; }
        // Navigációs property a Client entitáshoz
        public virtual Client Client { get; set; } = null!;
    }
}
