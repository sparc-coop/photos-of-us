app.controller('DownloadCtrl', ['$scope', '$window', '$mdDialog', '$http', 'userApi', ($scope, $window, $mdDialog, $http, userApi) => {

    $scope.getOrders = (userId) => {
        $http.get('/api/Photo/GetOrderPhotos/' + userId).then(x => {
            $scope.orders = x.data;
            angular.forEach($scope.orders, function (key, value) {
                $scope.getOrderItems(key.Id);
            });           
        });
    };

    $scope.orderItems = [];

    $scope.getOrderItems = (orderId) => {
        $http.get('/api/Photo/GetOrderItems/' + orderId).then(x => {
            $scope.orderItems.push(x.data);          
        });
    };

    $scope.bulkDownload = (userId) => {
        $http.post('/api/Photo/GetForDownload/' + userId);
    };


}])