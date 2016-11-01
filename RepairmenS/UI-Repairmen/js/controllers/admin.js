repairmenControllers.controller('AdminController', function ($scope, $rootScope, $translate, $http, $location, $timeout, authService, WebApiUrl) {
    $scope.adminLogged = function () {
        if (authService.isAdmin() == false) {
            $location.path('/ads');
        }
    }



    var app = this;
    var categoryErAmbiguos = "categoryErAmbiguos",
        categoryChangesSaved = "categoryChangesSaved",
        commentErAmbiguos = "commentErAmbiguos",
        commentChangesSaved = "commentChangesSaved",
        adErAmbiguos = "adErAmbiguos",
        adChangesSaved = "adChangesSaved";

    $scope.initialiseAdmin = function () {
        $scope.adminLogged();
        getCategories();
        getAds();
        getComments();
        translateMsgs();
    };

    $scope.description = [];

    $scope.showDescription = []; //comment descriptions
    $scope.showAdDescriptions = [];

    $scope.descriptionsAd = [];

    clearMessages();

    function clearMessages() {
        $scope.errorMessages = [];
        $scope.successMessages = [];
    }



    $rootScope.$on("$translateChangeSuccess", function () {
        translateMsgs();
    });
    
    function translateMsgs() {

        $translate([
            "INFO_MESSAGES.ADMIN.CATEGORY_ER_AMB",
            "INFO_MESSAGES.ADMIN.CATEGORY_CHANGES_SAVED",
            "INFO_MESSAGES.ADMIN.COMMENT_ER_AMB",
            "INFO_MESSAGES.ADMIN.COMMENT_CHANGES_SAVED",
            "INFO_MESSAGES.ADMIN.AD_ER_AMB",
            "INFO_MESSAGES.ADMIN.AD_CHANGES_SAVED"
        ]).then(function (translations) {
            categoryErAmbiguos = translations['INFO_MESSAGES.ADMIN.CATEGORY_ER_AMB'];
            categoryChangesSaved = translations['INFO_MESSAGES.ADMIN.CATEGORY_CHANGES_SAVED'];
            commentErAmbiguos = translations['INFO_MESSAGES.ADMIN.COMMENT_ER_AMB'];
            commentChangesSaved = translations['INFO_MESSAGES.ADMIN.COMMENT_CHANGES_SAVED'];
            adErAmbiguos = translations['INFO_MESSAGES.ADMIN.AD_ER_AMB'];
            adChangesSaved = translations['INFO_MESSAGES.ADMIN.AD_CHANGES_SAVED'];
        });
    };

   

    $scope.addCategory = function (categories) {
        clearMessages();
        var categoriesToPost = [];
        categories.forEach(function (entry) {

            if (entry.Delete && entry.Approved) {
                var msg = categoryErAmbiguos;
                $scope.errorMessages.push(msg);
                return;
            }

            if (entry.Delete || entry.Approved) {
                categoriesToPost.push(entry);
            }

        });

        if (categoriesToPost.length > 0) {
            $http.post(WebApiUrl+'api/categories', categoriesToPost)
            .success(function (data) {
                $scope.category = data;
                $scope.successMessages.push(categoryChangesSaved);
                getCategories();
                removeMsgs();
            }).error(function (data) {
                var msg = JSON.stringify(data.Message);
                $scope.errorMessages.push(msg);
                removeMsgs();
            });
        }
    }

    $scope.postComments = function (comments) {
        var commentsToPost = [];
        clearMessages();
        comments.forEach(function (entry) {
            commentsToPost.push(entry);
        });

        if (commentsToPost.length > 0) {
            $http.post(WebApiUrl+'api/Comments', commentsToPost)
                .success(function (data) {
                    $scope.successMessages.push(commentChangesSaved);
                    getComments();
                    removeMsgs();
                }).error(function (data) {
                    var msg = JSON.stringify(data.Message);
                    $scope.errorMessages.push(msg);
                    removeMsgs();
                }); 
        }
    }

    $scope.getDescriptions = function (commentId, index) {
        if ($scope.showDescription[index]) {
            return;
        }

        $http.get(WebApiUrl+'api/Comments/Inappropriate/' + commentId)
            .success(function (data) {
                $scope.description[index] = data;
            }).error(function (data) {
                var msg = JSON.stringify(data.Message)
                $scope.errorMessages.push(msg);
            });
    }

    $scope.postAds = function (ads) {
        clearMessages();
        var adsToPost = [];

        ads.forEach(function (entry) {
            if (entry.Delete && entry.Approved) {
                var msg = adErAmbiguos;
                $scope.errorMessages.push(msg);
                return;
            }


            if (entry.Delete || entry.Approved) {
                adsToPost.push(entry);
            }

        });

        if (adsToPost.length > 0) {
            $http.post(WebApiUrl+'api/Ads/Inappropriate', adsToPost)
                .success(function (data) {
                    $scope.successMessages.push(adChangesSaved);
                    getAds();
                    removeMsgs();
                }).error(function (data) {
                    var msg = JSON.stringify(data.Message);
                    $scope.errorMessages.push(msg);
                    removeMsgs();
                });
        }
    }

    $scope.getAdDescriptions = function (adId, index) {
        if ($scope.showAdDescriptions[index]) {
            return;
        }

        $http.get(WebApiUrl+'api/Ads/Inappropriate/' + adId)
            .success(function (data) {
                $scope.descriptionsAd[index] = data;
            }).error(function (data) {
                var msg = JSON.stringify(data.Message)
                $scope.errorMessages.push(msg);
            });
    }

    function getCategories() {
        $http.get(WebApiUrl+'api/categories', {
            params: { approved: false }
        })
            .success(function (data, status) {
                $scope.categories = data;
            }).error(function (data) {
                $scope.categories = {};
                //var msg = JSON.stringify(data.Message);
                //$scope.errorMessages.push(msg);
            });
    }

    function getAds() {
        $http.get(WebApiUrl+'api/Ads/Inappropriate')
            .success(function (data) {
                $scope.ads = data;
            }).error(function (data) {
                $scope.ads = {};
                //var msg = JSON.stringify(data.ModelState);
                //$scope.errorMessages.push(msg);
            })
    }

    function getComments() {
        $http.get(WebApiUrl+'api/Comments/Inappropriate')
            .success(function (data) {
                $scope.comments = data;
            }).error(function (data) {
                $scope.comments = {};
                //var msg = JSON.stringify(data.Message);
                //$scope.errorMessages.push(msg);
            });
    }

    // when checkbox is checked, put their sibling to be unchacked and change appropriate values
    $scope.doCheck = function (event, index, tip) {
        var thisElem = $(event.target);
        var parent = $(event.target).parent();
        var thisId = $(event.target).attr("id");
        var rootName = thisId.substr(0, thisId.indexOf("-"));
        var newElemId = "";
        if (rootName == "aprove") {
            newElemId = "#delete-" + index;
        }
        else {
            newElemId = "#aprove-" + index;
        }

        var sibling = parent.find(newElemId);
        if (thisElem.is(":checked")) {
            thisElem.prop('checked', true);
            sibling.prop('checked', false);
            if(rootName == "aprove")
            {
                tip.Approved = true;
                tip.Delete = false;
            }
            else
            {
                tip.Delete = true;
                tip.Approved = false;
            }
            
        }
        else
        {
            thisElem.prop('checked', false);
            sibling.prop('checked', true);
            if (rootName == "aprove") {
                tip.Approved = true;
                tip.Delete = false;
            }
            else {
                tip.Delete = true;
                tip.Approved = false;
            }
        }
    }

    function removeMsgs() {
        $timeout(function () {
            $scope.successMessages = [];
            $scope.errorMessages = [];
        }, 4000);
    }


});