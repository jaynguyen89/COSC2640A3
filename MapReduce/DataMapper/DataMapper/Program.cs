using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataMapper {

    public sealed class Program {

        private readonly IAmazonDynamoDB _dynamoService;
        private readonly SqlConnection _dbConnection;

        private const string MapReduceDataTable = "cosc2640a3.map.reduce.data";
        private const string DbConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=COSC2640A3;Trusted_Connection=True;";

        public Program() {
            _dynamoService = new AmazonDynamoDBClient(
                "AKIAJSENDXCAPZWGB6HQ",
                "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3",
                new AmazonDynamoDBConfig {
                    RegionEndpoint = RegionEndpoint.APSoutheast2
                }
            );

            _dbConnection = new SqlConnection(DbConnectionString);
        }

        public static async Task Main(string[] args) {
            var program = new Program();

            var query = "SELECT E.Id, I.DueAmount FROM Enrolment E, Invoice I WHERE E.InvoiceId = I.Id;";
            var command = new SqlCommand(query, program._dbConnection);

            program._dbConnection.Open();
            using var reader = command.ExecuteReader();

            var enrolmentInvoices = new List<EnrolmentInvoice>();
            while (reader.Read()) {
                var item = new EnrolmentInvoice {
                    Id = reader[nameof(EnrolmentInvoice.Id)].ToString(),
                    DueAmount = double.Parse(reader[nameof(EnrolmentInvoice.DueAmount)].ToString())
                };

                enrolmentInvoices.Add(item);
            }

            program._dbConnection.Close();
            await SaveToDynamoAsync(enrolmentInvoices.ToArray(), program._dynamoService);
        }

        private static async Task SaveToDynamoAsync(EnrolmentInvoice[] data, IAmazonDynamoDB dynamoDB) {
            Table mrTable = Table.LoadTable(dynamoDB, MapReduceDataTable);
            foreach (var d in data) await mrTable.PutItemAsync(d.CreateDocument());
        }

        public sealed class EnrolmentInvoice {
            [DynamoDBHashKey]
            public string Id { get; set; }
            public double DueAmount { get; set; }

            public Document CreateDocument() {
                return new Document {
                    [nameof(Id)] = Id.ToLower(),
                    [nameof(DueAmount)] = DueAmount
                };
            }
        }
    }
}
