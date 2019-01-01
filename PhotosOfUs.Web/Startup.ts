namespace PhotosOfUs {
    angular.module('app', [
        'ngMaterial',
        'angularFileUpload',
        'monospaced.elastic',
        '720kb.socialshare',
        'ui.bootstrap',
        'ngTagsInput',
        'angular.filter'
    ]);

    angular.module('app').filter('startFrom', () => {
        return (data, start) => data.slice(start);
    });
}
