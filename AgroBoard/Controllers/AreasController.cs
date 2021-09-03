﻿using System;
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
    public class AreasController : ApiController
    {
        private AgroboardDataEntities db = new AgroboardDataEntities();

        // GET: api/Areas/GetArea
        public IQueryable<Area> GetArea()
        {
            return db.Area;
        }

        // GET: api/Areas/GetArea/5
        [ResponseType(typeof(Area))]
        public IHttpActionResult GetArea(int id)
        {
            Area area = db.Area.Find(id);
            if (area == null)
            {
                return NotFound();
            }

            return Ok(area);
        }

        // GET: api/Areas/GetAreaInfo/5
        [ResponseType(typeof(object))]
        public IHttpActionResult GetAreaInfo(int id)
        {

            try
            {

                Area selectedArea = (from area in db.Area.Where(a => a.Id == id) select area).SingleOrDefault();
                selectedArea.DeviceAndSensor = (from devSen in db.DeviceAndSensor.Where(d => d.aId == selectedArea.Id) select devSen).ToList();
                if (selectedArea.DeviceAndSensor != null && selectedArea.DeviceAndSensor.Count > 0)
                {
                    foreach (DeviceAndSensor devAndSens in selectedArea.DeviceAndSensor)
                    {
                        devAndSens.Telemetry = (from tel in db.Telemetry.Where(t => t.DeviceName == devAndSens.Name) select tel).ToList();
                        if (devAndSens.Telemetry != null && devAndSens.Telemetry.Count > 0)
                        {
                            foreach (Telemetry telemetry in devAndSens.Telemetry)
                            {
                                telemetry.TelemetryData = (from telData in db.TelemetryData.Where(tData => tData.TelemetryId == telemetry.Id) select telData).ToList();
                            }
                        }
                    }
                }
                return Ok(selectedArea);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT: api/Areas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutArea(int id, Area area)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != area.Id)
            {
                return BadRequest();
            }

            db.Entry(area).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(id))
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

        // POST: api/Areas/PostArea
        [ResponseType(typeof(Area))]
        public IHttpActionResult PostArea(Area area)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Area.Add(area);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AreaExists(area.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = area.Id }, area);
        }

        // DELETE: api/Areas/5
        [ResponseType(typeof(Area))]
        public IHttpActionResult DeleteArea(int id)
        {
            Area area = db.Area.Find(id);
            if (area == null)
            {
                return NotFound();
            }

            db.Area.Remove(area);
            db.SaveChanges();

            return Ok(area);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AreaExists(int id)
        {
            return db.Area.Count(e => e.Id == id) > 0;
        }

        public IHttpActionResult GetAreaJoin(int areaId)

        {
            List<AreaInfo> areaInfoList = (from a in db.Area
                                           join
                                           ds in db.DeviceAndSensor on a.Id equals ds.aId
                                           join
                                           tele in db.Telemetry on ds.Name equals tele.DeviceName
                                           //join 
                                           // teleData in db.TelemetryData on tele.Id equals teleData.TelemetryId
                                           where a.Id == areaId
                                           select new AreaInfo
                                           {
                                               AreaId = a.Id,
                                               AreaName = a.name,
                                               dsName = ds.Name,
                                               Protocol = ds.Protocol,
                                               Variable = tele.Variable,
                                               TelemetryId = tele.Id,
                                               //Value = teleData.Value
                                           }).ToList();

            foreach (AreaInfo areaInfoInstance in areaInfoList)
            {
                try
                {
                    List<TelemetryData> telDataList = (from telemetryDataList in db.TelemetryData.Where(t => t.TelemetryId == areaInfoInstance.TelemetryId) select telemetryDataList).ToList();//.LastOrDefault());//.Value;
                    if (telDataList != null && telDataList.Count > 0)
                    {
                        TelemetryData telDataInst = telDataList[telDataList.Count - 1];
                        areaInfoInstance.Value = telDataInst.Value;
                    }
                }

                catch (Exception e)
                {

                    string messsage = e.Message;

                }
            }


            return Ok(areaInfoList);
        }
    }

    public class AreaInfo
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public string dsName { get; set; }
        public string Protocol { get; set; }
        public int TelemetryId { get; set; }
        public string Variable { get; set; }
        public string Value { get; set; }
    }
}