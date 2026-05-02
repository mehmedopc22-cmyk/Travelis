using DAL.Interfaces;
using Dapper;
using Domain.DTOs.RentalCar;
using Domain.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.DAOs
{
    public class RentalCarDAO(IFactory<SqlConnection> databaseFactory) : IRentalCarDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory = databaseFactory;

        public RentalCarEntity? CreateForHotel(Guid hotelId, RentalCarCreationDTO rentalCar)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();
            sqlConnection.Open();

            using SqlTransaction transaction = sqlConnection.BeginTransaction();

            try
            {
                RentalCarEntity entity = new()
                {
                    Id = Guid.NewGuid(),
                    Brand = rentalCar.Brand,
                    Model = rentalCar.Model,
                    Kilometers = rentalCar.Kilometers
                };

                int insertedCars = sqlConnection.Execute(
                    SQLQueries.RentalCars_Insert,
                    entity,
                    transaction);

                int insertedLinks = sqlConnection.Execute(
                    SQLQueries.HotelRentalCars_Insert,
                    new
                    {
                        Id = Guid.NewGuid(),
                        HotelId = hotelId,
                        RentalCarId = entity.Id
                    },
                    transaction);

                if (insertedCars == 0 || insertedLinks == 0)
                {
                    transaction.Rollback();
                    return null;
                }

                transaction.Commit();
                return entity;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return null;
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                int rowsAffected = sqlConnection.Execute(SQLQueries.RentalCars_Delete, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public RentalCarEntity Insert(RentalCarEntity entity)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();
            sqlConnection.Execute(SQLQueries.RentalCars_Insert, entity);
            return entity;
        }

        public bool RemoveFromHotel(Guid hotelId, Guid rentalCarId)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                int rowsAffected = sqlConnection.Execute(
                    SQLQueries.HotelRentalCars_DeleteByHotelAndRentalCar,
                    new
                    {
                        HotelId = hotelId,
                        RentalCarId = rentalCarId
                    });

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<RentalCarEntity> SelectAll()
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                return sqlConnection.Query<RentalCarEntity>(SQLQueries.RentalCars_SelectAll);
            }
            catch (Exception)
            {
                return Enumerable.Empty<RentalCarEntity>();
            }
        }

        public RentalCarEntity? SelectById(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                return sqlConnection.QueryFirstOrDefault<RentalCarEntity>(
                    SQLQueries.RentalCars_SelectById,
                    new { Id = id });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<RentalCarEntity> SelectByHotelId(Guid hotelId)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                return sqlConnection.Query<RentalCarEntity>(
                    SQLQueries.RentalCars_SelectByHotelId,
                    new { HotelId = hotelId });
            }
            catch (Exception)
            {
                return Enumerable.Empty<RentalCarEntity>();
            }
        }

        public bool Update(RentalCarEntity entity)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                int rowsAffected = sqlConnection.Execute(SQLQueries.RentalCars_Update, entity);

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
