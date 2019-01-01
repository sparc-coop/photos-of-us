namespace PhotosOfUs {
    angular.module('app').controller('ModalController', ['$scope', '$mdDialog', ($scope, $mdDialog) => {
        $scope.close = () => $mdDialog.hide();
    }]);
}
