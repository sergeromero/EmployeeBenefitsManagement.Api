namespace Benefits.Application.DemoDataSeeder
{
    public interface IApplicationDataSeeder
    {
        Task SeedAsync(CancellationToken cancellationToken);
    }
}
