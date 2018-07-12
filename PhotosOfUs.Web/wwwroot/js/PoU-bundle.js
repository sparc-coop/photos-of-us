'use strict';

var app = angular.module('app', ['ngMaterial', 'angularFileUpload', 'monospaced.elastic', 'ngTagsInput']);





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
            add: function (foldername) { return $http.post(apiRoot+'/?name='+ foldername); },
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
       

        photoApi.getFolders()
            .then(function (x) {
                angular.forEach(x.data, function (f) { $scope.folders.push(f); });
                //$scope.folders = x.data;
                console.log(JSON.stringify(x.data));
            })
    }

    $scope.newFolderModal = () => {
        $mdDialog.show({
            templateUrl: '/Photographer/NewFolderModal',
            controller: 'FolderCtrl',
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

    $scope.renameFolder = function () {

    }

    $scope.$on('FolderAdded', function (e, folder) {

        console.log('added folder - ' + JSON.stringify(folder));

        $scope.folders.push(folder);
        
    });
}])
app.controller('ModalController', ['$scope', '$window', '$mdDialog', ($scope, $window, $mdDialog) => {
    $scope.close = () => $mdDialog.hide();

}])
app.controller('PaymentCtrl', ['$scope', '$window', ($scope, $window) => {

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
app.controller('PhotoCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', ($scope, $window, $location, $http, $mdDialog) => {
    $scope.viewPhoto = (photoId) => {
        $window.location.href = '/Photographer/Photo/' + photoId;
    };

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

    $scope.openBulkEdit = (folder) => {
        $mdDialog.show({
            templateUrl: '/Photographer/BulkEditModal',
            controller: 'BulkEditController',
            locals: { folder },
            clickOutsideToClose: true
        });
    };

    $scope.openPhotoEdit = (folder) => {
        $mdDialog.show({
            templateUrl: '/Photographer/PhotoEditModal',
            controller: 'BulkEditController',
            locals: { folder },
            clickOutsideToClose: true
        });
    };

    $scope.getPhotographer = (id) => {
        $http.get('/api/Photo/GetPhotographer/' + id).then(x => {
            $scope.photographer = x.data;
        });
    };

    
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
        console.log("clicked upload");
        var errorsFound = $scope.VerifyErrorsInPhotoCode();

        if (errorsFound === false) {
            var promises = [];
            angular.forEach(items, function (item) {
                //todo only pushes if not photo code
                promises.push(item.upload());
            });
            $q.all(promises).then(x => { $window.location.reload(); })
            
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
            item.formData.push({ photoName: item.file.name, photoCode: photoCode, extension: '.' + item.file.fileExtension, folderId: $scope.folderId });
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
        
        if (response != "") {
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
        //alert("Complete");
        //$window.location.reload(); //.location.href = '/Photographer/Dashboard';
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
app.controller('PhotographerCtrl', ['$scope', '$window', '$location', '$http', '$mdDialog', 'photographerApi', ($scope, $window, $location, $http, $mdDialog, photographerApi) => {
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
    }
}])
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
        $window.location.reload(); //.location.href = '/Photographer/Dashboard';
    };

}]);
angular.module('app').controller('BulkEditController', function ($scope, $http, $window, $mdDialog, $filter, folder) {

    $scope.close = () => $mdDialog.hide();
    $scope.folderID = folder;

    $scope.tags = [];

    $scope.load = () => {
 
        $http.get('/api/Photo/GetFolders').then(x => {
            //photo filter by folder or code?
            $scope.folder = x.data.filter(fold => fold.Id === $scope.folderID)[0];
            $scope.folderName = $scope.folder.Name;
            $scope.num = $scope.folder.Photos.length;


            //$scope.folder.Photos.forEach(ph => {
            //    $scope.tags.push(ph.Tags);
            //})
            
        });
    }

    

   
    //$scope.tags = ['pretty', 'travel', 'dog']; //placeholder until database updated with tags

    $scope.updatePhotos = () => {

        $scope.folder.Name = $scope.folderName;

        //angular.forEach(values, function (value, key) {
        //    $scope.folder.Photos.Tags.push(tags);
        //});

        $http.post(`/api/Photo/${folder}`, $scope.folder).then(res => {
            console.log("test?", res)
        }).catch(err => console.log(err));

        console.log($scope.folder);
        console.log($scope.tags);
    }

    

});