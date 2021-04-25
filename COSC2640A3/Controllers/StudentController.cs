using System.Threading.Tasks;
using COSC2640A3.Attributes;
using COSC2640A3.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [Route("student")]
    public sealed class StudentController {
        
        private readonly ILogger<StudentController> _logger;
        private readonly IEnrolmentService _enrolmentService;

        public StudentController(
            ILogger<StudentController> logger,
            IEnrolmentService enrolmentService
        ) {
            _logger = logger;
            _enrolmentService = enrolmentService;
        }

        public async Task<JsonResult> EnrolIntoClassroom() {
            
        }
        
        public async Task<JsonResult> GetEnrolmentsByStudent() {
            
        }
        
        public async Task<JsonResult> UnenrolToClassroom() {
            
        }
        
        public async Task<JsonResult> GetAllInvoices() {
            
        }
    }
}