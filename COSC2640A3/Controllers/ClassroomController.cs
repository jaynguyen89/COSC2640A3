using System.Threading.Tasks;
using COSC2640A3.Attributes;
using COSC2640A3.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [Route("classroom")]
    public sealed class ClassroomController {
        
        private readonly ILogger<ClassroomController> _logger;
        private readonly IClassroomService _classroomService;

        public ClassroomController(
            ILogger<ClassroomController> logger,
            IClassroomService classroomService
        ) {
            _logger = logger;
            _classroomService = classroomService;
        }

        [RoleAuthorize(Role.Teacher)]
        [HttpPost("create")]
        public async Task<JsonResult> CreateClass() {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(CreateClass) }: Service starts.");
            
        }
        
        [RoleAuthorize(Role.Teacher)]
        [HttpPut("update")]
        public async Task<JsonResult> UpdateClass() {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(UpdateClass) }: Service starts.");
            
        }
        
        [RoleAuthorize(Role.Teacher)]
        [HttpDelete("remove")]
        public async Task<JsonResult> RemoveClass() {
            _logger.LogInformation($"{ nameof(ClassroomController) }.{ nameof(RemoveClass) }: Service starts.");
            
        }
    }
}