app.controller('CardCtrl', ['$scope', '$rootScope', '$mdDialog', '$timeout', '$http', ($scope, $rootScope, $mdDialog, $timeout, $http) => {
    $scope.close = () => $mdDialog.hide();
    $scope.cards = [];
    $scope.cardsToExport = [];
    $scope.pageSize = 10;
    $scope.currentPage = 1;

    $scope.getCards = function (eventId) {
        $scope.eventId = eventId;
        $http.get('/api/Events/' + eventId + '/Cards').then(x => {
            $scope.cards = x.data;
        });
    };

    $scope.exportMultipleCardsModal = function () {
        $mdDialog.show({
            templateUrl: '/Events/Admin/Modals/MultipleCardsModal',
            scope: $scope,
            clickOutsideToClose: true
        })
    };

    $scope.exportMultipleCards = function (quantity) {
        $http.post('/api/Events/' + $scope.eventId + '/Cards', quantity).then(function (x) {
            $scope.cards = x.data.concat($scope.cards);
            $mdDialog.hide();
            $scope.downloadCards(x.data);
        });
    };

    $scope.MooModal = () => {
        $mdDialog.show({
            templateUrl: '/Events/Admin/Modals/MooOrderModal',
            scope: $scope,
            clickOutsideToClose: true
        })
    };

    $scope.downloadCards = function (cards) {
        $scope.cardsToExport = cards;
        // Use timeout to wait for Angular to finish rendering the hidden inputs for the POST form
        $timeout(function () { $('#card-downloader').submit() });
    };

    $scope.addFolder = function (folderName) {
        folderApi.add(folderName)
            .then(function (x) {
                //adds to list view
                $scope.close();
                $rootScope.$broadcast('FolderAdded', x.data);
            })
    }

    $scope.$on('FolderAdded', function (e, folder) {

        $scope.folders.push(folder);

    });
}])