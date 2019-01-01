namespace PhotosOfUs {
    angular.module('app').controller('AccountCtrl',
        ['$scope', '$window', '$mdDialog', 'UserApiClient',
        ($scope, $window, $mdDialog, UserApiClient: UserApiClient) => {
            $scope.selectedPhotos = [];
            $scope.profilePhotos = [];

            $scope.discardChanges = () => window.location.reload();

            $scope.saveAccountSettings = (ev) => ev.closest('form').submit();

            $scope.uploadProfileImage = () => {
                $mdDialog.show({
                    templateUrl: '/Photographer/Modals/UploadProfileImageModal',
                    controller: 'UploadProfileImageModalCtrl',
                    clickOutsideToClose: true,
                });
            };

            $scope.close = () => $mdDialog.hide();

            $scope.deactivateModal = (option: string) => {
                if (option === 'true') {
                    $mdDialog.show({
                        templateUrl: '/Photographer/Modals/DeactivateModal',
                        scope: $scope,
                        controller: 'ModalCtrl',
                        clickOutsideToClose: true
                    });
                } else if (option === 'false') {
                    $scope.reactivateAccount();
                }
            };

            $scope.reactivateAccount = () => {
                UserApiClient.reactivateAccount().then(function () { $window.location.reload(); });
            };

            $scope.deactivateAccount = () => {
                UserApiClient.deactivateAccount().then(function () { $window.location.reload(); });
            };

            $scope.getProfilePhotos = (userId: number) => {
                UserApiClient.getProfilePhotos(userId).then(x => $scope.profilePhotos = x);
            };

            $scope.selectPhoto = function (photoId) {
                const idx = $scope.selectedPhotos.indexOf(photoId);
                if (idx > -1) {
                    $scope.selectedPhotos.splice(idx, 1);
                } else {
                    $scope.selectedPhotos.push(photoId);
                }
            };

            $scope.selected = 'details';

            $scope.isBulkEditEnabled = false;
            $scope.toggleSelection = () => $scope.isBulkEditEnabled = !$scope.isBulkEditEnabled;

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

            $scope.setSelected = (selected) => {
                $scope.selected = selected;
            };

            $scope.isNotHighlighted = {
                'border': '3px solid green'
            };

            $scope.isHighlighted = {
                'border': '3px solid blue'
            };

            $scope.isPhotoSelected = photoId => $scope.selectedPhotos.indexOf(photoId) > -1;

            $scope.templateSelected = '';

            $scope.selectTemplate = (id) => {
                $scope.templateSelected = id;
            };

        }]);
    }
