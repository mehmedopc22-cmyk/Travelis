using Domain.Entities;

namespace DAL.Interfaces
{
    public interface ITaxiCompanyDAO
    {
        IEnumerable<TaxiCompanyEntity> SelectAll();
        TaxiCompanyEntity? SelectById(Guid id);
        TaxiCompanyEntity Insert(TaxiCompanyEntity taxiCompany);
    }
}
