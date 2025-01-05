namespace Roomify.Entities.EntityDescriptors
{
    public interface IHaveCreateAndUpdateAudit : IHaveCreateOnlyAudit
    {
        public DateTimeOffset UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
