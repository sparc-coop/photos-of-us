app.controller('PhotographerAccountCtrl', ['$scope', '$rootScope', '$window', '$mdDialog', 'photoApi', 'folderApi', ($scope, $rootScope, $window, $mdDialog, photoApi, folderApi) => {
    $scope.close = () => $mdDialog.hide();
    $scope.hidden = $scope.hidden;

    $scope.deactivateModal = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/DeactivateModal',
            controller: 'PhotographerAccountCtrl',
            clickOutsideToClose: true,
        })
    }

    $scope.deactivateStatus = () => {
       // $scope.hidden = !$scope.hidden;
        $scope.close();
    }

    $scope.discard = () => {

    }

    
}])