app.controller('BulkEditModalCtrl', ['$scope', '$window', '$mdDialog', '$http', 'selectedPhotos', ($scope, $window, $mdDialog, $http, selectedPhotos) => {
    $scope.close = () => $mdDialog.hide();
    $scope.selectedPhotos = selectedPhotos;

    $scope.deletePhotos = function (photos) {
        $http.post('/api/Photographer/deletePhotos/', photos);
    }
}])