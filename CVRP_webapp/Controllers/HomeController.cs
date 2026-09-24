using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CVRP_webapp.Models;
using CVRPlib;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using CVRPlib.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CVRP_webapp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Get([FromForm] IFormFile file, [FromForm] int multistartCount)
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

            InputData data = InputDataReader.ParseData(lines);

            var task = new CVRPTask(data, new HillClimbingSolver(), new TargetFunctionStrict());
            var res = await Task.Run(() => task.GetSolution());
            return Ok(res);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
