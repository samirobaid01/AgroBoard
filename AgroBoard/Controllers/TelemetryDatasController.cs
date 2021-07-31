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
    public class TelemetryDatasController : ApiController
    {
        private AgroboardDataEntities db = new AgroboardDataEntities();

        // GET: api/TelemetryDatas
        public IQueryable<TelemetryData> GetTelemetryData()
        {
            return db.TelemetryData;
        }

        // GET: api/TelemetryDatas/5
        [ResponseType(typeof(TelemetryData))]
        public IHttpActionResult GetTelemetryData(int id)
        {
            TelemetryData telemetryData = db.TelemetryData.Find(id);
            if (telemetryData == null)
            {
                return NotFound();
            }

            return Ok(telemetryData);
        }

        // PUT: api/TelemetryDatas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTelemetryData(int id, TelemetryData telemetryData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != telemetryData.Id)
            {
                return BadRequest();
            }

            db.Entry(telemetryData).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TelemetryDataExists(id))
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

        // POST: api/TelemetryDatas
        [ResponseType(typeof(TelemetryData))]
        public IHttpActionResult PostTelemetryData(TelemetryData telemetryData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TelemetryData.Add(telemetryData);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = telemetryData.Id }, telemetryData);
        }

        // DELETE: api/TelemetryDatas/5
        [ResponseType(typeof(TelemetryData))]
        public IHttpActionResult DeleteTelemetryData(int id)
        {
            TelemetryData telemetryData = db.TelemetryData.Find(id);
            if (telemetryData == null)
            {
                return NotFound();
            }

            db.TelemetryData.Remove(telemetryData);
            db.SaveChanges();

            return Ok(telemetryData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TelemetryDataExists(int id)
        {
            return db.TelemetryData.Count(e => e.Id == id) > 0;
        }
    }
}