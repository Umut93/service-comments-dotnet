using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System;
using System.Threading.Tasks;
using Unik.Comments.Domain.Exceptions;
using Unik.Comments.Domain.Models;
using Unik.Comments.Domain.Repositories;
using Unik.Comments.Domain.Services;
using Xunit;

namespace Unik.Comments.UnitTest;

public class CommentsServiceTests
{
    private readonly CommentsService _sut;
    private readonly ICommentsRepository _commentsRepository = Substitute.For<ICommentsRepository>();

    public CommentsServiceTests()
    {
        _sut = new CommentsService(_commentsRepository);
    }

    [Fact]
    public async Task CreateCommentAsync_ShouldReturnComment_WhenCommentIsCreated()
    {
        // Arrange
        var comment = new Comment
        {
            PublicId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            Content = new CommentContent
            {
                Data = string.Empty
            }
        };

        _commentsRepository.CreateCommentAsync(comment).Returns(comment);

        // Act
        var result = await _sut.CreateCommentAsync(comment);

        // Assert
        result.Should().BeEquivalentTo(comment);
    }

    [Fact]
    public async Task CreateCommentAsync_ShouldThrowException_WhenExceptionIsThrown()
    {
        // Arrange
        var comment = new Comment
        {
            PublicId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            Content = new CommentContent
            {
                Data = string.Empty
            }
        };
        _commentsRepository.CreateCommentAsync(comment).Throws(new DataAccessException());

        // Act
        var requestAction = async () => await _sut.CreateCommentAsync(comment);

        // Assert
        await requestAction.Should().ThrowAsync<DataAccessException>();
    }

    [Fact]
    public async Task CreateCommentAsync_ShouldReturnCommentWithParentAndThreadId_WhenCommentWithParentIsCreated()
    {
        // Arrange
        var parent = new Comment
        {
            InternalId = 1,
            PublicId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            Created = DateTime.Now,
            ThreadId = 1,
            Content = new CommentContent
            {
                Data = string.Empty
            }
        };

        var comment = new Comment
        {
            PublicId = Guid.NewGuid(),
            ParentPublicId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            Created = DateTime.Now,
            Content = new CommentContent
            {
                Data = string.Empty
            }
        };

        var expectedComment = new Comment
        {
            PublicId = comment.PublicId,
            CreatedBy = comment.CreatedBy,
            Created = comment.Created,
            Content = comment.Content,
            ThreadId = parent.ThreadId,
            ParentId = parent.ParentId,
        };

        _commentsRepository.GetCommentByIdAsync(comment.ParentPublicId.Value).Returns(parent);
        _commentsRepository.CreateCommentAsync(comment).Returns(expectedComment);

        // Act
        var result = await _sut.CreateCommentAsync(comment);
        // Assert
        result.Should().BeEquivalentTo(expectedComment);
    }

    [Fact]
    public async Task CreateCommentAsync_ShouldThrowException_WhenParentDoesNotExist()
    {
        // Arrange
        var comment = new Comment
        {
            PublicId = Guid.NewGuid(),
            ParentPublicId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            Created = DateTime.Now,
            Content = new CommentContent
            {
                Data = string.Empty
            }
        };

        _commentsRepository.GetCommentByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        // Act
        var requestAction = async () => await _sut.CreateCommentAsync(comment);

        // Assert
        await requestAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetCommentByIdAsync_ShouldReturnComment_WhenCommentExists()
    {
        // Arrange
        var existingComment = new Comment
        {
            InternalId = 1,
            PublicId = Guid.NewGuid(),
            Created = DateTime.Now,
            CreatedBy = Guid.NewGuid(),
            Content = new CommentContent
            {
                Id = 1,
                Data = string.Empty
            }
        };

        _commentsRepository.GetCommentByIdAsync(existingComment.PublicId).Returns(existingComment);

        // Act
        var result = await _sut.GetCommentByIdAsync(existingComment.PublicId);

        // Assert
        result.Should().BeEquivalentTo(existingComment);
    }

    [Fact]
    public async Task GetCommentByIdAsync_ShouldReturnNull_WhenNoCommentExists()
    {
        // Arrange
        _commentsRepository.GetCommentByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        // Act
        var result = await _sut.GetCommentByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCommentByIdAsync_ShouldAddParentPublicId_WhenParentExists()
    {
        // Arrange
        var parentPublicId = Guid.NewGuid();
        var comment = new Comment
        {
            ParentId = 1,
            ParentPublicId = null,
            PublicId = Guid.NewGuid(),
            ThreadId = 1,
            Created = DateTime.Now,
            CreatedBy = Guid.NewGuid(),
            Content = new CommentContent
            {
                Id = 1,
                Data = string.Empty
            }
        };

        var expectedComment = new Comment
        {
            ParentId = comment.ParentId,
            ParentPublicId = parentPublicId,
            PublicId = comment.PublicId,
            ThreadId = comment.ThreadId,
            Created = comment.Created,
            CreatedBy = comment.CreatedBy,
            Content = comment.Content
        };

        _commentsRepository.GetCommentByIdAsync(comment.PublicId).Returns(comment);
        _commentsRepository.GetCommentPublicIdAsync(comment.ParentId.Value).Returns(parentPublicId);

        // Act
        var result = await _sut.GetCommentByIdAsync(comment.PublicId);
        // Assert
        result.Should().BeEquivalentTo(expectedComment);
    }

    [Fact]
    public async Task DeleteCommentAsync_ShouldDeleteComment_WhenCommentExists()
    {
        // Arrange
        var publicId = Guid.NewGuid();
        var comment = new Comment { PublicId = publicId };

        _commentsRepository.DeleteCommentAsync(comment).Returns(true);
        _commentsRepository.GetCommentByIdAsync(publicId).Returns(comment);

        // Act
        var result = await _sut.DeleteCommentAsync(publicId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteCommentAsync_ShouldNotDeleteComment_WhenCommentDoesNotExists()
    {
        // Arrange
        var publicId = Guid.NewGuid();
        var comment = new Comment { PublicId = publicId };
        _commentsRepository.DeleteCommentAsync(comment).Returns(false);

        // Act
        var result = await _sut.DeleteCommentAsync(publicId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateCommentAsync_ShouldReturnTrue_WhenCommentIsUpdated()
    {
        // Arrange
        var publicId = Guid.NewGuid();
        var data = string.Empty;
        var comment = new Comment
        {
            PublicId = publicId,
            CreatedBy = Guid.NewGuid(),
            Content = new CommentContent
            {
                Data = data
            }
        };

        _commentsRepository.UpdateCommentAsync(comment).Returns(true);
        _commentsRepository.GetCommentByIdAsync(publicId).Returns(comment);

        // Act
        var result = await _sut.UpdateCommentAsync(publicId, data);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatecommentAsync_ShouldThrowException_WhenCommentIsNotUpdated()
    {
        // Arrange
        var publicId = Guid.NewGuid();
        var data = string.Empty;
        var comment = new Comment
        {
            PublicId = publicId,
            CreatedBy = Guid.NewGuid(),
            Content = new CommentContent
            {
                Data = data
            }
        };

        _commentsRepository.UpdateCommentAsync(comment).Throws(new DataAccessException());
        _commentsRepository.GetCommentByIdAsync(publicId).Returns(comment);

        // Act
        var requestAction = async () => await _sut.UpdateCommentAsync(publicId, data);

        //Assert
        await requestAction.Should().ThrowAsync<DataAccessException>();
    }
}