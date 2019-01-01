app.controller('PhotographerCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', '$filter', '$timeout', 'userApi', ($scope, $window, $location, $http, $mdDialog, $filter, $timeout, userApi) => {

    $scope.tags = [];
    $scope.allTags = [];
    $scope.isBulkEditEnabled = false;
    $scope.selectedPhotos = [];
    $scope.profilePhotos = [];
    $scope.isNotHighlighted = {
        "border": "3px solid green"
    }
    $scope.isHighlighted = {
        "border": "3px solid blue"
    }

    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };

    $scope.isPhotoSelected = function (photoId) {
        var idx = $scope.selectedPhotos.indexOf(photoId);
        if (idx > -1) {
            return true;
        }
        else {
            return false;
        }
    }

    $scope.openUpload = () => {
        $mdDialog.show({
            locals: { selectedPhotos: $scope.selectedPhotos },
            templateUrl: '/Photographer/UploadProfilePhoto',
            controller: 'ModalController',
            clickOutsideToClose: true,
        });
    };

    $scope.openBulkEdit = () => {
        $mdDialog.show({
            locals: { selectedPhotos: $scope.selectedPhotos },
            templateUrl: '/Events/Admin/Modals/BulkEditModal',
            controller: 'BulkEditModalCtrl',
            clickOutsideToClose: true,
        });
    };

    $scope.photographerId = $location.absUrl().split('Profile/')[1];

    $scope.toggleSelection = function () {
        $scope.isBulkEditEnabled = !$scope.isBulkEditEnabled;
    }

    $scope.selectPhoto = function (photoId) {
        var idx = $scope.selectedPhotos.indexOf(photoId);
        if (idx > -1) {
            $scope.selectedPhotos.splice(idx, 1);
        }
        else {
            $scope.selectedPhotos.push(photoId);
        }
    }

    $scope.getProfilePhotos = function () {
        $http.get('/api/Photographer/getProfilePhotos/' + $scope.photographerId)
        .then(function (x) {
            angular.forEach(x.data, function (f) { $scope.profilePhotos.push(f); });
        });
    };

    $scope.arrangePhotos = function () {
        var grid = document.querySelector('.grid');
        var msnry;

        imagesLoaded(grid, function () {
            // init Isotope after all images have loaded
            msnry = new Masonry(grid, {
                itemSelector: '.grid-item',
                columnWidth: '.grid-sizer',
                percentPosition: true
            });
        });
    }
    
    $scope.getUser = () => {
        $http.get('/api/User')
        .then(x => {
            $scope.user = x.data;
        });
    };

}])