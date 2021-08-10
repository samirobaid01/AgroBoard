//Get Area by id controller
var ngApp = angular.module('myApp', [])
ngApp.controller('myGetIdController', ['$scope', '$http', '$window', function ($scope, $http, $window) {
    var str = "";
    $(document).ready(function () {
        // Get value on button click and show alert
        $("#myBtn").click(function () {
            str = $("#myInput").val();
            var getQueryString = "/api/Areas/GetArea/" + str;
            $http({
                method: 'GET',
                url: getQueryString,
            }).then(function successCallback(response) {
                $scope.areas = response.data;
                console.log("records fetched")
            }, function errorCallback(response) {
                alert("NOTHING FOUND");
            });
        });
        $scope.AreaDetails = function (id) {
            localStorage.setItem("areaId", id);
            $window.location.href = "Area.html";
        };
    });
}]);
ngApp.controller('myGetJoinController', ['$scope', '$http', function ($scope, $http) {
   var Id = localStorage.getItem("areaId");
    var getQueryString = "/api/Areas/GetAreaInfo/" + Id;
    $http({
        method: 'GET',
        url: getQueryString,
    }).then(function successCallback(response) {
        $scope.res = response.data;
        alert(JSON.stringify(response.data));
        console.log("records fetched")
    }, function errorCallback(response) {
        alert("NOTHING FOUND");
    });
}]);
// Define area controller 
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
        alert(JSON.stringify(data));
        $http.post("/api/Areas/PostArea", JSON.stringify(data))
            .then(function (response) {

                alert("Added Successfully");
            }, function errorCallback(response) {
                alert(JSON.stringify(response));
            });
    }
});

//Define deviceAndSensor controller
ngApp.controller('deviceAndSensorPostController', function ($scope, $http) {
    $scope.name = null;
    $scope.description = null;
    $scope.protocol = null;
    $scope.Aid = null;

    $scope.postData = function (name, description, protocol, Aid) {
        //creating object
        var data = {

            name: name,
            description: description,
            protocol: protocol,
            Aid: Aid
        }
        //call http service
        $http.post("/api/DeviceAndSensors", JSON.stringify(data))
            .then(function (response) {

                alert("Added Successfully");
            }, function errorCallback(response) {
                alert("NOTHING Added");
            });
    }
});

/*ngApp.controller('postController', function ($scope, Area) {

    $scope.postData = function () {
        Area.addAreaToDB($scope.area);
    }

})
    .factory("Area", ['$http', function ($http) {

        var fac = {};
        fac.addAreaToDB = function (area) {
            $http.post("/api/Areas", area).success(function (response) {
                alert(response.status);
            })
        }
        return fac;
    }])






/*ngApp.controller('postController', function ($scope, $http) {


    $scope.postData = function () {
        //creating object
        var buttonclick = document.getElementById("addBtn").getAttribute("value");
        if (buttonclick == "Add") {
            $scope.areas = {};
            $scope.areas._Id = $scope._Id;
            $scope.areas.name = $scope.name;
            $scope.areas.description = $scope.description;

            $http(
                {
                    method: 'POST',
                    url: '/api/Areas',
                   
                    data: JSON.stringify($scope.areas)
                }).then(function (result) {
                    alert(result.data);
                    $scope._Id = "";
                    $scope.name = "";
                    $scope.description = "";

                });



        }
        /*alert("post function called")
        var data = {
            _Id: _Id,
            name: name,
            description: description
        }
        //call http service
        $http.post("/api/areas", data)
            .then(function (response) {
                alert(response.data)

            })
    }
})

	
	
       /*   ngApp.controller('myGetController',['$scope','$http', function($scope, $http){
    
                
                $http({
            method: 'GET',
            url: 'https://localhost:44389/api/products'
            }).then(function successCallback(response) {
                console.log(response.data)
                $scope.products=response.data
                console.log("records fetched")
            }, function errorCallback(response) {
                alert("NOTHING FOUND")
            });
            
                            // Simple GET request example:
            
            
        }])

            ngApp.controller('postCont', function($scope,$http){
               
            
                $scope.postData = function(title, company, category, price, description){
                    //creating object
                    alert("post function called")
                    var data = {
                        title:title,
                        company:company,
                        category:category,
                        price:price,
                        description:description
                    }
                                        //call http service
                    $http.post("https://localhost:44389/api/products",data)
                    .then(function(response){
                        alert(response.data)
                        
                    })
                }
            })

            ngApp.controller('delCont', function($scope,$http){
               
            
                $scope.delData = function(title){
                    //creating object
                    alert("delete function called")

                    $http.delete("https://localhost:44389/api/products"+title)
                    .then(function(response){
                        alert(response.data)
                        
                    })
                }
            })*/
            