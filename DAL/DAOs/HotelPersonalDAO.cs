using DAL.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.DAOs
{
    public class HotelPersonalDAO(IFactory<SqlConnection> databaseFactory) : IHotelPersonalDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory = databaseFactory;

        public IEnumerable<HotelPersonalEntity> SelectAll()
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<HotelPersonalEntity>("SELECT HotelId, UserId FROM [HotelPersonal]");
            }
            catch (Exception)
            {
                return Enumerable.Empty<HotelPersonalEntity>();
            }
        }

        public IEnumerable<HotelPersonalEntity> SelectByHotelId(Guid hotelId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<HotelPersonalEntity>(
                    "SELECT HotelId, UserId FROM [HotelPersonal] WHERE HotelId = @HotelId",
                    new { HotelId = hotelId });
            }
            catch (Exception)
            {
                return Enumerable.Empty<HotelPersonalEntity>();
            }
        }

        public IEnumerable<HotelPersonalEntity> SelectByUserId(Guid userId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<HotelPersonalEntity>(
                    "SELECT HotelId, UserId FROM [HotelPersonal] WHERE UserId = @UserId",
                    new { UserId = userId });
            }
            catch (Exception)
            {
                return Enumerable.Empty<HotelPersonalEntity>();
            }
        }

        public bool Exists(Guid hotelId, Guid userId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.ExecuteScalar<int>(
                    """
                    SELECT COUNT(1)
                    FROM [HotelPersonal]
                    WHERE HotelId = @HotelId AND UserId = @UserId
                    """,
                    new { HotelId = hotelId, UserId = userId }) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Assign(Guid hotelId, Guid userId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                if (Exists(hotelId, userId))
                {
                    return true;
                }

                int rows = sqlConnection.Execute(
                    "INSERT INTO [HotelPersonal] (HotelId, UserId) VALUES (@HotelId, @UserId)",
                    new { HotelId = hotelId, UserId = userId });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Remove(Guid hotelId, Guid userId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    "DELETE FROM [HotelPersonal] WHERE HotelId = @HotelId AND UserId = @UserId",
                    new { HotelId = hotelId, UserId = userId });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
