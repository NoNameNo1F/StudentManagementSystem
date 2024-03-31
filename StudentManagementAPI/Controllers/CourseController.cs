using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private ILogger<CourseController> _logger;

        public CourseController(
            ICourseRepository courseRepo,
            ILogger<CourseController> logger,
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
        public IActionResult GetCourses()
        {
            var courseList = _courseRepo.GetCourses();
            var courseDto = new List<CourseDto>();

            foreach (var course in courseList)
            {
                courseDto.Add(_mapper.Map<CourseDto>(course));
            }
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
        public IActionResult GetCourse(int courseId)
        {
            var courseObj = _courseRepo.GetCourse(courseId);

            if(courseObj == null)
                return NotFound();

            var coursetDto = _mapper.Map<CourseDto>(courseObj);

            return Ok(coursetDto);
        }
        /// <summary>
        /// Create a new course
        /// </summary>
        /// <param name="courseDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CourseDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCourse([FromBody] CourseDto courseDto)
        {
            if(courseDto == null)
            {
                return BadRequest(ModelState);
            }

            if(_courseRepo.ExistsCourseByName(courseDto.Name))
            {
                ModelState.AddModelError("", "CourseName is Exists");
                return StatusCode(404, ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var courseObj = _mapper.Map<Course>(courseDto);
            if(!_courseRepo.CreateCourse(courseObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving record {courseObj.Name}");
                return StatusCode(500, ModelState);
            }

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
        public IActionResult UpdateCourse(int courseId, [FromBody] CourseDto courseDto)
        {
            if(courseDto == null|| courseId != courseDto.Id)
            {
                return BadRequest(ModelState);
            }

            var courseObj = _mapper.Map<Course>(courseDto);

            if(!_courseRepo.UpdateCourse(courseObj))
            {
                ModelState.AddModelError("",$"Something went wrong when saving record {courseObj.Id}");
                return StatusCode(500, ModelState);
            }

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
        public IActionResult DeleteCourse(int courseId)
        {
            if(courseId == null)
            {
                return NotFound();
            }

            var courseObj = _courseRepo.GetCourse(courseId);
            if(!_courseRepo.DeleteCourse(courseObj))
            {
                ModelState.AddModelError("",$"Something went wrong when deleting record {courseObj.Id}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
