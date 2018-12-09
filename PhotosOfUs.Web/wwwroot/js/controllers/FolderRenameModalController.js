app.controller('FolderModalCtrl', function ($scope, $rootScope, $mdDialog, folderApi, folderId, folderName, user) {

    $scope.close = () => $mdDialog.hide();
    $scope.folderId = folderId;
    $scope.folderName = folderName;
    
   
    $scope.renameFolder = function (folderId, folderName) {
        folderApi.rename(folderId, folderName, user.Id)
            .then(function (x) {
                $scope.close();
                $rootScope.$broadcast('FolderRenamed', x.data);
            })
    };

    $scope.addFolder = function (folderName) {
        folderApi.add(folderName, user.Id)
            .then(function (x) {
                //adds to list view
                $scope.close();
                $rootScope.$broadcast('FolderAdded', x.data);
            })
    };
})
