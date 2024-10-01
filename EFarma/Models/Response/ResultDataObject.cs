namespace EFarma.Models.Response
{
    public class ResultDataObject<TEntity> : ResultObject
    {
        public required TEntity Data { get; set; }
    }
}
