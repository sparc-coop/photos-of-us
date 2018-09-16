app.controller('CardCtrl', ['$scope', '$rootScope', '$window', '$mdDialog', 'photoApi', 'cardApi', '$timeout', ($scope, $rootScope, $window, $mdDialog, photoApi, cardApi, $timeout) => {
    $scope.close = () => $mdDialog.hide();
    $scope.cards = [];
    $scope.cardsToExport = [];
    $scope.pageSize = 5;
    $scope.currentPage = 1;
    
    $scope.initCardCtrl = function () {
        $scope.cards = [];
        cardApi.getAll()
            .then(function (x) {
                $scope.cards = x.data;
            });
    };

    $scope.exportMultipleCardsModal = function () {
        $mdDialog.show({
            templateUrl: '/Photographer/MultipleCardsModal',
            scope: $scope,
            clickOutsideToClose: true
        })
    };

    $scope.exportMultipleCards = function (quantity) {
        cardApi.create(quantity).then(function(x) {
            console.log(x.data);
            console.log($scope.cards);
            $scope.cards = x.data.concat($scope.cards);
            console.log($scope.cards);
            $mdDialog.hide();
            $scope.downloadCards(x.data);
        });
    };

    $scope.exportMultipleCards = function (quantity) {
        cardApi.create(quantity).then(function (x) {
            console.log(x.data);
            console.log($scope.cards);
            $scope.cards = x.data.concat($scope.cards);
            console.log($scope.cards);
            $mdDialog.hide();
            $scope.downloadCards(x.data);
        });
    };

    $scope.MooModal = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/MooOrderModal',
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

        console.log('added folder - ' + JSON.stringify(folder));

        $scope.folders.push(folder);

    });
}])