using CVRPlib;
using CVRPlib.Interfaces;
using CVRPlib.Models;
using CVRPlib.Solvers;
using System.Diagnostics;
using System.Net.WebSockets;

namespace CVRP_webapp.Services
{
#pragma warning disable S101
    public class CVRPBackgroundService : BackgroundService
#pragma warning restore S101
    {
        readonly CVRPJobStore _jobStore;
        readonly CVRPJobQueue _jobQueue;
        readonly JsonJobStorage _jobStorage;

        public CVRPBackgroundService(CVRPJobStore jobStore, CVRPJobQueue jobQueue, JsonJobStorage jobStorage)
        {
            _jobQueue = jobQueue;
            _jobStore = jobStore;
            _jobStorage = jobStorage;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var jobId = await _jobQueue.DequeueAsync(stoppingToken);

                if(!_jobStore.TryGetValue(jobId, out var job))
                {
                    continue;
                }

                await ProcessJob(job!, stoppingToken);
            }
        }

        async Task ProcessJob(CVRPJob job, CancellationToken cancellationToken)
        {
            job.JobStatus = JobStatus.Running;
            await _jobStorage.SaveAsync(job);

            try
            {
                ISolver algo = job.Parameters.Solver switch
                {
                    "HillClimbingSolver" => new HillClimbingSolver(),
                    _ => new HillClimbingSolver()
                };

                var task = new CVRPTask(job.InputData, algo, new TargetFunctionStrict());

                var res = await Task.Run(() => task.GetSolution(job.Parameters.MultistartCount, job.Parameters.HistoryCount), cancellationToken);

                job.Result = res;
                job.JobStatus = JobStatus.Completed;
                await _jobStorage.SaveAsync(job);
            }
            catch(Exception ex)
            {
                job.Error = ex.Message;
                job.JobStatus = JobStatus.Failed;
                await _jobStorage.SaveAsync(job);
            }
        }
    }
}
