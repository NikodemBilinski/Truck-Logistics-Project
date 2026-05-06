using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : Controller
    {
        private readonly DataContext _dataContext;

        public JobsController(DataContext datacontext)
        {
            _dataContext = datacontext;
        }

        //HTTP GETS

        [HttpGet("Get_All_Jobs")]

        public async Task<ActionResult<List<Job>>> GetAllJobs()
        {

            Console.WriteLine("GetAllJobs: Requested.");
            var alljobs = await _dataContext.Jobs.ToListAsync();

            if (alljobs != null)
            {
                Console.WriteLine("GetAllJobs: Returning All Jobs.");
                return Ok(alljobs);
            }
            return BadRequest("Error: No Jobs Found");
        }

        //HTTP POSTS
        public async Task<ActionResult<Job>> AddJob(Job JobToAdd)
        {
            Console.WriteLine("AddJob: Requested To Add Job: " + JobToAdd.Name + ".");
            if (JobToAdd != null)
            {
                _dataContext.Jobs.Add(JobToAdd);
                await _dataContext.SaveChangesAsync();
                Console.WriteLine("AddJob: Added Job: " + JobToAdd.ID + ". " + JobToAdd.Name + ", To Database.");
                return Ok("Successfully added new job: " + JobToAdd.Name);
            }
            else
            {
                Console.WriteLine("AddJob: Error, Job Cannot Be Null.");
                return BadRequest("Error: Job cannot be null.");
            }
        }

        //HTTP PUTS

        [HttpPut("Update_Job/{ID}")]
        public async Task<ActionResult<Job>> UpdateJob(int id, Job updatedJob)
        {
            var job = await _dataContext.Jobs.FindAsync(id);

            if (job == null)
            {
                Console.WriteLine("UpdateJob: Error, Job with the specified ID not found.");
                return BadRequest("Job with the specified ID not found.");
            }
            job.Name = updatedJob.Name;
            job.CompanyName = updatedJob.CompanyName;
            job.ClientContactNumber = updatedJob.ClientContactNumber;
            job.Created = updatedJob.Created;
            job.DeadLine = updatedJob.DeadLine;
            job.LocationFrom = updatedJob.LocationFrom;
            job.LocationTo = updatedJob.LocationTo;
            job.AssignedUserId = updatedJob.AssignedUserId;
            job.Description = updatedJob.Description;

            if (job.AssignedUserId != null)
            {
                job.Status = "assigned";
            }
            else
            {
                job.Status = "open";
            }
            job.RequiredLanguages = updatedJob.RequiredLanguages;
            job.RequiredMinimumCapacity = updatedJob.RequiredMinimumCapacity;
            job.RequiredTruckBrand = updatedJob.RequiredTruckBrand;

            await _dataContext.SaveChangesAsync();
            Console.WriteLine("UpdateJob: Job Updated.");
            return Ok("Job Updated Successfullyyyyyyyyyyyyy.");
        }

        //HTTP DELETES

        [HttpDelete("Delete_Job/{ID}")]
        public async Task<ActionResult<Job>> DeleteJob(int ID)
        {
            Console.WriteLine("DeleteJob: Request To Delete Job With ID: " + ID);

            var JobToDelete = await _dataContext.Jobs.FindAsync(ID);

            if (JobToDelete != null)
            {
                _dataContext.Jobs.Remove(JobToDelete);
                await _dataContext.SaveChangesAsync();
                Console.WriteLine("DeleteJob: Job Successfully deleted.");
                return Ok("Job Deleted Successfully");
            }
            Console.WriteLine("DeleteJob: Error, Job not found.");
            return NotFound("Error: Job not Found.");
        }
    }
}
