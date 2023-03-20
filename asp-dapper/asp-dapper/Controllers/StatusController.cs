using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace asp_dapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {

        private readonly IConfiguration _config;
        public StatusController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet]
        [Route("All")]
        public async Task<ActionResult<List<Status>>> GetAllStatuses()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Status> statuses = await SelectAllStatuses(connection);

            return Ok(statuses);
        }


        [HttpGet("Get/{statusId}")]
        public async Task<ActionResult<Status>> GetStatus(int statusId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            Status status = await SelectSingleStatus(statusId, connection);

            return Ok(status);
        }


        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<List<Status>>> AddStatus(Status status)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("INSERT INTO tblSTATUS (name) VALUES (@Name)", status);

            return Ok(await SelectAllStatuses(connection));
        }


        [HttpPut]
        [Route("Edit")]
        public async Task<ActionResult<List<Status>>> EditStatus(Status status)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("UPDATE tblSTATUS SET Name = @Name WHERE Id = @Id", status);

            return Ok(await SelectSingleStatus(status.Id, connection));
        }


        [HttpDelete("Delete/{statusId}")]
        public async Task<ActionResult<List<Status>>> DeleteStatus(int statusId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("DELETE FROM tblSTATUS WHERE ID = @Id", new
            {
                Id = statusId
            });

            return Ok(await SelectAllStatuses(connection));
        }


        // Return all statuses
        private static async Task<IEnumerable<Status>> SelectAllStatuses(SqlConnection connection)
        {
            return await connection.QueryAsync<Status>("select * from tblSTATUS");
        }

        // Return a single status with a specified ID
        private static async Task<Status> SelectSingleStatus(int statusId, SqlConnection connection)
        {
            return await connection.QueryFirstAsync<Status>("select * from tblSTATUS where id = @Id",
                            new { Id = statusId });
        }
    }
}
