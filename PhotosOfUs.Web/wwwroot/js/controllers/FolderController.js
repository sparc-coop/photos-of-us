app.controller('FolderCtrl', ['$scope', '$rootScope', '$window', '$mdDialog', 'userApi', 'folderApi', '$http', ($scope, $rootScope, $window, $mdDialog, userApi, folderApi, $http) => {

    $scope.close = () => $mdDialog.hide();
    $scope.folders = [];


    $scope.initFolderCtrl = function (id) {
        $scope.orderByOption = "Name";
        $http.get('/api/User/GetFolders/' + id).then(function (x) {
            angular.forEach(x.data, function (f) { $scope.folders.push(f); });
        });
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
        $scope.folders.push(folder);
        
    });

    $scope.$on('FolderRenamed', function (e, folder) {
        var index = $scope.folders.findIndex(f => f.Id == folder.Id);
        $scope.folders[index] = folder;

    });

    $scope.$on('FolderRemoved', function (e, folderId) {
        var index = $scope.folders.findIndex(f => f.Id == folderId);
        $scope.folders.splice(index,1);

    });

    $scope.getUser = (id) => {
        $http.get("/api/User/GetOne/" + id).then(x => { $scope.startTour(x.data)});
    };

    $scope.startTour = (user) => {
        if (user.DashboardTour == null) {
            let tour = new Shepherd.Tour({
                defaultStepOptions: {
                    classes: 'shepherd-theme-arrows'
                }
            });

            tour.addStep('dash-center1', {
                title: 'Intro',
                text: "Photos Of Us makes it easy to take photos and share them " +
                    "with your clients.No more exchanging  contact details, writing your email " +
                    "on the back of a napkin, or trying to remember vague requests for specific photos from various guests.",
                classes: 'center',
                buttons: [
                    {
                        text: 'Next',
                        action: tour.next,
                        classes: 'next'
                    }
                ]
            });

            tour.addStep('dash-center2', {
                title: 'Print Your Cards',
                text: "First, you need to make sure you have your code-cards ready. " +
                    "You can design and print these by heading over to photosof.us / cards.",
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

            tour.addStep('dash-center3', {
                title: 'Take Your Photo',
                text: "Snap away! Take photos as you typically would. You can take them " +
                    "one at a time or as a collection, however you prefer to do things!",
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

            tour.addStep('dash-center4', {
                title: 'Photograph Your Code',
                text: "Once you have taken your photo, take a photo of a " +
                    "unique code-card and hand it to one of your clients.",
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

            tour.addStep('dash-center5', {
                title: 'Upload Your Photos',
                text: "Upload your photos to POU. The system will then attach each " +
                    "code-card to every photo you took before it, until it reaches " +
                    "another code-card. It will do this for each and every photo you " +
                    "upload. Your subjects can then use the card you gave them with " +
                    "the same code attached to retrieve their photo and view purchasing options!",
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

            tour.addStep('dash-center6', {
                title: 'Done and Done!',
                text: "That’s it! You now know how to link the photos you take to the " +
                    "codes you hand out. You can use this in any way you want to make " +
                    "the handing over of your information easier for you and your clients.",
                buttons: [
                    {
                        text: 'Back',
                        action: tour.back,
                        classes: 'back'
                    },
                    {
                        text: 'Start Tutorial',
                        action: tour.next,
                        classes: 'next'
                    }
                ]
            });

            tour.addStep('dash-step1', {
                title: 'Create A New Folder',
                text: "Let's get started by creating an Album Folder",
                attachTo: '#step1 right',
                classes: 'arrow--bottom',
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
            tour.start();

            $http.post('/api/User/ViewedDashboard/' + user.Id);
        }
    };
}])
