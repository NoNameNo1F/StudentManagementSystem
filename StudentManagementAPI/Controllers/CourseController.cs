using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.Logging;
using StudentManagementAPI.Models;
using StudentManagementAPI.Models.Dtos.CourseDto;
using StudentManagementAPI.Repositories.IRepositories;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class CourseController : ControllerBase
    {
        private ICourseRepository _courseRepo;
        private readonly IMapper _mapper;
        private ILogging _logger;

        public CourseController(
            ICourseRepository courseRepo,
            ILogging logger,
            IMapper mapper)
        {
            _courseRepo = courseRepo;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Gets All Course
        /// </summary>
        /// <returns>List of Course</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<CourseDto>))]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courseList = await _courseRepo.GetAllAsync();
            var courseDto = _mapper.Map<List<CourseDto>>(courseList);

            _logger.Log("Get all courses","info");
            return Ok(courseDto);
        }
        /// <summary>
        /// Get Specific Course By Id
        /// </summary>
        /// <param name="courseId">id of course</param>
        /// <returns></returns>
        [HttpGet("{courseId:int}", Name = "GetCourse")]
        [ProducesResponseType(200, Type = typeof(CourseDto))]
        [ProducesResponseType(404)]
        [Authorize]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CourseDto>> GetCourse(int courseId)
        {
            var courseObj = await _courseRepo.GetAsync(u => u.Id == courseId);

            if(courseObj == null)
            {
                _logger.Log($"Course id {courseId} not exists","error");
                return NotFound();
            }

            var coursetDto = _mapper.Map<CourseDto>(courseObj);
            _logger.Log($"Get course id {courseId}", "info");
            return Ok(coursetDto);
        }
        /// <summary>
        /// Create a new course
        /// </summary>
        /// <param name="courseCreateDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CourseDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CourseCreateDto courseCreateDto)
        {
            if(courseCreateDto == null)
            {
                _logger.Log($"Not having Course to creating", "error");
                return BadRequest(ModelState);
            }

            if(await _courseRepo.GetAsync(u => u.Name.ToLower() == courseCreateDto.Name.ToLower()) != null)
            {
                _logger.Log($"Course Name {courseCreateDto.Name} already exists", "error");
                ModelState.AddModelError("", "CourseName is Exists");
                return StatusCode(404, ModelState);
            }

            if(!ModelState.IsValid)
            {
                _logger.Log($"Course needed to be created not valid!", "error");
                return BadRequest(ModelState);
            }

            var courseObj = _mapper.Map<Course>(courseCreateDto);
            // if(!_courseRepo.CreateCourse(courseObj))
            // {
            //     _logger.Log($"Course can't be created!", "error");
            //     ModelState.AddModelError("", $"Something went wrong when saving record {courseObj.Name}");
            //     return StatusCode(500, ModelState);
            // }
            await _courseRepo.CreateAsync(courseObj);
            _logger.Log($"Created Course is {courseObj.Id}", "info");
            return CreatedAtRoute("GetCourse", new
            {
                courseId = courseObj.Id
            }, courseObj);
        }
        /// <summary>
        /// Edit Course by id
        /// </summary>
        /// <param name="courseId">id of course</param>
        /// <param name="courseDto"></param>
        /// <returns></returns>
        [HttpPatch("{courseId:int}", Name = "UpdateCourse")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCourse(int courseId, [FromBody] CourseDto courseDto)
        {
            if(courseDto == null|| courseId != courseDto.Id)
            {
                _logger.Log($"Not having Course to updating", "error");
                return BadRequest(ModelState);
            }

            var courseObj = _mapper.Map<Course>(courseDto);

            await _courseRepo.UpdateAsync(courseObj);

            _logger.Log($"Updated Course Id: {courseObj.Id} successfully!", "info");
            return NoContent();
        }
        /// <summary>
        /// Delete a course by id
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpDelete("{courseId:int}", Name = "DeleteCourse")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            if(courseId == 0)
            {
                _logger.Log($"Not having courseId", "error");
                return BadRequest();
            }

            var courseObj = await _courseRepo.GetAsync(u => u.Id == courseId);
            if(courseObj == null)
            {
                _logger.Log($"Course id {courseId} not exists", "error");
                return NotFound();
            }

            await _courseRepo.RemoveAsync(courseObj);

            _logger.Log($"Deleted Course Id: {courseId} successfully!", "info");
            return NoContent();
        }
    }
}
