app.controller('PhotographerCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', 'photographerApi', 'userApi', ($scope, $window, $location, $http, $mdDialog, photographerApi, userApi) => {

    $scope.tags = [];
    $scope.loadedtags = [];
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
            templateUrl: '/Photographer/BulkEditModal',
            controller: 'BulkEditModalCtrl',
            clickOutsideToClose: true,
        });
    };

    $scope.getProfile = function () {
        photographerApi.getAccountSettings().then(function (x) {
            $scope.photographer = x.data;
        })
    }

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

    $scope.deletePhotos = function (photos) {
        $http.post('/api/Photographer/deletePhotos/', photos);
    }

    //$scope.getPhotographer = (id) => {
    //    $http.get('/api/Photo/GetPhotographer/' + id).then(x => {
    //        $scope.photographer = x.data;
    //    });
    //};

    $scope.loadTags = function () {
        $http.get('/api/Photo/GetAllTags/')
            .then(function (x) {
                angular.forEach(x.data, function (f) { $scope.loadedtags.push(f); });
                console.log(JSON.stringify(x.data));
            });
    };

    $scope.getSearchString = function (searchterms) {

        var string = "";

        searchterms.forEach(function (element, index) {
            string += "+" + element.text;
        });

        return string;
    };

    $scope.searchPhotos = (searchterms) => {
        $window.location.href = '/Photographer/Results?tagnames=Image' + $scope.getSearchString(searchterms);
    };

    $scope.getProfilePhotos = function () {
        $http.get('/api/Photographer/getProfilePhotos/')
            .then(function (x) {
                angular.forEach(x.data, function (f) { $scope.profilePhotos.push(f); });
            });
    }

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
        userApi.getUser().then(function (x) {
            $scope.user = x.data;
        });
    };
}])