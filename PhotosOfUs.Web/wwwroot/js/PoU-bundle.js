'use strict';

var app = angular.module('app', ['ngMaterial', 'angularFileUpload', 'monospaced.elastic', '720kb.socialshare', 'ui.bootstrap', 'ngTagsInput']);

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
        $http.post('/api/Photographer/GetTagsByPhotos/', $scope.selectedPhotos)
            .then(function (x) {
                $scope.tags = x.data;
                
                console.log($scope.tags);
            });
    };

    $scope.editPhotos = function (photos, tags) {
        photos.forEach(function (item) {
            $scope.photosviewmodel.photos.push(item);
        });
        tags.forEach(function (item) {
            $scope.photosviewmodel.tags.push(item);
        });
        $http.post('/api/Photographer/AddTags/', tags)
            .then(function () {
                $http.post('/api/Photographer/EditPhotos/', $scope.photosviewmodel)
                    .then(function (x) {
                        $scope.tags = x.data;

                        console.log($scope.tags);
                        $window.location.href = '/Photographer/Profile/';
                    });
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

    $scope.getPrintTypes = () => {
        $http.get('/api/Photo/GetPrintTypes').then(x => {
            $scope.printTypes = x.data;
            console.log($scope.printTypes);
        });
    };

    $scope.selectedItems = [];

    $scope.select = (printTypeId) => {
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
        console.log(photoId);
        cartLocalStorage[photoId] = object;

        localStorage.setItem("cart", JSON.stringify(cartLocalStorage));
        console.log(localStorage.getItem("cart"));

        if ($scope.selectedItems.length === 0) {
            $scope.selectedItems.push(printTypeId);
        }
        else if ($scope.selectedItems.indexOf(printTypeId) !== -1) {
            var index = $scope.selectedItems.indexOf(printTypeId);
            $scope.selectedItems.splice(index, 1)
        }
        else {
            $scope.selectedItems.push(printTypeId);
        }
    };

    $scope.selectAll = function (printTypes) {
        for (var i = 0; i < printTypes.length; i++) {
            $scope.select(printTypes[i].Id);
        }
        
    }

    $scope.isSelected = function (printId) {
        if ($scope.selectedItems.indexOf(printId) !== - 1) {
            return true;
        }
        return false;
    }

    $scope.addToCart = function (printId) {
        $scope.select(printId);
        $scope.createOrder();
        //todo broadcast added to cart to update menu
    }

    $scope.createOrder = () => {
        // $http.post('/api/Checkout/CreateOrder', $scope.selectedItems).then(x => {
        $window.location.href = '/Photo/Cart';
        //});
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

    $scope.getOrder = () => {
        $http.get('/api/Checkout/GetOrder').then(x => {
            $scope.printTypes = x.data;
            console.log($scope.printTypes);
        });
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
                console.log(JSON.stringify(x.data));
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
        console.log('findIndex ' + index);
        $scope.folders[index] = folder;

    });

    $scope.$on('FolderRemoved', function (e, folderId) {

        console.log('removed folder - ' + JSON.stringify(folderId));
        var index = $scope.folders.findIndex(f => f.Id == folderId);
        console.log('findIndex ' + index);
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
                StripeToken: token.id,
                Amount: $scope.sumCart()
            }

            // Submit the form
            //form.submit();

            var apiRoot = "/api/Payment/Charge";
            //var apiRoot = "/Photo/Charge";
            $http.post(apiRoot, formData);
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

    $scope.goToPurchase = (photoId) => {
        $window.location.href = '/Photo/Purchase/' + photoId;
    };

    $scope.goToGallery = (folderId) => {
        $window.location.href = '/Photographer/Photos/' + folderId;
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
        console.log(fileItem);
        console.log(uploader.queue);
        
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
app.controller('PhotographerCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', 'photographerApi', ($scope, $window, $location, $http, $mdDialog, photographerApi) => {

    $scope.tags = [];
    $scope.loadedtags = [];
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

    $scope.isPhotoSelected = function (photo) {
        var idx = $scope.selectedPhotos.indexOf(photo);
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

    $scope.selectPhoto = function (item) {
            var idx = $scope.selectedPhotos.indexOf(item);
            if (idx > -1) {
                $scope.selectedPhotos.splice(idx, 1);
            }
            else {
                $scope.selectedPhotos.push(item);
            }
            console.log($scope.selectedPhotos);
    }

    $scope.deletePhotos = function (photos) {
        $http.post('/api/Photographer/deletePhotos/', photos);
    }

    //$scope.getPhotographer = (id) => {
    //    $http.get('/api/Photo/GetPhotographer/' + id).then(x => {
    //        $scope.photographer = x.data;
    //    });
    //};

    $scope.loadTags = function () {
        $http.get('/api/Photo/GetAllTags/')
            .then(function (x) {
                angular.forEach(x.data, function (f) { $scope.loadedtags.push(f); });
                console.log(JSON.stringify(x.data));
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
                console.log(JSON.stringify(x.data));
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
        $window.location.reload(); //.location.href = '/Photographer/Dashboard';
    };

}]);
app.controller('PhotographerAccountCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', 'photographerApi', ($scope, $window, $location, $http, $mdDialog, photographerApi) => {
    $scope.originalSettings = {};
    $scope.initAccountSettings = function () {
        photographerApi.getAccountSettings().then(function (x) {
            console.log(JSON.stringify(x));
            $scope.accountSettings = x.data;
            angular.copy(x.data, $scope.originalSettings);
            console.log(JSON.stringify($scope.originalSettings));
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


}])
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