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