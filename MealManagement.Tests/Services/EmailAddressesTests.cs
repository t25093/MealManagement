using MealManagement.Service.Services;

namespace MealManagement.Tests.Services
{
    public class EmailAddressesTests
    {
        [Theory]
        [InlineData("dina@test.com", "dina@test.com")]
        [InlineData("  Dina@Test.com ", "dina@test.com")]
        [InlineData("dina @ test.com", "dina@test.com")]
        [InlineData("‫‎dina@test.com‎‬", "dina@test.com")]
        [InlineData("di na@te st.com\t", "dina@test.com")]
        public void Normalize_RemovesSpacesAndInvisibleMarks(string input, string expected)
        {
            Assert.Equal(expected, EmailAddresses.Normalize(input));
        }
    }
}
