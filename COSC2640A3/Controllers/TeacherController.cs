using COSC2640A3.Attributes;
using COSC2640A3.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [Route("teacher")]
    public sealed class TeacherController {
        
        private readonly ILogger<TeacherController> _logger;
        private readonly IStudentMarkService _markService;

        public TeacherController(
            ILogger<TeacherController> logger,
            IStudentMarkService markService
        ) {
            _logger = logger;
            _markService = markService;
        }
    }
}