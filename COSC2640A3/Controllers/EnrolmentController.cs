using COSC2640A3.Attributes;
using COSC2640A3.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [Route("enrolment")]
    public sealed class EnrolmentController {
        
        private readonly ILogger<EnrolmentController> _logger;
        private readonly IEnrolmentService _classroomService;

        public EnrolmentController(
            ILogger<EnrolmentController> logger,
            IEnrolmentService classroomService
        ) {
            _logger = logger;
            _classroomService = classroomService;
        }
    }
}