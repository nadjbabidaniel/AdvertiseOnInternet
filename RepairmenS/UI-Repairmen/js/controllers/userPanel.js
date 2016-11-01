repairmenControllers.controller('UserPanelController', function ($scope,$rootScope,$translate, $http, $location, $timeout, ngDialog, authService, WebApiUrl, localStorageService, purchaseService) {
    
    var adUpdated = "adUpdated",
        userInfoUpdated = "userInfoUpdated",
        adDeleted = "adDeleted",
        passVerifyError = "passVerifyError",
        passOldDoesntMatch = "passOldDoesntMatch",
        resetForbidden = "resetForbidden";


    $rootScope.$on("$translateChangeSuccess", function () {
        translateMsgs();
    });

    function translateMsgs() {

        $translate([
            "INFO_MESSAGES.USER_PANEL.AD_UPDATED",
            "INFO_MESSAGES.USER_PANEL.USERINFO_UPDATED",
            "INFO_MESSAGES.USER_PANEL.AD_DELETED",
            "INFO_MESSAGES.SECURITY.PASSWORD_VERIFY",
            "INFO_MESSAGES.SECURITY.PASSWORD_OLD",
            "INFO_MESSAGES.RESET_PASSWORD.FORBIDDEN"
        ]).then(function (translations) {
            adUpdated = translations['INFO_MESSAGES.USER_PANEL.AD_UPDATED'];
            userInfoUpdated = translations['INFO_MESSAGES.USER_PANEL.USERINFO_UPDATED'];
            adDeleted = translations['INFO_MESSAGES.USER_PANEL.AD_DELETED'];
            passVerifyError = translations['INFO_MESSAGES.SECURITY.PASSWORD_VERIFY'];
            passOldDoesntMatch = translations['INFO_MESSAGES.SECURITY.PASSWORD_OLD'];
            resetForbidden = translations['INFO_MESSAGES.RESET_PASSWORD.FORBIDDEN'];
        })
    };

    $scope.transMsg = function () {
        translateMsgs();
    }
    
    $scope.errorMessages = [];
    $scope.successMessages = [];

    function clearMsgs() {
        $scope.errorMessages = [];
        $scope.successMessages = [];
    }

    resetUser();        
     
    $scope.initialiseUserPanel = function () {
        $scope.transMsg();
        clearMsgs();
        $scope.getUser();
        getAds();
    }
     
    var id = authService.authentication.userId;
    var mail = $scope.userPage;

    $scope.checkMail = function (data) {
        var re = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$/;
        var valid = re.test(data);
        if (!valid)
        {
            return "Email is not valid.";
        }
    };
    $scope.checkPhone = function (data) {
        var re = /^[/+0-9()]{1}[\-_()\/\\ \d]{8,17}$/;
        var valid = re.test(data);
        if (!valid) {
            return "Phone is not valid.";
        }
    };
    $scope.checkWebsite = function (data) {
        var re = /^(((http(?:s)?\:\/\/)|www\.)[a-zA-Z0-9\-]+(?:\.[a-zA-Z0-9\-]+)*\.[a-zA-Z]{2,6}(?:\/?|(?:\/[\w\-]+)*)(?:\/?|\/\w+\.[a-zA-Z]{2,4}(?:\?[\w]+\=[\w\-]+)?)?(?:\&[\w]+\=[\w\-]+)*)$/;
        var valid = re.test(data);
        if (!valid) {
            return "Website is not valid.";
        }
    };

    $scope.getUser = function () {
    $http.get(WebApiUrl + 'api/Users/' + id)
    .success(function (data, status) {
        $scope.user.displayName = data.Username;
        $scope.user.notifyEmail = data.NotifyEmail;
        $scope.user.notifySMS = data.NotifySMS;
        $scope.user.phoneNumber = data.PhoneNumber;
    }).error(function (data) {
            var msg = JSON.stringify(data.Message);
            $scope.errorMessages.push(msg);
    });
    };

    function getAds() {
    $http.get(WebApiUrl + 'api/Ads/UserId/' + id)        
      .success(function (data, status) {
          $scope.posts = data;
          $scope.postsBkp = data;
      }).error(function (data) {
          $scope.posts = [];
          //var msg = JSON.stringify(data.Message);
         // $scope.errorMessages.push(msg);
      });
    };

    //Delete Ad:
    $scope.deleteAd = function (post) {
        ngDialog.openConfirm({
            template: 'partials/widgets/confirm-dialog.html',
            className: 'ngdialog-theme-default',
            preCloseCallback: 'preCloseCallbackOnScope',
            scope: $scope
        }).then(function () {
            $http.post(WebApiUrl + 'api/Ads/Delete', post)
                .success(function (data) {
                    clearMsgs();
                    getAds();
                    $scope.successMessages.push(adDeleted);
                    removeSuccessMsg();
                }).error(function (data) {
                    clearMsgs();
                    var msg = JSON.stringify(data.Message);
                    $scope.errorMessages.push(msg);
                });

        }, function (reason) {
        });  
    };

    $scope.updatePost = function (ad) {

        $http.put(WebApiUrl + 'api/Ads/UpdateSingleAd/', ad)
                   .success(function (data) {
                       getAds();
                       clearMsgs();
                       $scope.successMessages.push(adUpdated);
                       removeSuccessMsg();
                       return (ad && ad.length) ? selected[0].CatName : 'Not Set';
                   }).error(function (data) {
                       var msg = JSON.stringify(data.Message)
                       clearMsgs();
                       $scope.errorMessages.push(msg);
                       $scope.posts = $scope.postsBkp;
                   });
    }

    $scope.updateUser = function () {
        var dn = $scope.user.displayName;
        var old = $scope.OldPassword;
        var pom = $scope.NewPassword;
        var pom2 = $scope.ConfirmPassword;
        if (old) {
            $scope.user.OldPassword = $scope.generatePassHash(old);
        }
        if (pom == pom2) {
            $scope.user.NewPassword = $scope.generatePassHash(pom);
            $http.post(WebApiUrl + 'api/Users', $scope.user)
                       .success(function (data) {
                           clearMsgs();
                           saveToLocalStorage();
                           $scope.updateInformationForm.$setPristine();
                           resetUser();
                           $scope.user.displayName = data.Username;
                           $scope.user.notifyEmail = data.NotifyEmail;
                           $scope.user.notifySMS = data.NotifySMS;
                           $scope.user.phoneNumber = data.PhoneNumber;
                           $scope.successMessages.push(userInfoUpdated);
                           removeSuccessMsg();
                       }).error(function (data, status) {
                           if (status == '403') {
                               $scope.errorMessages.push(resetForbidden);
                               resetUser();
                           }
                           else{
                               var msg = JSON.stringify(data.Message);
                               msg = msg.replace(/\"/g, "");
                               if (msg === 'Old') {
                                   msg = passOldDoesntMatch;
                               }
                               clearMsgs();
                               $scope.errorMessages.push(msg);
                               $scope.user.username = authService.authentication.userName;
                               $scope.updateInformationForm.$setPristine();
                               resetUser();
                           }
                           $scope.user.displayName = dn;
                           removeSuccessMsg();
                       });
        }
        else
        {
            clearMsgs();
            $scope.errorMessages.push(passVerifyError);
            removeSuccessMsg();
        }
    }

    $scope.prePurchase = function (ad) {
        purchaseService.prePurchase(ad);
    };

    $scope.generatePassHash = function (password) {
        var passHash = SHA512(password);
        return passHash;
    }
   
    $scope.categories = [];
    $scope.cities = []

    $scope.loadCategory = function () {
        return $scope.categories.length ? null : $http.get(WebApiUrl + 'api/categories', {
            params: { approved: true }
        })
            .success(function (data) {
                $scope.categories = data;
            });
    };

    $scope.loadCity = function () {
        return $scope.cities.length ? null : $http.get(WebApiUrl + 'api/Cities/Country/Serbia')
            .success(function (data) {
                $scope.cities = data;
            });
    };
   
    function resetUser() {
       $scope.user =  {
            username: authService.authentication.userName,
            displayName: null,
            notifyEmail: null,
            notifySMS: null,
            phoneNumber:null,
            userId: authService.authentication.userId,
            OldPassword: null,
            NewPassword: null,
            ConfirmPassword: null
       };
       $scope.NewPassword = null;
       $scope.OldPassword = null;
       $scope.ConfirmPassword = null;
    }

    function saveToLocalStorage() {
        var authData = localStorageService.get('authorizationData');
        authData.userName = $scope.user.username;
        localStorageService.set('authorizationData', authData);
        authService.authentication.userName = $scope.user.username;
    }

    function removeSuccessMsg() {
        $timeout(function () {
            $scope.successMessages = [];
            $scope.errorMessages = [];
        }, 5000);
    }
});

repairmenControllers.controller('EditMapController', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams,$translate, authService, WebApiUrl, localStorageService) {
    var app = this;
    var id = $routeParams.id;
    var markers = [];
    var adLat = null;
    var adLong = null;
    var ad = null;

    var adUpdated = "adUpdated";

    $rootScope.$on("$translateChangeSuccess", function () {
        translateMsgs();
    });

    function translateMsgs() {

        $translate([
            "INFO_MESSAGES.USER_PANEL.AD_UPDATED"
        ]).then(function (translations) {
            adUpdated = translations['INFO_MESSAGES.USER_PANEL.AD_UPDATED'];
        })
    };

    $scope.transThis = function () {
        translateMsgs();
    }

    $scope.errorMessages = [];
    $scope.successMessages = [];

    function clearMsgs() {
        $scope.errorMessages = [];
        $scope.successMessages = [];
    }

    $scope.getSingleAd = function () {
        $http.get(WebApiUrl + 'api/Ads/' + id, {
            params: { id: id }
        }).success(function (data, status) {
            $scope.post = data;
            ad = data;
            $scope.initializeMap();
            $scope.reloadMap(data.latitude, data.longitude);
            adOwnerId = data.UserId;
        }).error(function (data) {
            clearMsgs();
            var msg = JSON.stringify(data.Message);
            $scope.errorMessages.push(msg);
        });
    }

    $scope.initializeMap = function () {

        var mapProp = {
            center: new google.maps.LatLng(44.1, 21.0667),
            zoom: 7,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

        google.maps.event.addListener(map, 'click', function (event) {
            deleteMarkers();
            placeMarker(event.latLng);
            adLat = event.latLng.lat();
            adLong = event.latLng.lng();
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

    $scope.reloadMap = function (lat, long) {
        map.setZoom(15);
        var location = new google.maps.LatLng( lat, long);
        map.setCenter(location);

        placeMarker(location);
        adLat = lat;
        adLong = long;

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
            });
        }
    }

    $scope.initialiseSingleAd = function () {
        $scope.getSingleAd();
        $scope.transThis();
    };

    function removeSuccessMsg() {
        $timeout(function () {
            $scope.successMessages = [];
        }, 2500);
    }

    $scope.submitData = function () {
        var image = $('#croppedpic').attr('ng-src');
        var oldImage = $('#imgold').attr('src');
        if ((oldImage=="../img/320x150.png" && image == "")||(oldImage != "" && image == "")) {
            ad.ImagePath = oldImage;
        }
        else {
            ad.ImagePath = image;
        }
        //update ad
        ad.longitude = adLong;
        ad.latitude = adLat;   
        $http.put(WebApiUrl + 'api/Ads/UpdateSingleAd/', ad)
               .success(function (data) {
                   $scope.getSingleAd();
                   clearMsgs();
                   $scope.successMessages.push(adUpdated);
                   removeSuccessMsg();
               }).error(function (data) {
                   var msg = JSON.stringify(data.Message)
                   clearMsgs();
                   $scope.errorMessages.push(msg);
         });
    };

    $scope.deleteImage = function () {
        ad.ImagePath = "";
        ad.longitude = adLong;
        ad.latitude = adLat;
        $http.put(WebApiUrl + 'api/Ads/UpdateSingleAd/', ad)
               .success(function (data) {
                   $scope.getSingleAd();
                   clearMsgs();
                   $scope.successMessages.push(adUpdated);
                   removeSuccessMsg();
               }).error(function (data) {
                   var msg = JSON.stringify(data.Message)
                   clearMsgs();
                   $scope.errorMessages.push(msg);
               });
    }

});


//Reset password: verification
repairmenControllers.controller('VerifyPasswordCtrl', function ($scope, $http, $rootScope, $routeParams, $location, $timeout,$translate, authService, WebApiUrl) {
    var id = $routeParams.id;
    var userID = id.substring(32);
    resetUser(userID);
    clearMessages();

    var resetSuccess = "resetSuccess";
    var resetFailed = "resetFeiled";

    $rootScope.$on("$translateChangeSuccess", function () {
        translateMsgs();
    });

    function translateMsgs() {

        $translate([
            "INFO_MESSAGES.RESET_PASSWORD.RESET_SUCCESS",
            "INFO_MESSAGES.RESET_PASSWORD.RESET_ERROR"
        ]).then(function (translations) {
            resetSuccess = translations['INFO_MESSAGES.RESET_PASSWORD.RESET_SUCCESS'];
            resetFailed = translations['INFO_MESSAGES.RESET_PASSWORD.RESET_ERROR'];
        })
    };

    $scope.initMessages = function () {
        translateMsgs();
    };

    function clearMessages() {
        $scope.errorMessages = [];
        $scope.successMessages = [];
    }

    $scope.user.UserId = userID;
    $scope.updateUser = function () {
        $scope.initMessages();
        var pom2 = id;
        var pom = $scope.model.NewPassword;
        if (pom && pom2) {

            $scope.user.OldPassword = $scope.generatePassHash(pom2);
            $scope.user.NewPassword = $scope.generatePassHash(pom);
        }

        $http.post(WebApiUrl + 'api/Users', $scope.user)
                   .success(function (data) {
                       $scope.successMessages = [];
                       $scope.errorMessages = [];
                       $scope.model.NewPassword = null;
                       $scope.model.ConfirmPassword = null;
                       resetUser();
                       $scope.successMessages.push(resetSuccess);
                       redirect();
                   }).error(function (data) {
                       $scope.successMessages = [];
                       $scope.errorMessages = [];
                       resetUser();
                       $scope.updateInformationForm.$setPristine();
                       $scope.errorMessages.push(resetFailed);
                   });
    }

    $scope.generatePassHash = function (password) {
        var passHash = SHA512(password);
        return passHash;
    }
    function resetUser(usrID) {
        $scope.user = {
            username: null,
            displayName: null,
            notifyEmail: null,
            notifySMS: null,
            phoneNumber: null,
            userId:usrID,
            OldPassword: null,
            NewPassword: null,
            ConfirmPassword: null
        };
    }

    function redirect() {
        $timeout(function () {
            $scope.successMessages = [];
            $location.path('/signin');
        }, 3000);
    }
});
