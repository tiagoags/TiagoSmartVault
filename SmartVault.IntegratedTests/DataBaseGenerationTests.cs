using Dapper;
using System.Data.SQLite;

namespace SmartVault.IntegratedTests
{
    public class DataBaseGenerationTests
    {

        [Test]
        public async Task DataBaseGeneration_ShouldTakeLessThan30seconds()
        {
            int timeout = 30000;
            Task task = Task.Run(() => DataGeneration.Program.DataBaseGeneration());

            // Wait for the task to complete or timeout
            if (Task.WaitAny(new Task[] { task }, timeout) != 0)
            {
                // The task timed out
                Assert.Fail("DataBaseGeneration execution timed out.");
            }
            else
            {

                Assert.Pass("DataBaseGeneration has been completed under 30 seconds");
            }
        }

        [Test]
        public async Task DataBaseGeneration_ShouldCreateEntites()
        {
            DataGeneration.Program.DataBaseGeneration(1, 1);
            using (var connection = new SQLiteConnection(DataGeneration.Program.GetConfiguration().FormatedConnectionString()))
            {
                var accounts = connection.Query("SELECT * FROM Account;");

                var account = accounts.Single();
                Assert.AreEqual(0, account.Id);
                Assert.AreEqual("Account0", account.Name);

                var users = connection.Query("SELECT * FROM User;");

                var user = users.Single();
                //Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password
                Assert.AreEqual(0, user.Id);
                Assert.AreEqual("FName0", user.FirstName);
                Assert.AreEqual("LName0", user.LastName);
                Assert.AreEqual(0, user.AccountId);
                //TODO complete check the expected seed data info...

                var documents = connection.Query("SELECT * FROM Document;");
                var document = documents.Single();
                Assert.AreEqual(0, document.Id);
            }
        }


    }
}