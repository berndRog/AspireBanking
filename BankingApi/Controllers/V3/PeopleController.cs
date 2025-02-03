using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Asp.Versioning;
using AutoMapper;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace BankingApi.Controllers.V2;

[ApiVersion("3.0")]
[Route("banking/v{version:apiVersion}")]

[ApiController]
public class PeopleController(
   IPeopleRepository repository,
   IDataContext dataContext,
   IMapper mapper,
   ILogger<PeopleController> logger
) : ControllerBase {

 
   #region methods
   /// <summary>
   /// Get all people 
   /// </summary>
   /// <returns>IEnumerable{PersonDto}; </returns>
   /// <response code="200">Ok. List{PersonDto} returned</response>
   [HttpGet("people")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<IEnumerable<PersonDto>>> Get() {
      logger.LogDebug("GetAll()");
      var people = await repository.SelectAsync();
      return Ok(mapper.Map<IEnumerable<PersonDto>>(people));
   }
   
   /// <summary>
   /// Find the person with the given id 
   /// </summary>
   /// <param name="id:Guid">id of the person</param>
   /// <returns>PersonDto?</returns>
   /// <response code="200">Ok: PersonDto with given id returned</response>
   /// <response code="404">NotFound: Person with given id not found</response>
   /// <response code="500">Server internal error.</response>
   [HttpGet("people/{id:Guid}")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<PersonDto>> GetById(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("Get {id}",id);
      Person? person = await repository.FindByIdAsync(id);
      if(person == null) return NotFound("Person with given id not found");
      return Ok(mapper.Map<PersonDto>(person));        
   }
 
   /// <summary>
   /// Insert a Person
   /// </summary>
   /// <param name="personDto"></param>
   /// <returns>PersonDto?</returns>
   /// <response code="201">Created: Person is created</response>
   /// <response code="409">Conflict: Person with given id already exists.</response>
   [HttpPost("people")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<PersonDto>> Post(
      [FromBody] PersonDto personDto
   ) {
      logger.LogDebug("Post");
      var person = mapper.Map<Person>(personDto);
      
      if (await repository.FindByIdAsync(person.Id) != null) 
         return Conflict($"Post: Person with given id already exists");

      // save to repository and write to database 
      repository.Add(person);
      await dataContext.SaveAllChangesAsync();
      
      // https://ochzhen.com/blog/created-createdataction-createdatroute-methods-explained-aspnet-core
      // Request == null in unit tests
      var path = Request == null 
         ? $"/banking/v1/people/{person.Id}" 
         : $"{Request.Path}/{person.Id}";
      var uri = new Uri(path, UriKind.Relative);
      personDto = mapper.Map<PersonDto>(person);
      return Created(uri: uri, value: personDto);
   }

   /// <summary>
   /// Update an person (person with given id must exist).
   /// </summary>
   /// <param name="id:Guid">Given id</param>
   /// <param name="updPersonDto">Person with new properties</param>
   /// <returns>PersonDto?</returns>
   /// <response code="200">Ok: Person with given id updated.</response>
   /// <response code="400">Bad Request: id and Person.Id do not match.</response>
   [HttpPut("people/{id:Guid}")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<PersonDto>> Put(
      [FromRoute] Guid id,
      [FromBody]  PersonDto updPersonDto
   ) {
      logger.LogDebug("Put {id}", id);

      Person updPerson = mapper.Map<Person>(updPersonDto);
      
      Person? person = await repository.FindByIdAsync(updPerson.Id);
      if (person == null)
         return NotFound($"Put: Person with given id not found.");

      // save to repository and write to database 
      await repository.UpdateAsync(updPerson);
      await dataContext.SaveAllChangesAsync();

      return Ok(mapper.Map<PersonDto>(updPerson));
   }

   /// <summary>
   /// Delete person with given id
   /// </summary>
   /// <param name="id"></param>
   /// <returns></returns>
   /// <response code="204">NoContent: Person deleted.</response>
   /// <response code="404">NotFound: Person with given id not found</response>
   [HttpDelete("people/{id:Guid}")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
   [ProducesDefaultResponseType]
   public async Task<IActionResult> Delete(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("Delete {id}",id);

      Person? person = await repository.FindByIdAsync(id);
      if(person == null) return NotFound($"Delete: Person with given id not found");
      repository.Remove(person);
      await dataContext.SaveAllChangesAsync();
      return NoContent();  // 204 = Ok with no content
   }
   #endregion
}