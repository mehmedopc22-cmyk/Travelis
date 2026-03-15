using DAL.Interfaces;
using Dapper;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.DAO
{
    public class UserDAO : IUserDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory;

        public UserDAO(IFactory<SqlConnection> databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public UserLoginDTO? SelectByEmail(string email)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.QueryFirstOrDefault<UserLoginDTO>(
                    SQLQueries.SelectUserByEmail,
                    new { Email = email }
                );
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UserEntity? SelectById(Guid id)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.QueryFirstOrDefault<UserEntity>(
                    SQLQueries.SelectUserById,
                    new { Id = id }
                );
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool EmailExists(string email)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.ExecuteScalar<bool>(
                    SQLQueries.EmailExists,
                    new { Email = email }
                );
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Guid Insert(UserEntity user)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                Guid userId = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id;

                sqlConnection.Execute(SQLQueries.InsertUser, new
                {
                    Id = userId,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.PasswordHash,
                    user.RoleId,
                    user.Status,
                    user.IsVerified,
                    CreatedAt = DateTime.UtcNow
                });

                return userId;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        public bool UpdateLastLogin(Guid userId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    SQLQueries.UpdateLastLogin,
                    new
                    {
                        UserId = userId,
                        LastLoginAt = DateTime.UtcNow
                    });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}