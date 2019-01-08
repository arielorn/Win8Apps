using Quartz;
using Raven.Client;
using Raven.Client.Document;

namespace WSReaper.Jobs
{
    public abstract class ReapJob : IJob
    {
        public IDocumentStore Store { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            using (Store = new DocumentStore
                    {
                        ConnectionStringName = "RavenHQ"
                    }.Initialize())
            {

                ConcreteExecute(context);
            }
        }

        protected abstract void ConcreteExecute(IJobExecutionContext context);

    }
}