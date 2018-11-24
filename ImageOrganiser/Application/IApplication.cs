using System.Threading.Tasks;

namespace ImageOrganiser.Application
{
    internal interface IApplication
    {
        Task Run(string[] args);
    }
}
