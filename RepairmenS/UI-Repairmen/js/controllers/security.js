angular.module('repairmenControllers').controller('UserRegistrationController', function ($scope, $rootScope, $translate, $http) {
    var regstrationSuccess = "registrationSuccess";

    $scope.errorMessages = [];
    $scope.successMessages = [];
    $scope.addUser = function (signup) {

        $http.put('index.html', signup)
            .success(function (data) {
                $scope.people = data;
                $scope.successMessages.push(regstrationSuccess);
                $scope.signup_form.$setPristine();
            }).error(function (data) {
                var msg = JSON.stringify(data.Message)
                $scope.errorMessages.push(msg);
                $scope.signup_form.$setPristine();
            });
    };

    $rootScope.$on("$translateChangeSuccess", function () {
        translateMsgs();
    });

    function translateMsgs() {
        $translate([
            "INFO_MESSAGES.SECURITY.REGISTRATION_SUCCESS"
        ]).then(function (translations) {
            regstrationSuccess = translations['INFO_MESSAGES.SECURITY.REGISTRATION_SUCCESS'];
        });
    };
});

angular.module('repairmenControllers').controller('UserLogInController', ['$http', '$scope', '$rootScope', '$location', '$translate', '$timeout', 'authService', 'returnToUrl', 'WebApiUrl', function ($http, $scope, $rootScope, $location, $translate, $timeout, authService, returnToUrl, WebApiUrl) {

    $scope.userLogged = function()
    {
        if (authService.isLoggedIn()) {
            $location.path('/ads');
        }
    }

    var wrongPass = "wrongPass";
    var wrongUser = "wrongUser";
    var userLocked = "userLocked";
    var resetSendSuccess = "resetSendSuccess";
    var resetSendError = "resetSendError";
    var resetForbidden = "resetForbidden";

    $rootScope.$on("$translateChangeSuccess", function () {
        translateMsg();
    });

    function translateMsg() {
        $translate([
            "INFO_MESSAGES.USER_LOGIN.EMAIL_NOTFOUND",
            "INFO_MESSAGES.USER_LOGIN.WRONG_PASSWORD",
            "INFO_MESSAGES.USER_LOGIN.USER_LOCKED",
            "INFO_MESSAGES.RESET_PASSWORD.SEND_SUCCESS",
            "INFO_MESSAGES.RESET_PASSWORD.SEND_ERROR",
            "INFO_MESSAGES.RESET_PASSWORD.FORBIDDEN"
        ]).then(function (translation) {
            wrongUser = translation['INFO_MESSAGES.USER_LOGIN.EMAIL_NOTFOUND'];
            wrongPass = translation['INFO_MESSAGES.USER_LOGIN.WRONG_PASSWORD'];
            userLocked = translation['INFO_MESSAGES.USER_LOGIN.USER_LOCKED'];
            resetSendSuccess = translation['INFO_MESSAGES.RESET_PASSWORD.SEND_SUCCESS'];
            resetSendError = translation['INFO_MESSAGES.RESET_PASSWORD.SEND_ERROR'];
            resetForbidden = translation['INFO_MESSAGES.RESET_PASSWORD.FORBIDDEN'];
        })
    };

    $scope.initLoginMsg = function () {
        translateMsg();
    };

    $scope.loginData = {
        userName: "",
        password: ""

    };

    $scope.errorMessages = [];
    $scope.successMessages = [];
    $scope.login = function () {

     $scope.myPromise =  authService.login($scope.loginData,$scope).then(function (response) {
            $rootScope.$broadcast('UserLoggedIn');
            console.log(response);
            if (authService.authentication.role === "repairmen") {
                returnToUrl.redirect();
                
            }
            else {
                $rootScope.$broadcast('UserIsAdmin');
                if (returnToUrl.getComment === null) {

                    $location.path('/admin');
                }
                else {
                    returnToUrl.redirect();
                }
            }

        },
         function (err) {
             $scope.errorMessages = [];
             var msg = "";
             switch(err) {
                 case 'Wrong password.':
                     msg = wrongPass;
                     break;
                 case 'The user does not exists.':
                     msg = wrongUser;
                     break;
                 default:
                    msg=userLocked;
             } 
             $scope.errorMessages.push(msg);
         });
    };
    $scope.FBlogin = function () {
        authService.FBlogin().then(function(response) {
            $rootScope.$broadcast('UserLoggedIn');
            returnToUrl.redirect();
        },
         function (err) {
             $scope.errorMessages.push(err);
         });
    };
    //reset password
    $scope.resetPassword = function (user) {
        $scope.$parent.errorMessages = [];
        $scope.$parent.successMessages = [];
        var userModel = {
            Username: 'xxxxxxxx',
            Password: 'xxxxxxxx',
            FirstName: 'xxxxxxxx',
            LastName: 'xxxxxxxx',
            Email: user.email
        }
       $scope.$parent.myPromise = $http.put(WebApiUrl + 'api/Login/resetPassword/', userModel)
            .success(function (data) {
                $scope.$parent.successMessages.push(resetSendSuccess);
                $timeout(function () {
                    $scope.$parent.successMessages = [];
                }, 3000);
            }).error(function (data, status) {
                if (status == '403') {
                    $scope.$parent.errorMessages.push(resetForbidden);
                }
                else {
                    $scope.$parent.errorMessages.push(resetSendError);
                }
                $timeout(function () {
                    $scope.$parent.errorMessages = [];
                }, 5000);
            });
    }




}]);

angular.module('repairmenControllers').controller('UserSignUpController', function ($scope,$rootScope, $translate, $http, $location, $timeout, WebApiUrl) {
 
    $scope.errorMessages = [];
    $scope.successMessages = [];
    var clearMsgs = function()
    {
        $scope.errorMessages = [];
        $scope.successMessages = [];
    }
    var regSuccess = "emailVerification";
    var passVerifyError = "passVerifyError";

    $rootScope.$on("$translateChangeSuccess", function () {
        translateMsgs();
    });
    
    function translateMsgs() {
        $translate([
            "INFO_MESSAGES.SECURITY.EMAIL_VERIFICATION",
            "INFO_MESSAGES.SECURITY.PASSWORD_VERIFY"
        ]).then(function (translations) {
            regSuccess = translations['INFO_MESSAGES.SECURITY.EMAIL_VERIFICATION'];
            passVerifyError = translations['INFO_MESSAGES.SECURITY.PASSWORD_VERIFY'];
        });
    };

    $scope.initSignUpMsg = function () {
        translateMsgs();
    };

    $scope.addUser = function (signup) {
        var pom = $scope.Password;
        var pom2 = $scope.passwordVerify;
        if (pom == pom2) {
            var name = SHA512(pom);
            $scope.signup.Password = name;
            $scope.signup.RoleId = "56dc86b4-1229-e411-9417-a41f7255f9b5"
            $http.put(WebApiUrl + 'api/login', signup)
                .success(function (data) {
                    $scope.signup.Password = pom;
                    clearMsgs();
                    $scope.successMessages.push(regSuccess);
                    $timeout(function () {
                        clearMsgs();
                        $location.path('/signin');
                    }, 3000);

                }).error(function (data) {
                    $scope.signup.Password = pom;
                    clearMsgs();
                    var msg = JSON.stringify(data.Message);
                    $scope.errorMessages.push(msg);
                    $timeout(function () {
                        clearMsgs();
                    }, 5000);
                });
        }
        else
        {
            clearMsgs();
            $scope.errorMessages.push(passVerifyError);
            $timeout(function () {
                clearMsgs();
            }, 5000);
        }
    }

    $scope.generatePassHash = function () {
        var data = $scope.signup.Password;
        var passHash = SHA512(data);
        return passHash;
    }

    var clearFields = function () {
        $scope.signup.PhoneNumber = "";
        $scope.signup.FirstName = "";
        $scope.signup.LastName = "";
        $scope.signup.Username = "";
        $scope.signup.Email = "";
        $scope.Password = "";
        $scope.passwordVerify = "";
    }
});

angular.module('repairmenControllers').controller('MainController', function ($scope, $location, authService, localStorageService) {
    var authData = localStorageService.get('authorizationData');
    $scope.userLoggedIn = false;
    $scope.userIsAdmin = false;
    if (authData) {
        $scope.userLoggedIn = true;
        $scope.userPage = authService.authentication.userName;
        if (authData.Role === "admin")
            $scope.userIsAdmin = true;
    }

    $scope.logOut = function () {
        authService.logOut();
        $scope.$emit('UserLoggedOut');
        $scope.$emit('UserIsntAdmin');
        $location.path('/');

    }

    $scope.authentication = authService.authentication;

    $scope.$on('UserLoggedIn', function () {
        $scope.userPage = authService.authentication.userName;
        $scope.userLoggedIn = true;
    });

    $scope.$on('UserLoggedOut', function () {
        $scope.userPage = "My Page";
        $scope.userLoggedIn = false;
    });

    $scope.$on('UserIsAdmin', function () {
        $scope.userPage = authService.authentication.userName;
        $scope.userIsAdmin = true;
    });

    $scope.$on('UserIsntAdmin', function () {
        $scope.userIsAdmin = false;
    });

    $scope.userId = authService.authentication.userId;
});

// Google plus login procedure:
angular.module('repairmenControllers').controller('GoogleCtrl', ['$scope', '$rootScope', '$http', '$location', 'localStorageService', 'authService', 'GooglePlus', 'returnToUrl', 'WebApiUrl', function ($scope, $rootScope, $http, $location, localStorageService, authService, GooglePlus, returnToUrl, WebApiUrl) {
    $scope.login = function () {
        GooglePlus.login().then(function (authResult) {
            GooglePlus.getUser().then(function (user) {
                // IMPLEMENT AJAX CALL AND LocalStorage things....
                var un = {
                    firstName: user.name.givenName,
                    lastName: user.name.familyName,
                    email: user.emails[0].value
                }
                var unString = JSON.stringify(un);
                var data = "grant_type=password&username=" + unString + "&password=Gplus";
                $http.post(WebApiUrl+'Token', data, {
                           headers:
                       { 'Content-Type': 'application/x-www-form-urlencoded' }
                       }).success(function (tokenResponse) {
                           $http.get(WebApiUrl + 'api/Users/user', { params: { username: user.emails[0].value } }).
                                        success(function (userData) {
                                            localStorageService.set('authorizationData', { token: tokenResponse.access_token, userId: userData.Id, userName: userData.Email, Role: tokenResponse.Role });
                                            authService.authentication.isAuth = true;
                                            authService.authentication.userName = userData.Email;
                                            authService.authentication.userId = userData.Id;
                                            authService.authentication.role = tokenResponse.Role;
                                            $rootScope.$broadcast('UserLoggedIn');
                                            returnToUrl.redirect();
                                        }).error(function (userData) {

                                        });
                       }).error(function (userData) {
                                  var msg = JSON.stringify(userData.Message);
                                   $scope.errorMessages.push(msg);
                       });

            });
        }, function (err) {
            console.log(err);
        });
    };
}])
