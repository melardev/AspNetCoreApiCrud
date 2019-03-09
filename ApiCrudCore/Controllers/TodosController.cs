using System.Net;
using System.Threading.Tasks;
using ApiCrudCore.Entities;
using ApiCrudCore.Enums;
using ApiCrudCore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ApiCrudCore.Controllers
{
    [Route("api/[controller]")]
    public class TodosController : Controller
    {
        private readonly TodoService _todosService;

        public TodosController(TodoService todosService)
        {
            _todosService = todosService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var todos = await _todosService.FetchMany(TodoShow.All);
            return new OkObjectResult(todos);
        }

        [HttpGet]
        [Route("pending")]
        public async Task<IActionResult> GetPending([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var todos = await _todosService.FetchMany(TodoShow.Pending);
            return Ok(todos);
        }

        [HttpGet]
        [Route("completed")]
        public async Task<IActionResult> GetCompleted([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var todos = await _todosService.FetchMany(TodoShow.Completed);
            return StatusCode((int) HttpStatusCode.OK, Json(todos));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetTodoDetails(int id) => Json(await _todosService.Get(id));

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] Todo todo)
        {
            await _todosService.CreateTodo(todo);
            var response = new ObjectResult(todo);
            response.StatusCode = (int) HttpStatusCode.Created;
            return response;
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] Todo todo) =>
            new OkObjectResult(await _todosService.Update(id, todo));


        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteTodo(int id)
        {
            EntityEntry<Todo> result = _todosService.Delete(id);
            return NoContent();
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _todosService.DeleteAll();
            return new NoContentResult();
        }
    }
}