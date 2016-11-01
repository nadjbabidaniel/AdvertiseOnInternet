repairmenControllers.controller('AdGetController', function ($scope, $rootScope, $http, $timeout, authService, WebApiUrl, returnToUrl, $location, $translate) {
    var app = this;
    var l10nSortValues = [];
    var erNotFound = "erNotFound",
        innapAdReported = "innapAdReported",
        innapAdAlready = "innapAdAlready";

    $scope.errorMessages = [];
    $scope.successMessages = [];

    $scope.showInnappropriateAd = [];
    $scope.description = [];

    $scope.dirValue = 'desc';

    $scope.erMsg = "";

    $rootScope.$on('$translateChangeSuccess', function () {
        translateSortValues();
    });

    function translateSortValues() {

        $translate([
            'ALL_ADS.CONTROLLER.NAME',
            'ALL_ADS.CONTROLLER.DATE',
            'ALL_ADS.CONTROLLER.COMMENT',
            'ALL_ADS.CONTROLLER.RATING',
            'INFO_MESSAGES.ADS.ER_NOTFOUND',
            'INFO_MESSAGES.ADS.INNAP_AD_REPORTED',
            'INFO_MESSAGES.ADS.INAP_AD_ALREADY'
        ]).then(function (translations) {

            l10nSortValues = [
                translations['ALL_ADS.CONTROLLER.NAME'],
                translations['ALL_ADS.CONTROLLER.DATE'],
                translations['ALL_ADS.CONTROLLER.COMMENT'],
                translations['ALL_ADS.CONTROLLER.RATING']
            ];

            $scope.sortValues = l10nSortValues;

            erNotFound = translations['INFO_MESSAGES.ADS.ER_NOTFOUND'];
            innapAdReported = translations['INFO_MESSAGES.ADS.INNAP_AD_REPORTED']; 
            innapAdAlready = translations['INFO_MESSAGES.ADS.INAP_AD_ALREADY'];
        });
    };

    $scope.sortValueIndex = "1";
    $scope.sortValue = 'Date';

    function setSortValue() {

        switch ($scope.sortValueIndex) {
            case "0":
                $scope.sortValue = 'Name';
                break;
            case "1":
                $scope.sortValue = 'Date';
                break;
            case "2":
                $scope.sortValue = 'Comment';
                break;
            case "3":
                $scope.sortValue = 'Rating';
                break;
        }
    };

    $scope.numberOfItems = [2, 5, 10, 20, 50];
    $scope.itemNumber = 5;

    $scope.totalItems = 0;
    $scope.currentPage = 1;

    $scope.maxSize = 5;
    $scope.bigCurrentPage = 1;

    $scope.getAllAds = function () {
        $scope.myPromise = $http.post(WebApiUrl + 'api/Ads', {
            params: {
                sort: $scope.sortValue,
                dir: $scope.dirValue,
                items: 5,
                pageno: 1
            }
        }).success(function (data) {
            $scope.posts = data.adModel;
            $scope.totalItems = data.numberOfResults;
            $scope.bigTotalItems = data.numberOfResults;
        }).error(function (data) {
            $scope.totalItems = 0;
            $scope.bigTotalItems = 0;
            var msg = erNotFound;
            $scope.errorMessages.push(msg);
            $scope.erMsg = JSON.stringify(data.Message); //For testing purposes only
            removeMsgs();
        });
    };

    $scope.defaultAds = function () {
        $scope.errorMessages = [];
        $scope.successMessages = [];
        $scope.data.keyword = '';
        $scope.data.city = '';
        $scope.data.category = '';
        $scope.myPromise = $http.post(WebApiUrl + 'api/Ads', {
            params: {
                sort: 'Date',
                dir: 'desc',
                items: 5,
                pageno: 1
            }
        }).success(function (data) {
            $scope.posts = data.adModel;
            $scope.totalItems = data.numberOfResults;
            $scope.bigTotalItems = data.numberOfResults;
        }).error(function (data) {
            $scope.totalItems = 0;
            $scope.bigTotalItems = 0;
            var msg = erNotFound;
            $scope.errorMessages.push(msg);
            $scope.erMsg = JSON.stringify(data.Message); //For testing purposes only
            removeMsgs();
        });
    }

    $scope.getCategories = function () {

        $http.get(WebApiUrl + 'api/categories', {
            params: { approved: true }
        })
            .success(function (data) {
                $scope.categories = data;
            }).error(function (data) {
                var msg = JSON.stringify(data.Message);
                $scope.errorMessages.push(msg);
                removeMsgs();
            });
    };

    $scope.getCities = function () {
        $http.get(WebApiUrl + 'api/Cities/Country/Serbia')
            .success(function (data) {
                $scope.cities = data;
            }).error(function (data) {
                var msg = JSON.stringify(data.Message);
                $scope.errorMessages.push(msg);
                removeMsgs();
            });
    };

    $scope.filterAds = function (data, fPage) {

        if (fPage == 'first')
            $scope.currentPage = 0;
        setSortValue();

        $scope.myPromise = $http.post(WebApiUrl + 'api/Ads', data, {
            params: {
                sort: $scope.sortValue,
                dir: $scope.dirValue,
                items: $scope.itemNumber,
                pageno: $scope.currentPage
            }
        }).success(function (data, status) {
            $scope.posts = data.adModel;
            $scope.totalItems = data.numberOfResults;
            $scope.bigTotalItems = data.numberOfResults;
            $scope.paidAds = data.paidAds;
            $scope.errorMessages = [];
            if (status == '204') {
                $scope.errorMessages.push(erNotFound);
                removeMsgs();
            }
        }).error(function (data) {
            var msg = erNotFound;
            $scope.totalItems = 0;
            $scope.bigTotalItems = 0;
            $scope.errorMessages.push(msg);
            $scope.posts = [];
            $scope.erMsg = JSON.stringify(data.Message); //For testing purposes only
            removeMsgs();
        });
    };

    $scope.addInnappropriateAd = function (adId, description, index) {

        var inappropriateAd = {
            adId: adId,
            userId: authService.authentication.userId,
            description: description
        };
        if (authService.isLoggedIn()) {
            $http.put(WebApiUrl + 'api/Ads/Inappropriate', inappropriateAd)
                .success(function (data) {
                    $scope.showInnappropriateAd[index] = false;
                    $scope.description[index] = "";
                    var msg = innapAdReported;
                    //hide element after success report of Inapp Ad
                    $scope.showInnappropriateAd = [];
                    $scope.successMessages.push(msg);
                    removeMsgs();
                }).error(function (data, status) {
                    if (status == "403") {
                        $scope.errorMessages.push(innapAdAlready);
                        $scope.showInnappropriateAd = [];
                    }
                    else {
                        var msg = JSON.stringify(data.Message);
                        $scope.errorMessages.push(status);
                    }
                    $scope.description[index] = "";
                    $scope.showInnappropriateAd[index] = false;
                    $scope.erMsg = JSON.stringify(data.Message); //For testing purposes only
                    removeMsgs();
                });
        }
        else {
            returnToUrl.storeData(inappropriateAd);
            $location.path('/signin');
        }
    };

    $scope.initialise = function () {

        translateSortValues();
        $scope.getAllAds();
        $scope.getCategories();
        $scope.getCities();
    };
    $scope.shortDescription = [];
    $scope.longDescription = [];
    $scope.seeMore = [];
    var shownAdDescription = '';
   
    $scope.checkLength = function (descript,id) {
        if (descript.length < 60) {
            return false;
        }
        else
        {
            if (shownAdDescription === id) {
                $scope.seeMore[shownAdDescription] = false;
            }
            else {
                $scope.seeMore[id] = true;
            }
            return true;
        }
    };
    
    $scope.showLongDescription = function (id) {
        if (shownAdDescription !== '') {
            $scope.shortDescription[shownAdDescription] = false;
            $scope.longDescription[shownAdDescription] = false;
            $scope.seeMore[shownAdDescription] = true;
        }
        shownAdDescription = id;
        $scope.shortDescription[id] = true;
        $scope.longDescription[id] = true;
       
    };

    function removeMsgs() {
        $timeout(function () {
            $scope.successMessages = [];
            $scope.errorMessages = [];
        }, 4000);
    }
});



repairmenControllers.controller('ActivateController', function ($scope, $http, $rootScope, $routeParams, $translate, $timeout, $location, authService, WebApiUrl) {
    var app = this;
    var id = $routeParams.id;
    var activationSuccess = "activationSuccess";
    var activationFailed = "activationFailed";

    $rootScope.$on('$translateChangeSuccess', function () {
        translateMsgs();
    });

    function translateMsgs() {

        $translate([
            'ACTIVATE_ACC.ACTIVATED',
            'ACTIVATE_ACC.NOT_ACTIVATED',
        ]).then(function (translations) {
            activationSuccess = translations['ACTIVATE_ACC.ACTIVATED'];
            activationFailed = translations['ACTIVATE_ACC.NOT_ACTIVATED'];
        });
    };

    $scope.initTranslate = function () {
        translateMsgs();
    }

    $http.get(WebApiUrl + 'api/login/activate/' + id, {
        params: { id: id }
    }).success(function (data, status) {
        $scope.post = data;
        $('.actClass').css('color', '#2FA4E7');
        $scope.activate_result = activationSuccess;
        $timeout(function () {
            $location.path('/signin');
        }, 2500);
    }).error(function (data) {
        $('.actClass').css('color', 'red');
        $scope.activate_result = activationFailed;
    });
});

repairmenControllers.controller('SingleAdController', function ($scope, $rootScope, $timeout, $translate, $http, $routeParams, $window, authService, WebApiUrl, $location, returnToUrl, ngDialog) {
    var app = this;
    var id = $routeParams.id;
    var adOwnerId = "";
    var mailTo = "";
    var mailSubject = "";

    var commentAdded = "commentAdded",
        commentReported = "commentReported",
        commentReportedAlready = 'commentReportedAlready';

    $rootScope.$on('$translateChangeSuccess', function () {
        translateMsgs();
    });

    function translateMsgs() {

        $translate([
            'INFO_MESSAGES.ADS.COMMENT_ADDED',
            'INFO_MESSAGES.ADS.COMMENT_REPORTED',
            'INFO_MESSAGES.ADS.COMMENT_REPORTED_ALREADY'
        ]).then(function (translations) {
            commentAdded = translations['INFO_MESSAGES.ADS.COMMENT_ADDED'];
            commentReported = translations['INFO_MESSAGES.ADS.COMMENT_REPORTED'];
            commentReportedAlready = translations['INFO_MESSAGES.ADS.COMMENT_REPORTED_ALREADY'];
        });
    };

    $scope.comment = {};
    
    if (returnToUrl.getType() === "c") {
        $scope.comment.text = returnToUrl.getComment();
    };
   

    clearMessages();

    $scope.showInnappropriateDescription = [];
    $scope.description = [];
    if (returnToUrl.getType() === "d") {
        $scope.showInnappropriateDescription[returnToUrl.getIndex()] = true;
        $scope.description[returnToUrl.getIndex()] = returnToUrl.getComment();
    };

    $scope.vote = null;

    function clearMessages() {
        $scope.errorMessages = [];
        $scope.successMessages = [];
    }

    $scope.getSingleAd = function () {
        $http.get(WebApiUrl + 'api/Ads/' + id, {
            params: { id: id }
        }).success(function (data, status) {
            $scope.post = data;
            $scope.initializeMap();
            listComments();
            mailSubject = "Repairmen: message about your ad " + data.Name;
            adOwnerId = data.UserId;
            $http.get(WebApiUrl + 'api/Users/' + adOwnerId)
            .success(function (data, status) {
                mailTo = data.Email;
            }).error(function (data) {
                var msg = JSON.stringify(data.Message);
                $scope.errorMessages.push(msg);
                removeMsgs();
            });

        }).error(function (data) {
            var msg = JSON.stringify(data.Message);
            $scope.errorMessages.push(msg);
            removeMsgs();
        });
    }

    $scope.addComment = function (comment) {
        if (authService.isLoggedIn()) {
            comment.adId = id;
            comment.userId = authService.authentication.userId;

            $scope.myPromise = $http.put(WebApiUrl + 'api/Comments', comment)
                .success(function (data) {
                    $scope.comment.text = "";
                    $scope.addCommentForm.$setPristine();
                    var msg = commentAdded;
                    clearMessages();
                    $scope.successMessages.push(msg);
                    listComments();
                    returnToUrl.clearData();
                    removeMsgs();                
                }).error(function (data) {
                    var msg = JSON.stringify(data.Message);
                    $scope.errorMessages.push(msg);
                    removeMsgs();
                });
        }
        else {
            returnToUrl.storeData(comment.text,"c","");
            $location.path('/signin');
        }
    }




    $scope.addInnappropriateComment = function (description, commentId, index) {
        if (authService.isLoggedIn()) {
        var inputComment = {
            commentId: commentId,
            userId: authService.authentication.userId,
            description: description
        };

            $http.put(WebApiUrl + 'api/Comments/Inappropriate', inputComment)
                .success(function (data) {
                    $scope.showInnappropriateDescription[index] = false;
                    $scope.description[index] = "";
                    var msg = commentReported;
                    clearMessages();
                    $scope.successMessages.push(msg);
                    removeMsgs();
                }).error(function (data, status) {
                    clearMessages();
                    if (status == "403") {
                        $scope.errorMessages.push(commentReportedAlready);
                    }
                    else {
                        var msg = JSON.stringify(data.Message);
                        $scope.errorMessages.push(msg);
                    }
                    $scope.showInnappropriateDescription[index] = false;
                    $scope.description[index] = "";
                    removeMsgs();
                });
        }
        else {
            returnToUrl.storeData(description,"d",index);
            $location.path('/signin');
        }

    }

    $scope.addVote = function (commentId, vote, index) {
        if (authService.isLoggedIn()) {
        var voting = {
            commentId: commentId,
            userId: authService.authentication.userId,
            vote: vote
        }
       
            $http.put(WebApiUrl + 'api/Comments/Vote', voting)
                .success(function (data) {
                    $scope.vote = vote;
                    //  $scope.isReadonly = true;

                    $http.get(WebApiUrl + 'api/Comments/Id/' + commentId).success(function (data) {
                        $scope.comments[index].PositiveVote = data.PositiveVote;
                        $scope.comments[index].NegativeVote = data.NegativeVote;
                    }).error(function (data) {
                        var msg = JSON.stringify(data.Message);
                        $scope.errorMessages.push(msg);
                        removeMsgs();
                    });

                }).error(function (data) {
                    var msg = JSON.stringify(data.Message);
                    $scope.errorMessages.push(msg);
                    removeMsgs();
                });
        }
        else {
            returnToUrl.storeData("","","");
            $location.path('/signin');
        }

    }

    function listComments() {
        $scope.myPromise = $http.get(WebApiUrl + 'api/Comments/AdId/' + id)
            .success(function (data) {
                $scope.comments = data;
            }).error(function (data) {
                //var msg = JSON.stringify(data.Message);
                //$scope.errorMessages.push(msg);
            });
    }

    //Dialog for sending email
    $scope.clickToOpen = function () {
        if (authService.isLoggedIn()) {
            var em = {
                From: "",
                To: "",
                Subject: "",
                Body: ""
            }
            ngDialog.openConfirm({
                template: 'partials/emailDialog.html',
                className: 'ngdialog-theme-default',
                preCloseCallback: 'preCloseCallbackOnScope',
                scope: $scope
            }).then(function (value) {
                em.Subject = mailSubject;
                em.Body = value;
                em.From = authService.authentication.userName;
                em.To = mailTo;

                $http.put(WebApiUrl + 'api/Users/Send', em)
                   .success(function (data) {
                       var msg = "Mail sent!";
                       $scope.successMessages.push(msg);
                       removeMsgs();
                   }).error(function (data) {
                       var msg = JSON.stringify(data.Message);
                       $scope.errorMessages.push(msg);
                       removeMsgs();
                   });
            }, function (reason) {
            });
        }
        else {
            returnToUrl.storeData("", "", "");
            $location.path('/signin');
        }
    };

    function removeMsgs() {
        $timeout(function () {
            $scope.successMessages = [];
            $scope.errorMessages = [];
        }, 4000);
    }

    $scope.initializeMap = function () {

        var markLat = $scope.post.latitude;
        var markLng = $scope.post.longitude;

        var location;
        var zoom;

        if (markLat && markLng) {
            location = new google.maps.LatLng(markLat, markLng);
            zoom = 15;
        } else {
            location = new google.maps.LatLng(44.1, 21.0667);
            zoom = 6;
        };

        var mapProp = {
            center: location,
            zoom: zoom,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
        var markers = [];

        if (markLat && markLng) {
            placeMarker(location);
        }

        function placeMarker(location) {
            var marker = new google.maps.Marker({
                position: location,
                map: map,
            });
            markers.push(marker);
            var infoWindow = new google.maps.InfoWindow();
            marker.content = "<div style='height:60px'><h5>" + $scope.post.Name + "</h5><div>Location: " + $scope.post.Location + "</div></div>";

            google.maps.event.addListener(marker, 'click', function () {
                infoWindow.setContent(marker.content);
                infoWindow.open(map, marker);
                map.setCenter(location);
                map.setZoom(zoom);
            });
        }      
    }

    $scope.initialiseSingleAd = function () {
        $scope.getSingleAd();
        translateMsgs();
    };

});

repairmenControllers.controller('AdRegistrationController', function ($scope, $rootScope, $translate, $http, $location, WebApiUrl) {

    var adPosted = "adPosted";

    $rootScope.$on('$translateChangeSuccess', function () {
        translateMsgs();
    });

    function translateMsgs() {

        $translate([
            'INFO_MESSAGES.ADS.AD_POSTED'
        ]).then(function (translations) {
            adPosted = translations['INFO_MESSAGES.ADS.AD_POSTED'];
        });
    };

    $scope.errorMessages = [];
    $scope.successMessages = [];

    //initializeMap();

    $scope.newAd = function (ad) {
        ad.Image = $('#croppedpic').attr('ng-src');
        if (!ad.website)
            ad.website = "";
        $http.put(WebApiUrl+'api/Ads', ad)
                   .success(function (data) {
                       $scope.people = data;
                       $scope.successMessages = [];
                       $scope.successMessages.push(adPosted);
                       $location.path('/ads');
                   }).error(function (data) {
                       var msg = JSON.stringify(data.Message)
                       $scope.errorMessages = [];
                       $scope.errorMessages.push(msg);
                   });
    }

    function getCategories() {
        $http.get(WebApiUrl + 'api/categories', {
            params: { approved: true }
        })
            .success(function (data) {
                $scope.categories = data;
            }).error(function (data) {
                var msg = JSON.stringify(data.Message);
                $scope.errorMessages.push(msg);
            });
    };

    function getCities() {
        $http.get(WebApiUrl + 'api/Cities/Country/Serbia')
            .success(function (data) {
                $scope.cities = data;
            }).error(function (data) {
                var msg = JSON.stringify(data.Message);
                $scope.errorMessages.push(msg);
            });
    };

    function initializeMap() {

        var mapProp = {
            center: new google.maps.LatLng(44.1, 21.0667),
            zoom: 6,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
        var markers = [];

        google.maps.event.addListener(map, 'click', function (event) {
            deleteMarkers();
            placeMarker(event.latLng);
            $scope.ad.latitude = event.latLng.lat();
            $scope.ad.longitude = event.latLng.lng();
        });

        function placeMarker(location) {
            var marker = new google.maps.Marker({
                position: location,
                map: map,
            });
            markers.push(marker);
        }
        function deleteMarkers() {
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(null);
            }
            markers = [];
        }
    }

    $scope.reloadMap = function () {
        var markers = [];
        map.setZoom(13);
        var location = new google.maps.LatLng($scope.ad.city.Latitude, $scope.ad.city.Longitude);
        map.setCenter(location);
        var marker = new google.maps.Marker({
            position: location,
            map: map,
        });
        markers.push(marker);
        $scope.ad.latitude = $scope.ad.city.Latitude;
        $scope.ad.longitude = $scope.ad.city.Longitude;
    }

    $scope.initialiseAdRegistration = function () {
        initializeMap();
        getCategories();
        getCities();
    };
});


repairmenControllers.controller('Ctrl', function ($scope) {
    $scope.myImage = '';
    $scope.myCroppedImage = '';

    var handleFileSelect = function (evt) {
        var file = evt.currentTarget.files[0];
        var reader = new FileReader();
        reader.onload = function (evt) {
            $scope.$apply(function ($scope) {
                $scope.myImage = evt.target.result;
            });
        };
        reader.readAsDataURL(file);
    };
    angular.element(document.querySelector('#fileInput')).on('change', handleFileSelect);
});

repairmenControllers.controller('SharingCtrl', function ($scope, $routeParams) {
    $scope.current_title = "Repairmen";
    $scope.current_description = "See my Ad on Repairmen Website";
    $scope.data_url = "http://htrepairmen.cloudapp.net/#/ads/" + $routeParams.id;
});