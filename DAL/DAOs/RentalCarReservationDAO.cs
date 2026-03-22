using DAL.Interfaces;
using Dapper;
using Domain.DTOs;
using Domain.DTOs.RentalCarReservation;
using Microsoft.Data.SqlClient;


namespace DAL.DAOs
{
    public class RentalCarReservationDAO(IFactory<SqlConnection> databaseFactory) : IRentalCarReservationDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory = databaseFactory;

        public RentalCarReservationResponseDTO InsertRentalCarReservation(RentalCarReservationCreationDTO creationData)
        {
            try
            {
                Guid reservationId = Guid.NewGuid();

                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                var rowsAffected = sqlConnection.Execute(SQLQueries.RentalCarReservations_Insert, new
                {
                    Id = reservationId,
                    UserId = creationData.UserId,
                    CarId = creationData.CarId,
                    UseFrom = creationData.UseFrom,
                    UseTo = creationData.UseTo,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                });

                if (rowsAffected == 0)
                    return null;

                RentalCarReservationResponseDTO? createdRentalCarReservation = SelectById(reservationId);
                return createdRentalCarReservation;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();
                var rowsAffected = sqlConnection.Execute(SQLQueries.RentalCarReservations_Delete, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public RentalCarReservationResponseDTO Insert(RentalCarReservationResponseDTO entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RentalCarReservationResponseDTO> SelectAll()
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();

                var sql = SQLQueries.RentalCarReservations_SelectAll;
                var reservations = sqlConnection.Query<RentalCarReservationResponseDTO, CarResponseDTO, UserResponseDTO, RentalCarReservationResponseDTO>(
                    sql,
                    (res, car, user) =>
                    {
                        res.Car = car;
                        res.User = user;
                        return res;
                    },
                    splitOn: "CarId,UserId"
                );

                return reservations;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<RentalCarReservationResponseDTO>();
            }
        }

        public RentalCarReservationResponseDTO? SelectById(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();

                var sql = SQLQueries.RentalCarReservations_SelectById;
                var reservation = sqlConnection.Query<RentalCarReservationResponseDTO, CarResponseDTO, UserResponseDTO, RentalCarReservationResponseDTO>(
                    sql,
                    (res, car, user) =>
                    {
                        res.Car = car;
                        res.User = user;
                        return res;
                    },
                    new { Id = id },
                    splitOn: "CarId,UserId"
                ).FirstOrDefault();

                return reservation;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<RentalCarReservationResponseDTO> SelectUserCarReservations(Guid id)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();

                var sql = SQLQueries.RentalCarReservations_SelectByUserId;
                var reservations = sqlConnection.Query<RentalCarReservationResponseDTO, CarResponseDTO, UserResponseDTO, RentalCarReservationResponseDTO>(
                    sql,
                    (res, car, user) =>
                    {
                        res.Car = car;
                        res.User = user;
                        return res;
                    },
                    new { Id = id },
                    splitOn: "CarId,UserId"
                );

                return reservations;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<RentalCarReservationResponseDTO>();
            }
        }

        public bool Update(RentalCarReservationResponseDTO entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRentalCarReservation(Guid reservationId, RentalCarReservationPatchDTO newReservationData)
        {
            try
            {
                using SqlConnection sqlConnection = _databaseFactory.GetConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", reservationId);
                parameters.Add("@UpdatedAt", DateTime.UtcNow); // Always update the timestamp

                var setClauses = new List<string> { "[UpdatedAt] = @UpdatedAt" };

                if (newReservationData.UseFrom.HasValue)
                {
                    setClauses.Add("[UseFrom] = @UseFrom");
                    parameters.Add("@UseFrom", newReservationData.UseFrom.Value);
                }

                if (newReservationData.UseTo.HasValue)
                {
                    setClauses.Add("[UseTo] = @UseTo");
                    parameters.Add("@UseTo", newReservationData.UseTo.Value);
                }

                if (newReservationData.CarID.HasValue)
                {
                    setClauses.Add("[CarID] = @CarID");
                    parameters.Add("@CarID", newReservationData.CarID.Value);
                }
            
                if (setClauses.Count == 1)
                {
                    return false;
                }

                var sql = $@"UPDATE [RentalCarReservation] SET {string.Join(", ", setClauses)} WHERE [Id] = @Id";
                var rowsAffected = sqlConnection.Execute(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
