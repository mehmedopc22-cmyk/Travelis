using DAL.Interfaces;
using Dapper;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.DAOs;

public class TaxiReservationDAO(IFactory<SqlConnection> databaseFactory) : ITaxiReservationDAO
{
    private readonly IFactory<SqlConnection> _databaseFactory = databaseFactory;

   

    public IEnumerable<TaxiReservationEntity> SelectAll()
    {
        using SqlConnection sqlConnection = _databaseFactory.GetConnection();

        try
        {
            return sqlConnection.Query<TaxiReservationEntity>(SQLQueries.TaxiReservations_SelectAll);
        }
        catch (Exception)
        {
            return Enumerable.Empty<TaxiReservationEntity>();
        }
    }

    public TaxiReservationEntity? SelectById(Guid Id)
    {
        using SqlConnection sqlConnection = _databaseFactory.GetConnection();

        try
        {
            return sqlConnection.QuerySingle<TaxiReservationEntity>(SQLQueries.TaxiReservations_SelectById, new{ Id = Id });
        }
        catch (SqlException ex)
        {
            return Enumerable.Empty<TaxiReservationEntity>().FirstOrDefault();
        }
    }

    public TaxiReservationEntity Insert(TaxiReservationEntity taxiReservation)
    {
        using SqlConnection sqlConnection = _databaseFactory.GetConnection();

        try
        {
            Guid taxiReservationId = taxiReservation.Id == Guid.Empty ? Guid.NewGuid() : taxiReservation.Id;

            sqlConnection.Execute(SQLQueries.TaxiReservations_Insert, new
            {
                Id = taxiReservationId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                taxiReservation.DestinationAddress,
                taxiReservation.PickupAddress,
                TaxiCompantyID = taxiReservation.TaxiCompanyID,
                taxiReservation.UserID,
                taxiReservation.Time
            });
            return taxiReservation;
        }
        catch (SqlException ex)
        {
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public bool Update(TaxiReservationEntity taxiReservation)
    {
        using SqlConnection sqlConnection = _databaseFactory.GetConnection();

        try
        {
            int rows = sqlConnection.Execute(SQLQueries.TaxiReservations_Update, new
            {
                taxiReservation.Id,
                taxiReservation.DestinationAddress,
                taxiReservation.PickupAddress,
                TaxiCompantyID = taxiReservation.TaxiCompanyID,
                taxiReservation.UserID,
                taxiReservation.Time,
                UpdatedAt = DateTime.UtcNow
            });

            return rows > 0;
        }

        catch (SqlException ex)
        {
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
        
    }

    public bool Delete(Guid id)
    {
        using SqlConnection sqlConnection = _databaseFactory.GetConnection();

        try
        {
            int rows = sqlConnection.Execute(SQLQueries.TaxiReservations_Delete, new
            {
                Id = id
            });
            return rows == 1;
        }
        catch (SqlException ex)
        {
            return  false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public IEnumerable<TaxiReservationEntity> SelectByUserId(Guid userId)
    {
        using SqlConnection sqlConnection = _databaseFactory.GetConnection();

        try
        {
            return sqlConnection.Query<TaxiReservationEntity>(SQLQueries.TaxiReservations_SelectByUserId,
                new { UserId = userId });
        }
        catch (Exception)
        {
            return Enumerable.Empty<TaxiReservationEntity>();
        }
    
    }

    public IEnumerable<TaxiReservationResponseDTO> SelectAllWithUserName()
    {
        using SqlConnection sqlConnection = _databaseFactory.GetConnection();

        try
        {
            return sqlConnection.Query<TaxiReservationResponseDTO>(SQLQueries.TaxiReservations_SelectAllWithUserName);
        }
        catch (Exception)
        {
            return Enumerable.Empty<TaxiReservationResponseDTO>();
        }
        
    }

    public IEnumerable<TaxiReservationResponseDTO> SelectByUserIdWithUserName(Guid userId)
    {
        using SqlConnection sqlConnection = _databaseFactory.GetConnection();

        try
        {
            return sqlConnection.Query<TaxiReservationResponseDTO>(SQLQueries.TaxiReservations_SelectByUserIdWithUserName,
                new { UserId = userId });
        }
        catch (Exception)
        {
            return Enumerable.Empty<TaxiReservationResponseDTO>();
        }
    }

    public TaxiReservationCreationDTO InsertSimple(TaxiReservationCreationDTO taxiReservation)
    {
        using SqlConnection sqlConnection = _databaseFactory.GetConnection();

        try
        {
            //Guid taxiReservationId = taxiReservation.Id == Guid.Empty ? Guid.NewGuid() : taxiReservation.Id;

            sqlConnection.Execute(SQLQueries.TaxiReservations_InsertSimple, new
            {
                UserID = taxiReservation.UserId,
                taxiReservation.TaxiCompanyId,
                taxiReservation.PickupAddress,
                taxiReservation.DestinationAddress,
                taxiReservation.Time,
                taxiReservation.Status
            });
            return taxiReservation;
        }
        catch (SqlException ex)
        {
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public bool UpdateSimple(TaxiReservationUpdateDTO taxiReservation)
    {
        using SqlConnection sqlConnection = _databaseFactory.GetConnection();

        try
        {
            int rows = sqlConnection.Execute(SQLQueries.TaxiReservations_UpdateSimple, new
            {
                taxiReservation.Id,
                taxiReservation.DestinationAddress,
                taxiReservation.PickupAddress,
                taxiReservation.TaxiCompanyId,
                taxiReservation.Time,
                taxiReservation.Status,
                UpdatedAt = DateTime.UtcNow
            });

            return rows > 0;
        }

        catch (SqlException ex)
        {
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}