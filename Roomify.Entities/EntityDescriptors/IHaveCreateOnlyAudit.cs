namespace Roomify.Entities.EntityDescriptors
{
    public interface IHaveCreateOnlyAudit
    {
        public DateTimeOffset CreatedAt { get; set; }

        public string? CreatedBy { get; set; }
    }
}
