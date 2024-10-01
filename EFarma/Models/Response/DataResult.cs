namespace EFarma.Models.Response
{
    public class DataResult<TEntity> : VoidResult where TEntity : class
    {
        public required TEntity Result { get; set; }
    }
}
