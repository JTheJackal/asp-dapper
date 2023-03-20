using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace asp_dapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        private readonly IConfiguration _config;
        public RoleController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet]
        [Route("All")]
        public async Task<ActionResult<List<Role>>> GetAllRoles()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Role> Roles = await SelectAllRoles(connection);

            return Ok(Roles);
        }


        [HttpGet("Get/{RoleId}")]
        public async Task<ActionResult<Role>> GetRole(int RoleId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            Role Role = await SelectSingleRole(RoleId, connection);

            return Ok(Role);
        }


        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<List<Role>>> AddRole(Role Role)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("INSERT INTO tblROLE (name) VALUES (@Name)", Role);

            return Ok(await SelectAllRoles(connection));
        }


        [HttpPut]
        [Route("Edit")]
        public async Task<ActionResult<List<Role>>> EditRole(Role Role)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("UPDATE tblROLE SET Name = @Name WHERE Id = @Id", Role);

            return Ok(await SelectSingleRole(Role.Id, connection));
        }


        [HttpDelete("Delete/{RoleId}")]
        public async Task<ActionResult<List<Role>>> DeleteRole(int RoleId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("DELETE FROM tblROLE WHERE ID = @Id", new
            {
                Id = RoleId
            });

            return Ok(await SelectAllRoles(connection));
        }


        // Return all Roles
        private static async Task<IEnumerable<Role>> SelectAllRoles(SqlConnection connection)
        {
            return await connection.QueryAsync<Role>("select * from tblROLE");
        }

        // Return a single Role with a specified ID
        private static async Task<Role> SelectSingleRole(int RoleId, SqlConnection connection)
        {
            return await connection.QueryFirstAsync<Role>("select * from tblROLE where id = @Id",
                            new { Id = RoleId });
        }
    }
}
