using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLocationsWebAPI.Models
{
    public class BusinessLocations
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }

        public static explicit operator BusinessLocations(ObjectResult v)
        {
            throw new NotImplementedException();
        }

        public static explicit operator BusinessLocations(List<BusinessLocations> v)
        {
            throw new NotImplementedException();
        }
    }


public static class BusinessLocationsEndpoints
{
	public static void MapBusinessLocationsEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/BusinessLocations").WithTags(nameof(BusinessLocations));

        group.MapGet("/", () =>
        {
            return new [] { new BusinessLocations() };
        })
        .WithName("GetAllBusinessLocations")
        .WithOpenApi();

        group.MapGet("/{id}", (int id) =>
        {
            //return new BusinessLocations { ID = id };
        })
        .WithName("GetBusinessLocationsById")
        .WithOpenApi();

        group.MapPut("/{id}", (int id, BusinessLocations input) =>
        {
            return TypedResults.NoContent();
        })
        .WithName("UpdateBusinessLocations")
        .WithOpenApi();

        group.MapPost("/", (BusinessLocations model) =>
        {
            //return TypedResults.Created($"/api/BusinessLocations/{model.ID}", model);
        })
        .WithName("CreateBusinessLocations")
        .WithOpenApi();

        group.MapDelete("/{id}", (int id) =>
        {
            //return TypedResults.Ok(new BusinessLocations { ID = id });
        })
        .WithName("DeleteBusinessLocations")
        .WithOpenApi();
    }
}}
