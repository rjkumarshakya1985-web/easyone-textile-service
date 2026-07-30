

namespace Textile.Core.Entities.DbEnitites
{
    public class RefreshToken : DatabaseEntity<Guid>
    {
       
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }
        public string ? ReplacedByToken { get; set; }

        public User User { get; set; }


    }
}
