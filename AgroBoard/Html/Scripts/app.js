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
    $scope.device.Telemetry = [];
    $scope.deviceSuccess.TelemetrySuccess = [];
    $scope.deviceAlt.TelemetryAlt = [];
    var telemetryInfo = [];
    var telemetrySuccessInfo = [];
    var telemetryAltInfo = [];
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
    $scope.panels = [];
    $scope.addNewPanel = function () {
        var newItemNo = $scope.panels.length + 1;
        $scope.panels.push({ 'id': 'panel' + newItemNo, 'name': 'panel' + newItemNo });
    };
    $scope.selectedItemChanged = function (selectedDev) {
        $scope.device.Telemetry = [];
        $http({
            method: 'Get',  //request message will not be visible in url
            url: '/api/telemetry/GetTelemetrySource?deviceName=' + selectedDev.Name,
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            }
        }).then(
            function (response) {  //on success it returns all records
                for (var i = 0; i < response.data.length; i++) {
                    $scope.device.Telemetry.push({ 'Id': response.data[i].Id, 'Name': response.data[i].Variable });
                }

            },
            function (error) { //on fail
                alert(JSON.stringify(error));
            });//close then 

    }
    $scope.selectedSuccessItemChanged = function (selectedDevSuccess) {
        $scope.deviceSuccess.TelemetrySuccess = [];
        $http({
            method: 'Get',  //request message will not be visible in url
            url: '/api/telemetry/GetTelemetrySource?deviceName=' + selectedDevSuccess.Name,
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            }
        }).then(
            function (response) {  //on success it returns all records
                for (var i = 0; i < response.data.length; i++) {
                    $scope.deviceSuccess.TelemetrySuccess.push({ 'Id': response.data[i].Id, 'Name': response.data[i].Variable });
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
                    $scope.deviceAlt.TelemetryAlt.push({ 'Id': response.data[i].Id, 'Name': response.data[i].Variable });
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
        /////////////////////////////////////
        $http({
            method: 'Get',  //request message will not be visible in url
            url: '/api/Telemetry/GetTelemetryData?telemetryId=' + $scope.selectedTel.Id,
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            }
        }).then(
            function (response) {  //on success it returns all records
                if (response.data.Value === null)
                    telemetryInfo.push({ 'deviceName': $scope.selectedDev.Name, 'Telemetry': $scope.selectedTel.Name, 'Value': "" });
                telemetryInfo.push({ 'deviceName': $scope.selectedDev.Name, 'Telemetry': $scope.selectedTel.Name, 'Value': response.data.Value });
                $scope.telemetryInfo = telemetryInfo;
            },
            function (error) { //on fail
                alert(error.status);
            });//close then 
        /////////////////////////////////////
    }
    $scope.addSuccessTelemetryInfo = function () {
        /////////////////////////////////////
        $http({
            method: 'Get',  //request message will not be visible in url
            url: '/api/Telemetry/GetTelemetryData?telemetryId=' + $scope.selectedTelSuccess.Id,
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            }
        }).then(
            function (response) {  //on success it returns all records
                telemetrySuccessInfo.push({ 'deviceName': $scope.selectedDevSuccess.Name, 'Telemetry': $scope.selectedTelSuccess.Name, 'Value': $scope.telSuccessData });
                $scope.telemetrySuccessInfo = telemetrySuccessInfo;
            },
            function (error) { //on fail
                alert(error.status);
            });//close then 
        /////////////////////////////////////
    }
    $scope.addAltTelemetryInfo = function () {
        /////////////////////////////////////
        $http({
            method: 'Get',  //request message will not be visible in url
            url: '/api/Telemetry/GetTelemetryData?telemetryId=' + $scope.selectedTelAlt.Id,
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            }
        }).then(
            function (response) {  //on success it returns all records
                telemetryAltInfo.push({ 'deviceName': $scope.selectedDevAlt.Name, 'Telemetry': $scope.selectedTelAlt.Name, 'Value': $scope.telAltData });
                $scope.telemetryAltInfo = telemetryAltInfo;
            },
            function (error) { //on fail
                alert(error.status);
            });//close then 
        /////////////////////////////////////
    }
    $scope.addRule = function () {
        var Rule = {
            'QueryString': $scope.QueryString,
            'Success': JSON.stringify($scope.telemetrySuccessInfo),
            'Alternate': JSON.stringify($scope.telemetryAltInfo),
            'TelemetryInfo': JSON.stringify($scope.device.Telemetry)
        };

        alert(JSON.stringify(Rule));
        $http({
            method: 'POST',  //request message will not be visible in url
            data: JSON.stringify(Rule),
            url: '/api/Telemetry/PostRule',
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            }
        }).then(
            function (response) {  //on success it returns all records
                alert(JSON.stringify(response));
            },
            function (error) { //on fail
                alert(error.status);
            });//close then 
    }
});
