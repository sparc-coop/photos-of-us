angular.module('app').controller('BulkEditController', function ($scope, $http, $window, $mdDialog, $filter, folder) {

    $scope.close = () => $mdDialog.hide();
    $scope.folder = folder;

    $scope.tags = [];

    $scope.test = () => {
        console.log("-->", $scope.folder);
        $http.get('/api/Photo/GetFolders').then(x => {
            //photo filter by folder or code?
            $scope.folder = x.data.filter(fold => fold.Id === folder)[0];
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

        //$http.put('/api/Photo/' + folder, folder).then(res => console.log(res));
        console.log($scope.folder);
        console.log($scope.tags);
    }

    

});