﻿app.controller('PaymentCtrl', ['$scope', '$window', '$http', ($scope, $window, $http) => {

    $scope.getOrderDetails = (orderId) => {
        $http.get('/api/Orders/GetOrderDetails/' + orderId).then(x => {
            $scope.orderDetails = x.data;
        });
    };

    $scope.initStripe = () => {
        var stripe = Stripe('');
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


    $scope.address = {};
    $scope.saveAddress = (address) => {
        var addressInfo = {
            FullName: address.FirstName + ' ' + address.LastName,
            Address1: address.Address1,
            City: address.City,
            State: address.State,
            ZipCode: address.ZipCode,
            Email: address.Email,
            Phone: address.Phone
        };

        $http.post('/api/Cart/SaveAddress', addressInfo).then(x => {
        });
    };

}])