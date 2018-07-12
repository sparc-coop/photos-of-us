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

    $scope.tags = [];

    $scope.loadedtags = [];

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