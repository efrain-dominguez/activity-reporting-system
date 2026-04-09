using ARS.Application.DTOs.Reviews;
using ARS.Application.Services;
using ARS.Domain.Entities;
using ARS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ARS.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : BaseApiController
    {
        private readonly IReviewRepository _reviewRepository;

       
        public ReviewsController(IReviewRepository reviewRepository, IUserRepository userRepository,
         ICurrentUserService currentUserService) : base(currentUserService, userRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReviewById(string id)
        {

            var review = await _reviewRepository.GetByIdAsync(id);

            if (review == null)
                return NotFound($"Review with ID {id} not found");

            return Ok(review);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,PMO")]
        public async Task<ActionResult<Review>> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userId = await GetCurrentUserIdAsync();

            // Verificar si ya existe una review de este usuario para este assignment
            var existingReview = await _reviewRepository.GetByAssignmentIdAndReviewerAsync(
                dto.AssignmentId,
                userId
            );

            if (existingReview != null)
            {
                return Conflict("You have already reviewed this assignment");
            }

            var review = new Review
            {
                AssignmentId = dto.AssignmentId,
                RequestId = dto.RequestId,
                ReviewedByUserId = userId,    
                Decision = dto.Decision,
                Comments = dto.Comments,
                ReviewedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var createdReview = await _reviewRepository.CreateAsync(review);
            return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.Id }, createdReview);
        }


        [HttpGet("assignment/{assignmentId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByAssignment(string assignmentId)
        {
            var reviews = await _reviewRepository.GetByAssignmentIdAsync(assignmentId);
            return Ok(reviews);
        }

        [HttpGet("request/{requestId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByRequest(string requestId)
        {
            var reviews = await _reviewRepository.GetByRequestIdAsync(requestId);
            return Ok(reviews);
        }


        [HttpGet("my-reviews")]
        public async Task<ActionResult<IEnumerable<Review>>> GetMyReviews()
        {
            var userId = await GetCurrentUserIdAsync();
            var reviews = await _reviewRepository.GetByReviewerIdAsync(userId);
            return Ok(reviews);
        }
    }
}