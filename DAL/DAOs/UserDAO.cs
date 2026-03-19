using DAL.Interfaces;
using Dapper;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.DAOs
{
    public class UserDAO : IUserDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory;

        public UserDAO(IFactory<SqlConnection> databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public IEnumerable<UserEntity> SelectAll()
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<UserEntity>(SQLQueries.Users_SelectAll);
            }
            catch (Exception)
            {
                return Enumerable.Empty<UserEntity>();
            }
        }

        public UserLoginDTO? SelectByEmail(string email)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.QueryFirstOrDefault<UserLoginDTO>(
                    SQLQueries.Users_SelectByEmail,
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
                    SQLQueries.Users_SelectById,
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
                    SQLQueries.Users_EmailExists,
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

                sqlConnection.Execute(SQLQueries.Users_Insert, new
                {
                    Id = userId,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.LoyaltyPoints,
                    user.AvatarURL,
                    user.PasswordHash,
                    user.MFAType,
                    user.TFASecret,
                    user.IsVerified,
                    user.Status,
                    user.RoleId,
                    user.LastLoginAt,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                return userId;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        public bool Update(UserEntity user)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(SQLQueries.Users_Update, new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.LoyaltyPoints,
                    user.AvatarURL,
                    user.PasswordHash,
                    user.MFAType,
                    user.TFASecret,
                    user.IsVerified,
                    user.Status,
                    user.RoleId,
                    user.LastLoginAt,
                    UpdatedAt = DateTime.UtcNow
                });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Guid id)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    SQLQueries.Users_Delete,
                    new { Id = id }
                );

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateLastLogin(Guid userId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    SQLQueries.Users_UpdateLastLogin,
                    new
                    {
                        UserId = userId,
                        LastLoginAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
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