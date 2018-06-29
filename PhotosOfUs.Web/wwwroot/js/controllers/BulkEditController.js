angular.module('app').controller('BulkEditController', function ($scope, $http, $window, $mdDialog, $filter, code) {

    $scope.code = code;
    $scope.tags = ['pretty', 'travel', 'dog'];

    $scope.test = () => {
        console.log("hello?", $scope.tags)
    }

});