namespace Textile.Core.Entities.Data
{
    public class TableResult<T>
    {
        public int TotalRows { get; set; }
        public IEnumerable<T> Result { get; set; }
    }
}
