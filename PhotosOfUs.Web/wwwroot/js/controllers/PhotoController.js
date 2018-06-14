app.controller('PhotoCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', '$timeout', '$q', ($scope, $window, $location, $http, $mdDialog, $timeout, $q) => {
    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };

    $scope.goToPurchase = (photoId) => {
        $window.location.href = '/Photo/Purchase/' + photoId;
    };

    $scope.goToGallery = (folderId) => {
        $window.location.href = '/Photographer/Photos/' + folderId;
    };

    $scope.openUpload = (folderId) => {
        $mdDialog.show({

            templateUrl: '/Photographer/Upload',
            controller: 'UploadController',
            locals: { folder: folderId },
            clickOutsideToClose: true
        });
    };

    $scope.getPhotoCode = () => {
        $scope.code = $location.absUrl().split('=')[1];
    };

    $scope.getPhotographer = (id) => {
        $http.get('/api/Photo/GetPhotographer/' + id).then(x => {
            $scope.photographer = x.data;
        });
    };

  
        var self = this;

        self.readonly = false;

        // Lists of fruit names and Vegetable objects
        self.fruitNames = ['Apple', 'Banana', 'Orange'];
        self.ngChangeFruitNames = angular.copy(self.fruitNames);
        self.roFruitNames = angular.copy(self.fruitNames);
        self.editableFruitNames = angular.copy(self.fruitNames);

        self.tags = [];
        self.vegObjs = [
            {
                'name': 'Broccoli',
                'type': 'Brassica'
            },
            {
                'name': 'Cabbage',
                'type': 'Brassica'
            },
            {
                'name': 'Carrot',
                'type': 'Umbelliferous'
            }
        ];

        self.newVeg = function (chip) {
            return {
                name: chip,
                type: 'unknown'
            };
        };

        self.onModelChange = function (newModel) {
            alert('The model has changed');
        };



}]);