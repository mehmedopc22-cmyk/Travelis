using Domain.DTOs;
using Domain.Entities;

namespace WEB.Models
{
    public class AdminHotelAssignmentsViewModel
    {
        public List<HotelEntity> Hotels { get; set; } = [];
        public List<UserEntity> HotelAdmins { get; set; } = [];
        public List<HotelAdminAssignmentDTO> Assignments { get; set; } = [];

        public IEnumerable<HotelAdminAssignmentDTO> AssignmentsFor(Guid hotelId)
        {
            return Assignments.Where(assignment => assignment.HotelId == hotelId);
        }
    }
}
