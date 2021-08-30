using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AgroBoard.Models;

namespace AgroBoard.Controllers
{
    public class DeviceAndSensorsController : ApiController
    {
        private AgroboardDataEntities db = new AgroboardDataEntities();

        // GET: api/DeviceAndSensors
        public IQueryable<DeviceAndSensor> GetDeviceAndSensor()
        {
            return db.DeviceAndSensor;
        }

        // GET: api/DeviceAndSensors/5
        [ResponseType(typeof(DeviceAndSensor))]
        public IHttpActionResult GetDeviceAndSensor(string id)
        {
            DeviceAndSensor deviceAndSensor = db.DeviceAndSensor.Find(id);
            if (deviceAndSensor == null)
            {
                return NotFound();
            }

            return Ok(deviceAndSensor);
        }

        // PUT: api/DeviceAndSensors/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDeviceAndSensor(string id, DeviceAndSensor deviceAndSensor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != deviceAndSensor.Name)
            {
                return BadRequest();
            }

            db.Entry(deviceAndSensor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceAndSensorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
        // POST: api/DeviceAndSensors
        // IsActive will be null if it is declared as nullable in model class
        [ResponseType(typeof(DeviceAndSensor))]
        public IHttpActionResult PostDeviceAndSensor(DeviceAndSensor deviceAndSensor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DeviceAndSensor.Add(deviceAndSensor);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DeviceAndSensorExists(deviceAndSensor.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = deviceAndSensor.Name }, deviceAndSensor);
        }

        // DELETE: api/DeviceAndSensors/5
        [ResponseType(typeof(DeviceAndSensor))]
        public IHttpActionResult DeleteDeviceAndSensor(string id)
        {
            DeviceAndSensor deviceAndSensor = db.DeviceAndSensor.Find(id);
            if (deviceAndSensor == null)
            {
                return NotFound();
            }

            db.DeviceAndSensor.Remove(deviceAndSensor);
            db.SaveChanges();

            return Ok(deviceAndSensor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DeviceAndSensorExists(string id)
        {
            return db.DeviceAndSensor.Count(e => e.Name == id) > 0;
        }
    }
}