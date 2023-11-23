using Microsoft.AspNetCore.Http;

namespace AadhaarVerification.Models
{
    public class DataViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string AadharNumber { get; set; }
        public string Email { get; set; }
        public IFormFile FilePath { get; set; }

       
    }
}
