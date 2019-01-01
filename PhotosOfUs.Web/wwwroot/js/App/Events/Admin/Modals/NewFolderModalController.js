var PhotosOfUs;
(function (PhotosOfUs) {
    angular.module('app').controller('NewFolderModalCtrl', ['$scope', '$rootScope', '$mdDialog', 'FolderApiClient', 'folderId', 'folderName',
        ($scope, $rootScope, $mdDialog, FolderApiClient, folderId, folderName) => {
            $scope.close = () => $mdDialog.hide();
            $scope.folderId = folderId;
            $scope.folderName = folderName;
            $scope.renameFolder = (currentFolderId, currentFolderName) => {
                FolderApiClient.put(currentFolderId, currentFolderName)
                    .then(x => {
                    $scope.close();
                    $rootScope.$broadcast('FolderRenamed', x);
                });
            };
            $scope.addFolder = (newFolderName) => {
                FolderApiClient.post(newFolderName)
                    .then(x => {
                    // adds to list view
                    $scope.close();
                    $rootScope.$broadcast('FolderAdded', x);
                });
            };
        }]);
})(PhotosOfUs || (PhotosOfUs = {}));
//# sourceMappingURL=NewFolderModalController.js.map