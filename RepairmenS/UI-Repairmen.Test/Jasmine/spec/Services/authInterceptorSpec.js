describe('Factory: authInterceptorService', function () {

    var authInterceptor,
        $location;

    beforeEach(module("app"));

    beforeEach(inject(function (authInterceptorService, _$location_) {
        authInterceptor = authInterceptorService;
        $location = _$location_;
    }));

    it("should be defined", function () {
        expect(authInterceptor).toBeDefined();
    });

    it("should intercept an http request", function () {

        var config = {
            test: "data"
        };

        expect(authInterceptor.request(config)).toBeDefined();
    });

    it("should intercept an http response", function () {

        var rejection = {
            status: 401
        };

        authInterceptor.responseError(rejection);

        expect($location.path()).toEqual('/signin');
    });
});