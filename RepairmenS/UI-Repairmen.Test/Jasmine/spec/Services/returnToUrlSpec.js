describe('Factory: ReturnToUrl', function () {

    var returnToUrl,
        $location;

    beforeEach(module("app"));

    beforeEach(inject(function (_returnToUrl_, _$location_) {
        returnToUrl = _returnToUrl_;
        $location = _$location_;
    }));

    it("should be defined", function () {
        expect(returnToUrl).toBeDefined();
    });

    //The only way to test storing/retrieving of data
    //on this service is by calling one function and
    //then another. No direct access to Data, so
    //its pointless to separate this test into two
    //as they would be identical.
    it("should store and retrieve data", function () {

        var comment = "comment",
            type = "type",
            index = "index";

        returnToUrl.storeData(comment, type, index);

        expect(returnToUrl.getComment()).toEqual(comment);
        expect(returnToUrl.getType()).toEqual(type);
        expect(returnToUrl.getIndex()).toEqual(index);
    });

    it("should clear data", function () {

        var comment = "comment",
            type = "type",
            index = "index";

        returnToUrl.storeData(comment, type, index);

        returnToUrl.clearData();

        expect(returnToUrl.getComment()).not.toBeDefined();
    });

    it("should perform a redirect to a return url", function () {

        var comment = "comment",
            type = "type",
            index = "index";

        $location.path('/return/url')
        returnToUrl.storeData(comment, type, index);
        $location.path('/login/page');
        returnToUrl.redirect();

        expect($location.path()).toEqual('/return/url');
    });

    it("should perform a redirect to a default page, when there is no url", function () {

        var comment = "comment",
            type = "type",
            index = "index";

        $location.path('/return/url')
        returnToUrl.clearData();
        returnToUrl.redirect();

        expect($location.path()).toEqual('/ads');
    });
});