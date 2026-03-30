namespace ARS.Domain.Exceptions
{

    public class NotFoundException : DomainException
    {
        public NotFoundException(string entityName, string entityId)
            : base($"{entityName} with ID '{entityId}' was not found.")
        {
        }
    }

}