using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AadhaarVerification.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AadhaarVerification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        private readonly UserDataDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UserDataController(UserDataDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Data>>> GetUserData()
        {
            return await _context.Datas
                .Select(x => new Data()
                {
                  Id = x.Id,
                  FirstName = x.FirstName,
                    LastName = x.LastName,
                  Age = x.Age,
                  AadharNumber = x.AadharNumber,
                  Address = x.Address,
                  Phone = x.Phone,
                  Email = x.Email,
                    ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, x.ImageName)
                })
                .ToListAsync();
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Data>> GetDataModel(int id)
        {
            var dataModel = await _context.Datas.FindAsync(id);

            if (dataModel == null)
            {
                return NotFound();
            }

            return dataModel;
        }

        // PUT: api/Employee/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataModel(int id, [FromForm] Data dataModel)
        {
            if (id != dataModel.Id)
            {
                return BadRequest();
            }

            if (dataModel.ImageFile != null)
            {
                DeleteImage(dataModel.ImageName);
                dataModel.ImageName = await SaveImage(dataModel.ImageFile);
            }

            _context.Entry(dataModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Employee
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Data>> PostEmployeeModel([FromForm] Data dataModel)
        {
            dataModel.ImageName = await SaveImage(dataModel.ImageFile);
            _context.Datas.Add(dataModel);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Data>> DeleteDataModel(int id)
        {
            var dataModel = await _context.Datas.FindAsync(id);
            if (dataModel == null)
            {
                return NotFound();
            }
            DeleteImage(dataModel.ImageName);
            _context.Datas.Remove(dataModel);
            await _context.SaveChangesAsync();

            return dataModel;
        }

        private bool EmployeeModelExists(int id)
        {
            return _context.Datas.Any(e => e.Id == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }

        [NonAction]
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}