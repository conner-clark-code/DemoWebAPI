using Asp.Versioning;
using DemoWebAPI.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.Controllers.v2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class UsersController : ControllerBase
{
    public IConfiguration _config { get; }

    public UsersController(IConfiguration config)
    {
        _config = config;
    }

    // GET: api/<UsersController>
    [HttpGet]
    [AllowAnonymous]
    public IEnumerable<string> Get()
    {
        return ["value1", "value2"];
    }

    // GET api/<UsersController>/5
    [HttpGet("{id}")]
    [Authorize(Policy = PolicyConstants.MustBeTheOwner)]
    public string Get(int id)
    {
        return _config.GetConnectionString("DefaultConnection");
    }

    // POST: Creates a new record
    // POST api/<UsersController>
    [HttpPost]
    public void Post([FromBody] string value)
    {

    }

    // Updates a WHOLE record (or possibly create)
    // PUT api/<UsersController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // Updates a PARTIAL record
    // PATCH api/<UsersController>/5
    [HttpPatch("{id}")]
    public void Patch(int id, [FromBody] string email)
    {
    }

    // DELETE api/<UsersController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
