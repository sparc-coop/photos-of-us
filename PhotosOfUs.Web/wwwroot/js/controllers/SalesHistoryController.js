app.controller('SalesHistoryCtrl', ['$scope', '$window', '$location', '$http', ($scope, $window, $location, $http) => {

    $scope.query = "";

    $scope.querySalesHistory = (query) => {
        $('.sales-container .overlay').addClass('loading');
        $http.get('/api/Photographer/SalesHistory?query=' + query).then(x => {
            if (x.status === 200) {
                $('.sales-content').html(x.data);
                $('.sales-container .overlay').removeClass('loading');
            }
        });
    }
}])