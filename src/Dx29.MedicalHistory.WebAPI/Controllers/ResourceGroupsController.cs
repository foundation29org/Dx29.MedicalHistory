using System;
using System.Collections.Generic;
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
    public class ResourceGroupsController : ControllerBase
    {
        public ResourceGroupsController(MedicalHistoryService medicalHistoryService)
        {
            MedicalHistoryService = medicalHistoryService;
        }

        public MedicalHistoryService MedicalHistoryService { get; }

        /// <summary>
        /// Get Resource Groups by userId, caseId. Filter by type or name
        /// </summary>
        [HttpGet("{userId}/{caseId}")]
        public async Task<IActionResult> GetResourceGroupsAsync(string userId, string caseId, string type = null, string name = null)
        {
            try
            {
                if (type != null && name != null)
                {
                    var item = await MedicalHistoryService.GetResourceGroupByTypeNameAsync(userId, caseId, MedicalHistoryService.ParseResourceGroupType(type), name);
                    return Ok(item);
                }
                else if (type != null)
                {
                    var items = await MedicalHistoryService.GetResourceGroupsByTypeAsync(userId, caseId, MedicalHistoryService.ParseResourceGroupType(type));
                    return Ok(items);
                }
                else if (name != null)
                {
                    var items = await MedicalHistoryService.GetResourceGroupsByNameAsync(userId, caseId, name);
                    return Ok(items);
                }
                else
                {
                    var items = await MedicalHistoryService.GetResourceGroupsAsync(userId, caseId);
                    if (items != null)
                    {
                        return Ok(items);
                    }
                    return NotFound("MedicalCase not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get Resource Group by Id
        /// </summary>
        [HttpGet("{userId}/{caseId}/{groupId}")]
        public async Task<IActionResult> GetResourceGroupByIdAsync(string userId, string caseId, string groupId)
        {
            try
            {
                var item = await MedicalHistoryService.GetResourceGroupByIdAsync(userId, caseId, groupId);
                if (item != null)
                {
                    return Ok(item);
                }
                return NotFound("MedicalCase or ResourceGroup not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create Resource Group
        /// </summary>
        [HttpPost("{userId}/{caseId}")]
        public async Task<IActionResult> CreateResourceGroupAsync([FromBody] IList<Resource> resources, string userId, string caseId, string type, string name)
        {
            try
            {
                var item = await MedicalHistoryService.CreateResourceGroupAsync(userId, caseId, MedicalHistoryService.ParseResourceGroupType(type), name, resources);
                if (item != null)
                {
                    return Ok(item);
                }
                return NotFound("MedicalCase not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Upsert Resource Group
        /// </summary>
        [HttpPut("{userId}/{caseId}")]
        public async Task<IActionResult> UpsertResourceGroupAsync([FromBody] IList<Resource> resources, string userId, string caseId, string type, string name, bool replace = false)
        {
            try
            {
                var item = await MedicalHistoryService.UpsertResourceGroupAsync(userId, caseId, MedicalHistoryService.ParseResourceGroupType(type), name, resources, replace);
                if (item != null)
                {
                    return Ok(item);
                }
                return NotFound("MedicalCase not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete Reource Gorup by groupId
        /// </summary>
        [HttpDelete("{userId}/{caseId}/{groupId}")]
        public async Task<IActionResult> DeleteResourceGroupAsync(string userId, string caseId, string groupId)
        {
            try
            {
                await MedicalHistoryService.DeleteResourceGroupAsync(userId, caseId, groupId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
