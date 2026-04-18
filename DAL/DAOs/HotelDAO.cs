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

        public IEnumerable<HotelEntity> SelectByCountryName(string countryName)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<HotelEntity>(
                    SQLQueries.Hotels_SelecyByCoutryName,
                    new { Country = countryName }
                );
            }
            catch (Exception)
            {
                return Enumerable.Empty<HotelEntity>();
            }
        }

        public IEnumerable<HotelEntity> SelectByEmail(string hotelEmail)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<HotelEntity>(
                    SQLQueries.Hotels_SelectByEmail,
                    new { Email = hotelEmail }
                );
            }
            catch (Exception)
            {
                return Enumerable.Empty<HotelEntity>();
            }
        }

        public int CheckHotelStatus(Guid hotelId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.ExecuteScalar<int>(
                    SQLQueries.Hotels_CheckHotelStatus,
                    new { Id = hotelId }
                );
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public HotelEntity Insert(HotelEntity hotel)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                hotel.Id = hotel.Id == Guid.Empty ? Guid.NewGuid() : hotel.Id;
                hotel.CreatedAt = DateTime.Now;
                hotel.UpdatedAt = DateTime.Now;

                sqlConnection.Execute(SQLQueries.Hotels_Insert, new
                {
                    hotel.Id,
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
                    hotel.Id,
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

        public bool UpdateHotelStatusTrue(Guid hotelId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    SQLQueries.Hotels_UpdateHotelStatus,
                    new
                    {
                        Id = hotelId,
                        UpdatedAt = DateTime.UtcNow
                    });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateHotelStatusRejected(Guid hotelId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    SQLQueries.Hotels_UpdateHotelStatusRejected,
                    new
                    {
                        Id = hotelId,
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