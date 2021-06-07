using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DataReducer {

    public sealed class Program {

        private readonly IAmazonDynamoDB _dynamoService;

        private const string MapReduceDataTable = "cosc2640a3.map.reduce.data";
        private const string MapReduceResultsTable = "cosc2640a3.map.reduce.results";

        public Program() {
            _dynamoService = new AmazonDynamoDBClient(
                "AKIAJSENDXCAPZWGB6HQ",
                "HeGULGolRgnxwKIIm4K2d8E+sAoHVBukvR+5umU3",
                new AmazonDynamoDBConfig {
                    RegionEndpoint = RegionEndpoint.APSoutheast2
                }
            );
        }

        public static async Task Main(string[] args) {
            var program = new Program();

            var data = await GetData(program._dynamoService);
            if (data == null) return;

            var range50Counts = 0;
            var range100Counts = 0;
            var range250Counts = 0;
            var range500Counts = 0;
            var range1000Counts = 0;
            var range1001Counts = 0;

            foreach (var item in data) {
                if (item.DueAmount <= 50) range50Counts++;
                if (item.DueAmount > 50 && item.DueAmount <= 100) range100Counts++;
                if (item.DueAmount > 100 && item.DueAmount <= 250) range250Counts++;
                if (item.DueAmount > 250 && item.DueAmount <= 500) range500Counts++;
                if (item.DueAmount > 500 && item.DueAmount <= 1000) range1000Counts++;
                if (item.DueAmount > 1000) range1001Counts++;
            }

            var total = data.Length;
            var stats = new {
                Range50 = Math.Round(range50Counts * 100.0 / total, 2),
                Range100 = Math.Round(range100Counts * 100.0 / total, 2),
                Range250 = Math.Round(range250Counts * 100.0 / total, 2),
                Range500 = Math.Round(range500Counts * 100.0 / total, 2),
                Range1000 = Math.Round(range1000Counts * 100.0 / total, 2),
                Range1001 = Math.Round(range1001Counts * 100.0 / total, 2)
            };

            await SaveResult(JsonConvert.SerializeObject(stats), program._dynamoService);
            foreach (var item in data) await DeleteData(item.Id, program._dynamoService);
        }

        private static async Task SaveResult(string result, IAmazonDynamoDB dynamoService) {
            var resultTable = Table.LoadTable(dynamoService, MapReduceResultsTable);
            await resultTable.PutItemAsync(new Document {
                ["Id"] = Guid.NewGuid().ToString().ToLower(),
                ["Statistics"] = result,
                ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
        }

        private static async Task<EnrolmentInvoice[]> GetData(IAmazonDynamoDB dynamoService) {
            try {
                var scanAllRequest = new ScanRequest { TableName = MapReduceDataTable };

                var response = await dynamoService.ScanAsync(scanAllRequest);
                if (response.HttpStatusCode != HttpStatusCode.OK) throw new InternalServerErrorException("Request to AWS DynamoDb failed.");

                return response.Items.Select(item => (EnrolmentInvoice) item).ToArray();
            }
            catch (Exception) { return null; }
        }

        private static async Task DeleteData(string key, IAmazonDynamoDB dynamoDB) {
            var deleteItemRequest = new DeleteItemRequest {
                TableName = MapReduceDataTable,
                ReturnValues = ReturnValue.NONE,
                Key = new Dictionary<string, AttributeValue> {
                    { nameof(EnrolmentInvoice.Id), new AttributeValue { S = key } }
                }
            };

            _ = await dynamoDB.DeleteItemAsync(deleteItemRequest);
        }

        public sealed class EnrolmentInvoice {
            [DynamoDBHashKey]
            public string Id { get; set; }
            public double DueAmount { get; set; }

            public static implicit operator EnrolmentInvoice(Dictionary<string, AttributeValue> item) {
                return new EnrolmentInvoice {
                    Id = item[nameof(Id)].S,
                    DueAmount = double.Parse(item[nameof(DueAmount)].N)
                };
            }
        }
    }
}
