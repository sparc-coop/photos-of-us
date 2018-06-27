'use strict';

var app = angular.module('app', ['ngMaterial', 'angularFileUpload', 'monospaced.elastic', '720kb.socialshare', 'ui.bootstrap']);

app.filter('startFrom', function () {
    return function (data, start) {
        return data.slice(start)
    }
})


