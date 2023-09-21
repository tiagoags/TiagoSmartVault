using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace SmartVault.Core.Services
{
    public class DocumentService : IDisposable
    {
        private SQLiteConnection _connection;
        private SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(SmartVaultConfigurationManager.GetDBConfiguration().FormatedConnectionString());
                }
                return _connection;
            }
        }

        public long GetAllFileSizes()
        {//Implement a way to get the total file size of all files.

            string query = @"select sum(d.Length) as TotalSize from document as d";
            long totalSize = Connection.Query(query).First().TotalSize;
            Console.WriteLine($"Total file size: {totalSize}");

            return totalSize;
        }

        public string WriteEveryThirdFileToFile()
        {//Implement a way to output the contents of every third file of an account to a single file.            

            //TODO change it to use sql builder and better reuse of queries
            string query = @"SELECT rd.FilePath FROM
                                    (
                                    SELECT
                                        d.*,
                                        ROW_NUMBER() OVER (PARTITION BY d.AccountId ORDER BY d.Id) AS row_num
                                    FROM
                                        Document AS d
                                )as rd
                                WHERE
                                    rd.row_num = 3;
                                ";

            var results = Connection.Query(query);
            var sourceFilePaths = results.Select<dynamic, string>(f => f.FilePath.ToString());
            string newFileName = Guid.NewGuid().ToString();

            CopytFilesCotentsIntoSingleFile(sourceFilePaths, newFileName);

            Console.WriteLine($"{results.Count()} Files been copied into {newFileName} successfully!");

            return newFileName;
        }

        public string WriteEveryThirdFileToFile(string accountId)
        {//Implement a way to output the contents of every third file of an account to a single file.            

            //TODO change it to use sql builder and better reuse of queries
            string query = @"SELECT rd.FilePath FROM
                                    (
                                    SELECT
                                        d.*,
                                        ROW_NUMBER() OVER (PARTITION BY d.AccountId ORDER BY d.Id) AS row_num
                                    FROM
                                        Document AS d 
                                    where d.AccountId = @accId
                                )as rd
                                WHERE
                                    rd.row_num = 3;
                                ";

            var results = Connection.Query(query, new { accId = accountId });
            var sourceFilePaths = results.Select<dynamic, string>(f => f.FilePath.ToString());
            string newFileName = Guid.NewGuid().ToString();

            CopytFilesCotentsIntoSingleFile(sourceFilePaths, newFileName);

            Console.WriteLine($"{results.Count()} Files been copied into {newFileName} successfully!");

            return newFileName;
        }

        private static void CopytFilesCotentsIntoSingleFile(IEnumerable<string> sourceFilePaths, string newFileName)
        {
            try
            {
                using (FileStream destinationStream = new FileStream(newFileName, FileMode.Create, FileAccess.Write))
                {
                    foreach (var sourceFilePath in sourceFilePaths)
                    {
                        using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
                        {
                            sourceStream.CopyTo(destinationStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
    }
}