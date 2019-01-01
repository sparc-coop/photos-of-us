namespace PhotosOfUs {
    angular.module('app').controller('SearchCtrl', ['$scope', '$timeout', '$filter', 'EventApiClient',
    ($scope, $timeout, $filter, EventApiClient: EventApiClient) => {
        $scope.getAllTags = (eventId: number) => {
            EventApiClient.getAllTags(eventId).then(tags => $scope.allTags = tags);
        };

        $scope.loadTags = (query: string) => {
            return $timeout(() => $filter('filter')($scope.allTags, query));
        };

        $scope.searchPhotos = (tags) => {
            window.location.href = '/Photographer/Results?tagnames=Image' + getSearchString(tags);
        };

        const getSearchString = (searchterms) => {
            let string = '';
            searchterms.forEach(element => string += '+' + element.text);
            return string;
        };
    }]);
}

