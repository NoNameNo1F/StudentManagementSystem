using AutoMapper;
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
        }
        /// <summary>
        ///  Gets All Student
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<StudentDto>))]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var studentList = await _studentRepo.GetAllAsync();
            var studentDto = new List<StudentDto>();

            studentDto = _mapper.Map<List<StudentDto>>(studentList);
            _logger.Log("Get all students", "info");
            return Ok(studentDto);
        }
        /// <summary>
        /// Get Specific Student By Id
        /// </summary>
        /// <param name="studentId">the id of student</param>
        /// <returns></returns>
        [HttpGet("{studentId:int}", Name = "GetStudent")]
        [ProducesResponseType(200, Type = typeof(StudentDto))]
        [ProducesResponseType(404)]
        [Authorize]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<StudentDto>> GetStudent(int studentId)
        {
            var studentObj = await _studentRepo.GetAsync(u => u.Id == studentId);

            if(studentObj == null)
            {
                _logger.Log($"Student id {studentId} not exists", "error");
                return NotFound();
            }
            var studentDto = _mapper.Map<StudentDto>(studentObj);
            _logger.Log($"Get student id {studentId}", "info");
            return Ok(studentDto);
        }
        /// <summary>
        ///  Create a new student
        /// </summary>
        /// <param name="studentCreateDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] StudentCreateDto studentCreateDto)
        {
            if(studentCreateDto == null)
            {
                _logger.Log($"Not having Student to creating", "error");
                return BadRequest(ModelState);
            }

            if(await _studentRepo.GetAsync(u => u.StudentId.ToLower() == studentCreateDto.StudentId) != null)
            {
                _logger.Log($"StudentID {studentCreateDto.StudentId} already exists", "error");
                ModelState.AddModelError("", "StudentId is Exists");
                return StatusCode(404, ModelState);
            }

            if(!ModelState.IsValid)
            {
                _logger.Log($"Student needed to be created not valid!", "error");
                return BadRequest(ModelState);
            }

            var studentObj = _mapper.Map<Student>(studentCreateDto);
            // if(!_studentRepo.CreateStudent(studentObj))
            // {
            //     _logger.Log($"Student can't be created!", "error");
            //     ModelState.AddModelError("", $"Something went wrong when saving record {studentObj.StudentId}");
            //     return StatusCode(500, ModelState);
            // }
            await _studentRepo.CreateAsync(studentObj);
            _logger.Log($"Created Student is {studentObj.Id}", "info");
            return CreatedAtRoute("GetStudent", new
            {
                studentId = studentObj.Id
            }, studentObj);
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
        public async Task<IActionResult> UpdateStudent(int studentId, [FromBody] StudentDto studentDto)
        {
            if(studentDto == null|| studentId != studentDto.Id)
            {
                _logger.Log($"Not having Student to updating", "error");
                return BadRequest(ModelState);
            }

            var studentObj = _mapper.Map<Student>(studentDto);

            // if(!_studentRepo.UpdateStudent(studentObj))
            // {
            //     _logger.Log($"Student can't be updated!", "error");
            //     ModelState.AddModelError("",$"Something went wrong when saving record {studentObj.Id}");
            //     return StatusCode(500, ModelState);
            // }
            await _studentRepo.UpdateAsync(studentObj);
            _logger.Log($"Updated Student Id: {studentObj.Id} successfully!", "info");
            return NoContent();
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
        public async Task<IActionResult> DeleteStudent(int studentId)
        {
            // if(studentId == null)
            // {
            //     _logger.LogError($"Student id {studentId} not exists");
            //     return NotFound();
            // }

            var studentObj = await _studentRepo.GetAsync(u => u.Id == studentId);
            // if(!_studentRepo.DeleteStudent(studentObj))
            // {
            //     _logger.Log($"Student can't be deleted!", "error");
            //     ModelState.AddModelError("",$"Something went wrong when deleting record {studentObj.Id}");
            //     return StatusCode(500, ModelState);
            // }
            await _studentRepo.RemoveAsync(studentObj);
            _logger.Log($"Deleted Student Id: {studentId} successfully!", "info");
            return NoContent();
        }
    }
}
