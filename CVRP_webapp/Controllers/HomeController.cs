using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CVRP_webapp.Models;
using CVRPlib;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using CVRPlib.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using CVRPlib.Interfaces;
using CVRPlib.DataReaders;
using CVRPlib.Solvers;
using CVRP_webapp.Services;

namespace CVRP_webapp.Controllers;

public class HomeController : Controller
{
    readonly CVRPJobStore _jobStore;
    readonly CVRPJobQueue _jobQueue;
    public HomeController(CVRPJobStore jobStore, CVRPJobQueue jobQueue)
    {
        _jobStore = jobStore;
        _jobQueue = jobQueue;
    }
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Open()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Start([FromForm] IFormFile file, [FromForm] int multistartCount, [FromForm] int historyCount)
    {
        if(file == null || file.Length == 0)
        {
            return NotFound();
        }

        try
        {
            List<string> lines;

            using(var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            {
                var tmp = await stream.ReadToEndAsync();
                lines = tmp.Split("\n").ToList();
            }

            IInputDataReader dataReader;

            if (lines[0].Contains("NAME"))
            {
                dataReader = new InputDataReader();
            }
            else
            {
                dataReader = new SolomonInputDataReader();
            }

            InputData data = dataReader.ParseData(lines);

            var job = new CVRPJob() { InputData = data, Parameters = new CVRPJobParameters() { MultistartCount = multistartCount, HistoryCount = historyCount, Solver = new HillClimbingSolver().GetType().Name } };

            _jobStore.Add(job);
            await _jobQueue.EnqueueAsync(job.Id);

            return Ok(new { jobId =  job.Id });          
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Status([FromQuery] Guid guid)
    {
        if(!_jobStore.TryGetValue(guid, out var job))
        {
            return NotFound();
        }

        return Ok(new { job.Id, job.JobStatus, job.Result, job.Error });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
