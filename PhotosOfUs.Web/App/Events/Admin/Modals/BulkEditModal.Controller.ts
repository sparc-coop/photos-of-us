namespace PhotosOfUs {
    angular.module('app').controller('BulkEditModalCtrl',
    ['$scope', '$window', '$mdDialog', 'EventApiClient', 'selectedPhotos',
    ($scope, $window, $mdDialog, EventApiClient: EventApiClient, selectedPhotos: Photo[]) => {

        $scope.close = () => $mdDialog.hide();
        $scope.selectedPhotos = selectedPhotos;

        $scope.deletePhotos = (photos: Photo[]) => {
            const photoIds = photos.map(x => x.id);
            EventApiClient.bulkDelete(photos[0].eventId, photoIds).then(() => $window.location.reload());
        };

        $scope.price = $scope.selectedPhotos.length < 2 ? $scope.selectedPhotos[0].price : null;

        $scope.editPhotos = function (photos: Photo[], tags, price) {
            if ($scope.selectedPhotos.length > 2) {
                price = null;
            }

            const photoIds = photos.map(x => x.id);
            EventApiClient.bulkEditSave(photos[0].eventId, photoIds, new BulkEditModel({ }))
                .then(() => $window.location.reload());
        };

    }]);
}
