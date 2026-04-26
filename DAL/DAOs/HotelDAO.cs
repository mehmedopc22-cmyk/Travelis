using DAL.Interfaces;
using Dapper;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using System.Text;

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

        public IEnumerable<HotelEntity> SelectFiltered(HotelFilterRequestDTO filters)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                DynamicParameters parameters = new();
                StringBuilder sql = new(SQLQueries.Hotels_SelectFilteredBase);

                if (!string.IsNullOrWhiteSpace(filters.Destination))
                {
                    sql.AppendLine();
                    sql.AppendLine(SQLQueries.Hotels_SelectFilteredDestinationClause);
                    parameters.Add("@Destination", $"%{filters.Destination.Trim()}%");
                }

                if (filters.MinPrice.HasValue || filters.MaxPrice.HasValue)
                {
                    sql.AppendLine();
                    sql.AppendLine(SQLQueries.Hotels_SelectFilteredRoomExistsStart);

                    if (filters.MinPrice.HasValue)
                    {
                        sql.AppendLine(SQLQueries.Hotels_SelectFilteredMinPriceClause);
                        parameters.Add("@MinPrice", filters.MinPrice.Value);
                    }

                    if (filters.MaxPrice.HasValue)
                    {
                        sql.AppendLine(SQLQueries.Hotels_SelectFilteredMaxPriceClause);
                        parameters.Add("@MaxPrice", filters.MaxPrice.Value);
                    }

                    sql.AppendLine(SQLQueries.Hotels_SelectFilteredRoomExistsEnd);
                }

                sql.AppendLine();
                sql.AppendLine(GetHotelSortClause(filters.SortBy));

                return sqlConnection.Query<HotelEntity>(sql.ToString(), parameters);
            }
            catch (Exception)
            {
                return Enumerable.Empty<HotelEntity>();
            }
        }

        public PagedResultDTO<HotelEntity> SelectFilteredPaged(HotelFilterRequestDTO filters)
        {
            using SqlConnection sqlConnection = _databaseFactory.GetConnection();

            try
            {
                DynamicParameters parameters = new();
                StringBuilder selectSql = new(SQLQueries.Hotels_SelectFilteredBase);
                StringBuilder countSql = new("SELECT COUNT(*) FROM Hotels h WHERE 1 = 1");

                if (!string.IsNullOrWhiteSpace(filters.Destination))
                {
                    selectSql.AppendLine();
                    selectSql.AppendLine(SQLQueries.Hotels_SelectFilteredDestinationClause);
                    countSql.AppendLine();
                    countSql.AppendLine(SQLQueries.Hotels_SelectFilteredDestinationClause);
                    parameters.Add("@Destination", $"%{filters.Destination.Trim()}%");
                }

                if (filters.MinPrice.HasValue || filters.MaxPrice.HasValue)
                {
                    selectSql.AppendLine();
                    selectSql.AppendLine(SQLQueries.Hotels_SelectFilteredRoomExistsStart);
                    countSql.AppendLine();
                    countSql.AppendLine(SQLQueries.Hotels_SelectFilteredRoomExistsStart);

                    if (filters.MinPrice.HasValue)
                    {
                        selectSql.AppendLine(SQLQueries.Hotels_SelectFilteredMinPriceClause);
                        countSql.AppendLine(SQLQueries.Hotels_SelectFilteredMinPriceClause);
                        parameters.Add("@MinPrice", filters.MinPrice.Value);
                    }

                    if (filters.MaxPrice.HasValue)
                    {
                        selectSql.AppendLine(SQLQueries.Hotels_SelectFilteredMaxPriceClause);
                        countSql.AppendLine(SQLQueries.Hotels_SelectFilteredMaxPriceClause);
                        parameters.Add("@MaxPrice", filters.MaxPrice.Value);
                    }

                    selectSql.AppendLine(SQLQueries.Hotels_SelectFilteredRoomExistsEnd);
                    countSql.AppendLine(SQLQueries.Hotels_SelectFilteredRoomExistsEnd);
                }

                int pageSize = filters.PageSize <= 0 ? 5 : Math.Min(filters.PageSize, 25);
                int totalCount = sqlConnection.ExecuteScalar<int>(countSql.ToString(), parameters);
                int totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
                int page = Math.Clamp(filters.Page <= 0 ? 1 : filters.Page, 1, totalPages);

                parameters.Add("@Offset", (page - 1) * pageSize);
                parameters.Add("@PageSize", pageSize);

                selectSql.AppendLine();
                selectSql.AppendLine(GetHotelSortClause(filters.SortBy));
                selectSql.AppendLine("OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

                return new PagedResultDTO<HotelEntity>
                {
                    Items = sqlConnection.Query<HotelEntity>(selectSql.ToString(), parameters).ToList(),
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception)
            {
                return new PagedResultDTO<HotelEntity>();
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

        private static string GetHotelSortClause(string? sortBy)
        {
            return sortBy?.Trim().ToLowerInvariant() switch
            {
                "name" => SQLQueries.Hotels_SortByName,
                "city" => SQLQueries.Hotels_SortByCity,
                "price_asc" => SQLQueries.Hotels_SortByPriceAsc,
                "price_desc" => SQLQueries.Hotels_SortByPriceDesc,
                _ => SQLQueries.Hotels_SortDefault
            };
        }
    }
}
