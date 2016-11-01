repairmenControllers.controller("l10nController", function ($scope, $translate) {
    $scope.changeLanguage = function (langKey) {
        $translate.use(langKey);
    };
});