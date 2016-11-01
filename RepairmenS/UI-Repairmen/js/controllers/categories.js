repairmenControllers.controller('AddCategoryController', function ($scope, $rootScope, $translate, $http, $timeout, WebApiUrl) {

    var categoryRequest = "categoryRequest";

    $scope.errorMessages = [];
    $scope.successMessages = [];

    $rootScope.$on("$translateChangeSuccess", function () {
        translateMsg();
    });

    function translateMsg() {
        
        $translate([
            "INFO_MESSAGES.CATEGORIES.CATEGORY_REQUEST"
        ]).then(function (translation) {
            categoryRequest = translation['INFO_MESSAGES.CATEGORIES.CATEGORY_REQUEST'];
        })
    };

    $scope.initializeCatTranslate = function () {
        translateMsg();
    };

    $scope.addCategory = function (category) {
        $http.put(WebApiUrl + 'api/categories', category)
            .success(function (data) {
                $scope.category = data;
                var msg = categoryRequest;
                $scope.successMessages.push(msg);
                removeMsgs();
            }).error(function (data) {
                var msg = JSON.stringify(data.Message)
                $scope.errorMessages.push(msg);
                removeMsgs();
            });
    }

    function removeMsgs() {
        $timeout(function () {
            $scope.successMessages = [];
            $scope.errorMessages = [];
        }, 4000);
    }
});