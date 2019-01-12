namespace PhotosOfUs {
    angular.module('app').controller('UploadModalCtrl',
    ['$scope', '$http', 'FileUploader', '$window', '$mdDialog', 'eventId', 'UserApiClient',
    ($scope, $http, FileUploader, $window, $mdDialog, eventId, UserApiClient: UserApiClient) => {

        $scope.eventId = eventId;
        $scope.close = () => $mdDialog.hide();

        const uploader = $scope.uploader = new FileUploader({
            url: 'api/Event/' + eventId + '/Photos'
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
        uploader.codeGenerator = fileItem => fileItem.upload();

        $scope.photoNameValidation = (e) => {
            const prohibited = ['drop table', 'alter table', 'create database', 'create table', 'create view',
                                'delete from', 'drop database', 'drop index',
                                'insert into', 'truncate table', 'script>'];

            for (let i = 0; i < prohibited.length; i++) {
                if (e.target.value.indexOf(prohibited[i]) > -1 ||
                    e.target.value.indexOf('select') > -1 && e.target.value.indexOf('from') > -1 ||
                    e.target.value.indexOf('update') > -1 && e.target.value.indexOf('set') > -1) {
                    e.target.value = 'Code detected';
                    document.getElementById('photo-preview-name').focus();
                    document.getElementById('photo-preview-name').blur();
                }
            }
        };

        $scope.photoCodeValidation = e => {
            const regex = new RegExp('^[A-Za-z0-9_-]+$');
            if (!regex.test(e.key)) {
                e.preventDefault();
            }
        };

        // FILTERS
        uploader.filters.push({
            name: 'imageFilter',
            fn: function (item /*{File|FileLikeObject}*/, options) {
                const type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                return '|jpg|png|jpeg|bmp|gif|tif'.indexOf(type) !== -1;
            }
        });

        $scope.upload = (item) => {
            const errorsFound = $scope.VerifyErrorsInPhotoCode();

            if (errorsFound === false) {
                item.upload();
            } else {
                alert('Fix the photos with exclamation first before uploading');
            }
        };

        $scope.uploadAll = (items) => {
            const errorsFound = $scope.VerifyErrorsInPhotoCode();

            if (errorsFound === false) {
                angular.forEach(items, function (item) {
                    item.formData[0].photoName = item.file.name;

                    angular.forEach(item.tags, function (tag) {
                        item.formData[0].tags += ' ' + tag.text;
                    });

                    item.upload();
                });

                $scope.saveAllUpload = true;

            } else {
                alert('Fix the photos with exclamation first before uploading');
            }


        };

        $scope.VerifyErrorsInPhotoCode = () => {
            let errorsFound = false;
            const els = angular.element(document.querySelector('.code'));
            angular.forEach(els, function (el) {
                if (el.classList.contains('error')) {
                    errorsFound = true;
                }
            });

            return errorsFound;
        };

        $scope.selectItem = (e, i) => $scope.selectedItem = i;

        $scope.removeItem = function (removedItem) {
            this.uploader.removeFromQueue(removedItem);

            // if removed and in photo preview, show other
            if (this.uploader.queue.length > 0) {
                $scope.selectedItem = this.uploader.queue[0];
            }
        };

        // CALLBACKS
        uploader.onWhenAddingFileFailed = (item /*{File|FileLikeObject}*/, filter, options) => {
        };

        uploader.onAfterAddingFile = (fileItem) => {
            // decrease height to drop zone if photo uploaded
            $scope.dropZone = {
                Height: 100
            };

            const extension = fileItem.file.name;
            fileItem.file.fileExtension = extension.split('.').pop();

            uploader.codeGenerator(fileItem);

            const image = new Image();
            image.src = window.URL.createObjectURL(fileItem._file);
            image.onload = function (e) {
                fileItem.file.dimensions = image.naturalWidth + 'x' + image.naturalHeight;
                fileItem.file.src = image.src;
                $scope.$apply();
            };

            $scope.selectedItem = fileItem;
        };

        uploader.onAfterAddingAll = function (addedFileItems) {
        };

        uploader.onBeforeUploadItem = function (item) {
            let photoCode = '';
            if (item.code) {
                photoCode = item.code;
            }

            if (item.formData.length > 0) {
                item.formData[0].photoCode = photoCode;
            } else {
                item.formData.push({ photoCode: photoCode, eventId: $scope.eventId });
            }

        };

        uploader.onProgressItem = function (fileItem, progress) {
        };

        uploader.onProgressAll = function (progress) {
        };

        uploader.onSuccessItem = function (fileItem, response, status, headers) {

            if (response.Code !== '') {
                fileItem.formData[0].photoCode = response.Code;
                fileItem.code = response.Code;
                fileItem.isCode = true;
                fileItem.suggestedTags = response.SuggestedTags;
            } else {
                const index = uploader.queue.indexOf(fileItem);
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

        $scope.showTour = () => UserApiClient.get().then(x => $scope.startTour(x));

        $scope.startTour = (user: User) => {
            if (user.photoTour == null) {
                const tour = new Shepherd.Tour({
                    defaultStepOptions: {
                        classes: 'shepherd-theme-arrows'
                    }
                });

                tour.addStep('uploadmodal1', {
                    title: 'Bulk Uploading Photos',
                    text: 'You can Bulk Upload Photos. They will appear in this list.',
                    attachTo: '.upload--container top',
                    classes: 'small',
                    buttons: [
                        {
                            text: 'Next',
                            action: tour.next,
                            classes: 'next'
                        }
                    ]
                });

                tour.addStep('uploadmodal2', {
                    title: 'Photo Codes',
                    text: 'You will notice each photo will recieve a code based on the last code-photo taken',
                    attachTo: '.photo-code top',
                    classes: 'arrow--bottom small',
                    buttons: [
                        {
                            text: 'Back',
                            action: tour.back,
                            classes: 'back'
                        },
                        {
                            text: 'Next',
                            action: tour.next,
                            classes: 'next'
                        }
                    ]
                });

                tour.addStep('uploadmodal3', {
                    title: 'Photo Codes Error',
                    text: 'If for some reason the code could not be auto-applied, you can manually enter the code',
                    attachTo: '.photo-code right',
                    classes: 'arrow--bottom small',
                    buttons: [
                        {
                            text: 'Back',
                            action: tour.back,
                            classes: 'back'
                        },
                        {
                            text: 'Next',
                            action: tour.next,
                            classes: 'next'
                        }
                    ]
                });

                tour.addStep('uploadmodal4', {
                    title: 'Photo Settings',
                    text: 'You can then give your photos a name, price, and tags so it is easier to find.',
                    attachTo: '.photo-preview__name left',
                    classes: 'arrow--bottom small',
                    buttons: [
                        {
                            text: 'Back',
                            action: tour.back,
                            classes: 'back'
                        },
                        {
                            text: 'Next',
                            action: tour.next,
                            classes: 'next'
                        }
                    ]
                });

                tour.addStep('uploadmodal5', {
                    title: 'Make & Save Some Changes',
                    text: 'Why not give it a go. Give your photo a name and a tag, then hit save.',
                    attachTo: '.button--upload right',
                    classes: 'arrow--bottom small',
                    buttons: [
                        {
                            text: 'Back',
                            action: tour.back,
                            classes: 'back'
                        },
                        {
                            text: 'Close Tip',
                            action: tour.next,
                            classes: 'next'
                        }
                    ]
                });

                tour.start();
                UserApiClient.viewedPhotoTour(user.id);
            }
        };

    }]);
}
