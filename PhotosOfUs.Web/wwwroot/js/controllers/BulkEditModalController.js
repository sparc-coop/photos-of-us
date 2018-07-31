app.controller('BulkEditModalCtrl', ['$scope', '$window', '$mdDialog', '$http', 'selectedPhotos', ($scope, $window, $mdDialog, $http, selectedPhotos) => {
    $scope.close = () => $mdDialog.hide();
    $scope.selectedPhotos = selectedPhotos;
    $scope.photosviewmodel = { identifier: "ok", "photosid": [], tagsid: [] };

    $scope.deletePhotos = function (photos) {
        $http.post('/api/Photographer/deletePhotos/', photos);
        $window.location.href = '/Photographer/Profile/';
    }

    $scope.getTagsByPhotos = function () {
        $http.post('/api/Photographer/GetTagsByPhotos/', $scope.selectedPhotos)
            .then(function (x) {
                $scope.tags = x.data;
                
                console.log($scope.tags);
            });
    };

    $scope.editPhotos = function (photos, tags) {
        photos.forEach(function (item) {
            $scope.photosviewmodel.photosid.push(item.Id);
        });
        tags.forEach(function (item) {
            $scope.photosviewmodel.tagsid.push(item.Id);
        });
        $http.post('/api/Photographer/AddTags/', tags)
            .then($http.post('/api/Photographer/EditPhotos/', $scope.photosviewmodel)
                    .then(function (x) {
                        $scope.tags = x.data;

                        console.log($scope.tags);
                    }));
    };

}])