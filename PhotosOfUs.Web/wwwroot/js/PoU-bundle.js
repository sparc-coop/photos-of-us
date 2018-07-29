'use strict';

var app = angular.module('app', ['ngMaterial', 'angularFileUpload', 'monospaced.elastic', '720kb.socialshare', 'angular.filter', 'ui.bootstrap']);





app.factory('photoApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/Photo';
        return {
            getFolders: function () { return $http.get(apiRoot + '/GetFolders'); }
        };
    }
]);


app.factory('folderApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/Folder';
        return {
            add: function (foldername) { return $http.post(apiRoot + '/?name=' + foldername); },
            rename: function (folderId, folderName) { return $http.post(apiRoot + "/RenameFolder", JSON.stringify({ Id: folderId, NewName: folderName}))},
            delete: function (folderId) { return $http.post(apiRoot + "/DeleteFolder/" + folderId); }
        };
    }
]);


app.factory('cardApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/Card';
        return {
            getAll: function () { return $http.get(apiRoot) },
            create: function (quantity) { return $http.post(apiRoot + '/Create/' + quantity) }
        };
    }
]);


app.factory('photographerApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/Photographer';
        return {
            getAccountSettings: function () { return $http.get(apiRoot + '/GetAccountSettings') },
            saveAccountSettings: function (accountSettings) { console.log(JSON.stringify(accountSettings)); return $http.post(apiRoot + '/PostAccountSettings', accountSettings)}
        };
    }
]);

app.factory('checkoutApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/Checkout';
        return {
            createOrder: function (userId, orderItems) { return $http.get(apiRoot + '/CreateOrder/' + userId, orderItems) },
        };
    }
]);

app.factory('userApi', [
    '$http', '$rootScope', function ($http, $rootScope) {
        var apiRoot = '/api/User';
        return {
            getUser: function () { return $http.get(apiRoot + '/GetUser')}
        };
    }
]);


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

        console.log($scope.selectedItems);
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

    //$scope.addToCart = function (printId) {
    //    $scope.select(printId);
    //    $scope.createOrder();
    //    //todo broadcast added to cart to update menu
    //}

    $scope.createOrder = (userId) => {
        console.log($scope.selectedItems);
        var photoId = $location.absUrl().split('Purchase/')[1];
        $http.post('/api/Checkout/CreateOrder/' + userId + '/' + photoId, $scope.selectedItems).then(x => {
            $window.location.href = '/Photo/Cart/' + userId;
        });
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

    $scope.getOrderDetails = (orderId) => {
        $http.get('/api/Photo/GetOrderItems/' + orderId).then(x => {           
            $scope.orderDetails = x.data;
            angular.forEach($scope.orderDetails, function (value, key) {
                $scope.orderDetailsList.push(value);
            });
            console.log($scope.orderDetails);
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
            console.log($scope.order);
            $scope.getOrderTotal($scope.order.Id);
        });
    }; 
}])
app.controller('DownloadCtrl', ['$scope', '$window', '$mdDialog', '$http', 'userApi', ($scope, $window, $mdDialog, $http, userApi) => {

    $scope.getOrders = (userId) => {
        $http.get('/api/Photo/GetOrderPhotos/' + userId).then(x => {
            $scope.orders = x.data;
            console.log(x.data);
            angular.forEach($scope.orders, function (key, value) {
                $scope.getOrderItems(key.Id);
            });           
        });
    };

    $scope.orderItems = [];

    $scope.getOrderItems = (orderId) => {
        $http.get('/api/Photo/GetOrderItems/' + orderId).then(x => {
            $scope.orderItems.push(x.data);          
        });
    };

    $scope.bulkDownload = (userId) => {
        $http.post('/api/Photo/GetForDownload/' + userId);
    };


}])
app.controller('FolderCtrl', ['$scope', '$rootScope', '$window', '$mdDialog', 'photoApi', 'folderApi', ($scope, $rootScope, $window, $mdDialog, photoApi, folderApi) => {

    $scope.close = () => $mdDialog.hide();
    $scope.folders = [];


    $scope.initFolderCtrl = function () {
        $scope.orderByOption = "Name";
        photoApi.getFolders()
            .then(function (x) {
                angular.forEach(x.data, function (f) { $scope.folders.push(f); });
            })
    }

    $scope.initRenameFolderModal = function () {
        $scope.folderId = folderId;
        $scope.folderName = folderName;
    }

    $scope.newFolderModal = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/NewFolderModal',
            controller: 'FolderCtrl',
            clickOutsideToClose: true,
        })
    }

    $scope.renameFolderModal = (folder) => {
        $mdDialog.show({
            templateUrl: '/Photographer/NewFolderModal',
            controller: 'FolderRenameModalCtrl',
            locals: { folderId: folder.Id, folderName: folder.Name },
            clickOutsideToClose: true,
        })
    }

    $scope.addFolder = function (folderName) {
        folderApi.add(folderName)
            .then(function (x) {
                //adds to list view
                $scope.close();
                $rootScope.$broadcast('FolderAdded', x.data);
            })
    }

    $scope.deleteFolder = function (folderId) {
        swal({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.value) {
                folderApi.delete(folderId)
                    .then(data => {
                        $rootScope.$broadcast('FolderRemoved', folderId);
                        swal(
                            'Deleted!',
                            'Your folder has been deleted.',
                            'success'
                        )
                    })
                
            }
        })
    }

    $scope.$on('FolderAdded', function (e, folder) {

        console.log('added folder - ' + JSON.stringify(folder));

        $scope.folders.push(folder);
        
    });

    $scope.$on('FolderRenamed', function (e, folder) {

        console.log('renamed folder - ' + JSON.stringify(folder));
        var index = $scope.folders.findIndex(f => f.Id == folder.Id);
        $scope.folders[index] = folder;

    });

    $scope.$on('FolderRemoved', function (e, folderId) {

        console.log('removed folder - ' + JSON.stringify(folderId));
        var index = $scope.folders.findIndex(f => f.Id == folderId);
        $scope.folders.splice(index,1);

    });
}])

app.controller('FolderRenameModalCtrl', function ($scope, $rootScope, $window, $mdDialog, photoApi, folderApi, folderId, folderName) {

    $scope.close = () => $mdDialog.hide();
    $scope.folderId = folderId;
    $scope.folderName = folderName;
    
   
    $scope.renameFolder = function (folderId,folderName) {
        folderApi.rename(folderId, folderName)
            .then(function (x) {
                $scope.close();
                $rootScope.$broadcast('FolderRenamed', x.data);
            })
    }
    
  
})

app.controller('ModalController', ['$scope', '$window', '$mdDialog', ($scope, $window, $mdDialog) => {
    $scope.close = () => $mdDialog.hide();

}])
app.controller('PaymentCtrl', ['$scope', '$window', '$http', ($scope, $window, $http) => {

    $scope.orderTotal = 0;

    $scope.getOrderDetails = (orderId) => {
        $http.get('/api/Photo/GetOrderItems/' + orderId).then(x => {
            $scope.orderDetails = x.data;
            console.log($scope.orderDetails);
        });
        angular.forEach($scope.orderDetails, function (value, key) {
            $scope.orderTotal + value.UnitPrice;
        }); 
    };

    $scope.getOrderTotal = (orderId) => {
        $http.get('/api/Checkout/GetOrderTotal/' + orderId).then(x => {
            $scope.orderTotal = x.data;
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


    //$scope.printTypes = {};
    //$scope.getPrintTypes = () => {
    //    $http.get('/api/Photo/GetPrintTypes').then(x => {
    //        $scope.printTypes = x.data;
    //        console.log($scope.printTypes);
    //    });
    //};
    //$scope.getPrintTypes();

    //$scope.cartItems = [];
    //var cartLocalStorage = {};
    //if (testLocalStorage()) {
    //    var item = localStorage.getItem("cart");
    //    if (item) {
    //        cartLocalStorage = JSON.parse(item);
    //    } else {
    //        cartLocalStorage = {};
    //    }

    //    console.log(cartLocalStorage);

    //    Object.values(cartLocalStorage).map(x => $scope.cartItems.push(new Photo(x, $http)));
    //    console.log($scope.cartItems);
    //} else {
    //    console.log("local storage unavailable");
    //}
    //function testLocalStorage () {
    //    var available = true;
    //    try {
    //        localStorage.setItem("__availability_test", "test");
    //        localStorage.removeItem("__availability_test");
    //    }
    //    catch (e) {
    //        available = false;
    //    }
    //    finally {
    //        return available;
    //    }
    //}

    //$scope.getAssociatedPrintType = (x) => {
    //    var type = $scope.printTypes[x];
    //    if (!type) return x;
    //    return new PrintType($scope.printTypes[x]);
    //}

    //$scope.sumCart = () => {
    //    var value = $scope.cartItems.reduce((a, b) => (a.price || 0) + (b.price || 0), 0);
    //    console.log("sum", value);
    //    return value;
    //}

    $scope.address = {};
    $scope.saveAddress = (address) => {
        var addressInfo = {
            FullName: address.FirstName + ' ' + address.LastName,
            City: address.City,
            State: address.State,
            ZipCode: address.ZipCode,
            Email: address.Email
        };

        $http.post('/api/Checkout/SaveAddress', addressInfo).then(x => {
            console.log("Address saved");
        });
    };

    //$scope.initStripe = () => {
    //    var stripe = Stripe('pk_test_P8L41KOstSk7oCzeV7mDoRY3');
    //    var elements = stripe.elements();

    //    // Custom styling can be passed to options when creating an Element.
    //    var style = {
    //        base: {
    //            // Add your base input styles here. For example:
    //            fontSize: '16px',
    //            lineHeight: '24px'
    //        }
    //    };

    //    // Create an instance of the card Element
    //    var card = elements.create('card', { style });

    //    // Add an instance of the card Element into the `card-element` <div>
    //    card.mount('#card-element');

    //    card.addEventListener('change', ({ error }) => {
    //        var displayError = document.getElementById('card-errors');
    //        if (error) {
    //            displayError.textContent = error.message;
    //        } else {
    //            displayError.textContent = '';
    //        }
    //    });

    //    var stripeTokenHandler = (token) => {
    //        // Insert the token ID into the form so it gets submitted to the server
    //        var form = document.getElementById('payment-form');
    //        var hiddenInput = document.createElement('input');
    //        hiddenInput.setAttribute('type', 'hidden');
    //        hiddenInput.setAttribute('name', 'stripeToken');
    //        hiddenInput.setAttribute('value', token.id);
    //        form.appendChild(hiddenInput);

    //        var formData = {
    //            StripeToken: token.id,
    //            Amount: $scope.sumCart()
    //        }

    //        // Submit the form
    //        //form.submit();

    //        var apiRoot = "/api/Payment/Charge";
    //        //var apiRoot = "/Photo/Charge";
    //        $http.post(apiRoot, formData);
    //    };

    //    // Create a token or display an error the form is submitted.
    //    var form = document.getElementById('payment-form');
    //    form.addEventListener('submit', event => {
    //        event.preventDefault();

    //        stripe.createToken(card).then(result => {
    //            if (result.error) {
    //                // Inform the user if there was an error
    //                var errorElement = document.getElementById('card-errors');
    //                errorElement.textContent = result.error.message;
    //            } else {
    //                // Send the token to your server
    //                stripeTokenHandler(result.token);
    //            }
    //        });
    //    });

    //};

}])

//function PrintType (data) {
//    this.id;
//    this.type;
//    this.height;
//    this.length;
//    this.icon;

//    function constructor(data) {
//        this.id = data.id;
//        this.type = data.type;
//        this.height = data.height;
//        this.length = data.length;
//        this.icon = data.icon;
//    }
//    constructor.call(this, data);
//}

//function Photo(data, $http) {
//    var self = this;

//    function constructor(data) {
//        this.printTypeId = data.printTypeId;
//        this.getPhotoInfo = getPhotoInfo.bind(this);

//        getPhotoInfo(data.photoId);
//    }
//    constructor.call(this, data);

//    function getPhotoInfo(photoId) {
//        $http.get('/api/Photo/' + photoId).then(x => {
//                console.log(x.data);
//                Object.keys(x.data).map(y => {
//                self[y] = x.data[y];
//            });
//        });
//    }
//}
app.controller('PhotoCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', '$timeout', '$q', 'Socialshare', ($scope, $window, $location, $http, $mdDialog, $timeout, $q, Socialshare) => {
    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };

    $scope.goToPurchase = (photoId) => {
        $window.location.href = '/Photo/Purchase/' + photoId;
    };

    $scope.goToGallery = (folderId) => {
        $window.location.href = '/Photographer/Photos/' + folderId;
    };

    $scope.signInCustomer = (photoId) => {
        $http.get('/Session/SignIn/').then(
            $window.location.href = '/Photo/Purchase/' + photoId
        );
    };

    $scope.openUpload = (folderId) => {
        $mdDialog.show({
            templateUrl: '/Photographer/Upload',
            controller: 'UploadController',
            locals: { folder: folderId },
            clickOutsideToClose: true
        });
    };

    $scope.getPhotoCode = () => {
        $scope.code = $location.absUrl().split('=')[1];
        $scope.getPhotosByCode($scope.code);
    };

    $scope.currentPage = 1;
    $scope.photosPerPage = 8;

    $scope.getPhotosByCode = (code) => {
        $http.get('/api/Photo/GetCodePhotos/' + code).then(x => {
            $scope.codePhotos = x.data;
            console.log($scope.codePhotos);
        });
    };

    $scope.getPhotographer = (id) => {
        $http.get('/api/Photo/GetPhotographer/' + id).then(x => {
            $scope.photographer = x.data;
        });
    };

  
        var self = this;

        self.readonly = false;

        // Lists of fruit names and Vegetable objects
        self.fruitNames = ['Apple', 'Banana', 'Orange'];
        self.ngChangeFruitNames = angular.copy(self.fruitNames);
        self.roFruitNames = angular.copy(self.fruitNames);
        self.editableFruitNames = angular.copy(self.fruitNames);

        self.tags = [];
        self.vegObjs = [
            {
                'name': 'Broccoli',
                'type': 'Brassica'
            },
            {
                'name': 'Cabbage',
                'type': 'Brassica'
            },
            {
                'name': 'Carrot',
                'type': 'Umbelliferous'
            }
        ];

        self.newVeg = function (chip) {
            return {
                name: chip,
                type: 'unknown'
            };
        };

        self.onModelChange = function (newModel) {
            alert('The model has changed');
        };
    $scope.shareFacebook = function (photoId) {
        var url = $location.absUrl().split('?')[0];
        console.log(url);
        Socialshare.share({
            'provider': 'facebook',
            'attrs': {
                'socialshareUrl': url,
                'socialshareHashtags': 'PoU, kuvio, PhotosOfUs'
            }
        });
    }


    $scope.shareTwitter = function (photoId) {
        var url = $location.absUrl().split('?')[0];
        console.log(url);
        Socialshare.share({
            'provider': 'twitter',
            'attrs': {
                'socialshareUrl': url,
                'socialshareText' : "Look at this awesome photo at PoU",
                'socialshareHashtags': 'PoU, kuvio, PhotosOfUs'
            }
        });
    }

    $scope.shareGooglePlus = function (photoId) {
        var url = $location.absUrl().split('?')[0];
        console.log(url);
        Socialshare.share({
            'provider': 'google',
            'attrs': {
                'socialshareUrl': url
            }
        });
    }

    $scope.sharePinterest = function (photoId, photoUrl) {
        var url = $location.absUrl().split('?')[0];
        console.log(url);
        Socialshare.share({
            'provider': 'pinterest',
            'attrs': {
                'socialshareUrl': url,
                'socialshareText': 'Look at this awesome photo at PoU',
                'socialshareMedia': photoUrl
            }
        });
    }

    $scope.shareTumblr = function (photoId, photoUrl) {
        var url = $location.absUrl().split('?')[0];
        console.log(url);
        Socialshare.share({
            'provider': 'tumblr',
            'attrs': {
                'socialshareUrl': url,
                'socialshareText': 'Look at this awesome photo at PoU',
                'socialshareMedia': photoUrl
            }
        });
    }
    
}])
angular.module('app').controller('UploadController', function ($scope, $http, FileUploader, $window, $mdDialog, $filter, folder) {

    $scope.folderId = folder;
    $scope.close = () => $mdDialog.hide();

    var uploader = $scope.uploader = new FileUploader({
        url: '/Photographer/UploadPhotoAsync'
    });

    $scope.$watch(function () {
        return uploader.queue.length;
    }, function (newValue, oldValue) {
        if (newValue < oldValue) {
            // increase height to drop zone if no photo uploaded
            if (uploader.queue.length === 0) {
                $scope.dropZone = {
                    Height: 300
                };
            }
        }
    });

    // HELPERS
    uploader.codeGenerator = function (fileItem) {
        //check if photo has code
        //if it has a code then saves it into currentCode
        //else returns newCode
        fileItem.upload();

        //var fnc = function () {
        //    return ((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1);
        //};

        //return fnc() + fnc();
    };

    $scope.photoNameValidation = function (e, selectedItem) {
        var prohibited = ['drop table', 'alter table', 'create database', 'create table', 'create view', 'delete from', 'drop database', 'drop index',
            'insert into', 'truncate table', 'script>'];

        for (var i = 0; i < prohibited.length; i++) {
            if (e.target.value.indexOf(prohibited[i]) > -1 ||
                e.target.value.indexOf('select') > -1 && e.target.value.indexOf('from') > -1 ||
                e.target.value.indexOf('update') > -1 && e.target.value.indexOf('set') > -1) {
                e.target.value = "Code detected";
                document.getElementById("photo-preview-name").focus();
                document.getElementById("photo-preview-name").blur();
            }
        }
    };

    $scope.photoCodeValidation = function (e, selectedItem) {
        var regex = new RegExp("^[A-Za-z0-9_-]+$");
        if (!regex.test(e.key)) {
            e.preventDefault();
        } else {
            $http.get('VerifyIfCodeAlreadyUsed').
                then(function (response) {
                    if (response.data.photoExisting === true) {
                        angular.element(e.target).addClass('error');
                        angular.element(e.target).prop('title', 'Photo code already used, choose another');
                    } else {
                        angular.element(e.target).removeClass('error');
                        angular.element(e.target).prop('title', '');
                    }
                });
        }
    };

    // FILTERS
    uploader.filters.push({
        name: 'imageFilter',
        fn: function (item /*{File|FileLikeObject}*/, options) {
            var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
            return '|jpg|png|jpeg|bmp|gif|tif'.indexOf(type) !== -1;
        }
    });

    $scope.upload = function (item) {
        var errorsFound = $scope.VerifyErrorsInPhotoCode();

        if (errorsFound === false) {
            item.upload();
        } else {
            alert("Fix the photos with exclamation first before uploading");
        }
    };

    $scope.uploadAll = function (items) {
        console.log("upload all clicked");
       
        var errorsFound = $scope.VerifyErrorsInPhotoCode();

        if (errorsFound === false) {
          
            angular.forEach(items, function (item) {
                //todo only pushes if not photo code
                item.upload();
            });

            $scope.saveAllUpload = true;
            
        } else {
            alert("Fix the photos with exclamation first before uploading");
        }

        
    };

    $scope.VerifyErrorsInPhotoCode = function () {
        var errorsFound = false;
        var els = angular.element(document.querySelector('.code'));
        angular.forEach(els, function (el) {
            if (el.classList.contains("error")) {
                errorsFound = true;
            }
        });

        return errorsFound;
    };

    $scope.selectItem = function (e, i) {
        $scope.selectedItem = i;
    };

    $scope.removeItem = function (removedItem) {
        this.uploader.removeFromQueue(removedItem);

        // if removed and in photo preview, show other
        if (this.uploader.queue.length > 0) {
            $scope.selectedItem = this.uploader.queue[0];
        }
    };

    // CALLBACKS
    uploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {

    };

    uploader.onAfterAddingFile = function (fileItem) {
        // decrease height to drop zone if photo uploaded
        console.log('on after adding file');
        $scope.dropZone = {
            Height: 100
        };

        var extension = fileItem.file.name;
        fileItem.file.fileExtension = extension.split('.').pop();

        uploader.codeGenerator(fileItem);

        var image = new Image();
        image.src = window.URL.createObjectURL(fileItem._file);
        image.onload = function (e) {
            fileItem.file.dimensions = image.naturalWidth + "x" + image.naturalHeight;
            fileItem.file.src = image.src;
            $scope.$apply();
        };

        $scope.selectedItem = fileItem;
    };

    uploader.onAfterAddingAll = function (addedFileItems) {

    };

    uploader.onBeforeUploadItem = function (item) {
        var photoCode = "";
        console.log(item.code);
        if (item.code)
            photoCode = item.code;

        if (item.formData.length > 0) {
            item.formData[0].photoCode = photoCode;
        } else {
            item.formData.push({ photoName: item.file.name, photoCode: photoCode, price: item.file.price, extension: '.' + item.file.fileExtension, folderId: $scope.folderId });
        }
        
    };

    uploader.onProgressItem = function (fileItem, progress) {

    };

    uploader.onProgressAll = function (progress) {

    };

    uploader.onSuccessItem = function (fileItem, response, status, headers) {
        console.log('uploader.onSuccessItem ' + response);
        
        if (response !== "") {
            fileItem.formData[0].photoCode = response;
            fileItem.code = response;
            fileItem.isCode = true;

            var foundItem = $filter('filter')(uploader.queue, { code: response }, true)[0];
            //get the index
            var index = uploader.queue.indexOf(foundItem);
            console.log(index);

            for (var i = (index - 1); i >= 0; i--) {
                if (!uploader.queue[i].isCode) {
                    uploader.queue[i].code = response;
                } else {
                    break;
                }
            }
        }
        
    };

    uploader.onErrorItem = function (fileItem, response, status, headers) {

    };

    uploader.onCancelItem = function (fileItem, response, status, headers) {

    };

    uploader.onCompleteItem = function (fileItem, response, status, headers) {

    };

    uploader.onCompleteAll = () => {
        
        if ($scope.saveAllUpload) {
             $window.location.reload();
        }
        
    };

});
app.controller('CardCtrl', ['$scope', '$rootScope', '$window', '$mdDialog', 'photoApi', 'cardApi', '$timeout', ($scope, $rootScope, $window, $mdDialog, photoApi, cardApi, $timeout) => {
    $scope.close = () => $mdDialog.hide();
    $scope.cards = [];
    $scope.cardsToExport = [];

    $scope.initCardCtrl = function () {
        $scope.cards = [];
        cardApi.getAll()
            .then(function (x) {
                $scope.cards = x.data;
            });
    };

    $scope.exportMultipleCardsModal = function () {
        $mdDialog.show({
            templateUrl: '/Photographer/MultipleCardsModal',
            scope: $scope,
            clickOutsideToClose: true
        })
    };

    $scope.exportMultipleCards = function (quantity) {
        cardApi.create(quantity).then(function(x) {
            console.log(x.data);
            console.log($scope.cards);
            $scope.cards = x.data.concat($scope.cards);
            console.log($scope.cards);
            $mdDialog.hide();
            $scope.downloadCards(x.data);
        });
    };

    $scope.downloadCards = function (cards) {
        $scope.cardsToExport = cards;
        // Use timeout to wait for Angular to finish rendering the hidden inputs for the POST form
        $timeout(function () { $('#card-downloader').submit() });
    };

    $scope.addFolder = function (folderName) {
        folderApi.add(folderName)
            .then(function (x) {
                //adds to list view
                $scope.close();
                $rootScope.$broadcast('FolderAdded', x.data);
            })
    }

    $scope.$on('FolderAdded', function (e, folder) {

        console.log('added folder - ' + JSON.stringify(folder));

        $scope.folders.push(folder);

    });
}])
app.controller('PhotographerCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', 'photographerApi', 'userApi', ($scope, $window, $location, $http, $mdDialog, photographerApi, userApi) => {
    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };
    
    $scope.openUpload = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/UploadProfilePhoto',
            controller: 'ModalController',
            clickOutsideToClose: true,
        });
    };

    $scope.getProfile = function () {      
        photographerApi.getAccountSettings().then(function (x) {
            $scope.photographer = x.data;
        })
    }

    $scope.getUser = () => {
        userApi.getUser().then(function (x) {
            $scope.user = x.data;
        });
    };
}])
app.controller('UploadPhotographerProfileCtrl', ['$scope', '$http', 'FileUploader', '$window', '$mdDialog', function ($scope, $http, FileUploader, $window, $mdDialog) {

    var uploader = $scope.uploader = new FileUploader({
        url: '/Photographer/UploadProfilePhotoAsync'
    });

    $scope.$watch(function () {
        return uploader.queue.length;
    }, function (newValue, oldValue) {
        if (newValue < oldValue) {
            // increase height to drop zone if no photo uploaded
            if (uploader.queue.length === 0) {
                $scope.dropZone = {
                    Height: 300
                };
            }
        }
    });


    $scope.photoNameValidation = function (e, selectedItem) {
        var prohibited = ['drop table', 'alter table', 'create database', 'create table', 'create view', 'delete from', 'drop database', 'drop index',
            'insert into', 'truncate table', 'script>'];

        for (var i = 0; i < prohibited.length; i++) {
            if (e.target.value.indexOf(prohibited[i]) > -1 ||
                e.target.value.indexOf('select') > -1 && e.target.value.indexOf('from') > -1 ||
                e.target.value.indexOf('update') > -1 && e.target.value.indexOf('set') > -1) {
                e.target.value = "Code detected";
                document.getElementById("photo-preview-name").focus();
                document.getElementById("photo-preview-name").blur();
            }
        }
    };


    // FILTERS
    uploader.filters.push({
        name: 'imageFilter',
        fn: function (item /*{File|FileLikeObject}*/, options) {
            var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
            return '|jpg|png|jpeg|bmp|gif|tif'.indexOf(type) !== -1;
        }
    });

    $scope.upload = function (item) {
        item.upload();
    };

    $scope.uploadAll = function (items) {
        console.log("clicked upload");
        angular.forEach(items, function (item) {
            item.upload();
        });
    };

   

    $scope.selectItem = function (e, i) {
        $scope.selectedItem = i;
    };

    $scope.removeItem = function (removedItem) {
        this.uploader.removeFromQueue(removedItem);

        // if removed and in photo preview, show other
        if (this.uploader.queue.length > 0) {
            $scope.selectedItem = this.uploader.queue[0];
        }
    };

    // CALLBACKS
    uploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {

    };

    uploader.onAfterAddingFile = function (fileItem) {
        // decrease height to drop zone if photo uploaded
        console.log('on after adding file');
        $scope.dropZone = {
            Height: 100
        };
        
        var extension = fileItem.file.name;
        fileItem.file.fileExtension = extension.split('.').pop();

        var image = new Image();
        image.src = window.URL.createObjectURL(fileItem._file);
        image.onload = function (e) {
            fileItem.file.dimensions = image.naturalWidth + "x" + image.naturalHeight;
            fileItem.file.src = image.src;
            $scope.$apply();
        };

        $scope.selectedItem = fileItem;
    };

    uploader.onAfterAddingAll = function (addedFileItems) {

    };

    uploader.onBeforeUploadItem = function (item) {
        item.formData.push({ photoName: item.file.name, price: item.file.price, extension: '.' + item.file.fileExtension });
    };

    uploader.onProgressItem = function (fileItem, progress) {

    };

    uploader.onProgressAll = function (progress) {

    };

    //uploader.onSuccessItem = function (fileItem, response, status, headers) {
    //    console.log('uploader.onSuccessItem ' + JSON.stringify(fileItem));
    //};

    uploader.onErrorItem = function (fileItem, response, status, headers) {

    };

    uploader.onCancelItem = function (fileItem, response, status, headers) {

    };

    uploader.onCompleteItem = function (fileItem, response, status, headers) {

    };

    uploader.onCompleteAll = () => {
        //alert("Complete");
        $window.location.reload(); //.location.href = '/Photographer/Dashboard';
    };

}]);
app.controller('PhotographerAccountCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', 'photographerApi', ($scope, $window, $location, $http, $mdDialog, photographerApi) => {
    $scope.originalSettings = {};
    $scope.initAccountSettings = function () {
        photographerApi.getAccountSettings().then(function (x) {
            console.log(x.data);
            $scope.accountSettings = x.data;
            if ($scope.accountSettings.Facebook == null)
                $scope.accountSettings.Facebook = 'https://www.facebook.com/';
            if ($scope.accountSettings.Twitter == null)
                $scope.accountSettings.Twitter = 'https://www.twitter.com/';
            if ($scope.accountSettings.Instagram == null)
                $scope.accountSettings.Instagram = 'https://www.instagram.com/';
            if ($scope.accountSettings.Dribbble == null)
                $scope.accountSettings.Dribbble = 'https://www.dribbble.com/';
            angular.copy(x.data, $scope.originalSettings);
        })
    }

    $scope.discardChanges = function () {
        console.log(JSON.stringify($scope.originalSettings));
        $scope.accountSettings = angular.copy($scope.originalSettings);
    }

    $scope.saveAccountSettings = function (accountSettings) {
        console.log(JSON.stringify(accountSettings));
        $scope.showLoader = true;
        photographerApi.saveAccountSettings(accountSettings).then(function (x) {
            console.log(JSON.stringify(x));
            $scope.showLoader = false;
            swal({
                position: 'top-end',
                type: 'success',
                title: 'Your work has been saved',
                showConfirmButton: false,
                timer: 1500
            });
        }, function (x) {
            console.log(JSON.stringify(x));
            $scope.showLoader = false;
            swal({
                position: 'top-end',
                type: 'error',
                title: 'Oops... Something went wrong!',
                showConfirmButton: false,
                timer: 1500
            });
        });
    }

    $scope.uploadProfileImage = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/UploadProfileImage',
            controller: 'UploadProfileImageCtrl',
            clickOutsideToClose: true,
        });
    };

    $scope.selected = 'details';

    $scope.setSelected = (selected) => {
        $scope.selected = selected;
    };


}])
app.controller('CardCtrl', ['$scope', '$rootScope', '$window', '$mdDialog', 'photoApi', 'cardApi', '$timeout', ($scope, $rootScope, $window, $mdDialog, photoApi, cardApi, $timeout) => {
    $scope.close = () => $mdDialog.hide();
    $scope.cards = [];
    $scope.cardsToExport = [];

    $scope.initCardCtrl = function () {
        $scope.cards = [];
        cardApi.getAll()
            .then(function (x) {
                $scope.cards = x.data;
            });
    };

    $scope.exportMultipleCardsModal = function () {
        $mdDialog.show({
            templateUrl: '/Photographer/MultipleCardsModal',
            scope: $scope,
            clickOutsideToClose: true
        })
    };

    $scope.exportMultipleCards = function (quantity) {
        cardApi.create(quantity).then(function(x) {
            console.log(x.data);
            console.log($scope.cards);
            $scope.cards = x.data.concat($scope.cards);
            console.log($scope.cards);
            $mdDialog.hide();
            $scope.downloadCards(x.data);
        });
    };

    $scope.downloadCards = function (cards) {
        $scope.cardsToExport = cards;
        // Use timeout to wait for Angular to finish rendering the hidden inputs for the POST form
        $timeout(function () { $('#card-downloader').submit() });
    };

    $scope.addFolder = function (folderName) {
        folderApi.add(folderName)
            .then(function (x) {
                //adds to list view
                $scope.close();
                $rootScope.$broadcast('FolderAdded', x.data);
            })
    }

    $scope.$on('FolderAdded', function (e, folder) {

        console.log('added folder - ' + JSON.stringify(folder));

        $scope.folders.push(folder);

    });
}])
app.controller('SalesHistoryCtrl', ['$scope', '$window', '$location', '$http', ($scope, $window, $location, $http) => {

    $scope.query = "";

    $scope.querySalesHistory = (query) => {
        $('.sales-container .overlay').addClass('loading');
        $http.get('/api/Photographer/SalesHistory?query=' + query).then(x => {
            if (x.status === 200) {
                $('.sales-content').html(x.data);
                $('.sales-container .overlay').removeClass('loading');
            }
        });
    };
}]);
app.controller('PhotoTagCtrl', TagCtrl);

    function TagCtrl($timeout, $q) {
        var self = this;

        self.tags = [
            {
                'tag_title': 'Travel'
            }
        ];

        self.newTag = function (chip) {
            return {
                tag_title: chip
            };
        };
    }
//})();

app.controller('UploadProfileImageCtrl', ['$scope', '$http', 'FileUploader', '$window', '$mdDialog', function ($scope, $http, FileUploader, $window, $mdDialog) {

    $scope.close = () => $mdDialog.hide();

    var uploader = $scope.uploader = new FileUploader({
        url: '/Photographer/UploadProfileImageAsync'
    });

    $scope.$watch(function () {
        return uploader.queue.length;
    }, function (newValue, oldValue) {
        if (newValue < oldValue) {
            // increase height to drop zone if no photo uploaded
            if (uploader.queue.length === 0) {
                $scope.dropZone = {
                    Height: 300
                };
            }
        }
    });


    $scope.photoNameValidation = function (e, selectedItem) {
        var prohibited = ['drop table', 'alter table', 'create database', 'create table', 'create view', 'delete from', 'drop database', 'drop index',
            'insert into', 'truncate table', 'script>'];

        for (var i = 0; i < prohibited.length; i++) {
            if (e.target.value.indexOf(prohibited[i]) > -1 ||
                e.target.value.indexOf('select') > -1 && e.target.value.indexOf('from') > -1 ||
                e.target.value.indexOf('update') > -1 && e.target.value.indexOf('set') > -1) {
                e.target.value = "Code detected";
                document.getElementById("photo-preview-name").focus();
                document.getElementById("photo-preview-name").blur();
            }
        }
    };


    // FILTERS
    uploader.filters.push({
        name: 'imageFilter',
        fn: function (item /*{File|FileLikeObject}*/, options) {
            var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
            return '|jpg|png|jpeg|bmp|gif|tif'.indexOf(type) !== -1;
        }
    });

    $scope.upload = function (item) {
        item.upload();

    };

    $scope.uploadAll = function (items) {
        console.log("clicked upload");
        angular.forEach(items, function (item) {
            item.upload();
        });
    };



    $scope.selectItem = function (e, i) {
        $scope.selectedItem = i;
    };

    $scope.removeItem = function (removedItem) {
        this.uploader.removeFromQueue(removedItem);

        // if removed and in photo preview, show other
        if (this.uploader.queue.length > 0) {
            $scope.selectedItem = this.uploader.queue[0];
        }
    };

    // CALLBACKS
    uploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {

    };

    uploader.onAfterAddingFile = function (fileItem) {
        // decrease height to drop zone if photo uploaded
        console.log('on after adding file');
        $scope.dropZone = {
            Height: 100
        };

        var extension = fileItem.file.name;
        fileItem.file.fileExtension = extension.split('.').pop();

        var image = new Image();
        image.src = window.URL.createObjectURL(fileItem._file);
        image.onload = function (e) {
            fileItem.file.dimensions = image.naturalWidth + "x" + image.naturalHeight;
            fileItem.file.src = image.src;
            $scope.$apply();
        };

        $scope.selectedItem = fileItem;
    };

    uploader.onAfterAddingAll = function (addedFileItems) {

    };

    uploader.onBeforeUploadItem = function (item) {
        item.formData.push({ photoName: item.file.name, extension: '.' + item.file.fileExtension });
    };

    uploader.onProgressItem = function (fileItem, progress) {

    };

    uploader.onProgressAll = function (progress) {

    };

    //uploader.onSuccessItem = function (fileItem, response, status, headers) {
    //    console.log('uploader.onSuccessItem ' + JSON.stringify(fileItem));
    //};

    uploader.onErrorItem = function (fileItem, response, status, headers) {

    };

    uploader.onCancelItem = function (fileItem, response, status, headers) {

    };

    uploader.onCompleteItem = function (fileItem, response, status, headers) {

    };

    uploader.onCompleteAll = () => {
        //alert("Complete");
        $window.location.reload(); //.location.href = '/Photographer/Dashboard'
    };

}]);