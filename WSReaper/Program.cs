using System.Collections.Specialized;
using Quartz;
using Quartz.Impl;
using Topshelf;
using WSReaper.Jobs;

namespace WSReaper
{
    internal class Program
    {
        private static void Main()
        {
            var featuredAppsJob = new FeaturedAppsJob();
            featuredAppsJob.Execute(null);

            var getAppsJob = new GetAppsJob();
            getAppsJob.Execute();

            var loadAllAppsJob = new LoadAllAppsJob();
            loadAllAppsJob.Execute(null);

            var fixFriendlyNamesJob = new FixFriendlyNamesJob();
            fixFriendlyNamesJob.AllApps = loadAllAppsJob.AllApps;
            fixFriendlyNamesJob.Execute(null);

            var markRemovedAppsJob = new MarkRemovedAppsJob();
            markRemovedAppsJob.AllApps = loadAllAppsJob.AllApps;
            markRemovedAppsJob.AllActiveAppIds = getAppsJob.AppIds;
            markRemovedAppsJob.Execute(null);

            new StatsJob().Execute(null);
            
            var createSiteMapJob = new CreateSiteMapJob();
            createSiteMapJob.AllApps = loadAllAppsJob.AllApps;
            createSiteMapJob.Execute(null);

            return;

            HostFactory.Run(x =>
            {
                x.Service<ReaperService>(s =>
                {
                    s.ConstructUsing(name => new ReaperService());
                    s.WhenStarted(rs => rs.Start());
                    s.WhenStopped(rs => rs.Stop());
                });

                x.EnablePauseAndContinue();
                x.RunAsLocalSystem();

                x.SetDescription("Reaper Service");
                x.SetDisplayName("Reaper Service");
                x.SetServiceName("ReaperService");
            });     

         
        }
    }

    public class ReaperService
    {
        public IScheduler Scheduler { get; set; }

        public void Start()
        {
            var properties = new NameValueCollection();
            properties["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz";
            properties["quartz.plugin.xml.fileNames"] = "./quartz_jobs.xml";

            Scheduler = new StdSchedulerFactory(properties).GetScheduler();
            Scheduler.Start();


            //Scheduler.ScheduleJob()

            //var facetsJob = JobBuilder.Create<FacetsJob>()
            //                          .Build();

            //var facetsTrigger = TriggerBuilder.Create()
            //    .ForJob(facetsJob)
            //    .WithSimpleSchedule(a => a.WithIntervalInSeconds(20).RepeatForever().Build())
            //    .Build();


            //Scheduler.ScheduleJob(facetsJob, facetsTrigger);
        }

        public void Stop()
        {
            Scheduler.Shutdown();
        }
    }
}


