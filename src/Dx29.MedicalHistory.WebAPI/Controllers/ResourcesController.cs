using System;
using System.Linq;
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
    public class ResourcesController : ControllerBase
    {
        public ResourcesController(MedicalHistoryService medicalHistoryService)
        {
            MedicalHistoryService = medicalHistoryService;
        }

        public MedicalHistoryService MedicalHistoryService { get; }

        /// <summary>
        /// Get Resources by type?, name?, resourceId?
        /// </summary>
        /// 
        [HttpGet("{userId}/{caseId}")]
        public async Task<IActionResult> GetResourcesAsync(string userId, string caseId, string groupType = null, string groupName = null, string resourceId = null)
        {
            try
            {
                var items = await MedicalHistoryService.GetResourcesAsync(userId, caseId, MedicalHistoryService.TryParseResourceGroupType(groupType), groupName, resourceId);
                if (items != null)
                {
                    return Ok(items);
                }
                return NotFound("MedicalCase not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get Resource by groupId and resourceId
        /// </summary>
        [HttpGet("{userId}/{caseId}/{groupId}/{resourceId}")]
        public async Task<IActionResult> GetResourceByIdAsync(string userId, string caseId, string groupId, string resourceId)
        {
            try
            {
                var item = await MedicalHistoryService.GetResourceByIdAsync(userId, caseId, groupId, resourceId);
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
        /// Get Resource by type, name and resourceId
        /// </summary>
        [HttpGet("{userId}/{caseId}/{groupType}/{groupName}/{resourceId}")]
        public async Task<IActionResult> GetResourceByIdAsync(string userId, string caseId, string groupType, string groupName, string resourceId)
        {
            try
            {
                var item = await MedicalHistoryService.GetResourceByTypeNameIdAsync(userId, caseId, MedicalHistoryService.ParseResourceGroupType(groupType), groupName, resourceId);
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
        /// Upsert Resources grouped by groupId
        /// </summary>
        [HttpPut("{userId}/{caseId}")]
        public async Task<IActionResult> UpsertResourcesAsync(string userId, string caseId, [FromBody] IDictionary<string, IList<Resource>> grouped)
        {
            try
            {
                var item = await MedicalHistoryService.UpsertResourcesAsync(userId, caseId, grouped);
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
        /// Upsert Resources by groupId
        /// </summary>
        [HttpPut("{userId}/{caseId}/{resourceGroupId}")]
        public async Task<IActionResult> UpsertResourcesAsync(string userId, string caseId, string groupId, [FromBody] IList<Resource> resources)
        {
            try
            {
                var item = await MedicalHistoryService.UpsertResourcesAsync(userId, caseId, groupId, resources.ToArray());
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
        /// Upsert Resources by type, name
        /// </summary>
        [HttpPut("{userId}/{caseId}/{groupType}/{groupName}")]
        public async Task<IActionResult> UpsertResourcesAsync(string userId, string caseId, string groupType, string groupName, [FromBody] IList<Resource> resources)
        {
            try
            {
                var item = await MedicalHistoryService.UpsertResourcesAsync(userId, caseId, MedicalHistoryService.ParseResourceGroupType(groupType), groupName, resources.ToArray());
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
        /// Delete Resources by groupId, resourceId
        /// </summary>
        [HttpDelete("{userId}/{caseId}/{groupId}")]
        public async Task<IActionResult> DeleteResourcesAsync(string userId, string caseId, string groupId, [FromQuery] string[] resourceId)
        {
            try
            {
                var resourceGroup = await MedicalHistoryService.DeleteResourcesAsync(userId, caseId, groupId, resourceId);
                return Ok(resourceGroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete Resources by groupId, resourceId
        /// </summary>
        [HttpDelete("{userId}/{caseId}/{groupType}/{groupName}")]
        public async Task<IActionResult> DeleteResourcesAsync(string userId, string caseId, string groupType, string groupName, [FromQuery] string[] resourceId)
        {
            try
            {
                var resourceGroup = await MedicalHistoryService.DeleteResourcesAsync(userId, caseId, MedicalHistoryService.ParseResourceGroupType(groupType), groupName, resourceId);
                return Ok(resourceGroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
