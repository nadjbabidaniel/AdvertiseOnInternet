repairmenControllers.controller('RatingCtrl', function ($scope, $http, authService, WebApiUrl, $location,returnToUrl) {

    $scope.errorMessages = [];
    $scope.successMessages = [];

    if ($("div[ng-controller='RatingCtrl']")) {

        var url = $(location).attr('href');
        var n = url.lastIndexOf("/");
        var resID = url.substring(n + 1);
        if (resID.length === 36) {
            $http.get(WebApiUrl + 'api/Ads/' + resID).success(function (data) {
                $scope.rate = data.AvgRate;
            }).error(function (data) {
                $scope.rate = $scope.post.AvgRate;
            });

        }
    else {$scope.rate = $scope.post.AvgRate; }
    }
    else {
        $scope.rate = $scope.post.AvgRate;
    }
       
    $scope.max = 5;


    $scope.hoveringOver = function (value) {
        $scope.overStar = value;
        $scope.percent = 100 * (value / $scope.max);
    };

    $scope.ratingStates = [
      { stateOn: 'glyphicon-ok-sign', stateOff: 'glyphicon-ok-circle' },
      { stateOn: 'glyphicon-star', stateOff: 'glyphicon-star-empty' },
      { stateOn: 'glyphicon-heart', stateOff: 'glyphicon-ban-circle' },
      { stateOn: 'glyphicon-heart' },
      { stateOff: 'glyphicon-off' }
    ];

    $scope.addRate = function (adId, rate) {
        var rating = {
            adId: adId,
            userId: authService.authentication.userId,
            value: rate
        }

        if (authService.isLoggedIn()) {
            $http.put(WebApiUrl + 'api/Ratings', rating)
                .success(function (data) {
                    $scope.rate = rate;

                    $http.get(WebApiUrl + 'api/Ads/' + $scope.post.Id).success(function (data) {
                        $scope.post.AvgRate = data.AvgRate;
                        $scope.post.VoteCounter = data.VoteCounter;
                    }).error(function (data) {
                        var msg = JSON.stringify(data.Message);
                        $scope.errorMessages.push(msg);
                    });

                }).error(function (data, status) {
                    var msg = JSON.stringify(data.Message);
                    $scope.errorMessages.push(msg);

                });
        }
        else {
            returnToUrl.storeData("","","");
            $location.path('/signin');
        }
        
    }
});

angular.module('repairmenControllers').controller('NavCtrl', function ($scope) {
    $('.nav a').on('click', function () {
        if ($(window).width() < 768) {
            $(".navbar-toggle").click();
        }
    });
});