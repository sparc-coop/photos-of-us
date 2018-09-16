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
