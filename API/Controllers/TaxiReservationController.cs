using DAL.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("taxi-reservations")]
public class TaxiReservationController(ITaxiReservationDAO taxiReservationDAO) : ControllerBase
{
    private readonly ITaxiReservationDAO _taxiReservationDAO = taxiReservationDAO;

    [HttpGet]
    public ActionResult<IEnumerable<ITaxiReservationDAO>> GetTaxiReservations()
    {
        return Ok(_taxiReservationDAO.SelectAllWithUserName());
    }

    [HttpGet("user/{UserId}")]
    public ActionResult<IEnumerable<ITaxiReservationDAO>> GetTaxiReservationsByUserId(Guid UserId)
    {
        var userReservations = _taxiReservationDAO.SelectByUserIdWithUserName(UserId);
        if (userReservations == null)
        {
            return NotFound();
        }
        return Ok(userReservations);
    }
    
    [HttpGet("{id}")]
    public ActionResult<ITaxiReservationDAO> GetTaxiReservationById(Guid id)
    {
        var reservation = _taxiReservationDAO.SelectById(id);
        if (reservation == null)
        {
            return NotFound();
        }
        return Ok(reservation);
    }

    [HttpPost]
    public ActionResult<TaxiReservationCreationDTO> CreateTaxiReservation([FromBody] TaxiReservationCreationDTO taxiReservation)
    { 
        var createdTaxiReservation = _taxiReservationDAO.InsertSimple(taxiReservation);
        if (createdTaxiReservation == null)
        {
            return BadRequest("Failed to create taxi reservation. Please check the provided data.");
        }
        return Ok(createdTaxiReservation);
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateSimple(Guid id, [FromBody] TaxiReservationUpdateDTO taxiReservation)
    {
        if (id != taxiReservation.Id)
        {
            return BadRequest("ID mismatch");
        }
        

        var updated = _taxiReservationDAO.UpdateSimple(taxiReservation);
        if (!updated)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTaxiReservation(Guid id)
    {
        var deleted = _taxiReservationDAO.Delete(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
