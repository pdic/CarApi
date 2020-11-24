using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Web.Http.Cors;

namespace CarApi.Controllers
{
   
    [ApiController]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CarServiceController : ControllerBase
    {

        // Create appropriate DB Manager. 
        private IDbManager GetDbManager()
        {
            return new SqlDbManager();
        }

        private readonly ILogger<CarServiceController> _logger;

        public CarServiceController(ILogger<CarServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("api/searchcars")]
        public IEnumerable<TCar> SearchCar()
        {
            string term;
            List<TCar> cars = new List<TCar>();

            if(!string.IsNullOrEmpty(Request.Query["brand"]))
            {
                term = Request.Query["brand"][0];
                cars = this.getCars().FindAll(car => car.Brand.Contains(term, StringComparison.InvariantCultureIgnoreCase));
            }
            if(!string.IsNullOrEmpty(Request.Query["model"]))
            {
                term = Request.Query["model"][0];
                cars = this.getCars().FindAll(car => car.Model.Contains(term, StringComparison.InvariantCultureIgnoreCase));
            }
            if(!string.IsNullOrEmpty(Request.Query["color"]))
            {
                term = Request.Query["color"][0];
                cars = this.getCars().FindAll(car => car.Color.Contains(term, StringComparison.InvariantCultureIgnoreCase));
            }
            return cars;
        }

        [HttpGet]
        [Route("api/getcars")]
        public IEnumerable<TCar> Get() => getCars();

        [HttpGet]
        [Route("api/getcars/{id}")]
        public TCar GetCar(int id)
        {
            using(IDbManager db = this.GetDbManager()) {
                return TCar.GetCar(db, id);
            }
        }

        [HttpPut]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/updatecar")]
        public void UpdateCar([FromBody] TCar car)
        {
            using(IDbManager db = this.GetDbManager()) {
                car.Update(db);
            }
        }

        [HttpPost]
        [Route("api/addcar")]
        public TCar AddCar([FromBody] TCar car)
        {
            using(IDbManager db = this.GetDbManager()) {
                car.Add(db);
            }

            return this.GetCar(car.Id);
        }

        [HttpDelete]
        [Route("api/deletecar/{id}")]
        public void DeleteCar(int id)
        {
            using(IDbManager db = this.GetDbManager()) {
                TCar car = new TCar(id, ItemStatus.Deleted);
                car.Delete(db);
            }
            return;
        }

        private List<TCar> getCars() {
            List<TCar> cars;

            using(IDbManager db = this.GetDbManager()) {
                cars = TCar.GetCarList(db);
            }
            return cars;
        }
        
    }
}
