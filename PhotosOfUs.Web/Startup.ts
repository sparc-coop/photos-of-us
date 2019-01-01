﻿import * as angular from 'angular';

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

angular.module('app').filter('startFrom', () => {
    return (data, start) => data.slice(start);
});