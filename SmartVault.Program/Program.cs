using SmartVault.Core.Services;

namespace SmartVault.Program
{

    partial class Program
    {
        static void Main(string[] args)
        {
            var documentService = new DocumentService();
            if (args.Length == 0)
            {
                return;
            }

            documentService.WriteEveryThirdFileToFile(args[0]);
            documentService.GetAllFileSizes();
        }

    }
}