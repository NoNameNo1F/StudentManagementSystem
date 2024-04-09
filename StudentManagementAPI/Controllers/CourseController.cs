using System.Net;
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
        protected APIResponse _response;
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
            this._response = new();
        }
        /// <summary>
        /// Gets All Course
        /// </summary>
        /// <returns>List of Course</returns>
        [HttpGet]
        //[ProducesResponseType(200, Type = typeof(List<CourseDto>))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetCourses()
        {
            try
            {
                IEnumerable<Course> courseList = await _courseRepo.GetAllAsync();

                _response.Result = _mapper.Map<List<CourseDto>>(courseList);
                _response.StatusCode = HttpStatusCode.OK;

                _logger.Log("Get all courses","info");
                return Ok(_response);
            }
            catch (Exception exception)
            {
                _logger.Log($"{exception}", "error");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    exception.ToString()
                };
            }

            return _response;

        }
        /// <summary>
        /// Get Specific Course By Id
        /// </summary>
        /// <param name="courseId">id of course</param>
        /// <returns></returns>
        [HttpGet("{courseId:int}", Name = "GetCourse")]
        //[ProducesResponseType(200, Type = typeof(CourseDto))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(404)]
        [Authorize]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<APIResponse>> GetCourse(int courseId)
        {
            try
            {
                Course courseObj = await _courseRepo.GetAsync(u => u.Id == courseId);

                if(courseObj == null)
                {
                    _logger.Log($"Course id {courseId} not exists","error");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<CourseDto>(courseObj);
                _response.StatusCode = HttpStatusCode.OK;

                _logger.Log($"Get course id {courseId}", "info");
                return Ok(_response);
            }
            catch (Exception exception)
            {
                _logger.Log($"{exception}", "error");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    exception.ToString()
                };
            }

            return _response;

        }
        /// <summary>
        /// Create a new course
        /// </summary>
        /// <param name="courseCreateDto"></param>
        /// <returns></returns>
        [HttpPost]
        //[ProducesResponseType(201, Type = typeof(CourseDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateCourse([FromBody] CourseCreateDto courseCreateDto)
        {
            try
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
                    return BadRequest(ModelState);
                }

                if(!ModelState.IsValid)
                {
                    _logger.Log($"Course needed to be created not valid!", "error");
                    ModelState.AddModelError("", "CourseId is not Valid");
                    return BadRequest(ModelState);
                }

                Course courseObj = _mapper.Map<Course>(courseCreateDto);
                await _courseRepo.CreateAsync(courseObj);

                _response.Result = _mapper.Map<CourseDto>(courseObj);
                _response.StatusCode = HttpStatusCode.Created;

                _logger.Log($"Created Course is {courseObj.Id}", "info");
                return CreatedAtRoute("GetCourse", new
                {
                    courseId = courseObj.Id
                }, _response);
            }
            catch (Exception exception)
            {
                _logger.Log($"{exception}","error");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    exception.ToString()
                };
            }

            return _response;
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
        public async Task<ActionResult<APIResponse>> UpdateCourse(int courseId, [FromBody] CourseDto courseDto)
        {
            try
            {
                if(courseDto == null|| courseId != courseDto.Id)
                {
                    _logger.Log($"Not having Course to updating", "error");
                    return BadRequest(ModelState);
                }

                Course courseObj = _mapper.Map<Course>(courseDto);

                await _courseRepo.UpdateAsync(courseObj);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                _logger.Log($"Updated Course Id: {courseObj.Id} successfully!", "info");
                return Ok(_response);
            }
            catch (Exception exception)
            {
                _logger.Log($"{exception}", "error");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    exception.ToString()
                };
            }

            return _response;
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
        public async Task<ActionResult<APIResponse>> DeleteCourse(int courseId)
        {
            try
            {
                var courseObj = await _courseRepo.GetAsync(u => u.Id == courseId);
                if(courseObj == null)
                {
                    _logger.Log($"Course id {courseId} not exists", "error");
                    return NotFound();
                }

                await _courseRepo.RemoveAsync(courseObj);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                _logger.Log($"Deleted Course Id: {courseId} successfully!", "info");
                return Ok(_response);
            }
            catch (Exception exception)
            {
                _logger.Log($"{exception}", "error");
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    exception.ToString()
                };
            }

            return _response;
        }
    }
}
