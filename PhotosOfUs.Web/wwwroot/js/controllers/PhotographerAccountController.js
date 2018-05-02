app.controller('PhotographerAccountCtrl', ['$scope', '$rootScope', '$window', '$mdDialog', 'photoApi', 'folderApi', ($scope, $rootScope, $window, $mdDialog, photoApi, folderApi) => {
    $scope.close = () => $mdDialog.hide();
   

    $scope.deactivateModal = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/DeactivateModal',
            controller: 'PhotographerAccountCtrl',
            clickOutsideToClose: true,
        })
    }

    
}])