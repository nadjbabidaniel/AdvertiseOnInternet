'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService', 'WebApiUrl', function ($http, $q, localStorageService, WebApiUrl) {
    var authServiceFactory = {};
    var serviceBase = WebApiUrl;

    var _authentication = {
        isAuth: false,
        userName: "",
        userId: "",
        role:""
    };

    var _saveRegistration = function (registration) {

        _logOut();

        return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };
    var _FBlogin = function () {
        var deferred = $q.defer();
       
                FB.login(function (response) {
                    if (response.authResponse) {
                        console.log('Welcome!  Fetching your information.... ');
                        var accessToken = response.authResponse.accessToken;
                        var data = "grant_type=password&username=" + accessToken + "&password=FB";
                        $http.post(serviceBase + 'Token', data, {
                            headers:
                        { 'Content-Type': 'application/x-www-form-urlencoded' }
                        }).success(function (tokenResponse) {
                            $http.get(WebApiUrl + 'api/Users/facebook', { params: { accessToken: accessToken } }).
                             success(function (userData) {

                                 localStorageService.set('authorizationData', { token: tokenResponse.access_token, userName: userData.userName, userId: userData.Id, Role: tokenResponse.Role });

                                 _authentication.isAuth = true;
                                 _authentication.userName = userData.Email;
                                 _authentication.userId = userData.Id;
                                 _authentication.role = tokenResponse.Role;
                                 deferred.resolve(tokenResponse);
                             }).error(function (userData) {
                                 FB.logout();
                                 var msg = JSON.stringify(userData.Message);
                                 $scope.errorMessages.push(msg);
                             });
                        })
                            .error(function (data) {
                                //FB.logout();
                                _logOut();
                                alert(WebApiUrl);
                                deferred.reject(err);
                            });
                    }
                    else {
                        console.log('User cancelled login or did not fully authorize.');
                    }
                }, { scope: 'public_profile,email' });
            
        return deferred.promise;
    };
    var _login = function (loginData, $scope) {
        if (loginData.password) {
            var hashedPassword = SHA512(loginData.password);
            var backendRndm;
            var deferred = $q.defer();

           $http.get(WebApiUrl + 'api/login/initiate', { params: { username: loginData.userName } }).success(function (data) {
                backendRndm = data.RndSrv;

                for (var frontendRndm = ''; frontendRndm.length < 32;) {
                    frontendRndm += Math.random().toString(36).substr(2, 1);
                }
                var finalHash = SHA512(frontendRndm + hashedPassword + backendRndm);
                var data = "grant_type=password&username=" + loginData.userName + "&password=" + frontendRndm + finalHash;
                $http.post(serviceBase + 'Token', data, {
                    headers:
                { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (response) {

                    $http.get(WebApiUrl + 'api/Users/user', { params: { username: loginData.userName } }).
                    success(function (userData) {

                        localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, userId: userData.Id, Role: response.Role });

                        _authentication.isAuth = true;
                        _authentication.userName = loginData.userName;
                        _authentication.userId = userData.Id;
                        _authentication.role = response.Role;
                        _authentication.displayName = userData.Username;
                    
                        deferred.resolve(response);
                        
                    }).error(function (userData) {
                        var msg = JSON.stringify(userData.Message);
                        //$scope.errorMessages.push(msg);
                    });                    

                }).error(function (err, status) {
                    _logOut();
                    var msg = JSON.stringify(err.error_description);
                    msg = msg.replace(/\"/g, "");
                    deferred.reject(msg);
                });
            }).error(function (data) {
                var msg = JSON.stringify(data.ModelState);
                deferred.reject(err);
            });
           return deferred.promise;
        }

    };

    var _logOut = function () {

        localStorageService.remove('authorizationData');

        _authentication.isAuth = false;
        _authentication.userName = "";
        _authentication.userId = "";
        _authentication.role = "";

    };

    var _fillAuthData = function () {
        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
            _authentication.userId = authData.userId;
            _authentication.role = authData.Role;
            _authentication.displayName = authData.displayName;
        }

    }

    var _isLoggedIn = function () {
        return _authentication.isAuth;
    };

    var _isAdmin = function () {
        if(_authentication.role == "admin")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    authServiceFactory.FBlogin = _FBlogin;
    authServiceFactory.isLoggedIn = _isLoggedIn;
    authServiceFactory.isAdmin = _isAdmin;
    return authServiceFactory;
}]);

app.factory('authInterceptorService', ['$q', '$location', 'localStorageService', function ($q, $location, localStorageService) {

    var authInterceptorServiceFactory = {};

    var _request = function (config) {

        config.headers = config.headers || {};

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token;
        }

        return config;
    }

    var _responseError = function (rejection) {

        if (rejection.status === 401) {
            $location.path('/signin');
        }
        return $q.reject(rejection);
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}]);

app.factory('clearMessages', ['$q', '$location', 'localStorageService', function ($q, $location, localStorageService) {

    var authInterceptorServiceFactory = {};

    var _request = function (config) {

        config.headers = config.headers || {};

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token;
        }

        return config;
    }

    var _responseError = function (rejection) {
        if (rejection.status === 401) {
            $location.path('/signin');
        }
        return $q.reject(rejection);
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}]);

app.factory('returnToUrl', ['$location', function ($location) {

    var returnToUrlFactory = {};

    var _returnData = {
        url: "",
        comment: "",
        type: "",
        index:""
    };

    var _getComment = function () {
        return _returnData.comment;
    };

    var _storeData = function (comment,type,index) {
        _returnData.url = $location.path();
        _returnData.comment = comment;
        _returnData.type = type;
        _returnData.index = index;
    };

    var _redirect = function () {
        if (_returnData.url)
            $location.path(_returnData.url);
        else
            $location.path('/ads');

        _returnData.url = "";
    };

    var _getType = function () {
        return _returnData.type;

    };

    var _getIndex = function () {
        return _returnData.index;

    };

    var _clearData = function () {
        _returnData = {};
    };

    returnToUrlFactory.getComment = _getComment;
    returnToUrlFactory.storeData = _storeData;
    returnToUrlFactory.redirect = _redirect;
    returnToUrlFactory.clearData = _clearData;
    returnToUrlFactory.getType = _getType;
    returnToUrlFactory.getIndex = _getIndex;

    return returnToUrlFactory;
}]);

app.factory("purchaseService", ["$http", "$location", "WebApiUrl", function ($http, $location, WebApiUrl) {

    var purchaseServiceFactory = {};

    var _adData = {
        id: "",
        name: "",
        isAvailable: false
    };

    var _getData = function () {
        return _adData;
    };

    var _setPurchased = function () {
        _adData.isAvailable = false;
    };

    function _prePurchase(ad) {

        _adData.id = ad.Id;
        _adData.name = ad.Name;

        var canPurchase;

        $http.get(WebApiUrl + "api/PayPal/IsAvailable",
            {
                params: {
                    adId: ad.Id,
                    yesNo: true
                }
            }).then(function (result) {
                _adData.isAvailable = result.data;
                $location.path("/paypal");
            });
    };

    purchaseServiceFactory.setPurchased = _setPurchased;
    purchaseServiceFactory.getData = _getData;
    purchaseServiceFactory.prePurchase = _prePurchase;

    return purchaseServiceFactory;
}]);

app.factory('debugInterceptorService', [ '$q', function ($q) {

    var debugInterceptorFactory = {};
    var printResponse = "";

    var _request = function (config) {

        //if (config.params != null) {

            //console.clear();
            //console.log("Method: " + config.method);
            //console.log("Url: " + config.url);
            //console.log("Params:");
            //console.dir(config.params);
            //console.log("----------------------------------------");
       // }

        return config;
    };

    //var _response = function (response) {

    //    if (response.url == printResponse) {
    //        console.log("Response:");
    //        console.log(JSON.stringify(response, null, '/t'));

    //        printResponse = "";
    //    }

    //    return response;
    //};

    debugInterceptorFactory.request = _request;
    //debugInterceptorFactory.response = _response;

    return debugInterceptorFactory;
}]);