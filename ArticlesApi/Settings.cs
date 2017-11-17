using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace ArticlesApi
{
    public enum DbType
    {
        Primary, Second
    }
    public static class Settings
    {
        // Primary = EAST
        public static string EndpointPrimary = @"https://itemsdb-east.documents.azure.com:443/";
        public static string PrimaryKey = @"6Mjdly6aVhhTPuhEwS1a26nL7MRcSFH2hPBHyYbOiJOOflVcJOO1sayQMw5rHTLsQVjJayDHfipA5ya1Go4fPw==";
        public static List<string> PrimaryLocations = new List<string> { "eastus", "westus" };

        // Secondary WEST
        public static string EndpointSecondary = @"https://itemsdb-west.documents.azure.com:443/";
        public static string SecondaryKey = @"3gAb8VXGBOa6eYBzBUwUSkXBs8PZzNE9l7su8KZTUFFWFwRg7L3j5d65ZvCiCXGagJg9Qr9Ef75OeOFwaavpeg==";
        public static List<string> SecondaryLocations = new List<string> { "westus", "eastus" };

        public static string DatabaseId = "ToDoList";
        public static string CollectionId = "Items";

        public static DocumentClient GetDocumentClient(DbType type)
        {
            var policy = new ConnectionPolicy();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                policy.ConnectionMode = ConnectionMode.Direct;
                policy.ConnectionProtocol = Protocol.Tcp;
            }

            if (type == DbType.Primary)
            {
                policy.PreferredLocations.Add(LocationNames.EastUS);
                policy.PreferredLocations.Add(LocationNames.WestUS);
                return new DocumentClient(new Uri(EndpointPrimary), PrimaryKey, policy);
            }
            else
            {
                policy.PreferredLocations.Add(LocationNames.WestUS);
                policy.PreferredLocations.Add(LocationNames.EastUS);
                return new DocumentClient(new Uri(EndpointSecondary), SecondaryKey, policy);
            }
        }
    }
}
