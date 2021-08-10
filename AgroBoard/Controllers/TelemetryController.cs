using System;
using System.Collections;
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
using Newtonsoft.Json;

namespace AgroBoard.Controllers
{
    public class TelemetryController : ApiController
    {
        private AgroboardDataEntities db = new AgroboardDataEntities();

        [HttpGet]
        // GET: api/Telemetry
        public IHttpActionResult GetDeviceName()
        {
            IList<String> nameList = (from device in db.DeviceAndSensor select device.Name).ToList();
            return Ok(nameList);
        }
        [HttpGet]
        public IHttpActionResult GetTelemetrySource(String deviceName)
        {
            try
            {
                IList<Telemetry> telemetrySource = (from telemetry in db.Telemetry.Where(t => t.DeviceName == deviceName) select telemetry).ToList();
                return Ok(telemetrySource);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetTelemetryData(int telemetryId)
        {
            try
            {
                IList<TelemetryData> telemetryData = (from telData in db.TelemetryData.Where(tData => tData.TelemetryId == telemetryId) select telData).ToList();
                TelemetryData teleData = telemetryData.LastOrDefault();
                if (teleData == null)
                    return Ok();
                return Ok(teleData);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpPost]

        public IHttpActionResult PostRule(Models.Rule rule)
        {
            try
            {
                db = new AgroboardDataEntities();
                rule.TelemetryInfo.Replace('\\',' ');
                rule.Success.Replace('\\', ' ');
                rule.Alternate.Replace('\\', ' ');
                rule = new Models.Rule();
                db.Rule.Add(rule);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }

        }

        [HttpGet]
        public IHttpActionResult GetDeviceStatus(string deviceName)
        {
            LastTelemetry telemetry = null;
            try
            {
                DeviceAndSensor device = (from dev in db.DeviceAndSensor.Where(devInst => devInst.Name == deviceName) select dev).SingleOrDefault();
                device.Telemetry = (from tel in db.Telemetry.Where(telInstance => telInstance.DeviceName == device.Name) select tel).ToList();
                foreach (Telemetry t in device.Telemetry)
                {
                    t.TelemetryData = (from data in db.TelemetryData.Where(dataInst => dataInst.TelemetryId == t.Id) select data).ToList();
                    telemetry = new LastTelemetry() { Name = device.Name, Variable = t.Variable, Value = (t.TelemetryData.LastOrDefault()).Value };
                }
                return Ok(telemetry);
            }
            catch (Exception ex)
            {
                return Ok(BadRequest(ex.Message));
            }
        }

        // GET: api/Telemetries
        public IQueryable<Telemetry> GetTelemetry()
        {
            return db.Telemetry;
        }

        // GET: api/Telemetries/5
        [ResponseType(typeof(Telemetry))]
        public IHttpActionResult GetTelemetry(int id)
        {
            Telemetry telemetry = db.Telemetry.Find(id);
            if (telemetry == null)
            {
                return NotFound();
            }

            return Ok(telemetry);
        }

        // PUT: api/Telemetries/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTelemetry(int id, Telemetry telemetry)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != telemetry.Id)
            {
                return BadRequest();
            }

            db.Entry(telemetry).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TelemetryExists(id))
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

        // POST: api/Telemetries
        [ResponseType(typeof(Telemetry))]
        public IHttpActionResult PostTelemetry(Telemetry telemetry)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Telemetry.Add(telemetry);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = telemetry.Id }, telemetry);
        }

        // DELETE: api/Telemetries/5
        [ResponseType(typeof(Telemetry))]
        public IHttpActionResult DeleteTelemetry(int id)
        {
            Telemetry telemetry = db.Telemetry.Find(id);
            if (telemetry == null)
            {
                return NotFound();
            }

            db.Telemetry.Remove(telemetry);
            db.SaveChanges();

            return Ok(telemetry);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TelemetryExists(int id)
        {
            return db.Telemetry.Count(e => e.Id == id) > 0;
        }

        [HttpPost]
        public IHttpActionResult PostData(LastTelemetry lastTelemetry)
        {
            try
            {
                List<Telemetry> telInfoList = (from tInfo in db.Telemetry.Where(tInfoInstance => tInfoInstance.DeviceName == lastTelemetry.Name) select tInfo).ToList();
                Telemetry singleTelemetryInstance = (from tinst in telInfoList.Where(telInfoInst => telInfoInst.Variable == lastTelemetry.Variable) select tinst).SingleOrDefault();
                TelemetryData telemetryData = new TelemetryData();
                telemetryData.Value = lastTelemetry.Value;
                telemetryData.Telemetry = singleTelemetryInstance;
                db.TelemetryData.Add(telemetryData);
                db.SaveChanges();
                //////////// FindLatestTelemetry()/////////////////////
                /// 
                db = new AgroboardDataEntities();
                List<String> responseTrigers = new List<string>();
                String response = "";

                foreach (Models.Rule rule in db.Rule)
                {
                    var telemetryInfo = rule.TelemetryInfo; //"{\"temperature\":20,\"humidity\":40,\"LightIntensity\":33,\"status\":false}";
                    string expressionString = rule.QueryString;
                    dynamic telemetryStringData = JsonConvert.DeserializeObject(telemetryInfo);
                    ArrayList telemetryList = new ArrayList();
                    foreach (var d in telemetryStringData)
                    {
                        //get variable Id from telemetryStringData
                        int variableId = (int)d.Id;

                        //get variables from database on basis of variable id 
                        Telemetry telemetry = (from tel in db.Telemetry.Where(t => t.Id == variableId) select tel).FirstOrDefault();
                        if (telemetry != null)
                        {
                            //get telemetry data from database on basis of variable id
                            var telemetryDatas = (from telData in db.TelemetryData.Where(tel => tel.TelemetryId == telemetry.Id) select telData);
                            List<TelemetryData> telemetryDataList = telemetryDatas.ToList();
                            TelemetryData telemetryDataInstance = (telemetryDataList.OrderBy(tData => tData.Id)).LastOrDefault();
                            telemetryList.Add(new String[] { telemetry.Variable, telemetryDataInstance.Value });
                        }
                    }

                    string jsonExpression = "{";
                    foreach (String[] d in telemetryList)
                    {
                        jsonExpression += d[0] + ":" + d[1] + ",";
                    }
                    jsonExpression = jsonExpression.Remove(jsonExpression.Length - 1, 1);
                    jsonExpression += "}";
                    Parser parser = new Parser();
                    bool result = parser.Execute(jsonExpression, expressionString);
                    if (result)
                    {
                        db = new AgroboardDataEntities();
                        //update success trigger in database
                        if (rule.Success != null)
                        {
                            var successInfo = rule.Success;
                            dynamic successString = JsonConvert.DeserializeObject(successInfo);
                            foreach (var data in successString)
                            {
                                string deviceName = data.deviceName;
                                DeviceAndSensor devAndSensor = (from ds in db.DeviceAndSensor.Where(devInst => devInst.Name == deviceName) select ds).SingleOrDefault();
                                string variableName = data.Telemetry;
                                Telemetry telemetry = (from tel in db.Telemetry.Where(telInstance => telInstance.Variable == variableName) select tel).SingleOrDefault();
                                string latestVariableValue = data.Value;
                                var telemetryDataInst = (from tdataInst in db.TelemetryData.Where(telDataInst => telDataInst.TelemetryId == telemetry.Id) select tdataInst).FirstOrDefault();
                                telemetryDataInst.Value = latestVariableValue;
                                db.TelemetryData.Add(telemetryDataInst);
                                db.Entry(telemetryDataInst).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        db = new AgroboardDataEntities();
                        if (rule.Alternate != null)
                        {
                            var alternateInfo = rule.Alternate;
                            dynamic alternateString = JsonConvert.DeserializeObject(alternateInfo);
                            foreach (var data in alternateString)
                            {
                                string deviceName = data.deviceName;
                                DeviceAndSensor devAndSensor = (from ds in db.DeviceAndSensor.Where(devInst => devInst.Name == deviceName) select ds).SingleOrDefault();
                                string variableName = data.Telemetry;
                                Telemetry telemetry = (from tel in db.Telemetry.Where(telInstance => telInstance.Variable == variableName) select tel).SingleOrDefault();
                                string latestVariableValue = data.Value;
                                var telemetryDataInst = (from tdataInst in db.TelemetryData.Where(telDataInst => telDataInst.TelemetryId == telemetry.Id) select tdataInst).FirstOrDefault();
                                telemetryDataInst.Value = latestVariableValue;
                                db.TelemetryData.Add(telemetryDataInst);
                                db.Entry(telemetryDataInst).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        public class LastTelemetry
        {
            public string Name { get; set; }
            public string Variable { get; set; }
            public string Value { get; set; }
        }
    }
}