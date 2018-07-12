angular.module('app').controller('BulkEditController', function ($scope, $http, $window, $mdDialog, $filter, folder) {

    $scope.close = () => $mdDialog.hide();
    $scope.folderID = folder;

    $scope.tags = [];

    $scope.load = () => {
 
        $http.get('/api/Photo/GetFolders').then(x => {
            //photo filter by folder or code?
            $scope.folder = x.data.filter(fold => fold.Id === $scope.folderID)[0];
            $scope.folderName = $scope.folder.Name;
            $scope.num = $scope.folder.Photos.length;


            //$scope.folder.Photos.forEach(ph => {
            //    $scope.tags.push(ph.Tags);
            //})
            
        });
    }

    

   
    //$scope.tags = ['pretty', 'travel', 'dog']; //placeholder until database updated with tags

    $scope.updatePhotos = () => {

        $scope.folder.Name = $scope.folderName;

        //angular.forEach(values, function (value, key) {
        //    $scope.folder.Photos.Tags.push(tags);
        //});

        $http.post(`/api/Photo/${folder}`, $scope.folder).then(res => {
            console.log("test?", res)
        }).catch(err => console.log(err));

        console.log($scope.folder);
        console.log($scope.tags);
    }

    

});