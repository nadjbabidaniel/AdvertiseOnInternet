describe("Controller group: Ads", function () {

    beforeEach(module("repairmenControllers"));

    beforeEach(module(function ($provide) {
        $provide.value('WebApiUrl', {
            getObjs: function () {
                return { then: function (callback) { callback(mockedResponseData); } };
            }
        });
    }));

    describe("Controller: AdRegistrationController", function () {

        var $scope,
            WebApiUrl,
            $httpBackend,
            adRegistrationController,
            errorMessage = "error";

        beforeEach(inject(function ($rootScope, $controller, $http, $location) {

            $scope = $rootScope.$new();
            WebApiUrl = {};

            adRegistrationController = $controller("AdRegistrationController", {
                "$scope": $scope,
                "$http": $http,
                "$location": $location,
                "WebApiUrl": WebApiUrl,
                "$translate": {}
            });
        }));

        beforeEach(inject(function (_$httpBackend_) {
            $httpBackend = _$httpBackend_;
        }));

        it("should be defined", function () {
            expect(adRegistrationController).toBeDefined();
        });

        it("should have no messages", function () {
            expect($scope.errorMessages).not.toContain();
            expect($scope.successMessages).not.toContain();
        });

        describe("$http tests", function () {

            it("should create new ad", function () {

                /*
                Used a spy to mock jQuery,
                must have actual jQuery lib
                as a reference for SpecRunner.html
                */

                var data = {
                    value: "test"
                },
                ad = {
                    Image: "path/to/image.jpg"
                };

                var spy = spyOn($.fn, 'attr');

                $httpBackend.when("PUT", WebApiUrl + "api/Ads", ad)
                    .respond(function () {
                        return [200, data, {}];
                    });

                $scope.newAd(ad);
                $httpBackend.flush();

                expect(spy).toHaveBeenCalledWith('ng-src');
                expect($scope.people).toEqual(data);
            });
        });
    });
});