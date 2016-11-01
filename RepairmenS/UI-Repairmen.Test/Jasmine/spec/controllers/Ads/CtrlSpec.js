describe("Controller group: Ads", function () {

    beforeEach(module("repairmenControllers"));

    describe("Controller: Ctrl", function () {

        var $scope,
            ctrl;

        beforeEach(inject(function ($rootScope, $controller) {

            $scope = $rootScope.$new();

            ctrl = $controller("Ctrl", {
                "$scope": $scope
            });
        }));

        it("should be defined", function () {
            expect(ctrl).toBeDefined();
        });

        it("should contain no image", function () {

            expect($scope.myImage).toEqual("");
            expect($scope.myCroppedImage).toEqual("");
        });
    });
});