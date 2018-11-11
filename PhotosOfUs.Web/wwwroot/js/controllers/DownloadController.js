app.controller('DownloadCtrl', ['$scope', '$window', '$mdDialog', '$http', 'userApi', ($scope, $window, $mdDialog, $http, userApi) => {

    $scope.getOrders = (userId) => {
        $http.get('/api/Photo/GetOrderPhotos/' + userId).then(x => {
            $scope.orders = x.data;
            angular.forEach($scope.orders, function (key, value) {
                $scope.getOrderItems(key.Id);
            });           
        });

        $http.get('/api/User/GetOne/' + userId).then(x => {
            $scope.user = x.data;
            console.log(x.data);
        });
    };

    $scope.orderItems = [];

    $scope.getOrderItems = (orderId) => {
        $http.get('/api/Orders/GetOrderDetails/' + orderId).then(x => {
            $scope.orderItems.push(x.data);          
        });
    };

    $scope.bulkDownload = (userId) => {
        $http.post('/api/Photo/GetForDownload/' + userId);
    };

}])