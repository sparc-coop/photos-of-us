﻿app.factory('photoApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/Photo';
        return {
            getFolders: function () { return $http.get(apiRoot + '/GetFolders'); }
        };
    }
]);


app.factory('folderApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/Folder';
        return {
            add: function (foldername) { return $http.post(apiRoot+'/?name='+ foldername); },
            delete: function (folderId) { return $http.post(apiRoot + "/DeleteFolder/" + folderId); }
        };
    }
]);


app.factory('cardApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/Card';
        return {
            getAll: function () {return $http.get(apiRoot)}
        };
    }
]);


app.factory('photographerApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/Photographer';
        return {
            getAccountSettings: function () { return $http.get(apiRoot + '/GetAccountSettings') },
            saveAccountSettings: function (accountSettings) { console.log(JSON.stringify(accountSettings)); return $http.post(apiRoot + '/PostAccountSettings', accountSettings)}
        };
    }
]);

app.factory('checkoutApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/Checkout';
        return {
            
        };
    }
]);

