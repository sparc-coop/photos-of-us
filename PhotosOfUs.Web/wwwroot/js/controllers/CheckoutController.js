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
        console.log(photoId);
        cartLocalStorage[photoId] = object;

        localStorage.setItem("cart", JSON.stringify(cartLocalStorage));
        console.log(localStorage.getItem("cart"));

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

    $scope.selectAll = function (printTypes) {
        for (var i = 0; i < printTypes.length; i++) {
            $scope.select(printTypes[i].Id);
        }
        
    }

    $scope.isSelected = function (printId) {
        if ($scope.selectedItems.indexOf(printId) !== - 1) {
            return true;
        }
        return false;
    }

    $scope.addToCart = function (printId) {
        $scope.select(printId);
        $scope.createOrder();
        //todo broadcast added to cart to update menu
    }

    $scope.createOrder = () => {
        // $http.post('/api/Checkout/CreateOrder', $scope.selectedItems).then(x => {
        $window.location.href = '/Photo/Cart';
        //});
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

    $scope.getOrder = () => {
        $http.get('/api/Checkout/GetOrder').then(x => {
            $scope.printTypes = x.data;
            console.log($scope.printTypes);
        });
    };

}])