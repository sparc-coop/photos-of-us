declare var Stripe;

namespace PhotosOfUs {
    angular.module('app').controller('CheckoutCtrl', () => {
        const stripe = Stripe('');
        const elements = stripe.elements();

        // Custom styling can be passed to options when creating an Element.
        const style = {
            base: {
                // Add your base input styles here. For example:
                fontSize: '16px',
                lineHeight: '24px'
            }
        };

        // Create an instance of the card Element
        const card = elements.create('card', {
            style
        });

        // Add an instance of the card Element into the `card-element` <div>
        card.mount('#card-element');

        card.addEventListener('change', ({ error }) => {
            const displayError = document.getElementById('card-errors');
            if (error) {
                displayError.textContent = error.message;
            } else {
                displayError.textContent = '';
            }
        });

        const stripeTokenHandler = (token) => {
            // Insert the token ID into the form so it gets submitted to the server
            const paymentForm = <HTMLFormElement>document.getElementById('payment-form');
            const hiddenInput = document.createElement('input');
            hiddenInput.setAttribute('type', 'hidden');
            hiddenInput.setAttribute('name', 'stripeToken');
            hiddenInput.setAttribute('value', token.id);
            paymentForm.appendChild(hiddenInput);

            // Submit the form
            paymentForm.submit();
        };

        // Create a token or display an error the form is submitted.
        const form = document.getElementById('payment-form');
        form.addEventListener('submit', event => {
            event.preventDefault();

            stripe.createToken(card).then(result => {
                if (result.error) {
                    // Inform the user if there was an error
                    const errorElement = document.getElementById('card-errors');
                    errorElement.textContent = result.error.message;
                } else {
                    // Send the token to your server
                    stripeTokenHandler(result.token);
                }
            });
        });
    });
}
