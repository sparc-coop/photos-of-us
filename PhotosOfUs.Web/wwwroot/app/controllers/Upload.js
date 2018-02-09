'use strict';

// upload
var upload = angular.module('upload', ['angularFileUpload']);

upload.controller('UploadController', ['$scope', 'FileUploader', function ($scope, FileUploader) {
    var uploader = $scope.uploader = new FileUploader({
        url: 'UploadPhotoAsync'
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
    uploader.codeGenerator = function () {
        var fnc = function () {
            return ((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1);
        };

        return fnc() + fnc();
    };

    uploader.addPhotoList = function (fileItem) {
        fileItem.file.photoCode = uploader.codeGenerator();

        var image = new Image();
        image.src = window.URL.createObjectURL(fileItem._file);
        image.onload = function (e) {
            $scope.dimensions = image.naturalWidth + "x" + image.naturalHeight;
            $scope.src = image.src;

            fileItem.file.dimensions = image.naturalWidth + "x" + image.naturalHeight;
            fileItem.file.src = image.src;
            $scope.$apply();
        };
    };

    // FILTERS
    uploader.filters.push({
        name: 'imageFilter',
        fn: function (item /*{File|FileLikeObject}*/, options) {
            var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
            return '|jpg|png|jpeg|bmp|gif|tif'.indexOf(type) !== -1;
        }
    });

    $scope.selectItem = function (item, index) {
        $scope.index = index;
    };

    $scope.removeItem = function (removedItem, index) {
        this.uploader.removeFromQueue(removedItem);

        // if removed and in photo preview, show other
        if (this.uploader.queue.length == index) {
            $scope.index = this.uploader.queue.length - 1;
        }
    };

    // CALLBACKS
    uploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {

    };

    uploader.onAfterAddingFile = function (fileItem) {
        $scope.index = 0;

        uploader.addPhotoList(fileItem);

        // decrease height to drop zone if photo uploaded
        $scope.dropZone = {
            Height: 100
        };
    };

    uploader.onAfterAddingAll = function (addedFileItems) {

    };

    uploader.onBeforeUploadItem = function (item) {

    };

    uploader.onProgressItem = function (fileItem, progress) {

    };

    uploader.onProgressAll = function (progress) {

    };

    uploader.onSuccessItem = function (fileItem, response, status, headers) {

    };

    uploader.onErrorItem = function (fileItem, response, status, headers) {

    };

    uploader.onCancelItem = function (fileItem, response, status, headers) {

    };

    uploader.onCompleteItem = function (fileItem, response, status, headers) {

    };

    uploader.onCompleteAll = function () {

    };
}]);