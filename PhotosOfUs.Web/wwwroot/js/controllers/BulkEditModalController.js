app.controller('BulkEditModalCtrl', ['$scope', '$window', '$mdDialog', '$http', 'selectedPhotos', ($scope, $window, $mdDialog, $http, selectedPhotos) => {
    $scope.close = () => $mdDialog.hide();
    $scope.selectedPhotos = selectedPhotos;
    $scope.photosviewmodel = { photos: [], tags: [] };

    $scope.deletePhotos = function (photos) {
        $http.post('/api/Photographer/deletePhotos/', photos);
        $window.location.href = '/Photographer/Profile/';
    }

    $scope.getTagsByPhotos = function () {
        console.log($scope.selectedPhotos);
        $http.post('/api/Photographer/GetTagsByPhotos/', $scope.selectedPhotos)
            .then(function (x) {
                $scope.tags = x.data;
                console.log($scope.tags);
            });
    };

    $scope.getPhotoPrice = () => {
        if ($scope.selectedPhotos.length < 2) {
            $http.get('/api/Photographer/GetPhotoPrice/' + $scope.selectedPhotos)
                .then(function (x) {
                    $scope.price = x.data;
                });
        }
    };

    $scope.editPhotos = function (photos, tags, price) {
        photos.forEach(function (item) {
            $scope.photosviewmodel.photos.push(item);
        });
        tags.forEach(function (item) {
            $scope.photosviewmodel.tags.push(item);
        });

        if (price != null && $scope.selectedPhotos.length < 2) {
            $http.post('/api/Photographer/SavePhotoPrice/' + photos + '/' + price + '/');
        }

        $http.post('/api/Photographer/AddTags/', tags)
            .then(function () {
                $http.post('/api/Photographer/EditPhotos/', $scope.photosviewmodel)
                    .then(function (x) {
                        $scope.tags = x.data;

                        console.log($scope.tags);
                        $window.location.reload();
                    });
                });
    };

}])