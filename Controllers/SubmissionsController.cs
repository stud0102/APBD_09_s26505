using Microsoft.AspNetCore.Mvc;
using APBD_09_s26505.DTOs;
using APBD_09_s26505.Services;

namespace APBD_09_s26505.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _submissionService;

    public SubmissionsController(SubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    public async Task<ActionResult<SubmissionDto>> CreateSubmission(CreateSubmissionDto dto)
    {
        var result = await _submissionService.CreateSubmissionAsync(dto);

        if (!result.Success)
        {
            return result.StatusCode switch
            {
                400 => BadRequest(result.Error),
                404 => NotFound(result.Error),
                409 => Conflict(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return Created(
            $"/api/submissions/{result.Data!.SubmissionId}",
            result.Data
        );
    }

    [HttpPut("{idSubmission}/grade")]
    public async Task<ActionResult<SubmissionDto>> GradeSubmission(
        int idSubmission,
        GradeSubmissionDto dto)
    {
        var result = await _submissionService.GradeSubmissionAsync(idSubmission, dto);

        if (!result.Success)
        {
            return result.StatusCode switch
            {
                400 => BadRequest(result.Error),
                404 => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return Ok(result.Data);
    }

    [HttpDelete("{idSubmission}")]
    public async Task<IActionResult> DeleteSubmission(int idSubmission)
    {
        var result = await _submissionService.DeleteSubmissionAsync(idSubmission);

        if (!result.Success)
        {
            return result.StatusCode switch
            {
                400 => BadRequest(result.Error),
                404 => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return NoContent();
    }
}