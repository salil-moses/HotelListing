using AutoMapper;
using HotelListing.Data;
using HotelListing.Models;
using HotelListing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        // private readonly SignInManager<ApiUser> _signInManager;
        // SIGN IN MANAGER IS FOR MAINLY USER SESSIONS/cookies THAT ARE NOT REQD
        // WE ARE USING JWT Tokens
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;

        // public AccountController(UserManager<ApiUser> userManager, SignInManager<ApiUser> signInManager, ILogger<AccountController> logger, IMapper mapper)
        public AccountController(UserManager<ApiUser> userManager, ILogger<AccountController> logger, IMapper mapper, IAuthManager authManager)
        {
            _userManager = userManager;
            //_signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _authManager = authManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registration attempted by {userDTO.Email}");
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = _mapper.Map<ApiUser>(userDTO);
                user.UserName = userDTO.Email;
                // if username is empty, copy email to it
                // since username is reqd by .net identity
                // var result = await _userManager.CreateAsync(user);
                var result = await _userManager.CreateAsync(user, userDTO.Password);
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        //THis gets list of all errors
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    // return BadRequest("User Registration Failed. Try Again later");
                    // in PRODUCTION Log the error and return the above generic message
                    return BadRequest(ModelState);
                }
                // return Ok(result);
                await _userManager.AddToRolesAsync(user, userDTO.Roles); // or just add a default role

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in the {nameof(Register)}");
                // return StatusCode(500, "Internal Server Error. Please Try Again Later");
                return Problem($"Error in the {nameof(Register)}", statusCode: 500);
            }
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            _logger.LogInformation($"Login attempted by {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {                
                // var result = await _signInManager.PasswordSignInAsync(userDTO.Email, userDTO.Password, false, false); 
                if (!await _authManager.ValidateUser(userDTO))  // Just a dummy result
                {
                    // return BadRequest("User Login Failed. Try Again later");
                    return Unauthorized();
                }
                // return Ok(result);
                return Accepted(new { Token = await _authManager.CreateToken() }); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in the {nameof(Login)}");
                // return StatusCode(500, "Internal Server Error. Please Try Again Later");
                return Problem($"Error in the {nameof(Login)}", statusCode: 500);
            }
        }

    }
}
