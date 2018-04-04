app.factory('photoApi', [
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

