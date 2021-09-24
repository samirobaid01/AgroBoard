/**
 * Custom AngularJS App File
 * for populating and creating
 * form fields dynamically
 **/

var app = angular.module("dynamicFieldsPlugin", []);

app.controller("dynamicFields", function ($scope, $http) {
    //code that executes on page load without call
    $scope.device = [];
    $scope.deviceSuccess = [];
    $scope.deviceAlt = [];
    var Telemetry = [];
    $scope.device.Telemetry = [];
    $scope.telemetryInfo = [];
    $scope.deviceSuccess.TelemetrySuccess = [];
    $scope.deviceAlt.TelemetryAlt = [];
    $scope.msg;
    var telemetrySuccessInfo = [];
    $scope.telemetrySuccessInfo = [];
    var telemetryAltInfo = [];
    $scope.telemetryAltInfo = [];
    $scope.panels = [];
                            /* get all devices on page load */
    $http({
        method: 'Get',  
        url: '/api/telemetry/GetDeviceName',
        headers: {
            'Content-type': 'application/json; charset=UTF-8',
        }
    }).then(
        function (response) {  //on success it returns all records
            for (var i = 0; i < response.data.length; i++) {
                $scope.device.push({ 'Name': response.data[i] });
            }

        },
        function (error) { //on fail
            alert("Error" + error.status);
        });//close then 
    $scope.addNewPanel = function () {
        var newItemNo = $scope.panels.length + 1;
        $scope.panels.push({ 'id': 'panel' + newItemNo, 'name': 'panel' + newItemNo });
    };
            /*Get telemetry and devices on selecting a DeviceAndSensor */
    $scope.selectedItemChanged = function (selectedDev)
    {
        $scope.device.Telemetry = [];
        $http({
            method: 'Get', 
            url: '/api/telemetry/GetTelemetrySource?deviceName=' + selectedDev.Name,
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            }
        }).then(
            function (response) {  //on success it returns all records
               for (var i = 0; i < response.data.length; i++) {
                    if (response.data[i].Name=== undefined ||response.data[i].Name === null) {
                        //Telemetry.push({'DeviceName': selectedDev.Name,'Id': response.data[i].Id, 'Name': response.data[i].Variable});
                        $scope.device.Telemetry.push({ 'Id': response.data[i].Id, 'Name': response.data[i].Variable});
                     }
                    else {
                       // Telemetry.push({ 'DeviceName': selectedDev.Name, 'Id': response.data[i].Id, 'Name': response.data[i].Name});
                        $scope.device.Telemetry.push({ 'Id': response.data[i].Id, 'Name': response.data[i].Name});
                      }
                }

            },
            function (error) { //on fail
                alert(JSON.stringify(error));
            });//close then 

    }
    $scope.selectedSuccessItemChanged = function (selectedDevSuccess) {
        $scope.deviceSuccess.TelemetrySuccess = [];
        $http({
            method: 'Get',  //request message will be visible in url
            url: '/api/telemetry/GetTelemetrySource?deviceName=' + selectedDevSuccess.Name,
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            }
        }).then(
            function (response) {  //on success it returns all records
                for (var i = 0; i < response.data.length; i++) {
                    $scope.deviceSuccess.TelemetrySuccess.push({ 'Id': response.data[i].Id, 'Name': response.data[i].Name });
                }

            },
            function (error) { //on fail
                alert(JSON.stringify(error));
            });//close then 

    }
    $scope.selectedAltItemChanged = function (selectedDevAlt) {
        $scope.deviceAlt.TelemetryAlt = [];
        $http({
            method: 'Get',  //request message will not be visible in url
            url: '/api/telemetry/GetTelemetrySource?deviceName=' + selectedDevAlt.Name,
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            }
        }).then(
            function (response) {  //on success it returns all records
                for (var i = 0; i < response.data.length; i++) {
                    $scope.deviceAlt.TelemetryAlt.push({ 'Id': response.data[i].Id, 'Name': response.data[i].Name });
                }
            },
            function (error) { //on fail
                alert(JSON.stringify(error));
            });//close then 

    }
    $scope.removeSelectedPanel = function (panel) {
        //remove panel on basis of id
        var index = $scope.panels.indexOf(panel)
        $scope.panels.splice(index, 1);
    }
    $scope.addTelemetryInfo = function () {
        var url = '/api/telemetry/gettelemetrydata?telemetryId=' + $scope.selectedTel.Id + '&deviceName=' + $scope.selectedDev.Name;
        $http.get(url).then(
            function (response) {
                if (response.data.Type === 'device') {
                    $scope.telemetryInfo.push({ 'deviceName': response.data.DeviceName, 'Type': response.data.Type, 'Telemetry': response.data.Name, 'TelemetryData': response.data.IsActive });
                    Telemetry.push({ 'deviceName': response.data.DeviceName, 'Type': response.data.Type, 'Telemetry': response.data.Name});
                    
                }
                else {
                    $scope.telemetryInfo.push({ 'deviceName': response.data.DeviceName, 'Type': response.data.Type, 'Telemetry': response.data.Variable, 'TelemetryData': response.data.Value });
                    Telemetry.push({ 'deviceName': response.data.DeviceName, 'Type': response.data.Type, 'Telemetry': response.data.Variable });

                }
            },
            function (error) {
                alert(JSON.stringify(error));
            });
    }
    $scope.addSuccessTelemetryInfo = function () {
        /////////////////////////////////////
        var url = '/api/Telemetry/GetTelemetryData?telemetryId=' + $scope.selectedTelSuccess.Id + '&deviceName=' + $scope.selectedDevSuccess.Name;
        $http.get(url).then(
            function (response) {  //on success it returns all records
                alert(JSON.stringify(response));
                if (response.data.Type === 'device') {
                    telemetrySuccessInfo.push({ 'deviceName': response.data.DeviceName, 'Type': response.data.Type, 'Telemetry': response.data.Name, 'TelemetryData': $scope.telSuccessData });
                    $scope.telemetrySuccessInfo.push({ 'deviceName': response.data.DeviceName, 'Type': response.data.Type, 'Telemetry': response.data.Name, 'TelemetryData': $scope.telSuccessData });
                }
            },
            function (error) { //on fail
                alert(error.status);
            });//close then 
        /////////////////////////////////////
    }
    $scope.addAltTelemetryInfo = function () {
        /////////////////////////////////////
        var url = '/api/Telemetry/GetTelemetryData?telemetryId=' + $scope.selectedTelAlt.Id + '&deviceName=' + $scope.selectedDevAlt.Name;
        $http.get(url).then(
            function (response) {  //on success it returns all records
                if (response.data.Type === 'device') {
                    telemetryAltInfo.push({ 'deviceName': response.data.DeviceName, 'Type': response.data.Type, 'Telemetry': response.data.Name, 'TelemetryData': $scope.telAltData });
                    $scope.telemetryAltInfo.push({ 'deviceName': response.data.DeviceName, 'Type': response.data.Type, 'Telemetry': response.data.Name, 'TelemetryData': $scope.telAltData });
                }
            },
            function (error) { //on fail
                alert(error.status);
            });//close then 
        /////////////////////////////////////
        /////////////////////////////////////
    }
    $scope.addRule = function () {
        var Rule = {
            'QueryString': $scope.QueryString,
            'Success': JSON.stringify(telemetrySuccessInfo),
            'Alternate': JSON.stringify(telemetryAltInfo),
            'TelemetryInfo': JSON.stringify(Telemetry)
        };

        alert(JSON.stringify(Rule.TelemetryInfo));
        $http({
            method: 'POST',  //request message will not be visible in url
            data: JSON.stringify(Rule),
            url: '/api/Telemetry/PostRule',
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            }
        }).then(
            function (response) {  //on success it returns all records
                alert(JSON.stringify("Successfully added"));
            },
            function (error) { //on fail
                alert(error.status);
            });//close then 
    }
});
