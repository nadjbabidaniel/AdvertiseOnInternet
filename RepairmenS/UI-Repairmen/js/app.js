var app = angular.module("app", ['ngRoute', 'repairmenControllers', 'repairmenDirectives', 'repairmenFilters', 'LocalStorageModule', 'ui.bootstrap', 'xeditable', 'ngImgCrop', 'pascalprecht.translate', 'ngCookies', 'ngDialog', 'googleplus', 'cgBusy','ngSocial']);

app.constant('WebApiUrl','http://localhost:58742/');

app.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/ads', {
            templateUrl: 'partials/available-ads.html',
            controller: 'AdGetController' 
        }).  
        when('/ads/:id', {
            templateUrl: 'partials/single-ad.html',
            controller: 'SingleAdController'
        }).
        when('/login/activate/:id', {
            templateUrl: 'partials/activate.html',
            controller: 'ActivateController'
        }).
        when('/login/updatePassword/:id', {
            templateUrl: 'partials/reset-password.html',
            controller: 'VerifyPasswordCtrl'
        }).
        when('/user-panel', {
            templateUrl: 'partials/user-panel.html',
            controller: 'UserPanelController'
        }).
        when('/new-category', {
            templateUrl: 'partials/widgets/widget-add-category.html',
            controller: 'AddCategoryController'
        }).
        when('/new-ad', {
            templateUrl: 'partials/ad-registration-form.html',
            controller: 'AdRegistrationController'
        }).
        when('/admin', {
            templateUrl: 'partials/administration.html',
            controller: 'AdminController'
        }).
        when('/signup', {
            templateUrl: 'partials/signup.html',
            controller: 'UserSignUpController'
        }).
        when('/signin', {
            templateUrl: 'partials/signin.html',
            controller: 'UserLogInController'
        }).
        when('/paypal', {
            templateUrl: 'partials/paypal-confirmation-page.html',
            controller: 'PayPalController'
        }).
        when('/paypal/success', {
            templateUrl: 'partials/paypal-success.html',
            controller: 'PayPalSuccessController'
        }).
        when('/paypal/cancel', {
            templateUrl: 'partials/paypal-cancel.html',
            controller: 'PayPalController'
        }).
        when('/edit/:id', {
              templateUrl: 'partials/EditAd.html',
              controller: 'EditMapController'
         }).
        otherwise({
            redirectTo: '/ads'
        });
  }]);

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);

app.run(function (editableOptions) {
    editableOptions.theme = 'bs3'; // bootstrap3 theme. Can be also 'bs2', 'default'
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

//configuration for ng-dialog: setting defaults values
app.config(['ngDialogProvider', function (ngDialogProvider) {
    ngDialogProvider.setDefaults({
        className: 'ngdialog-theme-default',
        plain: false,
        showClose: true,
        closeByDocument: true,
        closeByEscape: true,
        appendTo: false,
        preCloseCallback: function () {
            console.log('default pre-close callback');
        }
    });
}]);

// configuration for G+:
app.config(['GooglePlusProvider', function (GooglePlusProvider) {
    GooglePlusProvider.init({
        clientId: '112900437292-pq5f7nmtk9rbfim2vqf5e3vdik6kd38d.apps.googleusercontent.com',
        apiKey: 'AIzaSyASpZnrJgXCBJKRcq9GeeVn9_CO1J6V8uI'
    });
}]);

app.config(function ($translateProvider) {

    $translateProvider.translations('en', translationTable.en);
    $translateProvider.translations('sr', translationTable.sr);
    $translateProvider.preferredLanguage('en');

    $translateProvider.useLocalStorage();
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('debugInterceptorService');
});

app.value('cgBusyDefaults', {
    message: 'Loading...',
    backdrop: true
});

var repairmenControllers = angular.module('repairmenControllers', []);

window.fbAsyncInit = function () {
    FB.init({
        appId: '606741896102084',
        xfbml: true,
        version: 'v2.1',
        oauth: true
    });
};

(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement(s); js.id = id;
    js.src = "//connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

