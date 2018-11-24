using System;
using Xunit;

namespace ImageOrganiser.Tests
{
    public class StringExtensionTests
    {
        [Theory]
        [InlineData(@"\test\test\x.jpg", "x.jpg")]
        [InlineData(@"\abc.png", "abc.png")]
        [InlineData("ab.jpeg", "ab.jpeg")]
        public void GetFileFromKey_ShouldReturnCorrectResponse(string key, string expected)
        {
            var result = key.GetFileFromKey();

            Assert.Equal(expected, result);
        }
    }
}
