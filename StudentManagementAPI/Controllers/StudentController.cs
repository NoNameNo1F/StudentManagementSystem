using AutoMapper;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.Logging;
using StudentManagementAPI.Models;
using StudentManagementAPI.Models.Dtos.StudentDto;
using StudentManagementAPI.Repositories.IRepositories;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/v{version:apiVersion}/students")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class StudentController : ControllerBase
    {
        protected APIResponse _response;
        private IStudentRepository _studentRepo;
        private readonly IMapper _mapper;
        private ILogging _logger;

        public StudentController(
            IStudentRepository studentRepo,
            ILogging logger,
            IMapper mapper)
        {
            _studentRepo = studentRepo;
            _logger = logger;
            _mapper = mapper;
            this._response = new();
        }
        /// <summary>
        ///  Gets All Student
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[ProducesResponseType(200, Type = typeof(List<StudentDto>))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetStudents()
        {
            try
            {
                IEnumerable<Student> studentList = await _studentRepo.GetAllAsync();
                _response.Result = _mapper.Map<List<StudentDto>>(studentList);
                _response.StatusCode = HttpStatusCode.OK;
                _logger.Log("Get all students", "info");

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
        /// Get Specific Student By Id
        /// </summary>
        /// <param name="studentId">the id of student</param>
        /// <returns></returns>
        [HttpGet("{studentId:int}", Name = "GetStudent")]
        //[ProducesResponseType(200, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(404)]
        [Authorize]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<APIResponse>> GetStudent(int studentId)
        {
            try
            {
                Student studentObj = await _studentRepo.GetAsync(u => u.Id == studentId);

                if(studentObj == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _logger.Log($"Student id {studentId} not exists", "error");
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<StudentDto>(studentObj);
                _response.StatusCode = HttpStatusCode.OK;
                _logger.Log($"Get student id {studentId}", "info");

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
        ///  Create a new student
        /// </summary>
        /// <param name="studentCreateDto"></param>
        /// <returns></returns>
        [HttpPost]
        //[ProducesResponseType(201, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateStudent([FromBody] StudentCreateDto studentCreateDto)
        {
            try
            {
                if(studentCreateDto == null)
                {
                    _logger.Log($"Not having Student to creating", "error");
                    return BadRequest(studentCreateDto);
                }

                if(await _studentRepo.GetAsync(u => u.StudentId.ToLower() == studentCreateDto.StudentId.ToLower()) != null)
                {
                    _logger.Log($"StudentID {studentCreateDto.StudentId} already exists", "error");
                    ModelState.AddModelError("", "StudentId is Exists");
                    return BadRequest(ModelState);
                }

                if(!ModelState.IsValid)
                {
                    _logger.Log($"Student needed to be created not valid!", "error");
                    ModelState.AddModelError("", "StudentId is not Valid");
                    return BadRequest(ModelState);
                }

                Student studentObj = _mapper.Map<Student>(studentCreateDto);
                await _studentRepo.CreateAsync(studentObj);

                _response.Result = _mapper.Map<StudentDto>(studentObj);
                _response.StatusCode = HttpStatusCode.Created;
                _logger.Log($"Created Student is {studentObj.Id}", "info");
                return CreatedAtRoute("GetStudent", new
                {
                    studentId = studentObj.Id
                }, _response);
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
            // return CreatedAtRoute("GetStudent", new
            // {
            //     version = HttpContext.GetRequestedApiVersion().ToString(),
            //     studentId = studentObj.Id
            // }, studentObj);
        }


        /// <summary>
        /// Edit student by id
        /// </summary>
        /// <param name="studentId"> id of student to be updating</param>
        /// <param name="studentDto"></param>
        /// <returns></returns>
        [HttpPatch("{studentId:int}", Name = "UpdateStudent")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateStudent(int studentId, [FromBody] StudentDto studentDto)
        {
            try
            {
                if(studentDto == null|| studentId != studentDto.Id)
                {
                    _logger.Log($"Not having Student to updating", "error");
                    return BadRequest(ModelState);
                }
                Student studentObj = _mapper.Map<Student>(studentDto);
                await _studentRepo.UpdateAsync(studentObj);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                _logger.Log($"Updated Student Id: {studentObj.Id} successfully!", "info");
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
        /// Delete a student by its Id
        /// </summary>
        /// <param name="studentId">id of student to be deleted</param>
        /// <returns></returns>
        [HttpDelete("{studentId:int}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteStudent(int studentId)
        {
            try
            {
                var studentObj = await _studentRepo.GetAsync(u => u.Id == studentId);

                if(studentObj == null)
                {
                    return NotFound();
                }

                await _studentRepo.RemoveAsync(studentObj);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                _logger.Log($"Deleted Student Id: {studentId} successfully!", "info");
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
