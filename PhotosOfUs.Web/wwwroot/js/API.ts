app.factory('photoApi',
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