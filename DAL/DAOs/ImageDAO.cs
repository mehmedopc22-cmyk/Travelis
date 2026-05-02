using DAL.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.DAOs
{
    public class ImageDAO(IFactory<SqlConnection> databaseFactory) : IImageDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory = databaseFactory;

        public ImageEntity Insert(string imageName)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            ImageEntity image = new()
            {
                Id = Guid.NewGuid(),
                Name = imageName
            };

            sqlConnection.Execute(
                SQLQueries.Images_Insert,
                image);

            return image;
        }

        public ImageEntity? SelectById(Guid imageId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.QueryFirstOrDefault<ImageEntity>(
                    SQLQueries.Images_SelectById,
                    new { ImageId = imageId });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<ImageEntity> SelectByUserId(Guid userId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<ImageEntity>(
                    SQLQueries.Images_SelectByUserId,
                    new { UserId = userId });
            }
            catch (Exception)
            {
                return Enumerable.Empty<ImageEntity>();
            }
        }

        public IEnumerable<ImageEntity> SelectByHotelId(Guid hotelId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<ImageEntity>(
                    SQLQueries.Images_SelectByHotelId,
                    new { HotelId = hotelId });
            }
            catch (Exception)
            {
                return Enumerable.Empty<ImageEntity>();
            }
        }

        public IEnumerable<ImageEntity> SelectByRentalCarId(Guid rentalCarId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.Query<ImageEntity>(
                    SQLQueries.Images_SelectByRentalCarId,
                    new { RentalCarId = rentalCarId });
            }
            catch (Exception)
            {
                return Enumerable.Empty<ImageEntity>();
            }
        }

        public bool LinkUserImage(Guid userId, Guid imageId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    SQLQueries.UserImages_Link,
                    new
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        ImageId = imageId
                    });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool LinkHotelImage(Guid hotelId, Guid imageId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    SQLQueries.HotelImages_Link,
                    new
                    {
                        Id = Guid.NewGuid(),
                        HotelId = hotelId,
                        ImageId = imageId
                    });

                return rows > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool LinkRentalCarImage(Guid rentalCarId, Guid imageId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                int rows = sqlConnection.Execute(
                    SQLQueries.RentalCarImages_Link,
                    new
                    {
                        Id = Guid.NewGuid(),
                        RentalCarId = rentalCarId,
                        ImageId = imageId
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
