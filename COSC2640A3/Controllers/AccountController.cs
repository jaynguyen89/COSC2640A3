using System.Threading.Tasks;
using COSC2640A3.Attributes;
using COSC2640A3.Models;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Helper.Shared.SharedEnums;

namespace COSC2640A3.Controllers {

    [ApiController]
    [MainAuthorize]
    [Route("account")]
    public sealed class AccountController {
        
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(
            ILogger<AuthenticationController> logger,
            IAccountService accountService
        ) {
            _logger = logger;
            _accountService = accountService;
        }

        [RoleAuthorize(Role.Student)]
        [HttpPost("update-student")]
        public async Task<JsonResult> UpdateStudentDetails(Student student) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(UpdateStudentDetails) }: Service starts.");

            var updateResult = await _accountService.UpdateStudent(student);
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
        
        [RoleAuthorize(Role.Teacher)]
        [HttpPost("update-teacher")]
        public async Task<JsonResult> UpdateTeacherDetails(Teacher teacher) {
            _logger.LogInformation($"{ nameof(AccountController) }.{ nameof(UpdateTeacherDetails) }: Service starts.");
            
            var updateResult = await _accountService.UpdateTeacher(teacher);
            return !updateResult.HasValue || !updateResult.Value
                ? new JsonResult(new JsonResponse { Result = RequestResult.Failed, Messages = new [] { "An issue happened while processing your request." } })
                : new JsonResult(new JsonResponse { Result = RequestResult.Success });
        }
    }
}