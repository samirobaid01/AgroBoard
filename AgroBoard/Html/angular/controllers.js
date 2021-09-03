var ngApp = angular.module('myApp', [])
ngApp.controller('myGetIdController', ['$scope', '$http', function ($scope, $http) {
    var str = "";
    $(document).ready(function () {
        $http({
            method: 'GET',
            url: "/api/Areas/GetArea",
        }).then(function successCallback(response) {
            $scope.areas = response.data;
        }, function errorCallback(response) {
            alert("NOTHING FOUND");
        });
        // Get value on button click
        $("#myBtn").click(function () {
            $scope.areas = [];
            str = $("#myInput").val();
            var getQueryString = "/api/Areas/GetArea/" + str;
            $http({
                method: 'GET',
                url: getQueryString,
            }).then(function successCallback(response) {
                $scope.areas.push(response.data);
                console.log("records fetched")
            }, function errorCallback(response) {
                alert("NOTHING FOUND");
            });
        });
    });
    $scope.get = function (area) {
        localStorage.setItem("areaID", area.Id);
        alert(localStorage.getItem("areaID"));
        window.location = "Area.html";
    }
}]);

ngApp.controller('AreapostController', function ($scope, $http) {
    $scope.Id = null;
    $scope.name = null;
    $scope.description = null;

    $scope.postData = function (Id, name, description) {
        //creating object
        var data = {
            Id: Id,
            name: name,
            description: description
        }
        //call http service
        $http.post("/api/Areas/PostArea", JSON.stringify(data))
            .then(function (response) {

                alert("Added Successfully");
            }, function errorCallback(error) {
                alert(JSON.stringify(error));
            });
    }
});

ngApp.controller('deviceAndSensorPostController', function ($scope, $http) {
    $scope.name = null;
    $scope.description = null;
    $scope.protocol = null;
    $scope.Aid = null;
    $scope.postSensorTelemetry = function (name, description, protocol, Aid, telemetryDataObj) {
        var result = telemetryDataObj;
        var telemetryArray = [];
        $.each($.parseJSON(result), function (k, v) {
            var Telemetry = {
                variable: k,
                datatype: v,
                deviceName: name
            };
            telemetryArray.push(Telemetry);
        });
        //creating object
        var data = {
            name: name,
            description: description,
            protocol: protocol,
            Aid: Aid,
            Telemetry: telemetryArray
        }
        //call http service
        $http.post("/api/DeviceAndSensors/PostDeviceAndSensor", JSON.stringify(data))
            .then(function (response) {
                alert("Added Successfully");
            }, function errorCallback(response) {
                alert("NOTHING Added");
            });
    }
    $scope.postDeviceTelemetrySchema = function (name, description, protocol, Aid, telemetryDataObj) {
        var result = telemetryDataObj;
        var telemetryArray = [];
        $.each($.parseJSON(result), function (k, v) {
            var Telemetry = {
                name: v.name,
                IsAvtive: v.IsActive,
                deviceName: name
            };
            telemetryArray.push(Telemetry);
        });
        //creating object
        var data = {
            name: name,
            description: description,
            protocol: protocol,
            Aid: Aid,
            status: telemetryArray
        }
        JSON.stringify(telemetryArray);
        //call http service
        $http.post("/api/DeviceAndSensors/PostDeviceAndSensor", data)
            .then(function (response) {
                alert("Added Successfully");
            }, function errorCallback(response) {
                alert("NOTHING Added");
            });
    }
});

ngApp.controller('myGetAreaInfoController', ['$scope', '$interval', '$http', function ($scope, $interval, $http) {
    $interval(function () {
        $(document).ready(function () {
            $http({
                method: 'GET',
                url: "/api/Areas/GetAreaJoin?areaId=" + localStorage.getItem("areaID"),
            }).then(function successCallback(response) {
                $scope.info = response.data;
                //alert(JSON.stringify($scope.info));
            }, function errorCallback(response) {
                //alert("NOTHING FOUND");
            });

        });
    }, 1000);
}]);
ngApp.controller('myGetDeviceAndSensorController', ['$scope', '$http', function ($scope, $http) {
    $(document).ready(function () {
        $http({
            method: 'GET',
            url: "/api/DeviceAndSensors/GetDeviceAndSensor",
        }).then(function successCallback(response) {
            $scope.deviceAndSensor = response.data;
        }, function errorCallback(response) {
            alert("NOTHING FOUND");
        });

    });
}]);

ngApp.controller('myGetTelemetryDataController', ['$scope', '$http', '$interval', function ($scope, $http, $interval) {
    $interval(function () {
        $(document).ready(function () {
            $http({
                method: 'GET',
                url: "/api/TelemetryDatas/GetTelemetryData",
            }).then(function successCallback(response) {
                $scope.teleData = response.data;
            }, function errorCallback(response) {
                alert("Error ");
            });

        });
    }, 1000);
}]);