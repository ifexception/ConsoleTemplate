namespace ConsoleTemplate.App
{
    using System.Threading.Tasks;

    public interface IApplication
    {
        Task<string> RunAsync();
    }
}
