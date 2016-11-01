repairmenControllers.controller("PayPalController", function ($scope, $http, WebApiUrl, purchaseService) {

    $scope.ad = purchaseService.getData();
    
    $scope.purchasePackages = [
        poorMan = {
            name: "Poor man",
            views: 100,
            daysActive: 7
        }
    ];

    $scope.purchasePackage = $scope.purchasePackages[0];

    $scope.purchase = function () {

        alert("Congratulations on successfull purchase!\nYour ad " + $scope.ad.name + ", with id: " + $scope.ad.id +
            " has Ad Displayer 5000 active for " + $scope.purchasePackage.daysActive +
            " days, or " + $scope.purchasePackage.views +
            " views!\n" + $scope.purchasePackage.name + " package active.");

        purchaseService.setPurchased();
    };
});

repairmenControllers.controller("PayPalSuccessController", function ($scope, $http, WebApiUrl, purchaseService) {

    $scope.ad = purchaseService.getData();

    $scope.purchaseSuccessfull = function () {

        $http.get(WebApiUrl + "api/paypal/success")
            .success(function (data) {
                console.log(data);
            })
            .error(function (data) {
                console.log("Error!");
            });
    };
});

repairmenControllers.controller("PayPalCancelController", function ($scope, $http, WebApiUrl, purchaseService) {

    $scope.ad = purchaseService.getData();

    $scope.purchaseCanceled = function () {

        $http.get(WebApiUrl + "api/paypal/cancel")
            .success(function () {
                console.log("Successfully canceled!");
            })
            .error(function () {
                console.log("Encountered an error while canceling!");
            });
    };
});