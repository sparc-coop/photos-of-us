angular.module('app').controller('BulkEditController', function ($scope, $http, $window, $mdDialog, $filter) {

    $scope.tags = ['pretty', 'travel', 'dog'];

    $scope.test = () => {
        console.log("hello?")
    }

});