using ImageOrganiser.Application;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageOrganiser
{
    public interface IFileTraverser
    {
        Task TraverseFor(Settings settings);
    }
}
