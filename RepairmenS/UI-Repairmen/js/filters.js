var repairmenFilters = angular.module('repairmenFilters', []);

repairmenFilters.filter('checkmark', function () {
    return function (input) {
        return input ? '\u2713' : '\u2718';
    };
});
