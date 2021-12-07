using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Dx29.Models;
using Dx29.Services;

namespace Dx29.MedicalHistory.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class MedicalCaseShareController : ControllerBase
    {
        public MedicalCaseShareController(MedicalHistoryService medicalHistoryService)
        {
            MedicalHistoryService = medicalHistoryService;
        }

        public MedicalHistoryService MedicalHistoryService { get; }

        /// <summary>
        /// Get SharedBy
        /// </summary>
        [HttpGet("{userId}/{caseId}")]
        public async Task<IActionResult> GetSharedByAsync(string userId, string caseId)
        {
            try
            {
                var item = await MedicalHistoryService.GetSharedByAsync(userId, caseId);
                if (item != null)
                {
                    return Ok(item);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Share Medical Case
        /// </summary>
        [HttpPost("{userId}/{caseId}")]
        public async Task<IActionResult> ShareMedicalCaseAsync(string userId, string caseId, ShareModel model)
        {
            try
            {
                switch (model.Action?.ToLower())
                {
                    case "accept":
                        return Ok(await MedicalHistoryService.AcceptSharingMedicalCaseAsync(userId, caseId, model.Email));
                    case "revoke":
                        return Ok(await MedicalHistoryService.RevokeSharingMedicalCaseAsync(userId, caseId, model.Email));
                    case "delete":
                        return Ok(await MedicalHistoryService.DeleteSharingMedicalCaseAsync(userId, caseId, model.Email));
                    case "create":
                    default:
                        return Ok(await MedicalHistoryService.ShareMedicalCaseAsync(userId, caseId, model.Email));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Stop sharing Medical Case
        /// </summary>
        [HttpPatch("{userId}/{caseId}")]
        public async Task<IActionResult> StopSharingMedicalCaseAsync(string userId, string caseId, ShareModel model)
        {
            try
            {
                await MedicalHistoryService.StopSharingMedicalCaseAsync(userId, caseId, model.Email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
