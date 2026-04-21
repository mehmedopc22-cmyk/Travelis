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
                "INSERT INTO [Images] (Id, Name) VALUES (@Id, @Name)",
                image);

            return image;
        }

        public ImageEntity? SelectById(Guid imageId)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                return sqlConnection.QueryFirstOrDefault<ImageEntity>(
                    "SELECT Id, Name FROM [Images] WHERE Id = @ImageId",
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
                    """
                    SELECT i.Id, i.Name
                    FROM [UserImages] ui
                    INNER JOIN [Images] i ON i.Id = ui.ImageId
                    WHERE ui.UserId = @UserId
                    ORDER BY ui.Id DESC
                    """,
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
                    """
                    SELECT i.Id, i.Name
                    FROM [HotelImages] hi
                    INNER JOIN [Images] i ON i.Id = hi.ImageId
                    WHERE hi.HotelId = @HotelId
                    ORDER BY hi.Id DESC
                    """,
                    new { HotelId = hotelId });
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
                    "INSERT INTO [UserImages] (Id, UserId, ImageId) VALUES (@Id, @UserId, @ImageId)",
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
                    "INSERT INTO [HotelImages] (Id, HotelId, ImageId) VALUES (@Id, @HotelId, @ImageId)",
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
    }
}
