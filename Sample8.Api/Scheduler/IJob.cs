using System.Threading.Tasks;

namespace Sample8.Api.Scheduler
{
    public interface IJob
    {
        Task<bool> DoJob();
    }
}
