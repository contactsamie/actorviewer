var merge = function (x, y) {

    var res = {
        children: []
    };
    var xHas = x.children && x.children.length;
    var yHas = y.children && y.children.length;
    if ((!xHas && yHas) || !yHas && xHas) {
        x.children = x.children || [];
        y.children = y.children || [];

        for (q in x.children) {
            for (r in y.children) {
                if (x.children[q].text.name == y.children[r].text.name) {
                    console.log(x.children[q].text.name);
                } else {
                    res.children.push(y.children[r]);
                }
            }
        }
    }

    //res=$.extend({},x,y);
    console.log(res);
    return res;
};




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
                levelSeparation: 30,
                siblingSeparation: 30,
                subTeeSeparation: 30,
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
    var msges = [];
    hub.client("updateLog",
        function (response) {
            $timeout(function () {     });

            response.ActorDebugUpdateMessages = response.ActorDebugUpdateMessages || [response];
            msges = msges.concat(response.ActorDebugUpdateMessages);
            msges.reverse();
                if (response.ActorDebugUpdateMessages) {
                    function cloneObject(obj) {
                        var clone = Array.isArray(obj) ? [] : {};
                        for (var i in obj) {
                            if (typeof (obj[i]) == "object" && obj[i] != null) {
                                if (i === "children") {
                                    obj[i] = $.map(obj[i], function (value, index) {
                                        return [value];
                                    });
                                }
                                clone[i] = cloneObject(obj[i]);
                            } else {
                                clone[i] = obj[i];

                            }
                        }
                        return clone;
                    }

                    var arr = [] //your array;
                    var tree = {};

                    function addnode(obj) {
                        var splitpath = obj.DestinationActor.replace("akka://", "akka/").replace(/^\/|\/$/g, "").split('/');
                        var dest = obj.SourceActor.replace("akka://", "akka/").replace(/^\/|\/$/g, "").split('/');
                        var ptr = tree;
                        var dateOn = new Date(obj.ReceivedOn);
                        for (i = 0; i < splitpath.length; i++) {
                            node = {
                                text: {
                                    name: {
                                        val: splitpath[i]
                                    },
                                    title: obj.MessageNature + " " + obj.MessageType + " from " + dest[dest.length - 1] + " " + dateOn.getHours() + ":" + dateOn.getMinutes() + ":" + dateOn.getSeconds() + ":" + dateOn.getMilliseconds()
                                },
                                HTMLclass: "winner",
                                HTMLid: splitpath[i],
                                link: {
                                    href: "http://www.google.com",
                                    target: "_blank"
                                }
                            };
                            if (i == splitpath.length - 1) {
                                node.size = obj.size;
                                node.type = obj.type;
                            }
                            ptr[splitpath[i]] = ptr[splitpath[i]] || node;
                            ptr[splitpath[i]].children = ptr[splitpath[i]].children || {};
                            ptr = ptr[splitpath[i]].children;
                        }
                        //if (!stop) {
                        //    obj.SourceActor = obj.DestinationActor;
                        //    addnode(obj, true);
                        //}

                    }
                    msges.map(addnode);
                    var prevdata = cloneObject(tree);
                    prevdata.akka = prevdata.akka || {};
               
                    new Treant({
                        chart: {
                            container: "#OrganiseChart6",
                            levelSeparation: 20,
                            siblingSeparation: 20,
                            subTeeSeparation: 20,
                            rootOrientation: "WEST",

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
                        nodeStructure: {
                            text: {
                                name: {
                                    val: "Root"
                                }
                            },
                            HTMLclass: "winner",
                            children: [{
                                text: prevdata.akka.text,
                                children: prevdata.akka.children,
                                HTMLclass: "winner"
                            }]
                        }
                    });
                } else {
                }
                
               
                console.log(response);
       
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