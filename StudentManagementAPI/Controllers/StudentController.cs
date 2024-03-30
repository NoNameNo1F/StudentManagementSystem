using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.Models;
using StudentManagementAPI.Models.Dtos.StudentDto;
using StudentManagementAPI.Repositories.IRepositories;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/v{version:apiVersion}/nationalparks")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class StudentController : ControllerBase
    {
        private IStudentRepository _studentRepo;
        private readonly IMapper _mapper;
        private ILogger<StudentController> _logger;

        public StudentController(IStudentRepository studentRepo, ILogger<StudentController> logger, IMapper mapper)
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
        public IActionResult GetStudents()
        {
            var studentList = _studentRepo.GetStudents();
            var studentDto = new List<StudentDto>();

            foreach (var student in studentList)
            {
                studentDto.Add(_mapper.Map<StudentDto>(student));
            }
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
        public IActionResult GetStudent(int studentId)
        {
            var studentObj = _studentRepo.GetStudent(studentId);

            if(studentObj == null)
                return NotFound();

            var studentDto = _mapper.Map<StudentDto>(studentObj);

            return Ok(studentDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateStudent([FromBody] StudentCreateDto studentCreateDto)
        {
            if(studentCreateDto == null)
            {
                return BadRequest(ModelState);
            }

            if(_studentRepo.ExistsStudentByStudentId(studentCreateDto.StudentId))
            {
                ModelState.AddModelError("", "StudentId is Exists");
                return StatusCode(404, ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var studentObj = _mapper.Map<Student>(studentCreateDto);
            if(!_studentRepo.CreateStudent(studentObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving record {studentObj.StudentId}");
                return StatusCode(500, ModelState);
            }
            //return Created("GetStudent", studentObj.Id);
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

        [HttpPatch("{studentId:int}", Name = "UpdateStudent")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateStudent(int studentId, [FromBody] StudentDto studentDto)
        {
            if(studentDto == null|| studentId != studentDto.Id)
            {
                return BadRequest(ModelState);
            }

            var studentObj = _mapper.Map<Student>(studentDto);

            if(!_studentRepo.UpdateStudent(studentObj))
            {
                ModelState.AddModelError("",$"Something went wrong when saving record {studentObj.Id}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{studentId:int}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteStudent(int studentId)
        {
            if(studentId == null)
            {
                return NotFound();
            }

            var studentObj = _studentRepo.GetStudent(studentId);
            if(!_studentRepo.DeleteStudent(studentObj))
            {
                ModelState.AddModelError("",$"Something went wrong when deleting record {studentObj.Id}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
