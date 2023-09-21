using Dapper;
using NUnit.Framework;
using SmartVault.Core;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace SmartVault.IntegratedTests
{
    public class DataBaseGenerationTests
    {

        [Test]
        public async Task DataBaseGeneration_ShouldTakeLessThan15seconds()
        {
            int timeout = 15000;
            Task task = Task.Run(() => DataGeneration.Program.DataBaseGeneration());

            // Wait for the task to complete or timeout
            if (Task.WaitAny(new Task[] { task }, timeout) != 0)
            {
                // The task timed out
                Assert.Fail("DataBaseGeneration execution timed out.");
            }
            else
            {

                Assert.Pass("DataBaseGeneration has been completed under 15 seconds");
            }
        }

        [Test]
        public async Task DataBaseGeneration_ShouldCreateEntites()
        {
            DataGeneration.Program.DataBaseGeneration(1, 1);
            using (var connection = new SQLiteConnection(SmartVaultConfigurationManager.GetDBConfiguration().FormatedConnectionString()))
            {
                var accounts = connection.Query("SELECT * FROM Account;");

                var account = accounts.Single();
                Assert.AreEqual(0, account.Id);
                Assert.AreEqual("Account0", account.Name);
                Assert.IsTrue(HasCreatedOnProperty(account));
                Assert.AreNotEqual(DateTime.MinValue, account.CreatedOn);

                var users = connection.Query("SELECT * FROM User;");

                var user = users.Single();
                //Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password
                Assert.AreEqual(0, user.Id);
                Assert.AreEqual("FName0", user.FirstName);
                Assert.AreEqual("LName0", user.LastName);
                Assert.AreEqual(0, user.AccountId);
                Assert.IsTrue(HasCreatedOnProperty(user));
                Assert.AreNotEqual(DateTime.MinValue, user.CreatedOn);
                //TODO complete check the expected seed data info...

                var documents = connection.Query("SELECT * FROM Document;");
                var document = documents.Single();
                Assert.AreEqual(0, document.Id);
                Assert.IsTrue(HasCreatedOnProperty(document));
                Assert.AreNotEqual(DateTime.MinValue, document.CreatedOn);
            }

            static bool HasCreatedOnProperty(dynamic entity)
            {
                return ((IDictionary<string, object>)entity).ContainsKey("CreatedOn");
            }
        }



    }
}