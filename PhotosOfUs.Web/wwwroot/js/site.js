'use strict';

var app = angular.module('app', ['upload', 'ngMaterial']);
app.controller('PhotoCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', ($scope, $window, $location, $http, $mdDialog) => {
    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };

    $scope.goToPurchase = (photoId) => {
        $window.location.href = '/Photo/Purchase/' + photoId;
    };

    $scope.openUpload = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/Upload',
            controller: 'ModalController',
            clickOutsideToClose: true,
        });
    };
}])
app.controller('CheckoutCtrl', ['$scope', '$window', '$location', '$http', ($scope, $window, $location, $http) => {
    $scope.goToCart = () => {
        $window.location.href = '/Photo/Cart';
    };

    $scope.goToCheckout = () => {
        $window.location.href = '/Photo/Checkout';
    };
}])
app.controller('ModalController', ['$scope', '$window', '$mdDialog', ($scope, $window, $mdDialog) => {
    $scope.close = () => $mdDialog.hide();

}])
app.controller('PaymentCtrl', ['$scope', '$window', ($scope, $window) => {
    $scope.initStripe = () => {
        var stripe = Stripe('pk_test_P8L41KOstSk7oCzeV7mDoRY3');
        var elements = stripe.elements();

        // Custom styling can be passed to options when creating an Element.
        var style = {
            base: {
                // Add your base input styles here. For example:
                fontSize: '16px',
                lineHeight: '24px'
            }
        };

        // Create an instance of the card Element
        var card = elements.create('card', { style });

        // Add an instance of the card Element into the `card-element` <div>
        card.mount('#card-element');

        card.addEventListener('change', ({ error }) => {
            var displayError = document.getElementById('card-errors');
            if (error) {
                displayError.textContent = error.message;
            } else {
                displayError.textContent = '';
            }
        });

        var stripeTokenHandler = (token) => {
            // Insert the token ID into the form so it gets submitted to the server
            var form = document.getElementById('payment-form');
            var hiddenInput = document.createElement('input');
            hiddenInput.setAttribute('type', 'hidden');
            hiddenInput.setAttribute('name', 'stripeToken');
            hiddenInput.setAttribute('value', token.id);
            form.appendChild(hiddenInput);

            // Submit the form
            form.submit();

            //var apiRoot = "/api/Payment/Charge/";
            //$http.post(apiRoot, token.id);
        };

        // Create a token or display an error the form is submitted.
        var form = document.getElementById('payment-form');
        form.addEventListener('submit', event => {
            event.preventDefault();

            stripe.createToken(card).then(result => {
                if (result.error) {
                    // Inform the user if there was an error
                    var errorElement = document.getElementById('card-errors');
                    errorElement.textContent = result.error.message;
                } else {
                    // Send the token to your server
                    stripeTokenHandler(result.token);
                }
            });
        });

    };

}]);