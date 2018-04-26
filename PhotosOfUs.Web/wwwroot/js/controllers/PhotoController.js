app.controller('PhotoCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', ($scope, $window, $location, $http, $mdDialog) => {
    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };

    $scope.goToPurchase = (photoId) => {
        $window.location.href = '/Photo/Purchase/' + photoId;
    };

    $scope.goToGallery = (folderId) => {
        $window.location.href = '/Photographer/Photos/' + folderId;
    };

    $scope.openUpload = (folderId) => {
        $mdDialog.show({
            
            templateUrl: '/Photographer/Upload',
            controller: 'UploadController',
            locals: { folder: folderId },
            clickOutsideToClose: true
        });
    };
    
    $scope.getPhotoCode = () => {
        $scope.code = $location.absUrl().split('=')[1];
    };

    $scope.getPhotographer = (id) => {
        $http.get('/api/Photo/GetPhotographer/' + id).then(x => {
            $scope.photographer = x.data;
        });
    };

    
}])