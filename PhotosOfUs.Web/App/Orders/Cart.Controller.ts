namespace PhotosOfUs {
    angular.module('app')
    .controller('CartCtrl', ['$scope', '$rootScope', '$http', '$mdDialog', ($scope, $rootScope, $http, $mdDialog) => {
        $scope.showPurchaseTour = (user) => {
            if (user.PurchaseTour === true) {
                return null;
            } else {
                $scope.showPriceModal();
                $http.post('/api/User/ViewedPricing/' + user.Id);
            }
        };

        $scope.showPriceModal = () => {
            $mdDialog.show({
                templateUrl: '/Admin/Modals/PriceModal',
                controller: 'ModalController',
                clickOutsideToClose: true
            });
        };

        const cart = localStorage.getItem('cart');
        const selectedItems: IOrderDetail[] = cart ? JSON.parse(cart) : [];

        $scope.select = (photoId: number, printName: string, quantity: number, price: number) => {
            if (!quantity) { quantity = 1; }

            if (selectedItems.find(x => x.photoId === photoId && x.printType.type === print.name)) {
                const index = selectedItems.findIndex(x => x.printType.type === print.name);
                selectedItems.splice(index, 1);
            } else {
                selectedItems.push({
                    photoId: photoId,
                    printType: new PrintType({ type: printName, id: null, baseCost: null }),
                    quantity: quantity,
                    id: null,
                    orderId: null,
                    unitPrice: price
                });
            }
        };

        $scope.isSelected = (name: string) => selectedItems.find(x => x.printType.type === name);

        $scope.addToCart = () => {
            $rootScope.$broadcast('updatecart', selectedItems);
        };

        $scope.createPwintyOrder = () => {
            const data = {
                'merchantOrderId': '848',
                'recipientName': $scope.user.DisplayName,
                'Address1': '123 Test Street',
                'Address2': 'TESTING',
                'addressTownOrCity': 'TESTING',
                'stateOrCounty': 'TESTSHIRE',
                'postalOrZipCode': 'TE5 7IN',
                'email': $scope.user.Email,
                'countryCode': 'US',
                'preferredShippingMethod': 'Budget',
                'mobileTelephone': '01811 8055'
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

        $scope.printQuality = 'Standard';

        // Pwinty API documentation https://www.pwinty.com/api/2.6/
        $scope.getPwintyCatalog = () => {
            $http({
                method: 'GET',
                url: 'https://sandbox.pwinty.com/v2.6/Catalogue/US/Pro'
            }).then(x => $scope.proProducts = x.data);
            $http({
                method: 'GET',
                url: 'https://sandbox.pwinty.com/v2.6/Catalogue/US/Standard'
            }).then(x => $scope.standardProducts = x.data);
        };

        $scope.addPhotoToPwintyOrder = (orderId) => {
            angular.forEach(selectedItems, item => {
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
                        'orderId': orderId,
                        'url': 'https://photosofus.blob.core.windows.net/watermark/2/42/DSC_023820180626134405.JPG',
                        'copies': item.quantity,
                        'type': item.printTypeId,
                        'sizing': 'Crop',
                        'priceToUser': item.unitPrice
                    }
                });
            });
        };
    }]);
}
