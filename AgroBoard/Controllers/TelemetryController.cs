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
                dynamic telemetrySource = (from dev in db.DeviceAndSensor
                                       join status in db.Status on
                                       dev.Name equals status.DeviceName
                                       where
                dev.Name == deviceName && dev.Type == "device"
                                               select new { status.Name, status.Id }).ToList();
                        if (telemetrySource == null || telemetrySource.Count==0)
                        {
                            telemetrySource = (from dev in db.DeviceAndSensor
                                               join tel in db.Telemetry on
                                               dev.Name equals tel.DeviceName
                                               where
                dev.Name == deviceName && dev.Type == "sensor"
                                       select new { tel.Variable, tel.Id }).ToList();
                }               
                return Ok(telemetrySource);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetTelemetryData(int telemetryId, string deviceName)
        {
            try
            {
                dynamic telemetry = (from dev in db.DeviceAndSensor
                                     join status in db.Status
                                     on dev.Name equals status.DeviceName
                                     where
                                     dev.Name == deviceName && dev.Type == "device" && status.Id == telemetryId
                                     select new TelemetryInstance()
                                     {
                                         DeviceName =dev.Name,
                                         Type= dev.Type,
                                         Name=status.Name,
                                         IsActive=(bool)status.IsActive
                                     }).ToList();

                if (telemetry == null || telemetry.Count == 0)
                {
                    telemetry = ((from tel in db.Telemetry
                                 join telData in db.TelemetryData
                                 on tel.Id equals telData.TelemetryId
                                 where tel.Id == telemetryId
                                 select new TelemetryInstance()
                                 {
                                     DeviceName=deviceName,
                                     Type = "sensor",
                                     SensorTelemetryId = tel.Id,
                                     Variable = tel.Variable,
                                     Value = telData.Value
                                 }).ToList());
                   
                }
                if(telemetry!=null&&telemetry.Count>0)
                {
                    telemetry = telemetry[telemetry.Count-1];
                }
                return Ok(telemetry);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }
        public class TelemetryInstance
        {
            public String DeviceName { get; set; }
            public String Type { get; set; }
            /// <summary>
            /// variable Id in TelemetryData table
            /// </summary>
            public int VariableId { get; set; }
            /// <summary>
            /// variable value in TelemetryData table
            /// </summary>
            public String Value { get; set; }   
            /// <summary>
            /// Id in Telemetry table
            /// </summary>
            public int SensorTelemetryId { get; set; }
            /// <summary>
            /// variable in Telemetry table
            /// </summary>
            public String Variable { get; set; }
            /// <summary>
            /// represents Id in Status table
            /// </summary>
            public int DeviceStatusId { get; set; }
            /// <summary>
            /// represents Name in Status table
            /// </summary>
            public String Name { get; set; }
            /// <summary>
            /// represents IsActive in Status table
            /// </summary>
            public bool IsActive { get; set; }

        }
        [HttpPost]
        public IHttpActionResult GetDeviceStatus(int deviceId)
        {
            try
            {
                IList<Status> deviceStatus = (from devStatus in db.Status.Where(stateInstance => stateInstance.Id == deviceId) select devStatus).ToList();
                Status singleDeviceStatus = deviceStatus.LastOrDefault();
                if (singleDeviceStatus == null)
                    return Ok();
                return Ok(singleDeviceStatus);
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
                db.Rule.Add(rule);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(e.InnerException.Message);
            }

        }

        [HttpGet]
        public IHttpActionResult GetDeviceStatus(string deviceName)
        {
            LastTelemetry telemetry = null;
            try
            {
                //get unique device from database
                /*In future, it will be based on deviceAndSensorID*/
                //DeviceAndSensor device = (from dev in db.DeviceAndSensor.Where(devInst => devInst.Name == deviceName) select dev).SingleOrDefault();
                List<Status> status = (from st in db.Status.Where(statInst => statInst.DeviceName == deviceName) select st).ToList();
                foreach( Status st in status)
                {
                    if (st.IsActive == true)
                    {
                        LastTelemetry telInstance = new LastTelemetry() { Value = st.Name };
                       return Ok(telInstance);
                    }
                }
                return Ok();
                //old code
                //////get all variables that belongs to the selected device
                //device.Telemetry = (from tel in db.Telemetry.Where(telInstance => telInstance.DeviceName == device.Name) select tel).ToList();
                //foreach (Telemetry t in device.Telemetry)
                //{
                //    t.TelemetryData = (from data in db.TelemetryData.Where(dataInst => dataInst.TelemetryId == t.Id) select data).ToList();
                //    telemetry = new LastTelemetry() { Name = device.Name, Variable = t.Variable, Value = (t.TelemetryData.LastOrDefault()).Value };
                //}
                //return Ok(telemetry);
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
                //select telemetry list on basis of device name
                /*In future, will get telemetry list on basis of unique ID of DeviceAndSensor*/
                //List<Telemetry> telInfoList = (from tInfo in db.Telemetry.Where(tInfoInstance => tInfoInstance.DeviceName == lastTelemetry.Name) select tInfo).ToList();
                //Telemetry singleTelemetryInstance = (from tinst in telInfoList.Where(telInfoInst => telInfoInst.Variable == lastTelemetry.Variable) select tinst).SingleOrDefault();
                Telemetry singleTelemetryInstance = (from telInstance in db.Telemetry.Where(telInst => telInst.DeviceName == lastTelemetry.Name
                                                     &&telInst.Variable==lastTelemetry.Variable) select telInstance).SingleOrDefault();

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
                    //select a sensor and device
                    var telemetryInfo = rule.TelemetryInfo;                                     //id and name of a devices and sensors
                    string expressionString = rule.QueryString;                                 //represents LINQ expression
                    dynamic telemetryStringData = JsonConvert.DeserializeObject(telemetryInfo); //deserialize JSON to get ID and Name
                    ArrayList telemetryList = new ArrayList();
                    foreach (var d in telemetryStringData)
                    {
                            //get variable Id from telemetryStringData
                            //int variableId = (int)d.Id;
                            string deviceName = (d.deviceName).ToString();
                            string varName = (d.Telemetry).ToString();
                            var queryResult = (from devAndSens in db.DeviceAndSensor
                                             join telemetryInst in db.Telemetry on
                                               devAndSens.Name equals telemetryInst.DeviceName
                                             where devAndSens.Name == deviceName && telemetryInst.Variable == varName
                                               select new { telemetryInst.Id}).SingleOrDefault();
                        int variableId = Int32.Parse(queryResult.Id.ToString());
                        //get variables from database on basis of variable id 
                        Telemetry telemetry = (from tel in db.Telemetry.Where(t => t.Id == variableId) select tel).FirstOrDefault();
                        if (telemetry != null)
                        {
                            //get telemetry data from database on basis of variable id
                            var telemetryDataCollection = (from telData in db.TelemetryData.Where(tel => tel.TelemetryId == telemetry.Id) select telData);
                            if (telemetryDataCollection != null)
                            {
                                List<TelemetryData> telemetryDataList = telemetryDataCollection.ToList();
                                if (telemetryDataList.Count > 0)
                                {
                                    TelemetryData telemetryDataInstance = (telemetryDataList.OrderBy(tData => tData.Id)).LastOrDefault();
                                    telemetryList.Add(new String[] { telemetry.Variable, telemetryDataInstance.Value });
                                }
                            }
                        }
                    }
                    if (telemetryList.Count > 0)
                    {
                        string jsonExpression = "{";
                        foreach (String[] dt in telemetryList)
                        {
                            jsonExpression += dt[0] + ":" + dt[1] + ",";
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

                                    //get variable name from successString of Rule table
                                    string variableName = data.Telemetry;
                                    if (devAndSensor.Type.Equals("device"))
                                    {
                                        Status status = db.Status.Where(statusInst => statusInst.DeviceName == deviceName && statusInst.Name == variableName).SingleOrDefault();
                                        status.IsActive = data.TelemetryData;
                                        db.Entry(status).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        /*Sensor information are read-only till now*/
                                        //Telemetry telemetryInst = (from tel in db.Telemetry.Where(telInstance => telInstance.Variable == variableName) select tel).SingleOrDefault();
                                        //string latestVariableValue = data.Value;
                                        //var telemetryDataInst = (from tdataInst in db.TelemetryData.Where(telDataInst => telDataInst.TelemetryId == telemetryInst.Id) select tdataInst).FirstOrDefault();
                                        //telemetryDataInst.Value = latestVariableValue;
                                        //db.TelemetryData.Add(telemetryDataInst);
                                        //db.Entry(telemetryDataInst).State = EntityState.Modified;
                                        //db.SaveChanges();
                                    }
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
                                    if (devAndSensor.Type.Equals("device"))
                                    {
                                        Status status = db.Status.Where(statusInst => statusInst.DeviceName == deviceName && statusInst.Name == variableName).SingleOrDefault();
                                        status.IsActive = data.TelemetryData;
                                        db.Entry(status).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        /*sensor data is read-only till now*/
                                        //Telemetry telemetryInst = (from tel in db.Telemetry.Where(telInstance => telInstance.Variable == variableName) select tel).SingleOrDefault();
                                        //string latestVariableValue = data.Value;
                                        //var telemetryDataInst = (from tdataInst in db.TelemetryData.Where(telDataInst => telDataInst.TelemetryId == telemetryInst.Id) select tdataInst).FirstOrDefault();
                                        //telemetryDataInst.Value = latestVariableValue;
                                        //db.TelemetryData.Add(telemetryDataInst);
                                        //db.Entry(telemetryDataInst).State = EntityState.Modified;
                                        //db.SaveChanges();
                                    }
                                }
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