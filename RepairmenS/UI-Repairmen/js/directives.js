var repairmenDirectives = angular.module("repairmenDirectives", ['repairmenControllers', 'LocalStorageModule']);

repairmenDirectives.directive("headerNavigation", function () {
    return {
        restrict: 'E',
        templateUrl: 'partials/header-navigation.html'
    };
});

repairmenDirectives.directive("footerNavigation", function () {
    return {
        restrict: 'E',
        templateUrl: 'partials/footer-navigation.html'
    };
});

repairmenDirectives.directive('validEmail', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, model) {

            var EMAIL_REGEXP = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$/;
            var emailValidator = function (value) {
                if (!value || EMAIL_REGEXP.test(value)) {
                    model.$setValidity('email', true);
                    return value;
                } else {
                    model.$setValidity('email', false);
                    return undefined;
                }
               
            };
            model.$parsers.push(emailValidator);
            model.$formatters.push(emailValidator);
        }
    };
});

repairmenDirectives.directive('validWebsite', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, model) {

            var WEBSITE_REGEXP = /^(((http(?:s)?\:\/\/)|www\.)[a-zA-Z0-9\-]+(?:\.[a-zA-Z0-9\-]+)*\.[a-zA-Z]{2,6}(?:\/?|(?:\/[\w\-]+)*)(?:\/?|\/\w+\.[a-zA-Z]{2,4}(?:\?[\w]+\=[\w\-]+)?)?(?:\&[\w]+\=[\w\-]+)*)$/;
            var websiteValidator = function (value) {
                if (!value || WEBSITE_REGEXP.test(value)) {
                    model.$setValidity('website', true);
                    return value;
                } else {
                    model.$setValidity('website', false);
                    return undefined;
                }

            };
            model.$parsers.push(websiteValidator);
            model.$formatters.push(websiteValidator);
        }
    };
});

repairmenDirectives.directive('validPhone', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, model) {

            var PHONE_REGEXP = /^[/+0-9()]{1}[\-_()\/\\ \d]{8,17}$/;
            var phoneValidator = function (value) {
                if (!value || PHONE_REGEXP.test(value)) {
                    model.$setValidity('phone', true);
                    return value;
                } else {
                    model.$setValidity('phone', false);
                    return undefined;
                }

            };
            model.$parsers.push(phoneValidator);
            model.$formatters.push(phoneValidator);
        }
    };
});

repairmenDirectives.directive("passwordVerify", function () {
    return {
        require: "ngModel",
        scope: {
            passwordVerify: '='
        },
        link: function (scope, element, attrs, ctrl) {
            scope.$watch(function () {
                var combined;

                if (scope.passwordVerify || ctrl.$viewValue) {
                    combined = scope.passwordVerify + '_' + ctrl.$viewValue;
                }
                return combined;
            }, function (value) {
                if (value) {
                    ctrl.$parsers.unshift(function (viewValue) {
                        var origin = scope.passwordVerify;
                        if (origin !== viewValue) {
                            ctrl.$setValidity("passwordVerify", false);
                            return undefined;
                        } else {
                            ctrl.$setValidity("passwordVerify", true);
                            return viewValue;
                        }
                    });
                }
            });
        }
    };
});

repairmenDirectives.directive("widgetAddCategory", function () {
    return {
        restrict: 'E',
        templateUrl: 'partials/widgets/widget-add-category.html'
    };
});

repairmenDirectives.directive("widgetErrorMessages", function () {
    return {
        restrict: 'E',
        templateUrl: 'partials/widgets/widget-error-messages.html'
    };
});
repairmenDirectives.directive("widgetSuccessMessages", function () {
    return {
        restrict: 'E',
        templateUrl: 'partials/widgets/widget-success-messages.html'
    };
});
repairmenDirectives.directive("ensureUnique", ['$http', 'WebApiUrl', function ($http, WebApiUrl) {
    return {
        restrict: 'A',
        require: "ngModel",
        link: function (scope, element, attrs, ctrl) {
            scope.$watch(attrs.ngModel, function (value) {
                $http.get(WebApiUrl+'api/Login/UserExists', {
                    params: { email: value }
                }).success(function (data) {
                    ctrl.$setValidity('unique', data.isUnique == "true");                    
                }).error(function () {
                    ctrl.$setValidity('unique', false);
                });
            });
        }
    }
}]);

repairmenDirectives.directive("showIfAuthorizedAs", function (localStorageService) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs, ctrl) {            
            var authData = localStorageService.get('authorizationData');
            if (authData) {
                /* TODO add role in the user's data in local storage (and use instead userName :) )*/
                if (attrs.showIfAuthorizedAs === authData.userName) {
                    element.css('display', 'block');
                }
                else {
                    element.css('display', 'block');
                }
            } 
            else {
                element.css('display', 'none');
            }
        }
    };
});

