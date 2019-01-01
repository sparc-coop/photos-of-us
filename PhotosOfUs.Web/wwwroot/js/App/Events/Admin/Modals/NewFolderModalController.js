var PhotosOfUs;
(function (PhotosOfUs) {
    angular.module('app').controller('NewFolderModalCtrl', ['$scope', '$rootScope', '$mdDialog', 'FolderApiClient', 'folderId', 'folderName', 'user',
        ($scope, $rootScope, $mdDialog, FolderApiClient, folderId, folderName, user) => {
            $scope.close = () => $mdDialog.hide();
            $scope.folderId = folderId;
            $scope.folderName = folderName;
            $scope.renameFolder = (currentFolderId, currentFolderName) => {
            };
            $scope.addFolder = (newFolderName) => {
            };
        }]);
})(PhotosOfUs || (PhotosOfUs = {}));
//# sourceMappingURL=NewFolderModalController.js.map