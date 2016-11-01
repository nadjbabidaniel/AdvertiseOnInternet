describe("Controller group: Categories", function () {

    beforeEach(module("repairmenControllers"));

    describe("Controller: AdCategory", function () {
        
        var
            $scope,
            WebApiUrl,
            $httpBackend,
            addCategoryCtrler,
            errorMessage = "error";

        beforeEach(inject(function ($rootScope, $http, $controller) {

            $scope = $rootScope.$new();
            WebApiUrl = {};

            addCategoryCtrler = $controller("AddCategoryController", {
                "$scope": $scope,
                "$http": $http,
                "WebApiUrl": WebApiUrl,
                "$translate": {}
            });
        }));

        beforeEach(inject(function(_$httpBackend_){
            $httpBackend = _$httpBackend_;
        }));

        it("should be defined", function () {
            expect(addCategoryCtrler).toBeDefined();
        });

        it("should have no messages", function () {
            expect($scope.errorMessages).not.toContain();
            expect($scope.successMessages).not.toContain();
        });

        it("should add a category", function () {

            var
                category = {
                    id: 1,
                    name: "Sweets"
                },
                data = {
                    value: "Test"
                };

            $httpBackend.when("PUT", WebApiUrl + "api/categories", category)
                .respond(function () {
                    return [200, data, {}];
                });

            $scope.addCategory(category);
            $httpBackend.flush();

            expect($scope.category).toEqual(data);
        });

        it("should fail adding a category", function () {

            var
                category = {
                    id: 1,
                    name: "Sweets"
                },
                data = {
                    value: "Test",
                    Message: errorMessage
                };

            $httpBackend.when("PUT", WebApiUrl + "api/categories", category)
                .respond(function () {
                    return [500, data, {}];
                });

            $scope.addCategory(category);
            $httpBackend.flush();

            expect($scope.errorMessages).toContain(JSON.stringify(errorMessage));
        });
    });
});