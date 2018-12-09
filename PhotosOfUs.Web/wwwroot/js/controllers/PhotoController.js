app.controller('PhotoCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', '$timeout', '$q', 'Socialshare', ($scope, $window, $location, $http, $mdDialog, $timeout, $q, Socialshare) => {


    $scope.getUserTour = () => {
        $http.get('/api/User')
            .then(x => {
                $scope.viewTour(x.data);
            });
    };

    $scope.viewTour = (user) => {
        if (user.PhotoTour == null) {
            let tour = new Shepherd.Tour({
                defaultStepOptions: {
                    classes: 'shepherd-theme-arrows'
                }
            });

            tour.addStep('upload-step1', {
                title: 'Upload Photos',
                text: "Photos Of Us makes it easy ",
                attachTo: '.button--imageUpload bottom',
                classes: 'no-next'
            });

            tour.start();

            //$http.post('/api/User/ViewedPhoto/' + user.Id);
        }
    }

    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };

    $scope.checkFilter = (itemcode) => {
        itemcode = itemcode + "";

        if (itemcode.indexOf($scope.searchCode) >= 0) {
            return true;
        }
        else {
            return false;
        }
    }

    $scope.goToPurchase = (photoId) => {
        $window.location.href = '/Photographer/Purchase/' + photoId;
    };

    $scope.goToProfile = (photographerId) => {
        $window.location.href = '/Photographer/Profile/' + photographerId;
    };


    $scope.goToGallery = (folderId) => {
        $window.location.href = '/Photographer/Photos/' + folderId;
    };

    $scope.signInCustomer = (photoId) => {
        $http.get('/Session/SignIn/').then(
            $window.location.href = '/Photographer/Purchase/' + photoId
        );
    };

    $scope.openUpload = (eventId) => {
        $mdDialog.show({
            templateUrl: '/Events/Admin/Modals/UploadModal',
            controller: 'UploadController',
            locals: { eventId: eventId },
            clickOutsideToClose: true
        });
    };

    $scope.openPhotosEdit = (code) => {
        $scope.selectedPhotos = [];
        $http.get('/api/Photo/GetCodePhotos/' + code).then(x => {
            $scope.codePhotos = x.data;
            angular.forEach($scope.codePhotos, function (item) { $scope.selectedPhotos.push(item.Id) });
            $mdDialog.show({
                locals: { selectedPhotos: $scope.selectedPhotos },
                templateUrl: '/Events/Admin/Modals/BulkEditModal',
                controller: 'BulkEditModalCtrl',
                clickOutsideToClose: true,
            });
        });
    };
    
    $scope.getPhotosByCode = (code) => {
        $http.get('/api/Photo/GetCodePhotos/' + code).then(x => {
            $scope.codePhotos = x.data;
            $scope.getPhotographer(x.data[0].PhotographerId);
        });
    };

    $scope.openBulkEdit = (folder) => {
        $mdDialog.show({
            templateUrl: '/Events/Admin/Modals/BulkEditModal',
            controller: 'BulkEditController',
            locals: { folder },
            clickOutsideToClose: true
        });
    };

    $scope.openPhotoEdit = (folder) => {
        $mdDialog.show({
            templateUrl: '/Photographer/PhotoEditModal',
            controller: 'BulkEditController',
            locals: { folder },
            clickOutsideToClose: true
        });
    };

    $scope.getPhotographer = (id) => {
        $http.get('/api/User/GetOne/' + id).then(x => {
            $scope.photographer = x.data;
        });
    };

    $scope.getUser = () => {
        userApi.getUser().then(x => {
            $scope.user = x.data;
        });
    };

    $scope.shareFacebook = function (photoId) {
        var url = $location.absUrl().split('?')[0];
        Socialshare.share({
            'provider': 'facebook',
            'attrs': {
                'socialshareUrl': url,
                'socialshareHashtags': 'PoU, kuvio, PhotosOfUs'
            }
        });
    }


    $scope.shareTwitter = function (photoId) {
        var url = $location.absUrl().split('?')[0];
        Socialshare.share({
            'provider': 'twitter',
            'attrs': {
                'socialshareUrl': url,
                'socialshareText' : "Look at this awesome photo at PoU",
                'socialshareHashtags': 'PoU, kuvio, PhotosOfUs'
            }
        });
    }

    $scope.shareGooglePlus = function (photoId) {
        var url = $location.absUrl().split('?')[0];
        Socialshare.share({
            'provider': 'google',
            'attrs': {
                'socialshareUrl': url
            }
        });
    }

    $scope.sharePinterest = function (photoId, photoUrl) {
        var url = $location.absUrl().split('?')[0];
        Socialshare.share({
            'provider': 'pinterest',
            'attrs': {
                'socialshareUrl': url,
                'socialshareText': 'Look at this awesome photo at PoU',
                'socialshareMedia': photoUrl
            }
        });
    }

    $scope.shareTumblr = function (photoId, photoUrl) {
        var url = $location.absUrl().split('?')[0];
        Socialshare.share({
            'provider': 'tumblr',
            'attrs': {
                'socialshareUrl': url,
                'socialshareText': 'Look at this awesome photo at PoU',
                'socialshareMedia': photoUrl
            }
        });
    };

    $scope.pricingOption = 'option2';
    
}])