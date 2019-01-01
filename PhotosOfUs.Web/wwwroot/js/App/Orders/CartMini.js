var PhotosOfUs;
(function (PhotosOfUs) {
    angular.module('app').directive('cartMini', () => {
        return {
            restrict: 'E',
            templateUrl: 'CartMini.html',
            controller: ['$scope', '$rootScope', ($scope, $rootScope) => {
                    $scope.showCart = false;
                    const localStorageCart = localStorage.getItem('cart');
                    $rootScope.cart = localStorageCart ? JSON.parse(localStorageCart) : [];
                    $scope.$on('updatecart', (e, items) => {
                        localStorage.setItem('cart', JSON.stringify(items));
                        $rootScope.cart = items;
                        $scope.showCart = true;
                    });
                    $scope.calculatedTotal = () => {
                        if ($rootScope.cart) {
                            let sum = 0;
                            angular.forEach($rootScope.cart, (item) => sum += item.unitPrice);
                            return sum;
                        }
                        return null;
                    };
                    $scope.toggleCart = () => $scope.showCart = !$scope.showCart;
                }]
        };
    });
})(PhotosOfUs || (PhotosOfUs = {}));
//# sourceMappingURL=CartMini.js.map