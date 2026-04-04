using DAL.Interfaces;
using Dapper;
using Domain.DTOs;
using Domain.DTOs.HotelRoom;
using Microsoft.Data.SqlClient;

namespace DAL.DAOs
{
    public class HotelRoomDAO(IFactory<SqlConnection> databaseFactory) : IHotelRoomDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory = databaseFactory;

        public bool Delete(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                var rowsAffected = sqlConnection.Execute(SQLQueries.HotelRooms_Delete, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public HotelRoomResponseDTO Insert(HotelRoomResponseDTO entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HotelRoomResponseDTO> SelectAll()
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                var sql = SQLQueries.HotelRooms_SelectAll;
                var hotelRooms = sqlConnection.Query<HotelRoomResponseDTO, HotelRoomHotelResponseDTO, HotelRoomResponseDTO>(
                    sql,
                    (res, hotel) =>
                    {
                        res.Hotel = hotel;
                        return res;
                    },
                    splitOn: "HotelId"
                );

                return hotelRooms;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<HotelRoomResponseDTO>();
            }
        }

        public HotelRoomResponseDTO? SelectById(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                var sql = SQLQueries.HotelRooms_SelectById;
                var hotelRooms = sqlConnection.Query<HotelRoomResponseDTO, HotelRoomHotelResponseDTO, HotelRoomResponseDTO>(
                    sql,
                    (res, hotel) =>
                    {
                        res.Hotel = hotel;
                        return res;
                    },
                    new { Id = id },
                    splitOn: "HotelId"
                ).FirstOrDefault();

                return hotelRooms;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool Create(HotelRoomCreationDTO hotelRoomCreationDTO)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                var rowsAffected = sqlConnection.Execute(SQLQueries.HotelRooms_Insert, new
                {
                    Id = Guid.NewGuid(),
                    HotelId = hotelRoomCreationDTO.HotelId,
                    Description = hotelRoomCreationDTO.Description,
                    Price = hotelRoomCreationDTO.Price,
                    RoomNo = hotelRoomCreationDTO.RoomNo,
                    Floor = hotelRoomCreationDTO.Floor,
                    BedCount = hotelRoomCreationDTO.BedCount,
                    Capacity = hotelRoomCreationDTO.Capacity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateHotelRoom(Guid hotelRoomId, HotelRoomPatchDTO patchData)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();

                // 1. Initialize DynamicParameters and a list to hold our SQL SET clauses
                var parameters = new DynamicParameters();
                parameters.Add("@Id", hotelRoomId);
                parameters.Add("@UpdatedAt", DateTime.UtcNow); // Always update the timestamp

                var setClauses = new List<string> { "[UpdatedAt] = @UpdatedAt" };

                if (!string.IsNullOrEmpty(patchData.Description))
                {
                    setClauses.Add("[Description] = @Description");
                    parameters.Add("@Description", patchData.Description);
                }

                if (patchData.Price.HasValue)
                {
                    setClauses.Add("[Price] = @Price");
                    parameters.Add("@Price", patchData.Price.Value);
                }

                if (!string.IsNullOrEmpty(patchData.RoomNo))
                {
                    setClauses.Add("[RoomNo] = @RoomNo");
                    parameters.Add("@RoomNo", patchData.RoomNo);
                }

                if (patchData.Floor.HasValue)
                {
                    setClauses.Add("[Floor] = @Floor");
                    parameters.Add("@Floor", patchData.Floor);
                }

                if (patchData.BedCount.HasValue)
                {
                    setClauses.Add("[BedCount] = @BedCount");
                    parameters.Add("@BedCount", patchData.BedCount);
                }

                if (patchData.Capacity.HasValue)
                {
                    setClauses.Add("[Capacity] = @Capacity");
                    parameters.Add("@Capacity", patchData.Capacity);
                }

                // 3. If nothing was provided to update (except the timestamp), just return true or throw an error
                if (setClauses.Count == 1)
                {
                    return false; // Or throw a "No values provided for update" exception
                }

                var sql = $@"UPDATE [HotelRooms] SET {string.Join(", ", setClauses)} WHERE [Id] = @Id";
                var rowsAffected = sqlConnection.Execute(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(HotelRoomResponseDTO entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HotelRoomResponseDTO> SelectByHotelId(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                var sql = SQLQueries.HotelRooms_SelectByHotelId;
                var hotelRooms = sqlConnection.Query<HotelRoomResponseDTO, HotelRoomHotelResponseDTO, HotelRoomResponseDTO>(
                    sql,
                    (res, hotel) =>
                    {
                        res.Hotel = hotel;
                        return res;
                    },
                    new { Id = id },
                    splitOn: "HotelId"
                );

                return hotelRooms;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
