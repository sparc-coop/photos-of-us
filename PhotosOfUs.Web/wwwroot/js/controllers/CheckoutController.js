app.controller('CheckoutCtrl', ['$scope', '$window', '$location', '$http', 'userApi', ($scope, $window, $location, $http, userApi) => {
    $scope.goToCart = (userId) => {
        $window.location.href = '/Photo/Cart/' + userId;
    };

    $scope.goToCheckout = (userId) => {
        $window.location.href = '/Photo/Checkout/' + userId;
    };

    $scope.getPrintTypes = () => {
        $http.get('/api/Photo/GetPrintTypes').then(x => {
            $scope.printTypes = x.data;
        });
    };

    $scope.selectedItems = [];

    $scope.select = (printTypeId, quantity) => {
        var photoId = location.pathname.split("/").filter(x => !!x).pop();

        var object = {
            photoId,
            printTypeId
        }
        var cartLocalStorage = {};
        if (testLocalStorage()) {
            var item = localStorage.getItem("cart");
            if (item) {
                cartLocalStorage = JSON.parse(item);
            } else {
                cartLocalStorage = {};
            }
        }
        cartLocalStorage[photoId] = object;

        localStorage.setItem("cart", JSON.stringify(cartLocalStorage));

        if ($scope.selectedItems.length === 0) {
            if (quantity == undefined)
                quantity = 1;
            $scope.selectedItems.push({ printTypeId, quantity });
        }
        else if ($scope.selectedItems.find((x) => x.printTypeId == printTypeId)) {
            var index = $scope.selectedItems.find((x) => x.printTypeId == printTypeId);
            $scope.selectedItems.splice(index, 1)
        }
        else {
            if (quantity == undefined)
                quantity = 1;
            $scope.selectedItems.push({ printTypeId, quantity });
        }

        console.log($scope.selectedItems);
    };

    $scope.selectAll = function (printTypes) {
        for (var i = 0; i < printTypes.length; i++) {
            $scope.select(printTypes[i].Id);
        }
        
    }

    $scope.isSelected = function (printId) {
        if ($scope.selectedItems.find((x) => x.printTypeId == printId))
            return true;

        return false;
    }

    $scope.createOrder = (userId) => {
        console.log($scope.selectedItems);
        var photoId = $location.absUrl().split('Purchase/')[1];
        $http.post('/api/Checkout/CreateOrder/' + userId + '/' + photoId, $scope.selectedItems).then(x => {
            $window.location.href = '/Photo/Cart/' + userId;
        });
    };

    function testLocalStorage () {
        var available = true;
        try {
            localStorage.setItem("__availability_test", "test");
            localStorage.removeItem("__availability_test");
        }
        catch (e) {
            available = false;
        }
        finally {
            return available;
        }
    }

    $scope.orderDetailsList = [];
    $scope.orderTotalList = [];
    $scope.totalSales = 0;
    $scope.totalEarned = 0;

    $scope.getOrderDetails = (orderId) => {
        $http.get('/api/Photo/GetOrderItems/' + orderId).then(x => {           
            $scope.orderDetails = x.data;
            angular.forEach($scope.orderDetails, function (value, key) {
                $scope.orderDetailsList.push(value);
                $scope.totalEarned += value.Photo.Price;
            });
        });
        $scope.getOrderTotal(orderId);
    };

    $scope.getOrderTotal = (orderId) => {
        $http.get('/api/Checkout/GetOrderTotal/' + orderId).then(x => {
            $scope.orderTotal = x.data;
            $scope.orderTotalList.push(
                {
                    id: orderId,
                    total: $scope.orderTotal
                }
            );
            $scope.totalSales += x.data;
        });
    };

    $scope.getUser = () => {
        userApi.getUser().then(function (x) {
            $scope.user = x.data;
        })
    };

    $scope.getOpenOrder = (userId) => {
        $http.get('/api/Checkout/GetOpenOrder/' + userId).then(x => {
            $scope.order = x.data;
            $scope.getOrderTotal($scope.order.Id);
        });
    }; 

    $scope.getUserAndAddress = (userId) => {
        userApi.getUser().then(function (x) {
            $scope.user = x.data;
            $http.get('/api/Checkout/GetAddress/' + $scope.user.Id).then(x => {
                $scope.address = x.data;
                $scope.getOpenOrder($scope.user.Id);
            });
        })
    }; 

    $scope.initConfirmation = () => {
        $scope.getUserAndAddress();
    };
}])