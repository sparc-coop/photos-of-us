var PhotosOfUs;
(function (PhotosOfUs) {
    angular.module('app', [
        'ngMaterial',
        'angularFileUpload',
        'monospaced.elastic',
        '720kb.socialshare',
        'ui.bootstrap',
        'ngTagsInput',
        'angular.filter',
        'PhotosOfUs.API'
    ]);
    angular.module('app').filter('startFrom', function () {
        return function (data, start) { return data.slice(start); };
    });
})(PhotosOfUs || (PhotosOfUs = {}));
//# sourceMappingURL=Startup.js.map