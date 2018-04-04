app.controller('CheckoutCtrl', ['$scope', '$window', '$location', '$http', ($scope, $window, $location, $http) => {
    $scope.goToCart = () => {
        $window.location.href = '/Photo/Cart';
    };

    $scope.goToCheckout = () => {
        $window.location.href = '/Photo/Checkout';
    };

    $scope.getPrintTypes = () => {
        $http.get('/api/Photo/GetPrintTypes').then(x => {
            $scope.printTypes = x.data;
            console.log($scope.printTypes);
        });
    };

    $scope.selectedItems = [];

    $scope.select = (printTypeId) => {
        if ($scope.selectedItems.length === 0) {
            $scope.selectedItems.push(printTypeId);
        }
        else if ($scope.selectedItems.indexOf(printTypeId) !== -1) {
            var index = $scope.selectedItems.indexOf(printTypeId);
            $scope.selectedItems.splice(index, 1)
        }
        else {
            $scope.selectedItems.push(printTypeId);
        }
    };

    $scope.createOrder = () => {
        // $http.post('/api/Checkout/CreateOrder', $scope.selectedItems).then(x => {
        $window.location.href = '/Photo/Cart';
        //});
    };

    $scope.getOrder = () => {
        $http.get('/api/Checkout/GetOrder').then(x => {
            $scope.printTypes = x.data;
            console.log($scope.printTypes);
        });
    };

}])