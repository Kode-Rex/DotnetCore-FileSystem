using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StoneAge.FileStore.Domain;

namespace StoneAge.FileStore.Tests
{
    [TestFixture]
    public class FileSystemAsyncTests
    {
        [TestFixture]
        class ListAsync
        {
            [Test]
            public async Task WhenDirectoryExist_ExpectContents()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.ListAsync(path);
                //---------------Assert-----------------------
                result.Count().Should().BeGreaterThanOrEqualTo(1);
            }

            [Test]
            public async Task WhenFilePassedIn_ExpectEmptyList()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempFileName();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.ListAsync(path);
                //---------------Assert-----------------------
                result.Should().BeEmpty();
            }

            [TestCase(" ")]
            [TestCase("")]
            [TestCase(null)]
            public async Task WhenNullOrWhiteSpaceDirectory_ExpectEmptyList(string path)
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.ListAsync(path);
                //---------------Assert-----------------------
                result.Should().BeEmpty();
            }
        }

        [TestFixture]
        class ExistsAsync
        {
            [Test]
            public async Task WhenFileDoesNotExist_ExpectFalse()
            {
                //---------------Arrange-------------------
                var path = Create_Missing_File();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.ExistsAsync(path);
                //---------------Assert-----------------------
                result.Should().BeFalse();
            }

            [Test]
            public async Task WhenFileExist_ExpectTrue()
            {
                //---------------Arrange-------------------
                var path = Create_File();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.ExistsAsync(path);
                //---------------Assert-----------------------
                result.Should().BeTrue();
            }

            [TestCase(" ")]
            [TestCase("")]
            [TestCase(null)]
            public async Task WhenFileNullOrWhiteSpace_ExpectFalse(string path)
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.ExistsAsync(path);
                //---------------Assert-----------------------
                result.Should().BeFalse();
            }
            
            [Test]
            public async Task WhenDirectoryExist_ExpectTrue()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.ExistsAsync(path);
                //---------------Assert-----------------------
                result.Should().BeTrue();
            }
        }

        [TestFixture]
        class DeleteAsync
        {
            [Test]
            public async Task WhenFileExist_ExpectItIsRemoved()
            {
                //---------------Arrange-------------------
                var path = Create_File();
                
                var sut = new FileSystem();
                //---------------Act----------------------
                await sut.DeleteAsync(path);
                //---------------Assert-----------------------
                var fileExists = File.Exists(path);
                fileExists.Should().BeFalse();
            }

            [Test]
            public async Task WhenFileDoesNotExist_ExpectNothingToHappen()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                var sut = new FileSystem();
                //---------------Act----------------------
                await sut.DeleteAsync(path);
                //---------------Assert-----------------------
                var fileExists = File.Exists(path);
                fileExists.Should().BeFalse();
            }

            [TestCase(" ")]
            [TestCase("")]
            [TestCase(null)]
            public async Task WhenFileNullOrWhitespace_ExpectNoExceptionsThrown(string path)
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act & Assert-----------------------
                await sut.DeleteAsync(path);
                var fileExists = File.Exists(path);
                fileExists.Should().BeFalse();
            }

            [Test]
            public async Task WhenDirectoryExist_ExpectItIsRemoved()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(path);

                var sut = new FileSystem();
                //---------------Act----------------------
                await sut.DeleteAsync(path);
                //---------------Assert-----------------------
                var fileExists = Directory.Exists(path);
                fileExists.Should().BeFalse();
            }
        }

        [TestFixture]
        class ReadAsync
        {
            [Test]
            public async Task GivenFileExist_ExpectDocumentWithBytesReturned()
            {
                //---------------Arrange-------------------
                var contents = "hi, this is some text for a file";

                var path = Create_File(contents);

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.ReadAsync(path);
                //---------------Assert-----------------------
                var expected = "hi, this is some text for a file";
                actual.ToString().Should().Be(expected);
            }

            [Test]
            public async Task GivenFileDoesExist_ExpectNullDocument()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.ReadAsync(path);
                //---------------Assert-----------------------
                actual.Should().Be(FileSystem.NullDocument);
            }

            [Test]
            public async Task GivenNullPath_ExpectNullDocument()
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.ReadAsync(null);
                //---------------Assert-----------------------
                actual.Should().Be(FileSystem.NullDocument);
            }
        }

        [TestFixture]
        class GetDocumentAsync
        {
            [Test]
            public async Task GivenFileExist_ExpectDocumentWithBytesReturned()
            {
                //---------------Arrange-------------------
                var contents = "hi, this is some text for a file";

                var path = Create_File(contents);
                
                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.GetDocumentAsync(path);
                //---------------Assert-----------------------
                var expected = "hi, this is some text for a file";
                actual.ToString().Should().Be(expected);
            }

            [Test]
            public async Task GivenFileDoesExist_ExpectNullDocument()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.GetDocumentAsync(path);
                //---------------Assert-----------------------
                actual.Should().Be(FileSystem.NullDocument);
            }

            [Test]
            public async Task GivenNullPath_ExpectNullDocument()
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.GetDocumentAsync(null);
                //---------------Assert-----------------------
                actual.Should().Be(FileSystem.NullDocument);
            }
        }

        [TestFixture]
        class MoveAsync
        {
            [Test]
            public async Task GivenFileExist_ExpectItIsMoved()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = Create_File();
                File.Delete(newFileName);

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.MoveAsync(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(newFileName);

                actual.Should().BeTrue();
                oldFileExist.Should().BeFalse();
                newFileExist.Should().BeTrue();
            }

            [Test]
            public async Task GivenFileDoesNotExist_ExpectFalse()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = Create_File();
                File.Delete(newFileName);
                File.Delete(file);

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.MoveAsync(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(newFileName);

                actual.Should().BeFalse();
                oldFileExist.Should().BeFalse();
                newFileExist.Should().BeFalse();
            }

            [Test]
            public async Task GivenDestinationFileExist_ExpectItIsNotMoved()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = Create_File();
                
                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.MoveAsync(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(newFileName);

                actual.Should().BeFalse();
                oldFileExist.Should().BeTrue();
                newFileExist.Should().BeTrue();
            }
        }

        [TestFixture]
        class MoveWithOverwriteAsync
        {
            [Test]
            public async Task GivenDestinationFileExist_ExpectItIsMoved()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = Create_File();

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.MoveWithOverwriteAsync(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(newFileName);

                actual.Should().BeTrue();
                oldFileExist.Should().BeFalse();
                newFileExist.Should().BeTrue();
            }
            
            [Test]
            public async Task GivenFileDoesNotExist_ExpectFalse()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = Create_File();
                File.Delete(file);

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.MoveWithOverwriteAsync(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(newFileName);

                actual.Should().BeFalse();
                oldFileExist.Should().BeFalse();
                newFileExist.Should().BeTrue(); // Destination file should still exist
            }
        }

        [TestFixture]
        class RenameAsync
        {
            [Test]
            public async Task GivenFileExist_ExpectItIsRenamed()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = $"{Guid.NewGuid()}-moved.txt";

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.RenameAsync(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(Path.Combine(Path.GetTempPath(), newFileName));

                actual.Should().BeTrue();
                oldFileExist.Should().BeFalse();
                newFileExist.Should().BeTrue();
            }

            [Test]
            public async Task GivenFileDoesNotExist_ExpectFalse()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = $"{Guid.NewGuid()}-moved.txt";
                File.Delete(file);

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.RenameAsync(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(Path.Combine(Path.GetTempPath(), newFileName));

                actual.Should().BeFalse();
                oldFileExist.Should().BeFalse();
                newFileExist.Should().BeFalse();
            }
            
            [Test]
            public async Task GivenDestinationFileAlreadyExists_ExpectFalse()
            {
                //---------------Arrange-------------------
                var sourceFile = Create_File("source content");
                var existingFileName = Guid.NewGuid().ToString();
                var destinationFilePath = Path.Combine(Path.GetTempPath(), existingFileName);
                File.WriteAllText(destinationFilePath, "destination content");
                
                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.RenameAsync(sourceFile, existingFileName);
                //---------------Assert-----------------------
                var sourceFileExists = File.Exists(sourceFile);
                var destFileExists = File.Exists(destinationFilePath);
                
                actual.Should().BeFalse();
                sourceFileExists.Should().BeTrue(); // Source file should still exist
                destFileExists.Should().BeTrue(); // Destination file should still exist
                
                // Verify content hasn't changed
                var destContent = File.ReadAllText(destinationFilePath);
                destContent.Should().Be("destination content");
            }
        }

        private static string Create_File(string content)
        {
            var tmp = Path.GetTempPath();
            var path = Path.Combine(tmp, Guid.NewGuid().ToString());

            File.WriteAllText(path, content);

            return path;
        }

        private static string Create_File()
        {
            return Create_File(string.Empty);
        }

        private static string Create_Missing_File()
        {
            var tmp = Path.GetTempPath();
            var path = Path.Combine(tmp, Guid.NewGuid().ToString());

            return path;
        }
    }
} 