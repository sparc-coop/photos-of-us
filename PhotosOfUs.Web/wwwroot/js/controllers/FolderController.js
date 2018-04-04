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

    $scope.$on('FolderAdded', function (e, folder) {

        console.log('added folder - ' + JSON.stringify(folder));

        $scope.folders.push(folder);
        
    });
}])