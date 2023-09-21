using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartVault.Library;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SmartVault.DataGeneration
{

    public partial class Program
    {
        static void Main(string[] args)
        {
            DataBaseGeneration();
        }

        public static void DataBaseGeneration(int numberOfUsersAndAccounts = 100, int numberOfDocumentsPerUser = 10000)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            DataBaseCreationConfig configuration = GetConfiguration();

            SQLiteConnection.CreateFile(configuration.DatabaseFileName);
            File.WriteAllText("TestDoc.txt", $"This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}");

            var randomDayIterator = RandomDay().GetEnumerator();

            using (var connection = new SQLiteConnection(configuration.FormatedConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var files = Directory.GetFiles(@"..\..\..\..\BusinessObjectSchema");
                    //scheme creation
                    for (int i = 0; i <= 2; i++)
                    {
                        var serializer = new XmlSerializer(typeof(BusinessObject));
                        var businessObject = serializer.Deserialize(new StreamReader(files[i])) as BusinessObject;
                        connection.Execute(businessObject?.Script);

                    }


                    var newUsers = Enumerable.Range(0, numberOfUsersAndAccounts).Select(
                            userId =>
                            {
                                return new
                                {
                                    userId = userId,
                                    firstName = $"FName{userId}",
                                    lastName = $"LName{userId}",
                                    dateOfBirth = randomDayIterator.MoveNext() ? randomDayIterator.Current : DateTime.MinValue,
                                    accountId = userId,
                                    userName = $"UserName-{userId}",
                                    accountName = $"Account{userId}"
                                };
                            }
                            );
                    //https://github.com/DapperLib/Dapper#execute-a-command-multiple-times
                    var usersAndAccountsCreatedCount = connection.Execute(@"INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password)
                        VALUES(@userId,@firstName,@lastName,@dateOfBirth,@accountId,@userName, 'e10adc3949ba59abbe56e057f20f883e');
                        INSERT INTO Account(Id, Name) VALUES(@userId, @accountName);", newUsers, transaction);


                    var fileInfo = new FileInfo("TestDoc.txt");

                    int documentNumber = 0;
                    var allNewDocs = Enumerable.Range(0, numberOfUsersAndAccounts).SelectMany(userId =>
                    {
                        return Enumerable.Range(0, numberOfDocumentsPerUser).Select(
                            x =>
                            {
                                return new
                                {
                                    docNumber = documentNumber++,
                                    docName = $"Document{userId}-{x}.txt",
                                    docPath = fileInfo.FullName,
                                    docPathLength = fileInfo.Length,
                                    accountId = userId
                                };
                            }
                            );
                    });

                    var documentsCreatedCount = connection.Execute(@"insert into Document(Id, Name, FilePath, Length, AccountId) 
                    values(@docNumber,@docName,@docPath,@docPathLength,@accountId)", allNewDocs, transaction);

                    transaction.Commit();
                }

                stopWatch.Stop();
                Console.WriteLine($"creationg of db took: {stopWatch.ElapsedMilliseconds} ms");


                var accountData = connection.Query("SELECT COUNT(*) FROM Account;");
                Console.WriteLine($"AccountCount: {JsonConvert.SerializeObject(accountData)}");
                var documentData = connection.Query("SELECT COUNT(*) FROM Document;");
                Console.WriteLine($"DocumentCount: {JsonConvert.SerializeObject(documentData)}");
                var userData = connection.Query("SELECT COUNT(*) FROM User;");
                Console.WriteLine($"UserCount: {JsonConvert.SerializeObject(userData)}");
            }
        }

        public static DataBaseCreationConfig GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            DataBaseCreationConfig c = new DataBaseCreationConfig("", null);
            config.Bind(c);
            return c;
        }

        static IEnumerable<DateTime> RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                yield return start.AddDays(gen.Next(range));
        }
    }
}