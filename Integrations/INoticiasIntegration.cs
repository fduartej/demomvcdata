namespace demomvcdata.Integrations;

public interface INoticiasIntegration
{
    Task<IReadOnlyList<NoticiaItem>> GetNoticiasAsync(CancellationToken cancellationToken = default);
}