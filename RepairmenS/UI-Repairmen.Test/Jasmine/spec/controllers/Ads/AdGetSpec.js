describe("Controller group: Ads", function () {

    beforeEach(module("repairmenControllers"));

    describe("Controller: AdGetController", function () {

        var $scope,
            WebApiUrl,
            adsGetController;

        beforeEach(inject(function ($rootScope, $controller, $http) {

            $scope = $rootScope.$new();
            WebApiUrl = {};

            authService = {
                authentication: {
                    userId: "userid"
                },
                isLoggedIn: function () {
                    return true;
                }
            };

            adsGetController = $controller("AdGetController", {
                "$scope": $scope,
                "$rootScope": $rootScope,
                "$http": $http,
                "authService": authService,
                "WebApiUrl": WebApiUrl,
                "returnToUrl": {},
                "$location": {},
                "$translate": function () { }
            });
        }));

        it("should be defined", function () {
            expect(adsGetController).toBeDefined();
        });

        it("errorMessages should be empty", function () {
            expect($scope.errorMessages).not.toContain();
        });

        it("successMessages should be empty", function () {
            expect($scope.successMessages).not.toContain();
        });

        describe("$http tests", function () {

            var $httpBackend,
                someErrorMessage = "error";

            beforeEach(inject(function (_$httpBackend_) {
                $httpBackend = _$httpBackend_;
            }));

            it("should get ads", function () {

                var _adModel = {
                    id: 1,
                    name: "Banana"
                };

                var data = {
                    adModel: _adModel,
                    numberOfResults: 1
                };

                $httpBackend.when("POST", WebApiUrl + 'api/Ads')
                    .respond(function () {
                        return [200, data, {}];
                    });

                $scope.getAllAds();
                $httpBackend.flush();

                expect($scope.posts).toBeDefined();
                expect($scope.totalItems).toBeGreaterThan(0);
            });

            it("should fail getting ads", function () {

                var data = {
                    Message: someErrorMessage
                };

                $httpBackend.when("POST", WebApiUrl + 'api/Ads')
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.getAllAds();
                $httpBackend.flush();

                expect($scope.erMsg).toContain(JSON.stringify(someErrorMessage));
            });

            it("should get categories", function () {

                var data = {
                    categories: ["Plumber", "Carpenter", "Painter"]
                };

                $httpBackend.when("GET", WebApiUrl + "api/categories?approved=true")
                    .respond(function () {
                        return [200, data, {}];
                    });

                $scope.getCategories();
                $httpBackend.flush();

                expect($scope.categories).toBeDefined();
            });

            it("should fail getting categories", function () {

                var data = {
                    Message: someErrorMessage
                };

                $httpBackend.when("GET", WebApiUrl + "api/categories?approved=true")
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.getCategories();
                $httpBackend.flush();

                expect($scope.errorMessages).toContain(JSON.stringify(someErrorMessage));
            });

            it("should get cities", function () {

                var data = {
                    cities: ["Apatin", "Novi Sad", "Beograd"]
                };

                $httpBackend.when("GET", WebApiUrl + "api/Cities/Country/Serbia")
                    .respond(function () {
                        return [200, data, {}];
                    });

                $scope.getCities();
                $httpBackend.flush();

                expect($scope.cities).toBeDefined();
            });

            it("should fail getting cities", function () {

                var data = {
                    Message: someErrorMessage
                };

                $httpBackend.when("GET", WebApiUrl + "api/Cities/Country/Serbia")
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.getCities();
                $httpBackend.flush();

                expect($scope.errorMessages).toContain(JSON.stringify(someErrorMessage));
            });

            it("should filter ads", function () {

                var _adModel = {
                    id: 1,
                    name: "Banana"
                };

                var data = {
                    adModel: _adModel,
                    numberOfResults: 1
                };

                $httpBackend.when("POST", WebApiUrl + "api/Ads?dir=desc&items=5&pageno=1&sort=Date")
                    .respond(function () {
                        return [200, data, {}];
                    });

                $scope.filterAds();
                $httpBackend.flush();

                expect($scope.posts).toBeDefined();
                expect($scope.totalItems).toBeGreaterThan(0);
            });

            it("should try filtering and not find any ads", function () {
                
                var
                    data = {},
                    status = 204;

                $httpBackend.when("POST", WebApiUrl + "api/Ads?dir=desc&items=5&pageno=1&sort=Date")
                    .respond(function () {
                        return [204, data, {}];
                    });

                $scope.filterAds();
                $httpBackend.flush();

                expect($scope.errorMessages).toContain("erNotFound");
            });

            it("should fail filtering ads", function () {

                var data = {
                    Message: someErrorMessage
                };

                $httpBackend.when("POST", WebApiUrl + "api/Ads?dir=desc&items=5&pageno=1&sort=Date")
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.filterAds();
                $httpBackend.flush();

                expect($scope.erMsg).toContain(JSON.stringify(someErrorMessage));
            });

            it("should add inappropriate ad", function () {

                var adId = 1,
                    desc = "Inappropriate ad description!",
                    index = 2;

                var data = "empty";

                $httpBackend.when("PUT", WebApiUrl + "api/Ads/Inappropriate")
                    .respond(function () {
                        return [200, data, {}];
                    });

                $scope.addInnappropriateAd(adId, desc, index);
                $httpBackend.flush();

                expect($scope.successMessages).toContain("innapAdReported");
            });

            it("should fail adding inappropriate ad", function () {

                var adId = 1,
                    desc = "Inappropriate ad description!",
                    index = 2;

                var data = {
                    Message: someErrorMessage
                };

                $httpBackend.when("PUT", WebApiUrl + "api/Ads/Inappropriate")
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.addInnappropriateAd(adId, desc, index);
                $httpBackend.flush();


                expect($scope.erMsg).toContain(JSON.stringify(someErrorMessage));
            });
        });
    });
});