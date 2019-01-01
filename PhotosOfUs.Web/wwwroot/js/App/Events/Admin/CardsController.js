var PhotosOfUs;
(function (PhotosOfUs) {
    angular.module('app')
        .controller('CardsCtrl', ['$scope', '$mdDialog', '$timeout', 'EventApiClient',
        ($scope, $mdDialog, $timeout, EventApiClient) => {
            $scope.close = () => $mdDialog.hide();
            $scope.cards = [];
            $scope.cardsToExport = [];
            $scope.pageSize = 10;
            $scope.currentPage = 1;
            $scope.getCards = (eventId) => {
                $scope.eventId = eventId;
                EventApiClient.getEventCards(eventId).then(x => {
                    $scope.cards = x;
                });
            };
            $scope.exportMultipleCardsModal = () => {
                $mdDialog.show({
                    templateUrl: '/Events/Admin/Modals/MultipleCardsModal',
                    scope: $scope,
                    clickOutsideToClose: true
                });
            };
            $scope.exportMultipleCards = (quantity) => {
                EventApiClient.createEventCards($scope.eventId, quantity).then(x => {
                    $scope.cards = x.concat($scope.cards);
                    $mdDialog.hide();
                    $scope.downloadCards(x);
                });
            };
            $scope.downloadCards = (cards) => {
                $scope.cardsToExport = cards;
                // Use timeout to wait for Angular to finish rendering the hidden inputs for the POST form
                $timeout(function () { $('#card-downloader').submit(); });
            };
        }]);
})(PhotosOfUs || (PhotosOfUs = {}));
//# sourceMappingURL=CardsController.js.map