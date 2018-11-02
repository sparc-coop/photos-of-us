app.controller('FolderCtrl', ['$scope', '$rootScope', '$window', '$mdDialog', 'userApi', 'folderApi', '$http', ($scope, $rootScope, $window, $mdDialog, userApi, folderApi, $http) => {

    $scope.close = () => $mdDialog.hide();
    $scope.folders = [];


    $scope.initFolderCtrl = function (id) {
        $scope.orderByOption = "Name";
        $http.get('/api/User/GetFolders/' + id).then(function (x) {
            angular.forEach(x.data, function (f) { $scope.folders.push(f); });
        });
        console.log($scope.folders);
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

    let tour = new Shepherd.Tour({
        defaultStepOptions: {
            classes: 'shepherd-theme-arrows'
        }
    });

    tour.addStep('dash-center1', {
        title: 'Getting Started',
        text: "Photos Of Us makes it easy ",
        buttons: [
            {
                text: 'Next',
                action: tour.next,
                classes: 'next'
            }
        ]
    });

    tour.addStep('dash-center2', {
        title: 'Getting Started 2',
        text: "Let's get started by creating an Album Folder",
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

    tour.addStep('dash-step1', {
        title: 'Create A New Folder',
        text: "Let's get started by creating an Album Folder",
        attachTo: '#step1 right',
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

    //tour.addStep('dash-step2', {
    //    title: 'Folder Name',
    //    text: "Give the folder a name",
    //    attachTo: '.header-large bottom',
    //    buttons: [
    //        {
    //            text: 'Back',
    //            action: tour.back,
    //            classes: 'back'
    //        },
    //        {
    //            text: 'Next',
    //            action: tour.next
    //        }
    //    ]
    //});

    tour.start();
}])
