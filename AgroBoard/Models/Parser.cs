using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace AgroBoard.Models
{
    public class Parser
    {
        public bool Execute(String telemetryData, String expressionString)
        {
            var telemetryStringData = JsonConvert.DeserializeObject<Dictionary<string, object>>(telemetryData);
            var expList = new List<ParameterExpression>();
            List<Object> paramList = new List<Object>();
            foreach (KeyValuePair<string, object> d in telemetryStringData)
            {
                paramList.Add(d.Value);
                expList.Add(System.Linq.Expressions.Expression.Parameter(d.Value.GetType(), d.Key));
            }
            ParameterExpression[] pExp = expList.ToArray();
            var parseRes = System.Linq.Dynamic.DynamicExpression.ParseLambda(pExp, null, expressionString);
            Object[] inputParam = paramList.ToArray();
            var result = parseRes.Compile().DynamicInvoke(inputParam);
            return (bool)result;
        }
    }
}