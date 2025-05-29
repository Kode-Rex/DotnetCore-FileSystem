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
    public class FileSystemTests
    {
        [TestFixture]
        class Write
        {
            [Test]
            public async Task WhenFileAndPathValid_ExpectFileWritten()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();
                var fileName = Guid.NewGuid()+".csv";
                var document = Create_CsvFile(fileName);

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Write(path, document);
                //---------------Assert-----------------------
                var fileWritten = File.Exists(result.FullFilePath);
                result.HadError.Should().BeFalse();
                fileWritten.Should().BeTrue();
            }

            [Test]
            public async Task WhenFileAndPathContainNewSubDirectories_ExpectFileWritten()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                var fileName = Guid.NewGuid() + ".csv";
                var document = Create_CsvFile(fileName);

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Write(path, document);
                //---------------Assert-----------------------
                var fileWritten = File.Exists(Path.Combine(path, fileName));
                result.HadError.Should().BeFalse();
                fileWritten.Should().BeTrue();
            }

            [TestCase(" ")]
            [TestCase("")]
            [TestCase(null)]
            public async Task WhenPathContainsNullOrWhiteSpace_ExpectErrorMessage(string path)
            {
                //---------------Arrange-------------------
                var document = Create_CsvFile("test.csv");

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Write(path, document);
                //---------------Assert-----------------------
                result.HadError.Should().BeTrue();
            }
            
            [TestCase("abc")]
            [TestCase("~f0")]
            public async Task WhenRelativePath_ExpectFileWritten(string path)
            { 
                //---------------Arrange-------------------
                var document = Create_CsvFile("test.csv");

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Write(path, document);
                //---------------Assert-----------------------
                var fileWritten = File.Exists(Path.Combine(path, "test.csv"));
                result.HadError.Should().BeFalse();
                fileWritten.Should().BeTrue();
            }

            [Test]
            public async Task WhenFileAndPathValid_ExpectFullFilePathReturned()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();
                var fileName = Guid.NewGuid() + ".csv";
                var document = Create_CsvFile(fileName);

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Write(path, document);
                //---------------Assert-----------------------
                var expected = Path.Combine(path, fileName);
                result.FullFilePath.Should().Be(expected);
            }

            [Test]
            public async Task WhenFileExist_ExpectItOverWritten()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();
                var fileName = Guid.NewGuid() + ".csv";
                Write_File_Contents_For_Testing(path, fileName);
                var document = Create_CsvFile(fileName);

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Write(path, document);
                //---------------Assert-----------------------
                var contents = File.ReadAllBytes(result.FullFilePath);
                contents.Should().BeEquivalentTo(new byte[5]);
            }

            [Test]
            public async Task WhenFileDataIsNull_ExpectErrorMessage()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();
                var fileName = Guid.NewGuid() + ".txt";
                var document = new DocumentBuilder()
                                    .With_Name(fileName)
                                    .With_Bytes(null)    // explicitly set null data
                                    .Create_Document();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Write(path, document);
                //---------------Assert-----------------------
                result.HadError.Should().BeTrue();
                result.ErrorMessages.Should().Contain("No file data provided; cannot write file.");
            }

            private static void Write_File_Contents_For_Testing(string path, string fileName)
            {
                File.WriteAllText(Path.Combine(path, fileName), "test line");
            }
        }

        [TestFixture]
        class Append
        {
            [Test]
            public async Task WhenFileAndPathValid_ExpectFileWritten()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();
                var fileName = Guid.NewGuid() + ".csv";
                var document = Create_CsvFile(fileName);

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Append(path, document);
                //---------------Assert-----------------------
                var fileWritten = File.Exists(result.FullFilePath);
                result.HadError.Should().BeFalse();
                fileWritten.Should().BeTrue();
            }

            [Test]
            public async Task WhenFileAndPathContainNewSubDirectories_ExpectFileWritten()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                var fileName = Guid.NewGuid() + ".csv";
                var document = Create_CsvFile(fileName);

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Append(path, document);
                //---------------Assert-----------------------
                var fileWritten = File.Exists(Path.Combine(path, fileName));
                result.HadError.Should().BeFalse();
                fileWritten.Should().BeTrue();
            }

            [TestCase(" ")]
            [TestCase("")]
            [TestCase(null)]
            public async Task WhenPathContainsNullOrWhiteSpace_ExpectErrorMessage(string path)
            {
                //---------------Arrange-------------------
                var document = Create_CsvFile("test.csv");

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Append(path, document);
                //---------------Assert-----------------------
                result.HadError.Should().BeTrue();
            }

            [TestCase("abc")]
            [TestCase("~f0")]
            public async Task WhenRelativePath_ExpectFileWritten(string path)
            {
                //---------------Arrange-------------------
                var document = Create_CsvFile("test.csv");

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Append(path, document);
                //---------------Assert-----------------------
                var fileWritten = File.Exists(Path.Combine(path, "test.csv"));
                result.HadError.Should().BeFalse();
                fileWritten.Should().BeTrue();
            }

            [Test]
            public async Task WhenFileAndPathValid_ExpectFullFilePathReturned()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();
                var fileName = Guid.NewGuid() + ".csv";
                var document = Create_CsvFile(fileName);

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = await sut.Append(path, document);
                //---------------Assert-----------------------
                var expected = Path.Combine(path, fileName);
                result.FullFilePath.Should().Be(expected);
            }
        }

        [TestFixture]
        class List
        {
            [Test]
            public void WhenDirectoryExist_ExpectContents()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = sut.List(path);
                //---------------Assert-----------------------
                result.Count().Should().BeGreaterThanOrEqualTo(1);
            }

            [Test]
            public void WhenFilePassedIn_ExpectEmptyList()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempFileName();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = sut.List(path);
                //---------------Assert-----------------------
                result.Should().BeEmpty();
            }

            [TestCase(" ")]
            [TestCase("")]
            [TestCase(null)]
            public void WhenNullOrWhiteSpaceDirectory_ExpectEmptyList(string path)
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act----------------------
                var result = sut.List(path);
                //---------------Assert-----------------------
                result.Should().BeEmpty();
            }
        }

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

            [Test]
            public async Task ShouldBehaveSameAsSyncVersion()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();

                var sut = new FileSystem();
                //---------------Act----------------------
                var syncResult = sut.List(path);
                var asyncResult = await sut.ListAsync(path);
                //---------------Assert-----------------------
                asyncResult.Should().BeEquivalentTo(syncResult);
            }
        }

        [TestFixture]
        class Exists
        {
            [Test]
            public void WhenFileDoesNotExist_ExpectFalse()
            {
                //---------------Arrange-------------------
                var path = Create_Missing_File();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = sut.Exists(path);
                //---------------Assert-----------------------
                result.Should().BeFalse();
            }

            [Test]
            public void WhenFileExist_ExpectTrue()
            {
                //---------------Arrange-------------------
                var path = Create_File();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = sut.Exists(path);
                //---------------Assert-----------------------
                result.Should().BeTrue();
            }

            [TestCase(" ")]
            [TestCase("")]
            [TestCase(null)]
            public void WhenFileNullOrWhiteSpace_ExpectTrue(string path)
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act----------------------
                var result = sut.Exists(path);
                //---------------Assert-----------------------
                result.Should().BeFalse();
            }
            
            [Test]
            public void WhenDirectoryExist_ExpectTrue()
            {
                //---------------Arrange-------------------
                var path = Path.GetTempPath();

                var sut = new FileSystem();
                //---------------Act----------------------
                var result = sut.Exists(path);
                //---------------Assert-----------------------
                result.Should().BeTrue();
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

            [Test]
            public async Task ShouldBehaveSameAsSyncVersion()
            {
                //---------------Arrange-------------------
                var path = Create_File();
                var nonExistingPath = Create_Missing_File();

                var sut = new FileSystem();
                //---------------Act----------------------
                var syncExistResult = sut.Exists(path);
                var asyncExistResult = await sut.ExistsAsync(path);
                var syncNonExistResult = sut.Exists(nonExistingPath);
                var asyncNonExistResult = await sut.ExistsAsync(nonExistingPath);
                //---------------Assert-----------------------
                asyncExistResult.Should().Be(syncExistResult);
                asyncNonExistResult.Should().Be(syncNonExistResult);
            }
        }

        [TestFixture]
        class Delete
        {
            [Test]
            public void WhenFileExist_ExpectItIsRemoved()
            {
                //---------------Arrange-------------------
                var path = Create_File();
                
                var sut = new FileSystem();
                //---------------Act----------------------
                sut.Delete(path);
                //---------------Assert-----------------------
                var fileExists = File.Exists(path);
                fileExists.Should().BeFalse();
            }

            [Test]
            public void WhenFileDoesNotExist_ExpectNothingToHappen()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                var sut = new FileSystem();
                //---------------Act----------------------
                sut.Delete(path);
                //---------------Assert-----------------------
                var fileExists = File.Exists(path);
                fileExists.Should().BeFalse();
            }

            [TestCase(" ")]
            [TestCase("")]
            [TestCase(null)]
            public void WhenFileNullOrWhitespace_ExpectNoExceptionsThrown(string path)
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act----------------------
                Assert.DoesNotThrow(()=>sut.Delete(path));
                //---------------Assert-----------------------
                var fileExists = File.Exists(path);
                fileExists.Should().BeFalse();
            }

            [Test]
            public void WhenDirectoryExist_ExpectItIsRemoved()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(path);

                var sut = new FileSystem();
                //---------------Act----------------------
                sut.Delete(path);
                //---------------Assert-----------------------
                var fileExists = File.Exists(path);
                fileExists.Should().BeFalse();
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
                //---------------Act----------------------
                await sut.DeleteAsync(path);
                //---------------Assert-----------------------
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
                var fileExists = File.Exists(path);
                fileExists.Should().BeFalse();
            }

            [Test]
            public async Task ShouldBehaveSameAsSyncVersion()
            {
                //---------------Arrange-------------------
                var path1 = Create_File();
                var path2 = Create_File();
                
                var sut = new FileSystem();
                //---------------Act----------------------
                sut.Delete(path1);
                await sut.DeleteAsync(path2);
                //---------------Assert-----------------------
                var syncFileExists = File.Exists(path1);
                var asyncFileExists = File.Exists(path2);
                syncFileExists.Should().BeFalse();
                asyncFileExists.Should().BeFalse();
            }
        }

        [TestFixture]
        class ReadDocument
        {
            [Test]
            public void GivenFileExist_ExpectDocumentWithBytesReturned()
            {
                //---------------Arrange-------------------
                var contents = "hi, this is some text for a file";

                var path = Create_File(contents);
                
                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.GetDocument(path);
                //---------------Assert-----------------------
                var expected = "hi, this is some text for a file";
                actual.ToString().Should().Be(expected);
            }

            [Test]
            public void GivenFileDoesExist_ExpectNullDocument()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.GetDocument(path);
                //---------------Assert-----------------------
                actual.Should().Be(FileSystem.NullDocument);
            }

            [Test]
            public void GivenNullPath_ExpectNullDocument()
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.GetDocument(null);
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

            [Test]
            public async Task ShouldBehaveSameAsSyncVersion()
            {
                //---------------Arrange-------------------
                var contents = "hi, this is some text for a file";
                var path = Create_File(contents);
                
                var sut = new FileSystem();
                //---------------Act----------------------
                var syncResult = sut.GetDocument(path);
                var asyncResult = await sut.GetDocumentAsync(path);
                //---------------Assert-----------------------
                asyncResult.Name.Should().Be(syncResult.Name);
                asyncResult.Data.Should().BeEquivalentTo(syncResult.Data);
            }
        }

        [TestFixture]
        class Read
        {
            [Test]
            public void GivenFileExist_ExpectDocumentWithBytesReturned()
            {
                //---------------Arrange-------------------
                var contents = "hi, this is some text for a file";

                var path = Create_File(contents);

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.Read(path);
                //---------------Assert-----------------------
                var expected = "hi, this is some text for a file";
                actual.ToString().Should().Be(expected);
            }

            [Test]
            public void GivenFileDoesExist_ExpectNullDocument()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.Read(path);
                //---------------Assert-----------------------
                actual.Should().Be(FileSystem.NullDocument);
            }

            [Test]
            public void GivenNullPath_ExpectNullDocument()
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.Read(null);
                //---------------Assert-----------------------
                actual.Should().Be(FileSystem.NullDocument);
            }
        }

        [TestFixture]
        class ReadAllLines
        {
            [Test]
            public async Task GivenFileExist_ExpectDocumentWithBytesReturned()
            {
                //---------------Arrange-------------------
                var path = Create_File_With_Million_Lines();

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = await sut.ReadAllLines(path);
                //---------------Assert-----------------------
                var expectedFirst = "GlobalRank,TldRank,Domain,TLD,RefSubNets,RefIPs,IDN_Domain,IDN_TLD,PrevGlobalRank,PrevTldRank,PrevRefSubNets,PrevRefIPs";
                var expectedLast = "1000000,499336,alexandrevicenzi.com,com,341,364,alexandrevicenzi.com,com,982364,490355,345,368";
                var enumerable = actual as string[] ?? actual.ToArray();
                enumerable.Count().Should().Be(1000001);
                enumerable.FirstOrDefault().Should().BeEquivalentTo(expectedFirst);
                enumerable.LastOrDefault().Should().BeEquivalentTo(expectedLast);
            }

            [Test]
            public void GivenFileDoesExist_ExpectException()
            {
                //---------------Arrange-------------------
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = Assert.ThrowsAsync<FileNotFoundException>(async ()=>await sut.ReadAllLines(path));
                //---------------Assert-----------------------
                actual?.Message.Should().NotBeEmpty();
            }

            [Test]
            public void GivenNullPath_ExpectException()
            {
                //---------------Arrange-------------------
                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.ReadAllLines(null));
                //---------------Assert-----------------------
                actual?.Message.Should().NotBeEmpty();
            }
        }

        [TestFixture]
        class Move
        {
            [Test]
            public void GivenFileExist_ExpectItIsMoved()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = Create_File();
                File.Delete(newFileName);

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.Move(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(newFileName);

                actual.Should().BeTrue();
                oldFileExist.Should().BeFalse();
                newFileExist.Should().BeTrue();
            }

            [Test]
            public void GivenFileDoesNotExist_ExpectFalse()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = Create_File();
                File.Delete(newFileName);
                File.Delete(file);

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.Move(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(newFileName);

                actual.Should().BeFalse();
                oldFileExist.Should().BeFalse();
                newFileExist.Should().BeFalse();
            }

            [Test]
            public void GivenDestinationFileExist_ExpectItIsNotMoved()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = Create_File();
                
                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.Move(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(newFileName);

                actual.Should().BeFalse();
                oldFileExist.Should().BeTrue();
                newFileExist.Should().BeTrue();
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

            [Test]
            public async Task ShouldBehaveSameAsSyncVersion()
            {
                //---------------Arrange-------------------
                var file1 = Create_File();
                var file2 = Create_File();
                var newFileName1 = Create_File();
                var newFileName2 = Create_File();
                File.Delete(newFileName1);
                File.Delete(newFileName2);

                var sut = new FileSystem();
                //---------------Act----------------------
                var syncResult = sut.Move(file1, newFileName1);
                var asyncResult = await sut.MoveAsync(file2, newFileName2);
                //---------------Assert-----------------------
                syncResult.Should().BeTrue();
                asyncResult.Should().BeTrue();
                
                File.Exists(file1).Should().BeFalse();
                File.Exists(file2).Should().BeFalse();
                File.Exists(newFileName1).Should().BeTrue();
                File.Exists(newFileName2).Should().BeTrue();
            }
        }

        [TestFixture]
        class MoveWithOverwrite
        {
            [Test]
            public void GivenDestinationFileExist_ExpectItIsMoved()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = Create_File();

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.MoveWithOverwrite(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(newFileName);

                actual.Should().BeTrue();
                oldFileExist.Should().BeFalse();
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
                actual.Should().BeFalse();
            }

            [Test]
            public async Task ShouldBehaveSameAsSyncVersion()
            {
                //---------------Arrange-------------------
                var file1 = Create_File("test content 1");
                var file2 = Create_File("test content 2");
                var dest1 = Create_File("original content 1");
                var dest2 = Create_File("original content 2");

                var sut = new FileSystem();
                //---------------Act----------------------
                var syncResult = sut.MoveWithOverwrite(file1, dest1);
                var asyncResult = await sut.MoveWithOverwriteAsync(file2, dest2);
                //---------------Assert-----------------------
                syncResult.Should().BeTrue();
                asyncResult.Should().BeTrue();
                
                File.Exists(file1).Should().BeFalse();
                File.Exists(file2).Should().BeFalse();
                File.Exists(dest1).Should().BeTrue();
                File.Exists(dest2).Should().BeTrue();
            }
        }

        [TestFixture]
        class Rename
        {
            [Test]
            public void GivenFileExist_ExpectItIsRenamed()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = $"{Guid.NewGuid()}-moved.txt";

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.Rename(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(Path.Combine(Path.GetTempPath(), newFileName));

                actual.Should().BeTrue();
                oldFileExist.Should().BeFalse();
                newFileExist.Should().BeTrue();
            }

            [Test]
            public void GivenFileDoesNotExist_ExpectFalse()
            {
                //---------------Arrange-------------------
                var file = Create_File();
                var newFileName = $"{Guid.NewGuid()}-moved.txt";
                File.Delete(file);

                var sut = new FileSystem();
                //---------------Act----------------------
                var actual = sut.Rename(file, newFileName);
                //---------------Assert-----------------------
                var oldFileExist = File.Exists(file);
                var newFileExist = File.Exists(Path.Combine(Path.GetTempPath(), newFileName));

                actual.Should().BeFalse();
                oldFileExist.Should().BeFalse();
                newFileExist.Should().BeFalse();
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
            public async Task ShouldBehaveSameAsSyncVersion()
            {
                //---------------Arrange-------------------
                var file1 = Create_File();
                var file2 = Create_File();
                var newFileName1 = $"{Guid.NewGuid()}-moved.txt";
                var newFileName2 = $"{Guid.NewGuid()}-moved.txt";

                var sut = new FileSystem();
                //---------------Act----------------------
                var syncResult = sut.Rename(file1, newFileName1);
                var asyncResult = await sut.RenameAsync(file2, newFileName2);
                //---------------Assert-----------------------
                syncResult.Should().BeTrue();
                asyncResult.Should().BeTrue();
                
                File.Exists(file1).Should().BeFalse();
                File.Exists(file2).Should().BeFalse();
                File.Exists(Path.Combine(Path.GetTempPath(), newFileName1)).Should().BeTrue();
                File.Exists(Path.Combine(Path.GetTempPath(), newFileName2)).Should().BeTrue();
            }
        }

        private static string Create_File_With_Million_Lines()
        {
            var tmp = Path.GetTempPath();
            var path = Path.Combine(tmp, Guid.NewGuid().ToString());

            var location = TestContext.CurrentContext.WorkDirectory;
            var moveFilePath = Path.Combine(location, "majestic_million.csv");
            File.Move(moveFilePath, path);

            return path;
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

        private static IDocument Create_CsvFile(string fileName)
        {
            var csvFile = new DocumentBuilder()
                                .With_Name(fileName)
                                .With_Bytes(new byte[5])
                                .Create_Document();
            return csvFile;
        }

    }
}
