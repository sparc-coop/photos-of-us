var PhotosOfUs;
(function (PhotosOfUs) {
    angular.module('app').controller('EventCtrl', ['$scope', '$location', '$mdDialog', 'Socialshare', 'UserApiClient', 'EventApiClient',
        function ($scope, $location, $mdDialog, Socialshare, UserApiClient, EventApiClient) {
            $scope.getUserTour = function () {
                UserApiClient.get()
                    .then(function (user) {
                    if (user.photoTour == null) {
                        var tour = new Shepherd.Tour({
                            defaultStepOptions: {
                                classes: 'shepherd-theme-arrows'
                            }
                        });
                        tour.addStep('upload-step1', {
                            title: 'Upload Photos',
                            text: 'Photos Of Us makes it easy ',
                            attachTo: '.button--imageUpload bottom',
                            classes: 'no-next'
                        });
                        tour.start();
                    }
                });
            };
            $scope.openUpload = function (eventId) {
                $mdDialog.show({
                    templateUrl: '/Events/Admin/Modals/UploadModal',
                    controller: 'UploadController',
                    locals: { eventId: eventId },
                    clickOutsideToClose: true
                });
            };
            $scope.openPhotosEdit = function (eventId, code) {
                $scope.selectedPhotos = [];
                EventApiClient.getCodePhotos(eventId, code)
                    .then(function (x) {
                    $scope.codePhotos = x;
                    angular.forEach($scope.codePhotos, function (item) { return $scope.selectedPhotos.push(item.Id); });
                    $mdDialog.show({
                        locals: { selectedPhotos: $scope.selectedPhotos },
                        templateUrl: '/Events/Admin/Modals/BulkEditModal',
                        controller: 'BulkEditModalCtrl',
                        clickOutsideToClose: true,
                    });
                });
            };
            $scope.openBulkEdit = function (folder) {
                $mdDialog.show({
                    templateUrl: '/Events/Admin/Modals/BulkEditModal',
                    controller: 'BulkEditController',
                    locals: { folder: folder },
                    clickOutsideToClose: true
                });
            };
            $scope.openPhotoEdit = function (folder) {
                $mdDialog.show({
                    templateUrl: '/Events/Admin/Modals/PhotoEditModal',
                    controller: 'BulkEditController',
                    locals: { folder: folder },
                    clickOutsideToClose: true
                });
            };
            $scope.share = function (provider, photoUrl) {
                var url = $location.absUrl().split('?')[0];
                Socialshare.share({
                    'provider': provider,
                    'attrs': {
                        'socialshareUrl': url,
                        'socialshareText': 'Look at this awesome photo at PoU',
                        'socialshareHashtags': 'PoU, kuvio, PhotosOfUs',
                        'socialshareMedia': photoUrl
                    }
                });
            };
        }]);
})(PhotosOfUs || (PhotosOfUs = {}));
//# sourceMappingURL=EventController.js.map