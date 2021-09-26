using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DaprSample.Shared.Models;
using System;
using DaprSample.MicroService.UsersService.Helpers;
using DaprSample.MicroService.UsersService.Datas.Enums;
using DaprSample.MicroService.UsersService.Configs.Exceptions;
using DaprSample.MicroService.UsersService.Datas.Dto;
using DaprSample.MicroService.UsersService.Services;

namespace DaprSample.MicroService.UsersService.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController
    {
        private UserService _service;
 
        public UsersController(UserService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetUserById(long id)
        {
            try
            {
                var user = await _service.GetUserById(id);
                return ResponseHelper.GenerateResponse<User>(user, EnumResponseStatus.Success);
            }
            catch(ResourceNotFoundException e)
            {
                return ResponseHelper.GenerateResponse<string>(e.Message, EnumResponseStatus.NotFound);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return ResponseHelper.GenerateResponse<string>(e.Message, EnumResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreateUser(User user)
        {
            try
            {
                var addedUser = await _service.AddUser(user);
                return ResponseHelper.GenerateResponse<User>(addedUser, EnumResponseStatus.Success);               
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.Message);
                return ResponseHelper.GenerateResponse<string>(e.Message, EnumResponseStatus.Fail);
            }
        }

        [Route("login")]
        public async Task<JsonResult> Login(User user)
        {
            try
            {
                var loginUser = await _service.Login(user);
                return ResponseHelper.GenerateResponse<UserResponse>(loginUser, EnumResponseStatus.Success);
            }
            catch(ResourceNotFoundException e)
            {
                return ResponseHelper.GenerateResponse<string>(e.Message, EnumResponseStatus.NotFound);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException.Message);
                return ResponseHelper.GenerateResponse<string>(e.Message, EnumResponseStatus.Fail);
            }
        }
    }
}
