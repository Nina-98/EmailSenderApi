using EmailSenderApi.Models.Input;
using EmailSenderApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmailSenderApi.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly IEmaliService _emailService;

        public EmailController(IEmaliService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmailToUser([FromBody] EmailRequest emailRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(emailRequest.To) || string.IsNullOrEmpty(emailRequest.Subject))
                {
                    return BadRequest("To and Subject fields are required.");
                }

                await _emailService.SendEmail(emailRequest);
                return Ok("Email sent successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
