using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IHotelDAO : IBaseDAO<HotelEntity>
    {
        public IEnumerable<HotelEntity> SelectAll();
        public HotelEntity? SelectById(Guid id);
        public Guid Insert(HotelEntity hotel);
        public bool Update(HotelEntity hotel);
        public bool Delete(Guid id);
    }
}
