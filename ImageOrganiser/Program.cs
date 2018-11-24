using System.Configuration;
using System.IO;
using ImageOrganiser.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using System.Threading.Tasks;

namespace ImageOrganiser
{
    class Program
    {
        static void Main(string[] args)
        {
            BootstrapConfiguration(args);
        }

        private static void BootstrapConfiguration(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            
            var app = serviceProvider.GetService<IApplication>();
            app.Run(args).GetAwaiter().GetResult();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {

            serviceCollection.AddTransient<IApplication, Application.Application>();
            
            serviceCollection.AddTransient<IObjectUploader, S3ObjectUploader>();
            serviceCollection.AddTransient<IFileTraverser, FileTraverser>();
            serviceCollection.AddTransient<IFaceRecogniser, RekognitionFaceRecogniser>();
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appSettings.json");
            var configuration = builder.Build();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddDefaultAWSOptions(configuration.GetAWSOptions());
            serviceCollection.AddAWSService<IAmazonS3>();

        }
    }
}
