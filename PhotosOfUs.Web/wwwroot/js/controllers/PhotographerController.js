app.controller('PhotographerCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', 'photographerApi', ($scope, $window, $location, $http, $mdDialog, photographerApi) => {
    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };

    $scope.openUpload = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/UploadProfilePhoto',
            controller: 'ModalController',
            clickOutsideToClose: true,
        });
    };

    $scope.getProfile = function () {

        photographerApi.getAccountSettings().then(function (x) {
            $scope.photographer = x.data;
        })
    }

    $scope.tags = [
        { text: 'Tag1' },
        { text: 'Tag2' },
        { text: 'Tag3' }
    ];

    $scope.loadedtags = [{ text: 'TagLoca' }];

    //$scope.getPhotographer = (id) => {
    //    $http.get('/api/Photo/GetPhotographer/' + id).then(x => {
    //        $scope.photographer = x.data;
    //    });
    //};

    $scope.loadTags = function (query) {
        $http.get('/api/Photo/GetAllTags/')
            .then(function (x) {
                angular.forEach(x.data, function (f) { $scope.loadedtags.push(f); });
                console.log(JSON.stringify(x.data));
            });
        
        return $scope.loadedtags;
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
    
}])