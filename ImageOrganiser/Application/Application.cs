using Amazon.S3;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;

namespace ImageOrganiser.Application
{
    public class Application : IApplication
    {
        private readonly IFileTraverser _fileTraverser;
        private readonly IConfiguration _configuration;

        public Application(IFileTraverser fileTraverser, IConfiguration configuration)
        {
            _fileTraverser = fileTraverser;
            _configuration = configuration;
        }

        public async Task Run(string[] args)
        {
            var settings = GetUserInput();
            settings.BucketName = _configuration.GetSection("Settings:BucketName").Value.ToString();
            settings.SourcePrefix = _configuration.GetSection("Settings:SourcePrefix").Value.ToString();
            await _fileTraverser.TraverseFor(settings);
        }

        private Settings GetUserInput()
        {
            Console.WriteLine($"Please enter your destination prefix:");
            var destKey = Console.ReadLine();
            while (!isAlphaNumeric(destKey))
            {
                Console.WriteLine($"Invalid input, please only use alpha numeric with no spaces:");
                destKey = Console.ReadLine();
            }

            Console.WriteLine($"What age person are you looking for? Leave blank to ignore:");
            var age = Console.ReadLine();
            while (!String.IsNullOrWhiteSpace(age) && !int.TryParse(age, out int ageVal))
            {
                Console.WriteLine($"Invalid input, What age person are you looking for? Leave blank to ignore:");
                age = Console.ReadLine();
            }

            Console.WriteLine($"Are you looking for a male or female? Enter m/f or leave blank to ignore:");
            var maleOrFemale = Console.ReadLine();
            while (!String.IsNullOrWhiteSpace(maleOrFemale) && 
                !maleOrFemale.Equals("m", StringComparison.InvariantCultureIgnoreCase) && 
                !maleOrFemale.Equals("f", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"Are you looking for a male or female? Enter m/f or leave blank to ignore:");
                maleOrFemale = Console.ReadLine();
            }

            Console.WriteLine($"Should they be smiling? Enter y/n or leave blank to ignore:");
            var areSmiling = Console.ReadLine();
            while (!String.IsNullOrWhiteSpace(areSmiling) &&
                !areSmiling.Equals("y", StringComparison.InvariantCultureIgnoreCase) &&
                !areSmiling.Equals("n", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"Should they be smiling? Enter y/n or leave blank to ignore:");
                areSmiling = Console.ReadLine();
            }

            return new Settings
            {
                DestinationPrefix = destKey,
                Age = String.IsNullOrWhiteSpace(age) ? null : (int?)int.Parse(age),
                IsMale = String.IsNullOrWhiteSpace(maleOrFemale) ? (bool?)null : maleOrFemale.Equals("m", StringComparison.InvariantCultureIgnoreCase),
                IsSmiling = String.IsNullOrWhiteSpace(areSmiling) ? (bool?)null : areSmiling.Equals("y", StringComparison.InvariantCultureIgnoreCase)
            };
        }

        private Boolean isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9]*$");
            return rg.IsMatch(strToCheck);
        }
    }
}
