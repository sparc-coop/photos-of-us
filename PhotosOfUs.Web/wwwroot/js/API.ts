declare var angular;
var rootApp = angular.module('app', ['modal', 'upload']);
rootApp.factory('photoApi',
    [
        '$http', '$rootScope', ($http, $rootScope) => {
            var apiRoot = '/api/AccountDashboard';
            return {
                get: () => $http.get(apiRoot + '/Get')
                //uploadFile: (file, userId) => Upload.upload({
                //    url: apiRoot + '/UploadFile/' + userId,
                //    data: { file: file }
                //})
            }
        }
    ]);