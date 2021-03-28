using Denhub.Common.Extensions;
using Xunit;

namespace Denhub.Common.Tests.Extensions {
    public class StringExtensionsTests {
        [Theory]
        [InlineData("ThisIsATest", "this_is_a_test")]
        [InlineData("thisIsATest", "this_is_a_test")]
        [InlineData("this_is_a_test", "this_is_a_test")]
        public void ToSnakeCase_NonCamelCaseValue_SnakeCaseValue(string input, string output) {
            var result = input.ToSnakeCase();
            
            Assert.Equal(output, result);
        }
    }
}