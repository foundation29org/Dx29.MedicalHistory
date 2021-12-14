using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Dx29.Data;
using Dx29.Services;

namespace Dx29.MedicalHistory.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class MedicalCasesController : ControllerBase
    {
        public MedicalCasesController(MedicalHistoryService medicalHistoryService)
        {
            MedicalHistoryService = medicalHistoryService;
        }

        public MedicalHistoryService MedicalHistoryService { get; }

        /// <summary>
        /// Get all Medical Cases by userId
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMedicalCasesAsync(string userId, bool includeDeleted = false)
        {
            var items = await MedicalHistoryService.GetMedicalCasesAsync(userId, includeDeleted);
            return Ok(items);
        }

        /// <summary>
        /// Get Medical Case by userId, caseId
        /// </summary>
        [HttpGet("{userId}/{caseId}")]
        public async Task<IActionResult> GetMedicalCaseAsync(string userId, string caseId)
        {
            try
            {
                var item = await MedicalHistoryService.GetMedicalCaseAsync(userId, caseId);
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
        /// Create new Medical Case for userId
        /// </summary>
        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateMedicalCaseAsync(string userId, [FromBody] PatientInfo patientInfo)
        {
            try
            {
                var item = await MedicalHistoryService.CreateMedicalCaseAsync(userId, patientInfo);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Share Medical Case
        /// </summary>
        [HttpPost("share/{userId}/{caseId}")]
        public async Task<IActionResult> ShareMedicalCaseAsync(string userId, string caseId, string email)
        {
            try
            {
                var item = await MedicalHistoryService.ShareMedicalCaseAsync(userId, caseId, email);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update Medical Case by userId, caseId with PatientInfo
        /// </summary>
        [HttpPatch("{userId}/{caseId}")]
        public async Task<IActionResult> UpdateMedicalCaseAsync(string userId, string caseId, [FromBody] PatientInfo patientInfo)
        {
            try
            {
                var item = await MedicalHistoryService.UpdateMedicalCaseAsync(userId, caseId, patientInfo);
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
        /// Delete Medical Case by userId, caseId
        /// </summary>
        [HttpDelete("{userId}/{caseId}")]
        public async Task<IActionResult> DeleteMedicalCaseAsync(string userId, string caseId, bool force = false)
        {
            try
            {
                var item = await MedicalHistoryService.DeleteMedicalCaseAsync(userId, caseId, force);
                if (item != null)
                {
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete ALL Medical Cases by userId
        /// </summary>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserCasesAsync(string userId)
        {
            try
            {
                await MedicalHistoryService.DeleteUserCasesAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
