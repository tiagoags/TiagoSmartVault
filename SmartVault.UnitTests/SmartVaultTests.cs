using NUnit.Framework;
using SmartVault.Program.BusinessObjects;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SmartVault.UnitTests
{
    public class Tests
    {

        [Test]
        public async Task BusinesObjects_ShouldHaveCreatedOnProperty()
        {
            string targetNamespace = "SmartVault.Program.BusinessObjects";

            // Get the assembly where your classes are defined
            Assembly assembly = typeof(Document).Assembly;

            Type[] types = assembly.GetTypes()
                .Where(t => t.Namespace == targetNamespace)
                .ToArray();

            Assert.IsTrue(types.All(t => t.GetProperty("CreatedOn") != null));
        }
    }
}