using Microsoft.AspNetCore.Mvc;
using BusinessLocationsWebAPI.Models;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BusinessLocationsWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessLocationsController : ControllerBase
    {
        private IConfiguration _config;

        public BusinessLocationsController(IConfiguration config)
        {
            _config = config;
        }
        //this file path we have to get it from configuration 
        private readonly string _csvFilePath = "C:/src/code/BusinessLocationsWebAPI/Static Files/Business Locations Data.csv";
        //string _csvFilePath = _config.GetValue<string>("CustomSettings:CsvFilePath"); // ?? "DefaultValue";
        private static string GetFilePath(object config)
        {
            throw new NotImplementedException();
        }
                       
        [HttpGet]
        public IEnumerable<BusinessLocations> GetAllBusinesses()
        {
            try
            {
                //the business open time / start time and close time / end time to be get it from UI based on the selection
                //var startTime = _config.GetValue<string>("CustomSettings:OpeningTime");// ?? "DefaultValue"; //"10:00 AM";
                //var endTime = _config.GetValue <string>("CustomSettings:ClosingTime");//?? "DefaultValue"; //"01:00 PM";
                string startTime = "10:00 AM";
                string endTime = "01:00 PM";
                var businesses = ReadBusinessesFromCSV();
                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    var results = FindBusinessesBetweenTimings(businesses, startTime, endTime);
                    if (results == null)
                        return (IEnumerable<BusinessLocations>)StatusCode(404, $"Business not found");
                    return (IEnumerable<BusinessLocations>)(results);
                }
                else
                    return (IEnumerable<BusinessLocations>)(businesses);
            }
            catch (IOException ex)
            {
                return (IEnumerable<BusinessLocations>)StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // GET: api/<BusinessLocationsController>
        //[HttpGet("{startTime,endTime}")]
        [HttpGet("GetAllBusinessesBasedOpenCloseTimings")]
        public IActionResult<IEnumerable<BusinessLocations>> GetAllBusinessesBasedOpenCloseTimings(string startTime, string endTime) 
        {
            try
            {
                var businesses = ReadBusinessesFromCSV();
                var results = FindBusinessesBetweenTimings(businesses, startTime, endTime);
                if (results == null)
                    return (IActionResult<IEnumerable<BusinessLocations>>)StatusCode(404, $"Business not found");
                return (IActionResult<IEnumerable<BusinessLocations>>)results.ToList();
            }
            catch (IOException ex)
            {                
                return (IActionResult<IEnumerable<BusinessLocations>>)StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        static IEnumerable<BusinessLocations> FindBusinessesBetweenTimings(List<BusinessLocations> businesses, string startTime, string endTime)
        {
            return businesses.Where(business =>
                string.Compare(business.OpeningTime, startTime, StringComparison.OrdinalIgnoreCase) >= 0 &&
                string.Compare(business.ClosingTime, endTime, StringComparison.OrdinalIgnoreCase) <= 0);
        }

        // GET api/<BusinessLocationsController>/5
        [HttpGet("{businessName}")]
        public ActionResult<BusinessLocations> GetBusinessByName(string bunessName)
        {
            try
            {
                var businesses = ReadBusinessesFromCSV();
                var business = businesses.FirstOrDefault(b => b.Name.Equals(bunessName, StringComparison.OrdinalIgnoreCase));
                if (business == null)
                    return (BusinessLocations)StatusCode(404, $"Business not found");
                return (BusinessLocations)(business);
            }
            catch (IOException ex)
            {
                return (BusinessLocations)StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST api/<BusinessLocationsController>
        [HttpPost]
        public IActionResult AddBusiness(BusinessLocations business)
        {
            try
            {
                var businesses = ReadBusinessesFromCSV();
                businesses.Add(business);
                WriteBusinessesToCSV(businesses);
                var updatedBusiness = businesses.FirstOrDefault(b => b.Name.Equals(business.Name, StringComparison.OrdinalIgnoreCase));
                if (updatedBusiness == null)
                    return NotFound("Business not updated. Please try it again.");
                return Ok("Business updated successfully.");
            }
            catch (IOException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private object GetBusinessByName()
        {
            throw new NotImplementedException();
        }

        // PUT api/<BusinessLocationsController>/5
        [HttpPut("{businessName}")]
        public IActionResult UpdateBusiness(string businessName, BusinessLocations updatedBusiness)
        {
            try
            {
                var businesses = ReadBusinessesFromCSV();
                var existingBusiness = businesses.FirstOrDefault(b => b.Name.Equals(businessName, StringComparison.OrdinalIgnoreCase));
                if (existingBusiness == null)
                    return StatusCode(404, $"Business not found");

                existingBusiness.Location = updatedBusiness.Location;
                existingBusiness.OpeningTime = updatedBusiness.OpeningTime;
                existingBusiness.ClosingTime = updatedBusiness.ClosingTime;

                WriteBusinessesToCSV(businesses);
                return Ok("Business location updated successfully.");
            }
            catch (IOException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE api/<BusinessLocationsController>/5
        [HttpDelete("{bunessName}")]
        public IActionResult DeleteBusiness(string bunessName)
        {
            try
            {
                var businesses = ReadBusinessesFromCSV();
                var existingBusiness = businesses.FirstOrDefault(b => b.Name.Equals(bunessName, StringComparison.OrdinalIgnoreCase));
                if (existingBusiness == null)
                    return StatusCode(404, $"Business not found");

                businesses.Remove(existingBusiness);
                WriteBusinessesToCSV(businesses);
                return Ok("Business location deleted successfully.");
            }
            catch (IOException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private List<BusinessLocations> ReadBusinessesFromCSV()
        {
            //string _csvFilePath = _config.GetValue<string>("CustomSettings:CsvFilePath");// ?? "DefaultValue";
            var businesses = new List<BusinessLocations>();
            using (var reader = new StreamReader(_csvFilePath))
            {
                string line;
                // Skip the header row
                reader.ReadLine();

                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');
                    businesses.Add(new BusinessLocations
                    {
                        Name = values[0],
                        Location = values[1],
                        OpeningTime = values[2],
                        ClosingTime = values[3]
                    });
                }
            }
            return businesses;
        }

        private void WriteBusinessesToCSV(List<BusinessLocations> businesses)
        {
            using (var writer = new StreamWriter(_csvFilePath))
            {
                writer.WriteLine("Business Name,Location,Opening Time,Closing Time");
                foreach (var business in businesses)
                {
                    writer.WriteLine($"{business.Name},{business.Location},{business.OpeningTime},{business.ClosingTime}");
                }
            }
        }
       
    }
}
