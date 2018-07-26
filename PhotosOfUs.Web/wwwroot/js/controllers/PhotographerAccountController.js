app.controller('PhotographerAccountCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', 'photographerApi', ($scope, $window, $location, $http, $mdDialog, photographerApi) => {
    $scope.originalSettings = {};
    $scope.initAccountSettings = function () {
        photographerApi.getAccountSettings().then(function (x) {
            console.log(x.data);
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

    $scope.discardChanges = function () {
        console.log(JSON.stringify($scope.originalSettings));
        $scope.accountSettings = angular.copy($scope.originalSettings);
    }

    $scope.saveAccountSettings = function (accountSettings) {
        console.log(JSON.stringify(accountSettings));
        $scope.showLoader = true;
        photographerApi.saveAccountSettings(accountSettings).then(function (x) {
            console.log(JSON.stringify(x));
            $scope.showLoader = false;
            swal({
                position: 'top-end',
                type: 'success',
                title: 'Your work has been saved',
                showConfirmButton: false,
                timer: 1500
            });
        }, function (x) {
            console.log(JSON.stringify(x));
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

    $scope.selected = 'details';

    $scope.setSelected = (selected) => {
        $scope.selected = selected;
    };


}])