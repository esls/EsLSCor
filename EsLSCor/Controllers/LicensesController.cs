using EsLSCor.Entities;
using EsLSCor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace EsLSCor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LicensesController : ControllerBase
    {
        private readonly ILogger<LicensesController> _logger;
        private readonly LicenseDbContext _context;

        public LicensesController(ILogger<LicensesController> logger, LicenseDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult Get([FromQuery] int from = 0, [FromQuery] int count = 10, [FromQuery] string mailFilter = null, [FromQuery] string keyFilter = null)
        {
            var searchPool = _context.Licenses.AsQueryable();

            if (!string.IsNullOrEmpty(mailFilter))
                searchPool = searchPool.Where(x => x.Email.ToLower().Contains(mailFilter.ToLower()));

            if (!string.IsNullOrEmpty(keyFilter))
                searchPool = searchPool.Where(x => x.LicenseKey.ToUpper().Contains(keyFilter.ToLower()));

            return Ok(new LicenseSearchResults()
            {
                Licenses = searchPool.Skip(from).Take(count).ToArray(),
                TotalCount = searchPool.Count()
            });
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var license = _context.Licenses.SingleOrDefault(x => x.Id == id);
            if (license != null)
                return Ok(license);
            else
                return BadRequest("Invalid license ID");
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("update")]
        public IActionResult Update([FromBody] DbLicenseModel newLicense)
        {
            var existingLicense = _context.Licenses.SingleOrDefault(x => x.Id == newLicense.Id);

            if (existingLicense == null)
                return BadRequest("Invalid license ID");

            existingLicense.CreationDate = newLicense.CreationDate;
            existingLicense.Email = newLicense.Email;
            existingLicense.FullName = newLicense.FullName;
            existingLicense.LicenseKey = newLicense.LicenseKey;
            existingLicense.Price = newLicense.Price;

            _context.Licenses.Update(existingLicense);

            _context.SaveChanges();

            return Ok();
        }
    }
}
