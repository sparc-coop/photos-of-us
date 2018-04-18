app.controller('PhotographerCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', 'photographerApi', ($scope, $window, $location, $http, $mdDialog, photographerApi) => {
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

    $scope.getProfile = function () {
       
        photographerApi.getAccountSettings().then(function (x) {
            $scope.photographer = x.data;
        })
    }

    
}])