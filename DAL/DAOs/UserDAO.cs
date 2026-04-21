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
                    """
                    SELECT
                        Id,
                        Email,
                        FirstName,
                        LastName,
                        LoyaltyPoints,
                        AvatarID,
                        PasswordHash,
                        MFAType,
                        TFASecret,
                        IsVerified,
                        Status,
                        RoleId,
                        LastLoginAt,
                        CreatedAt,
                        UpdatedAt
                    FROM Users
                    WHERE Id = @Id
                    """,
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

        public UserEntity Insert(UserEntity user)
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
                    user.AvatarID,
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

                return user;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while inserting User", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while inserting User", ex);
            }
        }

        public UserEntity? SelectUserByEmail(string email)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.QueryFirstOrDefault<UserEntity>(
                    SQLQueries.Users_SelectUserByEmail,
                    new { Email = email }
                );
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<UserEntity> SelectByRoleId(Guid roleId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<UserEntity>(
                    SQLQueries.Users_SelectByRoleId,
                    new { RoleId = roleId }
                );
            }
            catch (Exception)
            {
                return Enumerable.Empty<UserEntity>();
            }
        }

        public IEnumerable<UserEntity> SelectByStatus(byte status)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<UserEntity>(
                    SQLQueries.Users_SelectByStatus,
                    new { Status = status }
                );
            }
            catch (Exception)
            {
                return Enumerable.Empty<UserEntity>();
            }
        }

        public bool UpdateStatus(Guid userId, byte status)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    SQLQueries.Users_UpdateStatus,
                    new
                    {
                        UserId = userId,
                        Status = status,
                        UpdatedAt = DateTime.UtcNow
                    });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateVerification(Guid userId, bool isVerified)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    SQLQueries.Users_UpdateVerification,
                    new
                    {
                        UserId = userId,
                        IsVerified = isVerified,
                        UpdatedAt = DateTime.UtcNow
                    });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePasswordHash(Guid userId, string passwordHash)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    "UPDATE Users SET PasswordHash = @PasswordHash, UpdatedAt = @UpdatedAt WHERE Id = @UserId",
                    new
                    {
                        UserId = userId,
                        PasswordHash = passwordHash,
                        UpdatedAt = DateTime.UtcNow
                    });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateRole(Guid userId, Guid roleId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    "UPDATE Users SET RoleId = @RoleId, UpdatedAt = @UpdatedAt WHERE Id = @UserId",
                    new
                    {
                        UserId = userId,
                        RoleId = roleId,
                        UpdatedAt = DateTime.UtcNow
                    });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateAvatar(Guid userId, Guid imageId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    "UPDATE Users SET AvatarID = @ImageId, UpdatedAt = @UpdatedAt WHERE Id = @UserId",
                    new
                    {
                        UserId = userId,
                        ImageId = imageId,
                        UpdatedAt = DateTime.UtcNow
                    });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
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
                    user.AvatarID,
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
