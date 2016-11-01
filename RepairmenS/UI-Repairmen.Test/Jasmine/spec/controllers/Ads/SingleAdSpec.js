describe("Controller group: Ads", function () {

    beforeEach(module("repairmenControllers"));

    describe("Controller: SingleAdController", function () {

        var $scope,
            ngDialog,
            WebApiUrl = {},
            singleAddController,
            errorMessage = "error",
            mockRemoveSuccessMessages,
            $routeParams = {
                id: 1
            },
            _returnData = {
                url: "",
                comment: ""
            };

        beforeEach(function () {

            mockRemoveSuccessMessages = function () { };

            module(function ($provide) {
                $provide.value('removeSuccessMsg', mockRemoveSuccessMessages);
            });
        });

        beforeEach(inject(function ($rootScope, $controller, $timeout, $http, $location) {

            $scope = $rootScope.$new();

            authService = {
                authentication: {
                    userId: "userid"
                },
                isLoggedIn: function () {
                    return true;
                }
            };

            returnToUrl = {
                getComment: function () {
                    return _returnData.comment;
                },
                clearData: function () {
                    _returnData = {};
                },
                storeData: function () {
                    _returnData.url = "";
                    _returnData.comment = "";
                },
                getType: function () {
                    return "x";
                }
            };

            ngDialog = {
                openConfirm: function () { return "Hello!"; }
            };

            singleAddController = $controller("SingleAdController", {
                "$scope": $scope,
                "$http": $http,
                "$routeParams": $routeParams,
                "authService": authService,
                "WebApiUrl": WebApiUrl,
                "returnToUrl": returnToUrl,
                "$location": $location,
                "$timeout": $timeout,
                "ngDialog": ngDialog,
                "$translate": {}
            });
        }));

        it("should be defined", function () {
            expect(singleAddController).toBeDefined();
        });

        it("should have no messages", function () {
            expect($scope.errorMessages).not.toContain();
            expect($scope.successMessages).not.toContain();
        });

        describe("$http tests", function () {

            var $httpBackend,
                $location,
                mockInterceptor,
                id = $routeParams.id;

            beforeEach(inject(function (_$httpBackend_, _$location_) {
                $httpBackend = _$httpBackend_;
                $location = _$location_;

                mockInterceptor = function (statusCode) {
                    if (statusCode === 401)
                        $location.path("/signin");
                };
            }));

            it("should get a single ad", function () {
                
                var data = {
                    UserId: 12321,
                    ad: "Sekac"
                },
                commentData = {
                    comment: "test"
                };

                var spy = spyOn($scope, "initializeMap")

                $httpBackend.when("GET", WebApiUrl + 'api/Ads/' + id + "?id=" + id)
                    .respond(function () {
                        return [200, data, {}];
                    });

                $httpBackend.when("GET", WebApiUrl + "api/Users/" + data.UserId)
                    .respond(function () {
                        return [200, {}, {}];
                    });

                $httpBackend.when("GET", WebApiUrl + "api/Comments/AdId/" + id)
                    .respond(function () {
                        return [200, commentData, {}];
                    });

                $scope.getSingleAd();
                $httpBackend.flush();

                expect($scope.post).toEqual(data);
            });

            xit("should get a single ad, but fail getting its comments", function () {

                var
                    data = {
                        ad: "Sekac"
                    }, commentData = {
                        comment: "test",
                        Message: errorMessage
                    };

                var spy = spyOn($scope, "initializeMap")

                $httpBackend.when("GET", WebApiUrl + 'api/Ads/' + id + "?id=" + id)
                    .respond(function () {
                        return [200, data, {}];
                    });

                $httpBackend.when("GET", WebApiUrl + "api/Users/" + data.UserId)
                    .respond(function () {
                        return [200, {}, {}];
                    });

                $httpBackend.when("GET", WebApiUrl + "api/Comments/AdId/" + id)
                    .respond(function () {
                        return [500, commentData, {}];
                    });

                $scope.getSingleAd();
                $httpBackend.flush();

                expect($scope.post).toEqual(data);
                expect($scope.errorMessages).toContain(JSON.stringify(errorMessage));
            });

            it("should fail getting a single ad", function () {

                var data = {
                    Message: errorMessage
                };

                $httpBackend.when("GET", WebApiUrl + 'api/Ads/' + id + "?id=" + id)
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.getSingleAd();
                $httpBackend.flush();

                expect($scope.errorMessages).toContain(JSON.stringify(errorMessage));
            });

            xit("should add a comment", function () {

                var
                    data = {
                        Message: errorMessage
                    },
                    comment = {
                        adId: id,
                        userId: 1,
                        text: "Comment!"
                    };

                $scope.addCommentForm = {
                    $valid: true,
                    $setPristine: function () { }
                };

                $httpBackend.when("PUT", WebApiUrl + "api/Comments", comment)
                    .respond(function () {
                        return [200, data, {}];
                    });

                $httpBackend.when("GET", WebApiUrl + "api/Comments/AdId/" + id)
                    .respond(function () {
                        return [200, data, {}];
                    });

                $scope.addComment(comment);
                $httpBackend.flush();

                expect($scope.successMessages).toContain("commentAdded");
            });

            it("should redirect anonymus user to login page when commenting", function () {

                var $location;

                inject(function (_$location_) {
                    $location = _$location_;
                });

                var data = {},
                    comment = {
                    adId: id,
                    text: "Comment!"
                };

                $httpBackend.when("PUT", WebApiUrl + "api/Comments", comment)
                    .respond(function () {
                        return [401, data, {}];
                    });

                $scope.addComment(comment);
                mockInterceptor(401);
                $httpBackend.flush();

                expect($location.path()).toEqual("/signin");
            });

            it("should fail adding comment", function () {

                var data = {
                    Message: errorMessage
                },
                    comment = {
                    adId: id,
                    text: "Comment!"
                    };

                $httpBackend.when("PUT", WebApiUrl + "api/Comments", comment)
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.addComment(comment);
                $httpBackend.flush();

                expect($scope.errorMessages).toContain(JSON.stringify(errorMessage));
            });

            it("should flag comment as inappropriate", function () {

                var data = {};

                $httpBackend.when("PUT", WebApiUrl + "api/Comments/Inappropriate")
                    .respond(function () {
                        return [200, data, {}];
                    });

                $scope.addInnappropriateComment("test", 1, 1);
                $httpBackend.flush();

                expect($scope.successMessages).toContain("commentReported");
            });

            it("should redirect anonymus user to login page when flagging", function () {

                var data = {},
                    $location;

                inject(function (_$location_) {
                    $location = _$location_;
                });

                $httpBackend.when("PUT", WebApiUrl + "api/Comments/Inappropriate")
                    .respond(function () {
                        return [401, data, {}];
                    });

                $scope.addInnappropriateComment("test", 1, 1);
                mockInterceptor(401);
                $httpBackend.flush();

                expect($location.path()).toEqual("/signin");
            });

            it("should fail flagging comment as inappropriate", function () {

                var data = {
                    Message: errorMessage
                };

                $httpBackend.when("PUT", WebApiUrl + "api/Comments/Inappropriate")
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.addInnappropriateComment("test", 1, 1);
                $httpBackend.flush();

                expect($scope.errorMessages).toContain(JSON.stringify(errorMessage));
            });

            it("should add vote", function () {

                var commentId = 1,
                    vote = 3,
                    index = 0;

                var data = {
                    PositiveVote: {
                        value: "plus"
                    },
                    NegativeVote: {
                        value: "minus"
                    }
                };

                $scope.comments = [
                    { PositiveVote: {}, NegativeVote: {} },
                    { PositiveVote: {}, NegativeVote: {} },
                    { PositiveVote: {}, NegativeVote: {} },
                ];

                $httpBackend.when("PUT", WebApiUrl + "api/Comments/Vote")
                    .respond(function () {
                        return [200, {}, {}];
                    });

                $httpBackend.when("GET", WebApiUrl + "api/Comments/Id/" + commentId)
                    .respond(function () {
                        return [200, data, {}];
                    });

                $scope.addVote(commentId, vote, index);
                $httpBackend.flush();

                expect($scope.comments[index]).toEqual(data);
            });

            it("should vote comment, but fail retrieving it", function () {

                var commentId = 1,
                    vote = 3,
                    index = 0;

                var data = {
                    Message: errorMessage
                };

                $httpBackend.when("PUT", WebApiUrl + "api/Comments/Vote")
                    .respond(function () {
                        return [200, {}, {}];
                    });

                $httpBackend.when("GET", WebApiUrl + "api/Comments/Id/" + commentId)
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.addVote(commentId, vote, index);
                $httpBackend.flush();

                expect($scope.errorMessages).toContain(JSON.stringify(errorMessage));
            });

            it("should redirect anonimus user to login page when voting", function () {

                var $location,
                    commentId = 1,
                    vote = 3,
                    index = 0;

                var data = {
                    Message: errorMessage
                };

                inject(function (_$location_) {
                    $location = _$location_;
                });

                $httpBackend.when("PUT", WebApiUrl + "api/Comments/Vote")
                    .respond(function () {
                        return [401, data, {}];
                    });

                $scope.addVote(commentId, vote, index);
                mockInterceptor(401);
                $httpBackend.flush();

                expect($location.path()).toEqual("/signin");
            });

            it("should fail voting comment", function () {
                
                var commentId = 1,
                    vote = 3,
                    index = 0;

                var data = {
                    Message: errorMessage
                };

                $httpBackend.when("PUT", WebApiUrl + "api/Comments/Vote")
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.addVote(commentId, vote, index);
                $httpBackend.flush();

                expect($scope.errorMessages).toContain(JSON.stringify(errorMessage));
            });

            it("should successfully send an eMail notification", function () {

                var
                    data = {
                        Message: errorMessage
                    },
                    deferred;

                inject(function (_$q_) {
                    deferred = _$q_.defer();
                });

                deferred.resolve('value');
                spyOn(ngDialog, "openConfirm").and.returnValue(deferred.promise);

                $httpBackend.when("PUT", WebApiUrl + "api/Users/Send")
                    .respond(function () {
                        return [200, data, {}];
                    });

                $scope.clickToOpen();
                $httpBackend.flush();

                expect($scope.successMessages).toContain("Mail sent!");
            });

            it("should fail sending an eMail notification", function () {

                var
                    data = {
                        Message: errorMessage
                    },
                    deferred;

                inject(function (_$q_) {
                    deferred = _$q_.defer();
                });

                deferred.resolve('value');
                spyOn(ngDialog, "openConfirm").and.returnValue(deferred.promise);

                $httpBackend.when("PUT", WebApiUrl + "api/Users/Send")
                    .respond(function () {
                        return [500, data, {}];
                    });

                $scope.clickToOpen();
                $httpBackend.flush();

                expect($scope.errorMessages).toContain(JSON.stringify(errorMessage));
            });

            it("should reject sending promise", function () {

                var deferred;

                inject(function (_$q_) {
                    deferred = _$q_.defer();
                });

                deferred.reject('value');
                var spy = spyOn(ngDialog, "openConfirm").and.returnValue(deferred.promise);

                $scope.clickToOpen();

                expect(spy).toHaveBeenCalled();
            });
        });
    });
});