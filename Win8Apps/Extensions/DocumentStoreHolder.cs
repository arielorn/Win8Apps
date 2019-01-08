using System;
using System.Net;
using Raven.Client;
using Raven.Client.Document;

namespace Win8Apps.Extensions
{
    public static class DocumentStoreHolder
    {
        private static readonly Lazy<IDocumentStore> _documentStore = new Lazy<IDocumentStore>(CreateDocumentStore);

        public static Guid ResourceManagerId { get; set; }
        public static IDocumentStore DocumentStore
        {
            get { return _documentStore.Value; }
        }

        private static IDocumentStore CreateDocumentStore()
        {
            var documentStore = new DocumentStore
                {
                    ConnectionStringName = "RavenHQ"
                }.Initialize();

            return documentStore;
        }

        public static IDocumentSession OpenSession(string db = null)
        {

            if (db == null)
                return DocumentStore.OpenSession();

            return DocumentStore.OpenSession(db);
        }
    }
}