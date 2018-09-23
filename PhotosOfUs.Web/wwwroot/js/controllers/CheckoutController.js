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

    $scope.showCart = false;

    $scope.cartPreview = () => {       
        if ($scope.showCart == false) {
            $scope.showCart = true;
            $scope.getOpenOrder($scope.user.Id);
            $scope.getPrintTypes();
        }
        else if ($scope.showCart == true) {
            $scope.showCart = false;
        }
    };

    $scope.createOrder = (userId) => {
        var photoId = $location.absUrl().split('Purchase/')[1];
        $http.post('/api/Checkout/CreateOrder/' + userId + '/' + photoId, $scope.selectedItems);
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
            if (x.data != '') {
                $scope.getOrderTotal($scope.order.Id);
            }
        });
    }; 

    $scope.initConfirmation = () => {
        userApi.getUser().then(function (x) {
            $scope.user = x.data;
            $scope.getOpenOrder($scope.user.Id);
        });
    };

    $scope.createPwintyOrder = () => {
        var data = {
            "merchantOrderId": "845",
            "recipientName": "Pwinty Tester",
            "Address1": "123 Test Street",
            "Address2": "TESTING",
            "addressTownOrCity": "TESTING",
            "stateOrCounty": "TESTSHIRE",
            "postalOrZipCode": "TE5 7IN",
            "email": "test@testing.com",
            "countryCode": "gb",
            "preferredShippingMethod": "CHEAPEST",
            "mobileTelephone": "01811 8055"
        };
        $http({
            method: 'POST',
            url: 'https://sandbox.pwinty.com/v2.6/Orders',
            headers: {
                'X-Pwinty-MerchantId': '',
                'X-Pwinty-REST-API-Key': '',
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'crossDomain': true,
            },
            data: data
        }).then(x => {
            $scope.orderId = x.data;
        });
    };

    $scope.createMooOrder = () => {
        var data = {
            'product': 'businesscard',
            "pack": {
                "numCards": 50,
                "productCode": "businesscard",
                "productVersion": 1,
                "sides": [
                ]
            }
        };
        $http.post({
            method: 'moo.pack.createPack',
            url: 'http://www.moo.com/api/service/',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
            },
            data: data
        }).then(x => {
            $scope.orderId = x.data;
        });
    };

    $scope.printQuality = 'Standard';

    $scope.getPwintyCatalog = () => {
        $http({
            method: 'GET',
            url: 'https://sandbox.pwinty.com/v2.6/Catalogue/US/Pro',
        }).then(x => {
            $scope.proProducts = x.data;
            });
        $http({
            method: 'GET',
            url: 'https://sandbox.pwinty.com/v2.6/Catalogue/US/Standard',
        }).then(x => {
            $scope.standardProducts = x.data;
        });
    };

    $scope.addPhotoToPwintyOrder = (orderName, quantity) => {
        $http({
            method: 'POST',
            url: 'https://sandbox.pwinty.com/v2.6/Orders/',///{orderId}/Photos
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'crossDomain': true,
                'X-Pwinty-MerchantId': '',
                'X-Pwinty-REST-API-Key': '',
            },
            data: data
        }).then(x => {
            $scope.orderId = x.data;
        });
    };
}])