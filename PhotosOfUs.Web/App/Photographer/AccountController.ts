namespace PhotosOfUs {
    angular.module('app').controller('AccountCtrl',
        ['$scope', '$window', '$mdDialog', 'UserApiClient',
        ($scope, $window, $mdDialog, UserApiClient: UserApiClient) => {
            $scope.discardChanges = () => window.location.reload();

            $scope.saveAccountSettings = (ev) => ev.closest('form').submit();

            $scope.uploadProfileImage = () => {
                $mdDialog.show({
                    templateUrl: '/Photographer/UploadProfileImage',
                    controller: 'UploadProfileImageCtrl',
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

            $scope.selected = 'details';

            $scope.setSelected = (selected) => {
                $scope.selected = selected;
            };

            $scope.templateSelected = '';

            $scope.selectTemplate = (id) => {
                $scope.templateSelected = id;
            };

        }]);
    }
