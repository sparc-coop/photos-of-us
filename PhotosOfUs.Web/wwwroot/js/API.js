﻿app.factory('photoApi', [
    '$http', '$rootScope', function ($http) {
        var apiRoot = '/api/Photo';
        return {
            getFolders: function () { return $http.get(apiRoot + '/GetFolders'); }
        };
    }
]);


app.factory('folderApi', [
    '$http', '$rootScope', function ($http) {
        var apiRoot = '/api/Folder';
        return {
            add: function (foldername) { return $http.post(apiRoot + '/?name=' + foldername); },
            rename: function (folderId, folderName) { return $http.post(apiRoot + "/RenameFolder", JSON.stringify({ Id: folderId, NewName: folderName}))},
            delete: function (folderId) { return $http.post(apiRoot + "/DeleteFolder/" + folderId); }
        };
    }
]);


app.factory('cardApi', [
    '$http', '$rootScope', function ($http) {
        var apiRoot = '/api/Card';
        return {
            getAll: function () { return $http.get(apiRoot) },
            create: function (quantity) { return $http.post(apiRoot + '/Create/' + quantity) }
        };
    }
]);

app.factory('checkoutApi', [
    '$http', '$rootScope', function ($http) {
        var apiRoot = '/api/Checkout';
        return {
            createOrder: function (userId, orderItems) { return $http.get(apiRoot + '/CreateOrder/' + userId, orderItems) },
        };
    }
]);

app.factory('userApi', [
    '$http', '$rootScope', function ($http) {
        var apiRoot = '/api/User';
        return {
            get: function () { return $http.get(apiRoot)}, 
            update: function (accountSettings) { return $http.put(apiRoot, accountSettings)}
        };
    }
]);

