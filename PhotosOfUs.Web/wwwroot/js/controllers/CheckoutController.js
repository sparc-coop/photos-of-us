app.controller('CheckoutCtrl', ['$scope', '$window', '$location', '$http', 'userApi', '$mdDialog', ($scope, $window, $location, $http, userApi, $mdDialog) => {
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

    $scope.priceModal = (user) => {
        if (user.PurchaseTour == true) {
            return null
        }
        else {
            $mdDialog.show({
                templateUrl: '/Photographer/PriceModal',
                controller: 'ModalController',
                clickOutsideToClose: true
            });
            $http.post('/api/User/ViewedPricing/' + user.Id);
        }
    };

    $scope.showPriceModal = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/PriceModal',
            controller: 'ModalController',
            clickOutsideToClose: true
        });
    };

    $scope.getPhotographer = (id) => {
        $http.get('/api/User/GetOne/' + id).then(x => $scope.photographer = x.data);
    }

    $scope.selectedItems = [];

    $scope.select = (printTypeId, quantity, price) => {
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
            $scope.selectedItems.push({ printTypeId, quantity, price });
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
            $scope.getOpenOrder();
            $scope.getPrintTypes();
        }
        else if ($scope.showCart == true) {
            $scope.showCart = false;
        }
    };

    $scope.createOrder = () => {
        var photoId = $location.absUrl().split('Purchase/')[1];
        $http.post('/api/Orders/' + photoId, $scope.selectedItems);
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
        $http.get('/api/Orders/GetOrderDetails/' + orderId).then(x => {           
            $scope.orderDetails = x.data.OrderDetail;
            angular.forEach($scope.orderDetails, function (value, key) {
                $scope.orderDetailsList.push(value);
                $scope.totalEarned += value.Photo.Price;
            });
        });
        $scope.getOrderTotal(orderId);
    };

    $scope.getSalesHistory = () => {
        $http.get('/api/Orders/GetSalesHistory/').then(x => {
            $scope.salesHistory = x.data;
            angular.forEach($scope.salesHistory, function (value, key) {
                $scope.totalEarned += value.Earnings;
                $scope.totalSales += value.Quantity;
                $scope.totalMade += value.UnitPrice * value.Quantity;
            });
        });
    };

    $scope.getOrderTotal = (orderId) => {
        $http.get('/api/Cart/GetOrderTotal/' + orderId).then(x => {
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
        $http.get('/api/User').then(x => { $scope.user = x.data; $scope.priceModal(x.data);});
    };

    $scope.getOpenOrder = () => {
        $http.get('/api/Cart').then(x => {
            $scope.order = x.data;
            if (x.data != '') {
                $scope.getOrderTotal($scope.order.Id);
            }
        });
    }; 

    $scope.initConfirmation = () => {
        userApi.getUser().then(function (x) {
            $scope.user = x.data;
            $scope.getOpenOrder();
        });
    };

    $scope.createPwintyOrder = () => {
        console.log($scope.selectedItems);
        var data = {
            "merchantOrderId": "848",
            "recipientName": $scope.user.DisplayName,
            "Address1": "123 Test Street",
            "Address2": "TESTING",
            "addressTownOrCity": "TESTING",
            "stateOrCounty": "TESTSHIRE",
            "postalOrZipCode": "TE5 7IN",
            "email": $scope.user.Email,
            "countryCode": "US",
            "preferredShippingMethod": "Budget",
            "mobileTelephone": "01811 8055"
        };
        $http({
            method: 'POST',
            url: 'https://sandbox.pwinty.com/v2.6/orders',
            headers: {
                'X-Pwinty-MerchantId': '',
                'X-Pwinty-REST-API-Key': 'test_',
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'crossDomain': true
            },
            data: data
        }).then(x => {
            console.log(x.data);
            $scope.order = x.data;
            $scope.addPhotoToPwintyOrder($scope.order.id);
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
            url: 'https://www.moo.com/api/service/',
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


    //Pwinty API documentation https://www.pwinty.com/api/2.6/
    $scope.getPwintyCatalog = () => {
        $http({
            method: 'GET',
            url: 'https://sandbox.pwinty.com/v2.6/Catalogue/US/Pro'
        }).then(x => {
            $scope.proProducts = x.data;
            console.log("Catalog");
            console.log(x);
            });
        $http({
            method: 'GET',
            url: 'https://sandbox.pwinty.com/v2.6/Catalogue/US/Standard'
        }).then(x => {
            $scope.standardProducts = x.data;
        });
    };

    $scope.addPhotoToPwintyOrder = (orderId) => {

        angular.forEach($scope.selectedItems, item => {
            $http({
                method: 'POST',
                url: 'https://sandbox.pwinty.com/v2.6/orders/' + orderId + '/photos',
                headers: {
                    'X-Pwinty-MerchantId': '',
                    'X-Pwinty-REST-API-Key': 'test_',
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'crossDomain': true
                },
                data: {
                    "orderId": orderId,
                    "url": "https://photosofus.blob.core.windows.net/watermark/2/42/DSC_023820180626134405.JPG",
                    "copies": item.quantity,
                    "type": item.printTypeId,
                    "sizing": "Crop",
                    "priceToUser": item.price,
                }
            });
        });     
    };
}])