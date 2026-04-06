using ARS.Application.DTOs.Reviews;
using ARS.Domain.Entities;
using ARS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace ARS.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        // TODO: Obtener del JWT cuando se implemente autenticación
        private const string TempUserId = "67460f8a1c2d3e4f5a6b7c8d";  // Usa un userId real

        public ReviewsController(IReviewRepository reviewRepository)
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
        public async Task<ActionResult<Review>> CreateReview([FromBody] CreateReviewDto dto)
        {

            // Verificar si ya existe una review de este usuario para este assignment
            var existingReview = await _reviewRepository.GetByAssignmentIdAndReviewerAsync(
                dto.AssignmentId,
                TempUserId
            );

            if (existingReview != null)
            {
                return Conflict("You have already reviewed this assignment");
            }

            var review = new Review
            {
                AssignmentId = dto.AssignmentId,
                RequestId = dto.RequestId,
                ReviewedByUserId = TempUserId,    
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
            var reviews = await _reviewRepository.GetByReviewerIdAsync(TempUserId);
            return Ok(reviews);
        }
    }
}