using Services;
using Domain;
using Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EchoAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        public IConfiguration _configuration;
        UserService _service;

        public UserController(IConfiguration i, UserService s)
        {
            _configuration = i;
            _service = s;
        }

        [HttpGet]
        public List<User> Get()
        {
            return _service.Data();
        }

        // POST api/<ValuesController>
        [HttpPost]
        public IActionResult Post([FromBody] JsonObject data)
        {

            if (_service.userValidation(data))
            {
                string username = data["username"].ToString();
                string password = data["password"].ToString();
                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWTParams:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("UserId", username)
                    };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
                var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(

                _configuration["JWTParams:Issuer"],

                _configuration["JWTParams:Audience"],

                claims,

                expires: DateTime.UtcNow.AddMinutes(20),

                signingCredentials: mac);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            return NotFound();
        }

        // PUT api/<ValuesController>/5

        [HttpPost]
        [Route("signup")]
        public IActionResult SignUp([FromBody] JsonObject data)
        {
            _service.addUser(data);
            return Ok();
        }

        }

    
}
