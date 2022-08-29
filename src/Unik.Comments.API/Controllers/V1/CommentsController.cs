using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Unik.Comments.API.Models.V1;
using Unik.Comments.Domain.Models;
using Unik.Comments.Domain.Services;
using Unik.Swagger;

namespace Unik.Comments.API.Controllers.V1;

/// <summary>
/// Comments controller
/// </summary>
[ApiVersion(ApiVersions.V1)]
public class CommentsController : ApiControllerBase
{
    private readonly ICommentsService _commentsService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor for comments controller
    /// </summary>
    /// <param name="commentsService"></param>
    /// <param name="mapper"></param>
    public CommentsController(ICommentsService commentsService, IMapper mapper)
    {
        _commentsService = commentsService;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a Comment
    /// </summary>
    /// <param name="commentContract">The comment create model</param>
    /// <returns>The created Comment of type <see cref="CommentContract"/></returns>
    [HttpPost]
    [ProducesResponseType(typeof(CommentContract), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CommentContract>> CreateCommentAsync(CommentCreateContract commentContract)
    {
        if (commentContract is null ||
            commentContract.ParentId == Guid.Empty ||
            string.IsNullOrWhiteSpace(commentContract.Content))
        {
            return BadRequest();
        }

        try
        {
            var comment = _mapper.Map<Comment>(commentContract);
            comment.CreatedBy = Guid.Parse(User.FindFirstValue("userId"));
            var created = await _commentsService.CreateCommentAsync(comment);
            return Ok(_mapper.Map<CommentContract>(created));
        }
        catch (Exception)
        {
            return BadRequest("Could not create Comment");
        }
    }

    /// <summary>
    /// Gets a Comment/>
    /// </summary>
    /// <param name="id">The public id of the comment to get</param>
    /// <returns>The requested <see cref="CommentContract"/></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CommentContract), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentContract>> GetCommentByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Not a valid Guid");
        }

        var comment = await _commentsService.GetCommentByIdAsync(id);

        if (comment is null)
        {
            return NotFound("Could not find Comment");
        }

        var commentContract = _mapper.Map<CommentContract>(comment);

        return Ok(commentContract);
    }

    /// <summary>
    /// Updates a Comment
    /// </summary>
    /// <param name="comment">The comment update model</param>
    /// <returns>A <see cref="OkResult"/> indicating that the Comment was updated, <br />
    /// or a <see cref="BadRequestResult"/> indicating that the Comment was not updated.
    /// </returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CommentContract>> UpdateCommentAsync(CommentUpdateContract comment)
    {
        if (comment is null ||
            string.IsNullOrWhiteSpace(comment.Data) ||
            comment.PublicId == Guid.Empty)
        {
            return BadRequest();
        }

        var result = await _commentsService.UpdateCommentAsync(comment.PublicId, comment.Data);

        return result ? Ok() : BadRequest();
    }

    /// <summary>
    /// Deletes a Comment
    /// </summary>
    /// <param name="id">The id of the comment to delete</param>
    /// <returns>A <see cref="OkResult"/> indicating that the Comment was deleted, <br />
    /// or a <see cref="NotFoundResult"/> indicating that the Comment was not found.
    /// </returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> DeleteCommentAsync(Guid id)
    {
        try
        {
            var result = await _commentsService.DeleteCommentAsync(id);
            return result ? Ok() : NotFound();
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong while trying to delete comment: " + id);
        }
    }
}