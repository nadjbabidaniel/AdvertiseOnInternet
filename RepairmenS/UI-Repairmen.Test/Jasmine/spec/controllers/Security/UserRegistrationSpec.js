describe("Controller group: Security", function () {

    beforeEach(module("repairmenControllers"));

    describe("Controller: UserRegistration", function () {

        var
            $scope,
            adduserCtrl,
            $httpBackend,
            errorMessage = "error";

        beforeEach(inject(function ($rootScope, $http, $controller) {

            $scope = $rootScope.$new();

            adduserCtrl = $controller("UserRegistrationController", {
                "$scope": $scope,
                "$http": $http,
                "$translate": {}
            });
        }));

        beforeEach(inject(function (_$httpBackend_) {
            $httpBackend = _$httpBackend_;
        }));

        it("should be defined", function () {
            expect(adduserCtrl).toBeDefined();
        });

        it("should contain no messages", function () {
            expect($scope.errorMessages).not.toContain();
            expect($scope.successMessages).not.toContain();
        });

        it("should successfully register a user", function () {

            var signup = {
                id: 1,
                userName: "test"
            },
            data = {
                Message: errorMessage
            };

            $scope.signup_form = {
                $valid: true,
                $setPristine: function () { }
            }

            $httpBackend.when("PUT", "index.html", signup)
                .respond(function () {
                    return [200, data, {}];
                });

            $scope.addUser(signup);
            $httpBackend.flush();

            expect($scope.people).toEqual(data);
        });

        it("should fail to register a user", function () {

            var signup = {
                id: 1,
                userName: "test"
            },
            data = {
                Message: errorMessage
            };

            $scope.signup_form = {
                $valid: true,
                $setPristine: function () { }
            }

            $httpBackend.when("PUT", "index.html", signup)
                .respond(function () {
                    return [500, data, {}];
                });

            $scope.addUser(signup);
            $httpBackend.flush();

            expect($scope.errorMessages).toContain(JSON.stringify(errorMessage));
        });
    });
});