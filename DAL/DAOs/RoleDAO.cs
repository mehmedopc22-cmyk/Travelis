using Dapper;
using DAL.Interfaces;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL.DAOs
{
    public class RoleDAO(IFactory<SqlConnection> databaseFactory) : IRoleDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory = databaseFactory;

        public IEnumerable<RoleEntity> SelectAll()
        {
            using IDbConnection db = _databaseFactory.GetConnection();

            return db.Query<RoleEntity>(SQLQueries.Roles_SelectAll);
        }

        public RoleEntity? SelectById(Guid roleId)
        {
            using IDbConnection db = _databaseFactory.GetConnection();

            return db.QueryFirstOrDefault<RoleEntity>(
                "SELECT Id, Name, Description, CreatedAt, UpdatedAt FROM Roles WHERE Id = @Id",
                new { Id = roleId });
        }
    }
}
