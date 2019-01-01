﻿declare var Shepherd;
namespace PhotosOfUs {
    angular.module('app').controller('PhotosCtrl',
        ['$scope', '$location', '$mdDialog', 'Socialshare', 'UserApiClient', 'EventApiClient',
        ($scope, $location, $mdDialog, Socialshare, UserApiClient: UserApiClient, EventApiClient: EventApiClient) => {

        $scope.getUserTour = () => {
            UserApiClient.get()
                .then(user => {
                    if (user.photoTour == null) {
                        const tour = new Shepherd.Tour({
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

        $scope.openUpload = (eventId: number) => {
            $mdDialog.show({
                templateUrl: '/Events/Admin/Modals/UploadModal',
                controller: 'UploadModalCtrl',
                locals: { eventId: eventId },
                clickOutsideToClose: true
            });
        };

        $scope.openPhotosEdit = (eventId: number, code: string) => {
            $scope.selectedPhotos = [];
            EventApiClient.getCodePhotos(eventId, code)
                .then(x => {
                    $scope.codePhotos = x;
                    angular.forEach($scope.codePhotos, item => $scope.selectedPhotos.push(item.Id));
                    $mdDialog.show({
                        locals: { selectedPhotos: $scope.selectedPhotos },
                        templateUrl: '/Events/Admin/Modals/BulkEditModal',
                        controller: 'BulkEditModalCtrl',
                        clickOutsideToClose: true,
                    });
            });
        };

        $scope.share = (provider: string, photoUrl?: string) => {
            const url = $location.absUrl().split('?')[0];
            Socialshare.share({
                'provider': provider,
                'attrs': {
                    'socialshareUrl': url,
                    'socialshareText' : 'Look at this awesome photo at PoU',
                    'socialshareHashtags': 'PoU, kuvio, PhotosOfUs',
                    'socialshareMedia': photoUrl
                }
            });
        };
    }]);
}
