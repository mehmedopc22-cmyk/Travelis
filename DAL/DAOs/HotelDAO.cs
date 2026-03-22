using DAL.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.DAOs
{
    public class HotelDAO : IHotelDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory;

        public HotelDAO(IFactory<SqlConnection> databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public IEnumerable<HotelEntity> SelectAll()
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<HotelEntity>(SQLQueries.Hotels_SelectAll);
            }
            catch (Exception)
            {
                return Enumerable.Empty<HotelEntity>();
            }
        }

        public HotelEntity? SelectById(Guid id)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.QueryFirstOrDefault<HotelEntity>(
                    SQLQueries.Hotels_SelectById,
                    new { Id = id }
                );
            }
            catch (Exception)
            {
                return null;
            }
        }

        public HotelEntity Insert(HotelEntity hotel)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                hotel.HotelId = hotel.HotelId == Guid.Empty ? Guid.NewGuid() : hotel.HotelId;

                hotel.CreatedAt = DateTime.Now;
                hotel.UpdatedAt = DateTime.Now;

                sqlConnection.Execute(SQLQueries.Hotels_Insert, new
                {
                    hotel.HotelId,
                    hotel.Name,
                    hotel.Country,
                    hotel.City,
                    hotel.Street,
                    hotel.PostalCode,
                    hotel.PhoneNumber,
                    hotel.Email,
                    hotel.Status,
                    hotel.Approved,
                    hotel.CreatedAt,
                    hotel.UpdatedAt
                });

                return hotel;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while inserting Hotel", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while inserting Hotel", ex);
            }
        }

        public bool Update(HotelEntity hotel)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(SQLQueries.Hotels_Update, new
                {
                    hotel.HotelId,
                    hotel.Name,
                    hotel.Country,
                    hotel.City,
                    hotel.Street,
                    hotel.PostalCode,
                    hotel.PhoneNumber,
                    hotel.Email,
                    hotel.Status,
                    hotel.Approved,
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
                    SQLQueries.Hotels_Delete,
                    new { Id = id }
                );

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}