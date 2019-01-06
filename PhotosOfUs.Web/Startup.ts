namespace PhotosOfUs {
    angular.module('app', [
        'ngMaterial',
        'angularFileUpload',
        'monospaced.elastic',
        '720kb.socialshare',
        'ui.bootstrap',
        'ngTagsInput',
        'angular.filter',
        'ngPatternRestrict'
    ]);

    // Register NSwag APIs
    angular.module('app')
        .value('baseUrl', '')
        .service('PhotographerClient', PhotographerClient)
        .service('EventApiClient', EventApiClient)
        .service('FolderApiClient', FolderApiClient)
        .service('RandomPhotoClient', RandomPhotoClient)
        .service('UserApiClient', UserApiClient);

    angular.module('app').filter('startFrom', () => {
        return (data, start) => data.slice(start);
    });
}
