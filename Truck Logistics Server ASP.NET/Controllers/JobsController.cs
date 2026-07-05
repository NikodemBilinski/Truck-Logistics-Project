using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Validation;
using System.Runtime.CompilerServices;
using TrucksLogisticsClient.Models.Helping_Models;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;
using TrucksLogisticsServerAPI.Models.Helping_Models;

namespace TrucksLogisticsServerAPI.Controllers
{
    [Authorize(Roles = "admin,user")]
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

        [HttpGet("Get_Jobs_Page/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<JobResponse>> GetJobsPage(int pageNumber, int pageSize, DateTime? creationdate = null, string? status = null, string? searchname = null)
        {
            var query = _dataContext.Jobs
                .Include(x => x.Client)
                .AsQueryable();

            if(!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status == status);
            }
            if(!string.IsNullOrEmpty(searchname))
            {
                query = query.Where(x => x.Name.Contains(searchname));
            }
            if(creationdate != null)
            {
                query = query.Where(x => x.Created.Date == creationdate.Value.Date);
            }

            var totalcount = await query.CountAsync();

            List<Job> jobs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new JobResponse() 
            { 
                Jobs = jobs,
                Jobs_Count = totalcount
            });
        }

        [HttpGet("Get_Jobs_Stats")]
        public async Task<ActionResult<JobResponse>> GetJobsStats()
        {
            var Alljobs = await _dataContext.Jobs.CountAsync();

            int OpenJobsCount = await _dataContext.Jobs.Where(x => x.Status == "open").CountAsync();

            int NearDeadlineCount = await _dataContext.Jobs.Where(x => x.DeadLine < DateTime.Now.AddDays(3)).CountAsync();

            int AssignedJobsCount = await _dataContext.Jobs.Where(x => x.Status == "assigned").CountAsync();

            int DeliveredJobsCount = await _dataContext.Jobs.Where(x => x.Status == "delivered").CountAsync();

            var jobstats = new JobResponse()
            {
                Jobs_Count = Alljobs,
                Open_Count = OpenJobsCount,
                NearDeadline_Count = NearDeadlineCount,
                Assigned_Count = AssignedJobsCount,
                Delivered_Count = DeliveredJobsCount
            };

            return Ok(jobstats);
        }

        [Authorize(Roles = "admin,user")]
        [HttpGet("Get_Current_User_Stats/{UserID}")]
        public async Task<ActionResult<CurrentUserStats>> GetCurrentUserStats(int UserID)
        {
            int Avaiablejobs = await _dataContext.Jobs.Where(x => x.Status == "open").CountAsync();
            int FinishedJobs = await _dataContext.Jobs.Where(x => x.Status == "delivered" && x.AssignedUserId == UserID).CountAsync();

            return Ok(new CurrentUserStats()
            {
                AvaiableJobs = Avaiablejobs,
                FinishedJobs = FinishedJobs
            });
        }

        [Authorize(Roles ="admin,user")]
        [HttpGet("Get_User_Assigned_Jobs_Page/{UserID}")]
        public async Task<ActionResult<List<Job>>> GetUserAssignedJobs(int UserID)
        {
            var userjobs = await _dataContext.Jobs.Where(x => x.AssignedUserId == UserID && x.Status == "assigned").ToListAsync();
            if (userjobs != null)
            {
                return Ok(userjobs);
            }

            return BadRequest("no jobs found for user");
        }

        [Authorize(Roles ="admin,user")]
        [HttpGet("Get_Jobs_Stats_User/{UserID}")]
        public async Task<ActionResult<JobResponse>> GetUserJobStats(int UserID)
        {
            var openjobscount = await _dataContext.Jobs.Where(x=> x.Status == "open").CountAsync();
            var assignedjobscount = await _dataContext.Jobs.Where(x => x.Status == "assigned" && x.AssignedUserId == UserID).CountAsync();


            return Ok(new JobResponse()
            {
                Open_Count = openjobscount,
                Assigned_Count = assignedjobscount
            });
        }

        [Authorize(Roles = "admin,user")]
        [HttpGet("Get_Open_Jobs_Page/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Job>>> GetOpenJobsPage(int pageNumber, int pageSize, int UserID)
        {
            var openjobs = await _dataContext.Jobs.Where(x => x.Status == "open")
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(openjobs);
            
        }

        [Authorize(Roles = "admin,user")]
        [HttpGet("Get_Assigned_Jobs_Page/{pageNumber}/{pageSize}/{UserID}")]
        public async Task<ActionResult<List<Job>>> GetAssignedJobsPage(int pageNumber, int pageSize, int UserID)
        {
            var assignedjobs = await _dataContext.Jobs
                .Where(x => x.Status == "assigned" && x.AssignedUserId == UserID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(assignedjobs);
        }

        [HttpGet("Get_Jobs_By_Client_ID/{id}")]

        public async Task<ActionResult<List<Job>>> GetJobsByClientID(int id)
        {
            var jobs = await _dataContext.Jobs.ToListAsync();

            var ClientJobs = jobs.FindAll(x => x.ClientID == id);

            Console.WriteLine(ClientJobs);

            return Ok(ClientJobs);
        }

        //HTTP POSTS
        [HttpPost("Add_Job")]
        public async Task<ActionResult<Job>> AddJob(Job JobToAdd)
        {
            Console.WriteLine("AddJob: Requested To Add Job: " + JobToAdd.Name + ".");
            if (JobToAdd != null)
            {
                #region validation
                if(string.IsNullOrEmpty(JobToAdd.Name))
                {
                    return BadRequest("Job Name is empty!");
                }
                if(string.IsNullOrEmpty(JobToAdd.CompanyName))
                {
                    return BadRequest("Job Company Name is empty!");
                }
                if(string.IsNullOrEmpty(JobToAdd.ClientContactNumber))
                {
                    return BadRequest("Job Client Contact Number is empty!");
                }

                if(JobToAdd.DeadLine <= DateTime.Now)
                {
                    return BadRequest("Deadline date is invalid.");
                }

                if(string.IsNullOrEmpty(JobToAdd.LocationFrom))
                {
                    return BadRequest("Job Location From is empty!");
                }
                if(string.IsNullOrEmpty(JobToAdd.LocationTo))
                {
                    return BadRequest("Job Location To is empty!");
                }
                if(JobToAdd.RequiredMinimumCapacity <= 0)
                {
                    return BadRequest("Job Required Minimum Capacity is below 0!");
                }
                if(string.IsNullOrEmpty(JobToAdd.RequiredTruckBrand))
                {
                    JobToAdd.RequiredTruckBrand = "all";
                }
                if(string.IsNullOrEmpty(JobToAdd.Description))
                {
                   return BadRequest("Job Description is empty!");
                }
                #endregion
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
        [Authorize(Roles = "admin,user")]
        [HttpPut("Update_Job/{ID}")]
        public async Task<ActionResult<Job>> UpdateJob(int id, Job updatedJob)
        {
            var job = await _dataContext.Jobs.FindAsync(id);

            #region validation
            if (job == null)
            {
                Console.WriteLine("UpdateJob: Error, Job with the specified ID not found.");
                return BadRequest("Job with the specified ID not found.");
            }
            if(updatedJob == null)
            {
                Console.WriteLine("UpdateJob: Error, Updated Job cannot be null.");
                return BadRequest("Updated Job cannot be null.");
            }

            if(string.IsNullOrEmpty(updatedJob.Name))
            {
                return BadRequest("Job Name is empty!");
            }
            if(string.IsNullOrEmpty(updatedJob.CompanyName))
            {
                return BadRequest("Job Company Name is empty!");
            }
            if(string.IsNullOrEmpty(updatedJob.ClientContactNumber))
            {
                return BadRequest("Job Client Contact Number is empty!");
            }
            if(updatedJob.DeadLine <= DateTime.Now)
            {
                return BadRequest("Deadline date is invalid.");
            }
            if(string.IsNullOrEmpty(updatedJob.LocationFrom))
            {
                return BadRequest("Job Location From is empty!");
            }
            if(string.IsNullOrEmpty(updatedJob.LocationTo))
            {
                return BadRequest("Job Location To is empty!");
            }
            if(updatedJob.RequiredMinimumCapacity <= 0)
            {
                return BadRequest("Job Required Minimum Capacity is below 0!");
            }
            if(string.IsNullOrEmpty(updatedJob.RequiredTruckBrand))
            {
                updatedJob.RequiredTruckBrand = "all";
            }
            if(string.IsNullOrEmpty(updatedJob.Description))
            {
                return BadRequest("Job Description is empty!");
            }

            #endregion
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

        [Authorize(Roles = "admin")]
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
