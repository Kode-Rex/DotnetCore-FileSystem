using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace StoneAge.FileStore.Tests
{
    [TestFixture]
    public class DocumentTests
    {
        [Test]
        public void GivenTextBytes_ExpectStringReturned()
        {
            //---------------Arrange-------------------
            var input = "a message in a file";
            var bytes = Encoding.UTF8.GetBytes(input);

            var sut = new DocumentBuilder()
                        .With_Name("test.txt")
                        .With_Bytes(bytes)
                        .Create_Document();
            //---------------Act----------------------
            var actual = sut.ToString();
            //---------------Assert-----------------------
            var expected = "a message in a file";
            actual.Should().Be(expected);
        }

        [Test]
        public void GivenNullBytes_ExpectEmptyString()
        {
            //---------------Arrange-------------------
            var sut = new DocumentBuilder()
                .With_Name("test.txt")
                .With_Bytes(null)
                .Create_Document();
            //---------------Act----------------------
            var actual = sut.ToString();
            //---------------Assert-----------------------
            var expected = string.Empty;
            actual.Should().Be(expected);
        }
    }
}
