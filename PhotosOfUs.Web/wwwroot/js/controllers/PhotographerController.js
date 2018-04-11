app.controller('PhotographerCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', ($scope, $window, $location, $http, $mdDialog) => {
    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };
    
    $scope.openUpload = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/UploadProfilePhoto',
            controller: 'ModalController',
            clickOutsideToClose: true,
        });
    };

    
}])