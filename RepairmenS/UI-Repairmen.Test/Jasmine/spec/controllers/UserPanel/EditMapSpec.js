describe("Controller group: UserPanel", function () {

    beforeEach(module("repairmenControllers"));

    describe("Controller: EditMap", function () {

        var id,
            $scope,
            routeParams,
            authService,
            $httpBackend,
            WebApiUrl = {},
            editMapController,
            localstorageService,
            errorMessage = "error";

        beforeEach(inject(function ($rootScope, $http, _$httpBackend_, $filter, $timeout, $controller) {

            $scope = $rootScope.$new();
            $httpBackend = _$httpBackend_;

            authService = {
                authentication: {
                    userId: 12321,
                    userName: "Boki"
                }
            };

            routeParams = {
                id: 1
            };

            id = routeParams.id;

            localstorageService = {
                get: function () {
                    return (authData = { userName: "Boki" });
                },
                set: function () { }
            };

            $scope.updateInformationForm = {
                $isValid: true,
                $setPristine: function () { }
            };

            editMapController = $controller("EditMapController", {
                "$scope": $scope,
                "$rootScope":$rootScope,
                "$http": $http,
                "$filter": $filter,
                "$timeout": $timeout,
                "$routeParams": routeParams,
                "$translate": function () { },
                "authService": authService,
                "WebApiUrl": WebApiUrl,
                "localStorageService": localstorageService
            });
        }));

        it("should be defined", function () {
            expect(editMapController).toBeDefined();
        });

        it("should successfully get a single ad", function () {

            var data = {
                UserId: "12321"
            };

            spyOn($scope, "initializeMap");
            spyOn($scope, "reloadMap");

            $httpBackend.when("GET", WebApiUrl + "api/Ads/" + id + "?id=" + id)
                .respond(function () {
                    return [200, data, {}];
                });

            $scope.getSingleAd();
            $httpBackend.flush();

            expect($scope.post).toEqual(data);
        });

        it("should fail getting a single ad", function () {

            var data = {
                Message: errorMessage
            };

            $httpBackend.when("GET", WebApiUrl + "api/Ads/" + id + "?id=" + id)
                .respond(function () {
                    return [500, data, {}];
                });

            $scope.getSingleAd();
            $httpBackend.flush();

            expect($scope.errorMessages).toContain(JSON.stringify(errorMessage));
        });
    });
});