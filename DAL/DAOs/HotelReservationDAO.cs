using DAL.Interfaces;
using Dapper;
using Domain.DTOs;
using Microsoft.Data.SqlClient;

namespace DAL.DAOs
{
    public class HotelReservationDAO(IFactory<SqlConnection> databaseFactory) : IHotelReservationDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory = databaseFactory;

        public IEnumerable<HotelReservationResponseDTO> SelectAll()
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();

                var sql = SQLQueries.HotelReservations_SelectAll;
                var reservations = sqlConnection.Query<HotelReservationResponseDTO, HotelResponseDTO, UserResponseDTO, HotelReservationRoomResponseDTO, HotelReservationResponseDTO>(
                    sql,
                    (res, hotel, user, room) =>
                    {
                        res.Hotel = hotel;
                        res.User = user;
                        res.Room = room;
                        return res;
                    },
                    splitOn: "HotelId,UserId,RoomId"
                );

                return reservations;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<HotelReservationResponseDTO>();
            }
        }

        public IEnumerable<HotelReservationResponseDTO> SelectByUserId(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();

                var sql = SQLQueries.HotelReservations_SelectByUserId;
                var reservations = sqlConnection.Query<HotelReservationResponseDTO, HotelResponseDTO, UserResponseDTO, HotelReservationRoomResponseDTO, HotelReservationResponseDTO>(
                     sql,
                     (res, hotel, user, room) =>
                     {
                         res.Hotel = hotel;
                         res.User = user;
                         res.Room = room;
                         return res;
                     },
                     new { UserId = id },
                     splitOn: "HotelId,UserId,RoomId"
                 );

                return reservations;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<HotelReservationResponseDTO>();
            }
        }

        public HotelReservationResponseDTO? SelectById(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();

                var sql = SQLQueries.HotelReservations_SelectById;

                var reservation = sqlConnection.Query<HotelReservationResponseDTO, HotelResponseDTO, UserResponseDTO, HotelReservationRoomResponseDTO, HotelReservationResponseDTO>(
                    sql,
                    (res, hotel, user, room) =>
                    {
                        res.Hotel = hotel;
                        res.User = user;
                        res.Room = room;
                        return res;
                    },
                    new { Id = id },
                    splitOn: "HotelId,UserId,RoomId"
                ).FirstOrDefault();

                return reservation;
            }
            catch (Exception ex)
            {
                // For debugging, consider logging ex.Message here
                return null;
            }
        }

        public bool Update(HotelReservationResponseDTO entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateHotelReservation(Guid reservationId, HotelReservationPatchDTO newReservationData)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();

                // 1. Initialize DynamicParameters and a list to hold our SQL SET clauses
                var parameters = new DynamicParameters();
                parameters.Add("@Id", reservationId);
                parameters.Add("@UpdatedAt", DateTime.UtcNow); // Always update the timestamp

                var setClauses = new List<string> { "[UpdatedAt] = @UpdatedAt" };

                if (newReservationData.CheckIn.HasValue)
                {
                    setClauses.Add("[CheckIn] = @CheckIn");
                    parameters.Add("@CheckIn", newReservationData.CheckIn.Value);
                }

                if (newReservationData.CheckOut.HasValue)
                {
                    setClauses.Add("[CheckOut] = @CheckOut");
                    parameters.Add("@CheckOut", newReservationData.CheckOut.Value);
                }

                // 3. If nothing was provided to update (except the timestamp), just return true or throw an error
                if (setClauses.Count == 1)
                {
                    return false; // Or throw a "No values provided for update" exception
                }

                var sql = $@"UPDATE [HotelReservation] SET {string.Join(", ", setClauses)} WHERE [Id] = @Id";
                var rowsAffected = sqlConnection.Execute(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public HotelReservationResponseDTO InsertHotelReservation(HotelReservationCreationDTO creationData)
        {
            try
            {
                Guid hotelId = Guid.NewGuid();

                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                var rowsAffected = sqlConnection.Execute(SQLQueries.HotelReservations_Insert, new
                {
                    Id = hotelId,
                    HotelID = creationData.HotelId,
                    UserID = creationData.UserId,
                    RoomID = creationData.RoomId,
                    CheckIn = creationData.CheckIn,
                    CheckOut = creationData.CheckOut,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                if (rowsAffected == 0)
                    return null;

                HotelReservationResponseDTO? createdHotelReservation = SelectById(hotelId);
                return createdHotelReservation;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public HotelReservationResponseDTO Insert(HotelReservationResponseDTO creationData)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                var rowsAffected = sqlConnection.Execute(SQLQueries.HotelReservations_Delete, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
