app.controller('PhotographerAccountCtrl', ['$scope', '$window', '$http', '$mdDialog', 'userApi', 'photographerApi'
    , ($scope, $window, $http, $mdDialog, userApi, photographerApi) => {
        $scope.originalSettings = {};

        $scope.initAccountSettings = function () {
            userApi.get().then(function (x) {
                $scope.accountSettings = x.data;
                if ($scope.accountSettings.Facebook == null)
                    $scope.accountSettings.Facebook = 'https://www.facebook.com/';
                if ($scope.accountSettings.Twitter == null)
                    $scope.accountSettings.Twitter = 'https://www.twitter.com/';
                if ($scope.accountSettings.Instagram == null)
                    $scope.accountSettings.Instagram = 'https://www.instagram.com/';
                if ($scope.accountSettings.Dribbble == null)
                    $scope.accountSettings.Dribbble = 'https://www.dribbble.com/';
                angular.copy(x.data, $scope.originalSettings);
            })
        }

        $scope.initBrandSettings = function () {
            photographerApi.getBrandSettings().then(function (x) {
                $scope.brandSettings = x.data;
            })
        }

        $scope.saveBrandSettings = function () {
            photographerApi.saveBrandSettings($scope.brandSettings).then(function (x) {
                $scope.brandSettings = x.data;
            })
        }

        $scope.discardChanges = function () {
            $scope.accountSettings = angular.copy($scope.originalSettings);
        }

        $scope.saveAccountSettings = function (accountSettings) {
            $scope.showLoader = true;
            userApi.update(accountSettings).then(function (x) {
                $scope.showLoader = false;
                swal({
                    position: 'top-end',
                    type: 'success',
                    title: 'Your work has been saved',
                    showConfirmButton: false,
                    timer: 1500
                });
            }, function (x) {
                $scope.showLoader = false;
                swal({
                    position: 'top-end',
                    type: 'error',
                    title: 'Oops... Something went wrong!',
                    showConfirmButton: false,
                    timer: 1500
                });
            });
        }

        $scope.uploadProfileImage = () => {
            $mdDialog.show({
                templateUrl: '/Photographer/UploadProfileImage',
                controller: 'UploadProfileImageCtrl',
                clickOutsideToClose: true,
            });
        };

        $scope.close = () => $mdDialog.hide();

        $scope.deactivateModal = (option, user) => {
            if (option == 'true') {
                $mdDialog.show({
                    templateUrl: '/Photographer/Modals/DeactivateModal',
                    controller: 'PhotographerAccountStatusCtrl',
                    user: user,
                    clickOutsideToClose: true,
                });
            }
            else if (option == 'false') {
                $scope.reactivateAccount(user.Id);
            }
        }


        $scope.reactivateAccount = () => {
            $http.post('/api/User').then(function () { $window.location.reload() });
        }

        $scope.selected = 'details';

        $scope.setSelected = (selected) => {
            $scope.selected = selected;
        };

    $scope.templateSelected = '';

    $scope.selectTemplate = (id) => {
        $scope.templateSelected = id;
    };

    }])
    .controller('PhotographerAccountStatusCtrl', ['$scope', '$window', '$http', 'user', ($scope, $window, $http, user) => {
        $scope.user = user;

        $scope.deactivateAccount = () => {
            $http.delete('/api/User').then(function () { $window.location.reload() });
        }
    }]);

