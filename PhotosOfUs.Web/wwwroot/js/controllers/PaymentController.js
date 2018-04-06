app.controller('PaymentCtrl', ['$scope', '$window', '$http', ($scope, $window, $http) => {
    $scope.cartItems = [];
    var cartLocalStorage = {};
    if (testLocalStorage()) {
        var item = localStorage.getItem("cart");
        if (item) {
            cartLocalStorage = JSON.parse(item);
        } else {
            cartLocalStorage = {};
        }

        console.log(cartLocalStorage);

        Object.values(cartLocalStorage).map(x => $scope.cartItems.push(new Photo(x, $http)));
        console.log($scope.cartItems);
    } else {
        console.log("local storage unavailable");
    }
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
    $scope.printTypes = {};
    $scope.getPrintTypes = () => {
        $http.get('/api/Photo/GetPrintTypes').then(x => {
            $scope.printTypes = x.data;
            console.log($scope.printTypes);
        });
    };
    $scope.getPrintTypes();

    $scope.getAssociatedPrintType = (x) => {
        var type = $scope.printTypes[x];
        if (!type) return x;
        return new PrintType($scope.printTypes[x]);
    }

    $scope.sumCart = () => {
        var value = $scope.cartItems.reduce((a, b) => (a.price || 0) + (b.price || 0), 0);
        console.log("sum", value);
        return value;
    }

    $scope.address = {};
    $scope.saveAddress = (address) => {
        var addressInfo = {
            FullName: address.FirstName + ' ' + address.LastName
        };

        $http.post('/api/Photo/GetPrintTypes', addressInfo).then(x => {
            $scope.printTypes = x.data;
            console.log($scope.printTypes);
        });
    };

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

            var formData = {
                stripeToken: token.id,
                amount: $scope.sumCart()
            }

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

}])

function PrintType (data) {
    this.id;
    this.type;
    this.height;
    this.length;
    this.icon;

    function constructor(data) {
        this.id = data.id;
        this.type = data.type;
        this.height = data.height;
        this.length = data.length;
        this.icon = data.icon;
    }
    constructor.call(this, data);
}

function Photo(data, $http) {
    var self = this;

    function constructor(data) {
        this.printTypeId = data.printTypeId;
        this.getPhotoInfo = getPhotoInfo.bind(this);

        getPhotoInfo(data.photoId);
    }
    constructor.call(this, data);

    function getPhotoInfo(photoId) {
        $http.get('/api/Photo/' + photoId).then(x => {
                console.log(x.data);
                Object.keys(x.data).map(y => {
                self[y] = x.data[y];
            });
        });
    }
}