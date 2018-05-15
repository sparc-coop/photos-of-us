app.controller('ModalController', ['$scope', '$window', '$mdDialog', ($scope, $window, $mdDialog) => {
    $scope.close = () => $mdDialog.hide();

    //$scope.deactivateStatus = () => {
    //    $scope.hidden = !$scope.hidden;
    //    $scope.close();
    //}
}])