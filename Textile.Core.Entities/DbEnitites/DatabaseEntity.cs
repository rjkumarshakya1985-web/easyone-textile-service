using System.Diagnostics.CodeAnalysis;

namespace Textile.Core.Entities.DbEnitites
{
    public class DatabaseEntity<TEntityId>
    {
        [NotNull]
        public TEntityId Id { get; set; }
    }
}
