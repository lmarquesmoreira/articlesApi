using Microsoft.Azure.Documents.Client;
using ArticlesApi.Model;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ArticlesApi.Services {

    public class ItemResult {
        public bool Saved { get;set;}
        public string Message { get; set; }
        public string StackTrace { get;set;}
    }

    public class ItemService 
    {
        public async Task<ItemResult> Insert(DocumentClient client, Item item) {
            var result = new ItemResult();

            item.id = DateTime.Now.Ticks.ToString();
            item.location = System.Net.Dns.GetHostName();
            item.fromEndpoint = client.WriteEndpoint.ToString();

            try {
                var uri = UriFactory.CreateDocumentCollectionUri(Settings.DatabaseId, Settings.CollectionId);
                var doc = await client.CreateDocumentAsync(uri, item);

                doc.Resource.SetPropertyValue("location", client.WriteEndpoint.ToString());

                await client.UpsertDocumentAsync(uri, doc.Resource);

                result.Saved = true;
                result.Message =  $"Added data in ${client.WriteEndpoint} ";

            } catch (Exception ex){
                result.Saved = true;
                result.Message =  $"Error during adding data in ${client.WriteEndpoint} - {ex.Message} ";
                result.StackTrace = ex.StackTrace;
            }

            return result;
        }

        public IEnumerable<Item> Get(DocumentClient first, DocumentClient secondaryClient) {
            var results = new HashSet<Item>(new ItemComparer());
            // re-init DocumentClient for each GET
            // just in case
            Console.WriteLine( "First Read");
            ExecuteQuery( first, results );

            Console.WriteLine("Second");
            ExecuteQuery( secondaryClient, results );

            return results.AsEnumerable<Item>();
        }

        private void ExecuteQuery( DocumentClient client, HashSet<Item> results )
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
            var uri = UriFactory.CreateDocumentCollectionUri(Settings.DatabaseId, Settings.CollectionId);

            Console.WriteLine( "Executing Query at: {0}", uri );
            IQueryable<Item> itemQuery = client.CreateDocumentQuery<Item>(uri, queryOptions);

            Console.WriteLine( string.Format("Query from endpoint: {0}", client.ReadEndpoint.ToString()));

            var counter = 0;
            foreach ( var i in itemQuery.AsEnumerable<Item>() )
            {
                var added = results.Add(i);
                if( ! added ) {
                    Console.WriteLine( string.Format("Element not added {0}", i.id));
                }
                counter++;
            }

            Console.WriteLine( string.Format( "Query with {0} results, collection with {1}", counter, results.Count));
        }

    }
}