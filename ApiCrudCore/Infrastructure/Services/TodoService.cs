using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCrudCore.Data;
using ApiCrudCore.Entities;
using ApiCrudCore.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ApiCrudCore.Infrastructure.Services
{
    public class TodoService
    {
        private readonly ApplicationDbContext _context;

        public TodoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Todo>> FetchMany(TodoShow show = TodoShow.All)
        {
            IQueryable<Todo> queryable = null;

            if (show == TodoShow.Completed)
            {
                queryable = _context.Todos.Where(t => t.Completed);
            }
            else if (show == TodoShow.Pending)
            {
                queryable = _context.Todos.Where(t => !t.Completed);
            }


            List<Todo> todos;
            if (queryable != null)
            {
                todos = await queryable.Select(t => new Todo
                {
                    Id = t.Id,
                    Title = t.Title,
                    Completed = t.Completed,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToListAsync();
            }
            else
            {
                todos = await _context.Todos.Select(t => new Todo
                {
                    Id = t.Id,
                    Title = t.Title,
                    Completed = t.Completed,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToListAsync();
            }


            return todos;
        }


        /// <summary>  
        /// Return a To do object 
        /// </summary>  
        /// <param name="todoId"></param>  
        /// <returns></returns>  
        public async Task<Todo> Get(int todoId) => await _context.Todos.FindAsync(todoId);

        public async Task CreateTodo(Todo todo)
        {
            await _context.Todos.AddAsync(todo);
            await _context.SaveChangesAsync();
        }

        public async Task<Todo> Update(int id, Todo todoFromUser)
        {
            Todo todoFromDb = _context.Todos.First(t => t.Id == id);
            todoFromDb.Title = todoFromUser.Title;

            if (todoFromUser.Description != null)
                todoFromDb.Description = todoFromUser.Description;

            todoFromDb.Completed = todoFromUser.Completed;

            _context.Entry(todoFromDb).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return todoFromDb;
        }


        public async Task<Todo> Update(Todo currentTodo, Todo todoFromUser)
        {
            currentTodo.Title = todoFromUser.Title;

            if (todoFromUser.Description != null)
                currentTodo.Description = todoFromUser.Description;

            currentTodo.Completed = todoFromUser.Completed;

            _context.Entry(currentTodo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return currentTodo;
        }

        /// <summary>  
        /// Deletes a To do
        /// </summary>  
        /// <param name="todoId"></param>  
        /// <returns></returns> 
        public EntityEntry<Todo> Delete(int todoId)
        {
            EntityEntry<Todo> result = _context.Todos.Remove(Get(todoId).Result);
            _context.SaveChangesAsync();
            return result;
        }

        public async Task DeleteAll()
        {
            _context.Todos.RemoveRange(_context.Todos);
            await _context.SaveChangesAsync();
        }

        public async Task<Todo> GetById(int id)
        {
            return await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}