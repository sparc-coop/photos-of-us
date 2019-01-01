namespace PhotosOfUs {
    angular.module('app').directive('cartMini', () => {
        return {
            restrict: 'E',
            templateUrl: 'CartMini.html',
            controller: ['$scope', ($scope) => {
                $scope.showCart = false;

                const localStorageCart = localStorage.getItem('cart');
                $scope.cart = localStorageCart ? JSON.parse(localStorageCart) : [];

                $scope.$on('updatecart', (e, items) => {
                    localStorage.setItem('cart', JSON.stringify(items));
                    $scope.cart = items;
                    $scope.showCart = true;
                });

                $scope.calculatedTotal = () => {
                    if ($scope.cart) {
                        let sum = 0;
                        angular.forEach($scope.cart, (item: IOrderDetail) => sum += item.unitPrice);
                        return sum;
                    }
                    return null;
                };

                $scope.toggleCart = () => $scope.showCart = !$scope.showCart;
            }]
        };
    });
}
