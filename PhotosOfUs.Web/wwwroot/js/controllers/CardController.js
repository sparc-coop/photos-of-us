app.controller('CardCtrl', ['$scope', '$rootScope', '$window', '$mdDialog', 'photoApi', 'cardApi', ($scope, $rootScope, $window, $mdDialog, photoApi, cardApi) => {
    $scope.close = () => $mdDialog.hide();
    $scope.cards = [];

    $scope.initCardCtrl = function () {
        
        cardApi.getAll()
            .then(function (x) {
                angular.forEach(x.data, function (c) { $scope.cards.push(c); });
                console.log(JSON.stringify(x.data));
            })
    }

    $scope.exportMultipleCardsModal = function() {
        $mdDialog.show({
            templateUrl: '/Photographer/MultipleCardsModal',
            controller: 'CardCtrl',
            clickOutsideToClose: true,
        })
    }

    $scope.exportMultipleCards = function (quantity) {
        location.href = "/Photographer/ExportMultipleCards/?quantity=" + quantity;
    }

    $scope.addFolder = function (folderName) {
        folderApi.add(folderName)
            .then(function (x) {
                //adds to list view
                $scope.close();
                $rootScope.$broadcast('FolderAdded', x.data);
            })
    }

    $scope.$on('FolderAdded', function (e, folder) {

        console.log('added folder - ' + JSON.stringify(folder));

        $scope.folders.push(folder);

    });
}])