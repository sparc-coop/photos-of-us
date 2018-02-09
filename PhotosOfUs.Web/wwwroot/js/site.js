'use strict';

var rootApp = angular.module('app', ['modal', 'upload']);

// modal
var modal = angular.module('modal', ['ui.bootstrap']);
modal.controller('ModalController', function ($scope, $uibModal, $timeout) {
    $scope.openUpload = function () {
        var modalInstance = $uibModal.open({
            windowTopClass: 'modal-upload',
            backdrop: false,
            templateUrl: 'Upload',
            size: 'lg',
            controller: function ($scope, $uibModalInstance) {
                $scope.close = function () {
                    $uibModalInstance.close(false);
                };
            }
        });
    };
});