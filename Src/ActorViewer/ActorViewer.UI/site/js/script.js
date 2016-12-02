

var app = angular.module("InventoryServiceApp", [
    "ngRoute", "jsonFormatter"
]);

app.config([
    "$routeProvider", function ($routeProvider) {
        //$routeProvider
        //    // Home
        //    .when("/", {
        //        templateUrl: "partials/home.html" /*,controller: ""*/
        //    })
        //    // else 404
        //    .otherwise("/404", { templateUrl: "partials/404.html", controller: "PageCtrl" });
    }
]);
angular.module("InventoryServiceApp").factory("endpoints", function () {
    return {
        hub: "/signalr",
        webApi: "/api/values/ActorSystemStates"
    };
});
angular.module("InventoryServiceApp").service("service", function ($q, $http, $rootScope) {
    this.POST = function (url, item) {
        var deferred = $q.defer();
        var load = JSON.stringify(item);
        $http.post(url, load, {
            headers: {
                'Content-Type': "application/json"
            }
        }).
            success(deferred.resolve).
            error(deferred.reject);
        $rootScope.allCurrentHttpPromises.push(deferred.promise);
        return deferred.promise;
    };
    this.GET = function (url) {
        var deferred = $q.defer();
        $http({
            method: "GET",
            url: url
        }).
            success(deferred.resolve).
            error(deferred.reject);
        $rootScope.allCurrentHttpPromises.push(deferred.promise);
        return deferred.promise;
    };
});
angular.module("InventoryServiceApp").factory("hub", function (endpoints, $timeout) {
    $.connection.hub.url = endpoints.hub;
    var actorViewerHub = $.connection.actorViewerHub;
    return {
        ready: function (f) {
            $.connection.hub.start().done(function () {
                var arg = arguments;
                $timeout(function () {
                    f && f.apply(null, arg);
                });
            });
        },
        actorViewerHub: actorViewerHub,
        server: actorViewerHub.server,
        client: function (name, f) {
            actorViewerHub.client[name] = function (response) {
                var arg = arguments;
                $timeout(function () {
                    f && f.apply(null, arg);
                });
            };
        }
    };
});

angular.module("InventoryServiceApp").controller("ActorsCtrl", function ($scope, $rootScope, $http, $q, $timeout, hub) {
    

    $scope.performOperation = function (operationName,id, quantity,messageQty) {
        hub.server.performOperation(operationName, id, quantity, messageQty);
    };

    $scope.messageQuantity = 1;
    $scope.current = {};
    $scope . setCurrent = function(inv) {
        $scope.current = inv;
    };
    $scope.model = {};
    $scope.model.logMessages = true;
    var storage = function(a,b) {
        if (typeof (Storage) !== "undefined") {
            if (b) {
                localStorage.setItem(a, b);
                return undefined;
            } else {
             return   localStorage.getItem(a);
            }
        } else {
            console.log("Sorry! No Web Storage support..");
            return undefined;
        }
    };
    var initStorage = function() {
        $scope.model.realtime=  storage("model.realtime");
        $scope.model.logMessages = false;// storage("model.logMessages") ;
    };
    $scope.updateStorage=function () {
        storage("model.realtime", $scope.model.realtime);
        storage("model.logMessages", $scope.model.logMessages);
    }
    initStorage();
    var hasLoaded = false;
    var lastResponse = {};
    var lastResponseDict = {};
    $scope.realTimeInventories = [];

   
    $scope. updateGrid = function () {
        $scope.newUpdateAvailable = 0;

        $scope.realTimeInventories = $.map(lastResponseDict, function (value, index) {
            return [value];
        });
     
    }
    $scope.sortType = 'ProductId'; // set the default sort type
    $scope.sortReverse = false;  // set the default sort order
    $scope.searchFish = '';     // set the default search/filter term
    $scope.newUpdateAvailable = 0;
 $scope.model.incomingMessages = [];
 $scope.operationNames = [];
    var node = {
        text: {
            name: {
                val: "Root"
            }
        },
        children:[]
    };
    var tree_structure = function (n) {
        return {
            chart: {
                container: "#OrganiseChart6",
                levelSeparation: 20,
                siblingSeparation: 15,
                subTeeSeparation: 15,
                rootOrientation: "EAST",

                node: {
                    HTMLclass: "tennis-draw",
                    drawLineThrough: true
                },
                connectors: {
                    type: "straight",
                    style: {
                        "stroke-width": 2,
                        "stroke": "#ccc"
                    }
                }
            },

            nodeStructure: n
        };
    };
    hub.client("updateLog",
        function (response) {
            $timeout(function () {

                if (response.ActorDebugUpdateMessages) {

                    

                    for (var key in response.ActorDebugUpdateMessages) {
                        // skip loop if the property is from prototype
                        if (!response.ActorDebugUpdateMessages.hasOwnProperty(key)) continue;

                        var obj = response.ActorDebugUpdateMessages[key];
                        for (var prop in obj) {
                            // skip loop if the property is from prototype
                            if (!obj.hasOwnProperty(prop)) continue;

                          var destinationActor=  obj.DestinationActor.replace("akka://", "");
                          var destinationActorPath = destinationActor.split('/');
                         
                          var arrayLength = destinationActorPath.length;
                          var lastData = { text: { name: destinationActorPath[arrayLength - 1] } };
                          var nextData = {};
                          for (var i = arrayLength - 1; i > 0; i--) {
                              nextData = {};
                              nextData.text = {
                                  name: destinationActorPath[i - 1]
                              };
                              nextData.children = [];
                              nextData.children.push(JSON.parse(JSON.stringify(lastData)));
                              lastData = nextData;
                          }
                          node.children[0] = node.children[0] || {};
                            var newdata = $.extend({}, lastData, node.children[0]);
                            node.children[0]=newdata;

                         
                        }
                    }
                   // $scope.model.incomingMessages = response;




                    new Treant(tree_structure(node));
                } else {
                }
                
               
                console.log(response);
            });
        });
  
    $scope.serverNotificationMessages = "";
    $scope.jsonNotificationMessages = "";
    $scope.messageSpeed = "";
   

    hub.client("jsonNotificationMessages",
       function (response) {
           $scope.jsonNotificationMessages = response;
       });

    hub.client("serverNotificationMessages",
        function(response) {
            $scope.serverNotificationMessages = response;
        });
 
    hub.ready(function () {
        hub.server.getList();
    });
});