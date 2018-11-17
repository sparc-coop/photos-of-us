app.factory('photoApi', [
    '$http', '$rootScope', function ($http) {
        var apiRoot = '/api/Photo';
        return {
            //getFolders: function () { return $http.get(apiRoot + '/GetFolders'); }
        };
    }
]);


app.factory('folderApi', [
    '$http', '$rootScope', function ($http) {
        var apiRoot = '/api/Folder';
        return {
            add: function (foldername, userId) { return $http.post(apiRoot + '/' + foldername + '/' + userId); },
            rename: function (folderId, folderName, userId) { return $http.post(apiRoot + "/RenameFolder", JSON.stringify({ Id: folderId, NewName: folderName, UserId: userId}))},
            delete: function (folderId, userId) { return $http.post(apiRoot + "/DeleteFolder/" + folderId + '/' + userId); }
        };
    }
]);


app.factory('cardApi', [
    '$http', '$rootScope', function ($http) {
        var apiRoot = '/api/Card';
        return {
            getAll: function () { return $http.get(apiRoot) },
            create: function (quantity, id) { return $http.post(apiRoot + '/Create/' + quantity + '/' + id) }
        };
    }
]);

app.factory('userApi', [
    '$http', '$rootScope', function ($http) {
        var apiRoot = '/api/User';
        return {
            get: function () { return $http.get(apiRoot)}, 
            update: function (accountSettings) { return $http.put(apiRoot, accountSettings) },
            getFolders: function () { return $http.get(apiRoot + '/GetFolders'); }
        };
    }
]);

app.factory('photographerApi', [
    '$http', '$rootScope', function ($http) {
        var apiRoot = '/api/Photographer';
        return {
            getBrandSettings: function () { return $http.get(apiRoot + '/GetBrandSettings'); },
            saveBrandSettings: function (brandSettings) { return $http.put(apiRoot + '/SaveBrandSettings', brandSettings) }
        };
    }
]);

