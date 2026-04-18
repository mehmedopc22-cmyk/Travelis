using DAL.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;

namespace DAL.DAOs
{
    public class TaxiCompanyDAO(IFactory<SqlConnection> databaseFactory) : ITaxiCompanyDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory = databaseFactory;

        public IEnumerable<TaxiCompanyEntity> SelectAll()
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();
            try
            {
                return sqlConnection.Query<TaxiCompanyEntity>(SQLQueries.TaxiCompanies_SelectAll);
            }
            catch (Exception)
            {
                return Enumerable.Empty<TaxiCompanyEntity>();
            }
        }

        public TaxiCompanyEntity? SelectById(Guid id)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();
            try
            {
                return sqlConnection.QueryFirstOrDefault<TaxiCompanyEntity>(
                    SQLQueries.TaxiCompanies_SelectById,
                    new { Id = id });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TaxiCompanyEntity Insert(TaxiCompanyEntity taxiCompany)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();
            try
            {
                taxiCompany.Id = taxiCompany.Id == Guid.Empty ? Guid.NewGuid() : taxiCompany.Id;
                taxiCompany.CreatedAt = DateTime.UtcNow;
                taxiCompany.UpdatedAt = DateTime.UtcNow;

                sqlConnection.Execute(SQLQueries.TaxiCompanies_Insert, new
                {
                    taxiCompany.Id,
                    taxiCompany.Name,
                    taxiCompany.Country,
                    taxiCompany.City,
                    taxiCompany.Street,
                    taxiCompany.PostalCode,
                    taxiCompany.PhoneNumber,
                    taxiCompany.Email,
                    taxiCompany.Status,
                    taxiCompany.Approved,
                    taxiCompany.CreatedAt,
                    taxiCompany.UpdatedAt
                });

                return taxiCompany;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while inserting TaxiCompany", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while inserting TaxiCompany", ex);
            }
        }
    }
}
