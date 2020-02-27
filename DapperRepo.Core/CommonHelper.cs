using DapperRepo.Core.Infrastructure;

namespace DapperRepo.Core
{
    public class CommonHelper
    {
        /// <summary>
        /// Gets or sets the default file provider
        /// </summary>
        public static IAppFileProvider DefaultFileProvider { get; set; }
    }
}
