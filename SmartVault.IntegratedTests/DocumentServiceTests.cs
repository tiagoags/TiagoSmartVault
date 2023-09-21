using NUnit.Framework;
using SmartVault.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace SmartVault.IntegratedTests
{
    public class DocumentServiceTests
    {
        DocumentService documentService;
        [OneTimeSetUp]
        public void Setup()
        {
            DataGeneration.Program.DataBaseGeneration(1, 3);
            documentService = new DocumentService();
        }

        [Test]
        public async Task WriteEveryThirdFileToFile_ShouldGenerateNewFile()
        {
            var newFileName = documentService.WriteEveryThirdFileToFile();
            Assert.IsTrue(File.Exists(newFileName));
        }

        [Test]
        public async Task WriteEveryThirdFileToFile_WhenFilteredForSpecificAccount_ShouldGenerateNewFile()
        {
            var newFileName = documentService.WriteEveryThirdFileToFile("0");
            Assert.IsTrue(File.Exists(newFileName));
        }

        [Test]
        public async Task WriteEveryThirdFileToFile_WhenFilteredForInvalidAccount_ShouldGenerateEmptyNewFile()
        {
            var newFileName = documentService.WriteEveryThirdFileToFile("9999999");
            var fileInfo = new FileInfo(newFileName);
            Assert.IsTrue(fileInfo.Exists);
            Assert.That(fileInfo.Length, Is.EqualTo(0));

        }

        [Test]
        public async Task SmartVault_GetAllFileSizes()
        {
            var size = documentService.GetAllFileSizes();

            Assert.AreEqual(7878, size);
        }
    }
}