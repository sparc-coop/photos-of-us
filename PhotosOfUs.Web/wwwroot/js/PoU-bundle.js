'use strict';

var app = angular.module('app', ['ngMaterial', 'angularFileUpload', 'monospaced.elastic', '720kb.socialshare', 'ui.bootstrap', 'ngTagsInput', 'angular.filter']);

app.filter('startFrom', function () {
    return function (data, start) {
        return data.slice(start)
    }
})
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


app.controller('BulkEditModalCtrl', ['$scope', '$window', '$mdDialog', '$http', 'selectedPhotos', ($scope, $window, $mdDialog, $http, selectedPhotos) => {
    $scope.close = () => $mdDialog.hide();
    $scope.selectedPhotos = selectedPhotos;
    $scope.photosviewmodel = { photos: [], tags: [] };

    $scope.deletePhotos = function (photos) {
        $http.post('/api/Photographer/deletePhotos/', photos);
        $window.location.href = '/Photographer/Profile/';
    }

    $scope.getTagsByPhotos = function () {
        console.log($scope.selectedPhotos);
        $http.post('/api/Photographer/GetTagsByPhotos/', $scope.selectedPhotos)
            .then(function (x) {
                $scope.tags = x.data;
                console.log($scope.tags);
            });
    };

    $scope.getPhotoPrice = () => {
        if ($scope.selectedPhotos.length < 2) {
            $http.get('/api/Photographer/GetPhotoPrice/' + $scope.selectedPhotos)
                .then(function (x) {
                    $scope.price = x.data;
                });
        }
    };

    $scope.editPhotos = function (photos, tags, price) {
        photos.forEach(function (item) {
            $scope.photosviewmodel.photos.push(item);
        });
        tags.forEach(function (item) {
            $scope.photosviewmodel.tags.push(item);
        });

        if (price != null && $scope.selectedPhotos.length < 2) {
            $http.post('/api/Photographer/SavePhotoPrice/' + photos + '/' + price + '/');
        }

        $http.post('/api/Photographer/AddTags/', tags)
            .then(function () {
                $http.post('/api/Photographer/EditPhotos/', $scope.photosviewmodel)
                    .then(function (x) {
                        $scope.tags = x.data;

                        console.log($scope.tags);
                        $window.location.reload();
                    });
                });
    };

}])
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
        console.log($scope.selectedItems);
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

    $scope.getUserAndAddress = (userId) => {
        userApi.getUser().then(function (x) {
            $scope.user = x.data;
            $http.get('/api/Checkout/GetAddress/' + $scope.user.Id).then(x => {
                $scope.address = x.data;
                $scope.getOpenOrder($scope.user.Id);
            });
        })
    }; 

    $scope.initConfirmation = () => {
        $scope.getUserAndAddress();
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
            console.log('return Pwinty');
            console.log(x);
            console.log(x.data);
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
            console.log('return Moo');
            console.log(x);
            console.log(x.data);
        });
    };

    $scope.printQuality = 'Standard';

    $scope.getPwintyCatalog = () => {
        $http({
            method: 'GET',
            url: 'https://sandbox.pwinty.com/v2.6/Catalogue/US/Pro',
        }).then(x => {
            console.log('Pwinty Catalog');
            console.log(x.data);
            $scope.proProducts = x.data;
            });
        $http({
            method: 'GET',
            url: 'https://sandbox.pwinty.com/v2.6/Catalogue/US/Standard',
        }).then(x => {
            console.log('Pwinty Catalog');
            console.log(x.data);
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
            console.log('return Pwinty');
            console.log(x);
            console.log(x.data);
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

        console.log(addressInfo);
        $http.post('/api/Checkout/SaveAddress', addressInfo).then(x => {
            console.log("Address saved");
        });

        $http.post('/api/Checkout/ConfirmationEmail', addressInfo).then(x => {
            $window.location.href = "/Customer/Confirmation";
        });
    };

}])
app.controller('PhotoCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', '$timeout', '$q', 'Socialshare', ($scope, $window, $location, $http, $mdDialog, $timeout, $q, Socialshare) => {
    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };

    $scope.checkFilter = (itemcode) => {
        itemcode = itemcode + "";

        if (itemcode.indexOf($scope.searchCode) >= 0) {
            return true;
        }
        else {
            return false;
        }
    }

    $scope.goToCode = (code) => {
        console.log('the code is ' + code)
        $window.location.href = '/Photographer/PhotoCode?code=' + code;
    }

    $scope.goToPurchase = (photoId) => {
        $window.location.href = '/Photo/Purchase/' + photoId;
    };

    $scope.goToProfile = (photographerId) => {
        $window.location.href = '/Photographer/Profile/' + photographerId;
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

    $scope.openBulkEdit = (code) => {
        $scope.getPhotosByCode(code);
        $scope.selectedPhotos = [];
        angular.forEach($scope.codePhotos, function (item) { $scope.selectedPhotos.push(item.Id) });
        $mdDialog.show({
            locals: { selectedPhotos: $scope.selectedPhotos },
            templateUrl: '/Photographer/BulkEditModal',
            controller: 'BulkEditModalCtrl',
            clickOutsideToClose: true,
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

    $scope.getUser = () => {
        userApi.getUser().then(x => {
            $scope.user = x.data;
        });
    };

  
        //var self = this;

        //self.readonly = false;

        //// Lists of fruit names and Vegetable objects
        //self.fruitNames = ['Apple', 'Banana', 'Orange'];
        //self.ngChangeFruitNames = angular.copy(self.fruitNames);
        //self.roFruitNames = angular.copy(self.fruitNames);
        //self.editableFruitNames = angular.copy(self.fruitNames);

        //self.tags = [];
        //self.vegObjs = [
        //    {
        //        'name': 'Broccoli',
        //        'type': 'Brassica'
        //    },
        //    {
        //        'name': 'Cabbage',
        //        'type': 'Brassica'
        //    },
        //    {
        //        'name': 'Carrot',
        //        'type': 'Umbelliferous'
        //    }
        //];

        //self.newVeg = function (chip) {
        //    return {
        //        name: chip,
        //        type: 'unknown'
        //    };
        //};

        //self.onModelChange = function (newModel) {
        //    alert('The model has changed');
        //};
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
    };

    $scope.pricingOption = 'option2';
    
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
                item.formData[0].photoName = item.file.name

                angular.forEach(item.tags, function (tag) {
                    item.formData[0].tags += " " + tag.text;
                })

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
        console.log(uploader.queue);
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
            item.formData.push({ photoName: item.file.name, photoCode: photoCode, extension: '.' + item.file.fileExtension, folderId: $scope.folderId, tags: "" });
        }
        
    };

    uploader.onProgressItem = function (fileItem, progress) {

    };

    uploader.onProgressAll = function (progress) {

    };

    uploader.onSuccessItem = function (fileItem, response, status, headers) {
        console.log('uploader.onSuccessItem ' + response);
        
        if (response.Code !== "") {
            fileItem.formData[0].photoCode = response.Code;
            fileItem.code = response.Code;
            fileItem.isCode = true;
            fileItem.suggestedTags = response.SuggestedTags;

           

        } else {
            var index = uploader.queue.indexOf(fileItem)

            fileItem.formData[0].photoCode = uploader.queue[index - 1].code;
            fileItem.code = uploader.queue[index - 1].code;
            fileItem.suggestedTags = response.SuggestedTags;
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
    $scope.pageSize = 5;
    $scope.currentPage = 1;
    
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

    $scope.exportMultipleCards = function (quantity) {
        cardApi.create(quantity).then(function (x) {
            console.log(x.data);
            console.log($scope.cards);
            $scope.cards = x.data.concat($scope.cards);
            console.log($scope.cards);
            $mdDialog.hide();
            $scope.downloadCards(x.data);
        });
    };

    $scope.MooModal = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/MooOrderModal',
            scope: $scope,
            clickOutsideToClose: true
        })
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
app.controller('PhotographerCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', '$filter', '$timeout', 'photographerApi', 'userApi', ($scope, $window, $location, $http, $mdDialog, $filter, $timeout, photographerApi, userApi) => {

    $scope.tags = [];
    $scope.allTags = [];
    $scope.isBulkEditEnabled = false;
    $scope.selectedPhotos = [];
    $scope.profilePhotos = [];
    $scope.isNotHighlighted = {
        "border": "3px solid green"
    }
    $scope.isHighlighted = {
        "border": "3px solid blue"
    }

    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };

    $scope.isPhotoSelected = function (photoId) {
        var idx = $scope.selectedPhotos.indexOf(photoId);
        if (idx > -1) {
            return true;
        }
        else {
            return false;
        }
    }

    $scope.openUpload = () => {
        $mdDialog.show({
            locals: { selectedPhotos: $scope.selectedPhotos },
            templateUrl: '/Photographer/UploadProfilePhoto',
            controller: 'ModalController',
            clickOutsideToClose: true,
        });
    };

    $scope.openBulkEdit = () => {
        $mdDialog.show({
            locals: { selectedPhotos: $scope.selectedPhotos },
            templateUrl: '/Photographer/BulkEditModal',
            controller: 'BulkEditModalCtrl',
            clickOutsideToClose: true,
        });
    };

    $scope.getProfile = function () {
        photographerApi.getAccountSettings().then(function (x) {
            $scope.photographer = x.data;
        })
    }

    $scope.toggleSelection = function () {
        $scope.isBulkEditEnabled = !$scope.isBulkEditEnabled;
    }

    $scope.selectPhoto = function (photoId) {
        var idx = $scope.selectedPhotos.indexOf(photoId);
        if (idx > -1) {
            $scope.selectedPhotos.splice(idx, 1);
        }
        else {
            $scope.selectedPhotos.push(photoId);
        }
    }

    $scope.deletePhotos = function (photos) {
        $http.post('/api/Photographer/deletePhotos/', photos);
    }

    //$scope.getPhotographer = (id) => {
    //    $http.get('/api/Photo/GetPhotographer/' + id).then(x => {
    //        $scope.photographer = x.data;
    //    });
    //};

    $scope.getAllTags = function () {
        $http.get('/api/Photo/GetAllTags/')
            .then(function (x) {
                angular.forEach(x.data, function (f) { $scope.allTags.push(f); });
                console.log(JSON.stringify(x.data));
            });
    };

    $scope.loadTags = function (query) {
        return $timeout(function () {
            return $filter('filter')($scope.allTags, query);
        });
    };

    $scope.getSearchString = function (searchterms) {

        var string = "";

        searchterms.forEach(function (element, index) {
            string += "+" + element.text;
        });

        return string;
    };

    $scope.searchPhotos = (searchterms) => {
        $window.location.href = '/Photographer/Results?tagnames=Image' + $scope.getSearchString(searchterms);
    };

    $scope.getProfilePhotos = function () {
        $http.get('/api/Photographer/getProfilePhotos/')
            .then(function (x) {
                angular.forEach(x.data, function (f) { $scope.profilePhotos.push(f); });
            });
    }

    $scope.arrangePhotos = function () {
        var grid = document.querySelector('.grid');
        var msnry;

        imagesLoaded(grid, function () {
            // init Isotope after all images have loaded
            msnry = new Masonry(grid, {
                itemSelector: '.grid-item',
                columnWidth: '.grid-sizer',
                percentPosition: true
            });
        });
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
            item.formData[0].photoName = item.file.name

            angular.forEach(item.tags, function (tag) {
                item.formData[0].tags += " " + tag.text;
            })

            item.upload();
        });
        $scope.saveAllUpload = true;
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

        fileItem.upload();

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


        item.formData.push({ photoName: item.file.name, price: item.file.price, extension: '.' + item.file.fileExtension, tags: "" });
    };

    uploader.onProgressItem = function (fileItem, progress) {

    };

    uploader.onProgressAll = function (progress) {

    };

    uploader.onSuccessItem = function (fileItem, response, status, headers) {
        //console.log('uploader.onSuccessItem ' + JSON.stringify(fileItem));
        fileItem.suggestedTags = response.SuggestedTags;
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

    $scope.close = () => $mdDialog.hide();

    $scope.deactivateModal = (option, user) => {
        if (option == 'true') {
            $mdDialog.show({
                templateUrl: '/Photographer/DeactivateModal',
                controller: 'PhotographerAccountStatusCtrl',
                user: user,
                clickOutsideToClose: true,
            });
        }
        else if(option == 'false'){
            $scope.reactivateAccount(user.Id);
        }
    }


    $scope.reactivateAccount = (userId) => {
        $http.post('/api/User/Reactivate/' + userId).then(
            $window.location.reload()
        );
    }

    $scope.selected = 'details';

    $scope.setSelected = (selected) => {
        $scope.selected = selected;
    };


}])
.controller('PhotographerAccountStatusCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', 'user', ($scope, $window, $location, $http, $mdDialog, user) => {
    $scope.user = user;

    $scope.deactivateAccount = () => {
        console.log($scope.user.Id);
        $http.post('/api/User/Deactivate/' + $scope.user.Id).then(
            $window.location.reload()
        );
    }
}]);


app.controller('CardCtrl', ['$scope', '$rootScope', '$window', '$mdDialog', 'photoApi', 'cardApi', '$timeout', ($scope, $rootScope, $window, $mdDialog, photoApi, cardApi, $timeout) => {
    $scope.close = () => $mdDialog.hide();
    $scope.cards = [];
    $scope.cardsToExport = [];
    $scope.pageSize = 5;
    $scope.currentPage = 1;
    
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

    $scope.exportMultipleCards = function (quantity) {
        cardApi.create(quantity).then(function (x) {
            console.log(x.data);
            console.log($scope.cards);
            $scope.cards = x.data.concat($scope.cards);
            console.log($scope.cards);
            $mdDialog.hide();
            $scope.downloadCards(x.data);
        });
    };

    $scope.MooModal = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/MooOrderModal',
            scope: $scope,
            clickOutsideToClose: true
        })
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
app.controller('RandomPhotoCtrl', ['$scope', '$window', '$location', '$http', ($scope, $window, $location, $http) => {

    function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    }

    //let area = document.getElementsByClassName("main-landing")[0];
    $scope.getRandomPosition = (element) => {
        var x = 800 - element.clientHeight;
        var y = document.body.offsetWidth - element.clientWidth;
        //var randomX = Math.floor(Math.random() * x);
        //var randomY = Math.floor(Math.random() * y);
        let coords = [getRandomInt(0, x), getRandomInt(650, 900)];
        console.log(coords);
        return coords;
    }

    //$scope.loadImages = (ph) => {
    //    var img = document.createElement('img');
    //    img.setAttribute("src", ph.Url);
    //    img.setAttribute("style", "position:absolute; border-radius: 20px; max-width: 400px; max-height: 400px; margin: 50px;");
    //    var xy = $scope.getRandomPosition(img);
    //    img.style.top = xy[0] + 'px';
    //    img.style.left = xy[1] + 'px';

    //    var element = document.getElementById("main-landing");
    //    element.appendChild(img);
    //};

    $scope.photoList = [];

    $scope.getRandomPhoto = () => {
        $http.get('/api/Photo/GetPublicIds').then(x => {
            $scope.randomize(x.data);
        });
    }; 

    $scope.randomize = (photos) => {        
        let counter = 0;
        let interval = setInterval(function () {
            counter += 1;
            if (counter === 15) {
                clearInterval(interval);
            }
            let idList = photos;

            var id = idList[getRandomInt(0, photos.length - 1)];
            $http.get('/api/Photo/' + id).then(x => {
                if ($scope.photoList['Id'] != id) {
                    $scope.photoList.push(x.data);
                }
            });

        }, 0);
    };

}]);